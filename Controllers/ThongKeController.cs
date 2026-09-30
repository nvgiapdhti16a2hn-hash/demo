using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers;

public class ThongKeController(AppDbContext db, CurrentUser currentUser) : AppController(currentUser)
{
    public async Task<IActionResult> Index()
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");

        var applicationsByMonth = await db.HoSoDangKyHocs
            .GroupBy(x => new { x.NgayNop.Year, x.NgayNop.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync();
        var schedulesByMonth = await db.LichKiemTraDauVaos
            .GroupBy(x => new { x.ThoiGianBatDau.Year, x.ThoiGianBatDau.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .ToListAsync();
        var reviewRates = await db.HoSoDangKyHocs
            .GroupBy(x => x.LopHoc!.TenLop)
            .Select(g => new
            {
                ClassName = g.Key,
                Total = g.Count(),
                Reviewed = g.Count(x =>
                    x.TrangThai != TrangThaiHoSo.ChoDuyet &&
                    x.TrangThai != TrangThaiHoSo.DaHuy)
            })
            .ToListAsync();
        var applicationsByStatus = await db.HoSoDangKyHocs
            .GroupBy(x => x.TrangThai)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();
        var testedCount = await db.HoSoDangKyHocs.CountAsync(x =>
            x.TrangThai == TrangThaiHoSo.DaKiemTraDauVao ||
            x.TrangThai == TrangThaiHoSo.DuocXepLop ||
            x.TrangThai == TrangThaiHoSo.ChuaDuocXepLop);
        var placedCount = await db.HoSoDangKyHocs.CountAsync(x => x.TrangThai == TrangThaiHoSo.DuocXepLop);
        var topClass = await db.HoSoDangKyHocs
            .GroupBy(x => x.LopHoc!.TenLop)
            .Select(g => new { ClassName = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync();

        var model = new ThongKeViewModel
        {
            SoChuongTrinh = await db.ChuongTrinhDaoTaos.CountAsync(),
            SoLop = await db.LopHocs.CountAsync(),
            SoHocVien = await db.HocViens.CountAsync(),
            SoHoSo = await db.HoSoDangKyHocs.CountAsync(),
            SoHoSoChoDuyet = await db.HoSoDangKyHocs.CountAsync(x => x.TrangThai == TrangThaiHoSo.ChoDuyet),
            SoHoSoDuDieuKien = await db.HoSoDangKyHocs.CountAsync(x => x.TrangThai == TrangThaiHoSo.DuDieuKien),
            SoLichSapToi = await db.LichKiemTraDauVaos.CountAsync(x =>
                x.TrangThai == TrangThaiLich.DaLenLich && x.ThoiGianBatDau >= DateTime.Now),
            SoHocVienDuocXepLop = placedCount,
            TyLeXepLop = testedCount == 0 ? 0 : Math.Round(placedCount * 100m / testedCount, 2),
            LopNhanNhieuHoSo = topClass?.ClassName,
            SoHoSoCuaLopNhanNhieuNhat = topClass?.Count ?? 0,
            LopTheoChuongTrinh = await db.LopHocs
                .GroupBy(x => x.ChuongTrinhDaoTao!.TenChuongTrinhDaoTao)
                .Select(g => new ThongKeDong(g.Key, g.Count())).OrderByDescending(x => x.SoLuong).ToListAsync(),
            HoSoTheoLop = await db.HoSoDangKyHocs
                .GroupBy(x => x.LopHoc!.TenLop)
                .Select(g => new ThongKeDong(g.Key, g.Count())).OrderByDescending(x => x.SoLuong).ToListAsync(),
            HoSoDuDieuKienTheoLop = await db.HoSoDangKyHocs
                .Where(x => x.TrangThai == TrangThaiHoSo.DuDieuKien)
                .GroupBy(x => x.LopHoc!.TenLop)
                .Select(g => new ThongKeDong(g.Key, g.Count())).OrderByDescending(x => x.SoLuong).ToListAsync(),
            XepLopTheoChuongTrinh = await db.HoSoDangKyHocs
                .Where(x => x.TrangThai == TrangThaiHoSo.DuocXepLop)
                .GroupBy(x => x.LopHoc!.ChuongTrinhDaoTao!.TenChuongTrinhDaoTao)
                .Select(g => new ThongKeDong(g.Key, g.Count())).OrderByDescending(x => x.SoLuong).ToListAsync(),
            HoSoTheoTrangThai = applicationsByStatus
                .Select(x => new ThongKeDong(x.Status.ToDisplayName(), x.Count))
                .OrderByDescending(x => x.SoLuong).ToList(),
            HoSoTheoThang = applicationsByMonth
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Select(x => new ThongKeDong($"{x.Month:00}/{x.Year}", x.Count))
                .ToList(),
            LichTheoThang = schedulesByMonth
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Select(x => new ThongKeDong($"{x.Month:00}/{x.Year}", x.Count))
                .ToList(),
            TyLeXetDuyetTheoLop = reviewRates
                .Select(x => new ThongKeTyLe(x.ClassName, x.Reviewed, x.Total))
                .OrderBy(x => x.Ten).ToList()
        };
        return View(model);
    }
}
