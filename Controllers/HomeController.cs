using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;

namespace QuanLyKhoaHoc.Controllers;

public class HomeController(AppDbContext db, CurrentUser currentUser) : AppController(currentUser)
{
    public async Task<IActionResult> Index()
    {
        ViewBag.ProgramCount = await db.ChuongTrinhDaoTaos.CountAsync();
        ViewBag.ClassCount = await db.LopHocs.CountAsync();
        ViewBag.OpenClassCount = await db.LopHocs.CountAsync(x => x.TrangThai == TrangThaiLop.DangMoDangKy);
        ViewBag.ApplicationCount = await db.HoSoDangKyHocs.CountAsync();
        ViewBag.PendingCount = await db.HoSoDangKyHocs.CountAsync(x => x.TrangThai == TrangThaiHoSo.ChoDuyet);
        ViewBag.PlacedCount = await db.HoSoDangKyHocs.CountAsync(x => x.TrangThai == TrangThaiHoSo.DuocXepLop);
        ViewBag.UpcomingExams = await db.LichKiemTraDauVaos.CountAsync(x =>
            x.TrangThai == TrangThaiLich.DaLenLich && x.ThoiGianBatDau >= DateTime.Now);
        return View();
    }

    public IActionResult Error() => View("Error");
}
