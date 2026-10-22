// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tài khoản, Đăng nhập, Phân quyền và Chương trình đào tạo
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels
{
    /// <summary>
    /// ViewModel phân trang danh sách Chương trình đào tạo
    /// </summary>
    public class ChuongTrinhDaoTaoListViewModel
    {
        public IEnumerable<ChuongTrinhDaoTao> Items { get; set; } = new List<ChuongTrinhDaoTao>();
        public string? Keyword { get; set; }
        public bool? TrangThai { get; set; }

        public int PageIndex { get; set; } = 1;
        public int PageNumber => PageIndex;
        public int PageSize { get; set; } = 8;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
