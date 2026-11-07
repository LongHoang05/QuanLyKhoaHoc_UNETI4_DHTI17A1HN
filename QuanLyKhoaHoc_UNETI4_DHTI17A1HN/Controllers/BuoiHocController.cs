// ========================================================
// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Phụ trách: Module 5 - Buổi học, Điểm danh, Kết quả học tập & Thống kê báo cáo
// Áp dụng: Repository Pattern theo hướng dẫn UNETI
// Controller không truy vấn trực tiếp DbContext / LINQ
// ========================================================

using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    [SessionAuthorize("Admin", "GiaoVu")]
    public class BuoiHocController : Controller
    {
        private readonly IBuoiHocRepository _buoiHocRepo;
        private readonly ILopHocRepository _lopHocRepo;
        private readonly IDangKyHocRepository _dangKyHocRepo;
        private readonly IGiangVienRepository _giangVienRepo;

        public BuoiHocController(
            IBuoiHocRepository buoiHocRepo,
            ILopHocRepository lopHocRepo,
            IDangKyHocRepository dangKyHocRepo,
            IGiangVienRepository giangVienRepo)
        {
            _buoiHocRepo = buoiHocRepo;
            _lopHocRepo = lopHocRepo;
            _dangKyHocRepo = dangKyHocRepo;
            _giangVienRepo = giangVienRepo;
        }

        public async Task<IActionResult> Index(int? maLop, int? MaLopHoc, string? phong, string? trangThai, string? search, int page = 1)
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (string.Equals(vaiTro, "HocVien", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("TienTrinh", "CongHocVien", new { tab = "tkb" });
            }

            int? targetMaLop = maLop ?? MaLopHoc;
            int pageSize = 8;
            if (page < 1) page = 1;

            // 1. Kiểm tra xung đột trùng lịch qua Repository
            var (conflictIds, soLoiTrung) = await _buoiHocRepo.GetConflictSessionsAsync();
            ViewBag.ConflictIds = conflictIds;
            ViewBag.SoLoiTrung = soLoiTrung;

            // 2. Thống kê KPI qua Repository
            var (tongSoBuoi, daHoanThanh, daLenLich, daHuy) = await _buoiHocRepo.GetThongKeBuoiHocAsync();
            ViewBag.TotalSessions = tongSoBuoi;
            ViewBag.CompletedSessions = daHoanThanh;
            ViewBag.ScheduledSessions = daLenLich;
            ViewBag.CanceledSessions = daHuy;

            // 3. Phân trang & Lọc LINQ qua Repository
            var (buoiHocs, totalRecords) = await _buoiHocRepo.GetPagedAsync(
                search,
                targetMaLop,
                phong,
                trangThai,
                null,
                null,
                "thoigian_desc",
                page,
                pageSize);

            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages)
            {
                page = totalPages;
                (buoiHocs, totalRecords) = await _buoiHocRepo.GetPagedAsync(
                    search, targetMaLop, phong, trangThai, null, null, "thoigian_desc", page, pageSize);
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.PageSize = pageSize;
            ViewBag.SelectedMaLop = targetMaLop;
            ViewBag.SelectedPhong = phong;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.Search = search;

            // 4. Dữ liệu nạp Dropdowns & Lịch tuần
            ViewBag.LopHocList = await _lopHocRepo.GetAllAsync();
            var allSessions = await _buoiHocRepo.GetAllAsync();
            ViewBag.AllBuoiHocs = allSessions;

            var activeSessions = allSessions.Where(b => b.TrangThai != "Đã hủy").ToList();
            ViewBag.SoThi = activeSessions.Count(b => b.NoiDung != null && (b.NoiDung.ToLower().Contains("thi") || b.NoiDung.ToLower().Contains("kiểm tra")));
            ViewBag.SoChinhKhoa = activeSessions.Count(b => b.NoiDung == null || (!b.NoiDung.ToLower().Contains("thi") && !b.NoiDung.ToLower().Contains("kiểm tra")));
            ViewBag.DanhSachPhong = allSessions.Select(b => b.PhongHoc).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            ViewBag.DanhSachGiangVien = allSessions.Select(b => b.GiangVien).Where(g => !string.IsNullOrEmpty(g)).Distinct().ToList();

            return View(buoiHocs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return RedirectToAction("Index", new { create = "true" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BuoiHoc buoiHoc)
        {
            ModelState.Remove("LopHoc");
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            // 1. Kiểm tra lớp có ít nhất 1 đăng ký hợp lệ không? (Mục 9.2)
            int currentEnrolled = await _dangKyHocRepo.CountEnrolledInLopAsync(buoiHoc.MaLopHoc);
            if (currentEnrolled == 0)
            {
                ModelState.AddModelError("", "Lớp này chưa có học viên nào được duyệt hoặc đóng học phí, không được xếp lịch.");
            }

            // 2. Validate thời gian hợp lệ (Mục 9.2)
            if (buoiHoc.ThoiGianBatDau < DateTime.Now)
            {
                ModelState.AddModelError("ThoiGianBatDau", "Thời gian học không được nằm trong quá khứ tại thời điểm xếp lịch.");
            }
            if (buoiHoc.ThoiGianKetThuc <= buoiHoc.ThoiGianBatDau)
            {
                ModelState.AddModelError("ThoiGianKetThuc", "Thời gian kết thúc phải sau thời gian bắt đầu.");
            }

            // 3. Kiểm tra số buổi học vượt quy định (Mục 9.2)
            var lopHoc = await _lopHocRepo.GetByIdAsync(buoiHoc.MaLopHoc);
            if (lopHoc != null)
            {
                int soBuoiChuaHuy = await _buoiHocRepo.CountBuoiHocChuaHuyAsync(buoiHoc.MaLopHoc);
                if (soBuoiChuaHuy >= lopHoc.SoBuoiHoc)
                {
                    ModelState.AddModelError("", $"Lớp này đã xếp đủ {lopHoc.SoBuoiHoc} buổi học theo quy định.");
                }
            }

            // 4. KIỂM TRA TRÙNG LỊCH BẰNG REPOSITORY (Lớp, Phòng, Giảng viên - Mục 9.2)
            var (isTrungLich, conflictMsg, _) = await _buoiHocRepo.KiemTraTrungLichChiTietAsync(
                buoiHoc.MaLopHoc,
                buoiHoc.PhongHoc,
                buoiHoc.GiangVien,
                buoiHoc.ThoiGianBatDau,
                buoiHoc.ThoiGianKetThuc);

            if (isTrungLich)
            {
                ModelState.AddModelError("", conflictMsg ?? "Lịch bị trùng! Kiểm tra lại Giảng viên, Phòng học hoặc Lớp đã có lịch trong khung giờ này.");
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(buoiHoc.TrangThai)) buoiHoc.TrangThai = "Đã lên lịch";
                if (!string.IsNullOrWhiteSpace(buoiHoc.GiangVien))
                {
                    var gvs = await _giangVienRepo.GetAllAsync();
                    var gvMatch = gvs.FirstOrDefault(g => g.HoTen == buoiHoc.GiangVien || buoiHoc.GiangVien.Contains(g.HoTen) || g.HoTen.Contains(buoiHoc.GiangVien));
                    buoiHoc.MaGiangVien = gvMatch?.MaGiangVien ?? (lopHoc?.TenGiangVien == buoiHoc.GiangVien ? lopHoc.MaGiangVien : null);
                }
                await _buoiHocRepo.AddAsync(buoiHoc);

                if (isAjax)
                {
                    return Json(new { success = true, message = "Xếp lịch học thành công!" });
                }
                TempData["Success"] = "Xếp lịch học thành công!";
                return RedirectToAction(nameof(Index));
            }

            string errorMsg = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            if (isAjax)
            {
                return Json(new { success = false, message = errorMsg });
            }
            TempData["Error"] = errorMsg;
            return RedirectToAction(nameof(Index));
        }

        // --- CẬP NHẬT BUỔI HỌC TỪ SLIDE-OVER DRAWER ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(BuoiHoc buoiHoc)
        {
            ModelState.Remove("LopHoc");
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var existing = await _buoiHocRepo.GetByIdAsync(buoiHoc.MaBuoiHoc);
            if (existing == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy buổi học cần chỉnh sửa." });
                TempData["Error"] = "Không tìm thấy buổi học cần chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }

            if (buoiHoc.ThoiGianKetThuc <= buoiHoc.ThoiGianBatDau)
            {
                if (isAjax) return Json(new { success = false, message = "Thời gian kết thúc phải sau thời gian bắt đầu." });
                TempData["Error"] = "Thời gian kết thúc phải sau thời gian bắt đầu.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra trùng lịch ngoại trừ chính buổi học này qua Repository (Mục 9.2)
            var (isTrungLich, conflictMsg, _) = await _buoiHocRepo.KiemTraTrungLichChiTietAsync(
                buoiHoc.MaLopHoc,
                buoiHoc.PhongHoc,
                buoiHoc.GiangVien,
                buoiHoc.ThoiGianBatDau,
                buoiHoc.ThoiGianKetThuc,
                buoiHoc.MaBuoiHoc);

            if (isTrungLich)
            {
                if (isAjax) return Json(new { success = false, message = $"Cập nhật thất bại: {conflictMsg}" });
                TempData["Error"] = $"Cập nhật thất bại: {conflictMsg}";
                return RedirectToAction(nameof(Index));
            }

            existing.MaLopHoc = buoiHoc.MaLopHoc;
            existing.PhongHoc = buoiHoc.PhongHoc;
            existing.GiangVien = buoiHoc.GiangVien;
            if (!string.IsNullOrWhiteSpace(buoiHoc.GiangVien))
            {
                var gvs = await _giangVienRepo.GetAllAsync();
                var gvMatch = gvs.FirstOrDefault(g => g.HoTen == buoiHoc.GiangVien || buoiHoc.GiangVien.Contains(g.HoTen) || g.HoTen.Contains(buoiHoc.GiangVien));
                existing.MaGiangVien = gvMatch?.MaGiangVien;
            }
            else
            {
                existing.MaGiangVien = null;
            }
            existing.ThoiGianBatDau = buoiHoc.ThoiGianBatDau;
            existing.ThoiGianKetThuc = buoiHoc.ThoiGianKetThuc;
            existing.NoiDung = buoiHoc.NoiDung;
            existing.GhiChu = buoiHoc.GhiChu;
            existing.TrangThai = buoiHoc.TrangThai ?? existing.TrangThai;

            await _buoiHocRepo.UpdateAsync(existing);

            if (isAjax) return Json(new { success = true, message = "Cập nhật buổi học thành công!" });
            TempData["Success"] = "Cập nhật buổi học thành công!";
            return RedirectToAction(nameof(Index));
        }

        // --- XÓA BUỔI HỌC TỪ DELETE MODAL ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var buoiHoc = await _buoiHocRepo.GetByIdAsync(id);
            if (buoiHoc == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy buổi học để xóa." });
                TempData["Error"] = "Không tìm thấy buổi học để xóa.";
                return RedirectToAction(nameof(Index));
            }

            await _buoiHocRepo.DeleteAsync(id);

            if (isAjax) return Json(new { success = true, message = "Đã xóa vĩnh viễn buổi học khỏi hệ thống." });
            TempData["Success"] = "Đã xóa vĩnh viễn buổi học khỏi hệ thống.";
            return RedirectToAction(nameof(Index));
        }

        // --- CẬP NHẬT HOÀN THÀNH SAU BUỔI HỌC (Yêu cầu 9.3) ---
        [HttpPost]
        public async Task<IActionResult> HoanThanh(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var buoiHoc = await _buoiHocRepo.GetByIdAsync(id);
            if (buoiHoc == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy buổi học." });
                return NotFound();
            }

            if (buoiHoc.TrangThai == "Đã hủy")
            {
                if (isAjax) return Json(new { success = false, message = "Không thể hoàn thành buổi học đã hủy." });
                TempData["Error"] = "Không thể hoàn thành buổi học đã hủy.";
                return RedirectToAction(nameof(Index));
            }

            if (buoiHoc.ThoiGianKetThuc > DateTime.Now)
            {
                if (isAjax) return Json(new { success = false, message = "Buổi học chưa kết thúc (hoặc chưa diễn ra), không thể đánh dấu hoàn thành." });
                TempData["Error"] = "Buổi học chưa kết thúc (hoặc chưa diễn ra), không thể đánh dấu hoàn thành.";
                return RedirectToAction(nameof(Index));
            }

            buoiHoc.TrangThai = "Đã hoàn thành";
            await _buoiHocRepo.UpdateAsync(buoiHoc);

            if (isAjax) return Json(new { success = true, message = "Đã cập nhật trạng thái hoàn thành thành công." });
            TempData["Success"] = "Đã cập nhật trạng thái hoàn thành thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Huy(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var buoiHoc = await _buoiHocRepo.GetByIdAsync(id);
            if (buoiHoc == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy buổi học." });
                return NotFound();
            }

            buoiHoc.TrangThai = "Đã hủy";
            await _buoiHocRepo.UpdateAsync(buoiHoc);

            if (isAjax) return Json(new { success = true, message = "Đã hủy ca học thành công." });
            TempData["Success"] = "Đã hủy ca học thành công.";
            return RedirectToAction(nameof(Index));
        }

        // --- API KIỂM TRA TRÙNG LỊCH REALTIME TRÊN FORM / DRAWER ---
        [HttpGet]
        public async Task<IActionResult> KiemTraTrungLich(int? maBuoiHoc, int maLopHoc, string phongHoc, string? giangVien, DateTime? batDau, DateTime? ketThuc)
        {
            if (maLopHoc <= 0 || string.IsNullOrWhiteSpace(phongHoc) || !batDau.HasValue || !ketThuc.HasValue)
            {
                return Json(new { hasConflict = false, message = "Chưa đủ thông tin kiểm tra." });
            }

            if (ketThuc.Value <= batDau.Value)
            {
                return Json(new { hasConflict = true, message = "Thời gian kết thúc phải sau thời gian bắt đầu." });
            }

            var (hasConflict, conflictMsg, _) = await _buoiHocRepo.KiemTraTrungLichChiTietAsync(
                maLopHoc,
                phongHoc.Trim(),
                giangVien?.Trim(),
                batDau.Value,
                ketThuc.Value,
                maBuoiHoc > 0 ? maBuoiHoc : null);

            return Json(new { hasConflict = hasConflict, message = conflictMsg ?? "Khung giờ và phòng học hoàn toàn trống!" });
        }

        // --- TỰ ĐỘNG XỬ LÝ DỌN DẸP CÁC CA TRÙNG LỊCH TRONG DB ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XuLyTrungLich()
        {
            int soCaDaXuLy = await _buoiHocRepo.XuLyCaTrungLichTuDongAsync();
            TempData["Success"] = $"Đã tự động tối ưu và giải phóng {soCaDaXuLy} ca học trùng lịch thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
