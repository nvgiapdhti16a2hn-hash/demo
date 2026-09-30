using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKhoaHoc.Migrations
{
    /// <inheritdoc />
    public partial class AddPersistentNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThongBaos",
                columns: table => new
                {
                    MaThongBao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    MaHoSo = table.Column<int>(type: "int", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayDoc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaos", x => x.MaThongBao);
                    table.ForeignKey(
                        name: "FK_ThongBaos_HoSoDangKyHocs_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoDangKyHocs",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThongBaos_TaiKhoans_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaHoSo",
                table: "ThongBaos",
                column: "MaHoSo");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_MaTaiKhoan_NgayDoc_NgayTao",
                table: "ThongBaos",
                columns: new[] { "MaTaiKhoan", "NgayDoc", "NgayTao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThongBaos");
        }
    }
}
