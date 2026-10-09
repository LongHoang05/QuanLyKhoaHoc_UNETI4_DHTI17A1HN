// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Khung dự án - DbContext dùng chung

using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models.Entities;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data
{
    public class KhoaHocDbContext : DbContext
    {
        public KhoaHocDbContext(DbContextOptions<KhoaHocDbContext> options) : base(options) { }

        public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
        public DbSet<ChuongTrinhDaoTao> ChuongTrinhDaoTaos => Set<ChuongTrinhDaoTao>();
        public DbSet<LopHoc> LopHocs => Set<LopHoc>();
        public DbSet<HocVien> HocViens => Set<HocVien>();
        public DbSet<DangKyHoc> DangKyHocs => Set<DangKyHoc>();
        public DbSet<BuoiHoc> BuoiHocs => Set<BuoiHoc>();
        public DbSet<KetQuaHocTap> KetQuaHocTaps => Set<KetQuaHocTap>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

            // Đề yêu cầu: tên đăng nhập và tên chương trình không được trùng
            modelBuilder.Entity<TaiKhoan>().HasIndex(t => t.TenDangNhap).IsUnique();
            modelBuilder.Entity<ChuongTrinhDaoTao>().HasIndex(c => c.TenChuongTrinh).IsUnique();

            // Không xóa dây chuyền: giữ dữ liệu lịch sử (đề mục 5.5 và 6.2)
            foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}