using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;

namespace QuanLyKhoaHoc.Controllers;

public class QuanLyLopHocController(AppDbContext db, CurrentUser currentUser) : AppController(currentUser)
{
    public async Task<IActionResult> Index()
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        return View(await db.LopHocs.AsNoTracking().Include(x => x.ChuongTrinhDaoTao)
            .OrderBy(x => x.TenLop).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var item = await db.LopHocs.AsNoTracking().Include(x => x.ChuongTrinhDaoTao)
            .SingleOrDefaultAsync(x => x.MaLop == id);
        return item is null ? NotFound() : View(item);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        await PopulateProgramsAsync();
        return View(new LopHoc
        {
            NgayBatDauDangKy = DateTime.Today,
            HanDangKy = DateTime.Today.AddDays(14)
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LopHoc model)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        await ValidateModelAsync(model);
        if (!ModelState.IsValid)
        {
            await PopulateProgramsAsync(model.MaChuongTrinhDaoTao);
            return View(model);
        }
        db.LopHocs.Add(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã tạo lớp học.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var model = await db.LopHocs.FindAsync(id);
        if (model is null)
            return NotFound();
        await PopulateProgramsAsync(model.MaChuongTrinhDaoTao);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LopHoc model)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        if (id != model.MaLop)
            return BadRequest();
        await ValidateModelAsync(model, id);
        if (!ModelState.IsValid)
        {
            await PopulateProgramsAsync(model.MaChuongTrinhDaoTao);
            return View(model);
        }
        db.Entry(model).State = EntityState.Modified;
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật lớp học.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var item = await db.LopHocs.Include(x => x.HoSos)
            .SingleOrDefaultAsync(x => x.MaLop == id);
        if (item is null)
            return NotFound();
        if (item.HoSos.Count != 0)
        {
            TempData["Error"] = "Không thể xóa lớp đã có hồ sơ đăng ký.";
            return RedirectToAction(nameof(Index));
        }
        db.LopHocs.Remove(item);
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã xóa lớp học.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateModelAsync(LopHoc model, int? id = null)
    {
        if (model.HanDangKy.Date < model.NgayBatDauDangKy.Date)
            ModelState.AddModelError(nameof(model.HanDangKy), "Hạn đăng ký phải sau hoặc bằng ngày bắt đầu.");
        if (!await db.ChuongTrinhDaoTaos.AnyAsync(x => x.MaChuongTrinhDaoTao == model.MaChuongTrinhDaoTao))
            ModelState.AddModelError(nameof(model.MaChuongTrinhDaoTao), "Chương trình đào tạo không tồn tại.");
        else if (model.TrangThai == TrangThaiLop.DangMoDangKy &&
                 !await db.ChuongTrinhDaoTaos.AnyAsync(x =>
                     x.MaChuongTrinhDaoTao == model.MaChuongTrinhDaoTao &&
                     x.TrangThai == TrangThaiChuongTrinh.HoatDong))
            ModelState.AddModelError(nameof(model.MaChuongTrinhDaoTao),
                "Không thể mở đăng ký cho lớp thuộc chương trình đã ngừng hoạt động.");
        if (!string.IsNullOrWhiteSpace(model.TenLop) && await db.LopHocs.AnyAsync(x =>
                x.MaLop != id && x.TenLop == model.TenLop.Trim()))
            ModelState.AddModelError(nameof(model.TenLop), "Tên lớp đã tồn tại.");
        if (id.HasValue)
        {
            var placed = await db.HoSoDangKyHocs.CountAsync(x =>
                x.MaLop == id.Value && x.TrangThai == TrangThaiHoSo.DuocXepLop);
            if (model.SiSoToiDa < placed)
                ModelState.AddModelError(nameof(model.SiSoToiDa), $"Sĩ số không thể thấp hơn {placed} học viên đã được xếp lớp.");
        }
        if (model.TrangThai == TrangThaiLop.DangMoDangKy &&
            (model.SiSoToiDa <= 0 || string.IsNullOrWhiteSpace(model.TrinhDoDauVao) ||
             model.NgayBatDauDangKy == default || model.HanDangKy == default))
            ModelState.AddModelError(nameof(model.TrangThai), "Cần hoàn thiện thông tin bắt buộc trước khi mở đăng ký.");
        model.TenLop = model.TenLop?.Trim() ?? "";
    }

    private async Task PopulateProgramsAsync(int? selected = null)
    {
        ViewBag.Programs = new SelectList(
            await db.ChuongTrinhDaoTaos.AsNoTracking().OrderBy(x => x.TenChuongTrinhDaoTao).ToListAsync(),
            nameof(ChuongTrinhDaoTao.MaChuongTrinhDaoTao),
            nameof(ChuongTrinhDaoTao.TenChuongTrinhDaoTao),
            selected);
    }
}
