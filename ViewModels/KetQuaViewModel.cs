using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.ViewModels;

public class KetQuaViewModel
{
    public int MaHoSo { get; set; }

    [Range(0, 100)]
    public decimal? DiemDanhGia { get; set; }

    [StringLength(1000)]
    public string? NhanXet { get; set; }

    public bool DuocXepLop { get; set; }
}
