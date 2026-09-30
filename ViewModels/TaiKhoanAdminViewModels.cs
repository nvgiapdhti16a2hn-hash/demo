using System.ComponentModel.DataAnnotations;
using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.ViewModels;

public class TaoTaiKhoanViewModel
{
    [Required, StringLength(50)]
    public string TenDangNhap { get; set; } = "";

    [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)]
    public string MatKhau { get; set; } = "";

    [Required, StringLength(120)]
    public string HoTen { get; set; } = "";

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    public VaiTro VaiTro { get; set; } = VaiTro.NhanVienDaoTao;
}
