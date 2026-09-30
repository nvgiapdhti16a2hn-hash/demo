using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.Models;

public class TaiKhoan
{
    public int MaTaiKhoan { get; set; }

    [Required, StringLength(50)]
    public string TenDangNhap { get; set; } = "";

    [Required, StringLength(500)]
    public string MatKhau { get; set; } = "";

    [Required, StringLength(120)]
    public string HoTen { get; set; } = "";

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    public VaiTro VaiTro { get; set; }
    public TrangThaiTaiKhoan TrangThai { get; set; } = TrangThaiTaiKhoan.HoatDong;
    public HocVien? HocVien { get; set; }
}
