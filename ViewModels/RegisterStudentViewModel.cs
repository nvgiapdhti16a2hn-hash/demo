// Họ và tên: Sinh viên 3 (bổ sung họ tên)
// Mã sinh viên: SV3 (bổ sung mã sinh viên)
// Nội dung thực hiện: ViewModel đăng ký tài khoản và hồ sơ học viên.
using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.ViewModels;

public class RegisterStudentViewModel
{
    [Required, StringLength(50)]
    public string TenDangNhap { get; set; } = "";

    [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)]
    public string MatKhau { get; set; } = "";

    [Required, StringLength(120)]
    public string HoTen { get; set; } = "";

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = "";

    [Phone, StringLength(30)]
    public string? SoDienThoai { get; set; }
}
