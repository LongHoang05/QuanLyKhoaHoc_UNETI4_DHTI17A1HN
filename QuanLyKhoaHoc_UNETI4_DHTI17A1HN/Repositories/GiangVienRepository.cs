// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Quản lý Dữ liệu dùng chung (Giảng viên)
// ========================================================

using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public class GiangVienRepository : IGiangVienRepository
    {
        private readonly ApplicationDbContext _context;

        public GiangVienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GiangVien>> GetAllAsync()
        {
            return await _context.GiangViens
                .AsNoTracking()
                .OrderBy(g => g.HoTen)
                .ToListAsync();
        }

        public async Task<GiangVien?> GetByIdAsync(int id)
        {
            return await _context.GiangViens.FindAsync(id);
        }

        public async Task<GiangVien?> GetByIdWithLopHocsAsync(int id)
        {
            return await _context.GiangViens
                .Include(g => g.LopHocs)
                .FirstOrDefaultAsync(g => g.MaGiangVien == id);
        }

        public async Task<(IEnumerable<GiangVien> Items, int TotalCount)> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize)
        {
            var query = _context.GiangViens
                .Include(g => g.LopHocs)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(g => g.HoTen.ToLower().Contains(kw) ||
                                         g.Email.ToLower().Contains(kw) ||
                                         (g.ChuyenNganh != null && g.ChuyenNganh.ToLower().Contains(kw)) ||
                                         (g.SoDienThoai != null && g.SoDienThoai.Contains(kw)));
            }

            if (trangThai.HasValue)
            {
                query = query.Where(g => g.TrangThai == trangThai.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(g => g.MaGiangVien)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(GiangVien giangVien)
        {
            await _context.GiangViens.AddAsync(giangVien);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GiangVien giangVien)
        {
            _context.GiangViens.Update(giangVien);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var gv = await _context.GiangViens.FindAsync(id);
            if (gv != null)
            {
                _context.GiangViens.Remove(gv);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsEmailAsync(string email, int? excludeId = null)
        {
            var clean = email.Trim().ToLower();
            return await _context.GiangViens.AnyAsync(g => 
                g.Email.ToLower() == clean && (!excludeId.HasValue || g.MaGiangVien != excludeId.Value));
        }

        public async Task<bool> CoLopHocLienQuanAsync(int maGiangVien)
        {
            return await _context.LopHocs.AnyAsync(l => l.MaGiangVien == maGiangVien);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.GiangViens.AnyAsync(g => g.MaGiangVien == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
