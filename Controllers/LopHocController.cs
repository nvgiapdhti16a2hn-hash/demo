// Họ và tên: Sinh viên 3 (bổ sung họ tên)
// Mã sinh viên: SV3 (bổ sung mã sinh viên)
// Nội dung thực hiện Module 3: Kiểm tra điều kiện và nộp hồ sơ đăng ký học.
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers;

public class LopHocController(AppDbContext db, CurrentUser currentUser) : AppController(currentUser)
{
    public async Task<IActionResult> Details(int id)
    {
        var item = await db.LopHocs.AsNoTracking()
            .Include(x => x.ChuongTrinhDaoTao)
            .SingleOrDefaultAsync(x => x.MaLop == id);
        if (item is null)
            return NotFound();
        ViewBag.SoLuongDaXepLop = await db.HoSoDangKyHocs.CountAsync(x =>
            x.MaLop == id && x.TrangThai == TrangThaiHoSo.DuocXepLop);
        return View(item);
    }

    public async Task<IActionResult> Index(
        string? tuKhoa,
        int? maChuongTrinh,
        string? trinhDo,
        decimal? diemToiDa,
        TrangThaiLop? trangThai,
        bool? conHan,
        string sapXep = "ten_az",
        int trang = 1)
    {
        const int pageSize = 10;
        var query = db.LopHocs.AsNoTracking().Include(x => x.ChuongTrinhDaoTao).AsQueryable();
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(x => x.TenLop.Contains(keyword) ||
                (x.ChuongTrinhDaoTao != null && x.ChuongTrinhDaoTao.TenChuongTrinhDaoTao.Contains(keyword)));
        }
        if (maChuongTrinh.HasValue)
            query = query.Where(x => x.MaChuongTrinhDaoTao == maChuongTrinh.Value);
        if (!string.IsNullOrWhiteSpace(trinhDo))
            query = query.Where(x => x.TrinhDoDauVao == trinhDo);
        if (diemToiDa.HasValue)
            query = query.Where(x => x.DiemDauVaoToiThieu <= diemToiDa.Value);
        if (trangThai.HasValue)
            query = query.Where(x => x.TrangThai == trangThai.Value);
        if (conHan.HasValue)
            query = query.Where(x => conHan.Value ? x.HanDangKy.Date >= DateTime.Today : x.HanDangKy.Date < DateTime.Today);

        query = sapXep switch
        {
            "ten_za" => query.OrderByDescending(x => x.TenLop),
            "han_tang" => query.OrderBy(x => x.HanDangKy),
            "han_giam" => query.OrderByDescending(x => x.HanDangKy),
            "siso_tang" => query.OrderBy(x => x.SiSoToiDa),
            "siso_giam" => query.OrderByDescending(x => x.SiSoToiDa),
            _ => query.OrderBy(x => x.TenLop)
        };

        var total = await query.CountAsync();
        var pages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        trang = Math.Clamp(trang, 1, pages);
        var model = new CourseListViewModel
        {
            LopHocs = await query.Skip((trang - 1) * pageSize).Take(pageSize).ToListAsync(),
            ChuongTrinhs = await db.ChuongTrinhDaoTaos.AsNoTracking().OrderBy(x => x.TenChuongTrinhDaoTao).ToListAsync(),
            TuKhoa = tuKhoa,
            MaChuongTrinh = maChuongTrinh,
            TrinhDo = trinhDo,
            DiemToiDa = diemToiDa,
            TrangThai = trangThai,
            ConHan = conHan,
            SapXep = sapXep,
            TrangHienTai = trang,
            TongSoTrang = pages
        };
        if (string.Equals(Request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.Ordinal))
            return PartialView("_CourseResults", model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DangKy(int? id)
    {
        if (!RequireRole(nameof(VaiTro.HocVien)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var studentIsActive = await db.HocViens.AnyAsync(x =>
            x.MaTaiKhoan == CurrentUser.AccountId && x.TrangThai);
        if (!studentIsActive)
            return Forbid();

        var classes = await LopDangDuocNhanHoSoAsync();
        var model = new EnrollmentViewModel
        {
            MaLop = id ?? 0,
            LopDangMo = classes.Select(x => new SelectListItem(x.TenLop, x.MaLop.ToString()))
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DangKy(EnrollmentViewModel model)
    {
        if (!RequireRole(nameof(VaiTro.HocVien)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var student = await db.HocViens.SingleOrDefaultAsync(x => x.MaTaiKhoan == CurrentUser.AccountId);
        if (student is null || !student.TrangThai)
            return Forbid();

        var canSubmitProfile = !string.IsNullOrWhiteSpace(student.HoTen) &&
            !string.IsNullOrWhiteSpace(student.Email) &&
            !string.IsNullOrWhiteSpace(student.SoDienThoai);
        if (!canSubmitProfile)
        {
            TempData["Error"] = "Hãy hoàn thiện họ tên, email và số điện thoại trong hồ sơ trước khi đăng ký.";
            return RedirectToAction("Sua", "HocVien");
        }

        if (!ModelState.IsValid)
        {
            await PopulateEligibleClassesAsync(model);
            return View(model);
        }

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var targetClass = await db.LopHocs.SingleOrDefaultAsync(x => x.MaLop == model.MaLop);
        if (!IsAcceptingApplications(targetClass))
        {
            ModelState.AddModelError(nameof(model.MaLop), "Lớp không mở nhận hồ sơ hoặc đã ngoài thời gian đăng ký.");
            await PopulateEligibleClassesAsync(model);
            return View(model);
        }
        var placedCount = await db.HoSoDangKyHocs.CountAsync(x =>
            x.MaLop == model.MaLop && x.TrangThai == TrangThaiHoSo.DuocXepLop);
        if (targetClass is null || placedCount >= targetClass.SiSoToiDa)
        {
            ModelState.AddModelError(nameof(model.MaLop), "Lớp đã đủ sĩ số tối đa và không nhận hồ sơ mới.");
            await PopulateEligibleClassesAsync(model);
            return View(model);
        }

        var inProgress = new[]
        {
            TrangThaiHoSo.ChoDuyet, TrangThaiHoSo.DuDieuKien,
            TrangThaiHoSo.ChoKiemTraDauVao, TrangThaiHoSo.DaKiemTraDauVao
        };
        if (await db.HoSoDangKyHocs.AnyAsync(x =>
                x.MaHocVien == student.MaHocVien && x.MaLop == model.MaLop && inProgress.Contains(x.TrangThai)))
        {
            ModelState.AddModelError(string.Empty, "Bạn đã có hồ sơ đang xử lý cho lớp này.");
            await PopulateEligibleClassesAsync(model);
            return View(model);
        }

        db.HoSoDangKyHocs.Add(new HoSoDangKyHoc
        {
            MaHocVien = student.MaHocVien,
            MaLop = model.MaLop,
            NgayNop = DateTime.Now,
            LyDoDangKy = model.LyDoDangKy?.Trim(),
            TrangThai = TrangThaiHoSo.ChoDuyet
        });
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        TempData["Success"] = "Đã nộp hồ sơ. Bạn có thể theo dõi trạng thái trong mục hồ sơ của tôi.";
        return RedirectToAction("CuaToi", "HoSoDangKyHoc");
    }

    private async Task<List<LopHoc>> LopDangDuocNhanHoSoAsync()
    {
        var today = DateTime.Today;
        return await db.LopHocs.AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiLop.DangMoDangKy &&
                        x.NgayBatDauDangKy.Date <= today && x.HanDangKy.Date >= today &&
                        db.HoSoDangKyHocs.Count(y =>
                            y.MaLop == x.MaLop && y.TrangThai == TrangThaiHoSo.DuocXepLop) < x.SiSoToiDa)
            .OrderBy(x => x.TenLop)
            .ToListAsync();
    }

    private async Task PopulateEligibleClassesAsync(EnrollmentViewModel model)
    {
        model.LopDangMo = (await LopDangDuocNhanHoSoAsync())
            .Select(x => new SelectListItem(x.TenLop, x.MaLop.ToString()));
    }

    private static bool IsAcceptingApplications(LopHoc? lop) =>
        lop is not null && lop.TrangThai == TrangThaiLop.DangMoDangKy &&
        lop.NgayBatDauDangKy.Date <= DateTime.Today && lop.HanDangKy.Date >= DateTime.Today;
}
