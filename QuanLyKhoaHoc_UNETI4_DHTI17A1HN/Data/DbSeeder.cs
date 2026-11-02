// ========================================================
// Đề tài 36: Quản lý đăng ký khóa học và học viên trung tâm đào tạo
// Nhóm 4 - Lớp ĐHTI17A1HN - Đại học Kinh tế - Kỹ thuật Công nghiệp (UNETI)
// File nạp dữ liệu mẫu phong phú theo đúng Mục 16 của đề tài
// ========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data
{
    public static class DbSeeder
    {
        public static void SeedMockData(ApplicationDbContext context)
        {
            // ========================================================
            // 1. CHƯƠNG TRÌNH ĐÀO TẠO (Mục 16: Tối thiểu 05 CTĐT)
            // ========================================================
            if (context.ChuongTrinhDaoTaos.Count() < 5)
            {
                var existingNames = context.ChuongTrinhDaoTaos.Select(c => c.TenChuongTrinh).ToList();

                var ctdtList = new List<ChuongTrinhDaoTao>
                {
                    new ChuongTrinhDaoTao
                    {
                        TenChuongTrinh = "Lập trình Web & Phần mềm Doanh nghiệp",
                        MoTa = "Đào tạo kỹ sư phần mềm thực chiến với ASP.NET Core 10 MVC, React 19, SQL Server và kiến trúc Clean Architecture.",
                        EmailLienHe = "cntt@uneti.edu.vn",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-60)
                    },
                    new ChuongTrinhDaoTao
                    {
                        TenChuongTrinh = "Khoa học Dữ liệu & Trí tuệ Nhân tạo",
                        MoTa = "Chuyên sâu Python, Machine Learning, Deep Learning, Power BI và phân tích thị trường thực tế.",
                        EmailLienHe = "ai@uneti.edu.vn",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-55)
                    },
                    new ChuongTrinhDaoTao
                    {
                        TenChuongTrinh = "Lập trình Ứng dụng Di động Đa nền tảng",
                        MoTa = "Phát triển ứng dụng mobile chuyên nghiệp với Flutter, Dart, React Native và tích hợp Cloud API.",
                        EmailLienHe = "mobile@uneti.edu.vn",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-50)
                    },
                    new ChuongTrinhDaoTao
                    {
                        TenChuongTrinh = "An toàn Thông tin & Quản trị Mạng",
                        MoTa = "An ninh mạng, kiểm thử xâm nhập CEH, quản trị máy chủ Linux/Windows Server và bảo mật ứng dụng Web.",
                        EmailLienHe = "security@uneti.edu.vn",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-45)
                    },
                    new ChuongTrinhDaoTao
                    {
                        TenChuongTrinh = "Thiết kế Đồ họa & Trải nghiệm Người dùng (UI/UX)",
                        MoTa = "Nghiên cứu hành vi người dùng, thiết kế giao diện Figma, hệ thống Design System và tạo mẫu tương tác cao cấp.",
                        EmailLienHe = "design@uneti.edu.vn",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-40)
                    }
                };

                foreach (var c in ctdtList)
                {
                    if (!existingNames.Any(en => en.Equals(c.TenChuongTrinh, StringComparison.OrdinalIgnoreCase)))
                    {
                        context.ChuongTrinhDaoTaos.Add(c);
                    }
                }
                context.SaveChanges();
            }

            var allCTDT = context.ChuongTrinhDaoTaos.OrderBy(c => c.MaChuongTrinh).ToList();

            // ========================================================
            // 2. GIẢNG VIÊN & TÀI KHOẢN GIẢNG VIÊN (Tối thiểu 3 giảng viên)
            // ========================================================
            if (context.GiangViens.Count() < 3)
            {
                var tkGv1 = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == "giangvien");
                if (tkGv1 == null)
                {
                    tkGv1 = new TaiKhoan
                    {
                        TenDangNhap = "giangvien",
                        MatKhau = "123456",
                        HoTen = "ThS. Trần Văn Hùng",
                        Email = "hung.tran@uneti.edu.vn",
                        VaiTro = "GiangVien",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-60)
                    };
                    context.TaiKhoans.Add(tkGv1);
                    context.SaveChanges();
                }

                var tkGv2 = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == "lan.hoang");
                if (tkGv2 == null)
                {
                    tkGv2 = new TaiKhoan
                    {
                        TenDangNhap = "lan.hoang",
                        MatKhau = "123456",
                        HoTen = "TS. Hoàng Thị Lan",
                        Email = "lan.hoang@uneti.edu.vn",
                        VaiTro = "GiangVien",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-50)
                    };
                    context.TaiKhoans.Add(tkGv2);
                    context.SaveChanges();
                }

                var tkGv3 = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == "nam.nguyen");
                if (tkGv3 == null)
                {
                    tkGv3 = new TaiKhoan
                    {
                        TenDangNhap = "nam.nguyen",
                        MatKhau = "123456",
                        HoTen = "ThS. Nguyễn Thành Nam",
                        Email = "nam.nguyen@uneti.edu.vn",
                        VaiTro = "GiangVien",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-40)
                    };
                    context.TaiKhoans.Add(tkGv3);
                    context.SaveChanges();
                }

                if (!context.GiangViens.Any(g => g.Email == "hung.tran@uneti.edu.vn"))
                {
                    context.GiangViens.Add(new GiangVien
                    {
                        MaTaiKhoan = tkGv1.MaTaiKhoan,
                        HoTen = "ThS. Trần Văn Hùng",
                        Email = "hung.tran@uneti.edu.vn",
                        SoDienThoai = "0912345678",
                        HocVi = "Thạc sĩ",
                        ChuyenNganh = "Kỹ thuật phần mềm & ASP.NET Core",
                        GioiThieu = "Hơn 8 năm kinh nghiệm giảng dạy lập trình C#, .NET và hệ quản trị CSDL SQL Server tại Khoa CNTT UNETI.",
                        TrangThai = true
                    });
                }

                if (!context.GiangViens.Any(g => g.Email == "lan.hoang@uneti.edu.vn"))
                {
                    context.GiangViens.Add(new GiangVien
                    {
                        MaTaiKhoan = tkGv2.MaTaiKhoan,
                        HoTen = "TS. Hoàng Thị Lan",
                        Email = "lan.hoang@uneti.edu.vn",
                        SoDienThoai = "0987654321",
                        HocVi = "Tiến sĩ",
                        ChuyenNganh = "Trí tuệ nhân tạo & Khoa học dữ liệu",
                        GioiThieu = "Tiến sĩ Khoa học Máy tính. Giảng dạy chuyên sâu về Python, Machine Learning và Xử lý ngôn ngữ tự nhiên.",
                        TrangThai = true
                    });
                }

                if (!context.GiangViens.Any(g => g.Email == "nam.nguyen@uneti.edu.vn"))
                {
                    context.GiangViens.Add(new GiangVien
                    {
                        MaTaiKhoan = tkGv3.MaTaiKhoan,
                        HoTen = "ThS. Nguyễn Thành Nam",
                        Email = "nam.nguyen@uneti.edu.vn",
                        SoDienThoai = "0934567890",
                        HocVi = "Thạc sĩ",
                        ChuyenNganh = "Mạng máy tính & An toàn thông tin",
                        GioiThieu = "Thạc sĩ An toàn thông tin. Chuyên trách học phần Điện toán đám mây và Bảo mật hệ thống.",
                        TrangThai = true
                    });
                }
                context.SaveChanges();
            }

            var gvList = context.GiangViens.ToList();
            int gvId1 = gvList.Count > 0 ? gvList[0].MaGiangVien : 1;
            int gvId2 = gvList.Count > 1 ? gvList[1].MaGiangVien : gvId1;
            int gvId3 = gvList.Count > 2 ? gvList[2].MaGiangVien : gvId1;

            // ========================================================
            // 3. TÀI KHOẢN ADMIN VÀ GIÁO VỤ (Mục 16: 1-2 Admin, 2-3 Giáo vụ)
            // ========================================================
            var adminAccounts = new List<TaiKhoan>
            {
                new TaiKhoan { TenDangNhap = "admin", MatKhau = "admin123", HoTen = "Vũ Đình Trường (Admin)", Email = "admin@uneti.edu.vn", VaiTro = "Admin", TrangThai = true, NgayTao = DateTime.Today.AddDays(-60) },
                new TaiKhoan { TenDangNhap = "admin_backup", MatKhau = "admin123", HoTen = "Phụ tá Quản trị", Email = "admin2@uneti.edu.vn", VaiTro = "Admin", TrangThai = true, NgayTao = DateTime.Today.AddDays(-40) },
                new TaiKhoan { TenDangNhap = "giaovu", MatKhau = "giaovu123", HoTen = "Nguyễn Hoàng Long (Giáo vụ)", Email = "giaovu@uneti.edu.vn", VaiTro = "GiaoVu", TrangThai = true, NgayTao = DateTime.Today.AddDays(-60) },
                new TaiKhoan { TenDangNhap = "giaovu_mai", MatKhau = "giaovu123", HoTen = "Nguyễn Thị Mai (Giáo vụ)", Email = "mai.nt@uneti.edu.vn", VaiTro = "GiaoVu", TrangThai = true, NgayTao = DateTime.Today.AddDays(-45) },
                new TaiKhoan { TenDangNhap = "giaovu_hung", MatKhau = "giaovu123", HoTen = "Trần Văn Hùng (Giáo vụ)", Email = "hung.tv@uneti.edu.vn", VaiTro = "GiaoVu", TrangThai = true, NgayTao = DateTime.Today.AddDays(-30) }
            };

            foreach (var tk in adminAccounts)
            {
                var existing = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == tk.TenDangNhap);
                if (existing == null)
                {
                    context.TaiKhoans.Add(tk);
                }
                else
                {
                    existing.VaiTro = tk.VaiTro;
                    existing.TrangThai = true;
                }
            }
            context.SaveChanges();

            // ========================================================
            // 4. LỚP HỌC (Mục 16: Tối thiểu 15 lớp học, đủ các trạng thái)
            // ========================================================
            if (context.LopHocs.Count() < 16)
            {
                int ctdt1 = allCTDT.Count > 0 ? allCTDT[0].MaChuongTrinh : 1;
                int ctdt2 = allCTDT.Count > 1 ? allCTDT[1].MaChuongTrinh : ctdt1;
                int ctdt3 = allCTDT.Count > 2 ? allCTDT[2].MaChuongTrinh : ctdt1;
                int ctdt4 = allCTDT.Count > 3 ? allCTDT[3].MaChuongTrinh : ctdt1;
                int ctdt5 = allCTDT.Count > 4 ? allCTDT[4].MaChuongTrinh : ctdt1;

                var existingClassNames = context.LopHocs.Select(l => l.TenLop).ToList();

                var newLopHocs = new List<LopHoc>
                {
                    // Lớp 1: Đang mở đăng ký
                    new LopHoc { TenLop = "NETCORE10-K01", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 25, SoBuoiHoc = 16, HocPhi = 3500000, NgayBatDauDangKy = DateTime.Today.AddDays(-10), HanDangKy = DateTime.Today.AddDays(15), NgayKhaiGiang = DateTime.Today.AddDays(20), MoTa = "Lập trình Web hiện đại với ASP.NET Core 10 MVC, EF Core 10 và SQL Server.", YeuCauHocVien = "Biết C# căn bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 402 - Lĩnh Nam" },
                    // Lớp 2: Đang mở đăng ký
                    new LopHoc { TenLop = "PYTHON-K02", MaChuongTrinh = ctdt2, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 30, SoBuoiHoc = 14, HocPhi = 3200000, NgayBatDauDangKy = DateTime.Today.AddDays(-8), HanDangKy = DateTime.Today.AddDays(12), NgayKhaiGiang = DateTime.Today.AddDays(18), MoTa = "Phân tích dữ liệu với Python, Pandas, Numpy và trực quan hóa dữ liệu.", YeuCauHocVien = "Không yêu cầu", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 305 - Lĩnh Nam" },
                    // Lớp 3: Đang mở đăng ký
                    new LopHoc { TenLop = "REACT-K05", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 25, SoBuoiHoc = 16, HocPhi = 3800000, NgayBatDauDangKy = DateTime.Today.AddDays(-5), HanDangKy = DateTime.Today.AddDays(15), NgayKhaiGiang = DateTime.Today.AddDays(22), MoTa = "Xây dựng Single Page App với React 19, Hooks, Redux Toolkit và Tailwind CSS.", YeuCauHocVien = "Nắm vững HTML/CSS/JS", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 402 - Lĩnh Nam" },
                    // Lớp 4: Đang mở đăng ký
                    new LopHoc { TenLop = "FLUTTER-K03", MaChuongTrinh = ctdt3, MaGiangVien = gvId3, TenGiangVien = "ThS. Nguyễn Thành Nam", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 20, SoBuoiHoc = 16, HocPhi = 4200000, NgayBatDauDangKy = DateTime.Today.AddDays(-7), HanDangKy = DateTime.Today.AddDays(10), NgayKhaiGiang = DateTime.Today.AddDays(16), MoTa = "Lập trình ứng dụng di động đa nền tảng iOS & Android với Flutter và Dart.", YeuCauHocVien = "Có tư duy lập trình", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 201 - Minh Khai" },
                    // Lớp 5: Đang mở đăng ký
                    new LopHoc { TenLop = "CEH-K08", MaChuongTrinh = ctdt4, MaGiangVien = gvId3, TenGiangVien = "ThS. Nguyễn Thành Nam", TrinhDoDauVao = "Nâng cao", SiSoToiDa = 18, SoBuoiHoc = 18, HocPhi = 5500000, NgayBatDauDangKy = DateTime.Today.AddDays(-12), HanDangKy = DateTime.Today.AddDays(8), NgayKhaiGiang = DateTime.Today.AddDays(14), MoTa = "Đạo đức Hacker, kỹ thuật rà soát lỗ hổng và kiểm thử xâm nhập thực chiến.", YeuCauHocVien = "Hiểu mạng máy tính căn bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Phòng Lab 501" },
                    // Lớp 6: Đang mở đăng ký
                    new LopHoc { TenLop = "UIUX-K01", MaChuongTrinh = ctdt5, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 25, SoBuoiHoc = 12, HocPhi = 2800000, NgayBatDauDangKy = DateTime.Today.AddDays(-3), HanDangKy = DateTime.Today.AddDays(20), NgayKhaiGiang = DateTime.Today.AddDays(25), MoTa = "Thiết kế UI/UX bài bản từ nghiên cứu User Persona đến sản phẩm hoàn chỉnh trên Figma.", YeuCauHocVien = "Đam mê thiết kế đồ họa", TrangThai = "Đang mở đăng ký", PhongHoc = "Phòng Studio 302" },
                    // Lớp 7: Đang mở đăng ký
                    new LopHoc { TenLop = "AI-VISION-K01", MaChuongTrinh = ctdt2, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Nâng cao", SiSoToiDa = 15, SoBuoiHoc = 16, HocPhi = 5900000, NgayBatDauDangKy = DateTime.Today.AddDays(-5), HanDangKy = DateTime.Today.AddDays(14), NgayKhaiGiang = DateTime.Today.AddDays(20), MoTa = "Thị giác máy tính và Deep Learning với OpenCV, PyTorch và YOLO.", YeuCauHocVien = "Thành thạo Python", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab AI 405" },
                    // Lớp 8: Đang mở đăng ký
                    new LopHoc { TenLop = "DEVOPS-K02", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 20, SoBuoiHoc = 12, HocPhi = 4500000, NgayBatDauDangKy = DateTime.Today.AddDays(-6), HanDangKy = DateTime.Today.AddDays(16), NgayKhaiGiang = DateTime.Today.AddDays(24), MoTa = "Triển khai CI/CD pipeline với Docker, Kubernetes, GitHub Actions và Azure Cloud.", YeuCauHocVien = "Biết lập trình web và Linux cơ bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 402" },
                    // Lớp 9: Đang mở đăng ký
                    new LopHoc { TenLop = "SQL-OPT-K04", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 28, SoBuoiHoc = 10, HocPhi = 2200000, NgayBatDauDangKy = DateTime.Today.AddDays(-4), HanDangKy = DateTime.Today.AddDays(18), NgayKhaiGiang = DateTime.Today.AddDays(26), MoTa = "Tối ưu hóa truy vấn SQL Server, lập chỉ mục Index, Partitioning và stored procedure.", YeuCauHocVien = "Đã học CSDL căn bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 301" },
                    // Lớp 10: Đang mở đăng ký
                    new LopHoc { TenLop = "FIGMA-PRO-K02", MaChuongTrinh = ctdt5, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 20, SoBuoiHoc = 10, HocPhi = 2600000, NgayBatDauDangKy = DateTime.Today.AddDays(-2), HanDangKy = DateTime.Today.AddDays(25), NgayKhaiGiang = DateTime.Today.AddDays(30), MoTa = "Xây dựng Design System quy mô lớn, Variables, Auto-layout nâng cao và Prototyping.", YeuCauHocVien = "Đã sử dụng Figma cơ bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Phòng Studio 302" },
                    
                    // Lớp 11: Đã đóng đăng ký (Đủ chỉ tiêu hoặc hết hạn)
                    new LopHoc { TenLop = "NETCORE10-K00", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 5, SoBuoiHoc = 16, HocPhi = 3500000, NgayBatDauDangKy = DateTime.Today.AddDays(-40), HanDangKy = DateTime.Today.AddDays(-15), NgayKhaiGiang = DateTime.Today.AddDays(-10), MoTa = "Lớp đã hoàn thành tuyển sinh và đang trong quá trình học tập.", YeuCauHocVien = "C# căn bản", TrangThai = "Đã đóng đăng ký", PhongHoc = "Lab 402" },
                    // Lớp 12: Đã đóng đăng ký
                    new LopHoc { TenLop = "PYTHON-K01", MaChuongTrinh = ctdt2, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 6, SoBuoiHoc = 14, HocPhi = 3200000, NgayBatDauDangKy = DateTime.Today.AddDays(-35), HanDangKy = DateTime.Today.AddDays(-12), NgayKhaiGiang = DateTime.Today.AddDays(-8), MoTa = "Lớp học đã đủ sĩ số tối đa và hoàn tất xếp lịch học.", YeuCauHocVien = "Không yêu cầu", TrangThai = "Đã đóng đăng ký", PhongHoc = "Lab 305" },
                    // Lớp 13: Đã đóng đăng ký (Hết hạn tuyển sinh)
                    new LopHoc { TenLop = "REACT-K04", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 20, SoBuoiHoc = 16, HocPhi = 3800000, NgayBatDauDangKy = DateTime.Today.AddDays(-45), HanDangKy = DateTime.Today.AddDays(-2), NgayKhaiGiang = DateTime.Today.AddDays(5), MoTa = "Đã kết thúc giai đoạn nộp hồ sơ, chuẩn bị khai giảng.", YeuCauHocVien = "HTML/CSS/JS", TrangThai = "Đã đóng đăng ký", PhongHoc = "Lab 402" },
                    
                    // Lớp 14: Tạm dừng
                    new LopHoc { TenLop = "CLOUD-AWS-K01", MaChuongTrinh = ctdt4, MaGiangVien = gvId3, TenGiangVien = "ThS. Nguyễn Thành Nam", TrinhDoDauVao = "Nâng cao", SiSoToiDa = 20, SoBuoiHoc = 14, HocPhi = 4900000, NgayBatDauDangKy = DateTime.Today.AddDays(-10), HanDangKy = DateTime.Today.AddDays(10), NgayKhaiGiang = DateTime.Today.AddDays(20), MoTa = "Kiến trúc đám mây AWS Solutions Architect Associate. Tạm dừng để nâng cấp phòng máy.", YeuCauHocVien = "Có kiến thức hạ tầng mạng", TrangThai = "Tạm dừng", PhongHoc = "Lab Cloud 502" },
                    // Lớp 15: Tạm dừng
                    new LopHoc { TenLop = "DATA-BI-K03", MaChuongTrinh = ctdt2, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 25, SoBuoiHoc = 12, HocPhi = 2900000, NgayBatDauDangKy = DateTime.Today.AddDays(-15), HanDangKy = DateTime.Today.AddDays(5), NgayKhaiGiang = DateTime.Today.AddDays(15), MoTa = "Phân tích và trực quan dữ liệu với Power BI, DAX & Data Modeling.", YeuCauHocVien = "Excel cơ bản", TrangThai = "Tạm dừng", PhongHoc = "Lab 305" },

                    // Lớp 16: Chưa mở
                    new LopHoc { TenLop = "GOLANG-MICRO-K01", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Nâng cao", SiSoToiDa = 20, SoBuoiHoc = 14, HocPhi = 4800000, NgayBatDauDangKy = DateTime.Today.AddDays(10), HanDangKy = DateTime.Today.AddDays(30), NgayKhaiGiang = DateTime.Today.AddDays(40), MoTa = "Xây dựng hệ thống Microservices hiệu năng cao với Golang, gRPC và Kafka.", YeuCauHocVien = "Thành thạo ít nhất 1 ngôn ngữ OOP", TrangThai = "Chưa mở", PhongHoc = "Lab 402" }
                };

                foreach (var lop in newLopHocs)
                {
                    if (!existingClassNames.Any(en => en.Equals(lop.TenLop, StringComparison.OrdinalIgnoreCase)))
                    {
                        context.LopHocs.Add(lop);
                    }
                }
                context.SaveChanges();
            }

            var allLops = context.LopHocs.OrderBy(l => l.MaLopHoc).ToList();

            // ========================================================
            // 5. HỌC VIÊN & TÀI KHOẢN HỌC VIÊN (Mục 16: Tối thiểu 30 học viên)
            // ========================================================
            if (context.HocViens.Count() < 30)
            {
                var sampleStudents = new (string HoTen, string Email, string Sdt, DateTime NgaySinh, string GioiTinh, string DiaChi, string TrinhDo, string NgheNghiep, bool TrangThai)[]
                {
                    ("Nguyễn Văn An", "an.nguyen@gmail.com", "0987654321", new DateTime(2003, 5, 20), "Nam", "Số 15 Hai Bà Trưng, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                    ("Trần Minh Tuấn", "tuan.tm@uneti.edu.vn", "0912345001", new DateTime(2003, 8, 20), "Nam", "Số 454 Minh Khai, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                    ("Lê Thu Hà", "ha.lt@gmail.com", "0912345002", new DateTime(2005, 2, 14), "Nữ", "Số 12 Chùa Bộc, Hà Nội", "THPT", "Học sinh", true),
                    ("Phạm Quốc Bảo", "bao.pq@gmail.com", "0912345003", new DateTime(2002, 11, 9), "Nam", "Số 88 Giải Phóng, Hà Nội", "Đại học", "Lập trình viên tập sự", true),
                    ("Lê Hoàng Nam", "nam.lh@gmail.com", "0912345004", new DateTime(2001, 9, 30), "Nam", "Số 56 Cầu Giấy, Hà Nội", "Kỹ sư", "Nhân viên IT", true),
                    ("Phạm Viết Khang", "khang.pv@gmail.com", "0912345005", new DateTime(2004, 3, 18), "Nam", "Hoàng Mai, Hà Nội", "Cao đẳng", "Sinh viên", false), // Bị khóa
                    ("Đặng Thùy Trang", "trang.dt@gmail.com", "0912345006", new DateTime(2003, 12, 5), "Nữ", "Bạch Mai, Hà Nội", "Đại học", "Kế toán viên", true),
                    ("Vũ Hải Đăng", "dang.vh@gmail.com", "0912345007", new DateTime(2004, 7, 22), "Nam", "Trương Định, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                    ("Hoàng Thùy Linh", "linh.ht@gmail.com", "0912345008", new DateTime(2002, 5, 16), "Nữ", "Lĩnh Nam, Hà Nội", "Đại học", "Chuyên viên Marketing", true),
                    ("Nguyễn Quang Hải", "hai.nq@gmail.com", "0912345009", new DateTime(2003, 1, 19), "Nam", "Định Công, Hà Nội", "Cao đẳng", "Kỹ thuật viên mạng", true),
                    ("Đỗ Mai Phương", "phuong.dm@gmail.com", "0912345010", new DateTime(2004, 10, 8), "Nữ", "Thanh Xuân, Hà Nội", "Đại học", "Sinh viên", true),
                    ("Bùi Tiến Dũng", "dung.bt@gmail.com", "0912345011", new DateTime(2002, 4, 12), "Nam", "Hà Đông, Hà Nội", "Đại học", "Nhân viên kinh doanh", true),
                    ("Dương Mỹ Linh", "linh.dm@gmail.com", "0912345012", new DateTime(2003, 6, 25), "Nữ", "Long Biên, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                    ("Ngô Kiến Huy", "huy.nk@gmail.com", "0912345013", new DateTime(2001, 12, 1), "Nam", "Ba Đình, Hà Nội", "Kỹ sư", "Kỹ sư hệ thống", true),
                    ("Lý Nhã Kỳ", "ky.ln@gmail.com", "0912345014", new DateTime(2000, 7, 14), "Nữ", "Tây Hồ, Hà Nội", "Thạc sĩ", "Quản lý dự án", true),
                    ("Hồ Quang Hiếu", "hieu.hq@gmail.com", "0912345015", new DateTime(2003, 9, 5), "Nam", "Nam Từ Liêm, Hà Nội", "Đại học", "Sinh viên", true),
                    ("Võ Thị Sáu", "sau.vt@gmail.com", "0912345016", new DateTime(2005, 3, 28), "Nữ", "Bắc Từ Liêm, Hà Nội", "THPT", "Học sinh", true),
                    ("Phan Đình Tùng", "tung.pd@gmail.com", "0912345017", new DateTime(2002, 8, 17), "Nam", "Đống Đa, Hà Nội", "Đại học", "Thiết kế đồ họa", true),
                    ("Chu Đăng Khoa", "khoa.cd@gmail.com", "0912345018", new DateTime(2004, 11, 23), "Nam", "Gia Lâm, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                    ("Trịnh Kim Chi", "chi.tk@gmail.com", "0912345019", new DateTime(2003, 2, 9), "Nữ", "Cầu Giấy, Hà Nội", "Đại học", "Sinh viên", true),
                    ("Lương Gia Huy", "huy.lg@gmail.com", "0912345020", new DateTime(2002, 10, 11), "Nam", "Thanh Trì, Hà Nội", "Cao đẳng", "Lập trình viên", true),
                    ("Mai Phương Thúy", "thuy.mp@gmail.com", "0912345021", new DateTime(2001, 8, 6), "Nữ", "Hoàn Kiếm, Hà Nội", "Đại học", "Trợ lý đối ngoại", true),
                    ("Đinh Tiến Đạt", "dat.dt@gmail.com", "0912345022", new DateTime(2004, 5, 14), "Nam", "Sóc Sơn, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                    ("Cao Thái Sơn", "son.ct@gmail.com", "0912345023", new DateTime(2003, 4, 30), "Nam", "Đông Anh, Hà Nội", "Đại học", "Sinh viên", true),
                    ("Lâm Khánh Chi", "chi.lk@gmail.com", "0912345024", new DateTime(2002, 1, 15), "Nữ", "Mê Linh, Hà Nội", "Đại học", "Chuyên viên truyền thông", true),
                    ("Khắc Việt", "viet.k@gmail.com", "0912345025", new DateTime(2001, 6, 20), "Nam", "Hoàng Mai, Hà Nội", "Kỹ sư", "Kỹ sư phần mềm", true),
                    ("Bảo Thy", "thy.b@gmail.com", "0912345026", new DateTime(2005, 9, 2), "Nữ", "Thường Tín, Hà Nội", "THPT", "Học sinh", true),
                    ("Đăng Khôi", "khoi.d@gmail.com", "0912345027", new DateTime(2004, 7, 7), "Nam", "Hai Bà Trưng, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                    ("Minh Hằng", "hang.m@gmail.com", "0912345028", new DateTime(2002, 12, 18), "Nữ", "Thanh Xuân, Hà Nội", "Đại học", "Content Creator", true),
                    ("Tuấn Hưng", "hung.t@gmail.com", "0912345029", new DateTime(2003, 3, 3), "Nam", "Đống Đa, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                    ("Mỹ Tâm", "tam.m@gmail.com", "0912345030", new DateTime(2000, 1, 16), "Nữ", "Cầu Giấy, Hà Nội", "Thạc sĩ", "Giảng viên thỉnh giảng", true),
                    ("Đan Trường", "truong.d@gmail.com", "0912345031", new DateTime(2002, 11, 29), "Nam", "Tây Hồ, Hà Nội", "Đại học", "Tester kiểm thử phần mềm", true)
                };

                for (int i = 0; i < sampleStudents.Length; i++)
                {
                    var s = sampleStudents[i];
                    var existingHv = context.HocViens.FirstOrDefault(h => h.Email == s.Email);
                    if (existingHv == null)
                    {
                        string username = i == 0 ? "hocvien" : $"hocvien{i:D2}";
                        var tkHv = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == username);
                        if (tkHv == null)
                        {
                            tkHv = new TaiKhoan
                            {
                                TenDangNhap = username,
                                MatKhau = "123456",
                                HoTen = s.HoTen,
                                Email = s.Email,
                                VaiTro = "HocVien",
                                TrangThai = s.TrangThai,
                                NgayTao = DateTime.Today.AddDays(-30 + i)
                            };
                            context.TaiKhoans.Add(tkHv);
                            context.SaveChanges();
                        }

                        var hv = new HocVien
                        {
                            MaTaiKhoan = tkHv.MaTaiKhoan,
                            HoTen = s.HoTen,
                            Email = s.Email,
                            SoDienThoai = s.Sdt,
                            NgaySinh = s.NgaySinh,
                            GioiTinh = s.GioiTinh,
                            DiaChi = s.DiaChi,
                            TrinhDoHocVan = s.TrinhDo,
                            NgheNghiep = s.NgheNghiep,
                            TrangThai = s.TrangThai,
                            NgayTao = DateTime.Today.AddDays(-30 + i)
                        };
                        context.HocViens.Add(hv);
                        context.SaveChanges();
                    }
                }
            }

            var allStudents = context.HocViens.OrderBy(h => h.MaHocVien).ToList();

            // ========================================================
            // 6. ĐĂNG KÝ HỌC (Mục 16: 40-50 đăng ký với nhiều trạng thái)
            // ========================================================
            if (context.DangKyHocs.Count() < 42 && allStudents.Any() && allLops.Any())
            {
                var existingDks = context.DangKyHocs.Select(d => new { d.MaHocVien, d.MaLopHoc }).ToHashSet();
                var dksToAdd = new List<DangKyHoc>();

                var statuses = new[]
                {
                    "Chờ duyệt", "Chờ duyệt", "Chờ duyệt", "Chờ duyệt", "Chờ duyệt", "Chờ duyệt",
                    "Đã duyệt", "Đã duyệt", "Đã duyệt", "Đã duyệt", "Đã duyệt", "Đã duyệt",
                    "Đã đóng học phí", "Đã đóng học phí", "Đã đóng học phí", "Đã đóng học phí", "Đã đóng học phí", "Đã đóng học phí",
                    "Đang học", "Đang học", "Đang học", "Đang học", "Đang học", "Đang học", "Đang học", "Đang học",
                    "Hoàn thành", "Hoàn thành", "Hoàn thành", "Hoàn thành", "Hoàn thành", "Hoàn thành",
                    "Không hoàn thành", "Không hoàn thành", "Không hoàn thành",
                    "Từ chối", "Từ chối",
                    "Đã hủy", "Đã hủy"
                };

                int studentIdx = 0;
                int lopIdx = 0;

                for (int i = 0; i < statuses.Length; i++)
                {
                    var hv = allStudents[studentIdx % allStudents.Count];
                    var lop = allLops[lopIdx % allLops.Count];

                    // Tránh trùng học viên và lớp
                    int attempts = 0;
                    while (existingDks.Contains(new { MaHocVien = hv.MaHocVien, MaLopHoc = lop.MaLopHoc }) && attempts < 20)
                    {
                        studentIdx++;
                        hv = allStudents[studentIdx % allStudents.Count];
                        attempts++;
                    }

                    string st = statuses[i];
                    decimal feePaid = 0;
                    DateTime? paidDate = null;
                    DateTime? processDate = null;
                    string note = "";

                    if (st == "Đã duyệt")
                    {
                        processDate = DateTime.Now.AddDays(-10);
                        note = "Đã duyệt hồ sơ đủ điều kiện";
                    }
                    else if (st == "Đã đóng học phí")
                    {
                        processDate = DateTime.Now.AddDays(-12);
                        feePaid = lop.HocPhi;
                        paidDate = DateTime.Now.AddDays(-8);
                        note = "Đã nộp đủ 100% học phí qua chuyển khoản";
                    }
                    else if (st == "Đang học" || st == "Hoàn thành" || st == "Không hoàn thành")
                    {
                        processDate = DateTime.Now.AddDays(-25);
                        feePaid = lop.HocPhi;
                        paidDate = DateTime.Now.AddDays(-20);
                        note = "Học viên đang học / đã hoàn tất chương trình";
                    }
                    else if (st == "Từ chối")
                    {
                        processDate = DateTime.Now.AddDays(-5);
                        note = "Không đáp ứng trình độ đầu vào của lớp";
                    }
                    else if (st == "Đã hủy")
                    {
                        note = "Học viên chủ động hủy đăng ký";
                    }

                    dksToAdd.Add(new DangKyHoc
                    {
                        MaHocVien = hv.MaHocVien,
                        MaLopHoc = lop.MaLopHoc,
                        NgayDangKy = DateTime.Now.AddDays(-20 + (i % 15)),
                        TrangThai = st,
                        SoTienDaDong = feePaid,
                        NgayDongHocPhi = paidDate,
                        NgayXuLy = processDate,
                        NhanXetGiaoVu = note,
                        GhiChu = $"Học viên {hv.HoTen} đăng ký {lop.TenLop}"
                    });

                    existingDks.Add(new { MaHocVien = hv.MaHocVien, MaLopHoc = lop.MaLopHoc });
                    studentIdx++;
                    if (i % 3 == 0) lopIdx++;
                }

                context.DangKyHocs.AddRange(dksToAdd);
                context.SaveChanges();
            }

            // ========================================================
            // 7. BUỔI HỌC (Mục 16: 20-30 buổi học, Đã hoàn thành, Đã lên lịch, Đã hủy)
            // ========================================================
            if (context.BuoiHocs.Count() < 22 && allLops.Any())
            {
                var sessionsToAdd = new List<BuoiHoc>();
                var rooms = new[] { "Lab 402 - Lĩnh Nam", "Lab 305 - Lĩnh Nam", "Phòng 201 - Minh Khai", "Lab AI 405", "Phòng Studio 302", "Hội trường B" };

                // Tạo các buổi học cho các lớp khác nhau
                for (int i = 0; i < allLops.Count; i++)
                {
                    var lop = allLops[i];
                    string rm = rooms[i % rooms.Length];
                    string gvName = lop.TenGiangVien ?? "ThS. Trần Văn Hùng";

                    // Buổi 1: Đã hoàn thành (quá khứ)
                    sessionsToAdd.Add(new BuoiHoc
                    {
                        MaLopHoc = lop.MaLopHoc,
                        MaGiangVien = lop.MaGiangVien,
                        GiangVien = gvName,
                        PhongHoc = rm,
                        ThoiGianBatDau = DateTime.Today.AddDays(-7).AddHours(8 + (i % 4) * 3),
                        ThoiGianKetThuc = DateTime.Today.AddDays(-7).AddHours(11 + (i % 4) * 3),
                        NoiDung = $"Buổi 1: Tổng quan và cài đặt môi trường học phần {lop.TenLop}",
                        GhiChu = "Đã hoàn thành tốt đẹp",
                        TrangThai = "Đã hoàn thành"
                    });

                    // Buổi 2: Đã hoàn thành (quá khứ)
                    sessionsToAdd.Add(new BuoiHoc
                    {
                        MaLopHoc = lop.MaLopHoc,
                        MaGiangVien = lop.MaGiangVien,
                        GiangVien = gvName,
                        PhongHoc = rm,
                        ThoiGianBatDau = DateTime.Today.AddDays(-3).AddHours(8 + (i % 4) * 3),
                        ThoiGianKetThuc = DateTime.Today.AddDays(-3).AddHours(11 + (i % 4) * 3),
                        NoiDung = $"Buổi 2: Kiến trúc nâng cao và thực hành mã nguồn {lop.TenLop}",
                        GhiChu = "Điểm danh đủ",
                        TrangThai = "Đã hoàn thành"
                    });

                    // Buổi 3: Đã lên lịch (tương lai)
                    sessionsToAdd.Add(new BuoiHoc
                    {
                        MaLopHoc = lop.MaLopHoc,
                        MaGiangVien = lop.MaGiangVien,
                        GiangVien = gvName,
                        PhongHoc = rm,
                        ThoiGianBatDau = DateTime.Today.AddDays(2 + i).AddHours(8),
                        ThoiGianKetThuc = DateTime.Today.AddDays(2 + i).AddHours(11).AddMinutes(30),
                        NoiDung = $"Buổi 3: Triển khai dự án thực chiến và giải quyết bài toán lớn",
                        GhiChu = "Chuẩn bị laptop",
                        TrangThai = "Đã lên lịch"
                    });

                    if (sessionsToAdd.Count >= 25) break;
                }

                // Thêm 2 buổi hủy mẫu
                if (sessionsToAdd.Count < 26)
                {
                    sessionsToAdd.Add(new BuoiHoc
                    {
                        MaLopHoc = allLops[0].MaLopHoc,
                        MaGiangVien = allLops[0].MaGiangVien,
                        GiangVien = allLops[0].TenGiangVien ?? "ThS. Trần Văn Hùng",
                        PhongHoc = "Hội trường B",
                        ThoiGianBatDau = DateTime.Today.AddDays(5).AddHours(13),
                        ThoiGianKetThuc = DateTime.Today.AddDays(5).AddHours(16),
                        NoiDung = "Hội thảo chuyên đề (Bị hoãn do bảo trì phòng)",
                        GhiChu = "Đã hủy ca học",
                        TrangThai = "Đã hủy"
                    });
                }

                context.BuoiHocs.AddRange(sessionsToAdd);
                context.SaveChanges();
            }

            // ========================================================
            // 8. KẾT QUẢ HỌC TẬP (Mục 16: Cả Hoàn thành và Không hoàn thành)
            // ========================================================
            if (context.KetQuaHocTaps.Count() < 10)
            {
                var gradedDks = context.DangKyHocs
                    .Where(d => d.TrangThai == "Hoàn thành" || d.TrangThai == "Không hoàn thành" || d.TrangThai == "Đang học")
                    .ToList();

                var existingKqDkIds = context.KetQuaHocTaps.Select(k => k.MaDangKy).ToHashSet();

                var kqsToAdd = new List<KetQuaHocTap>();
                var mockScores = new (double cc, double ck, string nhanxet)[]
                {
                    (9.0, 9.5, "Xuất sắc, tư duy lập trình và giải quyết vấn đề rất tốt"),
                    (8.5, 8.0, "Giỏi, bài tập nộp đầy đủ và đúng hạn, tham gia tích cực"),
                    (8.0, 7.5, "Khá, hiểu bản chất công nghệ và ứng dụng tốt"),
                    (7.0, 7.0, "Khá, hoàn thành đồ án môn học đúng tiến độ"),
                    (8.0, 6.0, "Trung bình khá, cần rèn luyện thêm kỹ năng debug mã nguồn"),
                    (6.0, 5.0, "Đạt yêu cầu tối thiểu chuẩn đầu ra"),
                    (4.0, 4.0, "Không đạt, vắng nhiều buổi học và bài thi chưa đạt yêu cầu"),
                    (3.0, 3.5, "Không đạt, chưa nắm vững kiến thức trọng tâm"),
                    (2.0, 2.5, "Không đạt, vi phạm quy chế điểm danh môn học"),
                    (9.5, 9.0, "Xuất sắc, nằm trong top 5 học viên tiêu biểu của khóa"),
                    (8.0, 8.5, "Giỏi, làm việc nhóm hiệu quả và sáng tạo")
                };

                int scoreIdx = 0;
                foreach (var dk in gradedDks)
                {
                    if (!existingKqDkIds.Contains(dk.MaDangKy))
                    {
                        var (cc, ck, nx) = mockScores[scoreIdx % mockScores.Length];
                        double tongKet = Math.Round(cc * 0.4 + ck * 0.6, 2);
                        string kqStr = tongKet >= 5.0 ? "Hoàn thành" : "Không hoàn thành";

                        kqsToAdd.Add(new KetQuaHocTap
                        {
                            MaDangKy = dk.MaDangKy,
                            DiemChuyenCan = cc,
                            DiemCuoiKhoa = ck,
                            DiemTongKet = tongKet,
                            KetQua = kqStr,
                            NhanXet = nx,
                            NgayCapNhat = DateTime.Now.AddDays(-scoreIdx)
                        });

                        // Đồng bộ trạng thái đăng ký khớp với kết quả
                        dk.TrangThai = kqStr;

                        existingKqDkIds.Add(dk.MaDangKy);
                        scoreIdx++;
                        if (kqsToAdd.Count >= 12) break;
                    }
                }

                if (kqsToAdd.Any())
                {
                    context.KetQuaHocTaps.AddRange(kqsToAdd);
                    context.SaveChanges();
                }
            }
        }
    }
}
