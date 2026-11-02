// ========================================================
// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Phụ trách: Module 5 - Buổi học, Điểm danh, Kết quả học tập & Thống kê báo cáo
// Bài tập thực hành: Repository Pattern (Bài tập 12, 13, 14, 15 - UNETI)
// ========================================================

using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Repositories
{
    public interface IThongKeRepository
    {
        // 1. Thống kê tổng hợp (KPI Cards)
        Task<Dictionary<string, object>> GetTongHopKpiAsync();

        // 2. Thống kê theo nhóm (Bài tập 12 - GroupBy & Count)
        Task<IEnumerable<dynamic>> GetLopTheoCTDTAsync();
        Task<IEnumerable<dynamic>> GetDKTheoLopAsync();
        Task<IEnumerable<dynamic>> GetDKTheoTrangThaiAsync();

        // 3. Thống kê cực trị (Bài tập 14 - Min/Max)
        Task<dynamic?> GetLopNhieuDKNhatAsync();

        // 4. Thống kê doanh thu & tài chính (Bài tập 13 - Sum & GroupBy)
        Task<IEnumerable<ThongKeDoanhThuVM>> GetDoanhThuTheoThangAsync();
        Task<IEnumerable<dynamic>> GetDoanhThuTheoCTDTAsync();

        // 5. Thống kê xu hướng thời gian
        Task<IEnumerable<dynamic>> GetDKTheoThangAsync();
        Task<IEnumerable<dynamic>> GetBuoiHocTheoThangAsync();

        // 6. Thống kê chất lượng & kết quả học tập
        Task<IEnumerable<dynamic>> GetDiemTBTheoLopAsync();
        Task<double> GetTyLeHoanThanhKhoaHocAsync();

        // 7. Thống kê công suất lớp học (Tỷ lệ lấp đầy sỉ số)
        Task<List<TyLeLapDayVM>> GetTyLeLapDayAsync();

        // 8. Bảng xếp hạng giảng viên phụ trách
        Task<IEnumerable<dynamic>> GetTopGiangVienAsync();
    }
}
