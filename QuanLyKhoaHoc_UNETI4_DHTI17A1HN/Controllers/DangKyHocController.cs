// ========================================================
// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Phụ trách: Module 4 - Tiếp nhận, Xét duyệt đăng ký và Thu học phí
// ========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    public class DangKyHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DangKyHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- DANH SÁCH HỒ SƠ ĐĂNG KÝ (LINQ MULTI-FILTER, SEARCH, PAGINATION, AJAX) ---
        public async Task<IActionResult> Index(
            string? search,
            int? maChuongTrinh,
            int? maLop,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sortBy,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.ChuongTrinhDaoTao)
                .AsNoTracking()
                .AsQueryable();

            // 1. Tìm kiếm theo tên học viên, email, số điện thoại, tên lớp
            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(d =>
                    (d.HocVien != null && d.HocVien.HoTen.ToLower().Contains(kw)) ||
                    (d.HocVien != null && d.HocVien.Email.ToLower().Contains(kw)) ||
                    (d.HocVien != null && d.HocVien.SoDienThoai != null && d.HocVien.SoDienThoai.Contains(kw)) ||
                    (d.LopHoc != null && d.LopHoc.TenLop.ToLower().Contains(kw)) ||
                    ("dk" + d.MaDangKy.ToString()).Contains(kw));
            }

            // 2. Lọc theo chương trình đào tạo
            if (maChuongTrinh.HasValue)
            {
                query = query.Where(d => d.LopHoc != null && d.LopHoc.MaChuongTrinh == maChuongTrinh.Value);
            }

            // 3. Lọc theo lớp học
            if (maLop.HasValue)
            {
                query = query.Where(d => d.MaLopHoc == maLop.Value);
            }

            // 4. Lọc theo trạng thái duyệt
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(d => d.TrangThai == trangThai);
            }

            // 5. Lọc theo khoảng ngày đăng ký
            if (tuNgay.HasValue)
            {
                query = query.Where(d => d.NgayDangKy >= tuNgay.Value.Date);
            }
            if (denNgay.HasValue)
            {
                DateTime end = denNgay.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.NgayDangKy <= end);
            }

            // 6. Sắp xếp theo ngày đăng ký hoặc tên học viên
            query = sortBy switch
            {
                "ngay_asc" => query.OrderBy(d => d.NgayDangKy),
                "ten_asc" => query.OrderBy(d => d.HocVien != null ? d.HocVien.HoTen : ""),
                "ten_desc" => query.OrderByDescending(d => d.HocVien != null ? d.HocVien.HoTen : ""),
                _ => query.OrderByDescending(d => d.NgayDangKy)
            };

            // 3 KPI Cards (Toàn hệ thống)
            ViewBag.ChoDuyetCount = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ duyệt");
            ViewBag.DaDuyetCount = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học");
            ViewBag.TongDoanhThu = await _context.DangKyHocs.SumAsync(d => (double?)d.SoTienDaDong) ?? 0;

            // Danh sách chương trình & lớp học cho dropdown filter
            ViewBag.ChuongTrinhList = await _context.ChuongTrinhDaoTaos.OrderBy(c => c.TenChuongTrinh).ToListAsync();
            ViewBag.LopHocList = await _context.LopHocs
                .OrderByDescending(l => l.MaLopHoc)
                .Select(l => new { l.MaLopHoc, l.TenLop, l.MaChuongTrinh })
                .ToListAsync();

            // Phân trang LINQ
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.Search = search;
            ViewBag.MaChuongTrinh = maChuongTrinh;
            ViewBag.MaLop = maLop;
            ViewBag.TrangThai = trangThai;
            ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");
            ViewBag.SortBy = sortBy;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DangKyHocTablePartial", items);
            }

            return View(items);
        }

        // --- CHI TIẾT ĐƠN ĐĂNG KÝ (AJAX JSON) ---
        [HttpGet]
        public async Task<IActionResult> GetDetailsJson(int id)
        {
            var dk = await _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l!.ChuongTrinhDaoTao)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dk == null) return NotFound(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });

            int currentEnrolled = await _context.DangKyHocs.CountAsync(d =>
                d.MaLopHoc == dk.MaLopHoc &&
                (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));

            return Json(new
            {
                success = true,
                data = new
                {
                    maDangKy = dk.MaDangKy,
                    hoTen = dk.HocVien?.HoTen ?? "N/A",
                    email = dk.HocVien?.Email ?? "N/A",
                    soDienThoai = dk.HocVien?.SoDienThoai ?? "N/A",
                    gioiTinh = dk.HocVien?.GioiTinh ?? "N/A",
                    ngaySinh = dk.HocVien?.NgaySinh?.ToString("dd/MM/yyyy") ?? "N/A",
                    diaChi = dk.HocVien?.DiaChi ?? "N/A",
                    trinhDo = dk.HocVien?.TrinhDoHocVan ?? "N/A",
                    ngheNghiep = dk.HocVien?.NgheNghiep ?? "N/A",
                    tenLop = dk.LopHoc?.TenLop ?? "N/A",
                    chuongTrinh = dk.LopHoc?.ChuongTrinhDaoTao?.TenChuongTrinh ?? "N/A",
                    giangVien = dk.LopHoc?.TenGiangVien ?? "N/A",
                    hocPhi = dk.LopHoc?.HocPhi ?? 0,
                    siSoToiDa = dk.LopHoc?.SiSoToiDa ?? 0,
                    currentEnrolled = currentEnrolled,
                    ngayKhaiGiang = dk.LopHoc?.NgayKhaiGiang.ToString("dd/MM/yyyy") ?? "N/A",
                    ngayDangKy = dk.NgayDangKy.ToString("dd/MM/yyyy HH:mm"),
                    ngayXuLy = dk.NgayXuLy?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa xử lý",
                    ngayDongHocPhi = dk.NgayDongHocPhi?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa nộp",
                    soTienDaDong = dk.SoTienDaDong,
                    trangThai = dk.TrangThai,
                    ghiChu = dk.GhiChu,
                    nhanXetGiaoVu = dk.NhanXetGiaoVu
                }
            });
        }

        // --- PHÊ DUYỆT ĐĂNG KÝ (KIỂM SOÁT SĨ SỐ TỐI ĐA) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuyetDangKyPost(int id, string? nhanXet)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var dk = await _context.DangKyHocs
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy đơn đăng ký." });
                return RedirectToAction(nameof(Index));
            }

            var lop = dk.LopHoc;
            if (lop == null)
            {
                if (isAjax) return Json(new { success = false, message = "Lớp học liên kết không tồn tại." });
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra sĩ số hiện tại của lớp
            int enrolledCount = await _context.DangKyHocs.CountAsync(d =>
                d.MaLopHoc == lop.MaLopHoc &&
                d.MaDangKy != id &&
                (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));

            if (enrolledCount >= lop.SiSoToiDa)
            {
                string msg = $"Không thể duyệt đơn này! Lớp '{lop.TenLop}' đã đủ chỉ tiêu sĩ số ({enrolledCount}/{lop.SiSoToiDa} học viên).";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            dk.TrangThai = "Đã duyệt";
            dk.NgayXuLy = DateTime.Now;
            dk.NhanXetGiaoVu = string.IsNullOrWhiteSpace(nhanXet) ? "Đã duyệt hồ sơ đủ điều kiện nhập học." : nhanXet;

            if (enrolledCount + 1 >= lop.SiSoToiDa)
            {
                lop.TrangThai = "Đã đóng đăng ký";
            }

            await _context.SaveChangesAsync();

            string okMsg = $"Đã duyệt hồ sơ thành công cho học viên! (Sĩ số hiện tại: {enrolledCount + 1}/{lop.SiSoToiDa})";
            if (isAjax) return Json(new { success = true, message = okMsg });
            TempData["Success"] = okMsg;
            return RedirectToAction(nameof(Index));
        }

        // --- TỪ CHỐI ĐĂNG KÝ ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoiDangKyPost(int id, string? lyDo)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var dk = await _context.DangKyHocs.FindAsync(id);
            if (dk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });
                return RedirectToAction(nameof(Index));
            }

            dk.TrangThai = "Từ chối";
            dk.NgayXuLy = DateTime.Now;
            dk.NhanXetGiaoVu = string.IsNullOrWhiteSpace(lyDo) ? "Hồ sơ không đáp ứng yêu cầu tuyển sinh." : lyDo;

            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = "Đã từ chối đơn đăng ký." });
            TempData["Success"] = "Đã từ chối đơn đăng ký.";
            return RedirectToAction(nameof(Index));
        }

        // --- GHI NHẬN ĐÓNG HỌC PHÍ ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThuHocPhiPost(int id, decimal soTien, string? ghiChu)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var dk = await _context.DangKyHocs
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy hồ sơ." });
                return RedirectToAction(nameof(Index));
            }

            if (soTien <= 0)
            {
                if (isAjax) return Json(new { success = false, message = "Số tiền đóng học phí phải lớn hơn 0 VNĐ." });
                return RedirectToAction(nameof(Index));
            }

            decimal hocPhiGoc = dk.LopHoc?.HocPhi ?? 0m;
            if (hocPhiGoc > 0 && soTien > hocPhiGoc)
            {
                if (isAjax) return Json(new { success = false, message = $"Số tiền nộp ({soTien:N0} đ) vượt quá mức học phí của lớp ({hocPhiGoc:N0} đ)." });
                return RedirectToAction(nameof(Index));
            }

            dk.SoTienDaDong = soTien;
            dk.NgayDongHocPhi = DateTime.Now;

            if (soTien >= hocPhiGoc)
            {
                dk.TrangThai = "Đã đóng học phí";
            }

            if (!string.IsNullOrWhiteSpace(ghiChu))
            {
                dk.NhanXetGiaoVu = (string.IsNullOrWhiteSpace(dk.NhanXetGiaoVu) ? "" : dk.NhanXetGiaoVu + " | ") + "Thu phí: " + ghiChu;
            }

            await _context.SaveChangesAsync();

            string msg = $"Ghi nhận thu {soTien:N0} đ thành công! Trạng thái: {dk.TrangThai}";
            if (isAjax) return Json(new { success = true, message = msg });
            TempData["Success"] = msg;
            return RedirectToAction(nameof(Index));
        }

        // --- CHUYỂN TRẠNG THÁI SANG 'ĐANG HỌC' ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatDauHocPost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var dk = await _context.DangKyHocs.FindAsync(id);
            if (dk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy hồ sơ." });
                return RedirectToAction(nameof(Index));
            }

            dk.TrangThai = "Đang học";
            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = "Đã cập nhật trạng thái học viên sang 'Đang học'!" });
            return RedirectToAction(nameof(Index));
        }

        // --- XÓA ĐƠN ĐĂNG KÝ (RÀNG BUỘC HỌC PHÍ) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var dk = await _context.DangKyHocs.FindAsync(id);
            if (dk == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy đơn đăng ký." });
                return RedirectToAction(nameof(Index));
            }

            if (dk.SoTienDaDong > 0)
            {
                string msg = $"Không thể xóa đơn đăng ký #{dk.MaDangKy} vì học viên đã đóng {dk.SoTienDaDong:N0} đ. Vui lòng hoàn phí hoặc chuyển sang trạng thái 'Đã hủy'.";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            _context.DangKyHocs.Remove(dk);
            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = "Đã xóa đơn đăng ký thành công!" });
            TempData["Success"] = "Đã xóa đơn đăng ký thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
