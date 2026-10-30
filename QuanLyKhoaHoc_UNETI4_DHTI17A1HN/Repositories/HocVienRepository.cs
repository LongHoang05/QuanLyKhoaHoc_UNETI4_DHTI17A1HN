using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh
    // Mã sinh viên: 23103100042
    // Phụ trách: Module 3 - Quản lý học viên, Hồ sơ cá nhân và Đăng ký học (Repository Skeleton)
    // ========================================================
    public class HocVienRepository : IHocVienRepository
    {
        private readonly ApplicationDbContext _context;

        public HocVienRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region 1. Nghiệp vụ Thực thể Học Viên (Entity HocVien - Mục 7.1)
        public async Task<List<HocVien>> GetAllAsync()
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Lấy toàn bộ học viên kèm TaiKhoan và DangKyHocs
            return await Task.FromResult(new List<HocVien>());
        }

        public List<HocVien> GetAll()
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Lấy danh sách học viên đồng bộ
            return new List<HocVien>();
        }

        public async Task<HocVien?> GetByIdAsync(int? id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Lấy 1 học viên theo MaHocVien
            return await Task.FromResult<HocVien?>(null);
        }

        public async Task<HocVien?> GetByIdWithDetailsAsync(int? id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Lấy học viên kèm chi tiết TaiKhoan và DangKyHocs
            return await Task.FromResult<HocVien?>(null);
        }

        public async Task<HocVien?> GetByMaTaiKhoanAsync(int maTaiKhoan)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Lấy học viên theo MaTaiKhoan
            return await Task.FromResult<HocVien?>(null);
        }

        public async Task AddAsync(HocVien hocVien)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Thêm mới học viên vào _context.HocViens
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(HocVien hocVien)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Cập nhật học viên trong _context.HocViens
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Xóa học viên theo MaHocVien
            await Task.CompletedTask;
        }

        public bool Exists(int id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Kiểm tra tồn tại học viên theo ID
            return false;
        }

        public async Task<bool> ExistsEmailAsync(string email, int? excludeId = null)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Kiểm tra trùng Email học viên
            return await Task.FromResult(false);
        }

        public async Task<bool> CoDangKyHocAsync(int maHocVien)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Kiểm tra học viên đã có lịch sử đăng ký lớp học chưa
            return await Task.FromResult(false);
        }

        public async Task SaveAsync()
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Lưu thay đổi xuống CSDL
            await Task.CompletedTask;
        }

        public async Task<List<HocVien>> SearchHocVienAsync(string? keyword, bool? trangThai = null, int? maLopHoc = null)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Tìm kiếm và lọc học viên theo từ khóa, trạng thái, lớp học
            return await Task.FromResult(new List<HocVien>());
        }

        public async Task<(IEnumerable<HocVien> Items, int TotalCount)> GetPagedAsync(
            string? keyword, bool? trangThai, int pageIndex, int pageSize)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]: Phân trang danh sách học viên
            return await Task.FromResult((Enumerable.Empty<HocVien>(), 0));
        }
        #endregion

        #region 2. Nghiệp vụ Thực thể Tài Khoản & Lớp Học (Bổ trợ Module 3)
        public async Task<List<TaiKhoan>> GetAllTaiKhoanAsync()
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            return await Task.FromResult(new List<TaiKhoan>());
        }

        public async Task<List<TaiKhoan>> SearchTaiKhoanAsync(string? keyword, string? vaiTro = null, bool? trangThai = null)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            return await Task.FromResult(new List<TaiKhoan>());
        }

        public async Task<TaiKhoan?> GetTaiKhoanByIdAsync(int id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            return await Task.FromResult<TaiKhoan?>(null);
        }

        public async Task<bool> ExistsUsernameAsync(string username)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            return await Task.FromResult(false);
        }

        public async Task AddTaiKhoanAsync(TaiKhoan taiKhoan)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            await Task.CompletedTask;
        }

        public async Task UpdateTaiKhoanAsync(TaiKhoan taiKhoan)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            await Task.CompletedTask;
        }

        public async Task<List<LopHoc>> GetAllLopHocAsync()
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]
            return await Task.FromResult(new List<LopHoc>());
        }
        #endregion
    }
}
