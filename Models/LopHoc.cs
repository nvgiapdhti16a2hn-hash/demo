using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.Models;

public class LopHoc
{
    public int MaLop { get; set; }

    [Required, StringLength(120)]
    public string TenLop { get; set; } = "";

    [Display(Name = "Chương trình đào tạo")]
    public int MaChuongTrinhDaoTao { get; set; }
    public ChuongTrinhDaoTao? ChuongTrinhDaoTao { get; set; }

    [Range(1, 10000)]
    public int SiSoToiDa { get; set; }

    [Required, StringLength(80)]
    public string TrinhDoDauVao { get; set; } = "";

    [Range(0, 100)]
    public decimal DiemDauVaoToiThieu { get; set; }

    [DataType(DataType.Date)]
    public DateTime NgayBatDauDangKy { get; set; }

    [DataType(DataType.Date)]
    public DateTime HanDangKy { get; set; }

    [StringLength(2000)]
    public string? MoTaLopHoc { get; set; }

    [StringLength(2000)]
    public string? YeuCauHocVien { get; set; }

    public TrangThaiLop TrangThai { get; set; } = TrangThaiLop.ChuaMo;
    public ICollection<HoSoDangKyHoc> HoSos { get; set; } = new List<HoSoDangKyHoc>();
}
