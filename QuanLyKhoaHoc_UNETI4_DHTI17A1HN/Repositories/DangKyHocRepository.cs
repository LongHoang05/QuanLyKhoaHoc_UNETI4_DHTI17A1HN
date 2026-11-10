// ========================================================
// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Phụ trách: Module 4 - Đăng ký khóa học, Quản lý hồ sơ & Thu học phí
// Bài tập thực hành: Repository Pattern - UNETI
// ========================================================

using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public class DangKyHocRepository : IDangKyHocRepository
    {
        private readonly ApplicationDbContext _context;

        public DangKyHocRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DangKyHoc>> GetAllAsync()
        {
            return await _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.ChuongTrinhDaoTao)
                .AsNoTracking()
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();
        }

        public async Task<DangKyHoc?> GetByIdAsync(int id)
        {
            return await _context.DangKyHocs.FindAsync(id);
        }

        public async Task<DangKyHoc?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.ChuongTrinhDaoTao)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);
        }

        public async Task AddAsync(DangKyHoc item)
        {
            await _context.DangKyHocs.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DangKyHoc item)
        {
            _context.DangKyHocs.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.DangKyHocs.FindAsync(id);
            if (item != null)
            {
                _context.DangKyHocs.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.DangKyHocs.AnyAsync(d => d.MaDangKy == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<DangKyHoc> Items, int TotalCount)> GetPagedAsync(
            string? search,
            int? maChuongTrinh,
            int? maLop,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sortBy,
            int page,
            int pageSize)
        {
            var query = _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.ChuongTrinhDaoTao)
                .AsQueryable();

            // 1. Tìm kiếm LINQ theo Tên học viên, Mã HV, Email, SĐT hoặc Tên lớp
            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(d =>
                    (d.HocVien != null && d.HocVien.HoTen.ToLower().Contains(s)) ||
                    (d.HocVien != null && d.HocVien.MaHocVien.ToString().Contains(s)) ||
                    (d.HocVien != null && d.HocVien.Email.ToLower().Contains(s)) ||
                    (d.HocVien != null && d.HocVien.SoDienThoai != null && d.HocVien.SoDienThoai.Contains(s)) ||
                    (d.LopHoc != null && d.LopHoc.TenLop.ToLower().Contains(s)));
            }

            // 2. Lọc theo Chương trình đào tạo
            if (maChuongTrinh.HasValue && maChuongTrinh.Value > 0)
            {
                query = query.Where(d => d.LopHoc != null && d.LopHoc.MaChuongTrinh == maChuongTrinh.Value);
            }

            // 3. Lọc theo Lớp học
            if (maLop.HasValue && maLop.Value > 0)
            {
                query = query.Where(d => d.MaLopHoc == maLop.Value);
            }

            // 4. Lọc theo Trạng thái hồ sơ
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(d => d.TrangThai == trangThai);
            }

            // 5. Lọc theo Khoảng thời gian đăng ký (Từ ngày - Đến ngày)
            if (tuNgay.HasValue)
            {
                query = query.Where(d => d.NgayDangKy >= tuNgay.Value.Date);
            }
            if (denNgay.HasValue)
            {
                DateTime endOfDay = denNgay.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.NgayDangKy <= endOfDay);
            }

            // 6. Sắp xếp LINQ đa tiêu chí
            switch (sortBy)
            {
                case "ngay_asc":
                    query = query.OrderBy(d => d.NgayDangKy);
                    break;
                case "ngay_desc":
                    query = query.OrderByDescending(d => d.NgayDangKy);
                    break;
                case "tien_desc":
                    query = query.OrderByDescending(d => d.SoTienDaDong);
                    break;
                case "tien_asc":
                    query = query.OrderBy(d => d.SoTienDaDong);
                    break;
                case "ten_asc":
                    query = query.OrderBy(d => d.HocVien != null ? d.HocVien.HoTen : "");
                    break;
                case "ten_desc":
                    query = query.OrderByDescending(d => d.HocVien != null ? d.HocVien.HoTen : "");
                    break;
                default:
                    query = query.OrderByDescending(d => d.MaDangKy);
                    break;
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }

        public async Task<(int ChoDuyetCount, int DaDuyetCount, double TongDoanhThu)> GetThongKeNhanhAsync()
        {
            int choDuyet = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ duyệt" || d.TrangThai == "Chờ xác nhận học phí");
            int daDuyet = await _context.DangKyHocs.CountAsync(d =>
                d.TrangThai == "Đã duyệt" ||
                d.TrangThai == "Chờ xác nhận học phí" ||
                d.TrangThai == "Đã đóng học phí" ||
                d.TrangThai == "Đang học" ||
                d.TrangThai == "Hoàn thành" ||
                d.TrangThai == "Không hoàn thành");
            double doanhThu = await _context.DangKyHocs.SumAsync(d => (double?)d.SoTienDaDong) ?? 0;

            return (choDuyet, daDuyet, doanhThu);
        }

        public async Task<int> CountEnrolledInLopAsync(int maLop)
        {
            return await _context.DangKyHocs.CountAsync(d =>
                d.MaLopHoc == maLop &&
                (d.TrangThai == "Đã duyệt" ||
                 d.TrangThai == "Chờ xác nhận học phí" ||
                 d.TrangThai == "Đã đóng học phí" ||
                 d.TrangThai == "Đang học" ||
                 d.TrangThai == "Hoàn thành" ||
                 d.TrangThai == "Không hoàn thành"));
        }

        public async Task<bool> HasEnrolledAsync(int maHocVien, int maLop)
        {
            return await _context.DangKyHocs.AnyAsync(d =>
                d.MaHocVien == maHocVien &&
                d.MaLopHoc == maLop &&
                d.TrangThai != "Đã hủy" &&
                d.TrangThai != "Từ chối");
        }

        public async Task<DangKyHoc?> GetActiveRegistrationAsync(int maHocVien, int maLop)
        {
            return await _context.DangKyHocs
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(d =>
                    d.MaHocVien == maHocVien &&
                    d.MaLopHoc == maLop &&
                    d.TrangThai != "Đã hủy" &&
                    d.TrangThai != "Từ chối");
        }

        public async Task<IEnumerable<DangKyHoc>> GetByHocVienIdAsync(int maHocVien)
        {
            return await _context.DangKyHocs
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.ChuongTrinhDaoTao)
                .Include(d => d.KetQuaHocTap)
                .Where(d => d.MaHocVien == maHocVien)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();
        }

        public async Task<IEnumerable<DangKyHoc>> GetPaymentHistoryByHocVienIdAsync(int maHocVien)
        {
            return await _context.DangKyHocs
                .Include(d => d.LopHoc)
                .Where(d => d.MaHocVien == maHocVien && d.NgayDongHocPhi != null)
                .OrderByDescending(d => d.NgayDongHocPhi)
                .ToListAsync();
        }

        public async Task<Dictionary<int, int>> GetSiSoDaDangKyMapAsync(IEnumerable<int> maLopList)
        {
            return await _context.DangKyHocs
                .Where(d => maLopList.Contains(d.MaLopHoc) &&
                            (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học" || d.TrangThai == "Hoàn thành" || d.TrangThai == "Không hoàn thành"))
                .GroupBy(d => d.MaLopHoc)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.MaLop, g => g.Count);
        }

        public async Task<Dictionary<int, int>> GetAllSiSoMapAsync()
        {
            return await _context.DangKyHocs
                .Where(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học" || d.TrangThai == "Hoàn thành" || d.TrangThai == "Không hoàn thành")
                .GroupBy(d => d.MaLopHoc)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.MaLop, g => g.Count);
        }

        public async Task<int> GetTotalCountExcludingCancelledAsync()
        {
            return await _context.DangKyHocs.CountAsync(d => d.TrangThai != "Đã hủy");
        }
    }
}
