using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Models;

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Data
{
    /// <summary>
    /// Helper nạp sẵn dữ liệu phong phú để test toàn diện giao diện giống Giao_DIen_Test
    /// </summary>
    public static class DbSeeder
    {
        public static void SeedMockData(ApplicationDbContext context)
        {
            // 1. Kiểm tra và bổ sung Lớp học mẫu nếu mới chỉ có 1 lớp
            if (context.LopHocs.Count() <= 1)
            {
                var lop2 = new LopHoc
                {
                    TenLop = "PYTHON-K02",
                    MaChuongTrinh = 2,
                    TenGiangVien = "ThS. Trần Văn Hùng",
                    TrinhDoDauVao = "Cơ bản",
                    SiSoToiDa = 30,
                    SoBuoiHoc = 14,
                    HocPhi = 3200000,
                    NgayBatDauDangKy = DateTime.Today.AddDays(-10),
                    HanDangKy = DateTime.Today.AddDays(10),
                    NgayKhaiGiang = DateTime.Today.AddDays(15),
                    MoTa = "Khoa học Dữ liệu & Lập trình phân tích dữ liệu với Python, Pandas, Numpy",
                    TrangThai = "Đang mở đăng ký",
                    PhongHoc = "Lab 305"
                };

                var lop3 = new LopHoc
                {
                    TenLop = "REACT-K05",
                    MaChuongTrinh = 1,
                    TenGiangVien = "ThS. Trần Văn Hùng",
                    TrinhDoDauVao = "Trung cấp",
                    SiSoToiDa = 25,
                    SoBuoiHoc = 16,
                    HocPhi = 3800000,
                    NgayBatDauDangKy = DateTime.Today.AddDays(-5),
                    HanDangKy = DateTime.Today.AddDays(15),
                    NgayKhaiGiang = DateTime.Today.AddDays(20),
                    MoTa = "Lập trình Web Frontend hiện đại với React 19, TypeScript, Next.js và Tailwind",
                    TrangThai = "Đang mở đăng ký",
                    PhongHoc = "Lab 402"
                };

                var lop4 = new LopHoc
                {
                    TenLop = "TOEIC-K12",
                    MaChuongTrinh = 1,
                    TenGiangVien = "Cô Hoàng Thị Lan",
                    TrinhDoDauVao = "Cơ bản",
                    SiSoToiDa = 35,
                    SoBuoiHoc = 24,
                    HocPhi = 2500000,
                    NgayBatDauDangKy = DateTime.Today.AddDays(-15),
                    HanDangKy = DateTime.Today.AddDays(5),
                    NgayKhaiGiang = DateTime.Today.AddDays(10),
                    MoTa = "Luyện thi TOEIC 650+ chuẩn đầu ra tốt nghiệp Đại học UNETI",
                    TrangThai = "Đang mở đăng ký",
                    PhongHoc = "Phòng 201"
                };

                context.LopHocs.AddRange(lop2, lop3, lop4);
                context.SaveChanges();
            }

            // 2. Bổ sung danh sách học viên mẫu nếu hiện tại mới chỉ có <= 2 học viên
            if (context.HocViens.Count() <= 2)
            {
                var lopNet = context.LopHocs.FirstOrDefault(l => l.TenLop == "NETCORE10-K01");
                var lopPy = context.LopHocs.FirstOrDefault(l => l.TenLop == "PYTHON-K02");
                var lopReact = context.LopHocs.FirstOrDefault(l => l.TenLop == "REACT-K05");
                var lopToeic = context.LopHocs.FirstOrDefault(l => l.TenLop == "TOEIC-K12");

                var studentList = new List<HocVien>
                {
                    new HocVien
                    {
                        HoTen = "Trần Minh Tuấn",
                        Email = "tuan.tm@uneti.edu.vn",
                        SoDienThoai = "0987654321",
                        NgaySinh = new DateTime(2003, 8, 20),
                        GioiTinh = "Nam",
                        DiaChi = "Số 454 Minh Khai, Hai Bà Trưng, Hà Nội",
                        TrinhDoHocVan = "Đại học Năm 3",
                        NgheNghiep = "Sinh viên UNETI",
                        TrangThai = true,
                        NgayTao = DateTime.Now.AddDays(-20)
                    },
                    new HocVien
                    {
                        HoTen = "Lê Thu Hà",
                        Email = "ha.lt@gmail.com",
                        SoDienThoai = "0901234888",
                        NgaySinh = new DateTime(2005, 2, 14),
                        GioiTinh = "Nữ",
                        DiaChi = "Số 12 Chùa Bộc, Đống Đa, Hà Nội",
                        TrinhDoHocVan = "Tốt nghiệp THPT",
                        NgheNghiep = "Học sinh",
                        TrangThai = true,
                        NgayTao = DateTime.Now.AddDays(-18)
                    },
                    new HocVien
                    {
                        HoTen = "Phạm Quốc Bảo",
                        Email = "bao.pq@gmail.com",
                        SoDienThoai = "0934567890",
                        NgaySinh = new DateTime(2002, 11, 9),
                        GioiTinh = "Nam",
                        DiaChi = "Số 88 Giải Phóng, Thanh Xuân, Hà Nội",
                        TrinhDoHocVan = "Đại học",
                        NgheNghiep = "Lập trình viên tập sự",
                        TrangThai = true,
                        NgayTao = DateTime.Now.AddDays(-15)
                    },
                    new HocVien
                    {
                        HoTen = "Lê Hoàng Nam",
                        Email = "nam.lh@gmail.com",
                        SoDienThoai = "0978901234",
                        NgaySinh = new DateTime(2001, 9, 30),
                        GioiTinh = "Nam",
                        DiaChi = "Số 56 Cầu Giấy, Hà Nội",
                        TrinhDoHocVan = "Kỹ sư",
                        NgheNghiep = "Nhân viên IT",
                        TrangThai = true,
                        NgayTao = DateTime.Now.AddDays(-12)
                    },
                    new HocVien
                    {
                        HoTen = "Phạm Viết Khang",
                        Email = "khang.pv@gmail.com",
                        SoDienThoai = "0965432109",
                        NgaySinh = new DateTime(2004, 3, 18),
                        GioiTinh = "Nam",
                        DiaChi = "Hoàng Mai, Hà Nội",
                        TrinhDoHocVan = "Cao đẳng",
                        NgheNghiep = "Sinh viên",
                        TrangThai = false, // Tạm khóa
                        NgayTao = DateTime.Now.AddDays(-10)
                    },
                    new HocVien
                    {
                        HoTen = "Đặng Thùy Trang",
                        Email = "trang.dt@gmail.com",
                        SoDienThoai = "0943219876",
                        NgaySinh = new DateTime(2003, 12, 5),
                        GioiTinh = "Nữ",
                        DiaChi = "Bạch Mai, Hà Nội",
                        TrinhDoHocVan = "Đại học",
                        NgheNghiep = "Kế toán viên",
                        TrangThai = true,
                        NgayTao = DateTime.Now.AddDays(-8)
                    },
                    new HocVien
                    {
                        HoTen = "Vũ Hải Đăng",
                        Email = "dang.vh@gmail.com",
                        SoDienThoai = "0918765432",
                        NgaySinh = new DateTime(2004, 7, 22),
                        GioiTinh = "Nam",
                        DiaChi = "Trương Định, Hà Nội",
                        TrinhDoHocVan = "Đại học",
                        NgheNghiep = "Sinh viên UNETI",
                        TrangThai = true,
                        NgayTao = DateTime.Now.AddDays(-6)
                    },
                    new HocVien
                    {
                        HoTen = "Hoàng Thùy Linh",
                        Email = "linh.ht@gmail.com",
                        SoDienThoai = "0922334455",
                        NgaySinh = new DateTime(2002, 5, 16),
                        GioiTinh = "Nữ",
                        DiaChi = "Lĩnh Nam, Hà Nội",
                        TrinhDoHocVan = "Đại học",
                        NgheNghiep = "Chuyên viên Marketing",
                        TrangThai = true,
                        NgayTao = DateTime.Now.AddDays(-4)
                    },
                    new HocVien
                    {
                        HoTen = "Nguyễn Quang Hải",
                        Email = "hai.nq@gmail.com",
                        SoDienThoai = "0933445566",
                        NgaySinh = new DateTime(2003, 1, 19),
                        GioiTinh = "Nam",
                        DiaChi = "Định Công, Hà Nội",
                        TrinhDoHocVan = "Cao đẳng",
                        NgheNghiep = "Kỹ thuật viên mạng",
                        TrangThai = true,
                        NgayTao = DateTime.Now.AddDays(-2)
                    }
                };

                context.HocViens.AddRange(studentList);
                context.SaveChanges();

                // Tạo đăng ký học mẫu cho các học viên
                var dks = new List<DangKyHoc>();
                if (lopNet != null && studentList.Count > 0)
                {
                    dks.Add(new DangKyHoc
                    {
                        MaHocVien = studentList[0].MaHocVien, // Tuấn
                        MaLopHoc = lopNet.MaLopHoc,
                        NgayDangKy = DateTime.Now.AddDays(-15),
                        TrangThai = "Đã duyệt",
                        SoTienDaDong = lopNet.HocPhi,
                        NgayDongHocPhi = DateTime.Now.AddDays(-10)
                    });
                }
                if (lopPy != null && studentList.Count > 2)
                {
                    dks.Add(new DangKyHoc
                    {
                        MaHocVien = studentList[2].MaHocVien, // Bảo
                        MaLopHoc = lopPy.MaLopHoc,
                        NgayDangKy = DateTime.Now.AddDays(-10),
                        TrangThai = "Đang học",
                        SoTienDaDong = lopPy.HocPhi,
                        NgayDongHocPhi = DateTime.Now.AddDays(-7)
                    });
                }
                if (lopToeic != null && studentList.Count > 1)
                {
                    dks.Add(new DangKyHoc
                    {
                        MaHocVien = studentList[1].MaHocVien, // Hà
                        MaLopHoc = lopToeic.MaLopHoc,
                        NgayDangKy = DateTime.Now.AddDays(-8),
                        TrangThai = "Chờ duyệt",
                        SoTienDaDong = 0
                    });
                }
                if (lopReact != null && studentList.Count > 3)
                {
                    dks.Add(new DangKyHoc
                    {
                        MaHocVien = studentList[3].MaHocVien, // Nam
                        MaLopHoc = lopReact.MaLopHoc,
                        NgayDangKy = DateTime.Now.AddDays(-5),
                        TrangThai = "Đã duyệt",
                        SoTienDaDong = lopReact.HocPhi,
                        NgayDongHocPhi = DateTime.Now.AddDays(-3)
                    });
                }

                if (dks.Any())
                {
                    context.DangKyHocs.AddRange(dks);
                    context.SaveChanges();
                }
            }

            // 3. Bổ sung các tài khoản mẫu cho khớp với admin-tai-khoan.html nếu chưa có
            if (context.TaiKhoans.Count() <= 3)
            {
                var taiKhoanThem = new List<TaiKhoan>
                {
                    new TaiKhoan
                    {
                        TenDangNhap = "giaovu_mai",
                        MatKhau = "giaovu123",
                        HoTen = "Nguyễn Thị Mai",
                        Email = "mai.nt@uneti.edu.vn",
                        VaiTro = "GiaoVu",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-40)
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "giaovu_hung",
                        MatKhau = "giaovu123",
                        HoTen = "Trần Văn Hùng",
                        Email = "hung.tv@uneti.edu.vn",
                        VaiTro = "GiaoVu",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-38)
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "admin_backup",
                        MatKhau = "admin123",
                        HoTen = "Phụ tá Quản trị",
                        Email = "admin2@uneti.edu.vn",
                        VaiTro = "Admin",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-35)
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "hocvien01",
                        MatKhau = "123456",
                        HoTen = "Nguyễn Văn An",
                        Email = "an.nguyen@gmail.com",
                        VaiTro = "HocVien",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-25)
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "hocvien02",
                        MatKhau = "123456",
                        HoTen = "Trần Minh Tuấn",
                        Email = "tuan.tm@uneti.edu.vn",
                        VaiTro = "HocVien",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-20)
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "hocvien03",
                        MatKhau = "123456",
                        HoTen = "Lê Thu Hà",
                        Email = "ha.lt@gmail.com",
                        VaiTro = "HocVien",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-18)
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "hocvien04",
                        MatKhau = "123456",
                        HoTen = "Phạm Quốc Bảo",
                        Email = "bao.pq@gmail.com",
                        VaiTro = "HocVien",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-15)
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "hocvien05",
                        MatKhau = "123456",
                        HoTen = "Lê Hoàng Nam",
                        Email = "nam.lh@gmail.com",
                        VaiTro = "HocVien",
                        TrangThai = true,
                        NgayTao = DateTime.Today.AddDays(-12)
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "hocvien_khoa",
                        MatKhau = "123456",
                        HoTen = "Phạm Viết Khang",
                        Email = "khang.pv@gmail.com",
                        VaiTro = "HocVien",
                        TrangThai = false, // Tài khoản bị khóa chuẩn Mục 5.1
                        NgayTao = DateTime.Today.AddDays(-10)
                    }
                };

                context.TaiKhoans.AddRange(taiKhoanThem);
                context.SaveChanges();

                // Liên kết tài khoản với học viên theo Email
                var allHocViens = context.HocViens.ToList();
                var allTaiKhoans = context.TaiKhoans.ToList();
                foreach (var hv in allHocViens)
                {
                    if (hv.MaTaiKhoan == null)
                    {
                        var tk = allTaiKhoans.FirstOrDefault(t => t.Email.ToLower() == hv.Email.ToLower());
                        if (tk != null)
                        {
                            hv.MaTaiKhoan = tk.MaTaiKhoan;
                        }
                    }
                }
                context.SaveChanges();
            }

            // 4. Bổ sung Buổi học mẫu (Module 5) nếu chưa có
            if (!context.BuoiHocs.Any())
            {
                var lopNet = context.LopHocs.FirstOrDefault(l => l.TenLop == "NETCORE10-K01");
                var lopPy = context.LopHocs.FirstOrDefault(l => l.TenLop == "PYTHON-K02");
                var lopReact = context.LopHocs.FirstOrDefault(l => l.TenLop == "REACT-K05");

                var buoiHocs = new List<BuoiHoc>();
                if (lopNet != null)
                {
                    buoiHocs.Add(new BuoiHoc
                    {
                        MaLopHoc = lopNet.MaLopHoc,
                        ThoiGianBatDau = DateTime.Today.AddDays(1).AddHours(8),
                        ThoiGianKetThuc = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(30),
                        PhongHoc = "Lab 402",
                        GiangVien = "ThS. Trần Văn Hùng",
                        NoiDung = "Tổng quan ASP.NET Core 10, Dependency Injection & Middleware",
                        GhiChu = "Yêu cầu cài sẵn .NET 10 SDK & Visual Studio 2026",
                        TrangThai = "Đã lên lịch"
                    });
                    buoiHocs.Add(new BuoiHoc
                    {
                        MaLopHoc = lopNet.MaLopHoc,
                        ThoiGianBatDau = DateTime.Today.AddDays(3).AddHours(8),
                        ThoiGianKetThuc = DateTime.Today.AddDays(3).AddHours(11).AddMinutes(30),
                        PhongHoc = "Lab 402",
                        GiangVien = "ThS. Trần Văn Hùng",
                        NoiDung = "Entity Framework Core 10, Repository & Unit of Work Pattern",
                        GhiChu = "Thực hành kết nối SQL Server",
                        TrangThai = "Đã lên lịch"
                    });
                }

                if (lopPy != null)
                {
                    buoiHocs.Add(new BuoiHoc
                    {
                        MaLopHoc = lopPy.MaLopHoc,
                        ThoiGianBatDau = DateTime.Today.AddDays(2).AddHours(13).AddMinutes(30),
                        ThoiGianKetThuc = DateTime.Today.AddDays(2).AddHours(17),
                        PhongHoc = "Lab 305",
                        GiangVien = "ThS. Nguyễn Văn A",
                        NoiDung = "Lập trình Python cơ bản & cấu trúc dữ liệu",
                        GhiChu = "Chuẩn bị Anaconda / Jupyter Notebook",
                        TrangThai = "Đã lên lịch"
                    });
                }

                if (lopReact != null)
                {
                    buoiHocs.Add(new BuoiHoc
                    {
                        MaLopHoc = lopReact.MaLopHoc,
                        ThoiGianBatDau = DateTime.Today.AddDays(-2).AddHours(18),
                        ThoiGianKetThuc = DateTime.Today.AddDays(-2).AddHours(21),
                        PhongHoc = "Phòng 201",
                        GiangVien = "Cô Hoàng Thị Lan",
                        NoiDung = "React 19 Hooks, JSX & State Management",
                        GhiChu = "Đã hoàn thành tốt",
                        TrangThai = "Đã hoàn thành"
                    });
                }

                if (buoiHocs.Any())
                {
                    context.BuoiHocs.AddRange(buoiHocs);
                    context.SaveChanges();
                }
            }

            // 5. Bổ sung Kết quả học tập mẫu (Module 5) nếu chưa có
            if (!context.KetQuaHocTaps.Any())
            {
                var dks = context.DangKyHocs
                    .Where(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học")
                    .OrderBy(d => d.MaDangKy)
                    .Take(3)
                    .ToList();

                var kqs = new List<KetQuaHocTap>();
                if (dks.Count > 0)
                {
                    kqs.Add(new KetQuaHocTap
                    {
                        MaDangKy = dks[0].MaDangKy,
                        DiemChuyenCan = 9.0,
                        DiemCuoiKhoa = 8.5,
                        DiemTongKet = 8.65,
                        KetQua = "Hoàn thành",
                        NhanXet = "Học viên chuyên cần, nắm vững kiến thức lập trình thực tế",
                        NgayCapNhat = DateTime.Now.AddDays(-5)
                    });
                }
                if (dks.Count > 1)
                {
                    kqs.Add(new KetQuaHocTap
                    {
                        MaDangKy = dks[1].MaDangKy,
                        DiemChuyenCan = 8.0,
                        DiemCuoiKhoa = 7.5,
                        DiemTongKet = 7.65,
                        KetQua = "Hoàn thành",
                        NhanXet = "Làm bài tập đầy đủ, tích cực tham gia thảo luận",
                        NgayCapNhat = DateTime.Now.AddDays(-3)
                    });
                }

                if (kqs.Any())
                {
                    context.KetQuaHocTaps.AddRange(kqs);
                    context.SaveChanges();
                }
            }
        }
    }
}
