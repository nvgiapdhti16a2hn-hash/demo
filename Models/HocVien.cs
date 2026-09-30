// Họ và tên: Sinh viên 3 (bổ sung họ tên)
// Mã sinh viên: SV3 (bổ sung mã sinh viên)
// Nội dung thực hiện: Quản lý học viên, hồ sơ cá nhân và đăng ký học.
using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.Models;

public class HocVien
{
    public int MaHocVien { get; set; }

    public int MaTaiKhoan { get; set; }
    public TaiKhoan? TaiKhoan { get; set; }

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

    public bool TrangThai { get; set; } = true;
    public ICollection<HoSoDangKyHoc> HoSos { get; set; } = new List<HoSoDangKyHoc>();
}
