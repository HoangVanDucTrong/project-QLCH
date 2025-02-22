<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QLCH.Models;
using QLCH.Models.resetpassword;
namespace QLCH.Data
{
    public class QLCHDbConText : IdentityDbContext<AuthorcationStore>
    {
        public QLCHDbConText(DbContextOptions<QLCHDbConText> options)
            : base(options)
        {
        }

        public DbSet<store> Stores { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<QR> QRs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<TaiKhoanNhanVien> taiKhoanNhanViens { get; set; }
        public DbSet<CaLamNhanVien>  caLamNhanViens { get; set; }
        public DbSet<NhapHang> nhapHangs { get; set; }
        public DbSet<Bans> bans { get; set; }
        public DbSet<PasswordResetCode> passwordResetCodes { get; set; }
        public DbSet<Thongtintaikhoan> Thongtintaikhoan { get; set; }
        public DbSet<transaction> transactions   { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed data cho vai trò
            var roles = new List<IdentityRole>
    {
        new IdentityRole
        {
            Id = "admin-role-id",
            Name = "Admin",
            NormalizedName = "ADMIN"
        },
        new IdentityRole
        {
            Id = "employee-role-id",
            Name = "Employee",
            NormalizedName = "EMPLOYEE"
        }
    };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }

}
=======
﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QLCH.Models;
using QLCH.Models.resetpassword;
namespace QLCH.Data
{
    public class QLCHDbConText : IdentityDbContext<AuthorcationStore>
    {
        public QLCHDbConText(DbContextOptions<QLCHDbConText> options)
            : base(options)
        {
        }

        public DbSet<store> Stores { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }
        public DbSet<QR> QRs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<TaiKhoanNhanVien> taiKhoanNhanViens { get; set; }
        public DbSet<CaLamNhanVien>  caLamNhanViens { get; set; }
        public DbSet<NhapHang> nhapHangs { get; set; }
        public DbSet<Bans> bans { get; set; }
        public DbSet<PasswordResetCode> passwordResetCodes { get; set; }
        public DbSet<Thongtintaikhoan> thongtintaikhoans { get; set; }
        public DbSet<transaction> transactions   { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed data cho vai trò
            var roles = new List<IdentityRole>
    {
        new IdentityRole
        {
            Id = "admin-role-id",
            Name = "Admin",
            NormalizedName = "ADMIN"
        },
        new IdentityRole
        {
            Id = "employee-role-id",
            Name = "Employee",
            NormalizedName = "EMPLOYEE"
        }
    };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }

}
>>>>>>> 2cd039424233f099f062a952f82ef6ddcda03b12
