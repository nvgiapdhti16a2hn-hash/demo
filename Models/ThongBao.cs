using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.Models;

public class ThongBao
{
    public int MaThongBao { get; set; }
    public int MaTaiKhoan { get; set; }
    public TaiKhoan? TaiKhoan { get; set; }
    public int MaHoSo { get; set; }
    public HoSoDangKyHoc? HoSoDangKyHoc { get; set; }

    [Required, StringLength(120)]
    public string TieuDe { get; set; } = "";

    [Required, StringLength(1000)]
    public string NoiDung { get; set; } = "";

    public DateTime NgayTao { get; set; } = DateTime.Now;
    public DateTime? NgayDoc { get; set; }
}
