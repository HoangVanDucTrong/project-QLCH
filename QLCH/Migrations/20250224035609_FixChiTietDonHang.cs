using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QLCH.Migrations
{
    /// <inheritdoc />
    public partial class FixChiTietDonHang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_SanPhams_SanPhamId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_QRs_Bans_BanId",
                table: "QRs");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietDonHangs_SanPhamId",
                table: "ChiTietDonHangs");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "004c7e80 - 7dfc - 44be - 8952 - 2c7130898655");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "71e282d3-76ca-485e-b094-eff019287fa5");

            migrationBuilder.DropColumn(
                name: "Gia",
                table: "ChiTietDonHangs");

            migrationBuilder.RenameColumn(
                name: "SoLuong",
                table: "ChiTietDonHangs",
                newName: "StoreId");

            migrationBuilder.RenameColumn(
                name: "SanPhamId",
                table: "ChiTietDonHangs",
                newName: "BanId");

            migrationBuilder.AlterColumn<int>(
                name: "BanId",
                table: "QRs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "SDT",
                table: "NhanVien",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<int>(
                name: "DonHangId",
                table: "ChiTietDonHangs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ImageCheckBank",
                table: "ChiTietDonHangs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsInUse",
                table: "Bans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Bans",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CaLamNhanVien",
                columns: table => new
                {
                    CaLamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NVid = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    NgayLam = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioBatDau = table.Column<TimeSpan>(type: "time", nullable: false),
                    GioKetThuc = table.Column<TimeSpan>(type: "time", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Thu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    calam = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaLamNhanVien", x => x.CaLamId);
                });

            migrationBuilder.CreateTable(
                name: "NhapHang",
                columns: table => new
                {
                    NHid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenHangHoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonVi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    soluong = table.Column<int>(type: "int", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NgayNhapHang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhapHang", x => x.NHid);
                });

            migrationBuilder.CreateTable(
                name: "passwordResetCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passwordResetCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SanPhamDonHang",
                columns: table => new
                {
                    SPDHId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CTDHId = table.Column<int>(type: "int", nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    Tensp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPhamDonHang", x => x.SPDHId);
                    table.ForeignKey(
                        name: "FK_SanPhamDonHang_ChiTietDonHangs_CTDHId",
                        column: x => x.CTDHId,
                        principalTable: "ChiTietDonHangs",
                        principalColumn: "CTDHId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoanNhanVien",
                columns: table => new
                {
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    NVid = table.Column<int>(type: "int", nullable: true),
                    TenDangNhap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sdt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuocGia = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoanNhanVien", x => x.TaiKhoanId);
                });

            migrationBuilder.CreateTable(
                name: "Thongtintaikhoan",
                columns: table => new
                {
                    bankid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcqId = table.Column<int>(type: "int", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thongtintaikhoan", x => x.bankid);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QRCodeUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.TransactionId);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "admin-role-id", null, "Admin", "ADMIN" },
                    { "employee-role-id", null, "Employee", "EMPLOYEE" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SanPhamDonHang_CTDHId",
                table: "SanPhamDonHang",
                column: "CTDHId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                table: "ChiTietDonHangs",
                column: "DonHangId",
                principalTable: "DonHangs",
                principalColumn: "DonHangId");

            migrationBuilder.AddForeignKey(
                name: "FK_QRs_Bans_BanId",
                table: "QRs",
                column: "BanId",
                principalTable: "Bans",
                principalColumn: "BanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                table: "ChiTietDonHangs");

            migrationBuilder.DropForeignKey(
                name: "FK_QRs_Bans_BanId",
                table: "QRs");

            migrationBuilder.DropTable(
                name: "CaLamNhanVien");

            migrationBuilder.DropTable(
                name: "NhapHang");

            migrationBuilder.DropTable(
                name: "passwordResetCodes");

            migrationBuilder.DropTable(
                name: "SanPhamDonHang");

            migrationBuilder.DropTable(
                name: "TaiKhoanNhanVien");

            migrationBuilder.DropTable(
                name: "Thongtintaikhoan");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "admin-role-id");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "employee-role-id");

            migrationBuilder.DropColumn(
                name: "ImageCheckBank",
                table: "ChiTietDonHangs");

            migrationBuilder.DropColumn(
                name: "IsInUse",
                table: "Bans");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Bans");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "ChiTietDonHangs",
                newName: "SoLuong");

            migrationBuilder.RenameColumn(
                name: "BanId",
                table: "ChiTietDonHangs",
                newName: "SanPhamId");

            migrationBuilder.AlterColumn<int>(
                name: "BanId",
                table: "QRs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SDT",
                table: "NhanVien",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "DonHangId",
                table: "ChiTietDonHangs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gia",
                table: "ChiTietDonHangs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "004c7e80 - 7dfc - 44be - 8952 - 2c7130898655", "004c7e80 - 7dfc - 44be - 8952 - 2c7130898655", "Read", "READ" },
                    { "71e282d3-76ca-485e-b094-eff019287fa5", "71e282d3-76ca-485e-b094-eff019287fa5", "Write", "WRITE" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_SanPhamId",
                table: "ChiTietDonHangs",
                column: "SanPhamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_DonHangs_DonHangId",
                table: "ChiTietDonHangs",
                column: "DonHangId",
                principalTable: "DonHangs",
                principalColumn: "DonHangId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHangs_SanPhams_SanPhamId",
                table: "ChiTietDonHangs",
                column: "SanPhamId",
                principalTable: "SanPhams",
                principalColumn: "SanPhamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QRs_Bans_BanId",
                table: "QRs",
                column: "BanId",
                principalTable: "Bans",
                principalColumn: "BanId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
