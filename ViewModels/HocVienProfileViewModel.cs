// Họ và tên: Sinh viên 3 (bổ sung họ tên)
// Mã sinh viên: SV3 (bổ sung mã sinh viên)
// Nội dung thực hiện: ViewModel cập nhật hồ sơ cá nhân học viên.
using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.ViewModels;

public class HocVienProfileViewModel
{
    [Required, StringLength(120)]
    public string HoTen { get; set; } = "";

    [DataType(DataType.Date)]
    public DateTime? NgaySinh { get; set; }

    [StringLength(20)]
    public string? GioiTinh { get; set; }

    [Phone, StringLength(30)]
    public string? SoDienThoai { get; set; }

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    [StringLength(300)]
    public string? DiaChi { get; set; }

    [StringLength(100)]
    public string? TrinhDoHienTai { get; set; }

    [StringLength(120)]
    public string? NgheNghiep { get; set; }

    [Range(0, 1200)]
    public int SoThangDaHoc { get; set; }

    [StringLength(1000)]
    public string? MucTieuHoc { get; set; }
}
