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
    public class TaiKhoanRepository : ITaiKhoanRepository
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaiKhoan?> GetByIdAsync(int id)
        {
            return await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);
        }

        public async Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap)
        {
            return await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap.ToLower() == tenDangNhap.ToLower().Trim());
        }

        public async Task<TaiKhoan?> GetByEmailAsync(string email)
        {
            return await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.Email.ToLower() == email.ToLower().Trim());
        }

        public async Task<List<TaiKhoan>> GetAllAsync()
        {
            return await _context.TaiKhoans
                .AsNoTracking()
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();
        }

        public async Task<(IEnumerable<TaiKhoan> Items, int TotalCount)> GetPagedAsync(
            string? keyword, string? vaiTro, bool? trangThai, int pageIndex, int pageSize)
        {
            var query = _context.TaiKhoans.AsNoTracking().AsQueryable();

            // 1. Tìm kiếm LINQ theo tên đăng nhập, họ tên hoặc email
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(t => t.TenDangNhap.ToLower().Contains(kw) ||
                                         t.HoTen.ToLower().Contains(kw) ||
                                         t.Email.ToLower().Contains(kw));
            }

            // 2. Lọc theo vai trò (Admin, GiaoVu, HocVien)
            if (!string.IsNullOrWhiteSpace(vaiTro))
            {
                query = query.Where(t => t.VaiTro == vaiTro);
            }

            // 3. Lọc theo trạng thái hoạt động
            if (trangThai.HasValue)
            {
                query = query.Where(t => t.TrangThai == trangThai.Value);
            }

            var totalCount = await query.CountAsync();

            // 4. Phân trang LINQ Skip & Take trên máy chủ CSDL
            var items = await query
                .OrderByDescending(t => t.MaTaiKhoan)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(TaiKhoan taiKhoan)
        {
            await _context.TaiKhoans.AddAsync(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaiKhoan taiKhoan)
        {
            _context.TaiKhoans.Update(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tk = await _context.TaiKhoans.FindAsync(id);
            if (tk != null)
            {
                _context.TaiKhoans.Remove(tk);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByTenDangNhapAsync(string tenDangNhap, int? excludeId = null)
        {
            var clean = tenDangNhap.Trim().ToLower();
            return await _context.TaiKhoans.AnyAsync(t => 
                t.TenDangNhap.ToLower() == clean && (!excludeId.HasValue || t.MaTaiKhoan != excludeId.Value));
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
        {
            var clean = email.Trim().ToLower();
            return await _context.TaiKhoans.AnyAsync(t => 
                t.Email.ToLower() == clean && (!excludeId.HasValue || t.MaTaiKhoan != excludeId.Value));
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TaiKhoans.AnyAsync(t => t.MaTaiKhoan == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
