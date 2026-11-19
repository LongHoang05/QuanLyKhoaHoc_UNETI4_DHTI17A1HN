// ========================================================
// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Phụ trách: Module 4 - Đăng ký khóa học, Quản lý hồ sơ & Thu học phí
// ========================================================

using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Helpers;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    [SessionAuthorize("Admin", "GiaoVu")]
    public class DangKyHocController : Controller
    {
        private readonly IDangKyHocService _dangKyHocService;
        private readonly ILopHocRepository _lopHocRepo;
        private readonly IChuongTrinhDaoTaoRepository _ctdtRepo;

        public DangKyHocController(
            IDangKyHocService dangKyHocService,
            ILopHocRepository lopHocRepo,
            IChuongTrinhDaoTaoRepository ctdtRepo)
        {
            _dangKyHocService = dangKyHocService;
            _lopHocRepo = lopHocRepo;
            _ctdtRepo = ctdtRepo;
        }

        // --- DANH SÁCH HỒ SƠ ĐĂNG KÝ (LINQ MULTI-FILTER, SEARCH, PAGINATION QUA SERVICE & REPOSITORY) ---
        public async Task<IActionResult> Index(
            string? search,
            int? maChuongTrinh,
            int? maLop,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sortBy,
            int page = 1,
            int pageSize = 6)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (string.Equals(vaiTro, "GiangVien", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Giảng viên không có quyền truy cập module Xét duyệt & Thu học phí.";
                return RedirectToAction("Index", "GiangVien");
            }
            if (string.Equals(vaiTro, "HocVien", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("TienTrinh", "CongHocVien", new { tab = "dangky" });
            }

            if (page < 1) page = 1;

            var (items, totalItems) = await _dangKyHocService.GetPagedAsync(
                search,
                maChuongTrinh,
                maLop,
                trangThai,
                tuNgay,
                denNgay,
                sortBy,
                page,
                pageSize);

            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages)
            {
                page = totalPages;
                (items, totalItems) = await _dangKyHocService.GetPagedAsync(
                    search, maChuongTrinh, maLop, trangThai, tuNgay, denNgay, sortBy, page, pageSize);
            }

            var (choDuyetCount, daDuyetCount, tongDoanhThu) = await _dangKyHocService.GetThongKeNhanhAsync();
            ViewBag.ChoDuyetCount = choDuyetCount;
            ViewBag.DaDuyetCount = daDuyetCount;
            ViewBag.TongDoanhThu = tongDoanhThu;

            ViewBag.ChuongTrinhList = await _ctdtRepo.GetAllAsync();
            ViewBag.LopHocList = await _lopHocRepo.GetAllAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.SelectedMaChuongTrinh = maChuongTrinh;
            ViewBag.SelectedMaLop = maLop;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");
            ViewBag.SortBy = sortBy;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DangKyHocTablePartial", items);
            }

            return View(items);
        }

        // --- AJAX: LẤY DANH SÁCH LỚP HỌC THEO CHƯƠNG TRÌNH ĐÀO TẠO ---
        [HttpGet]
        public async Task<IActionResult> GetLopByChuongTrinh(int maChuongTrinh)
        {
            var allLops = await _lopHocRepo.GetAllAsync();
            var lops = allLops
                .Where(l => l.MaChuongTrinh == maChuongTrinh)
                .Select(l => new
                {
                    maLop = l.MaLopHoc,
                    tenLop = l.TenLop,
                    hocPhi = l.HocPhi,
                    trangThai = l.TrangThai,
                    siSoToiDa = l.SiSoToiDa
                })
                .ToList();

            return Json(lops);
        }

        private async Task<object> GetStatsJsonAsync()
        {
            var (choDuyetCount, daDuyetCount, tongDoanhThu) = await _dangKyHocService.GetThongKeNhanhAsync();
            return new
            {
                choDuyetCount,
                daDuyetCount,
                tongDoanhThuFormatted = ((double)tongDoanhThu).ToString("N0") + " đ"
            };
        }

        private async Task<IActionResult> RespondActionAsync(bool success, string message, bool includeStatsOnSuccess = true)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                if (success && includeStatsOnSuccess)
                {
                    return Json(new { success = true, message, stats = await GetStatsJsonAsync() });
                }
                return Json(new { success, message });
            }

            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }

        // --- AJAX: LẤY THÔNG TIN ĐƠN ĐĂNG KÝ ĐỂ HIỂN THỊ MODAL THU HỌC PHÍ ---
        [HttpGet]
        public async Task<IActionResult> GetDangKy(int id)
        {
            var dk = await _dangKyHocService.GetByIdWithDetailsAsync(id);
            if (dk == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    maDangKy = dk.MaDangKy,
                    tenHocVien = dk.HocVien?.HoTen ?? $"HV #{dk.MaHocVien}",
                    tenLop = dk.LopHoc?.TenLop ?? $"Lớp #{dk.MaLopHoc}",
                    hocPhi = dk.LopHoc?.HocPhi ?? 0m,
                    soTienDaDong = dk.SoTienDaDong,
                    trangThai = dk.TrangThai,
                    ghiChu = dk.GhiChu
                }
            });
        }

        // --- DUYỆT HỒ SƠ ĐĂNG KÝ HỌC (Mục 7.2) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DangKyHoc/Duyet")]
        [Route("DangKyHoc/DuyetDangKyPost")]
        public async Task<IActionResult> Duyet(int id, string? nhanXet = null)
        {
            string nguoiDuyet = HttpContext.Session.GetString("HoTen") ?? "Ban Giáo vụ";
            var (success, message) = await _dangKyHocService.DuyetDangKyAsync(id, nhanXet, nguoiDuyet);
            return await RespondActionAsync(success, message);
        }

        // --- XÁC NHẬN THU HỌC PHÍ (Đóng đúng 1 lần 100%) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DangKyHoc/XacNhanHocPhi")]
        [Route("DangKyHoc/ThuHocPhiPost")]
        public async Task<IActionResult> XacNhanHocPhi(int id, decimal? soTien, string? phuongThuc, string? ghiChu)
        {
            string staffName = HttpContext.Session.GetString("HoTen") ?? "Ban Giáo vụ";
            var (success, message) = await _dangKyHocService.XacNhanHocPhiAsync(id, phuongThuc, ghiChu, staffName);
            return await RespondActionAsync(success, message);
        }

        // --- HOÀN TÁC KHI CHƯA NHẬN ĐƯỢC HỌC PHÍ (TRẢ VỀ 'Đã duyệt' ĐỂ HỌC VIÊN NỘP LẠI) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DangKyHoc/ChuaNhanHocPhiPost")]
        public async Task<IActionResult> ChuaNhanHocPhiPost(int id, string? ghiChu)
        {
            string staffName = HttpContext.Session.GetString("HoTen") ?? "Ban Giáo vụ";
            var (success, message) = await _dangKyHocService.HoanTacChuaNhanHocPhiAsync(id, ghiChu, staffName);
            return await RespondActionAsync(success, message);
        }

        // --- CHUYỂN TRẠNG THÁI BẮT ĐẦU HỌC ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DangKyHoc/BatDauHocPost")]
        public async Task<IActionResult> BatDauHocPost(int id)
        {
            var (success, message) = await _dangKyHocService.BatDauHocAsync(id);
            return await RespondActionAsync(success, message);
        }

        // --- TỪ CHỐI HỒ SƠ ĐĂNG KÝ (Mục 7.2) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DangKyHoc/TuChoi")]
        [Route("DangKyHoc/TuChoiDangKyPost")]
        public async Task<IActionResult> TuChoi(int id, string lyDo)
        {
            string staffName = HttpContext.Session.GetString("HoTen") ?? "Giáo vụ";
            var (success, message) = await _dangKyHocService.TuChoiDangKyAsync(id, lyDo, staffName);
            return await RespondActionAsync(success, message);
        }

        // --- HỦY ĐĂNG KÝ (Mục 7.2) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKy(int id, string lyDo)
        {
            string? vaiTro = HttpContext.Session.GetString("VaiTro");
            string staffName = HttpContext.Session.GetString("HoTen") ?? "Giáo vụ";
            var (success, message) = await _dangKyHocService.HuyDangKyAsync(id, lyDo, vaiTro, staffName);
            return await RespondActionAsync(success, message, includeStatsOnSuccess: false);
        }

        // --- XÓA HỒ SƠ ĐĂNG KÝ (DÀNH CHO ADMIN & GIÁO VỤ KIỂM SOÁT) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DangKyHoc/Xoa")]
        [Route("DangKyHoc/DeletePost")]
        public async Task<IActionResult> Xoa(int id)
        {
            var (success, message) = await _dangKyHocService.XoaDangKyAsync(id);
            return await RespondActionAsync(success, message);
        }

        // ========================================================
        // ACTION DELETE MULTIPLE: XÓA NHIỀU HỒ SƠ ĐĂNG KÝ HỌC
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int>? ids)
        {
            var (success, message, deleted, skipped) = await _dangKyHocService.XoaNhieuDangKyAsync(ids);
            if (!success)
            {
                return Json(new { success = false, message });
            }

            return Json(new { success = true, message, deletedCount = deleted, skippedCount = skipped, stats = await GetStatsJsonAsync() });
        }

        // ========================================================
        // XUẤT FILE EXCEL BẰNG CLOSEDXML
        // ========================================================
        [HttpGet]
        public async Task<IActionResult> ExportExcel(
            string? search,
            int? maChuongTrinh,
            int? maLop,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sortBy)
        {
            var (items, _) = await _dangKyHocService.GetPagedAsync(
                search, maChuongTrinh, maLop, trangThai, tuNgay, denNgay, sortBy, 1, 10000);

            string[] headers = { "STT", "Mã ĐK", "Học viên", "Email", "Lớp học", "Chương trình", "Ngày đăng ký", "Học phí (VNĐ)", "Đã đóng (VNĐ)", "Trạng thái", "Ghi chú" };

            var fileBytes = ExcelExportHelper.ExportToExcel(
                "DanhSachDangKyHoc",
                "DANH SÁCH ĐĂNG KÝ HỌC & THU HỌC PHÍ - UNETI",
                $"Tổng số hồ sơ: {items.Count()}",
                headers,
                items,
                (ws, row, stt, dk) =>
                {
                    ws.Cell(row, 1).Value = stt;
                    ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 2).Value = $"#DK{dk.MaDangKy:D3}";
                    ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 3).Value = dk.HocVien?.HoTen ?? "N/A";
                    ws.Cell(row, 4).Value = dk.HocVien?.Email ?? "";
                    ws.Cell(row, 5).Value = dk.LopHoc?.TenLop ?? "N/A";
                    ws.Cell(row, 6).Value = dk.LopHoc?.ChuongTrinhDaoTao?.TenChuongTrinh ?? "";
                    ws.Cell(row, 7).Value = dk.NgayDangKy.ToString("dd/MM/yyyy HH:mm");
                    ws.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var cHocPhi = ws.Cell(row, 8);
                    cHocPhi.Value = dk.LopHoc?.HocPhi ?? 0;
                    cHocPhi.Style.NumberFormat.Format = "#,##0";

                    var cDaDong = ws.Cell(row, 9);
                    cDaDong.Value = dk.SoTienDaDong;
                    cDaDong.Style.NumberFormat.Format = "#,##0";

                    ws.Cell(row, 10).Value = dk.TrangThai;
                    ws.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, 11).Value = dk.NhanXetGiaoVu ?? dk.GhiChu ?? "";
                });

            return File(fileBytes, ExcelExportHelper.ExcelContentType, $"DanhSachDangKyHoc_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }
    }
}
