using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers;

public class LichKiemTraDauVaoController(
    AppDbContext db,
    CurrentUser currentUser,
    ThongBaoService thongBaoService) : AppController(currentUser)
{
    public async Task<IActionResult> Index()
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        return View(await db.LichKiemTraDauVaos.AsNoTracking()
            .Include(x => x.HoSoDangKyHoc).ThenInclude(x => x!.HocVien)
            .Include(x => x.HoSoDangKyHoc).ThenInclude(x => x!.LopHoc)
            .OrderBy(x => x.ThoiGianBatDau)
            .ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> LapLich(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var application = await db.HoSoDangKyHocs.AsNoTracking()
            .Include(x => x.HocVien).Include(x => x.LopHoc)
            .SingleOrDefaultAsync(x => x.MaHoSo == id);
        if (application is null)
            return NotFound();
        if (application.TrangThai != TrangThaiHoSo.DuDieuKien)
        {
            TempData["Error"] = "Chỉ hồ sơ Đủ điều kiện mới được lập lịch.";
            return RedirectToAction("Index", "TiepNhanHoSo");
        }
        ViewBag.Application = application;
        return View(new LichKiemTraViewModel { MaHoSo = id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> LapLich(LichKiemTraViewModel model)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        if (model.ThoiGianKetThuc <= model.ThoiGianBatDau)
            ModelState.AddModelError(nameof(model.ThoiGianKetThuc), "Thời gian kết thúc phải sau thời gian bắt đầu.");
        if (model.ThoiGianBatDau < DateTime.Now)
            ModelState.AddModelError(nameof(model.ThoiGianBatDau), "Không thể lập lịch trong quá khứ.");
        if (!ModelState.IsValid)
        {
            ViewBag.Application = await db.HoSoDangKyHocs.AsNoTracking()
                .Include(x => x.HocVien).Include(x => x.LopHoc)
                .SingleOrDefaultAsync(x => x.MaHoSo == model.MaHoSo);
            return View(model);
        }

        var application = await db.HoSoDangKyHocs
            .Include(x => x.HocVien)
            .Include(x => x.LichKiemTras)
            .SingleOrDefaultAsync(x => x.MaHoSo == model.MaHoSo);
        if (application is null)
            return NotFound();
        if (application.TrangThai != TrangThaiHoSo.DuDieuKien ||
            application.HocVien is null || !application.HocVien.TrangThai ||
            application.LichKiemTras.Any(x => x.TrangThai == TrangThaiLich.DaLenLich))
        {
            TempData["Error"] = "Hồ sơ không còn đủ điều kiện lập lịch.";
            return RedirectToAction(nameof(Index));
        }

        var teacher = model.GiaoVienKiemTra.Trim();
        var collision = await db.LichKiemTraDauVaos
            .Where(x => x.TrangThai != TrangThaiLich.DaHuy &&
                        x.ThoiGianBatDau < model.ThoiGianKetThuc &&
                        x.ThoiGianKetThuc > model.ThoiGianBatDau)
            .AnyAsync(x => x.GiaoVienKiemTra == teacher ||
                           x.HoSoDangKyHoc!.MaHocVien == application.MaHocVien);
        if (collision)
        {
            ModelState.AddModelError(string.Empty, "Học viên hoặc giáo viên đã có lịch kiểm tra trùng thời gian.");
            ViewBag.Application = application;
            return View(model);
        }

        db.LichKiemTraDauVaos.Add(new LichKiemTraDauVao
        {
            MaHoSo = application.MaHoSo,
            ThoiGianBatDau = model.ThoiGianBatDau,
            ThoiGianKetThuc = model.ThoiGianKetThuc,
            HinhThucKiemTra = model.HinhThucKiemTra.Trim(),
            DiaDiemHoacLienKet = model.DiaDiemHoacLienKet?.Trim(),
            GiaoVienKiemTra = teacher,
            GhiChu = model.GhiChu?.Trim(),
            TrangThai = TrangThaiLich.DaLenLich
        });
        application.TrangThai = TrangThaiHoSo.ChoKiemTraDauVao;
        await thongBaoService.TaoChoHocVienAsync(
            application.MaHoSo,
            "Lịch kiểm tra đầu vào đã được lập",
            $"Lịch kiểm tra lớp {application.LopHoc?.TenLop} bắt đầu lúc {model.ThoiGianBatDau:dd/MM/yyyy HH:mm}.");
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã lập lịch kiểm tra đầu vào.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> HoanThanh(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var schedule = await db.LichKiemTraDauVaos.Include(x => x.HoSoDangKyHoc)
            .SingleOrDefaultAsync(x => x.MaLichKiemTraDauVao == id);
        if (schedule is null)
            return NotFound();
        if (schedule.TrangThai != TrangThaiLich.DaLenLich ||
            schedule.HoSoDangKyHoc?.TrangThai != TrangThaiHoSo.ChoKiemTraDauVao)
        {
            TempData["Error"] = "Lịch không ở trạng thái có thể hoàn thành.";
            return RedirectToAction(nameof(Index));
        }
        schedule.TrangThai = TrangThaiLich.DaHoanThanh;
        schedule.HoSoDangKyHoc.TrangThai = TrangThaiHoSo.DaKiemTraDauVao;
        await thongBaoService.TaoChoHocVienAsync(
            schedule.MaHoSo,
            "Đã hoàn thành kiểm tra đầu vào",
            "Lịch kiểm tra đầu vào của bạn đã hoàn thành. Kết quả xếp lớp sẽ được cập nhật sau.");
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã hoàn thành lịch kiểm tra. Có thể nhập kết quả xếp lớp.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Huy(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var schedule = await db.LichKiemTraDauVaos.Include(x => x.HoSoDangKyHoc)
            .ThenInclude(x => x!.LopHoc)
            .SingleOrDefaultAsync(x => x.MaLichKiemTraDauVao == id);
        if (schedule is null)
            return NotFound();
        if (schedule.TrangThai != TrangThaiLich.DaLenLich)
        {
            TempData["Error"] = "Chỉ lịch chưa hoàn thành mới có thể hủy.";
            return RedirectToAction(nameof(Index));
        }
        schedule.TrangThai = TrangThaiLich.DaHuy;
        if (schedule.HoSoDangKyHoc?.TrangThai == TrangThaiHoSo.ChoKiemTraDauVao)
        {
            schedule.HoSoDangKyHoc.TrangThai = TrangThaiHoSo.DuDieuKien;
            await thongBaoService.TaoChoHocVienAsync(
                schedule.MaHoSo,
                "Lịch kiểm tra đã bị hủy",
                $"Lịch kiểm tra lớp {schedule.HoSoDangKyHoc.LopHoc?.TenLop} đã bị hủy. Hồ sơ đã trở về trạng thái Đủ điều kiện.");
        }
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã hủy lịch kiểm tra.";
        return RedirectToAction(nameof(Index));
    }
}
