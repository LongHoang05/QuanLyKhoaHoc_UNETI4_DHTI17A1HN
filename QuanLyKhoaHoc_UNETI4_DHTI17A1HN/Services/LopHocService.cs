using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services
{
    // ========================================================
    // Họ và tên: Nguyễn Đình Kiên
    // Mã sinh viên: 23103100020
    // Phụ trách: Module 2 - Quản lý lớp học & Tìm kiếm, Lọc, Sắp xếp, Phân trang LINQ
    // Service xử lý logic nghiệp vụ, validation & điều phối Repository
    // ========================================================
    public class LopHocService : ILopHocService
    {
        private readonly ILopHocRepository _lopHocRepo;
        private readonly IGiangVienRepository _giangVienRepo;

        public LopHocService(ILopHocRepository lopHocRepo, IGiangVienRepository giangVienRepo)
        {
            _lopHocRepo = lopHocRepo;
            _giangVienRepo = giangVienRepo;
        }

        public async Task<LopHocListViewModel> GetDanhSachLopAsync(
            string? keyword,
            int? maChuongTrinh,
            string? trinhDo,
            string? trangThai,
            int? maGiangVien,
            decimal? minHocPhi,
            decimal? maxHocPhi,
            string? filterHanDangKy,
            string? filterConCho,
            string? sortBy,
            int pageIndex,
            int pageSize = 8)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 8;

            // 1. Gọi Repository thực thi LINQ Search, Filter, Sort, Pagination
            var (items, totalCount) = await _lopHocRepo.GetPagedAsync(
                keyword,
                maChuongTrinh,
                trinhDo,
                trangThai,
                maGiangVien,
                minHocPhi,
                maxHocPhi,
                filterHanDangKy,
                filterConCho,
                sortBy,
                pageIndex,
                pageSize);

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages < 1) totalPages = 1;

            // 2. Lấy thống kê nhanh
            var (tongSoLop, soLopDangMo, soLopDaDong, soLopTamDung) = await _lopHocRepo.GetThongKeNhanhAsync();

            // 3. Lấy dữ liệu cho dropdown filter
            var chuongTrinhs = await _lopHocRepo.GetChuongTrinhListAsync();
            var giangViens = await _giangVienRepo.GetAllAsync();

            return new LopHocListViewModel
            {
                Items = items,
                Keyword = keyword,
                MaChuongTrinh = maChuongTrinh,
                TrinhDoDauVao = trinhDo,
                TrangThai = trangThai,
                MaGiangVien = maGiangVien,
                MinHocPhi = minHocPhi,
                MaxHocPhi = maxHocPhi,
                FilterHanDangKy = filterHanDangKy,
                FilterConCho = filterConCho,
                SortBy = sortBy,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                TongSoLop = tongSoLop,
                SoLopDangMo = soLopDangMo,
                SoLopDaDong = soLopDaDong,
                SoLopTamDung = soLopTamDung,
                ChuongTrinhOptions = new SelectList(chuongTrinhs, "MaChuongTrinh", "TenChuongTrinh", maChuongTrinh),
                GiangVienOptions = new SelectList(giangViens, "MaGiangVien", "HoTen", maGiangVien)
            };
        }

        public async Task<LopHoc?> GetChiTietLopAsync(int id)
        {
            return await _lopHocRepo.GetByIdWithDetailsAsync(id);
        }

        public async Task<(bool Success, string? ErrorMessage)> TaoLopHocAsync(LopHoc lopHoc)
        {
            // Nghiệp vụ Validation theo Chuẩn Mục 6.2 Đề tài BTL UNETI
            var validationResult = ValidateLopHoc(lopHoc);
            if (!validationResult.IsValid)
            {
                return (false, validationResult.ErrorMessage);
            }

            // Đồng bộ tên giảng viên nếu đã chọn MaGiangVien
            if (lopHoc.MaGiangVien.HasValue && lopHoc.MaGiangVien.Value > 0)
            {
                var gv = await _giangVienRepo.GetByIdAsync(lopHoc.MaGiangVien.Value);
                if (gv != null)
                {
                    lopHoc.TenGiangVien = gv.HoTen;
                }
            }

            lopHoc.NgayTao = DateTime.Now;
            await _lopHocRepo.AddAsync(lopHoc);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> CapNhatLopHocAsync(LopHoc lopHoc)
        {
            if (!await _lopHocRepo.ExistsAsync(lopHoc.MaLopHoc))
            {
                return (false, "Lớp học không tồn tại trong hệ thống.");
            }

            // Nghiệp vụ Validation chuẩn Mục 6.2
            var validationResult = ValidateLopHoc(lopHoc);
            if (!validationResult.IsValid)
            {
                return (false, validationResult.ErrorMessage);
            }

            // Không cho giảm sĩ số tối đa xuống thấp hơn số học viên đã được duyệt (Mục 6.2)
            int soHocVienDaDuyet = await _lopHocRepo.CountEnrolledAsync(lopHoc.MaLopHoc);
            if (lopHoc.SiSoToiDa < soHocVienDaDuyet)
            {
                return (false, $"Không thể giảm sĩ số tối đa xuống {lopHoc.SiSoToiDa} vì lớp đã có {soHocVienDaDuyet} học viên được duyệt trúng tuyển!");
            }

            // Đồng bộ tên giảng viên
            if (lopHoc.MaGiangVien.HasValue && lopHoc.MaGiangVien.Value > 0)
            {
                var gv = await _giangVienRepo.GetByIdAsync(lopHoc.MaGiangVien.Value);
                if (gv != null)
                {
                    lopHoc.TenGiangVien = gv.HoTen;
                }
            }
            else
            {
                lopHoc.TenGiangVien = null;
            }

            await _lopHocRepo.UpdateAsync(lopHoc);
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> XoaLopHocAsync(int id)
        {
            if (!await _lopHocRepo.ExistsAsync(id))
            {
                return (false, "Lớp học không tồn tại.");
            }

            // Kiểm soát không cho xóa lớp nếu đã có đăng ký để bảo toàn lịch sử (Mục 6.2)
            if (await _lopHocRepo.HasRegistrationsAsync(id))
            {
                return (false, "Không thể xóa lớp học này vì đã có học viên đăng ký! Vui lòng chuyển trạng thái sang 'Đã đóng đăng ký' hoặc 'Tạm dừng'.");
            }

            await _lopHocRepo.DeleteAsync(id);
            return (true, null);
        }

        public async Task<IEnumerable<ChuongTrinhDaoTao>> GetDanhSachChuongTrinhAsync()
        {
            return await _lopHocRepo.GetChuongTrinhListAsync();
        }

        public async Task<IEnumerable<GiangVien>> GetDanhSachGiangVienAsync()
        {
            return await _giangVienRepo.GetAllAsync();
        }

        public async Task<int> GetSoLuongHocVienDaDuyetAsync(int maLopHoc)
        {
            return await _lopHocRepo.CountEnrolledAsync(maLopHoc);
        }

        /// <summary>
        /// Hàm nội bộ kiểm tra hợp lệ các quy định nghiệp vụ theo Mục 6.2 BTL
        /// </summary>
        private static (bool IsValid, string? ErrorMessage) ValidateLopHoc(LopHoc lopHoc)
        {
            if (string.IsNullOrWhiteSpace(lopHoc.TenLop))
            {
                return (false, "Tên lớp học không được để trống.");
            }

            if (lopHoc.MaChuongTrinh <= 0)
            {
                return (false, "Vui lòng chọn chương trình đào tạo hợp lệ.");
            }

            if (lopHoc.SiSoToiDa <= 0)
            {
                return (false, "Sĩ số tối đa phải lớn hơn 0.");
            }

            if (lopHoc.SoBuoiHoc <= 0)
            {
                return (false, "Số buổi học phải lớn hơn 0.");
            }

            if (lopHoc.HocPhi < 0)
            {
                return (false, "Học phí phải lớn hơn hoặc bằng 0.");
            }

            // Quy định về ngày tháng
            if (lopHoc.HanDangKy < lopHoc.NgayBatDauDangKy)
            {
                return (false, "Hạn chót đăng ký phải sau hoặc bằng ngày bắt đầu mở đăng ký.");
            }

            if (lopHoc.NgayKhaiGiang < lopHoc.HanDangKy)
            {
                return (false, "Ngày khai giảng phải sau hoặc bằng hạn chót đăng ký.");
            }

            // Không cho chuyển sang Đang mở đăng ký nếu đã quá hạn đăng ký
            if (lopHoc.TrangThai == "Đang mở đăng ký" && lopHoc.HanDangKy < DateTime.Today)
            {
                return (false, "Không thể chuyển lớp sang trạng thái 'Đang mở đăng ký' khi hạn đăng ký đã hết hạn so với hôm nay.");
            }

            return (true, null);
        }
    }
}
