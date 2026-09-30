using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.ViewModels;

public class CourseListViewModel
{
    public IReadOnlyList<LopHoc> LopHocs { get; init; } = [];
    public IReadOnlyList<ChuongTrinhDaoTao> ChuongTrinhs { get; init; } = [];
    public string? TuKhoa { get; init; }
    public int? MaChuongTrinh { get; init; }
    public string? TrinhDo { get; init; }
    public decimal? DiemToiDa { get; init; }
    public TrangThaiLop? TrangThai { get; init; }
    public bool? ConHan { get; init; }
    public string SapXep { get; init; } = "ten_az";
    public int TrangHienTai { get; init; }
    public int TongSoTrang { get; init; }
    public IEnumerable<int> SoTrang => Enumerable.Range(1, TongSoTrang);
}
