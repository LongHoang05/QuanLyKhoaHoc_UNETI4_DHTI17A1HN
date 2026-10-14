using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh - MSV: 23103100042
    // Module 3: Cài đặt HocVienService
    // Xử lý các quy tắc nghiệp vụ doanh nghiệp (Business Validation, Data Integrity)
    // ========================================================
    public class HocVienService : IHocVienService
    {
        private readonly IHocVienRepository _hocVienRepo;

        public HocVienService(IHocVienRepository hocVienRepo)
        {
            _hocVienRepo = hocVienRepo;
        }

        public async Task<List<HocVien>> GetDanhSachHocVienAsync()
        {
            return await _hocVienRepo.GetAllAsync();
        }

        public async Task<List<HocVien>> TimKiemHocVienAsync(string? keyword, string? searchType = null)
        {
            return await _hocVienRepo.SearchAsync(keyword, searchType);
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

            // 2. Kiểm tra trùng Email với người khác (loại trừ chính mã học viên đang sửa)
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

            // 4. Cập nhật các trường thông tin được phép sửa (bảo vệ trường nhạy cảm MaHocVien, NgayTao, MaTaiKhoan)
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

            // 2. RÀNG BUỘC TOÀN VẸN CSDL & QUY TẮC DOANH NGHIỆP:
            // Nếu học viên đã có lịch sử đăng ký lớp học -> TUYỆT ĐỐI KHÔNG XÓA VẬT LÝ để tránh lỗi Foreign Key Restrict
            bool coDangKy = await _hocVienRepo.CoDangKyHocAsync(id);
            if (coDangKy)
            {
                return (false, "Không thể xóa học viên này vì học viên đã có dữ liệu đăng ký khóa học liên quan trong hệ thống. Để bảo toàn dữ liệu, vui lòng chọn chức năng 'Khóa / Ngưng hoạt động'.");
            }

            // 3. Cho phép xóa an toàn qua Repository
            await _hocVienRepo.DeleteAsync(id);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DoiTrangThaiAsync(int id)
        {
            var existing = await _hocVienRepo.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy học viên.");
            }

            existing.TrangThai = !existing.TrangThai;
            await _hocVienRepo.UpdateAsync(existing);
            return (true, null);
        }
    }
}
