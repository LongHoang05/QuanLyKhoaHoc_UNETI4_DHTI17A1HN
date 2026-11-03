// ========================================================
// Đề tài 36: Quản lý đăng ký khóa học và học viên trung tâm đào tạo
// Nhóm 4 - Lớp ĐHTI17A1HN - Đại học Kinh tế - Kỹ thuật Công nghiệp (UNETI)
// File nạp dữ liệu mẫu phong phú theo đúng Mục 16 của đề tài
// Tự động băm mật khẩu SHA-256 cho toàn bộ tài khoản
// ========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Helpers;
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data
{
    public static class DbSeeder
    {
        public static void SeedMockData(ApplicationDbContext context)
        {
            // ========================================================
            // 0. TỰ ĐỘNG BĂM LẠI TOÀN BỘ MẬT KHẨU CHƯA HASH TRONG CSDL (SHA-256)
            // ========================================================
            // Chuỗi băm SHA-256 Base64 có độ dài 44 ký tự. Nếu MatKhau ngắn hơn 40 ký tự -> là text thuần -> băm ngay!
            var unhashedAccounts = context.TaiKhoans.Where(t => t.MatKhau.Length < 40).ToList();
            if (unhashedAccounts.Any())
            {
                foreach (var acc in unhashedAccounts)
                {
                    acc.MatKhau = SecurityHelper.HashPassword(acc.MatKhau);
                }
                context.SaveChanges();
            }

            // ========================================================
            // 1. CHƯƠNG TRÌNH ĐÀO TẠO (Mục 16: Tối thiểu 05 CTĐT -> Seed 08 CTĐT toàn diện)
            // ========================================================
            var existingCtdtNames = context.ChuongTrinhDaoTaos.Select(c => c.TenChuongTrinh).ToList();

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
                },
                new ChuongTrinhDaoTao
                {
                    TenChuongTrinh = "Điện toán Đám mây & Kỹ thuật DevOps (Cloud & CI/CD)",
                    MoTa = "Hạ tầng đám mây AWS, Azure, ảo hóa Docker, điều phối Kubernetes và tự động hóa CI/CD GitHub Actions.",
                    EmailLienHe = "devops@uneti.edu.vn",
                    TrangThai = true,
                    NgayTao = DateTime.Today.AddDays(-35)
                },
                new ChuongTrinhDaoTao
                {
                    TenChuongTrinh = "Phát triển Hệ thống Phân tán & Microservices với Golang",
                    MoTa = "Xây dựng backend tải cao, gRPC, RabbitMQ, Kafka và kiến trúc Event-Driven Microservices.",
                    EmailLienHe = "golang@uneti.edu.vn",
                    TrangThai = true,
                    NgayTao = DateTime.Today.AddDays(-30)
                },
                new ChuongTrinhDaoTao
                {
                    TenChuongTrinh = "Kiểm thử Phần mềm Tự động hóa & Đảm bảo Chất lượng (QA/QC)",
                    MoTa = "Kiểm thử tự động với Selenium WebDriver, Playwright, JMeter tải cao và quy trình chuẩn ISO/IEC 25010.",
                    EmailLienHe = "testing@uneti.edu.vn",
                    TrangThai = true,
                    NgayTao = DateTime.Today.AddDays(-25)
                }
            };

            foreach (var c in ctdtList)
            {
                if (!existingCtdtNames.Any(en => en.Equals(c.TenChuongTrinh, StringComparison.OrdinalIgnoreCase)))
                {
                    context.ChuongTrinhDaoTaos.Add(c);
                }
            }
            context.SaveChanges();

            var allCTDT = context.ChuongTrinhDaoTaos.OrderBy(c => c.MaChuongTrinh).ToList();

            // ========================================================
            // 2. GIẢNG VIÊN & TÀI KHOẢN GIẢNG VIÊN (Mục 16: Tối thiểu 3 giảng viên -> Seed 5 Giảng viên)
            // ========================================================
            var sampleLecturers = new[]
            {
                (Username: "giangvien", HoTen: "ThS. Trần Văn Hùng", Email: "hung.tran@uneti.edu.vn", Phone: "0912345678", Degree: "Thạc sĩ", Speciality: "Kỹ thuật phần mềm & ASP.NET Core", Bio: "Hơn 8 năm kinh nghiệm giảng dạy lập trình C#, .NET và hệ quản trị CSDL SQL Server tại Khoa CNTT UNETI."),
                (Username: "lan.hoang", HoTen: "TS. Hoàng Thị Lan", Email: "lan.hoang@uneti.edu.vn", Phone: "0987654321", Degree: "Tiến sĩ", Speciality: "Trí tuệ nhân tạo & Khoa học dữ liệu", Bio: "Tiến sĩ Khoa học Máy tính. Giảng dạy chuyên sâu về Python, Machine Learning và Xử lý ngôn ngữ tự nhiên."),
                (Username: "nam.nguyen", HoTen: "ThS. Nguyễn Thành Nam", Email: "nam.nguyen@uneti.edu.vn", Phone: "0934567890", Degree: "Thạc sĩ", Speciality: "Mạng máy tính & An toàn thông tin", Bio: "Thạc sĩ An toàn thông tin. Chuyên trách học phần Điện toán đám mây và Bảo mật hệ thống."),
                (Username: "thanh.le", HoTen: "PGS.TS. Lê Bá Thành", Email: "thanh.le@uneti.edu.vn", Phone: "0945678901", Degree: "PGS. Tiến sĩ", Speciality: "Điện toán đám mây & Kiến trúc Microservices", Bio: "Phó Giáo sư Khoa CNTT, tác giả nhiều công trình nghiên cứu quốc tế về hệ thống phân tán và bảo mật đám mây."),
                (Username: "trang.pham", HoTen: "ThS. Phạm Thu Trang", Email: "trang.pham@uneti.edu.vn", Phone: "0956789012", Degree: "Thạc sĩ", Speciality: "Kiểm thử phần mềm & Đảm bảo chất lượng QA/QC", Bio: "Chuyên gia kiểm thử phần mềm tự động hóa với 7 năm kinh nghiệm tại các tập đoàn công nghệ lớn.")
            };

            foreach (var gv in sampleLecturers)
            {
                var tkGv = context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == gv.Username);
                if (tkGv == null)
                {
                    tkGv = new TaiKhoan
                    {
                        TenDangNhap = gv.Username,
                        MatKhau = SecurityHelper.HashPassword("123456"),
                        HoTen = gv.HoTen,
                        Email = gv.Email,
                        VaiTro = "GiangVien",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-50)
                    };
                    context.TaiKhoans.Add(tkGv);
                    context.SaveChanges();
                }

                if (!context.GiangViens.Any(g => g.Email == gv.Email))
                {
                    context.GiangViens.Add(new GiangVien
                    {
                        MaTaiKhoan = tkGv.MaTaiKhoan,
                        HoTen = gv.HoTen,
                        Email = gv.Email,
                        SoDienThoai = gv.Phone,
                        HocVi = gv.Degree,
                        ChuyenNganh = gv.Speciality,
                        GioiThieu = gv.Bio,
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-50)
                    });
                }
            }
            context.SaveChanges();

            var gvList = context.GiangViens.ToList();
            int gvId1 = gvList.Count > 0 ? gvList[0].MaGiangVien : 1;
            int gvId2 = gvList.Count > 1 ? gvList[1].MaGiangVien : gvId1;
            int gvId3 = gvList.Count > 2 ? gvList[2].MaGiangVien : gvId1;
            int gvId4 = gvList.Count > 3 ? gvList[3].MaGiangVien : gvId1;
            int gvId5 = gvList.Count > 4 ? gvList[4].MaGiangVien : gvId2;

            // ========================================================
            // 3. TÀI KHOẢN ADMIN VÀ GIÁO VỤ (Mục 16: Băm mật khẩu SHA-256)
            // ========================================================
            var adminAccounts = new List<TaiKhoan>
            {
                new TaiKhoan { TenDangNhap = "admin", MatKhau = SecurityHelper.HashPassword("admin123"), HoTen = "Vũ Đình Trường (Admin)", Email = "admin@uneti.edu.vn", VaiTro = "Admin", TrangThai = true, NgayTao = DateTime.Today.AddDays(-60) },
                new TaiKhoan { TenDangNhap = "admin_backup", MatKhau = SecurityHelper.HashPassword("admin123"), HoTen = "Phụ tá Quản trị Hệ thống", Email = "admin2@uneti.edu.vn", VaiTro = "Admin", TrangThai = true, NgayTao = DateTime.Today.AddDays(-40) },
                new TaiKhoan { TenDangNhap = "giaovu", MatKhau = SecurityHelper.HashPassword("giaovu123"), HoTen = "Nguyễn Hoàng Long (Giáo vụ)", Email = "giaovu@uneti.edu.vn", VaiTro = "GiaoVu", TrangThai = true, NgayTao = DateTime.Today.AddDays(-60) },
                new TaiKhoan { TenDangNhap = "giaovu_mai", MatKhau = SecurityHelper.HashPassword("giaovu123"), HoTen = "Nguyễn Thị Mai (Giáo vụ Tuyển sinh)", Email = "mai.nt@uneti.edu.vn", VaiTro = "GiaoVu", TrangThai = true, NgayTao = DateTime.Today.AddDays(-45) },
                new TaiKhoan { TenDangNhap = "giaovu_hung", MatKhau = SecurityHelper.HashPassword("giaovu123"), HoTen = "Trần Văn Hùng (Giáo vụ Quản lý Lớp)", Email = "hung.tv@uneti.edu.vn", VaiTro = "GiaoVu", TrangThai = true, NgayTao = DateTime.Today.AddDays(-30) }
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
                    if (existing.MatKhau.Length < 40)
                    {
                        existing.MatKhau = tk.MatKhau;
                    }
                }
            }
            context.SaveChanges();

            // ========================================================
            // 4. LỚP HỌC (Mục 16: Tối thiểu 15 lớp học -> Seed 27 Lớp học đa dạng mọi trạng thái)
            // ========================================================
            int ctdt1 = allCTDT.Count > 0 ? allCTDT[0].MaChuongTrinh : 1;
            int ctdt2 = allCTDT.Count > 1 ? allCTDT[1].MaChuongTrinh : ctdt1;
            int ctdt3 = allCTDT.Count > 2 ? allCTDT[2].MaChuongTrinh : ctdt1;
            int ctdt4 = allCTDT.Count > 3 ? allCTDT[3].MaChuongTrinh : ctdt1;
            int ctdt5 = allCTDT.Count > 4 ? allCTDT[4].MaChuongTrinh : ctdt1;
            int ctdt6 = allCTDT.Count > 5 ? allCTDT[5].MaChuongTrinh : ctdt1;
            int ctdt7 = allCTDT.Count > 6 ? allCTDT[6].MaChuongTrinh : ctdt1;
            int ctdt8 = allCTDT.Count > 7 ? allCTDT[7].MaChuongTrinh : ctdt1;

            var existingClassNames = context.LopHocs.Select(l => l.TenLop).ToList();

            var newLopHocs = new List<LopHoc>
            {
                // Nhóm 1: Đang mở đăng ký (16 lớp)
                new LopHoc { TenLop = "NETCORE10-K01", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 25, SoBuoiHoc = 16, HocPhi = 3500000, NgayBatDauDangKy = DateTime.Today.AddDays(-10), HanDangKy = DateTime.Today.AddDays(15), NgayKhaiGiang = DateTime.Today.AddDays(20), MoTa = "Lập trình Web hiện đại với ASP.NET Core 10 MVC, EF Core 10 và SQL Server.", YeuCauHocVien = "Biết C# căn bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 402 - Lĩnh Nam" },
                new LopHoc { TenLop = "PYTHON-K02", MaChuongTrinh = ctdt2, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 30, SoBuoiHoc = 14, HocPhi = 3200000, NgayBatDauDangKy = DateTime.Today.AddDays(-8), HanDangKy = DateTime.Today.AddDays(12), NgayKhaiGiang = DateTime.Today.AddDays(18), MoTa = "Phân tích dữ liệu với Python, Pandas, Numpy và trực quan hóa dữ liệu.", YeuCauHocVien = "Không yêu cầu", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 305 - Lĩnh Nam" },
                new LopHoc { TenLop = "REACT-K05", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 25, SoBuoiHoc = 16, HocPhi = 3800000, NgayBatDauDangKy = DateTime.Today.AddDays(-5), HanDangKy = DateTime.Today.AddDays(15), NgayKhaiGiang = DateTime.Today.AddDays(22), MoTa = "Xây dựng Single Page App với React 19, Hooks, Redux Toolkit và Tailwind CSS.", YeuCauHocVien = "Nắm vững HTML/CSS/JS", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 402 - Lĩnh Nam" },
                new LopHoc { TenLop = "FLUTTER-K03", MaChuongTrinh = ctdt3, MaGiangVien = gvId3, TenGiangVien = "ThS. Nguyễn Thành Nam", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 20, SoBuoiHoc = 16, HocPhi = 4200000, NgayBatDauDangKy = DateTime.Today.AddDays(-7), HanDangKy = DateTime.Today.AddDays(10), NgayKhaiGiang = DateTime.Today.AddDays(16), MoTa = "Lập trình ứng dụng di động đa nền tảng iOS & Android với Flutter và Dart.", YeuCauHocVien = "Có tư duy lập trình", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 201 - Minh Khai" },
                new LopHoc { TenLop = "CEH-K08", MaChuongTrinh = ctdt4, MaGiangVien = gvId3, TenGiangVien = "ThS. Nguyễn Thành Nam", TrinhDoDauVao = "Nâng cao", SiSoToiDa = 18, SoBuoiHoc = 18, HocPhi = 5500000, NgayBatDauDangKy = DateTime.Today.AddDays(-12), HanDangKy = DateTime.Today.AddDays(8), NgayKhaiGiang = DateTime.Today.AddDays(14), MoTa = "Đạo đức Hacker, kỹ thuật rà soát lỗ hổng và kiểm thử xâm nhập thực chiến.", YeuCauHocVien = "Hiểu mạng máy tính căn bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Phòng Lab 501" },
                new LopHoc { TenLop = "UIUX-K01", MaChuongTrinh = ctdt5, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 25, SoBuoiHoc = 12, HocPhi = 2800000, NgayBatDauDangKy = DateTime.Today.AddDays(-3), HanDangKy = DateTime.Today.AddDays(20), NgayKhaiGiang = DateTime.Today.AddDays(25), MoTa = "Thiết kế UI/UX bài bản từ nghiên cứu User Persona đến sản phẩm hoàn chỉnh trên Figma.", YeuCauHocVien = "Đam mê thiết kế đồ họa", TrangThai = "Đang mở đăng ký", PhongHoc = "Phòng Studio 302" },
                new LopHoc { TenLop = "AI-VISION-K01", MaChuongTrinh = ctdt2, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Nâng cao", SiSoToiDa = 15, SoBuoiHoc = 16, HocPhi = 5900000, NgayBatDauDangKy = DateTime.Today.AddDays(-5), HanDangKy = DateTime.Today.AddDays(14), NgayKhaiGiang = DateTime.Today.AddDays(20), MoTa = "Thị giác máy tính và Deep Learning với OpenCV, PyTorch và YOLO.", YeuCauHocVien = "Thành thạo Python", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab AI 405" },
                new LopHoc { TenLop = "DEVOPS-K02", MaChuongTrinh = ctdt6, MaGiangVien = gvId4, TenGiangVien = "PGS.TS. Lê Bá Thành", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 20, SoBuoiHoc = 12, HocPhi = 4500000, NgayBatDauDangKy = DateTime.Today.AddDays(-6), HanDangKy = DateTime.Today.AddDays(16), NgayKhaiGiang = DateTime.Today.AddDays(24), MoTa = "Triển khai CI/CD pipeline với Docker, Kubernetes, GitHub Actions và Azure Cloud.", YeuCauHocVien = "Biết lập trình web và Linux cơ bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 402" },
                new LopHoc { TenLop = "SQL-OPT-K04", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 28, SoBuoiHoc = 10, HocPhi = 2200000, NgayBatDauDangKy = DateTime.Today.AddDays(-4), HanDangKy = DateTime.Today.AddDays(18), NgayKhaiGiang = DateTime.Today.AddDays(26), MoTa = "Tối ưu hóa truy vấn SQL Server, lập chỉ mục Index, Partitioning và stored procedure.", YeuCauHocVien = "Đã học CSDL căn bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 301" },
                new LopHoc { TenLop = "FIGMA-PRO-K02", MaChuongTrinh = ctdt5, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 20, SoBuoiHoc = 10, HocPhi = 2600000, NgayBatDauDangKy = DateTime.Today.AddDays(-2), HanDangKy = DateTime.Today.AddDays(25), NgayKhaiGiang = DateTime.Today.AddDays(30), MoTa = "Xây dựng Design System quy mô lớn, Variables, Auto-layout nâng cao và Prototyping.", YeuCauHocVien = "Đã sử dụng Figma cơ bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Phòng Studio 302" },
                new LopHoc { TenLop = "CLOUD-AWS-K02", MaChuongTrinh = ctdt6, MaGiangVien = gvId4, TenGiangVien = "PGS.TS. Lê Bá Thành", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 22, SoBuoiHoc = 14, HocPhi = 4900000, NgayBatDauDangKy = DateTime.Today.AddDays(-3), HanDangKy = DateTime.Today.AddDays(17), NgayKhaiGiang = DateTime.Today.AddDays(23), MoTa = "Khóa đào tạo luyện thi AWS Solutions Architect Associate với bài thực hành VPC, EC2, S3, RDS, Lambda.", YeuCauHocVien = "Hiểu mạng căn bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab Cloud 502" },
                new LopHoc { TenLop = "GOLANG-API-K01", MaChuongTrinh = ctdt7, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Nâng cao", SiSoToiDa = 18, SoBuoiHoc = 14, HocPhi = 4800000, NgayBatDauDangKy = DateTime.Today.AddDays(-1), HanDangKy = DateTime.Today.AddDays(20), NgayKhaiGiang = DateTime.Today.AddDays(28), MoTa = "Phát triển RESTful & gRPC API tải cao với Golang, Gin Framework và Redis Caching.", YeuCauHocVien = "Biết lập trình Backend", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 402 - Lĩnh Nam" },
                new LopHoc { TenLop = "QA-AUTO-K01", MaChuongTrinh = ctdt8, MaGiangVien = gvId5, TenGiangVien = "ThS. Phạm Thu Trang", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 24, SoBuoiHoc = 12, HocPhi = 3400000, NgayBatDauDangKy = DateTime.Today.AddDays(-4), HanDangKy = DateTime.Today.AddDays(16), NgayKhaiGiang = DateTime.Today.AddDays(24), MoTa = "Kiểm thử tự động Web với Playwright & TypeScript, tích hợp báo cáo Allure và CI/CD.", YeuCauHocVien = "Biết JavaScript/HTML căn bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 305 - Lĩnh Nam" },
                new LopHoc { TenLop = "JAVA-SPRING-K03", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 25, SoBuoiHoc = 16, HocPhi = 3900000, NgayBatDauDangKy = DateTime.Today.AddDays(-7), HanDangKy = DateTime.Today.AddDays(14), NgayKhaiGiang = DateTime.Today.AddDays(21), MoTa = "Lập trình Backend chuyên sâu với Java Spring Boot 3, Spring Security và Hibernate.", YeuCauHocVien = "Java căn bản", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 402 - Lĩnh Nam" },
                new LopHoc { TenLop = "K8S-MASTER-K01", MaChuongTrinh = ctdt6, MaGiangVien = gvId4, TenGiangVien = "PGS.TS. Lê Bá Thành", TrinhDoDauVao = "Chuyên sâu", SiSoToiDa = 16, SoBuoiHoc = 14, HocPhi = 5800000, NgayBatDauDangKy = DateTime.Today.AddDays(-2), HanDangKy = DateTime.Today.AddDays(22), NgayKhaiGiang = DateTime.Today.AddDays(30), MoTa = "Quản trị và vận hành cụm Kubernetes doanh nghiệp, Helm Chart, Ingress NGINX và Istio Service Mesh.", YeuCauHocVien = "Thành thạo Docker & Linux", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab Cloud 502" },
                new LopHoc { TenLop = "DATA-ENG-K01", MaChuongTrinh = ctdt2, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Nâng cao", SiSoToiDa = 20, SoBuoiHoc = 15, HocPhi = 5200000, NgayBatDauDangKy = DateTime.Today.AddDays(-5), HanDangKy = DateTime.Today.AddDays(15), NgayKhaiGiang = DateTime.Today.AddDays(22), MoTa = "Xây dựng Data Pipeline quy mô lớn với Apache Spark, Kafka và kho dữ liệu Snowflake.", YeuCauHocVien = "Biết Python & SQL", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab AI 405" },

                // Nhóm 2: Đã đóng đăng ký (5 lớp)
                new LopHoc { TenLop = "NETCORE10-K00", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 5, SoBuoiHoc = 16, HocPhi = 3500000, NgayBatDauDangKy = DateTime.Today.AddDays(-40), HanDangKy = DateTime.Today.AddDays(-15), NgayKhaiGiang = DateTime.Today.AddDays(-10), MoTa = "Lớp đã hoàn thành tuyển sinh và đang trong quá trình học tập thực tế.", YeuCauHocVien = "C# căn bản", TrangThai = "Đã đóng đăng ký", PhongHoc = "Lab 402" },
                new LopHoc { TenLop = "PYTHON-K01", MaChuongTrinh = ctdt2, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 6, SoBuoiHoc = 14, HocPhi = 3200000, NgayBatDauDangKy = DateTime.Today.AddDays(-35), HanDangKy = DateTime.Today.AddDays(-12), NgayKhaiGiang = DateTime.Today.AddDays(-8), MoTa = "Lớp học đã đủ sĩ số tối đa và hoàn tất xếp lịch học.", YeuCauHocVien = "Không yêu cầu", TrangThai = "Đã đóng đăng ký", PhongHoc = "Lab 305" },
                new LopHoc { TenLop = "REACT-K04", MaChuongTrinh = ctdt1, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Trung cấp", SiSoToiDa = 20, SoBuoiHoc = 16, HocPhi = 3800000, NgayBatDauDangKy = DateTime.Today.AddDays(-45), HanDangKy = DateTime.Today.AddDays(-2), NgayKhaiGiang = DateTime.Today.AddDays(5), MoTa = "Đã kết thúc giai đoạn nộp hồ sơ, chuẩn bị khai giảng.", YeuCauHocVien = "HTML/CSS/JS", TrangThai = "Đã đóng đăng ký", PhongHoc = "Lab 402" },
                new LopHoc { TenLop = "FLUTTER-K02", MaChuongTrinh = ctdt3, MaGiangVien = gvId3, TenGiangVien = "ThS. Nguyễn Thành Nam", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 22, SoBuoiHoc = 16, HocPhi = 4200000, NgayBatDauDangKy = DateTime.Today.AddDays(-50), HanDangKy = DateTime.Today.AddDays(-20), NgayKhaiGiang = DateTime.Today.AddDays(-15), MoTa = "Khóa học di động đợt trước, chuẩn bị bước vào giai đoạn làm đồ án.", YeuCauHocVien = "Có tư duy lập trình", TrangThai = "Đang mở đăng ký", PhongHoc = "Lab 201 - Minh Khai" },
                new LopHoc { TenLop = "CEH-K07", MaChuongTrinh = ctdt4, MaGiangVien = gvId3, TenGiangVien = "ThS. Nguyễn Thành Nam", TrinhDoDauVao = "Nâng cao", SiSoToiDa = 18, SoBuoiHoc = 18, HocPhi = 5500000, NgayBatDauDangKy = DateTime.Today.AddDays(-40), HanDangKy = DateTime.Today.AddDays(-10), NgayKhaiGiang = DateTime.Today.AddDays(-5), MoTa = "Đã kín chỗ khóa học an toàn thông tin chuyên sâu.", YeuCauHocVien = "Hiểu mạng máy tính căn bản", TrangThai = "Đã đóng đăng ký", PhongHoc = "Phòng Lab 501" },

                // Nhóm 3: Tạm dừng (3 lớp)
                new LopHoc { TenLop = "CLOUD-AWS-K01", MaChuongTrinh = ctdt6, MaGiangVien = gvId4, TenGiangVien = "PGS.TS. Lê Bá Thành", TrinhDoDauVao = "Nâng cao", SiSoToiDa = 20, SoBuoiHoc = 14, HocPhi = 4900000, NgayBatDauDangKy = DateTime.Today.AddDays(-10), HanDangKy = DateTime.Today.AddDays(10), NgayKhaiGiang = DateTime.Today.AddDays(20), MoTa = "Kiến trúc đám mây AWS Solutions Architect. Tạm dừng để nâng cấp phòng máy.", YeuCauHocVien = "Có kiến thức hạ tầng mạng", TrangThai = "Tạm dừng", PhongHoc = "Lab Cloud 502" },
                new LopHoc { TenLop = "DATA-BI-K03", MaChuongTrinh = ctdt2, MaGiangVien = gvId2, TenGiangVien = "TS. Hoàng Thị Lan", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 25, SoBuoiHoc = 12, HocPhi = 2900000, NgayBatDauDangKy = DateTime.Today.AddDays(-15), HanDangKy = DateTime.Today.AddDays(5), NgayKhaiGiang = DateTime.Today.AddDays(15), MoTa = "Phân tích và trực quan dữ liệu với Power BI, DAX & Data Modeling.", YeuCauHocVien = "Excel cơ bản", TrangThai = "Tạm dừng", PhongHoc = "Lab 305" },
                new LopHoc { TenLop = "LINUX-ADMIN-K01", MaChuongTrinh = ctdt4, MaGiangVien = gvId3, TenGiangVien = "ThS. Nguyễn Thành Nam", TrinhDoDauVao = "Cơ bản", SiSoToiDa = 25, SoBuoiHoc = 10, HocPhi = 2500000, NgayBatDauDangKy = DateTime.Today.AddDays(-10), HanDangKy = DateTime.Today.AddDays(10), NgayKhaiGiang = DateTime.Today.AddDays(20), MoTa = "Quản trị hệ thống máy chủ Linux RedHat/CentOS và shell script.", YeuCauHocVien = "Tin học văn phòng", TrangThai = "Tạm dừng", PhongHoc = "Lab 301" },

                // Nhóm 4: Chưa mở (3 lớp)
                new LopHoc { TenLop = "GOLANG-MICRO-K01", MaChuongTrinh = ctdt7, MaGiangVien = gvId1, TenGiangVien = "ThS. Trần Văn Hùng", TrinhDoDauVao = "Chuyên sâu", SiSoToiDa = 20, SoBuoiHoc = 14, HocPhi = 4800000, NgayBatDauDangKy = DateTime.Today.AddDays(10), HanDangKy = DateTime.Today.AddDays(30), NgayKhaiGiang = DateTime.Today.AddDays(40), MoTa = "Xây dựng hệ thống Microservices hiệu năng cao với Golang, gRPC và Kafka.", YeuCauHocVien = "Thành thạo ít nhất 1 ngôn ngữ OOP", TrangThai = "Chưa mở", PhongHoc = "Lab 402" },
                new LopHoc { TenLop = "CYBER-DEF-K01", MaChuongTrinh = ctdt4, MaGiangVien = gvId3, TenGiangVien = "ThS. Nguyễn Thành Nam", TrinhDoDauVao = "Nâng cao", SiSoToiDa = 20, SoBuoiHoc = 15, HocPhi = 5200000, NgayBatDauDangKy = DateTime.Today.AddDays(15), HanDangKy = DateTime.Today.AddDays(35), NgayKhaiGiang = DateTime.Today.AddDays(45), MoTa = "Phòng thủ an ninh mạng Blue Team, SIEM SOC và ứng cứu sự cố không gian mạng.", YeuCauHocVien = "Đã học bảo mật cơ bản", TrangThai = "Chưa mở", PhongHoc = "Phòng Lab 501" },
                new LopHoc { TenLop = "BLOCKCHAIN-K01", MaChuongTrinh = ctdt1, MaGiangVien = gvId4, TenGiangVien = "PGS.TS. Lê Bá Thành", TrinhDoDauVao = "Chuyên sâu", SiSoToiDa = 15, SoBuoiHoc = 14, HocPhi = 6500000, NgayBatDauDangKy = DateTime.Today.AddDays(20), HanDangKy = DateTime.Today.AddDays(40), NgayKhaiGiang = DateTime.Today.AddDays(50), MoTa = "Phát triển ứng dụng Web3 và Smart Contracts với Solidity, Ethereum và Hardhat.", YeuCauHocVien = "Lập trình web thành thạo", TrangThai = "Chưa mở", PhongHoc = "Lab Cloud 502" }
            };

            foreach (var lop in newLopHocs)
            {
                if (!existingClassNames.Any(en => en.Equals(lop.TenLop, StringComparison.OrdinalIgnoreCase)))
                {
                    context.LopHocs.Add(lop);
                }
            }
            context.SaveChanges();

            var allLops = context.LopHocs.OrderBy(l => l.MaLopHoc).ToList();

            // ========================================================
            // 5. HỌC VIÊN & TÀI KHOẢN HỌC VIÊN (Mục 16: Tối thiểu 30 -> Seed 42 Học viên)
            // ========================================================
            var sampleStudents = new (string HoTen, string Email, string Sdt, DateTime NgaySinh, string GioiTinh, string DiaChi, string TrinhDo, string NgheNghiep, bool TrangThai)[]
            {
                ("Nguyễn Văn An", "an.nguyen@gmail.com", "0987654321", new DateTime(2003, 5, 20), "Nam", "Số 15 Hai Bà Trưng, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                ("Trần Minh Tuấn", "tuan.tm@uneti.edu.vn", "0912345001", new DateTime(2003, 8, 20), "Nam", "Số 454 Minh Khai, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                ("Lê Thu Hà", "ha.lt@gmail.com", "0912345002", new DateTime(2005, 2, 14), "Nữ", "Số 12 Chùa Bộc, Hà Nội", "THPT", "Học sinh", true),
                ("Phạm Quốc Bảo", "bao.pq@gmail.com", "0912345003", new DateTime(2002, 11, 9), "Nam", "Số 88 Giải Phóng, Hà Nội", "Đại học", "Lập trình viên tập sự", true),
                ("Lê Hoàng Nam", "nam.lh@gmail.com", "0912345004", new DateTime(2001, 9, 30), "Nam", "Số 56 Cầu Giấy, Hà Nội", "Kỹ sư", "Nhân viên IT", true),
                ("Phạm Viết Khang", "khang.pv@gmail.com", "0912345005", new DateTime(2004, 3, 18), "Nam", "Hoàng Mai, Hà Nội", "Cao đẳng", "Sinh viên", false), // Bị khóa để test
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
                ("Đan Trường", "truong.d@gmail.com", "0912345031", new DateTime(2002, 11, 29), "Nam", "Tây Hồ, Hà Nội", "Đại học", "Tester kiểm thử phần mềm", true),
                ("Nguyễn Cẩm Ly", "ly.nc@gmail.com", "0912345032", new DateTime(2003, 10, 12), "Nữ", "Hai Bà Trưng, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                ("Dương Triệu Vũ", "vu.dt@gmail.com", "0912345033", new DateTime(2002, 6, 18), "Nam", "Hoàng Mai, Hà Nội", "Cao đẳng", "Kỹ thuật viên IT", true),
                ("Phạm Quỳnh Anh", "anh.pq@gmail.com", "0912345034", new DateTime(2004, 4, 25), "Nữ", "Cầu Giấy, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                ("Ưng Hoàng Phúc", "phuc.uh@gmail.com", "0912345035", new DateTime(2001, 8, 14), "Nam", "Ba Đình, Hà Nội", "Kỹ sư", "DevOps Engineer", true),
                ("Hà Anh Tuấn", "tuan.ha@gmail.com", "0912345036", new DateTime(2000, 12, 15), "Nam", "Hoàn Kiếm, Hà Nội", "Thạc sĩ", "Chuyên viên dữ liệu", true),
                ("Vũ Cát Tường", "tuong.vc@gmail.com", "0912345037", new DateTime(2003, 10, 2), "Nữ", "Thanh Xuân, Hà Nội", "Đại học", "Sinh viên UNETI", true),
                ("Sơn Tùng MTP", "tung.m@gmail.com", "0912345038", new DateTime(2004, 7, 5), "Nam", "Nam Từ Liêm, Hà Nội", "Đại học", "Thiết kế đồ họa", true),
                ("Bích Phương", "phuong.b@gmail.com", "0912345039", new DateTime(2002, 7, 12), "Nữ", "Hà Đông, Hà Nội", "Đại học", "Marketing Specialist", true),
                ("Soobin Hoàng Sơn", "son.sh@gmail.com", "0912345040", new DateTime(2001, 9, 10), "Nam", "Tây Hồ, Hà Nội", "Kỹ sư", "Frontend Developer", true),
                ("Tóc Tiên", "tien.tt@gmail.com", "0912345041", new DateTime(2003, 5, 13), "Nữ", "Đống Đa, Hà Nội", "Đại học", "Sinh viên UNETI", true)
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
                            MatKhau = SecurityHelper.HashPassword("123456"),
                            HoTen = s.HoTen,
                            Email = s.Email,
                            VaiTro = "HocVien",
                            TrangThai = s.TrangThai,
                            NgayTao = DateTime.Today.AddDays(-30 + (i % 25))
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
                        NgayTao = DateTime.Today.AddDays(-30 + (i % 25))
                    };
                    context.HocViens.Add(hv);
                    context.SaveChanges();
                }
            }

            var allStudents = context.HocViens.OrderBy(h => h.MaHocVien).ToList();

            // ========================================================
            // 6. ĐĂNG KÝ HỌC (Mục 16: Seed 60+ đăng ký với đầy đủ các trạng thái và nghiệp vụ)
            // ========================================================
            if (context.DangKyHocs.Count() < 60 && allStudents.Any() && allLops.Any())
            {
                var existingDks = context.DangKyHocs.Select(d => new { d.MaHocVien, d.MaLopHoc }).ToHashSet();
                var dksToAdd = new List<DangKyHoc>();

                var statuses = new[]
                {
                    // 10 Chờ duyệt
                    "Chờ duyệt", "Chờ duyệt", "Chờ duyệt", "Chờ duyệt", "Chờ duyệt",
                    "Chờ duyệt", "Chờ duyệt", "Chờ duyệt", "Chờ duyệt", "Chờ duyệt",
                    // 8 Đã duyệt
                    "Đã duyệt", "Đã duyệt", "Đã duyệt", "Đã duyệt",
                    "Đã duyệt", "Đã duyệt", "Đã duyệt", "Đã duyệt",
                    // 10 Đã đóng học phí
                    "Đã đóng học phí", "Đã đóng học phí", "Đã đóng học phí", "Đã đóng học phí", "Đã đóng học phí",
                    "Đã đóng học phí", "Đã đóng học phí", "Đã đóng học phí", "Đã đóng học phí", "Đã đóng học phí",
                    // 15 Đang học
                    "Đang học", "Đang học", "Đang học", "Đang học", "Đang học",
                    "Đang học", "Đang học", "Đang học", "Đang học", "Đang học",
                    "Đang học", "Đang học", "Đang học", "Đang học", "Đang học",
                    // 10 Hoàn thành
                    "Hoàn thành", "Hoàn thành", "Hoàn thành", "Hoàn thành", "Hoàn thành",
                    "Hoàn thành", "Hoàn thành", "Hoàn thành", "Hoàn thành", "Hoàn thành",
                    // 4 Không hoàn thành
                    "Không hoàn thành", "Không hoàn thành", "Không hoàn thành", "Không hoàn thành",
                    // 3 Từ chối
                    "Từ chối", "Từ chối", "Từ chối",
                    // 3 Đã hủy
                    "Đã hủy", "Đã hủy", "Đã hủy"
                };

                int studentIdx = 0;
                int lopIdx = 0;

                for (int i = 0; i < statuses.Length; i++)
                {
                    var hv = allStudents[studentIdx % allStudents.Count];
                    var lop = allLops[lopIdx % allLops.Count];

                    // Tránh trùng cặp (học viên, lớp)
                    int attempts = 0;
                    while (existingDks.Contains(new { MaHocVien = hv.MaHocVien, MaLopHoc = lop.MaLopHoc }) && attempts < 30)
                    {
                        studentIdx++;
                        hv = allStudents[studentIdx % allStudents.Count];
                        attempts++;
                    }

                    if (existingDks.Contains(new { MaHocVien = hv.MaHocVien, MaLopHoc = lop.MaLopHoc }))
                    {
                        lopIdx++;
                        continue;
                    }

                    string st = statuses[i];
                    decimal feePaid = 0;
                    DateTime? paidDate = null;
                    DateTime? processDate = null;
                    string note = "";

                    if (st == "Chờ duyệt")
                    {
                        note = "Học viên vừa gửi đơn đăng ký trực tuyến, chờ giáo vụ xét duyệt hồ sơ";
                    }
                    else if (st == "Đã duyệt")
                    {
                        processDate = DateTime.Now.AddDays(-8);
                        note = "Đã duyệt hồ sơ đủ điều kiện đầu vào, gửi thông báo đóng học phí";
                    }
                    else if (st == "Đã đóng học phí")
                    {
                        processDate = DateTime.Now.AddDays(-12);
                        feePaid = lop.HocPhi;
                        paidDate = DateTime.Now.AddDays(-7);
                        note = "Đã nộp đủ 100% học phí qua cổng ngân hàng / QR VietQR";
                    }
                    else if (st == "Đang học" || st == "Hoàn thành" || st == "Không hoàn thành")
                    {
                        processDate = DateTime.Now.AddDays(-28);
                        feePaid = lop.HocPhi;
                        paidDate = DateTime.Now.AddDays(-25);
                        note = st == "Đang học" ? "Học viên đang học tích cực theo lịch" : "Đã kết thúc môn học";
                    }
                    else if (st == "Từ chối")
                    {
                        processDate = DateTime.Now.AddDays(-5);
                        note = "Không đáp ứng điều kiện tiên quyết / hồ sơ thiếu minh chứng";
                    }
                    else if (st == "Đã hủy")
                    {
                        note = "Học viên liên hệ xin rút đơn do bận việc đột xuất";
                    }

                    dksToAdd.Add(new DangKyHoc
                    {
                        MaHocVien = hv.MaHocVien,
                        MaLopHoc = lop.MaLopHoc,
                        NgayDangKy = DateTime.Now.AddDays(-25 + (i % 20)),
                        TrangThai = st,
                        SoTienDaDong = feePaid,
                        NgayDongHocPhi = paidDate,
                        NgayXuLy = processDate,
                        NhanXetGiaoVu = note,
                        GhiChu = $"Học viên {hv.HoTen} đăng ký lớp {lop.TenLop}"
                    });

                    existingDks.Add(new { MaHocVien = hv.MaHocVien, MaLopHoc = lop.MaLopHoc });
                    studentIdx++;
                    if (i % 2 == 0) lopIdx++;
                }

                if (dksToAdd.Any())
                {
                    context.DangKyHocs.AddRange(dksToAdd);
                    context.SaveChanges();
                }
            }

            // ========================================================
            // 7. BUỔI HỌC / THỜI KHÓA BIỂU (Mục 16: Seed 35+ Buổi học phong phú các ca, các phòng)
            // ========================================================
            if (context.BuoiHocs.Count() < 35 && allLops.Any())
            {
                var sessionsToAdd = new List<BuoiHoc>();
                var rooms = new[] { "Lab 402 - Lĩnh Nam", "Lab 305 - Lĩnh Nam", "Lab 201 - Minh Khai", "Lab AI 405", "Phòng Studio 302", "Lab Cloud 502", "Hội trường B" };

                for (int i = 0; i < allLops.Count; i++)
                {
                    var lop = allLops[i];
                    string rm = rooms[i % rooms.Length];
                    string gvName = lop.TenGiangVien ?? "ThS. Trần Văn Hùng";

                    // Buổi quá khứ 1 (Đã hoàn thành)
                    sessionsToAdd.Add(new BuoiHoc
                    {
                        MaLopHoc = lop.MaLopHoc,
                        MaGiangVien = lop.MaGiangVien,
                        GiangVien = gvName,
                        PhongHoc = rm,
                        ThoiGianBatDau = DateTime.Today.AddDays(-10).AddHours(8 + (i % 3) * 4),
                        ThoiGianKetThuc = DateTime.Today.AddDays(-10).AddHours(11 + (i % 3) * 4),
                        NoiDung = $"Buổi 1: Giới thiệu đề cương môn học, cài đặt công cụ và môi trường cho lớp {lop.TenLop}",
                        GhiChu = "Đã hoàn thành tốt đẹp, sĩ số có mặt 100%",
                        TrangThai = "Đã hoàn thành"
                    });

                    // Buổi quá khứ 2 (Đã hoàn thành)
                    sessionsToAdd.Add(new BuoiHoc
                    {
                        MaLopHoc = lop.MaLopHoc,
                        MaGiangVien = lop.MaGiangVien,
                        GiangVien = gvName,
                        PhongHoc = rm,
                        ThoiGianBatDau = DateTime.Today.AddDays(-5).AddHours(8 + (i % 3) * 4),
                        ThoiGianKetThuc = DateTime.Today.AddDays(-5).AddHours(11 + (i % 3) * 4),
                        NoiDung = $"Buổi 2: Kiến trúc hệ thống cốt lõi và bài thực hành mã nguồn mở {lop.TenLop}",
                        GhiChu = "Thực hành trên phòng Lab đạt kết quả tốt",
                        TrangThai = "Đã hoàn thành"
                    });

                    // Buổi tương lai (Đã lên lịch)
                    sessionsToAdd.Add(new BuoiHoc
                    {
                        MaLopHoc = lop.MaLopHoc,
                        MaGiangVien = lop.MaGiangVien,
                        GiangVien = gvName,
                        PhongHoc = rm,
                        ThoiGianBatDau = DateTime.Today.AddDays(1 + (i % 7)).AddHours(8 + (i % 2) * 5),
                        ThoiGianKetThuc = DateTime.Today.AddDays(1 + (i % 7)).AddHours(11 + (i % 2) * 5),
                        NoiDung = $"Buổi 3: Thực hành đồ án chuyên sâu, hướng dẫn tối ưu và kiểm thử dự án",
                        GhiChu = "Yêu cầu học viên mang theo laptop cá nhân và làm trước bài tập về nhà",
                        TrangThai = "Đã lên lịch"
                    });

                    if (sessionsToAdd.Count >= 36) break;
                }

                // Thêm 2 buổi học Đã hủy (mẫu trường hợp hoãn/hủy ca học)
                sessionsToAdd.Add(new BuoiHoc
                {
                    MaLopHoc = allLops[0].MaLopHoc,
                    MaGiangVien = allLops[0].MaGiangVien,
                    GiangVien = allLops[0].TenGiangVien ?? "ThS. Trần Văn Hùng",
                    PhongHoc = "Hội trường B",
                    ThoiGianBatDau = DateTime.Today.AddDays(4).AddHours(13),
                    ThoiGianKetThuc = DateTime.Today.AddDays(4).AddHours(16),
                    NoiDung = "Chuyên đề mở rộng: Trải nghiệm công nghệ thế hệ mới (Hoãn do bảo trì hội trường)",
                    GhiChu = "Đã báo nghỉ cho sinh viên qua SMS và Email",
                    TrangThai = "Đã hủy"
                });

                sessionsToAdd.Add(new BuoiHoc
                {
                    MaLopHoc = allLops[1].MaLopHoc,
                    MaGiangVien = allLops[1].MaGiangVien,
                    GiangVien = allLops[1].TenGiangVien ?? "TS. Hoàng Thị Lan",
                    PhongHoc = "Lab 305 - Lĩnh Nam",
                    ThoiGianBatDau = DateTime.Today.AddDays(6).AddHours(18),
                    ThoiGianKetThuc = DateTime.Today.AddDays(6).AddHours(21),
                    NoiDung = "Ôn tập chuyên đề thi cuối kỳ (Dời sang buổi phụ đạo)",
                    GhiChu = "Giảng viên có lịch công tác đột xuất",
                    TrangThai = "Đã hủy"
                });

                context.BuoiHocs.AddRange(sessionsToAdd);
                context.SaveChanges();
            }

            // ========================================================
            // 8. KẾT QUẢ HỌC TẬP (Mục 16: Seed 25+ bản ghi điểm đầy đủ thang điểm Xuất sắc -> Chưa đạt)
            // ========================================================
            if (context.KetQuaHocTaps.Count() < 25)
            {
                var gradedDks = context.DangKyHocs
                    .Where(d => d.TrangThai == "Hoàn thành" || d.TrangThai == "Không hoàn thành" || d.TrangThai == "Đang học")
                    .ToList();

                var existingKqDkIds = context.KetQuaHocTaps.Select(k => k.MaDangKy).ToHashSet();

                var kqsToAdd = new List<KetQuaHocTap>();
                var mockScores = new (double cc, double ck, string nhanxet)[]
                {
                    (9.5, 9.8, "Xuất sắc: Nắm bắt kiến trúc rất nhanh, đồ án sáng tạo và bảo vệ xuất sắc trước hội đồng."),
                    (9.0, 9.5, "Xuất sắc: Kỹ năng code tốt, phong cách lập trình chuyên nghiệp, bài tập chuẩn chỉ."),
                    (8.5, 8.8, "Giỏi: Tham gia tích cực, bài thi cuối kỳ giải quyết bài toán phức tạp đúng thời hạn."),
                    (8.0, 8.5, "Giỏi: Tinh thần làm việc nhóm cao, nắm chắc các nguyên lý thiết kế và Clean Code."),
                    (8.5, 8.0, "Giỏi: Chăm chỉ đi học đúng giờ, hoàn thành đầy đủ bài tập thực hành trên phòng Lab."),
                    (7.5, 8.0, "Khá: Hiểu tốt kiến thức cốt lõi, sản phẩm chạy ổn định nhưng cần tối ưu thêm giao diện."),
                    (8.0, 7.5, "Khá: Tương tác tốt trên lớp, đạt các chuẩn đầu ra cơ bản của học phần."),
                    (7.0, 7.0, "Khá: Đồ án hoàn thành đúng tiến độ, đáp ứng đầy đủ yêu cầu của giảng viên hướng dẫn."),
                    (6.5, 7.0, "Trung bình khá: Đạt chuẩn đầu ra, cần trau dồi thêm kỹ năng debug và tư duy thuật toán."),
                    (8.0, 6.0, "Trung bình khá: Điểm danh đầy đủ, bài thi cuối khóa cần chú ý quản lý ngoại lệ cẩn thận hơn."),
                    (6.0, 5.5, "Trung bình: Đạt yêu cầu tối thiểu của học phần, cần rèn luyện thêm thời gian thực hành."),
                    (5.0, 5.0, "Trung bình: Vừa đủ điểm qua môn, khuyến khích tự học thêm các khóa nâng cao."),
                    (4.0, 4.0, "Không đạt: Điểm thi cuối kỳ dưới trung bình, chưa nắm vững các kiến thức nền tảng bắt buộc."),
                    (3.5, 4.5, "Không đạt: Chưa hoàn thành sản phẩm đồ án kết thúc môn học."),
                    (3.0, 3.0, "Không đạt: Vắng nhiều buổi thực hành quan trọng, không đủ điều kiện xét chứng chỉ."),
                    (2.0, 2.5, "Không đạt: Vi phạm quy chế thi và không nộp báo cáo đồ án đúng thời hạn."),
                    (9.8, 9.5, "Xuất sắc: Top 1 của lớp học phần, đồ án có tính ứng dụng doanh nghiệp thực tế cao."),
                    (8.8, 8.5, "Giỏi: Chủ động nghiên cứu tài liệu tiếng Anh, giải quyết lỗi mã nguồn nhanh nhạy."),
                    (7.8, 7.2, "Khá: Tiếp thu bài nhanh, làm bài tập đầy đủ."),
                    (6.8, 6.5, "Trung bình khá: Hoàn thành khóa học đúng chuẩn.")
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

                        // Cập nhật trạng thái đăng ký học đồng bộ với kết quả
                        dk.TrangThai = kqStr;

                        existingKqDkIds.Add(dk.MaDangKy);
                        scoreIdx++;
                        if (kqsToAdd.Count >= 28) break;
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
