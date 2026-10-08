using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<ChuongTrinhDaoTao> ChuongTrinhDaoTaos { get; set; }
        public DbSet<LopHoc> LopHocs { get; set; }
        public DbSet<HocVien> HocViens { get; set; }
        public DbSet<DangKyHoc> DangKyHocs { get; set; }
        public DbSet<BuoiHoc> BuoiHocs { get; set; }
        public DbSet<KetQuaHocTap> KetQuaHocTaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Cấu hình Unique
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            // ================= SEED DATA LỚN THEO YÊU CẦU 16 ================= //
            modelBuilder.Seed();
        }
    }
}
