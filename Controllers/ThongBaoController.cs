using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;

namespace QuanLyKhoaHoc.Controllers;

public class ThongBaoController(AppDbContext db, CurrentUser currentUser) : AppController(currentUser)
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!RequireRole(nameof(VaiTro.HocVien)))
            return RedirectToAction("DangNhap", "TaiKhoan");

        var notifications = await db.ThongBaos.AsNoTracking()
            .Where(x => x.MaTaiKhoan == CurrentUser.AccountId)
            .OrderByDescending(x => x.NgayTao)
            .ToListAsync();
        return View(notifications);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Doc(int id)
    {
        if (!RequireRole(nameof(VaiTro.HocVien)))
            return RedirectToAction("DangNhap", "TaiKhoan");

        var notification = await db.ThongBaos
            .SingleOrDefaultAsync(x => x.MaThongBao == id && x.MaTaiKhoan == CurrentUser.AccountId);
        if (notification is null)
            return NotFound();

        notification.NgayDoc ??= DateTime.Now;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MoHoSo(int id)
    {
        if (!RequireRole(nameof(VaiTro.HocVien)))
            return RedirectToAction("DangNhap", "TaiKhoan");

        var notification = await db.ThongBaos.AsNoTracking()
            .SingleOrDefaultAsync(x => x.MaThongBao == id && x.MaTaiKhoan == CurrentUser.AccountId);
        if (notification is null)
            return NotFound();

        if (notification.NgayDoc is null)
        {
            var tracked = await db.ThongBaos.FindAsync(id);
            if (tracked is null || tracked.MaTaiKhoan != CurrentUser.AccountId)
                return NotFound();
            tracked.NgayDoc = DateTime.Now;
            await db.SaveChangesAsync();
        }

        return RedirectToAction("ChiTiet", "HoSoDangKyHoc", new { id = notification.MaHoSo });
    }
}
