using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.Models;

public class LichKiemTraDauVao
{
    public int MaLichKiemTraDauVao { get; set; }
    public int MaHoSo { get; set; }
    public HoSoDangKyHoc? HoSoDangKyHoc { get; set; }
    public DateTime ThoiGianBatDau { get; set; }
    public DateTime ThoiGianKetThuc { get; set; }

    [Required, StringLength(80)]
    public string HinhThucKiemTra { get; set; } = "Trực tiếp";

    [StringLength(300)]
    public string? DiaDiemHoacLienKet { get; set; }

    [Required, StringLength(120)]
    public string GiaoVienKiemTra { get; set; } = "";

    [StringLength(1000)]
    public string? GhiChu { get; set; }

    public TrangThaiLich TrangThai { get; set; } = TrangThaiLich.DaLenLich;
}
