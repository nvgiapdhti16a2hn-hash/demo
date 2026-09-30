using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.Services;

public class ThongBaoService(AppDbContext db)
{
    public async Task TaoChoHocVienAsync(
        int maHoSo,
        string tieuDe,
        string noiDung,
        CancellationToken cancellationToken = default)
    {
        var maTaiKhoan = await db.HoSoDangKyHocs
            .Where(x => x.MaHoSo == maHoSo)
            .Select(x => (int?)x.HocVien!.MaTaiKhoan)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"Không tìm thấy học viên cho hồ sơ {maHoSo}.");

        db.ThongBaos.Add(new ThongBao
        {
            MaTaiKhoan = maTaiKhoan,
            MaHoSo = maHoSo,
            TieuDe = tieuDe,
            NoiDung = noiDung,
            NgayTao = DateTime.Now
        });
    }
}
