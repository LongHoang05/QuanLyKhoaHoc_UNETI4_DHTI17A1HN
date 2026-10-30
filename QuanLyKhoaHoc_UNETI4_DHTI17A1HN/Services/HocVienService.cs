using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh
    // Mã sinh viên: 23103100042
    // Phụ trách: Module 3 - Quản lý học viên, Hồ sơ cá nhân và Đăng ký học (Service Skeleton)
    // ========================================================
    public class HocVienService : IHocVienService
    {
        private readonly IHocVienRepository _repository;

        public HocVienService(IHocVienRepository repository)
        {
            _repository = repository;
        }

        #region 1. Nghiệp vụ Học Viên (HocVien - Mục 7.1)
        public async Task<List<HocVien>> GetDanhSachHocVienAsync()
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Lấy toàn bộ danh sách học viên
            return await _repository.GetAllAsync();
        }

        public async Task<List<HocVien>> TimKiemHocVienAsync(string? keyword, bool? trangThai = null, int? maLopHoc = null)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Tìm kiếm học viên theo từ khóa, trạng thái, lớp học
            return await _repository.SearchHocVienAsync(keyword, trangThai, maLopHoc);
        }

        public async Task<HocVien?> GetChiTietHocVienAsync(int? id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Lấy chi tiết học viên kèm lịch sử đăng ký lớp
            return await _repository.GetByIdWithDetailsAsync(id);
        }

        public async Task<HocVien?> GetHocVienByIdAsync(int? id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Lấy thông tin học viên theo ID
            return await _repository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? ErrorMessage)> ThemHocVienAsync(HocVien hocVien)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Kiểm tra trùng Email, kiểm tra tuổi hợp lệ và gọi _repository.AddAsync(hocVien)
            await Task.CompletedTask;
            return (false, "Chức năng Thêm học viên (Module 3) đang chờ Nguyễn Đức Linh tự triển khai.");
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatHocVienAsync(HocVien hocVien)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Kiểm tra tồn tại, chống trùng Email và gọi _repository.UpdateAsync(hocVien)
            await Task.CompletedTask;
            return (false, "Chức năng Cập nhật học viên (Module 3) đang chờ Nguyễn Đức Linh tự triển khai.");
        }

        public async Task<(bool Success, string? ErrorMessage)> XoaHocVienAsync(int id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Kiểm tra CoDangKyHocAsync(id) trước khi xóa hoặc khóa học viên
            await Task.CompletedTask;
            return (false, "Chức năng Xóa học viên (Module 3) đang chờ Nguyễn Đức Linh tự triển khai.");
        }

        public async Task<(bool Success, string? ErrorMessage, bool NewStatus)> DoiTrangThaiAsync(int id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Khóa / Mở khóa trạng thái hoạt động của học viên
            await Task.CompletedTask;
            return (false, "Chức năng Đổi trạng thái học viên (Module 3) đang chờ Nguyễn Đức Linh tự triển khai.", false);
        }
        #endregion

        #region 2. Nghiệp vụ Tài Khoản & Lớp Học (Bổ trợ Module 3)
        public async Task<List<TaiKhoan>> GetDanhSachTaiKhoanAsync()
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            return await _repository.GetAllTaiKhoanAsync();
        }

        public async Task<List<TaiKhoan>> TimKiemTaiKhoanAsync(string? keyword, string? vaiTro = null, bool? trangThai = null)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            return await _repository.SearchTaiKhoanAsync(keyword, vaiTro, trangThai);
        }

        public async Task<(bool Success, string? ErrorMessage, bool NewStatus, string Username)> DoiTrangThaiTaiKhoanAsync(int id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            await Task.CompletedTask;
            return (false, "Chức năng đang chờ triển khai.", false, string.Empty);
        }

        public async Task<(bool Success, string? ErrorMessage)> ResetMatKhauTaiKhoanAsync(int id, string? newPassword)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            await Task.CompletedTask;
            return (false, "Chức năng đang chờ triển khai.");
        }

        public async Task<(bool Success, string? ErrorMessage)> ThemTaiKhoanMoiAsync(ThemTaiKhoanViewModel model)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            await Task.CompletedTask;
            return (false, "Chức năng đang chờ triển khai.");
        }

        public async Task<List<LopHoc>> GetDanhSachLopHocAsync()
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            return await _repository.GetAllLopHocAsync();
        }

        public async Task<(int TongHocVien, int SoGiaoVu, int SoAdmin, int SoBiKhoa)> GetThongKeAsync()
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            await Task.CompletedTask;
            return (0, 0, 0, 0);
        }
        #endregion
    }
}
