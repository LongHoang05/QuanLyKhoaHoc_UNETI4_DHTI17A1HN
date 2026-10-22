// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// ========================================================

using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public class ChuongTrinhDaoTaoRepository : IChuongTrinhDaoTaoRepository
    {
        private readonly ApplicationDbContext _context;

        public ChuongTrinhDaoTaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChuongTrinhDaoTao>> GetAllAsync()
        {
            return await _context.ChuongTrinhDaoTaos
                .Include(c => c.LopHocs)
                .AsNoTracking()
                .OrderBy(c => c.TenChuongTrinh)
                .ToListAsync();
        }

        public async Task<ChuongTrinhDaoTao?> GetByIdAsync(int id)
        {
            return await _context.ChuongTrinhDaoTaos.FindAsync(id);
        }

        public async Task<ChuongTrinhDaoTao?> GetByIdWithLopHocsAsync(int id)
        {
            return await _context.ChuongTrinhDaoTaos
                .Include(c => c.LopHocs)
                .FirstOrDefaultAsync(c => c.MaChuongTrinh == id);
        }

        public async Task<(IEnumerable<ChuongTrinhDaoTao> Items, int TotalCount)> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize)
        {
            var query = _context.ChuongTrinhDaoTaos
                .Include(c => c.LopHocs)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(c => c.TenChuongTrinh.ToLower().Contains(kw) ||
                                         (c.MoTa != null && c.MoTa.ToLower().Contains(kw)) ||
                                         (c.EmailLienHe != null && c.EmailLienHe.ToLower().Contains(kw)));
            }

            if (trangThai.HasValue)
            {
                query = query.Where(c => c.TrangThai == trangThai.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.MaChuongTrinh)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(ChuongTrinhDaoTao item)
        {
            await _context.ChuongTrinhDaoTaos.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChuongTrinhDaoTao item)
        {
            _context.ChuongTrinhDaoTaos.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.ChuongTrinhDaoTaos.FindAsync(id);
            if (item != null)
            {
                _context.ChuongTrinhDaoTaos.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsTenChuongTrinhAsync(string tenChuongTrinh, int? excludeId = null)
        {
            var clean = tenChuongTrinh.Trim().ToLower();
            return await _context.ChuongTrinhDaoTaos.AnyAsync(c => 
                c.TenChuongTrinh.ToLower() == clean && (!excludeId.HasValue || c.MaChuongTrinh != excludeId.Value));
        }

        public async Task<bool> CoLopHocLienQuanAsync(int maChuongTrinh)
        {
            return await _context.LopHocs.AnyAsync(l => l.MaChuongTrinh == maChuongTrinh);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ChuongTrinhDaoTaos.AnyAsync(c => c.MaChuongTrinh == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
