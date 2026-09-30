using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers;

public class TaiKhoanController(
    AppDbContext db,
    IPasswordHasher<TaiKhoan> passwordHasher,
    CurrentUser currentUser) : AppController(currentUser)
{
    [HttpGet]
    public IActionResult DangNhap(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DangNhap(LoginViewModel model, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        if (!ModelState.IsValid)
            return View(model);

        var username = model.TenDangNhap.Trim();
        var account = await db.TaiKhoans.SingleOrDefaultAsync(x => x.TenDangNhap == username);
        if (account is null)
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập không tồn tại.");
            return View(model);
        }
        if (account.TrangThai != TrangThaiTaiKhoan.HoatDong)
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tài khoản đã bị khóa.");
            return View(model);
        }
        if (passwordHasher.VerifyHashedPassword(account, account.MatKhau, model.MatKhau) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(nameof(model.MatKhau), "Mật khẩu không đúng.");
            return View(model);
        }

        CurrentUser.SignIn(account);
        return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult DangKy() => View(new RegisterStudentViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DangKy(RegisterStudentViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var username = model.TenDangNhap.Trim();
        if (await db.TaiKhoans.AnyAsync(x => x.TenDangNhap == username))
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã được sử dụng.");
            return View(model);
        }

        var account = new TaiKhoan
        {
            TenDangNhap = username,
            HoTen = model.HoTen.Trim(),
            Email = model.Email.Trim(),
            VaiTro = VaiTro.HocVien,
            TrangThai = TrangThaiTaiKhoan.HoatDong
        };
        account.MatKhau = passwordHasher.HashPassword(account, model.MatKhau);
        account.HocVien = new HocVien
        {
            HoTen = account.HoTen,
            Email = account.Email,
            SoDienThoai = model.SoDienThoai
        };
        db.TaiKhoans.Add(account);
        await db.SaveChangesAsync();
        TempData["Success"] = "Tạo tài khoản thành công. Vui lòng đăng nhập.";
        return RedirectToAction(nameof(DangNhap));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult DangXuat()
    {
        CurrentUser.SignOut();
        TempData["Success"] = "Bạn đã đăng xuất.";
        return RedirectToAction("Index", "Home");
    }
}
