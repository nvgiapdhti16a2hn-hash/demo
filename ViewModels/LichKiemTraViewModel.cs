using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.ViewModels;

public class LichKiemTraViewModel
{
    public int MaHoSo { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime ThoiGianBatDau { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime ThoiGianKetThuc { get; set; }

    [Required, StringLength(80)]
    public string HinhThucKiemTra { get; set; } = "Trực tiếp";

    [StringLength(300)]
    public string? DiaDiemHoacLienKet { get; set; }

    [Required, StringLength(120)]
    public string GiaoVienKiemTra { get; set; } = "";

    [StringLength(1000)]
    public string? GhiChu { get; set; }
}
