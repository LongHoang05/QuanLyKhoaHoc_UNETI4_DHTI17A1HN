using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    // ========================================================
    // Bổ sung theo yêu cầu giảng viên hướng dẫn:
    // Cài đặt Service Cổng Giảng viên & Quản lý Giảng viên (Service Pattern)
    // ========================================================
    public class GiangVienService : IGiangVienService
    {
        private readonly IGiangVienRepository _giangVienRepo;

        public GiangVienService(IGiangVienRepository giangVienRepo)
        {
            _giangVienRepo = giangVienRepo;
        }

        #region 1. Xác định giảng viên hiện tại
        public async Task<GiangVien?> GetCurrentGiangVienAsync(int? maGiangVien, int? maTaiKhoan)
        {
            if (maGiangVien.HasValue)
            {
                var gv = await _giangVienRepo.GetByIdAsync(maGiangVien.Value);
                if (gv != null) return gv;
            }

            if (maTaiKhoan.HasValue)
            {
                var gv = await _giangVienRepo.GetByTaiKhoanIdAsync(maTaiKhoan.Value);
                if (gv != null) return gv;
            }

            // Fallback lấy giảng viên đầu tiên cho mục đích kiểm thử
            return await _giangVienRepo.GetFirstAsync();
        }
        #endregion

        #region 2. Nghiệp vụ Dashboard Giảng viên
        public async Task<List<LopHoc>> GetClassesForTeacherAsync(GiangVien gv)
        {
            return await _giangVienRepo.GetClassesByTeacherAsync(gv.MaGiangVien, gv.HoTen);
        }

        public async Task<List<BuoiHoc>> GetUpcomingSessionsAsync(GiangVien gv, int take = 6)
        {
            var classes = await GetClassesForTeacherAsync(gv);
            var classIds = classes.Select(c => c.MaLopHoc).ToList();
            return await _giangVienRepo.GetUpcomingSessionsAsync(gv.MaGiangVien, classIds, take);
        }

        public async Task<int> GetGradedCountAsync(List<int> classIds)
        {
            return await _giangVienRepo.CountGradedStudentsAsync(classIds);
        }
        #endregion

        #region 3. Nghiệp vụ Lịch giảng dạy / Thời khóa biểu
        public async Task<List<BuoiHoc>> GetWeeklyTimetableAsync(GiangVien gv, int? maLop, DateTime weekStart)
        {
            var classes = await GetClassesForTeacherAsync(gv);
            var classIds = classes.Select(c => c.MaLopHoc).ToList();
            DateTime weekEnd = weekStart.AddDays(7);

            return await _giangVienRepo.GetSessionsInDateRangeAsync(
                gv.MaGiangVien, classIds, weekStart.AddDays(-1), weekEnd.AddDays(1), maLop);
        }
        #endregion

        #region 4. Nghiệp vụ Chấm điểm học viên
        public async Task<List<DangKyHoc>> GetStudentsForGradingAsync(int maLop)
        {
            return await _giangVienRepo.GetClassStudentsWithGradesAsync(maLop);
        }

        public async Task<(bool Success, string Message, object? Data)> LuuDiemHocVienAsync(int maDangKy, double? diemCC, double? diemGK, double? diemCK, string? ghiChu)
        {
            var result = await _giangVienRepo.SaveGradeAsync(maDangKy, diemCC, diemGK, diemCK, ghiChu);
            if (!result.Success || result.KetQua == null)
            {
                return (false, result.Message, null);
            }

            var kq = result.KetQua;
            var returnData = new
            {
                diemTongKet = kq.DiemTongKet.ToString("0.0"),
                diemHe4 = kq.DiemHe4?.ToString("0.0") ?? "-",
                diemChu = kq.DiemChu ?? "-",
                xepLoai = kq.XepLoai ?? "-",
                ketQua = kq.KetQua ?? "-"
            };

            return (true, result.Message, returnData);
        }
        #endregion

        #region 5. Nghiệp vụ Quản trị Giảng viên
        public async Task<List<GiangVien>> GetDanhSachGiangVienAsync(string? search, string? hocVi)
        {
            return await _giangVienRepo.SearchAsync(search, hocVi);
        }

        public async Task<GiangVien?> GetGiangVienByIdAsync(int id)
        {
            return await _giangVienRepo.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message)> TaoGiangVienAsync(GiangVien gv, string? tenDangNhap, string? matKhau)
        {
            if (string.IsNullOrWhiteSpace(gv.HoTen) || string.IsNullOrWhiteSpace(gv.Email))
            {
                return (false, "Vui lòng nhập đầy đủ họ tên và email.");
            }

            if (await _giangVienRepo.ExistsEmailAsync(gv.Email))
            {
                return (false, $"Email '{gv.Email}' đã được đăng ký bởi giảng viên khác.");
            }

            if (!string.IsNullOrWhiteSpace(tenDangNhap))
            {
                if (await _giangVienRepo.ExistsUsernameAsync(tenDangNhap.Trim()))
                {
                    return (false, $"Tên đăng nhập '{tenDangNhap}' đã tồn tại trong hệ thống.");
                }

                var tk = new TaiKhoan
                {
                    TenDangNhap = tenDangNhap.Trim(),
                    MatKhau = string.IsNullOrWhiteSpace(matKhau) ? "123456" : matKhau.Trim(),
                    HoTen = gv.HoTen.Trim(),
                    Email = gv.Email.Trim(),
                    VaiTro = "GiangVien",
                    TrangThai = true,
                    NgayTao = DateTime.Now
                };

                await _giangVienRepo.AddTaiKhoanAsync(tk);
                gv.MaTaiKhoan = tk.MaTaiKhoan;
            }

            await _giangVienRepo.AddAsync(gv);
            return (true, $"Thêm mới giảng viên '{gv.HoTen}' thành công!");
        }

        public async Task<(bool Success, string Message)> CapNhatGiangVienAsync(int maGiangVien, GiangVien gv)
        {
            var existing = await _giangVienRepo.GetByIdAsync(maGiangVien);
            if (existing == null)
            {
                return (false, "Không tìm thấy hồ sơ giảng viên cần cập nhật.");
            }

            if (await _giangVienRepo.ExistsEmailAsync(gv.Email, maGiangVien))
            {
                return (false, $"Email '{gv.Email}' đã được sử dụng bởi giảng viên khác.");
            }

            existing.HoTen = gv.HoTen.Trim();
            existing.Email = gv.Email.Trim();
            existing.SoDienThoai = gv.SoDienThoai?.Trim();
            existing.HocVi = gv.HocVi;
            existing.ChuyenNganh = gv.ChuyenNganh?.Trim();
            existing.GioiThieu = gv.GioiThieu?.Trim();
            existing.TrangThai = gv.TrangThai;

            if (existing.TaiKhoan != null)
            {
                existing.TaiKhoan.HoTen = existing.HoTen;
                existing.TaiKhoan.Email = existing.Email;
                existing.TaiKhoan.TrangThai = existing.TrangThai;
            }

            await _giangVienRepo.UpdateAsync(existing);
            return (true, $"Cập nhật hồ sơ giảng viên '{existing.HoTen}' thành công!");
        }

        public async Task<(bool Success, string Message)> XoaGiangVienAsync(int id)
        {
            var existing = await _giangVienRepo.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy giảng viên.");
            }

            if (await _giangVienRepo.HasClassesAsync(id))
            {
                return (false, $"Không thể xóa giảng viên '{existing.HoTen}' vì đang phụ trách phân công lớp học.");
            }

            await _giangVienRepo.DeleteAsync(id);
            return (true, $"Đã xóa giảng viên '{existing.HoTen}' thành công!");
        }
        #endregion
    }
}
