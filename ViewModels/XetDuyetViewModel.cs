using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.ViewModels;

public class XetDuyetViewModel
{
    public int MaHoSo { get; set; }
    public bool DuDieuKien { get; set; }

    [Required, StringLength(1000)]
    public string NhanXet { get; set; } = "";
}
