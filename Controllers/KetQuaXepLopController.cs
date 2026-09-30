using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers;

public class KetQuaXepLopController(
    AppDbContext db,
    CurrentUser currentUser,
    ThongBaoService thongBaoService) : AppController(currentUser)
{
    [HttpGet]
    public async Task<IActionResult> CapNhat(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var application = await db.HoSoDangKyHocs.AsNoTracking()
            .Include(x => x.HocVien).Include(x => x.LopHoc)
            .SingleOrDefaultAsync(x => x.MaHoSo == id);
        if (application is null)
            return NotFound();
        if (application.TrangThai != TrangThaiHoSo.DaKiemTraDauVao)
        {
            TempData["Error"] = "Chỉ hồ sơ Đã kiểm tra đầu vào mới được nhập kết quả.";
            return RedirectToAction("Index", "TiepNhanHoSo");
        }
        ViewBag.Application = application;
        return View(new KetQuaViewModel { MaHoSo = id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhat(KetQuaViewModel model)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        if (!ModelState.IsValid)
        {
            ViewBag.Application = await GetApplicationAsync(model.MaHoSo);
            return View(model);
        }

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var application = await db.HoSoDangKyHocs
            .Include(x => x.LopHoc)
            .Include(x => x.KetQuaXepLop)
            .SingleOrDefaultAsync(x => x.MaHoSo == model.MaHoSo);
        if (application is null)
            return NotFound();
        if (application.TrangThai != TrangThaiHoSo.DaKiemTraDauVao || application.KetQuaXepLop is not null)
        {
            TempData["Error"] = "Hồ sơ không còn ở trạng thái cho phép ghi nhận kết quả.";
            return RedirectToAction("Index", "TiepNhanHoSo");
        }

        if (model.DuocXepLop)
        {
            var placedCount = await db.HoSoDangKyHocs.CountAsync(x =>
                x.MaLop == application.MaLop && x.TrangThai == TrangThaiHoSo.DuocXepLop);
            if (application.LopHoc is null || placedCount >= application.LopHoc.SiSoToiDa)
            {
                TempData["Error"] = "Lớp đã đủ sĩ số tối đa, không thể xếp thêm học viên.";
                await transaction.RollbackAsync();
                return RedirectToAction("Index", "TiepNhanHoSo");
            }
        }

        application.KetQuaXepLop = new KetQuaXepLop
        {
            MaHoSo = application.MaHoSo,
            DiemDanhGia = model.DiemDanhGia,
            NhanXet = model.NhanXet?.Trim(),
            DuocXepLop = model.DuocXepLop,
            NgayCapNhat = DateTime.Now
        };
        application.TrangThai = model.DuocXepLop
            ? TrangThaiHoSo.DuocXepLop
            : TrangThaiHoSo.ChuaDuocXepLop;
        await thongBaoService.TaoChoHocVienAsync(
            application.MaHoSo,
            "Đã có kết quả xếp lớp",
            model.DuocXepLop
                ? $"Bạn đã được xếp vào lớp {application.LopHoc?.TenLop}."
                : $"Bạn chưa được xếp vào lớp {application.LopHoc?.TenLop}. Vui lòng xem nhận xét kết quả.");
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        TempData["Success"] = "Đã cập nhật kết quả xếp lớp.";
        return RedirectToAction("Index", "TiepNhanHoSo");
    }

    private Task<HoSoDangKyHoc?> GetApplicationAsync(int id) =>
        db.HoSoDangKyHocs.AsNoTracking().Include(x => x.HocVien).Include(x => x.LopHoc)
            .SingleOrDefaultAsync(x => x.MaHoSo == id);
}
