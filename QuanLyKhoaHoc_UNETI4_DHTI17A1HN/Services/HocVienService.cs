using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh
    // Mã sinh viên: 23103100042
    // Phụ trách: Module 3 - Quản lý học viên, Hồ sơ cá nhân và Đăng ký học
    // ========================================================
    public class HocVienService : IHocVienService
    {
        private readonly IHocVienRepository _hocVienRepo;

        public HocVienService(IHocVienRepository hocVienRepo)
        {
            _hocVienRepo = hocVienRepo;
        }

        #region 1. Nghiệp vụ Thực thể Học Viên (Entity HocVien)
        public async Task<List<HocVien>> GetDanhSachHocVienAsync()
        {
            return await _hocVienRepo.GetAllAsync();
        }

        public async Task<List<HocVien>> TimKiemHocVienAsync(string? keyword, bool? trangThai = null, int? maLopHoc = null)
        {
            return await _hocVienRepo.SearchHocVienAsync(keyword, trangThai, maLopHoc);
        }

        public async Task<HocVien?> GetChiTietHocVienAsync(int? id)
        {
            if (id == null) return null;
            return await _hocVienRepo.GetByIdWithDetailsAsync(id);
        }

        public async Task<HocVien?> GetHocVienByIdAsync(int? id)
        {
            if (id == null) return null;
            return await _hocVienRepo.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? ErrorMessage)> ThemHocVienAsync(HocVien hocVien)
        {
            // 1. Quy tắc nghiệp vụ doanh nghiệp: Kiểm tra trùng Email
            if (await _hocVienRepo.ExistsEmailAsync(hocVien.Email))
            {
                return (false, "Email này đã được sử dụng bởi một học viên khác trong hệ thống.");
            }

            // 2. Quy tắc nghiệp vụ doanh nghiệp: Kiểm tra ngày sinh bắt buộc và độ tuổi tối thiểu
            if (!hocVien.NgaySinh.HasValue)
            {
                return (false, "Vui lòng nhập ngày tháng năm sinh của học viên.");
            }

            if (hocVien.NgaySinh.Value.Date > DateTime.Today)
            {
                return (false, "Ngày sinh không được lớn hơn ngày hiện tại.");
            }

            if (hocVien.NgaySinh.Value.Date > DateTime.Today.AddYears(-6))
            {
                return (false, "Học viên phải từ 6 tuổi trở lên.");
            }

            // 3. Chuẩn hóa dữ liệu đầu vào
            hocVien.HoTen = hocVien.HoTen.Trim();
            hocVien.Email = hocVien.Email.Trim().ToLower();
            hocVien.SoDienThoai = hocVien.SoDienThoai?.Trim();
            hocVien.DiaChi = hocVien.DiaChi?.Trim();
            hocVien.NgheNghiep = hocVien.NgheNghiep?.Trim();
            hocVien.NgayTao = DateTime.Now;

            // 4. Lưu xuống CSDL qua Repository
            await _hocVienRepo.AddAsync(hocVien);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatHocVienAsync(HocVien hocVien)
        {
            // 1. Kiểm tra sự tồn tại của học viên
            var existing = await _hocVienRepo.GetByIdAsync(hocVien.MaHocVien);
            if (existing == null)
            {
                return (false, "Không tìm thấy học viên trong hệ thống.");
            }

            // 2. Kiểm tra trùng Email với người khác
            if (await _hocVienRepo.ExistsEmailAsync(hocVien.Email, hocVien.MaHocVien))
            {
                return (false, "Email này đã được sử dụng bởi một học viên khác.");
            }

            // 3. Kiểm tra ngày sinh bắt buộc và hợp lệ
            if (!hocVien.NgaySinh.HasValue)
            {
                return (false, "Vui lòng nhập ngày tháng năm sinh của học viên.");
            }

            if (hocVien.NgaySinh.Value.Date > DateTime.Today)
            {
                return (false, "Ngày sinh không được lớn hơn ngày hiện tại.");
            }

            if (hocVien.NgaySinh.Value.Date > DateTime.Today.AddYears(-6))
            {
                return (false, "Học viên phải từ 6 tuổi trở lên.");
            }

            // 4. Cập nhật các trường thông tin được phép sửa
            existing.HoTen = hocVien.HoTen.Trim();
            existing.Email = hocVien.Email.Trim().ToLower();
            existing.SoDienThoai = hocVien.SoDienThoai?.Trim();
            existing.NgaySinh = hocVien.NgaySinh;
            existing.GioiTinh = hocVien.GioiTinh;
            existing.DiaChi = hocVien.DiaChi?.Trim();
            existing.TrinhDoHocVan = hocVien.TrinhDoHocVan;
            existing.NgheNghiep = hocVien.NgheNghiep?.Trim();
            existing.TrangThai = hocVien.TrangThai;

            // 5. Cập nhật xuống CSDL qua Repository
            await _hocVienRepo.UpdateAsync(existing);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> XoaHocVienAsync(int id)
        {
            // 1. Kiểm tra tồn tại
            var existing = await _hocVienRepo.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy học viên để xóa.");
            }

            // 2. Ràng buộc toàn vẹn CSDL: Không xóa vật lý nếu đã có lịch sử đăng ký học
            bool coDangKy = await _hocVienRepo.CoDangKyHocAsync(id);
            if (coDangKy)
            {
                return (false, "Không thể xóa học viên này vì học viên đã có dữ liệu đăng ký khóa học trong hệ thống. Để bảo toàn dữ liệu, vui lòng đổi trạng thái sang 'Tạm khóa'.");
            }

            // 3. Cho phép xóa an toàn qua Repository
            await _hocVienRepo.DeleteAsync(id);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage, bool NewStatus)> DoiTrangThaiAsync(int id)
        {
            var existing = await _hocVienRepo.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy học viên.", false);
            }

            existing.TrangThai = !existing.TrangThai;
            await _hocVienRepo.UpdateAsync(existing);
            return (true, null, existing.TrangThai);
        }
        #endregion

        #region 2. Nghiệp vụ Thực thể Tài Khoản (Entity TaiKhoan) & Lớp Học
        public async Task<List<TaiKhoan>> GetDanhSachTaiKhoanAsync()
        {
            return await _hocVienRepo.GetAllTaiKhoanAsync();
        }

        public async Task<List<TaiKhoan>> TimKiemTaiKhoanAsync(string? keyword, string? vaiTro = null, bool? trangThai = null)
        {
            return await _hocVienRepo.SearchTaiKhoanAsync(keyword, vaiTro, trangThai);
        }

        public async Task<(bool Success, string? ErrorMessage, bool NewStatus, string Username)> DoiTrangThaiTaiKhoanAsync(int id)
        {
            var acc = await _hocVienRepo.GetTaiKhoanByIdAsync(id);
            if (acc == null)
            {
                return (false, "Không tìm thấy tài khoản.", false, string.Empty);
            }

            acc.TrangThai = !acc.TrangThai;
            await _hocVienRepo.UpdateTaiKhoanAsync(acc);
            return (true, null, acc.TrangThai, acc.TenDangNhap);
        }

        public async Task<(bool Success, string? ErrorMessage)> ResetMatKhauTaiKhoanAsync(int id, string? newPassword)
        {
            var acc = await _hocVienRepo.GetTaiKhoanByIdAsync(id);
            if (acc == null)
            {
                return (false, "Không tìm thấy tài khoản.");
            }

            var pass = string.IsNullOrWhiteSpace(newPassword) ? "123456" : newPassword.Trim();
            acc.MatKhau = pass;
            await _hocVienRepo.UpdateTaiKhoanAsync(acc);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> ThemTaiKhoanMoiAsync(ThemTaiKhoanViewModel model)
        {
            // 1. Kiểm tra trùng tên đăng nhập
            if (await _hocVienRepo.ExistsUsernameAsync(model.TenDangNhap))
            {
                return (false, $"Tên đăng nhập '{model.TenDangNhap}' đã tồn tại trên hệ thống! Vui lòng chọn tên khác.");
            }

            // 2. Tạo bản ghi TaiKhoan
            var tk = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap.Trim(),
                MatKhau = string.IsNullOrWhiteSpace(model.MatKhau) ? "123456" : model.MatKhau.Trim(),
                HoTen = model.HoTen.Trim(),
                Email = model.Email.Trim().ToLower(),
                VaiTro = model.VaiTro,
                TrangThai = true,
                NgayTao = DateTime.Now
            };

            await _hocVienRepo.AddTaiKhoanAsync(tk);

            // 3. Nếu là vai trò Học viên -> Tự động khởi tạo hồ sơ HocVien liên kết
            if (model.VaiTro == "HocVien" || model.VaiTro == "Học viên")
            {
                var existingHv = await _hocVienRepo.ExistsEmailAsync(model.Email);
                if (!existingHv)
                {
                    var hv = new HocVien
                    {
                        MaTaiKhoan = tk.MaTaiKhoan,
                        HoTen = model.HoTen.Trim(),
                        Email = model.Email.Trim().ToLower(),
                        NgaySinh = new DateTime(2004, 1, 1),
                        GioiTinh = "Nam",
                        DiaChi = "Hà Nội",
                        TrinhDoHocVan = "Đại học",
                        NgheNghiep = "Học viên",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    };
                    await _hocVienRepo.AddAsync(hv);
                }
            }

            return (true, null);
        }

        public async Task<List<LopHoc>> GetDanhSachLopHocAsync()
        {
            return await _hocVienRepo.GetAllLopHocAsync();
        }

        public async Task<(int TongHocVien, int SoGiaoVu, int SoAdmin, int SoBiKhoa)> GetThongKeAsync()
        {
            var hocViens = await _hocVienRepo.GetAllAsync();
            var taiKhoans = await _hocVienRepo.GetAllTaiKhoanAsync();

            int tongHocVien = hocViens.Count;
            int soGiaoVu = taiKhoans.Count(t => t.VaiTro == "GiaoVu" || t.VaiTro == "Giáo vụ");
            int soAdmin = taiKhoans.Count(t => t.VaiTro == "Admin");
            int soBiKhoa = taiKhoans.Count(t => !t.TrangThai);

            return (tongHocVien, soGiaoVu, soAdmin, soBiKhoa);
        }
        #endregion
    }
}
