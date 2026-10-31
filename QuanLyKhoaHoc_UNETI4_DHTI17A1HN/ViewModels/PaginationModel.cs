using System;
using System.Collections.Generic;

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

        public int StartItem => TotalItems == 0 ? 0 : (Math.Max(1, CurrentPage) - 1) * PageSize + 1;
        public int EndItem => Math.Min(Math.Max(1, CurrentPage) * PageSize, TotalItems);

        public string BuildUrl(int page)
        {
            var queryList = new List<string> { $"page={page}" };
            foreach (var kvp in RouteParams)
            {
                if (!string.IsNullOrWhiteSpace(kvp.Value) && !string.Equals(kvp.Key, "page", StringComparison.OrdinalIgnoreCase))
                {
                    queryList.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
                }
            }
            string qs = string.Join("&", queryList);
            return string.IsNullOrEmpty(BaseUrl) ? $"?{qs}" : $"{BaseUrl}?{qs}";
        }
    }
}
