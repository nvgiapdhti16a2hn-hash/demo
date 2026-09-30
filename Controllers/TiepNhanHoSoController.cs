using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers;

public class TiepNhanHoSoController(AppDbContext db, CurrentUser currentUser) : AppController(currentUser)
{
    public async Task<IActionResult> Index(
        string? tuKhoa,
        int? maChuongTrinh,
        TrangThaiHoSo? trangThai,
        DateTime? ngayNop,
        string sapXep = "ngay_giam")
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");

        var query = db.HoSoDangKyHocs.AsNoTracking()
            .Include(x => x.HocVien)
            .Include(x => x.LopHoc).ThenInclude(x => x!.ChuongTrinhDaoTao)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var term = tuKhoa.Trim();
            query = query.Where(x => x.HocVien!.HoTen.Contains(term) || x.LopHoc!.TenLop.Contains(term));
        }
        if (trangThai.HasValue)
            query = query.Where(x => x.TrangThai == trangThai.Value);
        if (maChuongTrinh.HasValue)
            query = query.Where(x => x.LopHoc!.MaChuongTrinhDaoTao == maChuongTrinh.Value);
        if (ngayNop.HasValue)
        {
            var date = ngayNop.Value.Date;
            query = query.Where(x => x.NgayNop.Date == date);
        }

        query = sapXep == "ten_tang"
            ? query.OrderBy(x => x.HocVien!.HoTen)
            : sapXep == "ngay_tang"
                ? query.OrderBy(x => x.NgayNop)
                : query.OrderByDescending(x => x.NgayNop);
        ViewBag.TuKhoa = tuKhoa;
        ViewBag.MaChuongTrinh = maChuongTrinh;
        ViewBag.ChuongTrinhs = await db.ChuongTrinhDaoTaos.AsNoTracking()
            .OrderBy(x => x.TenChuongTrinhDaoTao).ToListAsync();
        ViewBag.TrangThai = trangThai;
        ViewBag.NgayNop = ngayNop?.ToString("yyyy-MM-dd");
        ViewBag.SapXep = sapXep;
        return View(await query.ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> XetDuyet(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var application = await db.HoSoDangKyHocs.AsNoTracking()
            .Include(x => x.HocVien)
            .Include(x => x.LopHoc)
            .SingleOrDefaultAsync(x => x.MaHoSo == id);
        if (application is null)
            return NotFound();
        if (application.TrangThai != TrangThaiHoSo.ChoDuyet)
        {
            TempData["Error"] = "Chỉ hồ sơ Chờ duyệt mới được xét duyệt.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Application = application;
        return View(new XetDuyetViewModel { MaHoSo = id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> XetDuyet(XetDuyetViewModel model)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        if (!ModelState.IsValid)
        {
            ViewBag.Application = await db.HoSoDangKyHocs.AsNoTracking()
                .Include(x => x.HocVien).Include(x => x.LopHoc)
                .SingleOrDefaultAsync(x => x.MaHoSo == model.MaHoSo);
            return View(model);
        }

        var application = await db.HoSoDangKyHocs
            .Include(x => x.HocVien)
            .Include(x => x.LopHoc)
            .SingleOrDefaultAsync(x => x.MaHoSo == model.MaHoSo);
        if (application is null)
            return NotFound();
        if (application.TrangThai != TrangThaiHoSo.ChoDuyet ||
            application.HocVien is null || !application.HocVien.TrangThai ||
            application.LopHoc is null || application.NgayXuLy.HasValue)
        {
            TempData["Error"] = "Hồ sơ không còn thỏa điều kiện xét duyệt.";
            return RedirectToAction(nameof(Index));
        }

        application.TrangThai = model.DuDieuKien
            ? TrangThaiHoSo.DuDieuKien
            : TrangThaiHoSo.KhongDuDieuKien;
        application.NgayXuLy = DateTime.Now;
        application.NhanXetXetDuyet = model.NhanXet.Trim();
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã lưu kết quả xét duyệt.";
        return RedirectToAction(nameof(Index));
    }
}
