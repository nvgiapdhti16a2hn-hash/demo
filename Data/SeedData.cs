using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services, AppDbContext db)
    {
        var hasher = services.GetRequiredService<IPasswordHasher<TaiKhoan>>();
        if (!await db.TaiKhoans.AnyAsync())
        {
            var accounts = new[]
            {
                NewAccount("admin", "Quản trị viên", VaiTro.Admin, hasher),
                NewAccount("staff", "Nhân viên đào tạo", VaiTro.NhanVienDaoTao, hasher),
                NewAccount("student", "Học viên mẫu", VaiTro.HocVien, hasher)
            };
            db.TaiKhoans.AddRange(accounts);
            await db.SaveChangesAsync();
            db.HocViens.Add(new HocVien
            {
                MaTaiKhoan = accounts[2].MaTaiKhoan,
                HoTen = accounts[2].HoTen,
                Email = "student@example.com",
                SoDienThoai = "0900000000",
                TrinhDoHienTai = "Cơ bản"
            });
        }

        if (!await db.ChuongTrinhDaoTaos.AnyAsync())
        {
            var program = new ChuongTrinhDaoTao
            {
                TenChuongTrinhDaoTao = "Tiếng Anh giao tiếp",
                MoTa = "Chương trình mẫu để kiểm thử luồng đăng ký khóa học.",
                EmailLienHe = "training@example.com"
            };
            db.ChuongTrinhDaoTaos.Add(program);
            await db.SaveChangesAsync();
            db.LopHocs.Add(new LopHoc
            {
                TenLop = "Giao tiếp căn bản - Khóa 01",
                MaChuongTrinhDaoTao = program.MaChuongTrinhDaoTao,
                SiSoToiDa = 25,
                TrinhDoDauVao = "Cơ bản",
                DiemDauVaoToiThieu = 0,
                NgayBatDauDangKy = DateTime.Today.AddDays(-1),
                HanDangKy = DateTime.Today.AddMonths(1),
                MoTaLopHoc = "Lớp học mẫu; có thể tìm kiếm và đăng ký bằng tài khoản học viên.",
                YeuCauHocVien = "Hoàn thành hồ sơ cá nhân trước khi đăng ký.",
                TrangThai = TrangThaiLop.DangMoDangKy
            });
        }

        await db.SaveChangesAsync();
    }

    private static TaiKhoan NewAccount(
        string username,
        string fullName,
        VaiTro role,
        IPasswordHasher<TaiKhoan> hasher)
    {
        var account = new TaiKhoan
        {
            TenDangNhap = username,
            HoTen = fullName,
            Email = $"{username}@example.com",
            VaiTro = role
        };
        account.MatKhau = hasher.HashPassword(account, "Test123!");
        return account;
    }
}
