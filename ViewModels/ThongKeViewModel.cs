namespace QuanLyKhoaHoc.ViewModels;

public record ThongKeDong(string Ten, int SoLuong);
public record ThongKeTyLe(string Ten, int TuSo, int MauSo)
{
    public decimal PhanTram => MauSo == 0 ? 0 : Math.Round(TuSo * 100m / MauSo, 2);
}

public class ThongKeViewModel
{
    public int SoChuongTrinh { get; init; }
    public int SoLop { get; init; }
    public int SoHocVien { get; init; }
    public int SoHoSo { get; init; }
    public int SoHoSoChoDuyet { get; init; }
    public int SoHoSoDuDieuKien { get; init; }
    public int SoLichSapToi { get; init; }
    public int SoHocVienDuocXepLop { get; init; }
    public decimal TyLeXepLop { get; init; }
    public string? LopNhanNhieuHoSo { get; init; }
    public int SoHoSoCuaLopNhanNhieuNhat { get; init; }
    public IReadOnlyList<ThongKeDong> LopTheoChuongTrinh { get; init; } = [];
    public IReadOnlyList<ThongKeDong> HoSoTheoLop { get; init; } = [];
    public IReadOnlyList<ThongKeDong> HoSoDuDieuKienTheoLop { get; init; } = [];
    public IReadOnlyList<ThongKeDong> XepLopTheoChuongTrinh { get; init; } = [];
    public IReadOnlyList<ThongKeDong> HoSoTheoTrangThai { get; init; } = [];
    public IReadOnlyList<ThongKeDong> HoSoTheoThang { get; init; } = [];
    public IReadOnlyList<ThongKeDong> LichTheoThang { get; init; } = [];
    public IReadOnlyList<ThongKeTyLe> TyLeXetDuyetTheoLop { get; init; } = [];
}
