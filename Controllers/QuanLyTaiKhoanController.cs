using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers;

public class QuanLyTaiKhoanController(
    AppDbContext db,
    IPasswordHasher<TaiKhoan> passwordHasher,
    CurrentUser currentUser) : AppController(currentUser)
{
    public async Task<IActionResult> Index()
    {
        if (!RequireRole(nameof(VaiTro.Admin)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        return View(await db.TaiKhoans.AsNoTracking().OrderBy(x => x.TenDangNhap).ToListAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!RequireRole(nameof(VaiTro.Admin)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        return View(new TaoTaiKhoanViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaoTaiKhoanViewModel model)
    {
        if (!RequireRole(nameof(VaiTro.Admin)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        if (model.VaiTro == VaiTro.HocVien)
            ModelState.AddModelError(nameof(model.VaiTro), "Học viên cần đăng ký qua chức năng tạo tài khoản học viên.");
        var username = model.TenDangNhap.Trim();
        if (await db.TaiKhoans.AnyAsync(x => x.TenDangNhap == username))
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");
        if (!ModelState.IsValid)
            return View(model);

        var account = new TaiKhoan
        {
            TenDangNhap = username,
            HoTen = model.HoTen.Trim(),
            Email = model.Email?.Trim(),
            VaiTro = model.VaiTro,
            TrangThai = TrangThaiTaiKhoan.HoatDong
        };
        account.MatKhau = passwordHasher.HashPassword(account, model.MatKhau);
        db.TaiKhoans.Add(account);
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã tạo tài khoản.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        if (id == CurrentUser.AccountId)
        {
            TempData["Error"] = "Không thể tự khóa tài khoản đang đăng nhập.";
            return RedirectToAction(nameof(Index));
        }
        var account = await db.TaiKhoans.FindAsync(id);
        if (account is null)
            return NotFound();
        account.TrangThai = account.TrangThai == TrangThaiTaiKhoan.HoatDong
            ? TrangThaiTaiKhoan.BiKhoa
            : TrangThaiTaiKhoan.HoatDong;
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật trạng thái tài khoản.";
        return RedirectToAction(nameof(Index));
    }
}
