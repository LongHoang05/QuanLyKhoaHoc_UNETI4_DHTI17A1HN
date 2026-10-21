using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh - MSV: 23103100042
    // Module 3: Cài đặt HocVienRepository
    // Chuẩn theo giáo trình bài giảng khoa CNTT UNETI (Slide 107, 121, 389)
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
                        .ThenInclude(l => l!.ChuongTrinhDaoTao)
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
        // Tìm kiếm LINQ phục vụ AJAX Search (Theo Slide 121 & Slide 389)
        public async Task<List<HocVien>> SearchAsync(string? keyword, string? searchType = null)
        {
            var query = _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();

                if (searchType == "name")
                {
                    // Tìm kiếm riêng theo Họ và Tên
                    query = query.Where(h => h.HoTen.ToLower().Contains(keyword));
                }
                else if (searchType == "email")
                {
                    // Tìm kiếm riêng theo Email
                    query = query.Where(h => h.Email.ToLower().Contains(keyword));
                }
                else if (searchType == "phone")
                {
                    // Tìm kiếm riêng theo Số điện thoại
                    query = query.Where(h => h.SoDienThoai != null && h.SoDienThoai.Contains(keyword));
                }
                else
                {
                    // Tìm kiếm thông minh tổng hợp (Tránh tình trạng tìm chữ 'mai' bị dính đuôi 'gmail.com')
                    if (keyword.Contains("@"))
                    {
                        query = query.Where(h =>
                            h.Email.ToLower().Contains(keyword) ||
                            h.HoTen.ToLower().Contains(keyword));
                    }
                    else
                    {
                        query = query.Where(h =>
                            h.HoTen.ToLower().Contains(keyword) ||
                            (h.SoDienThoai != null && h.SoDienThoai.Contains(keyword)) ||
                            (h.DiaChi != null && h.DiaChi.ToLower().Contains(keyword)) ||
                            EF.Functions.Like(h.Email, $"{keyword}%") ||
                            EF.Functions.Like(h.Email, $"%{keyword}%@%"));
                    }
                }
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
                .Include(h => h.DangKyHocs)
                .Include(h => h.TaiKhoan)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(h =>
                    h.HoTen.ToLower().Contains(kw) ||
                    h.Email.ToLower().Contains(kw) ||
                    (h.SoDienThoai != null && h.SoDienThoai.Contains(kw)) ||
                    (h.DiaChi != null && h.DiaChi.ToLower().Contains(kw)));
            }

            if (trangThai.HasValue)
            {
                query = query.Where(h => h.TrangThai == trangThai.Value);
            }

            query = query.OrderByDescending(h => h.NgayTao);
            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
