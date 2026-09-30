using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.Models;

public class KetQuaXepLop
{
    public int MaKetQuaXepLop { get; set; }
    public int MaHoSo { get; set; }
    public HoSoDangKyHoc? HoSoDangKyHoc { get; set; }

    [Range(0, 100)]
    public decimal? DiemDanhGia { get; set; }

    [StringLength(1000)]
    public string? NhanXet { get; set; }

    public bool DuocXepLop { get; set; }
    public DateTime NgayCapNhat { get; set; } = DateTime.Now;
}
