// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Quản lý Chương trình đào tạo & Phân quyền
// Áp dụng: Repository Pattern theo hướng dẫn UNETI
// Controller không truy vấn trực tiếp DbContext / LINQ
// ========================================================

using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    [QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters.SessionAuthorize("Admin", "GiaoVu")]
    public class ChuongTrinhDaoTaoController : Controller
    {
        private readonly IChuongTrinhDaoTaoRepository _ctdtRepository;

        public ChuongTrinhDaoTaoController(IChuongTrinhDaoTaoRepository ctdtRepository)
        {
            _ctdtRepository = ctdtRepository;
        }

        public async Task<IActionResult> Index(int page = 1, string search = "", string trangThai = "")
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (string.Equals(vaiTro, "GiangVien", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Giảng viên không có quyền quản lý Chương trình đào tạo.";
                return RedirectToAction("Index", "GiangVien");
            }
            if (string.Equals(vaiTro, "HocVien", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("DangKy", "CongHocVien");
            }

            int pageSize = 8;
            if (page < 1) page = 1;
            bool? filterTrangThai = null;
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                if (trangThai == "Đang hoạt động" || trangThai == "1" || trangThai.ToLower() == "true")
                {
                    filterTrangThai = true;
                }
                else if (trangThai == "Tạm dừng" || trangThai == "0" || trangThai.ToLower() == "false")
                {
                    filterTrangThai = false;
                }
            }

            var (items, totalItems) = await _ctdtRepository.GetPagedAsync(search, filterTrangThai, page, pageSize);

            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages)
            {
                page = totalPages;
                (items, totalItems) = await _ctdtRepository.GetPagedAsync(search, filterTrangThai, page, pageSize);
            }

            var ctdtIds = items.Select(c => c.MaChuongTrinh).ToList();
            var classCounts = await _ctdtRepository.GetClassCountsByProgramAsync(ctdtIds);

            ViewBag.ClassCounts = classCounts;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.SelectedTrangThai = trangThai;

            // 3 thẻ KPI từ Repository
            var (tongCTDT, dangHoatDong, tongLopLienKet) = await _ctdtRepository.GetKpiStatsAsync();
            ViewBag.TongCTDT = tongCTDT;
            ViewBag.DangHoatDong = dangHoatDong;
            ViewBag.TongLopLienKet = tongLopLienKet;

            return View(items);
        }

        // --- CHI TIẾT CHƯƠNG TRÌNH ĐÀO TẠO ---
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (string.Equals(vaiTro, "GiangVien", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Giảng viên không có quyền quản lý Chương trình đào tạo.";
                return RedirectToAction("Index", "GiangVien");
            }
            if (string.Equals(vaiTro, "HocVien", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("DangKy", "CongHocVien");
            }

            var ctdt = await _ctdtRepository.GetByIdWithLopHocsAsync(id);
            if (ctdt == null)
            {
                TempData["Error"] = $"Không tìm thấy chương trình đào tạo #{id:D3}.";
                return RedirectToAction(nameof(Index));
            }

            return View(ctdt);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return RedirectToAction(nameof(Index), new { create = "true" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([Bind("TenChuongTrinh,MoTa,EmailLienHe")] ChuongTrinhDaoTao ctdt, string? TrangThai)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (string.IsNullOrWhiteSpace(ctdt.TenChuongTrinh))
            {
                if (isAjax) return Json(new { success = false, message = "Tên chương trình đào tạo bắt buộc nhập." });
                TempData["Error"] = "Tên chương trình đào tạo bắt buộc nhập.";
                return RedirectToAction(nameof(Index));
            }

            ctdt.TenChuongTrinh = ctdt.TenChuongTrinh.Trim();
            bool exists = await _ctdtRepository.ExistsTenChuongTrinhAsync(ctdt.TenChuongTrinh);
            if (exists)
            {
                if (isAjax) return Json(new { success = false, message = "Tên chương trình đào tạo này đã tồn tại." });
                TempData["Error"] = "Tên chương trình đào tạo này đã tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            ctdt.TrangThai = string.IsNullOrEmpty(TrangThai) ||
                             TrangThai == "Đang hoạt động" ||
                             TrangThai == "1" ||
                             TrangThai.Equals("true", StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(ctdt.EmailLienHe)) ctdt.EmailLienHe = "contact@uneti.edu.vn";

            await _ctdtRepository.AddAsync(ctdt);

            if (isAjax) return Json(new { success = true, message = "Tạo chương trình đào tạo mới thành công!" });
            TempData["Success"] = "Tạo chương trình đào tạo mới thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int maChuongTrinh, string? tenChuongTrinh, string? moTa, string? emailLienHe, string? trangThai)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var ctdt = await _ctdtRepository.GetByIdAsync(maChuongTrinh);
            if (ctdt == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy chương trình đào tạo cần sửa." });
                TempData["Error"] = "Không tìm thấy chương trình đào tạo.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(tenChuongTrinh))
            {
                if (isAjax) return Json(new { success = false, message = "Tên chương trình đào tạo bắt buộc nhập." });
                TempData["Error"] = "Tên chương trình đào tạo bắt buộc nhập.";
                return RedirectToAction(nameof(Index));
            }

            tenChuongTrinh = tenChuongTrinh.Trim();
            bool exists = await _ctdtRepository.ExistsTenChuongTrinhAsync(tenChuongTrinh, maChuongTrinh);
            if (exists)
            {
                if (isAjax) return Json(new { success = false, message = "Tên chương trình đào tạo này đã trùng với chương trình khác." });
                TempData["Error"] = "Tên chương trình đào tạo này đã trùng.";
                return RedirectToAction(nameof(Index));
            }

            ctdt.TenChuongTrinh = tenChuongTrinh;
            ctdt.MoTa = moTa ?? "";
            ctdt.EmailLienHe = string.IsNullOrWhiteSpace(emailLienHe) ? "contact@uneti.edu.vn" : emailLienHe.Trim();
            ctdt.TrangThai = string.IsNullOrEmpty(trangThai) ||
                             trangThai == "Đang hoạt động" ||
                             trangThai == "1" ||
                             trangThai.Equals("true", StringComparison.OrdinalIgnoreCase);

            await _ctdtRepository.UpdateAsync(ctdt);

            if (isAjax) return Json(new { success = true, message = "Cập nhật chương trình đào tạo thành công!" });
            TempData["Success"] = "Cập nhật chương trình đào tạo thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var ctdt = await _ctdtRepository.GetByIdAsync(id);
            if (ctdt == null)
            {
                if (isAjax) return Json(new { success = false, message = "Chương trình đào tạo không tồn tại." });
                return RedirectToAction(nameof(Index));
            }

            // Quy định nghiệp vụ Mục 5.5: Không được xóa chương trình nếu có lớp học liên quan
            int countLop = await _ctdtRepository.CountLopHocByChuongTrinhAsync(id);
            if (countLop > 0)
            {
                string msg = $"Không thể xóa chương trình '{ctdt.TenChuongTrinh}' vì đang có {countLop} lớp học trực thuộc. Vui lòng chuyển hoặc xóa các lớp học trước.";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            await _ctdtRepository.DeleteAsync(id);

            if (isAjax) return Json(new { success = true, message = $"Đã xóa chương trình đào tạo '{ctdt.TenChuongTrinh}' thành công." });
            TempData["Success"] = "Đã xóa chương trình đào tạo thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // ACTION DELETE MULTIPLE: XÓA NHIỀU CHƯƠNG TRÌNH ĐÀO TẠO
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int>? ids)
        {
            if (ids == null || !ids.Any())
                return Json(new { success = false, message = "Vui lòng chọn ít nhất 1 chương trình đào tạo để xóa." });

            int deleted = 0;
            int skipped = 0;

            foreach (var id in ids)
            {
                var ctdt = await _ctdtRepository.GetByIdAsync(id);
                if (ctdt == null) continue;

                int countLop = await _ctdtRepository.CountLopHocByChuongTrinhAsync(id);
                if (countLop > 0)
                {
                    skipped++;
                    continue;
                }

                await _ctdtRepository.DeleteAsync(id);
                deleted++;
            }

            if (deleted == 0 && skipped > 0)
            {
                return Json(new { success = false, message = $"Không thể xóa {skipped} chương trình đã chọn vì đang có lớp học trực thuộc." });
            }

            string msg = $"Đã xóa thành công {deleted} chương trình đào tạo.";
            if (skipped > 0)
            {
                msg += $" Bỏ qua {skipped} chương trình do đang có lớp học liên kết.";
            }
            return Json(new { success = true, message = msg, deletedCount = deleted, skippedCount = skipped });
        }
    }
}
