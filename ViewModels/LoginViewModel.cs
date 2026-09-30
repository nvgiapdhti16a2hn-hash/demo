using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.ViewModels;

public class LoginViewModel
{
    [Required, StringLength(50)]
    public string TenDangNhap { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string MatKhau { get; set; } = "";
}
