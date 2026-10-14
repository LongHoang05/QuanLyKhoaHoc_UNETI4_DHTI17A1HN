using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh - MSV: 23103100042
    // Module 3: Repository triển khai truy vấn EF Core 10
    // Chuẩn theo giáo trình bài giảng khoa CNTT UNETI (Slide 105, 119)
    // ========================================================
    public class HocVienRepository : IHocVienRepository
    {
        private readonly ApplicationDbContext _context;

        public HocVienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<HocVien> GetAll()
        {
            return _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                .OrderByDescending(h => h.NgayTao)
                .ToList();
        }

        public async Task<List<HocVien>> GetAllAsync()
        {
            return await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                .OrderByDescending(h => h.NgayTao)
                .ToListAsync();
        }

        public async Task<HocVien?> GetByIdAsync(int? id)
        {
            if (id == null) return null;
            return await _context.HocViens.FindAsync(id.Value);
        }

        public async Task<HocVien?> GetByIdWithDetailsAsync(int? id)
        {
            if (id == null) return null;
            return await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                    .ThenInclude(d => d.LopHoc)
                        .ThenInclude(l => l!.KhoaHoc)
                .FirstOrDefaultAsync(h => h.MaHocVien == id.Value);
        }

        public async Task<HocVien?> GetByMaTaiKhoanAsync(int maTaiKhoan)
        {
            return await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
        }

        public async Task AddAsync(HocVien hocVien)
        {
            await _context.HocViens.AddAsync(hocVien);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HocVien hocVien)
        {
            _context.HocViens.Update(hocVien);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var hocVien = await _context.HocViens.FindAsync(id);
            if (hocVien != null)
            {
                _context.HocViens.Remove(hocVien);
                await _context.SaveChangesAsync();
            }
        }

        public bool Exists(int id)
        {
            return (_context.HocViens?.Any(e => e.MaHocVien == id)).GetValueOrDefault();
        }

        public async Task<bool> ExistsEmailAsync(string email, int? excludeId = null)
        {
            var cleanEmail = email.Trim().ToLower();
            var query = _context.HocViens.AsQueryable();

            if (excludeId.HasValue)
            {
                query = query.Where(h => h.MaHocVien != excludeId.Value);
            }

            return await query.AnyAsync(h => h.Email.ToLower() == cleanEmail);
        }

        public async Task<bool> CoDangKyHocAsync(int maHocVien)
        {
            return await _context.DangKyHocs.AnyAsync(d => d.MaHocVien == maHocVien);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Tìm kiếm LINQ phục vụ AJAX Search (Theo Slide 121 & Slide 389)
        public async Task<List<HocVien>> SearchAsync(string? keyword)
        {
            var query = _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(h =>
                    h.HoTen.ToLower().Contains(keyword) ||
                    h.Email.ToLower().Contains(keyword) ||
                    (h.SoDienThoai != null && h.SoDienThoai.Contains(keyword)) ||
                    (h.DiaChi != null && h.DiaChi.ToLower().Contains(keyword)));
            }

            return await query.OrderByDescending(h => h.NgayTao).ToListAsync();
        }

        public async Task<List<HocVien>> SearchTrangThaiAsync(bool? trangThai)
        {
            var query = _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                .AsQueryable();

            if (trangThai.HasValue)
            {
                query = query.Where(h => h.TrangThai == trangThai.Value);
            }

            return await query.OrderByDescending(h => h.NgayTao).ToListAsync();
        }

        public async Task<(IEnumerable<HocVien> Items, int TotalCount)> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize)
        {
            var query = _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(h =>
                    h.HoTen.ToLower().Contains(keyword) ||
                    h.Email.ToLower().Contains(keyword) ||
                    (h.SoDienThoai != null && h.SoDienThoai.Contains(keyword)));
            }

            if (trangThai.HasValue)
            {
                query = query.Where(h => h.TrangThai == trangThai.Value);
            }

            int total = await query.CountAsync();
            var items = await query
                .OrderByDescending(h => h.NgayTao)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
