using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Bổ sung theo yêu cầu giảng viên hướng dẫn:
    // Cài đặt Repository Quản lý Giảng viên (Repository Pattern)
    // ========================================================
    public class GiangVienRepository : IGiangVienRepository
    {
        private readonly ApplicationDbContext _context;

        public GiangVienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region 1. Quản lý thực thể Giảng viên
        public async Task<List<GiangVien>> GetAllAsync()
        {
            return await _context.GiangViens
                .Include(g => g.TaiKhoan)
                .Include(g => g.LopHocs)
                .OrderByDescending(g => g.MaGiangVien)
                .ToListAsync();
        }

        public async Task<GiangVien?> GetByIdAsync(int id)
        {
            return await _context.GiangViens
                .Include(g => g.TaiKhoan)
                .Include(g => g.LopHocs)
                .FirstOrDefaultAsync(g => g.MaGiangVien == id);
        }

        public async Task<GiangVien?> GetByTaiKhoanIdAsync(int maTaiKhoan)
        {
            return await _context.GiangViens
                .Include(g => g.TaiKhoan)
                .Include(g => g.LopHocs)
                .FirstOrDefaultAsync(g => g.MaTaiKhoan == maTaiKhoan);
        }

        public async Task<GiangVien?> GetFirstAsync()
        {
            return await _context.GiangViens
                .Include(g => g.TaiKhoan)
                .Include(g => g.LopHocs)
                .FirstOrDefaultAsync();
        }

        public async Task<List<GiangVien>> SearchAsync(string? search, string? hocVi)
        {
            var query = _context.GiangViens
                .Include(g => g.TaiKhoan)
                .Include(g => g.LopHocs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.Trim().ToLower();
                query = query.Where(g => g.HoTen.ToLower().Contains(s) || g.Email.ToLower().Contains(s) || (g.SoDienThoai != null && g.SoDienThoai.Contains(s)));
            }

            if (!string.IsNullOrWhiteSpace(hocVi))
            {
                string hv = hocVi.Trim();
                if (hv == "ThS." || hv == "Thạc sĩ")
                    query = query.Where(g => g.HocVi == "ThS." || g.HocVi == "Thạc sĩ");
                else if (hv == "TS." || hv == "Tiến sĩ")
                    query = query.Where(g => g.HocVi == "TS." || g.HocVi == "Tiến sĩ");
                else if (hv == "PGS.TS." || hv == "PGS.TS" || hv == "PGS. Tiến sĩ")
                    query = query.Where(g => g.HocVi == "PGS.TS." || g.HocVi == "PGS.TS" || g.HocVi == "PGS. Tiến sĩ");
                else if (hv == "GS.TS." || hv == "GS.TS" || hv == "GS. Tiến sĩ")
                    query = query.Where(g => g.HocVi == "GS.TS." || g.HocVi == "GS.TS" || g.HocVi == "GS. Tiến sĩ");
                else
                    query = query.Where(g => g.HocVi == hv);
            }

            return await query.OrderByDescending(g => g.MaGiangVien).ToListAsync();
        }

        public async Task AddAsync(GiangVien gv)
        {
            gv.NgayTao = DateTime.Now;
            _context.GiangViens.Add(gv);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(GiangVien gv)
        {
            _context.GiangViens.Update(gv);
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

        public async Task<bool> HasClassesAsync(int id)
        {
            return await _context.LopHocs.AnyAsync(l => l.MaGiangVien == id);
        }

        public async Task<bool> ExistsEmailAsync(string email, int? excludeId = null)
        {
            return await _context.GiangViens
                .AnyAsync(g => g.Email == email && (!excludeId.HasValue || g.MaGiangVien != excludeId.Value));
        }
        #endregion

        #region 2. Nghiệp vụ Lớp học & Buổi học của Giảng viên
        public async Task<List<LopHoc>> GetClassesByTeacherAsync(int maGiangVien, string teacherName)
        {
            return await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .Include(l => l.DangKyHocs)
                .Where(l => l.MaGiangVien == maGiangVien || l.TenGiangVien == teacherName)
                .ToListAsync();
        }

        public async Task<List<BuoiHoc>> GetUpcomingSessionsAsync(int maGiangVien, List<int> classIds, int count = 6)
        {
            return await _context.BuoiHocs
                .Include(b => b.LopHoc)
                .Where(b => (b.MaGiangVien == maGiangVien || classIds.Contains(b.MaLopHoc)) 
                            && b.ThoiGianBatDau >= DateTime.Today.AddDays(-1) 
                            && b.TrangThai != "Đã hủy")
                .OrderBy(b => b.ThoiGianBatDau)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<BuoiHoc>> GetSessionsInDateRangeAsync(int maGiangVien, List<int> classIds, DateTime fromDate, DateTime toDate, int? maLop = null)
        {
            var query = _context.BuoiHocs
                .Include(b => b.LopHoc)
                    .ThenInclude(l => l!.ChuongTrinhDaoTao)
                .Where(b => b.MaGiangVien == maGiangVien || classIds.Contains(b.MaLopHoc))
                .AsQueryable();

            if (maLop.HasValue && maLop.Value > 0)
            {
                query = query.Where(b => b.MaLopHoc == maLop.Value);
            }

            return await query
                .Where(b => b.ThoiGianBatDau >= fromDate && b.ThoiGianBatDau <= toDate)
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();
        }
        #endregion

        #region 3. Nghiệp vụ Học viên & Chấm điểm
        public async Task<List<DangKyHoc>> GetClassStudentsWithGradesAsync(int maLop)
        {
            return await _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.KetQuaHocTap)
                .Where(d => d.MaLopHoc == maLop &&
                            (d.TrangThai == "Đã duyệt" ||
                             d.TrangThai == "Đã đóng học phí" ||
                             d.TrangThai == "Đang học" ||
                             d.TrangThai == "Hoàn thành" ||
                             d.TrangThai == "Không hoàn thành"))
                .OrderBy(d => d.HocVien != null ? d.HocVien.HoTen : "")
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, KetQuaHocTap? KetQua)> SaveGradeAsync(int maDangKy, double? diemCC, double? diemGK, double? diemCK, string? ghiChu)
        {
            var dk = await _context.DangKyHocs
                .Include(d => d.KetQuaHocTap)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);

            if (dk == null)
            {
                return (false, "Không tìm thấy hồ sơ đăng ký học.", null);
            }

            if (diemCC.HasValue && (diemCC.Value < 0 || diemCC.Value > 10))
                return (false, "Điểm quá trình phải từ 0 đến 10.", null);
            if (diemGK.HasValue && (diemGK.Value < 0 || diemGK.Value > 10))
                return (false, "Điểm giữa kỳ phải từ 0 đến 10.", null);
            if (diemCK.HasValue && (diemCK.Value < 0 || diemCK.Value > 10))
                return (false, "Điểm cuối kỳ phải từ 0 đến 10.", null);

            double? diemQuaTrinh = diemCC ?? diemGK;
            if (!diemQuaTrinh.HasValue || !diemCK.HasValue)
            {
                return (false, "Vui lòng nhập đầy đủ Điểm quá trình (40%) và Điểm thi cuối kỳ (60%).", null);
            }

            var ketQua = dk.KetQuaHocTap;
            if (ketQua == null)
            {
                ketQua = new KetQuaHocTap
                {
                    MaDangKy = maDangKy,
                    NgayCapNhat = DateTime.Now
                };
                _context.KetQuaHocTaps.Add(ketQua);
            }

            ketQua.DiemChuyenCan = diemQuaTrinh.Value;
            ketQua.DiemCuoiKhoa = diemCK.Value;
            ketQua.NhanXet = ghiChu;
            ketQua.NgayCapNhat = DateTime.Now;

            // Tính điểm tổng kết thống nhất toàn hệ thống: 40% Quá trình + 60% Cuối kỳ (hoặc 10% CC + 40% GK + 50% CK nếu truyền đủ 3 cột)
            double tk = (diemCC.HasValue && diemGK.HasValue)
                ? Math.Round(diemCC.Value * 0.1 + diemGK.Value * 0.4 + diemCK.Value * 0.5, 2)
                : Math.Round(diemQuaTrinh.Value * 0.4 + diemCK.Value * 0.6, 2);

            ketQua.DiemTongKet = tk;

            if (tk >= 8.5) { ketQua.DiemChu = "A"; ketQua.DiemHe4 = 4.0; ketQua.XepLoai = "Xuất sắc"; ketQua.KetQua = "Hoàn thành"; }
            else if (tk >= 8.0) { ketQua.DiemChu = "B+"; ketQua.DiemHe4 = 3.5; ketQua.XepLoai = "Giỏi"; ketQua.KetQua = "Hoàn thành"; }
            else if (tk >= 7.0) { ketQua.DiemChu = "B"; ketQua.DiemHe4 = 3.0; ketQua.XepLoai = "Khá"; ketQua.KetQua = "Hoàn thành"; }
            else if (tk >= 6.5) { ketQua.DiemChu = "C+"; ketQua.DiemHe4 = 2.5; ketQua.XepLoai = "Trung bình khá"; ketQua.KetQua = "Hoàn thành"; }
            else if (tk >= 5.5) { ketQua.DiemChu = "C"; ketQua.DiemHe4 = 2.0; ketQua.XepLoai = "Trung bình"; ketQua.KetQua = "Hoàn thành"; }
            else if (tk >= 5.0) { ketQua.DiemChu = "D+"; ketQua.DiemHe4 = 1.5; ketQua.XepLoai = "Trung bình yếu"; ketQua.KetQua = "Hoàn thành"; }
            else if (tk >= 4.0) { ketQua.DiemChu = "D"; ketQua.DiemHe4 = 1.0; ketQua.XepLoai = "Yếu"; ketQua.KetQua = "Không hoàn thành"; }
            else { ketQua.DiemChu = "F"; ketQua.DiemHe4 = 0.0; ketQua.XepLoai = "Kém"; ketQua.KetQua = "Không hoàn thành"; }

            dk.TrangThai = ketQua.KetQua;

            await _context.SaveChangesAsync();
            return (true, "Cập nhật điểm thành công!", ketQua);
        }

        public async Task<int> CountGradedStudentsAsync(List<int> classIds)
        {
            var dkIds = await _context.DangKyHocs
                .Where(d => classIds.Contains(d.MaLopHoc))
                .Select(d => d.MaDangKy)
                .ToListAsync();

            return await _context.KetQuaHocTaps
                .CountAsync(k => dkIds.Contains(k.MaDangKy) && k.DiemTongKet > 0);
        }
        #endregion

        #region 4. Quản lý Tài khoản liên kết
        public async Task<bool> ExistsUsernameAsync(string username)
        {
            return await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == username);
        }

        public async Task AddTaiKhoanAsync(TaiKhoan tk)
        {
            _context.TaiKhoans.Add(tk);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
