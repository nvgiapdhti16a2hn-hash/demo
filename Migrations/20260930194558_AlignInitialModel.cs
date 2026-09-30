using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKhoaHoc.Migrations
{
    /// <inheritdoc />
    public partial class AlignInitialModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SiSoToiDa",
                table: "LopHocs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DuocXepLop",
                table: "KetQuaXepLops",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SoThangDaHoc",
                table: "HocViens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "HocViens",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SiSoToiDa",
                table: "LopHocs");

            migrationBuilder.DropColumn(
                name: "DuocXepLop",
                table: "KetQuaXepLops");

            migrationBuilder.DropColumn(
                name: "SoThangDaHoc",
                table: "HocViens");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "HocViens");
        }
    }
}
