// ========================================================
// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Phụ trách: Module 1 - Tầng Service Quản lý Tài khoản & Phân quyền
// Nghiệp vụ doanh nghiệp: Đăng nhập, Đăng ký, Đổi mật khẩu, Khóa/Mở tài khoản
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Helpers;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;

        public TaiKhoanService(ITaiKhoanRepository taiKhoanRepository)
        {
            _taiKhoanRepository = taiKhoanRepository;
        }

        public async Task<(bool Success, string? ErrorMessage, TaiKhoan? User)> KiemTraDangNhapAsync(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                return (false, "Tên đăng nhập và mật khẩu không được để trống.", null);
            }

            var user = await _taiKhoanRepository.GetByTenDangNhapAsync(tenDangNhap);
            if (user == null)
            {
                return (false, "Tài khoản không tồn tại trong hệ thống.", null);
            }

            // Kiểm tra trạng thái hoạt động của tài khoản
            if (!user.TrangThai)
            {
                return (false, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Quản trị viên để được hỗ trợ.", null);
            }

            // Kiểm tra mật khẩu (hỗ trợ cả hash SHA-256 lẫn seed data gốc)
            var isPasswordValid = SecurityHelper.VerifyPassword(matKhau, user.MatKhau);
            if (!isPasswordValid)
            {
                return (false, "Mật khẩu không chính xác. Vui lòng thử lại.", null);
            }

            return (true, null, user);
        }

        public async Task<(bool Success, string? ErrorMessage, TaiKhoan? User)> DangKyHocVienAsync(DangKyViewModel model)
        {
            // 1. Kiểm tra trùng tên đăng nhập
            if (await _taiKhoanRepository.ExistsByTenDangNhapAsync(model.TenDangNhap))
            {
                return (false, $"Tên đăng nhập '{model.TenDangNhap}' đã được sử dụng.", null);
            }

            // 2. Kiểm tra trùng Email
            if (await _taiKhoanRepository.ExistsByEmailAsync(model.Email))
            {
                return (false, $"Địa chỉ Email '{model.Email}' đã được đăng ký trong hệ thống.", null);
            }

            // 3. Khởi tạo tài khoản học viên mới
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap.Trim(),
                MatKhau = SecurityHelper.HashPassword(model.MatKhau),
                HoTen = model.HoTen.Trim(),
                Email = model.Email.Trim().ToLower(),
                VaiTro = "HocVien",
                TrangThai = true,
                NgayTao = DateTime.Now,
                // Tự động liên kết bản ghi Học viên (Module 3)
                HocVien = new HocVien
                {
                    HoTen = model.HoTen.Trim(),
                    Email = model.Email.Trim().ToLower(),
                    SoDienThoai = model.SoDienThoai?.Trim(),
                    TrangThai = true,
                    NgayTao = DateTime.Now
                }
            };

            await _taiKhoanRepository.AddAsync(taiKhoan);
            return (true, null, taiKhoan);
        }

        public async Task<(bool Success, string? ErrorMessage)> TaoTaiKhoanAsync(TaiKhoan taiKhoan)
        {
            if (await _taiKhoanRepository.ExistsByTenDangNhapAsync(taiKhoan.TenDangNhap))
            {
                return (false, $"Tên đăng nhập '{taiKhoan.TenDangNhap}' đã tồn tại.");
            }

            if (await _taiKhoanRepository.ExistsByEmailAsync(taiKhoan.Email))
            {
                return (false, $"Email '{taiKhoan.Email}' đã được sử dụng bởi tài khoản khác.");
            }

            // Mã hóa mật khẩu
            taiKhoan.MatKhau = SecurityHelper.HashPassword(taiKhoan.MatKhau);
            taiKhoan.NgayTao = DateTime.Now;

            // Nếu tạo học viên, tự khởi tạo hồ sơ học viên
            if (taiKhoan.VaiTro == "HocVien" && taiKhoan.HocVien == null)
            {
                taiKhoan.HocVien = new HocVien
                {
                    HoTen = taiKhoan.HoTen,
                    Email = taiKhoan.Email,
                    TrangThai = true,
                    NgayTao = DateTime.Now
                };
            }

            await _taiKhoanRepository.AddAsync(taiKhoan);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatTaiKhoanAsync(TaiKhoan taiKhoan)
        {
            var existing = await _taiKhoanRepository.GetByIdAsync(taiKhoan.MaTaiKhoan);
            if (existing == null)
            {
                return (false, "Không tìm thấy tài khoản cần cập nhật.");
            }

            if (await _taiKhoanRepository.ExistsByTenDangNhapAsync(taiKhoan.TenDangNhap, taiKhoan.MaTaiKhoan))
            {
                return (false, $"Tên đăng nhập '{taiKhoan.TenDangNhap}' đã được dùng bởi tài khoản khác.");
            }

            if (await _taiKhoanRepository.ExistsByEmailAsync(taiKhoan.Email, taiKhoan.MaTaiKhoan))
            {
                return (false, $"Email '{taiKhoan.Email}' đã được dùng bởi tài khoản khác.");
            }

            existing.TenDangNhap = taiKhoan.TenDangNhap.Trim();
            existing.HoTen = taiKhoan.HoTen.Trim();
            existing.Email = taiKhoan.Email.Trim().ToLower();
            existing.VaiTro = taiKhoan.VaiTro;
            existing.TrangThai = taiKhoan.TrangThai;

            // Nếu người dùng có nhập mật khẩu mới
            if (!string.IsNullOrWhiteSpace(taiKhoan.MatKhau) && taiKhoan.MatKhau != existing.MatKhau)
            {
                existing.MatKhau = SecurityHelper.HashPassword(taiKhoan.MatKhau);
            }

            // Đồng bộ sang HocVien nếu có
            if (existing.HocVien != null)
            {
                existing.HocVien.HoTen = existing.HoTen;
                existing.HocVien.Email = existing.Email;
                existing.HocVien.TrangThai = existing.TrangThai;
            }

            await _taiKhoanRepository.UpdateAsync(existing);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DoiMatKhauAsync(int maTaiKhoan, string matKhauCu, string matKhauMoi)
        {
            var user = await _taiKhoanRepository.GetByIdAsync(maTaiKhoan);
            if (user == null)
            {
                return (false, "Tài khoản không tồn tại.");
            }

            if (!SecurityHelper.VerifyPassword(matKhauCu, user.MatKhau))
            {
                return (false, "Mật khẩu hiện tại không chính xác.");
            }

            if (matKhauCu == matKhauMoi)
            {
                return (false, "Mật khẩu mới không được trùng với mật khẩu cũ.");
            }

            user.MatKhau = SecurityHelper.HashPassword(matKhauMoi);
            await _taiKhoanRepository.UpdateAsync(user);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> KhoaMoKhoaTaiKhoanAsync(int maTaiKhoan)
        {
            var user = await _taiKhoanRepository.GetByIdAsync(maTaiKhoan);
            if (user == null)
            {
                return (false, "Tài khoản không tồn tại.");
            }

            // Chặn Admin tự khóa chính mình
            if (user.VaiTro == "Admin" && user.TenDangNhap.ToLower() == "admin")
            {
                return (false, "Không thể khóa tài khoản Quản trị viên cấp cao nhất.");
            }

            user.TrangThai = !user.TrangThai;
            if (user.HocVien != null)
            {
                user.HocVien.TrangThai = user.TrangThai;
            }

            await _taiKhoanRepository.UpdateAsync(user);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> ResetMatKhauAsync(int maTaiKhoan, string matKhauMoi = "123456")
        {
            var user = await _taiKhoanRepository.GetByIdAsync(maTaiKhoan);
            if (user == null)
            {
                return (false, "Tài khoản không tồn tại.");
            }

            user.MatKhau = SecurityHelper.HashPassword(matKhauMoi);
            await _taiKhoanRepository.UpdateAsync(user);
            return (true, null);
        }

        public async Task<TaiKhoan?> GetTaiKhoanByIdAsync(int id)
        {
            return await _taiKhoanRepository.GetByIdAsync(id);
        }

        public async Task<TaiKhoanListViewModel> GetDanhSachTaiKhoanPagedAsync(
            string? keyword, string? vaiTro, bool? trangThai, int pageIndex, int pageSize)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 10;

            var (items, totalCount) = await _taiKhoanRepository.GetPagedAsync(
                keyword, vaiTro, trangThai, pageIndex, pageSize);

            return new TaiKhoanListViewModel
            {
                Items = items,
                Keyword = keyword,
                VaiTro = vaiTro,
                TrangThai = trangThai,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<HoSoCaNhanViewModel?> LayHoSoCaNhanAsync(int maTaiKhoan)
        {
            var user = await _taiKhoanRepository.GetByIdAsync(maTaiKhoan);
            if (user == null) return null;

            var model = new HoSoCaNhanViewModel
            {
                MaTaiKhoan = user.MaTaiKhoan,
                TenDangNhap = user.TenDangNhap,
                HoTen = user.HoTen,
                Email = user.Email,
                VaiTro = user.VaiTro,
                NgayTao = user.NgayTao
            };

            if (user.HocVien != null)
            {
                model.SoDienThoai = user.HocVien.SoDienThoai;
                model.DiaChi = user.HocVien.DiaChi;
                model.NgaySinh = user.HocVien.NgaySinh;
                model.GioiTinh = user.HocVien.GioiTinh;
                model.TrinhDoHocVan = user.HocVien.TrinhDoHocVan;
                model.NgheNghiep = user.HocVien.NgheNghiep;
            }

            return model;
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatHoSoCaNhanAsync(HoSoCaNhanViewModel model)
        {
            var user = await _taiKhoanRepository.GetByIdAsync(model.MaTaiKhoan);
            if (user == null)
            {
                return (false, "Không tìm thấy thông tin tài khoản.");
            }

            if (await _taiKhoanRepository.ExistsByEmailAsync(model.Email, model.MaTaiKhoan))
            {
                return (false, $"Email '{model.Email}' đã được sử dụng bởi tài khoản khác.");
            }

            user.HoTen = model.HoTen.Trim();
            user.Email = model.Email.Trim().ToLower();

            if (user.HocVien != null)
            {
                user.HocVien.HoTen = user.HoTen;
                user.HocVien.Email = user.Email;
                user.HocVien.SoDienThoai = model.SoDienThoai?.Trim();
                user.HocVien.DiaChi = model.DiaChi?.Trim();
                user.HocVien.NgaySinh = model.NgaySinh;
                user.HocVien.GioiTinh = model.GioiTinh;
                user.HocVien.TrinhDoHocVan = model.TrinhDoHocVan;
                user.HocVien.NgheNghiep = model.NgheNghiep;
            }

            await _taiKhoanRepository.UpdateAsync(user);
            return (true, null);
        }
    }
}
