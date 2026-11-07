// ========================================================
// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Phụ trách: Module 5 - Buổi học, Điểm danh, Kết quả học tập & Thống kê báo cáo
// Bài tập thực hành: Repository Pattern - UNETI
// ========================================================

using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public class KetQuaHocTapRepository : IKetQuaHocTapRepository
    {
        private readonly ApplicationDbContext _context;

        public KetQuaHocTapRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KetQuaHocTap>> GetAllAsync()
        {
            return await _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d!.HocVien)
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d!.LopHoc)
                .AsNoTracking()
                .OrderByDescending(k => k.NgayCapNhat)
                .ToListAsync();
        }

        public async Task<KetQuaHocTap?> GetByIdAsync(int id)
        {
            return await _context.KetQuaHocTaps.FindAsync(id);
        }

        public async Task<KetQuaHocTap?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d!.HocVien)
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d!.LopHoc)
                .FirstOrDefaultAsync(k => k.MaKetQua == id);
        }

        public async Task AddAsync(KetQuaHocTap item)
        {
            await _context.KetQuaHocTaps.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KetQuaHocTap item)
        {
            _context.KetQuaHocTaps.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.KetQuaHocTaps.FindAsync(id);
            if (item != null)
            {
                _context.KetQuaHocTaps.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.KetQuaHocTaps.AnyAsync(k => k.MaKetQua == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<KetQuaHocTap> Items, int TotalCount)> GetPagedAsync(
            string? search,
            int? maLop,
            string? xepLoai,
            string? ketQua,
            int page,
            int pageSize,
            string? sortBy = null)
        {
            var query = _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d!.HocVien)
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d!.LopHoc)
                .AsQueryable();

            // 1. Tìm kiếm LINQ theo Tên học viên, Mã học viên, Tên lớp
            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(k =>
                    (k.DangKyHoc != null && k.DangKyHoc.HocVien != null && k.DangKyHoc.HocVien.HoTen.ToLower().Contains(s)) ||
                    (k.DangKyHoc != null && k.DangKyHoc.HocVien != null && k.DangKyHoc.HocVien.MaHocVien.ToString().Contains(s)) ||
                    (k.DangKyHoc != null && k.DangKyHoc.LopHoc != null && k.DangKyHoc.LopHoc.TenLop.ToLower().Contains(s)));
            }

            // 2. Lọc theo Lớp học
            if (maLop.HasValue && maLop.Value > 0)
            {
                query = query.Where(k => k.DangKyHoc != null && k.DangKyHoc.MaLopHoc == maLop.Value);
            }

            // 3. Lọc theo Xếp loại (Dịch trực tiếp sang SQL dựa trên DiemTongKet do XepLoai là [NotMapped])
            if (!string.IsNullOrWhiteSpace(xepLoai))
            {
                string xl = xepLoai.Trim();
                if (xl == "Xuất sắc")
                    query = query.Where(k => k.DiemTongKet >= 8.5);
                else if (xl == "Giỏi")
                    query = query.Where(k => k.DiemTongKet >= 8.0 && k.DiemTongKet < 8.5);
                else if (xl == "Khá")
                    query = query.Where(k => k.DiemTongKet >= 7.0 && k.DiemTongKet < 8.0);
                else if (xl == "Trung bình khá")
                    query = query.Where(k => k.DiemTongKet >= 6.5 && k.DiemTongKet < 7.0);
                else if (xl == "Trung bình")
                    query = query.Where(k => k.DiemTongKet >= 5.0 && k.DiemTongKet < 7.0);
                else if (xl == "Trung bình yếu")
                    query = query.Where(k => k.DiemTongKet >= 5.0 && k.DiemTongKet < 5.5);
                else if (xl == "Yếu" || xl == "Kém")
                    query = query.Where(k => k.DiemTongKet < 5.0);
            }

            // 4. Lọc theo Kết quả (Hoàn thành / Chưa hoàn thành)
            if (!string.IsNullOrWhiteSpace(ketQua))
            {
                query = query.Where(k => k.KetQua == ketQua);
            }

            int totalCount = await query.CountAsync();

            query = sortBy switch
            {
                "diem_desc" => query.OrderByDescending(k => k.DiemTongKet),
                "diem_asc" => query.OrderBy(k => k.DiemTongKet),
                "ten_asc" => query.OrderBy(k => k.DangKyHoc != null && k.DangKyHoc.HocVien != null ? k.DangKyHoc.HocVien.HoTen : ""),
                "ten_desc" => query.OrderByDescending(k => k.DangKyHoc != null && k.DangKyHoc.HocVien != null ? k.DangKyHoc.HocVien.HoTen : ""),
                "quatrinh_desc" => query.OrderByDescending(k => k.DiemChuyenCan),
                "cuoiky_desc" => query.OrderByDescending(k => k.DiemCuoiKhoa),
                _ => query.OrderByDescending(k => k.NgayCapNhat)
            };

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(int TongSoDiem, int SoDat, int SoKhongDat, double DiemTrungBinh)> GetThongKePhoiDiemAsync()
        {
            var allScores = await _context.KetQuaHocTaps
                .Select(k => new { k.DiemTongKet, k.KetQua })
                .ToListAsync();

            int tong = allScores.Count;
            int dat = allScores.Count(k => k.KetQua == "Hoàn thành" || k.DiemTongKet >= 5.0);
            int khongDat = tong - dat;
            double dtb = tong > 0 ? (double)allScores.Average(k => k.DiemTongKet) : 0;

            return (tong, dat, khongDat, dtb);
        }

        public async Task<bool> ExistsByDangKyIdAsync(int maDangKy)
        {
            return await _context.KetQuaHocTaps.AnyAsync(k => k.MaDangKy == maDangKy);
        }

        public async Task<KetQuaHocTap?> GetByDangKyIdAsync(int maDangKy)
        {
            return await _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d!.HocVien)
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d!.LopHoc)
                .FirstOrDefaultAsync(k => k.MaDangKy == maDangKy);
        }

        public async Task<IEnumerable<DangKyHoc>> GetEligibleDangKysForGradingAsync(int? maLopHoc = null)
        {
            var query = _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .Include(d => d.KetQuaHocTap)
                .Where(d => (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học") && d.KetQuaHocTap == null);

            if (maLopHoc.HasValue && maLopHoc.Value > 0)
            {
                query = query.Where(d => d.MaLopHoc == maLopHoc.Value);
            }

            return await query.OrderBy(d => d.HocVien != null ? d.HocVien.HoTen : "").ToListAsync();
        }

        public async Task<IEnumerable<KetQuaHocTap>> GetByHocVienIdAsync(int maHocVien)
        {
            return await _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d!.LopHoc)
                        .ThenInclude(l => l!.ChuongTrinhDaoTao)
                .Where(k => k.DangKyHoc != null && k.DangKyHoc.MaHocVien == maHocVien)
                .OrderByDescending(k => k.NgayCapNhat)
                .ToListAsync();
        }

        public async Task<IEnumerable<KetQuaHocTap>> GetByLopIdAsync(int maLopHoc)
        {
            return await _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d!.HocVien)
                .Where(k => k.DangKyHoc != null && k.DangKyHoc.MaLopHoc == maLopHoc)
                .ToListAsync();
        }
    }
}
