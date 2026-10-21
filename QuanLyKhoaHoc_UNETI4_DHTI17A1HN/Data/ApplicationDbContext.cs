using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data
{
    /// <summary>
    /// DbContext chính của ứng dụng - Kiến trúc CSDL 7 thực thể cốt lõi chuẩn Đề tài 16 BTL UNETI
    /// ChuongTrinhDaoTao (1) -> (n) LopHoc (1) -> (n) DangKyHoc -> KetQuaHocTap
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ========================================================
        // ====== Module 1 - Vũ Đình Trường (23103100026) ======
        // ========================================================
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<ChuongTrinhDaoTao> ChuongTrinhDaoTaos { get; set; }

        // ========================================================
        // ====== Module 2 - Nguyễn Đình Kiên (23103100020) ======
        // ========================================================
        public DbSet<LopHoc> LopHocs { get; set; }
        public DbSet<GiangVien> GiangViens { get; set; } // Thực thể bổ sung theo Mục 10

        // ========================================================
        // ====== Module 3 - Nguyễn Đức Linh (23103100042) ======
        // ========================================================
        public DbSet<HocVien> HocViens { get; set; }

        // ========================================================
        // ====== Module 4 - Nguyễn Hoàng Long (23103100046) ======
        // ========================================================
        public DbSet<DangKyHoc> DangKyHocs { get; set; }

        // ========================================================
        // ====== Module 5 - Nguyễn Tùng Dương (23103100003) ======
        // ========================================================
        public DbSet<BuoiHoc> BuoiHocs { get; set; }
        public DbSet<KetQuaHocTap> KetQuaHocTaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Cấu hình quan hệ giữa các Entity theo Mục 10 =====

            // 1. TaiKhoan -> HocVien (1 - 0..1)
            modelBuilder.Entity<HocVien>()
                .HasOne(h => h.TaiKhoan)
                .WithOne(t => t.HocVien)
                .HasForeignKey<HocVien>(h => h.MaTaiKhoan)
                .OnDelete(DeleteBehavior.SetNull);

            // 2. ChuongTrinhDaoTao -> LopHoc (1 - n) - KHÔNG QUA BẢNG KHÓA HỌC TRUNG GIAN
            modelBuilder.Entity<LopHoc>()
                .HasOne(l => l.ChuongTrinhDaoTao)
                .WithMany(c => c.LopHocs)
                .HasForeignKey(l => l.MaChuongTrinh)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. GiangVien -> LopHoc (1 - n) (Tùy chọn)
            modelBuilder.Entity<LopHoc>()
                .HasOne(l => l.GiangVien)
                .WithMany(g => g.LopHocs)
                .HasForeignKey(l => l.MaGiangVien)
                .OnDelete(DeleteBehavior.SetNull);

            // 4. HocVien -> DangKyHoc (1 - n)
            modelBuilder.Entity<DangKyHoc>()
                .HasOne(d => d.HocVien)
                .WithMany(h => h.DangKyHocs)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. LopHoc -> DangKyHoc (1 - n)
            modelBuilder.Entity<DangKyHoc>()
                .HasOne(d => d.LopHoc)
                .WithMany(l => l.DangKyHocs)
                .HasForeignKey(d => d.MaLopHoc)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. LopHoc -> BuoiHoc (1 - 0..n)
            modelBuilder.Entity<BuoiHoc>()
                .HasOne(b => b.LopHoc)
                .WithMany(l => l.BuoiHocs)
                .HasForeignKey(b => b.MaLopHoc)
                .OnDelete(DeleteBehavior.Cascade);

            // 7. DangKyHoc -> KetQuaHocTap (1 - 0..1)
            modelBuilder.Entity<KetQuaHocTap>()
                .HasOne(k => k.DangKyHoc)
                .WithOne(d => d.KetQuaHocTap)
                .HasForeignKey<KetQuaHocTap>(k => k.MaDangKy)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Dữ liệu mẫu khởi tạo (Seed Data chuẩn Đề tài 16) =====
            modelBuilder.Entity<TaiKhoan>().HasData(
                new TaiKhoan
                {
                    MaTaiKhoan = 1,
                    TenDangNhap = "admin",
                    MatKhau = "admin123",
                    HoTen = "Quản trị viên Hệ thống",
                    Email = "admin@uneti.edu.vn",
                    VaiTro = "Admin",
                    TrangThai = true,
                    NgayTao = new DateTime(2026, 1, 1)
                },
                new TaiKhoan
                {
                    MaTaiKhoan = 2,
                    TenDangNhap = "giaovu",
                    MatKhau = "giaovu123",
                    HoTen = "Giáo vụ Trung tâm",
                    Email = "giaovu@uneti.edu.vn",
                    VaiTro = "GiaoVu",
                    TrangThai = true,
                    NgayTao = new DateTime(2026, 1, 1)
                },
                new TaiKhoan
                {
                    MaTaiKhoan = 3,
                    TenDangNhap = "hocvien",
                    MatKhau = "hocvien123",
                    HoTen = "Học viên Nguyễn Văn An",
                    Email = "an.nguyen@gmail.com",
                    VaiTro = "HocVien",
                    TrangThai = true,
                    NgayTao = new DateTime(2026, 1, 1)
                }
            );

            modelBuilder.Entity<ChuongTrinhDaoTao>().HasData(
                new ChuongTrinhDaoTao
                {
                    MaChuongTrinh = 1,
                    TenChuongTrinh = "Lập trình Web & Phần mềm Doanh nghiệp",
                    MoTa = "Đào tạo kỹ sư phần mềm thực chiến với ASP.NET Core 10, React, SQL Server",
                    EmailLienHe = "cntt@uneti.edu.vn",
                    TrangThai = true,
                    NgayTao = new DateTime(2026, 1, 1)
                },
                new ChuongTrinhDaoTao
                {
                    MaChuongTrinh = 2,
                    TenChuongTrinh = "Khoa học Dữ liệu & Trí tuệ Nhân tạo",
                    MoTa = "Chương trình chuyên sâu Python, Machine Learning, Deep Learning và Power BI",
                    EmailLienHe = "ai@uneti.edu.vn",
                    TrangThai = true,
                    NgayTao = new DateTime(2026, 1, 1)
                }
            );

            modelBuilder.Entity<GiangVien>().HasData(
                new GiangVien
                {
                    MaGiangVien = 1,
                    HoTen = "ThS. Trần Văn Hùng",
                    Email = "tvhung@uneti.edu.vn",
                    SoDienThoai = "0912345678",
                    ChuyenNganh = "Công nghệ phần mềm & Web",
                    TrangThai = true,
                    NgayTao = new DateTime(2026, 1, 1)
                }
            );

            modelBuilder.Entity<LopHoc>().HasData(
                new LopHoc
                {
                    MaLopHoc = 1,
                    TenLop = "NETCORE10-K01",
                    MaChuongTrinh = 1, // Trực thuộc ChuongTrinhDaoTao #1
                    MaGiangVien = 1,
                    TenGiangVien = "ThS. Trần Văn Hùng",
                    TrinhDoDauVao = "Cơ bản",
                    SiSoToiDa = 25,
                    SoBuoiHoc = 16,
                    HocPhi = 3500000,
                    NgayBatDauDangKy = new DateTime(2026, 1, 1),
                    HanDangKy = new DateTime(2026, 12, 31),
                    NgayKhaiGiang = new DateTime(2027, 1, 15),
                    MoTa = "Lớp học buổi tối thứ 2-4-6 từ 18:00 - 21:00",
                    YeuCauHocVien = "Đã có kiến thức lập trình C# cơ bản",
                    TrangThai = "Đang mở đăng ký",
                    PhongHoc = "Lab 402 - Cơ sở Lĩnh Nam",
                    NgayTao = new DateTime(2026, 1, 1)
                }
            );

            modelBuilder.Entity<HocVien>().HasData(
                new HocVien
                {
                    MaHocVien = 1,
                    MaTaiKhoan = 3,
                    HoTen = "Học viên Nguyễn Văn An",
                    Email = "an.nguyen@gmail.com",
                    SoDienThoai = "0987654321",
                    NgaySinh = new DateTime(2003, 5, 20),
                    GioiTinh = "Nam",
                    DiaChi = "Hà Nội",
                    TrinhDoHocVan = "Đại học",
                    NgheNghiep = "Sinh viên",
                    TrangThai = true,
                    NgayTao = new DateTime(2026, 1, 1)
                }
            );

            modelBuilder.Entity<DangKyHoc>().HasData(
                new DangKyHoc
                {
                    MaDangKy = 1,
                    MaHocVien = 1,
                    MaLopHoc = 1,
                    NgayDangKy = new DateTime(2026, 2, 1),
                    GhiChu = "Đăng ký lớp học NETCORE10-K01",
                    TrangThai = "Chờ duyệt",
                    SoTienDaDong = 0
                }
            );
        }
    }
}
