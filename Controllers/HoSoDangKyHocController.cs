// Họ và tên: Sinh viên 3 (bổ sung họ tên)
// Mã sinh viên: SV3 (bổ sung mã sinh viên)
// Nội dung thực hiện: Nộp, hủy và theo dõi hồ sơ đăng ký học của học viên.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.Services;

namespace QuanLyKhoaHoc.Controllers;

public class HoSoDangKyHocController(AppDbContext db, CurrentUser currentUser) : AppController(currentUser)
{
    [HttpGet]
    public async Task<IActionResult> CuaToi()
    {
        if (!RequireRole(nameof(VaiTro.HocVien)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var studentId = await db.HocViens.Where(x => x.MaTaiKhoan == CurrentUser.AccountId)
            .Select(x => (int?)x.MaHocVien).SingleOrDefaultAsync();
        if (studentId is null)
            return NotFound("Không tìm thấy hồ sơ học viên.");

        var applications = await db.HoSoDangKyHocs.AsNoTracking()
            .Where(x => x.MaHocVien == studentId)
            .Include(x => x.LopHoc).ThenInclude(x => x!.ChuongTrinhDaoTao)
            .Include(x => x.LichKiemTras)
            .Include(x => x.KetQuaXepLop)
            .OrderByDescending(x => x.NgayNop)
            .ToListAsync();
        return View(applications);
    }

    [HttpGet]
    public async Task<IActionResult> ChiTiet(int id)
    {
        if (!RequireRole(nameof(VaiTro.HocVien)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var studentId = await db.HocViens.Where(x => x.MaTaiKhoan == CurrentUser.AccountId)
            .Select(x => (int?)x.MaHocVien).SingleOrDefaultAsync();
        var application = await db.HoSoDangKyHocs.AsNoTracking()
            .Include(x => x.HocVien)
            .Include(x => x.LopHoc).ThenInclude(x => x!.ChuongTrinhDaoTao)
            .Include(x => x.LichKiemTras)
            .Include(x => x.KetQuaXepLop)
            .SingleOrDefaultAsync(x => x.MaHoSo == id && x.MaHocVien == studentId);
        return application is null ? NotFound() : View(application);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Huy(int id)
    {
        if (!RequireRole(nameof(VaiTro.HocVien)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var studentId = await db.HocViens.Where(x => x.MaTaiKhoan == CurrentUser.AccountId)
            .Select(x => (int?)x.MaHocVien).SingleOrDefaultAsync();
        var application = await db.HoSoDangKyHocs
            .Include(x => x.LichKiemTras)
            .SingleOrDefaultAsync(x => x.MaHoSo == id && x.MaHocVien == studentId);
        if (application is null)
            return NotFound();

        var hasActiveSchedule = application.LichKiemTras.Any(x => x.TrangThai == TrangThaiLich.DaLenLich);
        if (application.TrangThai is not (TrangThaiHoSo.ChoDuyet or TrangThaiHoSo.DuDieuKien) ||
            hasActiveSchedule)
        {
            TempData["Error"] = "Hồ sơ không còn ở trạng thái cho phép hủy.";
            return RedirectToAction(nameof(CuaToi));
        }

        application.TrangThai = TrangThaiHoSo.DaHuy;
        application.NgayXuLy = DateTime.Now;
        await db.SaveChangesAsync();
        TempData["Success"] = "Đã hủy hồ sơ.";
        return RedirectToAction(nameof(CuaToi));
    }

    [HttpGet]
    public async Task<IActionResult> DanhSach()
    {
        if (!RequireRole(nameof(VaiTro.Admin), nameof(VaiTro.NhanVienDaoTao)))
            return RedirectToAction("DangNhap", "TaiKhoan");
        var applications = await db.HoSoDangKyHocs.AsNoTracking()
            .Include(x => x.HocVien)
            .Include(x => x.LopHoc).ThenInclude(x => x!.ChuongTrinhDaoTao)
            .OrderByDescending(x => x.NgayNop)
            .ToListAsync();
        return View(applications);
    }
}
