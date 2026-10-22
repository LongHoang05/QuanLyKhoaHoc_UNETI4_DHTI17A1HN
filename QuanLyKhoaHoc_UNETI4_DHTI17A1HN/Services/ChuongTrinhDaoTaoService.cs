// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    public class ChuongTrinhDaoTaoService : IChuongTrinhDaoTaoService
    {
        private readonly IChuongTrinhDaoTaoRepository _repository;

        public ChuongTrinhDaoTaoService(IChuongTrinhDaoTaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ChuongTrinhDaoTao>> GetDanhSachAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ChuongTrinhDaoTao?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ChuongTrinhDaoTao?> GetByIdWithDetailsAsync(int id)
        {
            return await _repository.GetByIdWithLopHocsAsync(id);
        }

        public async Task<ChuongTrinhDaoTaoListViewModel> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize = 8)
        {
            if (pageIndex < 1) pageIndex = 1;

            var (items, totalCount) = await _repository.GetPagedAsync(keyword, trangThai, pageIndex, pageSize);

            return new ChuongTrinhDaoTaoListViewModel
            {
                Items = items,
                Keyword = keyword,
                TrangThai = trangThai,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> ThemChuongTrinhAsync(ChuongTrinhDaoTao chuongTrinh)
        {
            if (string.IsNullOrWhiteSpace(chuongTrinh.TenChuongTrinh))
            {
                return (false, "Tên chương trình đào tạo không được để trống.");
            }

            if (await _repository.ExistsTenChuongTrinhAsync(chuongTrinh.TenChuongTrinh))
            {
                return (false, $"Chương trình đào tạo '{chuongTrinh.TenChuongTrinh}' đã tồn tại trong hệ thống.");
            }

            chuongTrinh.TenChuongTrinh = chuongTrinh.TenChuongTrinh.Trim();
            chuongTrinh.MoTa = chuongTrinh.MoTa?.Trim();
            chuongTrinh.EmailLienHe = chuongTrinh.EmailLienHe?.Trim();
            chuongTrinh.NgayTao = DateTime.Now;

            await _repository.AddAsync(chuongTrinh);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatChuongTrinhAsync(ChuongTrinhDaoTao chuongTrinh)
        {
            var existing = await _repository.GetByIdAsync(chuongTrinh.MaChuongTrinh);
            if (existing == null)
            {
                return (false, "Chương trình đào tạo không tồn tại.");
            }

            if (await _repository.ExistsTenChuongTrinhAsync(chuongTrinh.TenChuongTrinh, chuongTrinh.MaChuongTrinh))
            {
                return (false, $"Tên chương trình '{chuongTrinh.TenChuongTrinh}' đã được sử dụng bởi chương trình khác.");
            }

            existing.TenChuongTrinh = chuongTrinh.TenChuongTrinh.Trim();
            existing.MoTa = chuongTrinh.MoTa?.Trim();
            existing.EmailLienHe = chuongTrinh.EmailLienHe?.Trim();
            existing.TrangThai = chuongTrinh.TrangThai;

            await _repository.UpdateAsync(existing);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> XoaChuongTrinhAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Chương trình đào tạo không tồn tại.");
            }

            // Ràng buộc nghiệp vụ: Không thể xóa chương trình nếu đã có các lớp học liên kết
            if (await _repository.CoLopHocLienQuanAsync(id))
            {
                return (false, $"Không thể xóa chương trình đào tạo '{existing.TenChuongTrinh}' vì đã có các lớp học trực thuộc đang được tổ chức.");
            }

            await _repository.DeleteAsync(id);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> ToggleTrangThaiAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Chương trình đào tạo không tồn tại.");
            }

            existing.TrangThai = !existing.TrangThai;
            await _repository.UpdateAsync(existing);
            return (true, null);
        }
    }
}
