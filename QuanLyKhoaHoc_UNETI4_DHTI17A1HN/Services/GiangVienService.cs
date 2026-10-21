// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tầng Service Giảng viên (Dữ liệu dùng chung)
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    public class GiangVienService : IGiangVienService
    {
        private readonly IGiangVienRepository _giangVienRepository;

        public GiangVienService(IGiangVienRepository giangVienRepository)
        {
            _giangVienRepository = giangVienRepository;
        }

        public async Task<List<GiangVien>> GetDanhSachAsync()
        {
            return await _giangVienRepository.GetAllAsync();
        }

        public async Task<GiangVien?> GetByIdAsync(int id)
        {
            return await _giangVienRepository.GetByIdAsync(id);
        }

        public async Task<GiangVien?> GetByIdWithLopHocsAsync(int id)
        {
            return await _giangVienRepository.GetByIdWithLopHocsAsync(id);
        }

        public async Task<(IEnumerable<GiangVien> Items, int TotalCount, int TotalPages, int PageIndex)> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, totalCount) = await _giangVienRepository.GetPagedAsync(keyword, trangThai, pageIndex, pageSize);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return (items, totalCount, totalPages, pageIndex);
        }

        public async Task<(bool Success, string? ErrorMessage)> ThemGiangVienAsync(GiangVien giangVien)
        {
            if (string.IsNullOrWhiteSpace(giangVien.HoTen))
            {
                return (false, "Họ tên giảng viên không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(giangVien.Email))
            {
                return (false, "Email giảng viên không được để trống.");
            }

            if (await _giangVienRepository.ExistsEmailAsync(giangVien.Email))
            {
                return (false, $"Địa chỉ email '{giangVien.Email}' đã được sử dụng bởi giảng viên khác.");
            }

            giangVien.HoTen = giangVien.HoTen.Trim();
            giangVien.Email = giangVien.Email.Trim().ToLower();
            giangVien.SoDienThoai = giangVien.SoDienThoai?.Trim();
            giangVien.ChuyenNganh = giangVien.ChuyenNganh?.Trim();
            giangVien.NgayTao = DateTime.Now;

            await _giangVienRepository.AddAsync(giangVien);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatGiangVienAsync(GiangVien giangVien)
        {
            var existing = await _giangVienRepository.GetByIdAsync(giangVien.MaGiangVien);
            if (existing == null)
            {
                return (false, "Không tìm thấy giảng viên cần cập nhật.");
            }

            if (await _giangVienRepository.ExistsEmailAsync(giangVien.Email, giangVien.MaGiangVien))
            {
                return (false, $"Địa chỉ email '{giangVien.Email}' đã được sử dụng bởi giảng viên khác.");
            }

            existing.HoTen = giangVien.HoTen.Trim();
            existing.Email = giangVien.Email.Trim().ToLower();
            existing.SoDienThoai = giangVien.SoDienThoai?.Trim();
            existing.ChuyenNganh = giangVien.ChuyenNganh?.Trim();
            existing.TrangThai = giangVien.TrangThai;

            await _giangVienRepository.UpdateAsync(existing);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> XoaGiangVienAsync(int id)
        {
            var existing = await _giangVienRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy giảng viên cần xóa.");
            }

            if (await _giangVienRepository.CoLopHocLienQuanAsync(id))
            {
                return (false, $"Không thể xóa giảng viên '{existing.HoTen}' vì đang được phân công giảng dạy cho các lớp học.");
            }

            await _giangVienRepository.DeleteAsync(id);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DoiTrangThaiAsync(int id)
        {
            var existing = await _giangVienRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy giảng viên.");
            }

            existing.TrangThai = !existing.TrangThai;
            await _giangVienRepository.UpdateAsync(existing);
            return (true, null);
        }
    }
}
