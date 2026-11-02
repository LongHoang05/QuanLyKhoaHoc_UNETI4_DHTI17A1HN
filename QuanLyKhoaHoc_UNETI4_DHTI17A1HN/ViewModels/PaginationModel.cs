// ========================================================
// Họ và tên: Nguyễn Đình Kiên
// Mã sinh viên: 23103100020
// Phụ trách: Module 2 - Quản lý lớp học, Tìm kiếm, Lọc, Sắp xếp và Phân trang
// Component phân trang dùng chung toàn hệ thống (Clean Architecture & DRY)
// ========================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels
{
    /// <summary>
    /// ViewModel dùng chung cho Component Phân trang toàn hệ thống (Clean Architecture & DRY)
    /// </summary>
    public class PaginationModel
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalItems { get; set; } = 0;
        public int PageSize { get; set; } = 8;
        public string ItemLabel { get; set; } = "bản ghi";
        public string BaseUrl { get; set; } = "";
        public Dictionary<string, string?> RouteParams { get; set; } = new();

        /// <summary>
        /// Tên hàm JavaScript AJAX nhận vào URL trang mới (ví dụ: 'loadLopHocPage', 'loadKetQuaPage')
        /// </summary>
        public string? AjaxFunction { get; set; }

        /// <summary>
        /// Tên hàm JavaScript nhận trực tiếp số trang (ví dụ: 'changePage' nhận page: int)
        /// </summary>
        public string? ButtonJsFunction { get; set; }
        public string? PageChangeFunction
        {
            get => ButtonJsFunction;
            set => ButtonJsFunction = value;
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public int StartItem => TotalItems == 0 ? 0 : (Math.Max(1, CurrentPage) - 1) * PageSize + 1;
        public int EndItem => Math.Min(Math.Max(1, CurrentPage) * PageSize, TotalItems);

        public string BuildUrl(int page)
        {
            var dict = new Dictionary<string, string?>(RouteParams)
            {
                ["page"] = page.ToString()
            };
            var qs = string.Join("&", dict
                .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value!)}"));
            return string.IsNullOrEmpty(BaseUrl) ? $"?{qs}" : $"{BaseUrl}?{qs}";
        }

        public string GetUrl(int page) => BuildUrl(page);
    }
}
