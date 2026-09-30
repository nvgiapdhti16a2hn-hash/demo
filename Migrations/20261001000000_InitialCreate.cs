using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using QuanLyKhoaHoc.Data;

namespace QuanLyKhoaHoc.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260929000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ChuongTrinhDaoTaos",
            columns: table => new
            {
                MaChuongTrinhDaoTao = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenChuongTrinhDaoTao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                MoTa = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                EmailLienHe = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                TrangThai = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_ChuongTrinhDaoTaos", x => x.MaChuongTrinhDaoTao));

        migrationBuilder.CreateTable(
            name: "TaiKhoans",
            columns: table => new
            {
                MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                MatKhau = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                HoTen = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                VaiTro = table.Column<int>(type: "int", nullable: false),
                TrangThai = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan));

        migrationBuilder.CreateTable(
            name: "LopHocs",
            columns: table => new
            {
                MaLop = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TenLop = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                MaChuongTrinhDaoTao = table.Column<int>(type: "int", nullable: false),
                TrinhDoDauVao = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                DiemDauVaoToiThieu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                NgayBatDauDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                HanDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                MoTaLopHoc = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                YeuCauHocVien = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                TrangThai = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LopHocs", x => x.MaLop);
                table.ForeignKey(
                    name: "FK_LopHocs_ChuongTrinhDaoTaos_MaChuongTrinhDaoTao",
                    column: x => x.MaChuongTrinhDaoTao,
                    principalTable: "ChuongTrinhDaoTaos",
                    principalColumn: "MaChuongTrinhDaoTao",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "HocViens",
            columns: table => new
            {
                MaHocVien = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                HoTen = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                GioiTinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                SoDienThoai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                DiaChi = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                TrinhDoHienTai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                NgheNghiep = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                MucTieuHoc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HocViens", x => x.MaHocVien);
                table.ForeignKey(
                    name: "FK_HocViens_TaiKhoans_MaTaiKhoan",
                    column: x => x.MaTaiKhoan,
                    principalTable: "TaiKhoans",
                    principalColumn: "MaTaiKhoan",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "HoSoDangKyHocs",
            columns: table => new
            {
                MaHoSo = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                MaHocVien = table.Column<int>(type: "int", nullable: false),
                MaLop = table.Column<int>(type: "int", nullable: false),
                NgayNop = table.Column<DateTime>(type: "datetime2", nullable: false),
                LyDoDangKy = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                GhiChu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                TrangThai = table.Column<int>(type: "int", nullable: false),
                NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                NhanXetXetDuyet = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HoSoDangKyHocs", x => x.MaHoSo);
                table.ForeignKey(
                    name: "FK_HoSoDangKyHocs_HocViens_MaHocVien",
                    column: x => x.MaHocVien,
                    principalTable: "HocViens",
                    principalColumn: "MaHocVien",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_HoSoDangKyHocs_LopHocs_MaLop",
                    column: x => x.MaLop,
                    principalTable: "LopHocs",
                    principalColumn: "MaLop",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "LichKiemTraDauVaos",
            columns: table => new
            {
                MaLichKiemTraDauVao = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                MaHoSo = table.Column<int>(type: "int", nullable: false),
                ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                HinhThucKiemTra = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                DiaDiemHoacLienKet = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                GiaoVienKiemTra = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                GhiChu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                TrangThai = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LichKiemTraDauVaos", x => x.MaLichKiemTraDauVao);
                table.ForeignKey(
                    name: "FK_LichKiemTraDauVaos_HoSoDangKyHocs_MaHoSo",
                    column: x => x.MaHoSo,
                    principalTable: "HoSoDangKyHocs",
                    principalColumn: "MaHoSo",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "KetQuaXepLops",
            columns: table => new
            {
                MaKetQuaXepLop = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                MaHoSo = table.Column<int>(type: "int", nullable: false),
                DiemDanhGia = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                NhanXet = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_KetQuaXepLops", x => x.MaKetQuaXepLop);
                table.ForeignKey(
                    name: "FK_KetQuaXepLops_HoSoDangKyHocs_MaHoSo",
                    column: x => x.MaHoSo,
                    principalTable: "HoSoDangKyHocs",
                    principalColumn: "MaHoSo",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ChuongTrinhDaoTaos_TenChuongTrinhDaoTao",
            table: "ChuongTrinhDaoTaos",
            column: "TenChuongTrinhDaoTao",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_HocViens_MaTaiKhoan",
            table: "HocViens",
            column: "MaTaiKhoan",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_HoSoDangKyHocs_MaHocVien",
            table: "HoSoDangKyHocs",
            column: "MaHocVien");
        migrationBuilder.CreateIndex(
            name: "IX_HoSoDangKyHocs_MaLop",
            table: "HoSoDangKyHocs",
            column: "MaLop");
        migrationBuilder.CreateIndex(
            name: "IX_KetQuaXepLops_MaHoSo",
            table: "KetQuaXepLops",
            column: "MaHoSo",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_LichKiemTraDauVaos_MaHoSo",
            table: "LichKiemTraDauVaos",
            column: "MaHoSo");
        migrationBuilder.CreateIndex(
            name: "IX_LopHocs_MaChuongTrinhDaoTao",
            table: "LopHocs",
            column: "MaChuongTrinhDaoTao");
        migrationBuilder.CreateIndex(
            name: "IX_TaiKhoans_TenDangNhap",
            table: "TaiKhoans",
            column: "TenDangNhap",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "KetQuaXepLops");
        migrationBuilder.DropTable(name: "LichKiemTraDauVaos");
        migrationBuilder.DropTable(name: "HoSoDangKyHocs");
        migrationBuilder.DropTable(name: "HocViens");
        migrationBuilder.DropTable(name: "LopHocs");
        migrationBuilder.DropTable(name: "TaiKhoans");
        migrationBuilder.DropTable(name: "ChuongTrinhDaoTaos");
    }
}
