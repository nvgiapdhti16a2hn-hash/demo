// Họ và tên: Sinh viên 3 (bổ sung họ tên)
// Mã sinh viên: SV3 (bổ sung mã sinh viên)
// Nội dung thực hiện: Quản lý hồ sơ cá nhân và danh sách học viên.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers;

public class HocVienController(AppDbContext db, CurrentUser currentUser) : AppController(currentUser)
{
    [HttpGet]
    public async Task<IActionResult> Sua()
    {
        if (!RequireRole(nameof(VaiTro.HocVien)))
            return RedirectToAction("DangNhap", "TaiKhoan");

        var student = await GetCurrentStudentAsync();
        if (student is null)
            return NotFound("Không tìm thấy hồ sơ học viên.");
        if (!student.TrangThai)
            return Forbid();
        return View(ToViewModel(student));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Sua(HocVienProfileViewModel model)
    {
        if (!RequireRole(nameof(VaiTro.HocVien)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        if (!ModelState.IsValid)
            return View(model);

        var student = await GetCurrentStudentAsync();
        if (student is null)
            return NotFound("Không tìm thấy hồ sơ học viên.");
        if (!student.TrangThai)
            return Forbid();

        student.HoTen = model.HoTen.Trim();
        student.NgaySinh = model.NgaySinh;
        student.GioiTinh = model.GioiTinh;
        student.SoDienThoai = model.SoDienThoai?.Trim();
        student.Email = model.Email?.Trim();
        student.DiaChi = model.DiaChi?.Trim();
        student.TrinhDoHienTai = model.TrinhDoHienTai?.Trim();
        student.NgheNghiep = model.NgheNghiep?.Trim();
        student.SoThangDaHoc = model.SoThangDaHoc;
        student.MucTieuHoc = model.MucTieuHoc?.Trim();
        if (student.TaiKhoan is not null)
        {
            student.TaiKhoan.HoTen = student.HoTen;
            student.TaiKhoan.Email = student.Email;
        }

        await db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật hồ sơ cá nhân.";
        return RedirectToAction(nameof(Sua));
    }

    [HttpGet]
    public async Task<IActionResult> DanhSach()
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        return View(await db.HocViens.AsNoTracking()
            .Include(x => x.TaiKhoan)
            .OrderBy(x => x.HoTen)
            .ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var student = await db.HocViens.SingleOrDefaultAsync(x => x.MaHocVien == id);
        if (student is null)
            return NotFound();
        student.TrangThai = !student.TrangThai;
        await db.SaveChangesAsync();
        TempData["Success"] = student.TrangThai
            ? "Đã kích hoạt học viên."
            : "Đã ngừng hoạt động học viên.";
        return RedirectToAction(nameof(DanhSach));
    }

    private Task<HocVien?> GetCurrentStudentAsync() =>
        db.HocViens.Include(x => x.TaiKhoan)
            .SingleOrDefaultAsync(x => x.MaTaiKhoan == CurrentUser.AccountId);

    private static HocVienProfileViewModel ToViewModel(HocVien student) => new()
    {
        HoTen = student.HoTen,
        NgaySinh = student.NgaySinh,
        GioiTinh = student.GioiTinh,
        SoDienThoai = student.SoDienThoai,
        Email = student.Email,
        DiaChi = student.DiaChi,
        TrinhDoHienTai = student.TrinhDoHienTai,
        NgheNghiep = student.NgheNghiep,
        SoThangDaHoc = student.SoThangDaHoc,
        MucTieuHoc = student.MucTieuHoc
    };
}
