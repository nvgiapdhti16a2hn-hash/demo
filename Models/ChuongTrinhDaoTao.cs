using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.Models;

public class ChuongTrinhDaoTao
{
    public int MaChuongTrinhDaoTao { get; set; }

    [Required, StringLength(150)]
    public string TenChuongTrinhDaoTao { get; set; } = "";

    [StringLength(2000)]
    public string? MoTa { get; set; }

    [EmailAddress, StringLength(150)]
    public string? EmailLienHe { get; set; }

    public TrangThaiChuongTrinh TrangThai { get; set; } = TrangThaiChuongTrinh.HoatDong;
    public ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();
}
