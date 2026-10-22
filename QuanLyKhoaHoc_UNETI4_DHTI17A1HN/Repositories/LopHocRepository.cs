using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Họ và tên: Nguyễn Đình Kiên
    // Mã sinh viên: 23103100020
    // Phụ trách: Module 2 - Quản lý lớp học & Tìm kiếm, Lọc, Sắp xếp, Phân trang LINQ
    // Repository triển khai ILopHocRepository
    // ========================================================
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

        public async Task<bool> HasRegistrationsAsync(int id)
        {
            return await _context.DangKyHocs.AnyAsync(d => d.MaLopHoc == id);
        }

        public async Task<int> CountEnrolledAsync(int id)
        {
            return await _context.DangKyHocs
                .Where(d => d.MaLopHoc == id &&
                           (d.TrangThai == "Đã duyệt" ||
                            d.TrangThai == "Đã đóng học phí" ||
                            d.TrangThai == "Đang học" ||
                            d.TrangThai == "Hoàn thành"))
                .CountAsync();
        }

        public async Task<IEnumerable<ChuongTrinhDaoTao>> GetChuongTrinhListAsync()
        {
            return await _context.ChuongTrinhDaoTaos
                .Where(c => c.TrangThai)
                .OrderBy(c => c.TenChuongTrinh)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(IEnumerable<LopHoc> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            int? maChuongTrinh,
            string? trinhDo,
            string? trangThai,
            int? maGiangVien,
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
                var kw = keyword.Trim();
                query = query.Where(l =>
                    l.TenLop.Contains(kw) ||
                    (l.ChuongTrinhDaoTao != null && l.ChuongTrinhDaoTao.TenChuongTrinh.Contains(kw)) ||
                    (l.TenGiangVien != null && l.TenGiangVien.Contains(kw)));
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

            // 2.4. Lọc theo Giảng viên (MaGiangVien là [NotMapped] nên lọc theo TenGiangVien)
            // Ghi chú: maGiangVien param được nhận nhưng bộ lọc theo TenGiangVien
            // thông qua keyword search ở trên. Filter này được bỏ qua ở DB layer.

            // 2.5. Lọc theo Khoảng học phí
            if (minHocPhi.HasValue)
            {
                query = query.Where(l => l.HocPhi >= minHocPhi.Value);
            }
            if (maxHocPhi.HasValue)
            {
                query = query.Where(l => l.HocPhi <= maxHocPhi.Value);
            }

            // 2.6. Lọc theo Hạn đăng ký (Còn hạn / Hết hạn)
            var today = DateTime.Today;
            if (filterHanDangKy == "con_han")
            {
                query = query.Where(l => l.HanDangKy >= today);
            }
            else if (filterHanDangKy == "het_han")
            {
                query = query.Where(l => l.HanDangKy < today);
            }

            // 2.7. Lọc theo Còn chỗ / Hết chỗ
            if (filterConCho == "con_cho")
            {
                query = query.Where(l => l.DangKyHocs.Count(d =>
                    d.TrangThai == "Đã duyệt" ||
                    d.TrangThai == "Đã đóng học phí" ||
                    d.TrangThai == "Đang học" ||
                    d.TrangThai == "Hoàn thành") < l.SiSoToiDa);
            }
            else if (filterConCho == "het_cho")
            {
                query = query.Where(l => l.DangKyHocs.Count(d =>
                    d.TrangThai == "Đã duyệt" ||
                    d.TrangThai == "Đã đóng học phí" ||
                    d.TrangThai == "Đang học" ||
                    d.TrangThai == "Hoàn thành") >= l.SiSoToiDa);
            }

            // 3. SẮP XẾP LINQ
            query = sortBy switch
            {
                "ten_asc" => query.OrderBy(l => l.TenLop),
                "ten_desc" => query.OrderByDescending(l => l.TenLop),
                "hocphi_asc" => query.OrderBy(l => l.HocPhi),
                "hocphi_desc" => query.OrderByDescending(l => l.HocPhi),
                "handk_asc" => query.OrderBy(l => l.HanDangKy),
                "handk_desc" => query.OrderByDescending(l => l.HanDangKy),
                "khaigiang_asc" => query.OrderBy(l => l.NgayKhaiGiang),
                "khaigiang_desc" => query.OrderByDescending(l => l.NgayKhaiGiang),
                "ngaytao_asc" => query.OrderBy(l => l.NgayTao),
                _ => query.OrderByDescending(l => l.NgayTao)
            };

            // Đếm tổng số bản ghi thỏa mãn điều kiện
            int totalCount = await query.CountAsync();

            // 4. PHÂN TRANG TRÊN SERVER BẰNG SKIP() & TAKE()
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(int TongSoLop, int SoLopDangMo, int SoLopDaDong, int SoLopTamDung)> GetThongKeNhanhAsync()
        {
            var tongSoLop = await _context.LopHocs.CountAsync();
            var soLopDangMo = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang mở đăng ký");
            var soLopDaDong = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đã đóng đăng ký");
            var soLopTamDung = await _context.LopHocs.CountAsync(l => l.TrangThai == "Tạm dừng" || l.TrangThai == "Chưa mở");

            return (tongSoLop, soLopDangMo, soLopDaDong, soLopTamDung);
        }
    }
}
