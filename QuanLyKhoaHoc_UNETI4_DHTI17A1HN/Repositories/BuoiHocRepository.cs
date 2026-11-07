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
    public class BuoiHocRepository : IBuoiHocRepository
    {
        private readonly ApplicationDbContext _context;

        public BuoiHocRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BuoiHoc>> GetAllAsync()
        {
            return await _context.BuoiHocs
                .Include(b => b.LopHoc)
                .Include(b => b.GiangVienObj)
                .AsNoTracking()
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();
        }

        public async Task<BuoiHoc?> GetByIdAsync(int id)
        {
            return await _context.BuoiHocs.FindAsync(id);
        }

        public async Task<BuoiHoc?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.BuoiHocs
                .Include(b => b.LopHoc)
                    .ThenInclude(l => l!.ChuongTrinhDaoTao)
                .Include(b => b.GiangVienObj)
                .FirstOrDefaultAsync(b => b.MaBuoiHoc == id);
        }

        public async Task AddAsync(BuoiHoc item)
        {
            await _context.BuoiHocs.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BuoiHoc item)
        {
            _context.BuoiHocs.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.BuoiHocs.FindAsync(id);
            if (item != null)
            {
                _context.BuoiHocs.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.BuoiHocs.AnyAsync(b => b.MaBuoiHoc == id);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<BuoiHoc> Items, int TotalCount)> GetPagedAsync(
            string? search,
            int? maLop,
            string? phongHoc,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sortBy,
            int page,
            int pageSize)
        {
            var query = _context.BuoiHocs
                .Include(b => b.LopHoc)
                    .ThenInclude(l => l!.ChuongTrinhDaoTao)
                .Include(b => b.GiangVienObj)
                .AsQueryable();

            // 1. Tìm kiếm LINQ
            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(b =>
                    (b.PhongHoc != null && b.PhongHoc.ToLower().Contains(s)) ||
                    (b.NoiDung != null && b.NoiDung.ToLower().Contains(s)) ||
                    (b.GiangVien != null && b.GiangVien.ToLower().Contains(s)) ||
                    (b.LopHoc != null && b.LopHoc.TenLop.ToLower().Contains(s)));
            }

            // 2. Lọc theo Lớp học
            if (maLop.HasValue && maLop.Value > 0)
            {
                query = query.Where(b => b.MaLopHoc == maLop.Value);
            }

            // 3. Lọc theo Phòng học
            if (!string.IsNullOrWhiteSpace(phongHoc))
            {
                query = query.Where(b => b.PhongHoc == phongHoc);
            }

            // 4. Lọc theo Trạng thái buổi học
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(b => b.TrangThai == trangThai);
            }

            // 5. Lọc theo Thời gian
            if (tuNgay.HasValue)
            {
                query = query.Where(b => b.ThoiGianBatDau >= tuNgay.Value.Date);
            }
            if (denNgay.HasValue)
            {
                DateTime endOfDay = denNgay.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(b => b.ThoiGianBatDau <= endOfDay);
            }

            // 6. Sắp xếp LINQ
            switch (sortBy)
            {
                case "thoigian_desc":
                    query = query.OrderByDescending(b => b.ThoiGianBatDau);
                    break;
                case "phong_asc":
                    query = query.OrderBy(b => b.PhongHoc);
                    break;
                case "lop_asc":
                    query = query.OrderBy(b => b.LopHoc != null ? b.LopHoc.TenLop : "");
                    break;
                case "thoigian_asc":
                    query = query.OrderBy(b => b.ThoiGianBatDau);
                    break;
                default:
                    query = query.OrderByDescending(b => b.ThoiGianBatDau);
                    break;
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }

        public async Task<(int TongSoBuoi, int DaHoanThanh, int DaLenLich, int DaHuy)> GetThongKeBuoiHocAsync()
        {
            int tong = await _context.BuoiHocs.CountAsync();
            int hoanThanh = await _context.BuoiHocs.CountAsync(b => b.TrangThai == "Đã hoàn thành");
            int lenLich = await _context.BuoiHocs.CountAsync(b => b.TrangThai == "Đã lên lịch");
            int daHuy = await _context.BuoiHocs.CountAsync(b => b.TrangThai == "Đã hủy");

            return (tong, hoanThanh, lenLich, daHuy);
        }

        public async Task<int> CountBuoiHocChuaHuyAsync(int maLopHoc)
        {
            return await _context.BuoiHocs.CountAsync(b => b.MaLopHoc == maLopHoc && b.TrangThai != "Đã hủy");
        }

        public async Task<Dictionary<int, int>> GetBuoiHocChuaHuyMapAsync(IEnumerable<int> lopIds)
        {
            return await _context.BuoiHocs
                .Where(b => lopIds.Contains(b.MaLopHoc) && b.TrangThai != "Đã hủy")
                .GroupBy(b => b.MaLopHoc)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);
        }

        public async Task<bool> KiemTraTrungLichPhongAsync(string phongHoc, DateTime batDau, DateTime ketThuc, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(phongHoc)) return false;

            return await _context.BuoiHocs.AnyAsync(b =>
                (!excludeId.HasValue || b.MaBuoiHoc != excludeId.Value) &&
                b.PhongHoc == phongHoc &&
                b.TrangThai != "Đã hủy" &&
                b.ThoiGianBatDau < ketThuc &&
                b.ThoiGianKetThuc > batDau);
        }

        public async Task<bool> KiemTraTrungLichGiangVienAsync(int maGiangVien, DateTime batDau, DateTime ketThuc, int? excludeId = null)
        {
            return await _context.BuoiHocs.AnyAsync(b =>
                (!excludeId.HasValue || b.MaBuoiHoc != excludeId.Value) &&
                b.MaGiangVien == maGiangVien &&
                b.TrangThai != "Đã hủy" &&
                b.ThoiGianBatDau < ketThuc &&
                b.ThoiGianKetThuc > batDau);
        }

        private static string NormalizePhong(string? p)
        {
            if (string.IsNullOrWhiteSpace(p)) return string.Empty;
            var s = p.Trim().ToLower();
            if (s.StartsWith("phòng ")) s = s.Substring(6).Trim();
            if (s.StartsWith("p.")) s = s.Substring(2).Trim();
            if (s.StartsWith("p ")) s = s.Substring(2).Trim();
            return s.Replace(" ", "");
        }

        private static string NormalizeTen(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            var s = name.Trim().ToLower();
            string[] prefixes = { "pgs.ts.", "pgs.ts", "pgs.", "ts.", "ts", "ths.", "ths", "thạc sĩ", "tiến sĩ", "gv.", "gv" };
            foreach (var pre in prefixes)
            {
                if (s.StartsWith(pre))
                {
                    s = s.Substring(pre.Length).Trim();
                    break;
                }
            }
            return s.Replace(" ", "");
        }

        public async Task<(bool HasConflict, string? ConflictMessage, BuoiHoc? ConflictingSession)> KiemTraTrungLichChiTietAsync(
            int maLopHoc, string phongHoc, string? tenGiangVien, DateTime batDau, DateTime ketThuc, int? excludeId = null)
        {
            var cleanPhong = NormalizePhong(phongHoc);
            var cleanGv = NormalizeTen(tenGiangVien);

            var activeSessions = await _context.BuoiHocs
                .Include(b => b.LopHoc)
                .Where(b => (!excludeId.HasValue || b.MaBuoiHoc != excludeId.Value) && b.TrangThai != "Đã hủy")
                .ToListAsync();

            foreach (var b in activeSessions)
            {
                // Kiểm tra khoảng thời gian giao nhau: batDau < b.ThoiGianKetThuc && ketThuc > b.ThoiGianBatDau
                bool isTimeOverlap = (batDau < b.ThoiGianKetThuc && ketThuc > b.ThoiGianBatDau);
                if (!isTimeOverlap) continue;

                var existingPhong = NormalizePhong(b.PhongHoc);
                var existingGv = NormalizeTen(b.GiangVien);
                string tenLop = b.LopHoc?.TenLop ?? $"Lớp #{b.MaLopHoc}";

                if (b.MaLopHoc == maLopHoc)
                {
                    return (true, $"Trùng lịch lớp học: Lớp '{tenLop}' đã có ca học từ {b.ThoiGianBatDau:HH:mm} đến {b.ThoiGianKetThuc:HH:mm} ngày {b.ThoiGianBatDau:dd/MM/yyyy} (Phòng: {b.PhongHoc})!", b);
                }

                if (!string.IsNullOrEmpty(cleanPhong) && cleanPhong == existingPhong)
                {
                    return (true, $"Trùng lịch phòng học: Phòng '{b.PhongHoc}' đã có lớp '{tenLop}' (GV: {b.GiangVien}) sử dụng từ {b.ThoiGianBatDau:HH:mm} đến {b.ThoiGianKetThuc:HH:mm} ngày {b.ThoiGianBatDau:dd/MM/yyyy}!", b);
                }

                if (!string.IsNullOrEmpty(cleanGv) && !string.IsNullOrEmpty(existingGv) && (cleanGv == existingGv || cleanGv.Contains(existingGv) || existingGv.Contains(cleanGv)))
                {
                    return (true, $"Trùng lịch giảng viên: Giảng viên '{b.GiangVien}' đã có ca dạy lớp '{tenLop}' từ {b.ThoiGianBatDau:HH:mm} đến {b.ThoiGianKetThuc:HH:mm} ngày {b.ThoiGianBatDau:dd/MM/yyyy} (Phòng: {b.PhongHoc})!", b);
                }
            }

            return (false, null, null);
        }

        public async Task<bool> KiemTraTrungLichBuoiHocAsync(int maLopHoc, string phongHoc, string? tenGiangVien, DateTime batDau, DateTime ketThuc, int? excludeId = null)
        {
            var res = await KiemTraTrungLichChiTietAsync(maLopHoc, phongHoc, tenGiangVien, batDau, ketThuc, excludeId);
            return res.HasConflict;
        }

        public async Task<(HashSet<int> ConflictIds, int SoLoiTrung)> GetConflictSessionsAsync()
        {
            var allActive = await _context.BuoiHocs.Where(b => b.TrangThai != "Đã hủy").ToListAsync();
            var conflictIds = new HashSet<int>();
            for (int i = 0; i < allActive.Count; i++)
            {
                for (int j = i + 1; j < allActive.Count; j++)
                {
                    var b1 = allActive[i];
                    var b2 = allActive[j];
                    bool timeOverlap = (b1.ThoiGianBatDau < b2.ThoiGianKetThuc && b1.ThoiGianKetThuc > b2.ThoiGianBatDau);
                    if (!timeOverlap) continue;

                    bool sameClass = (b1.MaLopHoc == b2.MaLopHoc);
                    bool sameRoom = (!string.IsNullOrEmpty(b1.PhongHoc) && NormalizePhong(b1.PhongHoc) == NormalizePhong(b2.PhongHoc));
                    bool sameTeacher = (!string.IsNullOrEmpty(b1.GiangVien) && NormalizeTen(b1.GiangVien) == NormalizeTen(b2.GiangVien));

                    if (sameClass || sameRoom || sameTeacher)
                    {
                        conflictIds.Add(b1.MaBuoiHoc);
                        conflictIds.Add(b2.MaBuoiHoc);
                    }
                }
            }
            return (conflictIds, conflictIds.Count);
        }

        public async Task<int> XuLyCaTrungLichTuDongAsync()
        {
            var allActive = await _context.BuoiHocs
                .Where(b => b.TrangThai != "Đã hủy")
                .OrderBy(b => b.MaBuoiHoc)
                .ToListAsync();

            int removedCount = 0;
            var validList = new List<BuoiHoc>();

            foreach (var b in allActive)
            {
                bool hasConflict = false;
                foreach (var v in validList)
                {
                    bool timeOverlap = (b.ThoiGianBatDau < v.ThoiGianKetThuc && b.ThoiGianKetThuc > v.ThoiGianBatDau);
                    if (!timeOverlap) continue;

                    bool sameClass = (b.MaLopHoc == v.MaLopHoc);
                    bool sameRoom = (!string.IsNullOrEmpty(b.PhongHoc) && NormalizePhong(b.PhongHoc) == NormalizePhong(v.PhongHoc));
                    bool sameTeacher = (!string.IsNullOrEmpty(b.GiangVien) && NormalizeTen(b.GiangVien) == NormalizeTen(v.GiangVien));

                    if (sameClass || sameRoom || sameTeacher)
                    {
                        hasConflict = true;
                        break;
                    }
                }

                if (hasConflict)
                {
                    b.TrangThai = "Đã hủy";
                    b.GhiChu = (b.GhiChu + " (Tự động hủy do xung đột trùng lịch)").Trim();
                    removedCount++;
                }
                else
                {
                    validList.Add(b);
                }
            }

            if (removedCount > 0)
            {
                await _context.SaveChangesAsync();
            }

            return removedCount;
        }

        public async Task<bool> HasLichHocLopChuaHuyAsync(int maLopHoc)
        {
            return await _context.BuoiHocs.AnyAsync(b => b.MaLopHoc == maLopHoc && b.TrangThai != "Đã hủy");
        }

        public async Task<IEnumerable<BuoiHoc>> GetByLopHocIdAsync(int maLopHoc)
        {
            return await _context.BuoiHocs
                .Include(b => b.GiangVienObj)
                .Include(b => b.LopHoc)
                .Where(b => b.MaLopHoc == maLopHoc)
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();
        }

        public async Task<IEnumerable<BuoiHoc>> GetBuoiHocsByLopIdsAsync(IEnumerable<int> lopHocIds)
        {
            return await _context.BuoiHocs
                .Include(b => b.LopHoc)
                .Include(b => b.GiangVienObj)
                .Where(b => lopHocIds.Contains(b.MaLopHoc))
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();
        }

        public async Task<BuoiHoc?> GetNextBuoiHocForLopAsync(int maLopHoc)
        {
            DateTime now = DateTime.Now;
            return await _context.BuoiHocs
                .Where(b => b.MaLopHoc == maLopHoc && b.ThoiGianBatDau >= now && b.TrangThai != "Đã hủy")
                .OrderBy(b => b.ThoiGianBatDau)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CountCompletedSessionsForLopAsync(int maLopHoc)
        {
            return await _context.BuoiHocs
                .CountAsync(b => b.MaLopHoc == maLopHoc && b.TrangThai == "Đã hoàn thành");
        }
    }
}
