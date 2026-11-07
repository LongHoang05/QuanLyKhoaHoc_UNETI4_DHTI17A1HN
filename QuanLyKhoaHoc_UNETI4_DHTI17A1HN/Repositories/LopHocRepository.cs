// ========================================================
// Họ và tên: Nguyễn Đình Kiên
// Mã sinh viên: 23103100020
// Phụ trách: Module 2 - Quản lý lớp học, Tìm kiếm, Lọc, Sắp xếp và Phân trang
// Bài tập thực hành: Repository Pattern - UNETI
// ========================================================

using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public class LopHocRepository : ILopHocRepository
    {
        private readonly ApplicationDbContext _context;

        public LopHocRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LopHoc>> GetAllAsync()
        {
            return await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Include(l => l.DangKyHocs)
                .AsNoTracking()
                .OrderByDescending(l => l.NgayTao)
                .ToListAsync();
        }

        public async Task<LopHoc?> GetByIdAsync(int id)
        {
            return await _context.LopHocs.FindAsync(id);
        }

        public async Task<LopHoc?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Include(l => l.DangKyHocs)
                    .ThenInclude(d => d.HocVien)
                .Include(l => l.BuoiHocs)
                .FirstOrDefaultAsync(l => l.MaLopHoc == id);
        }

        public async Task AddAsync(LopHoc lopHoc)
        {
            await _context.LopHocs.AddAsync(lopHoc);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LopHoc lopHoc)
        {
            _context.LopHocs.Update(lopHoc);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var lopHoc = await _context.LopHocs.FindAsync(id);
            if (lopHoc != null)
            {
                _context.LopHocs.Remove(lopHoc);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.LopHocs.AnyAsync(l => l.MaLopHoc == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasRegistrationsAsync(int id)
        {
            return await _context.DangKyHocs.AnyAsync(d => d.MaLopHoc == id);
        }

        public async Task<int> CountEnrolledAsync(int id)
        {
            return await _context.DangKyHocs
                .CountAsync(d => d.MaLopHoc == id &&
                    (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học" || d.TrangThai == "Hoàn thành" || d.TrangThai == "Không hoàn thành"));
        }

        public async Task<bool> ExistsTenLopAsync(string tenLop, int? excludeId = null)
        {
            var clean = tenLop.Trim().ToLower();
            return await _context.LopHocs.AnyAsync(l =>
                l.TenLop.ToLower() == clean && (!excludeId.HasValue || l.MaLopHoc != excludeId.Value));
        }

        public async Task<IEnumerable<ChuongTrinhDaoTao>> GetChuongTrinhListAsync()
        {
            return await _context.ChuongTrinhDaoTaos
                .Where(c => c.TrangThai)
                .OrderBy(c => c.TenChuongTrinh)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetDanhSachTenGiangVienAsync()
        {
            return await _context.LopHocs
                .Where(l => !string.IsNullOrEmpty(l.TenGiangVien))
                .Select(l => l.TenGiangVien!)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
        }

        public async Task<(IEnumerable<LopHoc> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            int? maChuongTrinh,
            string? trinhDo,
            string? trangThai,
            string? giangVien,
            decimal? minHocPhi,
            decimal? maxHocPhi,
            string? filterHanDangKy,
            string? filterConCho,
            string? sortBy,
            int pageIndex,
            int pageSize)
        {
            var query = _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Include(l => l.DangKyHocs)
                .AsQueryable();

            // 1. TÌM KIẾM BẰNG LINQ (Tên lớp, Tên CTĐT, Tên GV)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(l =>
                    l.TenLop.ToLower().Contains(kw) ||
                    (l.ChuongTrinhDaoTao != null && l.ChuongTrinhDaoTao.TenChuongTrinh.ToLower().Contains(kw)) ||
                    (l.TenGiangVien != null && l.TenGiangVien.ToLower().Contains(kw)));
            }

            // 2. LỌC ĐA ĐIỀU KIỆN
            // 2.1. Lọc theo Chương trình đào tạo
            if (maChuongTrinh.HasValue && maChuongTrinh.Value > 0)
            {
                query = query.Where(l => l.MaChuongTrinh == maChuongTrinh.Value);
            }

            // 2.2. Lọc theo Trình độ đầu vào
            if (!string.IsNullOrWhiteSpace(trinhDo))
            {
                query = query.Where(l => l.TrinhDoDauVao == trinhDo);
            }

            // 2.3. Lọc theo Trạng thái lớp
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(l => l.TrangThai == trangThai);
            }

            // 2.4. Lọc theo Giảng viên
            if (!string.IsNullOrWhiteSpace(giangVien))
            {
                query = query.Where(l => l.TenGiangVien != null && l.TenGiangVien.Contains(giangVien));
            }

            // 2.5. Lọc theo Khoảng học phí
            if (minHocPhi.HasValue && minHocPhi.Value >= 0)
            {
                query = query.Where(l => l.HocPhi >= minHocPhi.Value);
            }
            if (maxHocPhi.HasValue && maxHocPhi.Value > 0)
            {
                query = query.Where(l => l.HocPhi <= maxHocPhi.Value);
            }

            // 2.6. Lọc theo Hạn đăng ký
            var today = DateTime.Today;
            if (filterHanDangKy == "con_han")
            {
                query = query.Where(l => l.HanDangKy >= today && l.NgayBatDauDangKy <= DateTime.Now);
            }
            else if (filterHanDangKy == "het_han")
            {
                query = query.Where(l => l.HanDangKy < today);
            }

            // 2.7. Lọc theo Tình trạng chỗ
            if (filterConCho == "con_cho")
            {
                query = query.Where(l => l.DangKyHocs.Count(d =>
                    d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học") < l.SiSoToiDa);
            }
            else if (filterConCho == "het_cho")
            {
                query = query.Where(l => l.DangKyHocs.Count(d =>
                    d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học") >= l.SiSoToiDa);
            }

            // 3. SẮP XẾP LINQ
            switch (sortBy)
            {
                case "ten_asc":
                    query = query.OrderBy(l => l.TenLop);
                    break;
                case "ten_desc":
                    query = query.OrderByDescending(l => l.TenLop);
                    break;
                case "hocphi_asc":
                    query = query.OrderBy(l => l.HocPhi);
                    break;
                case "hocphi_desc":
                    query = query.OrderByDescending(l => l.HocPhi);
                    break;
                case "han_asc":
                    query = query.OrderBy(l => l.HanDangKy);
                    break;
                case "han_desc":
                    query = query.OrderByDescending(l => l.HanDangKy);
                    break;
                case "khaigiang_asc":
                    query = query.OrderBy(l => l.NgayKhaiGiang);
                    break;
                case "khaigiang_desc":
                    query = query.OrderByDescending(l => l.NgayKhaiGiang);
                    break;
                default:
                    query = query.OrderByDescending(l => l.MaLopHoc);
                    break;
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }

        public async Task<(int TongSoLop, int SoLopDangMo, int SoLopDaDong, int SoLopTamDung)> GetThongKeNhanhAsync()
        {
            var tongSoLop = await _context.LopHocs.CountAsync();
            var soLopDangMo = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang mở đăng ký");
            var soLopDaDong = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đã kết thúc");
            var soLopTamDung = await _context.LopHocs.CountAsync(l => l.TrangThai == "Tạm dừng");

            return (tongSoLop, soLopDangMo, soLopDaDong, soLopTamDung);
        }

        public async Task<int> CountOpenClassesAsync()
        {
            return await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang mở đăng ký");
        }

        public async Task<int> CountTotalClassesAsync()
        {
            return await _context.LopHocs.CountAsync();
        }

        public async Task<IEnumerable<LopHoc>> GetHotCoursesAsync(int take = 4)
        {
            return await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Where(l => l.TrangThai == "Đang mở đăng ký")
                .OrderByDescending(l => l.DangKyHocs.Count)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<LopHoc>> GetUpcomingCoursesAsync(int take = 4)
        {
            return await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Where(l => l.TrangThai == "Đang mở đăng ký" && l.NgayKhaiGiang >= DateTime.Now)
                .OrderBy(l => l.NgayKhaiGiang)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<LopHoc>> GetSimilarCoursesAsync(int maChuongTrinh, int excludeMaLop, int take = 3)
        {
            return await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Where(l => l.MaChuongTrinh == maChuongTrinh && l.MaLopHoc != excludeMaLop && l.TrangThai == "Đang mở đăng ký")
                .Take(take)
                .ToListAsync();
        }

        public async Task<Dictionary<int, int>> GetOpenClassCountsByProgramAsync()
        {
            return await _context.LopHocs
                .Where(l => l.TrangThai == "Đang mở đăng ký")
                .GroupBy(l => l.MaChuongTrinh)
                .Select(g => new { MaChuongTrinh = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaChuongTrinh, x => x.Count);
        }

        // --- BÀI TẬP THỰC HÀNH JAVASCRIPT, JQUERY, AJAX (BÀI 11 - BÀI 15) ---
        public async Task<IEnumerable<LopHoc>> SearchAjaxAsync(string? keyword, int? maChuongTrinh = null)
        {
            var query = _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Include(l => l.GiangVien)
                .Include(l => l.DangKyHocs)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(l =>
                    l.TenLop.Contains(kw) ||
                    (l.ChuongTrinhDaoTao != null && l.ChuongTrinhDaoTao.TenChuongTrinh.Contains(kw)) ||
                    (l.TenGiangVien != null && l.TenGiangVien.Contains(kw)) ||
                    (l.GiangVien != null && l.GiangVien.HoTen.Contains(kw)) ||
                    (l.PhongHoc != null && l.PhongHoc.Contains(kw)));
            }

            if (maChuongTrinh.HasValue && maChuongTrinh.Value > 0)
            {
                query = query.Where(l => l.MaChuongTrinh == maChuongTrinh.Value);
            }

            return await query
                .OrderByDescending(l => l.NgayKhaiGiang)
                .Take(50)
                .ToListAsync();
        }
    }
}
