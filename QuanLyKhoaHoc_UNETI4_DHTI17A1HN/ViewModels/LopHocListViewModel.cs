// ========================================================
// Họ và tên: Nguyễn Đình Kiên
// Mã sinh viên: 23103100020
// Phụ trách: Module 2 - Quản lý lớp học, Tìm kiếm, Lọc, Sắp xếp và Phân trang
// ========================================================

using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels
{
    public class LopHocListViewModel
    {
        // Danh sách dữ liệu lớp học
        public IEnumerable<LopHoc> Items { get; set; } = new List<LopHoc>();

        // Tham số tìm kiếm, lọc và sắp xếp
        public string? Keyword { get; set; }
        public int? MaChuongTrinh { get; set; }
        public string? TrinhDoDauVao { get; set; }
        public string? TrangThai { get; set; }
        public string? GiangVien { get; set; }
        public decimal? MinHocPhi { get; set; }
        public decimal? MaxHocPhi { get; set; }
        public string? FilterHanDangKy { get; set; } // "con_han", "het_han"
        public string? FilterConCho { get; set; }    // "con_cho", "het_cho"
        public string? SortBy { get; set; }

        // Thông tin phân trang trên Server
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public int TotalCount { get; set; } = 0;
        public int TotalPages { get; set; } = 1;

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        // Chỉ số thống kê nhanh
        public int TongSoLop { get; set; }
        public int SoLopDangMo { get; set; }
        public int SoLopDaDong { get; set; }
        public int SoLopTamDung { get; set; }

        // Danh sách Dropdown cho Form Lọc
        public SelectList? ChuongTrinhOptions { get; set; }
        public SelectList? GiangVienOptions { get; set; }
    }
}
