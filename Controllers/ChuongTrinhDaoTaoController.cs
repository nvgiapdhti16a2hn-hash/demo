using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;

namespace QuanLyKhoaHoc.Controllers;

public class ChuongTrinhDaoTaoController(AppDbContext db, CurrentUser currentUser) : AppController(currentUser)
{
    public async Task<IActionResult> Index()
    {
        if (!RequireRole(nameof(VaiTro.Admin)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        return View(await db.ChuongTrinhDaoTaos.AsNoTracking()
            .OrderBy(x => x.TenChuongTrinhDaoTao).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var item = await db.ChuongTrinhDaoTaos.AsNoTracking()
            .Include(x => x.LopHocs).SingleOrDefaultAsync(x => x.MaChuongTrinhDaoTao == id);
        return item is null ? NotFound() : View(item);
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!RequireRole(nameof(VaiTro.Admin)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        return View(new ChuongTrinhDaoTao());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ChuongTrinhDaoTao model)
    {
        if (!RequireRole(nameof(VaiTro.Admin)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        if (await db.ChuongTrinhDaoTaos.AnyAsync(x => x.TenChuongTrinhDaoTao == model.TenChuongTrinhDaoTao.Trim()))
            ModelState.AddModelError(nameof(model.TenChuongTrinhDaoTao), "Tên chương trình đã tồn tại.");
        if (!ModelState.IsValid)
            return View(model);
        model.TenChuongTrinhDaoTao = model.TenChuongTrinhDaoTao.Trim();
        db.ChuongTrinhDaoTaos.Add(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã tạo chương trình đào tạo.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var model = await db.ChuongTrinhDaoTaos.FindAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ChuongTrinhDaoTao model)
    {
        if (!RequireRole(nameof(VaiTro.Admin)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        if (id != model.MaChuongTrinhDaoTao)
            return BadRequest();
        if (await db.ChuongTrinhDaoTaos.AnyAsync(x =>
                x.MaChuongTrinhDaoTao != id && x.TenChuongTrinhDaoTao == model.TenChuongTrinhDaoTao.Trim()))
            ModelState.AddModelError(nameof(model.TenChuongTrinhDaoTao), "Tên chương trình đã tồn tại.");
        if (!ModelState.IsValid)
            return View(model);
        db.Update(model);
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật chương trình đào tạo.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!RequireRole(nameof(VaiTro.Admin)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var item = await db.ChuongTrinhDaoTaos.Include(x => x.LopHocs)
            .SingleOrDefaultAsync(x => x.MaChuongTrinhDaoTao == id);
        if (item is null)
            return NotFound();
        if (item.LopHocs.Count != 0)
        {
            TempData["Error"] = "Không thể xóa chương trình đang có lớp học.";
            return RedirectToAction(nameof(Index));
        }
        db.ChuongTrinhDaoTaos.Remove(item);
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã xóa chương trình đào tạo.";
        return RedirectToAction(nameof(Index));
    }
}
