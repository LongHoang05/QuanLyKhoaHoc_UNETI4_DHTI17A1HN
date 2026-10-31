using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh
    // Mã sinh viên: 23103100042
    // Phụ trách: Module 3 - Quản lý học viên, Hồ sơ cá nhân và Đăng ký học
    // ========================================================
    public class HocVienRepository : IHocVienRepository
    {
        private readonly ApplicationDbContext _context;

        public HocVienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region 1. Nghiệp vụ Thực thể Học Viên (Entity HocVien)
        public List<HocVien> GetAll()
        {
            return _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                    .ThenInclude(d => d.LopHoc)
                .OrderByDescending(h => h.NgayTao)
                .ToList();
        }

        public async Task<List<HocVien>> GetAllAsync()
        {
            return await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                    .ThenInclude(d => d.LopHoc)
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

        // Tìm kiếm và Lọc học viên LINQ đa năng: Từ khóa + Trạng thái + Lọc theo Lớp Học
        public async Task<List<HocVien>> SearchHocVienAsync(string? keyword, bool? trangThai = null, int? maLopHoc = null)
        {
            var query = _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyHocs)
                    .ThenInclude(d => d.LopHoc)
                .AsQueryable();

            // 1. Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(h =>
                    h.HoTen.ToLower().Contains(keyword) ||
                    (h.SoDienThoai != null && h.SoDienThoai.Contains(keyword)) ||
                    (h.DiaChi != null && h.DiaChi.ToLower().Contains(keyword)) ||
                    (h.TaiKhoan != null && h.TaiKhoan.TenDangNhap.ToLower().Contains(keyword)) ||
                    EF.Functions.Like(h.Email, $"{keyword}%") ||
                    EF.Functions.Like(h.Email, $"%{keyword}%@%"));
            }

            // 2. Lọc theo trạng thái hoạt động (true: Đang hoạt động, false: Đang bị khóa)
            if (trangThai.HasValue)
            {
                query = query.Where(h => h.TrangThai == trangThai.Value);
            }

            // 3. ĐẶC BIỆT: Lọc theo lớp học học viên đang đăng ký
            if (maLopHoc.HasValue && maLopHoc.Value > 0)
            {
                query = query.Where(h => h.DangKyHocs.Any(d => d.MaLopHoc == maLopHoc.Value));
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
        #endregion

        #region 2. Nghiệp vụ Thực thể Tài Khoản (Entity TaiKhoan) & Lớp Học
        public async Task<List<TaiKhoan>> GetAllTaiKhoanAsync()
        {
            return await _context.TaiKhoans
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();
        }

        public async Task<List<TaiKhoan>> SearchTaiKhoanAsync(string? keyword, string? vaiTro = null, bool? trangThai = null)
        {
            var query = _context.TaiKhoans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(t =>
                    t.TenDangNhap.ToLower().Contains(keyword) ||
                    t.HoTen.ToLower().Contains(keyword) ||
                    t.Email.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(vaiTro))
            {
                query = query.Where(t => t.VaiTro == vaiTro);
            }

            if (trangThai.HasValue)
            {
                query = query.Where(t => t.TrangThai == trangThai.Value);
            }

            return await query.OrderByDescending(t => t.NgayTao).ToListAsync();
        }

        public async Task<TaiKhoan?> GetTaiKhoanByIdAsync(int id)
        {
            return await _context.TaiKhoans.FindAsync(id);
        }

        public async Task<bool> ExistsUsernameAsync(string username)
        {
            var cleanUsername = username.Trim().ToLower();
            return await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == cleanUsername);
        }

        public async Task AddTaiKhoanAsync(TaiKhoan taiKhoan)
        {
            await _context.TaiKhoans.AddAsync(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaiKhoanAsync(TaiKhoan taiKhoan)
        {
            _context.TaiKhoans.Update(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LopHoc>> GetAllLopHocAsync()
        {
            return await _context.LopHocs
                .OrderBy(l => l.TenLop)
                .ToListAsync();
        }
        #endregion
    }
}
