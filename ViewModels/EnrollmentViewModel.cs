// Họ và tên: Sinh viên 3 (bổ sung họ tên)
// Mã sinh viên: SV3 (bổ sung mã sinh viên)
// Nội dung thực hiện: ViewModel nộp hồ sơ đăng ký khóa học.
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyKhoaHoc.ViewModels;

public class EnrollmentViewModel
{
    public int MaLop { get; set; }
    public string TenLop { get; set; } = "";

    [StringLength(1000)]
    public string? LyDoDangKy { get; set; }

    public IEnumerable<SelectListItem> LopDangMo { get; set; } = [];
}
