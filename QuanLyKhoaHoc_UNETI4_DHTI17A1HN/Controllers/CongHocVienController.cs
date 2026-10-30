using Microsoft.AspNetCore.Mvc;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Controllers
{
    // ========================================================
    // Họ và tên: Nguyễn Đức Linh
    // Mã sinh viên: 23103100042
    // Phụ trách: Module 3 - Cổng Học viên & Đăng ký lớp học (Mục 7.1, 7.3, 7.4, 7.5)
    // ========================================================
    public class CongHocVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CongHocVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CongHocVien/DangKy
        public async Task<IActionResult> DangKy(string? tuKhoa, int? maKhoaHoc, string? lichHoc)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.3 (Đăng ký lớp học):
            // 1. Lấy danh sách các lớp học có TrangThai == "Đang mở đăng ký" kèm KhoaHoc, GiangVien, DangKyHocs
            // 2. Lọc theo tuKhoa (TenLop, MaLopHoc, TenKhoaHoc), maKhoaHoc, lichHoc
            // 3. Truyền danh sách lớp học sang View để học viên tra cứu và chọn lớp đăng ký
            await Task.CompletedTask;
            return View(Enumerable.Empty<LopHoc>());
        }

        // GET: CongHocVien/ChiTiet/5
        public async Task<IActionResult> ChiTiet(int? id)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.3: Xem chi tiết thông tin lớp học, giảng viên, lịch học, học phí và sĩ số trước khi đăng ký
            await Task.CompletedTask;
            return View(new LopHoc { TenLop = "Lớp học mẫu (TODO - Module 3)" });
        }

        // POST: CongHocVien/DangKyLopPost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyLopPost(int maLop, string? ghiChu)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.3 (Quy tắc nghiệp vụ Đăng ký lớp học):
            // 1. Kiểm tra học viên đã đăng nhập (Session "HocVienId") và trạng thái HocVien.TrangThai == true
            // 2. Kiểm tra hồ sơ học viên đã đầy đủ thông tin bắt buộc (HoTen, SoDienThoai, Email, NgaySinh)
            // 3. Kiểm tra lớp học có TrangThai == "Đang mở đăng ký"
            // 4. Kiểm tra thời gian hiện tại nằm trong khoảng: NgayBatDauDangKy <= Today <= HanDangKy
            // 5. Kiểm tra lớp còn chỗ trống (số đăng ký hợp lệ < SiSoToiDa)
            // 6. Kiểm tra học viên chưa có đăng ký đang hoạt động ở cùng lớp đó
            // 7. Tạo bản ghi DangKyHoc mới với TrangThai = "Chờ duyệt", NgayDangKy = DateTime.Now
            await Task.CompletedTask;
            TempData["Error"] = "Chức năng Đăng ký lớp học (Module 3) đang chờ Nguyễn Đức Linh tự triển khai.";
            return RedirectToAction(nameof(DangKy));
        }

        // POST: CongHocVien/HuyDangKyPost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKyPost(int maDangKy, string lyDoHuy)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.4 (Quy tắc nghiệp vụ Hủy đăng ký):
            // 1. Kiểm tra đăng ký thuộc về đúng học viên đang đăng nhập (MaHocVien == Session "HocVienId")
            // 2. Chỉ cho phép hủy khi TrangThai == "Chờ duyệt" hoặc TrangThai == "Đã duyệt" (chưa đóng học phí)
            // 3. Không cho phép hủy khi đã "Đã đóng học phí", "Đang học" hoặc "Hoàn thành"
            // 4. Cập nhật TrangThai = "Đã hủy", NgayHuy = DateTime.Now, LyDoHuy = lyDoHuy
            // 5. Nếu đã có bản ghi HocPhi ở trạng thái "Chưa đóng" thì cập nhật sang "Đã hủy"
            await Task.CompletedTask;
            TempData["Error"] = "Chức năng Hủy đăng ký (Module 3) đang chờ Nguyễn Đức Linh tự triển khai.";
            return RedirectToAction(nameof(TienTrinh));
        }

        // GET: CongHocVien/TienTrinh
        public async Task<IActionResult> TienTrinh(string? tab)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.5 (Theo dõi đăng ký & Cổng học viên):
            // 1. Kiểm tra học viên đang đăng nhập (Session "HocVienId")
            // 2. Truy vấn danh sách DangKyHoc của chính học viên đó kèm LopHoc, KhoaHoc, GiangVien, BuoiHocs, HocPhis, KetQuaHocTaps
            // 3. Hiển thị các tab: Đăng ký của tôi, Thời khóa biểu, Kết quả học tập (40% Quá trình + 60% Cuối kỳ), Hồ sơ cá nhân
            await Task.CompletedTask;
            ViewBag.ActiveTab = tab ?? "registrations";
            return View("TienTrinh", Enumerable.Empty<DangKyHoc>());
        }

        // GET: CongHocVien/Portal
        public Task<IActionResult> Portal(string? tab)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Điều hướng về trang TienTrinh theo tab tương ứng
            return TienTrinh(tab);
        }

        // GET: CongHocVien/HoSo
        public Task<IActionResult> HoSo()
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.1: Mở tab Hồ sơ cá nhân của học viên
            return TienTrinh("profile");
        }

        // POST: CongHocVien/CapNhatHoSoPost
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatHoSoPost(HocVien model)
        {
            // TODO [Module 3 - Nguyễn Đức Linh - 23103100042]:
            // Mục 7.1: Cho phép học viên tự cập nhật thông tin cá nhân (HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi, TrinhDoHocVan, NgheNghiep)
            await Task.CompletedTask;
            TempData["Error"] = "Chức năng Cập nhật hồ sơ cá nhân (Module 3) đang chờ Nguyễn Đức Linh tự triển khai.";
            return RedirectToAction(nameof(TienTrinh), new { tab = "profile" });
        }
    }
}
