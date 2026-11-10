// ========================================================
// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Phụ trách: Module 4 - Đăng ký khóa học, Quản lý hồ sơ & Thu học phí
// ========================================================

using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    [SessionAuthorize("Admin", "GiaoVu")]
    public class DangKyHocController : Controller
    {
        private readonly IDangKyHocRepository _dangKyHocRepo;
        private readonly ILopHocRepository _lopHocRepo;
        private readonly IChuongTrinhDaoTaoRepository _ctdtRepo;
        private readonly IBuoiHocRepository _buoiHocRepo;

        public DangKyHocController(
            IDangKyHocRepository dangKyHocRepo,
            ILopHocRepository lopHocRepo,
            IChuongTrinhDaoTaoRepository ctdtRepo,
            IBuoiHocRepository buoiHocRepo)
        {
            _dangKyHocRepo = dangKyHocRepo;
            _lopHocRepo = lopHocRepo;
            _ctdtRepo = ctdtRepo;
            _buoiHocRepo = buoiHocRepo;
        }

        // --- DANH SÁCH HỒ SƠ ĐĂNG KÝ (LINQ MULTI-FILTER, SEARCH, PAGINATION QUA REPOSITORY) ---
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

            // Gọi Repository thực hiện toàn bộ câu lệnh truy vấn LINQ
            var (items, totalItems) = await _dangKyHocRepo.GetPagedAsync(
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
                (items, totalItems) = await _dangKyHocRepo.GetPagedAsync(
                    search, maChuongTrinh, maLop, trangThai, tuNgay, denNgay, sortBy, page, pageSize);
            }

            // Thống kê nhanh chỉ số qua Repository
            var (choDuyetCount, daDuyetCount, tongDoanhThu) = await _dangKyHocRepo.GetThongKeNhanhAsync();
            ViewBag.ChoDuyetCount = choDuyetCount;
            ViewBag.DaDuyetCount = daDuyetCount;
            ViewBag.TongDoanhThu = tongDoanhThu;

            // Danh mục Dropdown
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
            var (choDuyetCount, daDuyetCount, tongDoanhThu) = await _dangKyHocRepo.GetThongKeNhanhAsync();
            return new
            {
                choDuyetCount,
                daDuyetCount,
                tongDoanhThuFormatted = ((double)tongDoanhThu).ToString("N0") + " đ"
            };
        }

        // --- AJAX: LẤY THÔNG TIN ĐƠN ĐĂNG KÝ ĐỂ HIỂN THỊ MODAL THU HỌC PHÍ ---
        [HttpGet]
        public async Task<IActionResult> GetDangKy(int id)
        {
            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(id);
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
                    soTienDaDong = dk.SoTienDaDong
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
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(id);

            if (dk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });
                TempData["Error"] = "Không tìm thấy hồ sơ đăng ký.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra sĩ số tối đa của lớp học trước khi duyệt
            if (dk.LopHoc != null)
            {
                int currentEnrolled = await _dangKyHocRepo.CountEnrolledInLopAsync(dk.MaLopHoc);
                if (currentEnrolled >= dk.LopHoc.SiSoToiDa)
                {
                    string msg = $"Không thể duyệt hồ sơ #{id}! Lớp '{dk.LopHoc.TenLop}' đã đủ sĩ số tối đa ({dk.LopHoc.SiSoToiDa} học viên).";
                    if (isAjax) return Json(new { success = false, message = msg });
                    TempData["Error"] = msg;
                    return RedirectToAction(nameof(Index));
                }
            }

            // Cập nhật trạng thái
            string nguoiDuyet = HttpContext.Session.GetString("HoTen") ?? "Ban Giáo vụ";
            dk.TrangThai = "Đã duyệt";
            dk.NgayXuLy = DateTime.Now;
            dk.NhanXetGiaoVu = string.IsNullOrWhiteSpace(nhanXet) ? $"Đã duyệt bởi {nguoiDuyet}" : nhanXet.Trim();

            await _dangKyHocRepo.UpdateAsync(dk);

            string successMsg = $"Đã duyệt hồ sơ đăng ký #{id:D3} của học viên {dk.HocVien?.HoTen} thành công!";
            if (isAjax) return Json(new { success = true, message = successMsg, stats = await GetStatsJsonAsync() });
            TempData["Success"] = successMsg;
            return RedirectToAction(nameof(Index));
        }

        // --- XÁC NHẬN THU HỌC PHÍ (Đóng đúng 1 lần 100%) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DangKyHoc/XacNhanHocPhi")]
        [Route("DangKyHoc/ThuHocPhiPost")]
        public async Task<IActionResult> XacNhanHocPhi(int id, decimal? soTien, string? phuongThuc, string? ghiChu)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(id);

            if (dk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });
                TempData["Error"] = "Không tìm thấy hồ sơ.";
                return RedirectToAction(nameof(Index));
            }

            decimal hocPhiLop = dk.LopHoc?.HocPhi ?? 0m;

            if (dk.TrangThai == "Đã đóng học phí" || (hocPhiLop > 0 && dk.SoTienDaDong >= hocPhiLop))
            {
                string paidMsg = "Học viên này đã đóng đủ 100% học phí của lớp.";
                if (isAjax) return Json(new { success = false, message = paidMsg });
                TempData["Error"] = paidMsg;
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra sĩ số nếu hồ sơ trước đó chưa nằm trong nhóm đã duyệt
            if (dk.TrangThai == "Chờ duyệt" && dk.LopHoc != null)
            {
                int enrolledCount = await _dangKyHocRepo.CountEnrolledInLopAsync(dk.MaLopHoc);
                if (enrolledCount >= dk.LopHoc.SiSoToiDa)
                {
                    string msg = $"Không thể thu tiền! Lớp '{dk.LopHoc.TenLop}' đã đạt sĩ số tối đa ({dk.LopHoc.SiSoToiDa} học viên).";
                    if (isAjax) return Json(new { success = false, message = msg });
                    TempData["Error"] = msg;
                    return RedirectToAction(nameof(Index));
                }
            }

            // Thu đúng 1 lần 100% học phí của lớp
            dk.SoTienDaDong = hocPhiLop;
            dk.NgayDongHocPhi = DateTime.Now;
            dk.TrangThai = "Đã đóng học phí";

            string method = !string.IsNullOrWhiteSpace(phuongThuc) ? $"[{phuongThuc.Trim()}] " : "";
            string staffName = HttpContext.Session.GetString("HoTen") ?? "Ban Giáo vụ";

            dk.GhiChu = string.IsNullOrWhiteSpace(ghiChu)
                ? $"{method}Đã thu đủ 100% học phí ({hocPhiLop:N0} đ) bởi {staffName}"
                : $"{method}{ghiChu.Trim()} (Thu bởi: {staffName})";

            if (dk.NgayXuLy == null)
            {
                dk.NgayXuLy = DateTime.Now;
                dk.NhanXetGiaoVu = staffName;
            }

            await _dangKyHocRepo.UpdateAsync(dk);

            string sMsg = $"Xác nhận thu đủ 100% học phí ({hocPhiLop:N0} đ) cho học viên {dk.HocVien?.HoTen} thành công!";
            if (isAjax) return Json(new { success = true, message = sMsg, stats = await GetStatsJsonAsync() });
            TempData["Success"] = sMsg;
            return RedirectToAction(nameof(Index));
        }

        // --- CHUYỂN TRẠNG THÁI BẮT ĐẦU HỌC ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DangKyHoc/BatDauHocPost")]
        public async Task<IActionResult> BatDauHocPost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(id);
            if (dk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });
                TempData["Error"] = "Không tìm thấy hồ sơ.";
                return RedirectToAction(nameof(Index));
            }

            dk.TrangThai = "Đang học";
            await _dangKyHocRepo.UpdateAsync(dk);

            string msg = $"Đã chuyển học viên {dk.HocVien?.HoTen} sang trạng thái Đang học!";
            if (isAjax) return Json(new { success = true, message = msg, stats = await GetStatsJsonAsync() });
            TempData["Success"] = msg;
            return RedirectToAction(nameof(Index));
        }

        // --- TỪ CHỐI HỒ SƠ ĐĂNG KÝ (Mục 7.2) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DangKyHoc/TuChoi")]
        [Route("DangKyHoc/TuChoiDangKyPost")]
        public async Task<IActionResult> TuChoi(int id, string lyDo)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var dk = await _dangKyHocRepo.GetByIdAsync(id);

            if (dk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });
                TempData["Error"] = "Không tìm thấy hồ sơ.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(lyDo))
            {
                if (isAjax) return Json(new { success = false, message = "Vui lòng nhập lý do từ chối hồ sơ." });
                TempData["Error"] = "Lý do từ chối không được để trống.";
                return RedirectToAction(nameof(Index));
            }

            dk.TrangThai = "Từ chối";
            dk.NgayXuLy = DateTime.Now;
            dk.NhanXetGiaoVu = lyDo.Trim();
            dk.GhiChu = $"[Từ chối bởi {HttpContext.Session.GetString("HoTen") ?? "Giáo vụ"} vào {DateTime.Now:dd/MM/yyyy HH:mm}]: {lyDo.Trim()}";

            await _dangKyHocRepo.UpdateAsync(dk);

            if (isAjax) return Json(new { success = true, message = $"Đã từ chối hồ sơ #{id:D3} thành công!", stats = await GetStatsJsonAsync() });
            TempData["Success"] = $"Đã từ chối hồ sơ #{id:D3} thành công!";
            return RedirectToAction(nameof(Index));
        }

        // --- HỦY ĐĂNG KÝ (Mục 7.2) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKy(int id, string lyDo)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var dk = await _dangKyHocRepo.GetByIdWithDetailsAsync(id);

            if (dk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });
                TempData["Error"] = "Không tìm thấy hồ sơ.";
                return RedirectToAction(nameof(Index));
            }

            // Ràng buộc nghiệp vụ: Không được tự ý hủy khi lớp đã có lịch học/đang diễn ra nếu không có sự đồng ý của Admin
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            bool coLichHoc = await _buoiHocRepo.HasLichHocLopChuaHuyAsync(dk.MaLopHoc);

            if (coLichHoc && dk.TrangThai == "Đang học" && vaiTro != "Admin")
            {
                string warnMsg = "Lớp học đã lên lịch và đang học. Chỉ Quản trị viên (Admin) mới có quyền hủy hồ sơ trong giai đoạn này.";
                if (isAjax) return Json(new { success = false, message = warnMsg });
                TempData["Error"] = warnMsg;
                return RedirectToAction(nameof(Index));
            }

            dk.TrangThai = "Đã hủy";
            dk.GhiChu = $"[Hủy bởi {HttpContext.Session.GetString("HoTen") ?? "Giáo vụ"} lúc {DateTime.Now:dd/MM/yyyy HH:mm}]: {(string.IsNullOrWhiteSpace(lyDo) ? "Người dùng yêu cầu" : lyDo.Trim())}";

            await _dangKyHocRepo.UpdateAsync(dk);

            if (isAjax) return Json(new { success = true, message = $"Đã hủy hồ sơ đăng ký #{id} thành công!" });
            TempData["Success"] = $"Đã hủy hồ sơ đăng ký #{id} thành công!";
            return RedirectToAction(nameof(Index));
        }

        // --- XÓA HỒ SƠ ĐĂNG KÝ (DÀNH CHO ADMIN & GIÁO VỤ KIỂM SOÁT) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DangKyHoc/Xoa")]
        [Route("DangKyHoc/DeletePost")]
        public async Task<IActionResult> Xoa(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var dk = await _dangKyHocRepo.GetByIdAsync(id);

            if (dk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy hồ sơ cần xóa." });
                return RedirectToAction(nameof(Index));
            }

            // Chỉ cho xóa hẳn bản ghi khi hồ sơ chưa thu tiền hoặc đã hủy/từ chối
            if (dk.TrangThai != "Chờ duyệt" && dk.TrangThai != "Từ chối" && dk.TrangThai != "Đã hủy")
            {
                string msg = $"Không thể xóa hồ sơ #{id:D3} vì đang ở trạng thái '{dk.TrangThai}'. Vui lòng chuyển trạng thái sang 'Đã hủy' trước.";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            await _dangKyHocRepo.DeleteAsync(id);

            if (isAjax) return Json(new { success = true, message = $"Đã xóa hồ sơ đăng ký #{id:D3} khỏi hệ thống.", stats = await GetStatsJsonAsync() });
            TempData["Success"] = $"Đã xóa hồ sơ đăng ký #{id:D3} khỏi hệ thống.";
            return RedirectToAction(nameof(Index));
        }

        // ========================================================
        // ACTION DELETE MULTIPLE: XÓA NHIỀU HỒ SƠ ĐĂNG KÝ HỌC
        // ========================================================
        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int>? ids)
        {
            if (ids == null || !ids.Any())
                return Json(new { success = false, message = "Vui lòng chọn ít nhất 1 hồ sơ đăng ký để xóa." });

            int deleted = 0;
            int skipped = 0;

            foreach (var id in ids)
            {
                var dk = await _dangKyHocRepo.GetByIdAsync(id);
                if (dk == null) continue;

                if (dk.TrangThai != "Chờ duyệt" && dk.TrangThai != "Từ chối" && dk.TrangThai != "Đã hủy")
                {
                    skipped++;
                    continue;
                }

                await _dangKyHocRepo.DeleteAsync(id);
                deleted++;
            }

            if (deleted == 0 && skipped > 0)
            {
                return Json(new { success = false, message = $"Không thể xóa {skipped} hồ sơ đã chọn do đang ở trạng thái đã duyệt/đang học. Vui lòng chuyển sang 'Đã hủy' trước." });
            }

            string msg = $"Đã xóa thành công {deleted} hồ sơ đăng ký học.";
            if (skipped > 0)
            {
                msg += $" Bỏ qua {skipped} hồ sơ đang ở trạng thái Đã duyệt/Đang học.";
            }
            return Json(new { success = true, message = msg, deletedCount = deleted, skippedCount = skipped, stats = await GetStatsJsonAsync() });
        }
    }
}
