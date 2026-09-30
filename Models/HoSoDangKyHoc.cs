// Họ và tên: Sinh viên 3 (bổ sung họ tên)
// Mã sinh viên: SV3 (bổ sung mã sinh viên)
// Nội dung thực hiện: Nộp, hủy và theo dõi hồ sơ đăng ký học của học viên.
using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.Models;

public class HoSoDangKyHoc
{
    public int MaHoSo { get; set; }
    public int MaHocVien { get; set; }
    public HocVien? HocVien { get; set; }
    public int MaLop { get; set; }
    public LopHoc? LopHoc { get; set; }
    public DateTime NgayNop { get; set; } = DateTime.Now;

    [StringLength(1000)]
    public string? LyDoDangKy { get; set; }

    [StringLength(1000)]
    public string? GhiChu { get; set; }

    public TrangThaiHoSo TrangThai { get; set; } = TrangThaiHoSo.ChoDuyet;
    public DateTime? NgayXuLy { get; set; }

    [StringLength(1000)]
    public string? NhanXetXetDuyet { get; set; }

    public ICollection<LichKiemTraDauVao> LichKiemTras { get; set; } = new List<LichKiemTraDauVao>();
    public KetQuaXepLop? KetQuaXepLop { get; set; }
}
