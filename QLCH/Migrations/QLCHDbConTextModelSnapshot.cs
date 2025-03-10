
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QLCH.Data;

#nullable disable

namespace QLCH.Migrations
{
    [DbContext(typeof(QLCHDbConText))]
    partial class QLCHDbConTextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "admin-role-id",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = "employee-role-id",
                            Name = "Employee",
                            NormalizedName = "EMPLOYEE"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("QLCH.Models.AuthorcationStore", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DiaChi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("QuocGia")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sdt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("QLCH.Models.Bans", b =>
                {
                    b.Property<int?>("BanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("BanId"));

                    b.Property<string>("IsInUse")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SoBan")
                        .HasColumnType("int");

                    b.Property<int?>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("BanId");

                    b.ToTable("Bans");
                });

            modelBuilder.Entity("QLCH.Models.CaLamNhanVien", b =>
                {
                    b.Property<int?>("CaLamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("CaLamId"));

                    b.Property<string>("GhiChu")
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("GioBatDau")
                        .HasColumnType("time");

                    b.Property<TimeSpan>("GioKetThuc")
                        .HasColumnType("time");

                    b.Property<int>("NVid")
                        .HasColumnType("int");

                    b.Property<DateTime>("NgayLam")
                        .HasColumnType("datetime2");

                    b.Property<int?>("StoreId")
                        .HasColumnType("int");

                    b.Property<string>("Thu")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("calam")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CaLamId");

                    b.ToTable("CaLamNhanVien");
                });

            modelBuilder.Entity("QLCH.Models.ChiTietDonHang", b =>
                {
                    b.Property<int>("CTDHId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CTDHId"));

                    b.Property<int>("BanId")
                        .HasColumnType("int");

                    b.Property<int?>("DonHangId")
                        .HasColumnType("int");

                    b.Property<string>("ImageCheckBank")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("NgayTao")
                        .HasColumnType("datetime2");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("CTDHId");

                    b.HasIndex("DonHangId");

                    b.ToTable("ChiTietDonHangs");
                });

            modelBuilder.Entity("QLCH.Models.Client", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Pass")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("UserId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("QLCH.Models.DanhMuc", b =>
                {
                    b.Property<int?>("DanhMucId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("DanhMucId"));

                    b.Property<string>("MoTa")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("TenDanhMuc")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("DanhMucId");

                    b.ToTable("DanhMucs");
                });

            modelBuilder.Entity("QLCH.Models.DonHang", b =>
                {
                    b.Property<int>("DonHangId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DonHangId"));

                    b.Property<DateTime>("NgayDatHang")
                        .HasColumnType("datetime2");

                    b.Property<int>("TongTien")
                        .HasColumnType("int");

                    b.Property<string>("TrangThai")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("DonHangId");

                    b.HasIndex("UserId");

                    b.ToTable("DonHangs");
                });

            modelBuilder.Entity("QLCH.Models.NhanVien", b =>
                {
                    b.Property<int?>("NVid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("NVid"));

                    b.Property<string>("AnhNhanVien")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("MucLuong")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<DateTime>("NgayVaoLam")
                        .HasColumnType("datetime2");

                    b.Property<string>("SDT")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int?>("StoreId")
                        .HasColumnType("int");

                    b.Property<string>("TenNV")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("NVid");

                    b.ToTable("NhanVien");
                });

            modelBuilder.Entity("QLCH.Models.NhapHang", b =>
                {
                    b.Property<int?>("NHid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("NHid"));

                    b.Property<decimal>("DonGia")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("DonVi")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("NgayNhapHang")
                        .HasColumnType("datetime2");

                    b.Property<int?>("StoreId")
                        .HasColumnType("int");

                    b.Property<string>("TenHangHoa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("ThanhTien")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("soluong")
                        .HasColumnType("int");

                    b.HasKey("NHid");

                    b.ToTable("NhapHang");
                });

            modelBuilder.Entity("QLCH.Models.QR", b =>
                {
                    b.Property<int>("QRId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QRId"));

                    b.Property<int?>("BanId")
                        .HasColumnType("int");

                    b.Property<string>("DuLieuMaQR")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("QRId");

                    b.HasIndex("BanId");

                    b.ToTable("QRs");
                });

            modelBuilder.Entity("QLCH.Models.SanPham", b =>
                {
                    b.Property<int?>("SanPhamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("SanPhamId"));

                    b.Property<int>("DanhMucId")
                        .HasColumnType("int");

                    b.Property<int>("Gia")
                        .HasColumnType("int");

                    b.Property<string>("ImageBase64")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MoTa")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int?>("StoreId")
                        .HasColumnType("int");

                    b.Property<string>("Ten")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SanPhamId");

                    b.HasIndex("DanhMucId");

                    b.ToTable("SanPhams");
                });

            modelBuilder.Entity("QLCH.Models.SanPhamDonHang", b =>
                {
                    b.Property<int>("SPDHId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SPDHId"));

                    b.Property<int>("CTDHId")
                        .HasColumnType("int");

                    b.Property<string>("GhiChu")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Gia")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("SanPhamId")
                        .HasColumnType("int");

                    b.Property<int>("SoLuong")
                        .HasColumnType("int");

                    b.Property<string>("Tensp")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("TongTien")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("SPDHId");

                    b.HasIndex("CTDHId");

                    b.ToTable("SanPhamDonHang");
                });

            modelBuilder.Entity("QLCH.Models.TaiKhoanNhanVien", b =>
                {
                    b.Property<int?>("TaiKhoanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("TaiKhoanId"));

                    b.Property<string>("DiaChi")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MatKhau")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("NVid")
                        .HasColumnType("int");

                    b.Property<string>("QuocGia")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sdt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.Property<string>("TenDangNhap")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TaiKhoanId");

                    b.ToTable("TaiKhoanNhanVien");
                });

            modelBuilder.Entity("QLCH.Models.ThanhToan", b =>
                {
                    b.Property<int>("ThanhToanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ThanhToanId"));

                    b.Property<int>("DonHangId")
                        .HasColumnType("int");

                    b.Property<DateTime>("NgayThanhToan")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhuongThucThanhToan")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<int>("SoTien")
                        .HasColumnType("int");

                    b.Property<string>("TrangThai")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("ThanhToanId");

                    b.HasIndex("DonHangId");

                    b.ToTable("ThanhToans");
                });

            modelBuilder.Entity("QLCH.Models.Thongtintaikhoan", b =>
                {
                    b.Property<int?>("bankid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("bankid"));

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AcqId")
                        .HasColumnType("int");

                    b.Property<string>("BankAccount")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("BankAccount");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("bankid");

                    b.ToTable("Thongtintaikhoan");
                });

            modelBuilder.Entity("QLCH.Models.resetpassword.PasswordResetCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Expiration")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("passwordResetCodes");
                });

            modelBuilder.Entity("QLCH.Models.store", b =>
                {
                    b.Property<int>("StoreId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StoreId"));

                    b.Property<string>("DiaChi")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Pass")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("QuocGia")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Sdt")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("StoreId");

                    b.ToTable("Stores");
                });

            modelBuilder.Entity("QLCH.Models.transaction", b =>
                {
                    b.Property<int?>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("TransactionId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QRCodeUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("TransactionId");

                    b.ToTable("transactions");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("QLCH.Models.AuthorcationStore", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("QLCH.Models.AuthorcationStore", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QLCH.Models.AuthorcationStore", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("QLCH.Models.AuthorcationStore", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("QLCH.Models.ChiTietDonHang", b =>
                {
                    b.HasOne("QLCH.Models.DonHang", null)
                        .WithMany("ChiTietDonHangs")
                        .HasForeignKey("DonHangId");
                });

            modelBuilder.Entity("QLCH.Models.DonHang", b =>
                {
                    b.HasOne("QLCH.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");
                });

            modelBuilder.Entity("QLCH.Models.QR", b =>
                {
                    b.HasOne("QLCH.Models.Bans", "bans")
                        .WithMany()
                        .HasForeignKey("BanId");

                    b.Navigation("bans");
                });

            modelBuilder.Entity("QLCH.Models.SanPham", b =>
                {
                    b.HasOne("QLCH.Models.DanhMuc", null)
                        .WithMany("SanPhams")
                        .HasForeignKey("DanhMucId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("QLCH.Models.SanPhamDonHang", b =>
                {
                    b.HasOne("QLCH.Models.ChiTietDonHang", "ChiTietDonHang")
                        .WithMany("SanPhamDonHangs")
                        .HasForeignKey("CTDHId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChiTietDonHang");
                });

            modelBuilder.Entity("QLCH.Models.ThanhToan", b =>
                {
                    b.HasOne("QLCH.Models.DonHang", "DonHang")
                        .WithMany("ThanhToans")
                        .HasForeignKey("DonHangId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DonHang");
                });

            modelBuilder.Entity("QLCH.Models.ChiTietDonHang", b =>
                {
                    b.Navigation("SanPhamDonHangs");
                });

            modelBuilder.Entity("QLCH.Models.DanhMuc", b =>
                {
                    b.Navigation("SanPhams");
                });

            modelBuilder.Entity("QLCH.Models.DonHang", b =>
                {
                    b.Navigation("ChiTietDonHangs");

                    b.Navigation("ThanhToans");
                });
#pragma warning restore 612, 618
        }
    }
}

