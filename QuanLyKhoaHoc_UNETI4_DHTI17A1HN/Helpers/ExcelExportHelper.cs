// ========================================================
// Helper xuất file Excel (.xlsx) sử dụng thư viện ClosedXML
// Hệ thống Quản lý Khóa học & Học viên - UNETI (Nhóm 4 - DHTI17A1HN)
// ========================================================

using ClosedXML.Excel;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Helpers
{
    public static class ExcelExportHelper
    {
        public const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        /// <summary>
        /// Tạo file Excel (.xlsx) chuẩn giao diện UNETI từ danh sách dữ liệu bất kỳ
        /// </summary>
        public static byte[] ExportToExcel<T>(
            string sheetName,
            string reportTitle,
            string subtitle,
            string[] headers,
            IEnumerable<T> items,
            Action<IXLWorksheet, int, int, T> populateRow)
        {
            using var workbook = new XLWorkbook();
            string safeSheetName = string.IsNullOrWhiteSpace(sheetName)
                ? "Data"
                : new string(sheetName.Where(c => !"[]:*?/\\".Contains(c)).Take(31).ToArray());

            var ws = workbook.Worksheets.Add(safeSheetName);
            int colCount = headers.Length;

            // 1. Tiêu đề chính (Dòng 1)
            ws.Cell(1, 1).Value = reportTitle;
            var titleRange = ws.Range(1, 1, 1, colCount);
            titleRange.Merge();
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.FontSize = 14;
            titleRange.Style.Font.FontColor = XLColor.FromHtml("#0D47A1");
            titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            titleRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Row(1).Height = 28;

            // 2. Phụ đề / Thời gian xuất (Dòng 2)
            ws.Cell(2, 1).Value = string.IsNullOrWhiteSpace(subtitle)
                ? $"Ngày xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm}"
                : $"{subtitle} • Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}";
            var subRange = ws.Range(2, 1, 2, colCount);
            subRange.Merge();
            subRange.Style.Font.Italic = true;
            subRange.Style.Font.FontSize = 10;
            subRange.Style.Font.FontColor = XLColor.FromHtml("#64748B");
            subRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Row(2).Height = 20;

            // 3. Header bảng (Dòng 4)
            int headerRow = 4;
            ws.Row(headerRow).Height = 24;
            for (int c = 0; c < colCount; c++)
            {
                var cell = ws.Cell(headerRow, c + 1);
                cell.Value = headers[c];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontSize = 11;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0052CC");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            // 4. Đổ dữ liệu từng dòng (Từ dòng 5)
            int currentRow = headerRow + 1;
            int stt = 1;
            var itemList = items?.ToList() ?? new List<T>();

            foreach (var item in itemList)
            {
                ws.Row(currentRow).Height = 20;
                populateRow(ws, currentRow, stt, item);

                if (stt % 2 == 0)
                {
                    ws.Range(currentRow, 1, currentRow, colCount).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                }

                ws.Range(currentRow, 1, currentRow, colCount).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                currentRow++;
                stt++;
            }

            if (itemList.Count == 0)
            {
                ws.Cell(currentRow, 1).Value = "Không có dữ liệu phù hợp.";
                var emptyRange = ws.Range(currentRow, 1, currentRow, colCount);
                emptyRange.Merge();
                emptyRange.Style.Font.Italic = true;
                emptyRange.Style.Font.FontColor = XLColor.FromHtml("#64748B");
                emptyRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                currentRow++;
            }

            // 5. Đường viền bảng và tự động căn chỉnh độ rộng cột
            var tableRange = ws.Range(headerRow, 1, currentRow - 1, colCount);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#CBD5E1");
            tableRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#E2E8F0");

            ws.Columns().AdjustToContents();
            foreach (var col in ws.ColumnsUsed())
            {
                col.Width = Math.Min(Math.Max(col.Width + 3, 10), 48);
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
