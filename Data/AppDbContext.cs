using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
    public DbSet<ChuongTrinhDaoTao> ChuongTrinhDaoTaos => Set<ChuongTrinhDaoTao>();
    public DbSet<LopHoc> LopHocs => Set<LopHoc>();
    public DbSet<HocVien> HocViens => Set<HocVien>();
    public DbSet<HoSoDangKyHoc> HoSoDangKyHocs => Set<HoSoDangKyHoc>();
    public DbSet<LichKiemTraDauVao> LichKiemTraDauVaos => Set<LichKiemTraDauVao>();
    public DbSet<KetQuaXepLop> KetQuaXepLops => Set<KetQuaXepLop>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaiKhoan>().HasKey(x => x.MaTaiKhoan);
        modelBuilder.Entity<ChuongTrinhDaoTao>().HasKey(x => x.MaChuongTrinhDaoTao);
        modelBuilder.Entity<LopHoc>().HasKey(x => x.MaLop);
        modelBuilder.Entity<HocVien>().HasKey(x => x.MaHocVien);
        modelBuilder.Entity<HoSoDangKyHoc>().HasKey(x => x.MaHoSo);
        modelBuilder.Entity<LichKiemTraDauVao>().HasKey(x => x.MaLichKiemTraDauVao);
        modelBuilder.Entity<KetQuaXepLop>().HasKey(x => x.MaKetQuaXepLop);

        modelBuilder.Entity<TaiKhoan>()
            .HasIndex(x => x.TenDangNhap)
            .IsUnique();
        modelBuilder.Entity<ChuongTrinhDaoTao>()
            .HasIndex(x => x.TenChuongTrinhDaoTao)
            .IsUnique();
        modelBuilder.Entity<HocVien>()
            .HasIndex(x => x.MaTaiKhoan)
            .IsUnique();
        modelBuilder.Entity<KetQuaXepLop>()
            .HasIndex(x => x.MaHoSo)
            .IsUnique();
        modelBuilder.Entity<LopHoc>()
            .Property(x => x.DiemDauVaoToiThieu)
            .HasPrecision(18, 2);
        modelBuilder.Entity<KetQuaXepLop>()
            .Property(x => x.DiemDanhGia)
            .HasPrecision(18, 2);
        modelBuilder.Entity<HocVien>()
            .HasOne(x => x.TaiKhoan)
            .WithOne(x => x.HocVien)
            .HasForeignKey<HocVien>(x => x.MaTaiKhoan)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LopHoc>()
            .HasOne(x => x.ChuongTrinhDaoTao)
            .WithMany(x => x.LopHocs)
            .HasForeignKey(x => x.MaChuongTrinhDaoTao)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HoSoDangKyHoc>()
            .HasOne(x => x.HocVien)
            .WithMany(x => x.HoSos)
            .HasForeignKey(x => x.MaHocVien)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HoSoDangKyHoc>()
            .HasOne(x => x.LopHoc)
            .WithMany(x => x.HoSos)
            .HasForeignKey(x => x.MaLop)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<LichKiemTraDauVao>()
            .HasOne(x => x.HoSoDangKyHoc)
            .WithMany(x => x.LichKiemTras)
            .HasForeignKey(x => x.MaHoSo)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<KetQuaXepLop>()
            .HasOne(x => x.HoSoDangKyHoc)
            .WithOne(x => x.KetQuaXepLop)
            .HasForeignKey<KetQuaXepLop>(x => x.MaHoSo)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
