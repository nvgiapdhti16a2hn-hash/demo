using System.ComponentModel.DataAnnotations;

namespace QuanLyKhoaHoc.Models;

public enum VaiTro
{
    [Display(Name = "Quản trị viên")]
    Admin,
    [Display(Name = "Nhân viên đào tạo")]
    NhanVienDaoTao,
    [Display(Name = "Học viên")]
    HocVien
}

public enum TrangThaiTaiKhoan
{
    [Display(Name = "Hoạt động")]
    HoatDong,
    [Display(Name = "Bị khóa")]
    BiKhoa
}

public enum TrangThaiChuongTrinh
{
    [Display(Name = "Hoạt động")]
    HoatDong,
    [Display(Name = "Ngừng hoạt động")]
    NgungHoatDong
}

public enum TrangThaiLop
{
    [Display(Name = "Chưa mở")]
    ChuaMo,
    [Display(Name = "Đang mở đăng ký")]
    DangMoDangKy,
    [Display(Name = "Tạm dừng")]
    TamDung,
    [Display(Name = "Đã đóng")]
    DaDong
}

public enum TrangThaiHoSo
{
    [Display(Name = "Chờ duyệt")]
    ChoDuyet,
    [Display(Name = "Đủ điều kiện")]
    DuDieuKien,
    [Display(Name = "Không đủ điều kiện")]
    KhongDuDieuKien,
    [Display(Name = "Chờ kiểm tra đầu vào")]
    ChoKiemTraDauVao,
    [Display(Name = "Đã kiểm tra đầu vào")]
    DaKiemTraDauVao,
    [Display(Name = "Được xếp lớp")]
    DuocXepLop,
    [Display(Name = "Chưa được xếp lớp")]
    ChuaDuocXepLop,
    [Display(Name = "Đã hủy")]
    DaHuy
}

public enum TrangThaiLich
{
    [Display(Name = "Đã lên lịch")]
    DaLenLich,
    [Display(Name = "Đã hoàn thành")]
    DaHoanThanh,
    [Display(Name = "Đã hủy")]
    DaHuy
}
