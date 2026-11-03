// ========================================================
// Họ và tên: Nguyễn Đình Kiên
// Mã sinh viên: 23103100020
// Phụ trách: Module 2 - Quản lý lớp học, Tìm kiếm, Lọc, Sắp xếp và Phân trang
// ========================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    [SessionAuthorize("Admin", "GiaoVu")]
    public class LopHocController : Controller
    {
        private readonly ILopHocRepository _lopHocRepo;
        private readonly IChuongTrinhDaoTaoRepository _ctdtRepo;
        private readonly IDangKyHocRepository _dangKyHocRepo;
        private readonly IBuoiHocRepository _buoiHocRepo;
        private readonly IGiangVienRepository _giangVienRepo;

        public LopHocController(
            ILopHocRepository lopHocRepo,
            IChuongTrinhDaoTaoRepository ctdtRepo,
            IDangKyHocRepository dangKyHocRepo,
            IBuoiHocRepository buoiHocRepo,
            IGiangVienRepository giangVienRepo)
        {
            _lopHocRepo = lopHocRepo;
            _ctdtRepo = ctdtRepo;
            _dangKyHocRepo = dangKyHocRepo;
            _buoiHocRepo = buoiHocRepo;
            _giangVienRepo = giangVienRepo;
        }

        // --- DANH SÁCH LỚP HỌC (SEARCH + FILTER + SORT + PAGINATION QUA REPOSITORY) ---
        public async Task<IActionResult> Index(
            int page = 1,
            string search = "",
            int? maChuongTrinh = null,
            string trinhDo = "",
            string trangThai = "",
            decimal? hocPhiMin = null,
            decimal? hocPhiMax = null,
            string hanDangKyStatus = "",
            string choTrongStatus = "",
            string sortBy = "")
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (string.Equals(vaiTro, "HocVien", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("DangKy", "CongHocVien");
            }

            int pageSize = 6;

            // Gọi Repository thực hiện toàn bộ câu lệnh LINQ Skip & Take trên SQL Server
            var (items, totalItems) = await _lopHocRepo.GetPagedAsync(
                search,
                maChuongTrinh,
                trinhDo,
                trangThai,
                null,
                hocPhiMin,
                hocPhiMax,
                hanDangKyStatus,
                choTrongStatus,
                sortBy,
                page,
                pageSize);

            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var pageLopIds = items.Select(l => l.MaLopHoc).ToList();
            var siSoMap = await _dangKyHocRepo.GetSiSoDaDangKyMapAsync(pageLopIds);
            var buoiHocMap = await _buoiHocRepo.GetBuoiHocChuaHuyMapAsync(pageLopIds);

            // Gán ViewBag phục vụ Razor View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.Search = search;
            ViewBag.SelectedMaChuongTrinh = maChuongTrinh;
            ViewBag.SelectedTrinhDo = trinhDo;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.HocPhiMin = hocPhiMin;
            ViewBag.HocPhiMax = hocPhiMax;
            ViewBag.HanDangKyStatus = hanDangKyStatus;
            ViewBag.ChoTrongStatus = choTrongStatus;
            ViewBag.SortBy = sortBy;

            ViewBag.SiSoMap = siSoMap;
            ViewBag.BuoiHocMap = buoiHocMap;
            ViewBag.ChuongTrinhList = await _ctdtRepo.GetAllAsync();
            ViewBag.GiangVienList = await _giangVienRepo.GetAllAsync();

            // 3 thẻ KPI lấy từ Repository
            var (tongSoLop, soLopDangMo, _, _) = await _lopHocRepo.GetThongKeNhanhAsync();
            var (_, daDuyetCount, _) = await _dangKyHocRepo.GetThongKeNhanhAsync();
            ViewBag.TongSoLop = tongSoLop;
            ViewBag.LopDangMo = soLopDangMo;
            ViewBag.TongHocVienDangHoc = daDuyetCount;

            return View(items);
        }

        // --- CHI TIẾT LỚP HỌC ---
        public async Task<IActionResult> Details(int id)
        {
            var lop = await _lopHocRepo.GetByIdWithDetailsAsync(id);
            if (lop == null) return NotFound();

            var buoiHocs = await _buoiHocRepo.GetByLopHocIdAsync(id);
            var dangKys = lop.DangKyHocs
                .Where(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học")
                .ToList();

            ViewBag.BuoiHocs = buoiHocs;
            ViewBag.DangKys = dangKys;
            ViewBag.SiSoHienTai = dangKys.Count;

            return View(lop);
        }

        // --- CREATE LỚP HỌC (DRAWER AJAX) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(LopHoc lop)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            // Kiểm tra ràng buộc nghiệp vụ (Mục 6.2)
            if (string.IsNullOrWhiteSpace(lop.TenLop))
            {
                if (isAjax) return Json(new { success = false, message = "Tên lớp học bắt buộc nhập." });
                TempData["Error"] = "Tên lớp học bắt buộc nhập.";
                return RedirectToAction(nameof(Index));
            }

            if (lop.SiSoToiDa <= 0)
            {
                if (isAjax) return Json(new { success = false, message = "Sĩ số tối đa phải lớn hơn 0." });
                TempData["Error"] = "Sĩ số tối đa phải lớn hơn 0.";
                return RedirectToAction(nameof(Index));
            }

            if (lop.SoBuoiHoc <= 0)
            {
                if (isAjax) return Json(new { success = false, message = "Số buổi học phải lớn hơn 0." });
                TempData["Error"] = "Số buổi học phải lớn hơn 0.";
                return RedirectToAction(nameof(Index));
            }

            if (lop.HocPhi < 0)
            {
                if (isAjax) return Json(new { success = false, message = "Học phí phải lớn hơn hoặc bằng 0." });
                TempData["Error"] = "Học phí phải lớn hơn hoặc bằng 0.";
                return RedirectToAction(nameof(Index));
            }

            if (lop.HanDangKy < lop.NgayBatDauDangKy)
            {
                if (isAjax) return Json(new { success = false, message = "Hạn đăng ký phải sau hoặc bằng ngày bắt đầu đăng ký." });
                TempData["Error"] = "Hạn đăng ký không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (lop.NgayKhaiGiang < lop.HanDangKy)
            {
                if (isAjax) return Json(new { success = false, message = "Ngày khai giảng phải sau hoặc bằng hạn đăng ký." });
                TempData["Error"] = "Ngày khai giảng không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrEmpty(lop.TrangThai)) lop.TrangThai = "Chưa mở";
            if (string.IsNullOrEmpty(lop.TrinhDoDauVao)) lop.TrinhDoDauVao = "Cơ bản";
            if (string.IsNullOrEmpty(lop.TenGiangVien)) lop.TenGiangVien = "Chưa xếp";

            if (!string.IsNullOrEmpty(lop.TenGiangVien) && lop.TenGiangVien != "Chưa xếp")
            {
                var gvs = await _giangVienRepo.GetAllAsync();
                var gvMatch = gvs.FirstOrDefault(g => g.HoTen == lop.TenGiangVien || lop.TenGiangVien.Contains(g.HoTen) || g.HoTen.Contains(lop.TenGiangVien));
                if (gvMatch != null)
                {
                    lop.MaGiangVien = gvMatch.MaGiangVien;
                }
            }

            await _lopHocRepo.AddAsync(lop);

            if (isAjax) return Json(new { success = true, message = "Tạo lớp học mới thành công!" });
            TempData["Success"] = "Tạo lớp học mới thành công!";
            return RedirectToAction(nameof(Index));
        }

        // --- EDIT LỚP HỌC (DRAWER AJAX) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(
            int maLop,
            string tenLop,
            int maChuongTrinh,
            string? tenGiangVien,
            string? trinhDoDauVao,
            int siSoToiDa,
            int soBuoiHoc,
            decimal hocPhi,
            DateTime ngayBatDauDangKy,
            DateTime hanDangKy,
            DateTime ngayKhaiGiang,
            string? moTa,
            string? yeuCauHocVien,
            string trangThai)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var lop = await _lopHocRepo.GetByIdAsync(maLop);
            if (lop == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy lớp học cần cập nhật." });
                TempData["Error"] = "Không tìm thấy lớp học.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra ràng buộc nghiệp vụ Mục 6.2: Không cho giảm sĩ số tối đa xuống thấp hơn số học viên đã duyệt
            int soHocVienDaDuyet = await _dangKyHocRepo.CountEnrolledInLopAsync(maLop);
            if (siSoToiDa < soHocVienDaDuyet)
            {
                string msg = $"Không thể giảm sĩ số tối đa xuống {siSoToiDa} vì lớp đang có {soHocVienDaDuyet} học viên đã được duyệt/đóng học phí.";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            if (hanDangKy < ngayBatDauDangKy)
            {
                if (isAjax) return Json(new { success = false, message = "Hạn đăng ký phải sau hoặc bằng ngày bắt đầu đăng ký." });
                TempData["Error"] = "Hạn đăng ký không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (ngayKhaiGiang < hanDangKy)
            {
                if (isAjax) return Json(new { success = false, message = "Ngày khai giảng phải sau hoặc bằng hạn đăng ký." });
                TempData["Error"] = "Ngày khai giảng không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            lop.TenLop = tenLop.Trim();
            lop.MaChuongTrinh = maChuongTrinh;
            lop.TenGiangVien = tenGiangVien ?? "Chưa xếp";

            if (!string.IsNullOrEmpty(tenGiangVien) && tenGiangVien != "Chưa xếp")
            {
                var gvs = await _giangVienRepo.GetAllAsync();
                var gvMatch = gvs.FirstOrDefault(g => g.HoTen == tenGiangVien || tenGiangVien.Contains(g.HoTen) || g.HoTen.Contains(tenGiangVien));
                if (gvMatch != null)
                {
                    lop.MaGiangVien = gvMatch.MaGiangVien;
                }
            }
            else
            {
                lop.MaGiangVien = null;
            }

            lop.TrinhDoDauVao = trinhDoDauVao ?? "Cơ bản";
            lop.SiSoToiDa = siSoToiDa;
            lop.SoBuoiHoc = soBuoiHoc;
            lop.HocPhi = hocPhi;
            lop.NgayBatDauDangKy = ngayBatDauDangKy;
            lop.HanDangKy = hanDangKy;
            lop.NgayKhaiGiang = ngayKhaiGiang;
            lop.MoTa = moTa ?? "";
            lop.YeuCauHocVien = yeuCauHocVien ?? "";
            lop.TrangThai = trangThai;

            await _lopHocRepo.UpdateAsync(lop);

            if (isAjax) return Json(new { success = true, message = "Cập nhật thông tin lớp học thành công!" });
            TempData["Success"] = "Cập nhật thông tin lớp học thành công!";
            return RedirectToAction(nameof(Index));
        }

        // --- XÓA LỚP HỌC (AN TOÀN KIỂM SOÁT DỮ LIỆU QUA REPOSITORY) ---
        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var lop = await _lopHocRepo.GetByIdAsync(id);
            if (lop == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy lớp học." });
                return RedirectToAction(nameof(Index));
            }

            // Quy định nghiệp vụ Mục 6.2: Khi lớp đã có đăng ký, việc xóa phải được kiểm soát
            int soDangKy = await _dangKyHocRepo.CountEnrolledInLopAsync(id);
            if (soDangKy > 0)
            {
                string msg = $"Không thể xóa lớp '{lop.TenLop}' vì đã có {soDangKy} hồ sơ đăng ký của học viên liên quan. Bạn có thể chuyển trạng thái lớp sang 'Đã đóng đăng ký'.";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra các buổi học đã xếp
            int soBuoi = await _buoiHocRepo.CountBuoiHocChuaHuyAsync(id);
            if (soBuoi > 0)
            {
                string msg = $"Không thể xóa lớp '{lop.TenLop}' vì đang có {soBuoi} buổi học trong lịch học. Vui lòng hủy các buổi học trước.";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            await _lopHocRepo.DeleteAsync(id);

            if (isAjax) return Json(new { success = true, message = $"Đã xóa lớp học '{lop.TenLop}' thành công." });
            TempData["Success"] = "Đã xóa lớp học thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // BÀI TẬP THỰC HÀNH JAVASCRIPT, JQUERY, AJAX (BÀI 11 - BÀI 15)
        // ========================================================

        /// <summary>
        /// Bài 11: AJAX đơn giản tải chuỗi dữ liệu từ Server mà không tải lại trang
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult XinChao()
        {
            return Content($"Xin chào từ Server ASP.NET Core! [UNETI AJAX Response - Thời gian: {DateTime.Now:HH:mm:ss dd/MM/yyyy}]");
        }

        /// <summary>
        /// Bài 12 - 15: Tìm kiếm theo từ khóa (GET), Lọc theo danh mục Dropdown onchange, Tìm kiếm thời gian thực keyup
        /// Trả về PartialView khi gọi bằng AJAX, hoặc View đầy đủ khi truy cập qua URL
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TimKiemAjax(string? keyword, int? maChuongTrinh)
        {
            var dsLop = await _lopHocRepo.SearchAjaxAsync(keyword, maChuongTrinh);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DanhSachLopPartial", dsLop);
            }

            ViewBag.ChuongTrinhList = await _lopHocRepo.GetChuongTrinhListAsync();
            ViewBag.Keyword = keyword;
            ViewBag.MaChuongTrinh = maChuongTrinh;
            return View(dsLop);
        }

        /// <summary>
        /// Bài 13: AJAX gửi dữ liệu bằng phương thức POST
        /// </summary>
        [HttpPost]
        [ActionName("TimKiemAjax")]
        [AllowAnonymous]
        public async Task<IActionResult> TimKiemAjaxPost(string? keyword, int? maChuongTrinh)
        {
            var dsLop = await _lopHocRepo.SearchAjaxAsync(keyword, maChuongTrinh);
            return PartialView("_DanhSachLopPartial", dsLop);
        }
    }
}
