using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuongTrinhDaoTaos",
                columns: table => new
                {
                    MaChuongTrinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChuongTrinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailLienHe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuongTrinhDaoTaos", x => x.MaChuongTrinh);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "LopHocs",
                columns: table => new
                {
                    MaLop = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLop = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaChuongTrinh = table.Column<int>(type: "int", nullable: false),
                    GiangVien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrinhDoDauVao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SiSoToiDa = table.Column<int>(type: "int", nullable: false),
                    SoBuoiHoc = table.Column<int>(type: "int", nullable: false),
                    HocPhi = table.Column<double>(type: "float", nullable: false),
                    NgayBatDauDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKhaiGiang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YeuCauHocVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LopHocs", x => x.MaLop);
                    table.ForeignKey(
                        name: "FK_LopHocs_ChuongTrinhDaoTaos_MaChuongTrinh",
                        column: x => x.MaChuongTrinh,
                        principalTable: "ChuongTrinhDaoTaos",
                        principalColumn: "MaChuongTrinh",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HocViens",
                columns: table => new
                {
                    MaHocVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrinhDoHocVan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgheNghiep = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocViens", x => x.MaHocVien);
                    table.ForeignKey(
                        name: "FK_HocViens_TaiKhoans_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BuoiHocs",
                columns: table => new
                {
                    MaBuoiHoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhongHoc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiangVien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuoiHocs", x => x.MaBuoiHoc);
                    table.ForeignKey(
                        name: "FK_BuoiHocs_LopHocs_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHocs",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DangKyHocs",
                columns: table => new
                {
                    MaDangKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHocVien = table.Column<int>(type: "int", nullable: false),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NhanXetGiaoVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayDongHocPhi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoTienDaDong = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyHocs", x => x.MaDangKy);
                    table.ForeignKey(
                        name: "FK_DangKyHocs_HocViens_MaHocVien",
                        column: x => x.MaHocVien,
                        principalTable: "HocViens",
                        principalColumn: "MaHocVien",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DangKyHocs_LopHocs_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHocs",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaHocTaps",
                columns: table => new
                {
                    MaKetQua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDangKy = table.Column<int>(type: "int", nullable: false),
                    DiemChuyenCan = table.Column<double>(type: "float", nullable: false),
                    DiemCuoiKhoa = table.Column<double>(type: "float", nullable: false),
                    DiemTongKet = table.Column<double>(type: "float", nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NhanXet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaHocTaps", x => x.MaKetQua);
                    table.ForeignKey(
                        name: "FK_KetQuaHocTaps_DangKyHocs_MaDangKy",
                        column: x => x.MaDangKy,
                        principalTable: "DangKyHocs",
                        principalColumn: "MaDangKy",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ChuongTrinhDaoTaos",
                columns: new[] { "MaChuongTrinh", "EmailLienHe", "MoTa", "TenChuongTrinh", "TrangThai" },
                values: new object[,]
                {
                    { 1, "contact@uneti.edu.vn", "Chương trình Lập trình Web ASP.NET Core", "Lập trình Web ASP.NET Core", "Đang hoạt động" },
                    { 2, "contact@uneti.edu.vn", "Chương trình Lập trình ReactJS", "Lập trình ReactJS", "Đang hoạt động" },
                    { 3, "contact@uneti.edu.vn", "Chương trình Khoa học dữ liệu với Python", "Khoa học dữ liệu với Python", "Đang hoạt động" },
                    { 4, "contact@uneti.edu.vn", "Chương trình Tiếng Anh Giao Tiếp", "Tiếng Anh Giao Tiếp", "Đang hoạt động" },
                    { 5, "contact@uneti.edu.vn", "Chương trình Lập trình Mobile Flutter", "Lập trình Mobile Flutter", "Đang hoạt động" }
                });

            migrationBuilder.InsertData(
                table: "TaiKhoans",
                columns: new[] { "MaTaiKhoan", "Email", "HoTen", "MatKhau", "TenDangNhap", "TrangThai", "VaiTro" },
                values: new object[,]
                {
                    { 1, "admin1@uneti.edu.vn", "Admin 1", "123456", "admin1", "Đang hoạt động", "Admin" },
                    { 2, "admin2@uneti.edu.vn", "Admin 2", "123456", "admin2", "Đang hoạt động", "Admin" },
                    { 3, "giaovu1@uneti.edu.vn", "Giáo vụ 1", "123456", "giaovu1", "Đang hoạt động", "Giáo vụ" },
                    { 4, "giaovu2@uneti.edu.vn", "Giáo vụ 2", "123456", "giaovu2", "Đang hoạt động", "Giáo vụ" },
                    { 5, "giaovu3@uneti.edu.vn", "Giáo vụ 3", "123456", "giaovu3", "Đang hoạt động", "Giáo vụ" },
                    { 6, "hocvien1@gmail.com", "Học viên 1", "123456", "hocvien1", "Đang hoạt động", "Học viên" },
                    { 7, "hocvien2@gmail.com", "Học viên 2", "123456", "hocvien2", "Đang hoạt động", "Học viên" },
                    { 8, "hocvien3@gmail.com", "Học viên 3", "123456", "hocvien3", "Đang hoạt động", "Học viên" },
                    { 9, "hocvien4@gmail.com", "Học viên 4", "123456", "hocvien4", "Đang hoạt động", "Học viên" },
                    { 10, "hocvien5@gmail.com", "Học viên 5", "123456", "hocvien5", "Đang hoạt động", "Học viên" },
                    { 11, "hocvien6@gmail.com", "Học viên 6", "123456", "hocvien6", "Đang hoạt động", "Học viên" },
                    { 12, "hocvien7@gmail.com", "Học viên 7", "123456", "hocvien7", "Đang hoạt động", "Học viên" },
                    { 13, "hocvien8@gmail.com", "Học viên 8", "123456", "hocvien8", "Đang hoạt động", "Học viên" },
                    { 14, "hocvien9@gmail.com", "Học viên 9", "123456", "hocvien9", "Đang hoạt động", "Học viên" },
                    { 15, "hocvien10@gmail.com", "Học viên 10", "123456", "hocvien10", "Đang hoạt động", "Học viên" },
                    { 16, "hocvien11@gmail.com", "Học viên 11", "123456", "hocvien11", "Đang hoạt động", "Học viên" },
                    { 17, "hocvien12@gmail.com", "Học viên 12", "123456", "hocvien12", "Đang hoạt động", "Học viên" },
                    { 18, "hocvien13@gmail.com", "Học viên 13", "123456", "hocvien13", "Đang hoạt động", "Học viên" },
                    { 19, "hocvien14@gmail.com", "Học viên 14", "123456", "hocvien14", "Đang hoạt động", "Học viên" },
                    { 20, "hocvien15@gmail.com", "Học viên 15", "123456", "hocvien15", "Đang hoạt động", "Học viên" },
                    { 21, "hocvien16@gmail.com", "Học viên 16", "123456", "hocvien16", "Đang hoạt động", "Học viên" },
                    { 22, "hocvien17@gmail.com", "Học viên 17", "123456", "hocvien17", "Đang hoạt động", "Học viên" },
                    { 23, "hocvien18@gmail.com", "Học viên 18", "123456", "hocvien18", "Đang hoạt động", "Học viên" },
                    { 24, "hocvien19@gmail.com", "Học viên 19", "123456", "hocvien19", "Đang hoạt động", "Học viên" },
                    { 25, "hocvien20@gmail.com", "Học viên 20", "123456", "hocvien20", "Đang hoạt động", "Học viên" },
                    { 26, "hocvien21@gmail.com", "Học viên 21", "123456", "hocvien21", "Đang hoạt động", "Học viên" },
                    { 27, "hocvien22@gmail.com", "Học viên 22", "123456", "hocvien22", "Đang hoạt động", "Học viên" },
                    { 28, "hocvien23@gmail.com", "Học viên 23", "123456", "hocvien23", "Đang hoạt động", "Học viên" },
                    { 29, "hocvien24@gmail.com", "Học viên 24", "123456", "hocvien24", "Đang hoạt động", "Học viên" },
                    { 30, "hocvien25@gmail.com", "Học viên 25", "123456", "hocvien25", "Đang hoạt động", "Học viên" },
                    { 31, "hocvien26@gmail.com", "Học viên 26", "123456", "hocvien26", "Đang hoạt động", "Học viên" },
                    { 32, "hocvien27@gmail.com", "Học viên 27", "123456", "hocvien27", "Đang hoạt động", "Học viên" },
                    { 33, "hocvien28@gmail.com", "Học viên 28", "123456", "hocvien28", "Đang hoạt động", "Học viên" },
                    { 34, "hocvien29@gmail.com", "Học viên 29", "123456", "hocvien29", "Đang hoạt động", "Học viên" },
                    { 35, "hocvien30@gmail.com", "Học viên 30", "123456", "hocvien30", "Đang hoạt động", "Học viên" }
                });

            migrationBuilder.InsertData(
                table: "HocViens",
                columns: new[] { "MaHocVien", "DiaChi", "Email", "GioiTinh", "HoTen", "MaTaiKhoan", "NgaySinh", "NgheNghiep", "SoDienThoai", "TrangThai", "TrinhDoHocVan" },
                values: new object[,]
                {
                    { 1, "Hà Nội", "hocvien1@gmail.com", "Nữ", "Học viên 1", 6, new DateTime(2000, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654001", "Đang học", "Đại học" },
                    { 2, "Hà Nội", "hocvien2@gmail.com", "Nam", "Học viên 2", 7, new DateTime(2000, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654002", "Đang học", "Đại học" },
                    { 3, "Hà Nội", "hocvien3@gmail.com", "Nữ", "Học viên 3", 8, new DateTime(2000, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654003", "Đang học", "Đại học" },
                    { 4, "Hà Nội", "hocvien4@gmail.com", "Nam", "Học viên 4", 9, new DateTime(2000, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654004", "Đang học", "Đại học" },
                    { 5, "Hà Nội", "hocvien5@gmail.com", "Nữ", "Học viên 5", 10, new DateTime(2000, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654005", "Đang học", "Đại học" },
                    { 6, "Hà Nội", "hocvien6@gmail.com", "Nam", "Học viên 6", 11, new DateTime(2000, 6, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654006", "Đang học", "Đại học" },
                    { 7, "Hà Nội", "hocvien7@gmail.com", "Nữ", "Học viên 7", 12, new DateTime(2000, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654007", "Đang học", "Đại học" },
                    { 8, "Hà Nội", "hocvien8@gmail.com", "Nam", "Học viên 8", 13, new DateTime(2000, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654008", "Đang học", "Đại học" },
                    { 9, "Hà Nội", "hocvien9@gmail.com", "Nữ", "Học viên 9", 14, new DateTime(2000, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654009", "Đang học", "Đại học" },
                    { 10, "Hà Nội", "hocvien10@gmail.com", "Nam", "Học viên 10", 15, new DateTime(2000, 10, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654010", "Đang học", "Đại học" },
                    { 11, "Hà Nội", "hocvien11@gmail.com", "Nữ", "Học viên 11", 16, new DateTime(2000, 11, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654011", "Đang học", "Đại học" },
                    { 12, "Hà Nội", "hocvien12@gmail.com", "Nam", "Học viên 12", 17, new DateTime(2000, 12, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654012", "Đang học", "Đại học" },
                    { 13, "Hà Nội", "hocvien13@gmail.com", "Nữ", "Học viên 13", 18, new DateTime(2001, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654013", "Đang học", "Đại học" },
                    { 14, "Hà Nội", "hocvien14@gmail.com", "Nam", "Học viên 14", 19, new DateTime(2001, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654014", "Đang học", "Đại học" },
                    { 15, "Hà Nội", "hocvien15@gmail.com", "Nữ", "Học viên 15", 20, new DateTime(2001, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654015", "Đang học", "Đại học" },
                    { 16, "Hà Nội", "hocvien16@gmail.com", "Nam", "Học viên 16", 21, new DateTime(2001, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654016", "Đang học", "Đại học" },
                    { 17, "Hà Nội", "hocvien17@gmail.com", "Nữ", "Học viên 17", 22, new DateTime(2001, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654017", "Đang học", "Đại học" },
                    { 18, "Hà Nội", "hocvien18@gmail.com", "Nam", "Học viên 18", 23, new DateTime(2001, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654018", "Đang học", "Đại học" },
                    { 19, "Hà Nội", "hocvien19@gmail.com", "Nữ", "Học viên 19", 24, new DateTime(2001, 7, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654019", "Đang học", "Đại học" },
                    { 20, "Hà Nội", "hocvien20@gmail.com", "Nam", "Học viên 20", 25, new DateTime(2001, 8, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654020", "Đang học", "Đại học" },
                    { 21, "Hà Nội", "hocvien21@gmail.com", "Nữ", "Học viên 21", 26, new DateTime(2001, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654021", "Đang học", "Đại học" },
                    { 22, "Hà Nội", "hocvien22@gmail.com", "Nam", "Học viên 22", 27, new DateTime(2001, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654022", "Đang học", "Đại học" },
                    { 23, "Hà Nội", "hocvien23@gmail.com", "Nữ", "Học viên 23", 28, new DateTime(2001, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654023", "Đang học", "Đại học" },
                    { 24, "Hà Nội", "hocvien24@gmail.com", "Nam", "Học viên 24", 29, new DateTime(2001, 12, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654024", "Đang học", "Đại học" },
                    { 25, "Hà Nội", "hocvien25@gmail.com", "Nữ", "Học viên 25", 30, new DateTime(2002, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654025", "Đang học", "Đại học" },
                    { 26, "Hà Nội", "hocvien26@gmail.com", "Nam", "Học viên 26", 31, new DateTime(2002, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654026", "Đang học", "Đại học" },
                    { 27, "Hà Nội", "hocvien27@gmail.com", "Nữ", "Học viên 27", 32, new DateTime(2002, 3, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654027", "Đang học", "Đại học" },
                    { 28, "Hà Nội", "hocvien28@gmail.com", "Nam", "Học viên 28", 33, new DateTime(2002, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654028", "Đang học", "Đại học" },
                    { 29, "Hà Nội", "hocvien29@gmail.com", "Nữ", "Học viên 29", 34, new DateTime(2002, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654029", "Đang học", "Đại học" },
                    { 30, "Hà Nội", "hocvien30@gmail.com", "Nam", "Học viên 30", 35, new DateTime(2002, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654030", "Đang học", "Đại học" }
                });

            migrationBuilder.InsertData(
                table: "LopHocs",
                columns: new[] { "MaLop", "GiangVien", "HanDangKy", "HocPhi", "MaChuongTrinh", "MoTa", "NgayBatDauDangKy", "NgayKhaiGiang", "SiSoToiDa", "SoBuoiHoc", "TenLop", "TrangThai", "TrinhDoDauVao", "YeuCauHocVien" },
                values: new object[,]
                {
                    { 1, "Giảng viên 2", new DateTime(2023, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 1, "Mô tả lớp học", new DateTime(2023, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Lập trình Web ASP.NET Core Khóa 1", "Đang mở đăng ký", "Cơ bản", "Không" },
                    { 2, "Giảng viên 3", new DateTime(2023, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 2, "Mô tả lớp học", new DateTime(2023, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Lập trình ReactJS Khóa 2", "Đã đóng đăng ký", "Cơ bản", "Không" },
                    { 3, "Giảng viên 1", new DateTime(2023, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 3, "Mô tả lớp học", new DateTime(2023, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Khoa học dữ liệu với Python Khóa 3", "Đang học", "Cơ bản", "Không" },
                    { 4, "Giảng viên 2", new DateTime(2023, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 4, "Mô tả lớp học", new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Tiếng Anh Giao Tiếp Khóa 4", "Đã kết thúc", "Cơ bản", "Không" },
                    { 5, "Giảng viên 3", new DateTime(2023, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 5, "Mô tả lớp học", new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Lập trình Mobile Flutter Khóa 5", "Hết hạn đăng ký", "Cơ bản", "Không" },
                    { 6, "Giảng viên 1", new DateTime(2023, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 1, "Mô tả lớp học", new DateTime(2023, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Lập trình Web ASP.NET Core Khóa 6", "Đang mở đăng ký", "Cơ bản", "Không" },
                    { 7, "Giảng viên 2", new DateTime(2023, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 2, "Mô tả lớp học", new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Lập trình ReactJS Khóa 7", "Đã đóng đăng ký", "Cơ bản", "Không" },
                    { 8, "Giảng viên 3", new DateTime(2023, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 3, "Mô tả lớp học", new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Khoa học dữ liệu với Python Khóa 8", "Đang học", "Cơ bản", "Không" },
                    { 9, "Giảng viên 1", new DateTime(2023, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 4, "Mô tả lớp học", new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Tiếng Anh Giao Tiếp Khóa 9", "Đã kết thúc", "Cơ bản", "Không" },
                    { 10, "Giảng viên 2", new DateTime(2023, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 5, "Mô tả lớp học", new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Lập trình Mobile Flutter Khóa 10", "Hết hạn đăng ký", "Cơ bản", "Không" },
                    { 11, "Giảng viên 3", new DateTime(2023, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 1, "Mô tả lớp học", new DateTime(2023, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Lập trình Web ASP.NET Core Khóa 11", "Đang mở đăng ký", "Cơ bản", "Không" },
                    { 12, "Giảng viên 1", new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 2, "Mô tả lớp học", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Lập trình ReactJS Khóa 12", "Đã đóng đăng ký", "Cơ bản", "Không" },
                    { 13, "Giảng viên 2", new DateTime(2023, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 3, "Mô tả lớp học", new DateTime(2023, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Khoa học dữ liệu với Python Khóa 13", "Đang học", "Cơ bản", "Không" },
                    { 14, "Giảng viên 3", new DateTime(2023, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 4, "Mô tả lớp học", new DateTime(2023, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Tiếng Anh Giao Tiếp Khóa 14", "Đã kết thúc", "Cơ bản", "Không" },
                    { 15, "Giảng viên 1", new DateTime(2023, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, 5, "Mô tả lớp học", new DateTime(2023, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 15, "Lớp Lập trình Mobile Flutter Khóa 15", "Hết hạn đăng ký", "Cơ bản", "Không" }
                });

            migrationBuilder.InsertData(
                table: "BuoiHocs",
                columns: new[] { "MaBuoiHoc", "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[,]
                {
                    { 1, "", "Giảng viên 2", 1, "Buổi học 1", "Phòng 101", new DateTime(2023, 2, 2, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 2, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 2, "", "Giảng viên 3", 2, "Buổi học 2", "Phòng 102", new DateTime(2023, 2, 3, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 3, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 3, "", "Giảng viên 1", 3, "Buổi học 3", "Phòng 103", new DateTime(2023, 2, 4, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 4, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 4, "", "Giảng viên 2", 4, "Buổi học 4", "Phòng 104", new DateTime(2023, 2, 5, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 5, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 5, "", "Giảng viên 3", 5, "Buổi học 5", "Phòng 100", new DateTime(2023, 2, 6, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 6, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 6, "", "Giảng viên 1", 6, "Buổi học 6", "Phòng 101", new DateTime(2023, 2, 7, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 7, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 7, "", "Giảng viên 2", 7, "Buổi học 7", "Phòng 102", new DateTime(2023, 2, 8, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 8, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 8, "", "Giảng viên 3", 8, "Buổi học 8", "Phòng 103", new DateTime(2023, 2, 9, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 9, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 9, "", "Giảng viên 1", 9, "Buổi học 9", "Phòng 104", new DateTime(2023, 2, 10, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 10, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 10, "", "Giảng viên 2", 10, "Buổi học 10", "Phòng 100", new DateTime(2023, 2, 11, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 11, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 11, "", "Giảng viên 3", 11, "Buổi học 11", "Phòng 101", new DateTime(2023, 2, 12, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 12, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 12, "", "Giảng viên 1", 12, "Buổi học 12", "Phòng 102", new DateTime(2023, 2, 13, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 13, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 13, "", "Giảng viên 2", 13, "Buổi học 13", "Phòng 103", new DateTime(2023, 2, 14, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 14, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 14, "", "Giảng viên 3", 14, "Buổi học 14", "Phòng 104", new DateTime(2023, 2, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 15, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 15, "", "Giảng viên 1", 15, "Buổi học 15", "Phòng 100", new DateTime(2023, 2, 16, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 16, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 16, "", "Giảng viên 2", 1, "Buổi học 16", "Phòng 101", new DateTime(2023, 2, 17, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 17, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 17, "", "Giảng viên 3", 2, "Buổi học 17", "Phòng 102", new DateTime(2023, 2, 18, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 18, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 18, "", "Giảng viên 1", 3, "Buổi học 18", "Phòng 103", new DateTime(2023, 2, 19, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 19, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 19, "", "Giảng viên 2", 4, "Buổi học 19", "Phòng 104", new DateTime(2023, 2, 20, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 20, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 20, "", "Giảng viên 3", 5, "Buổi học 20", "Phòng 100", new DateTime(2023, 2, 21, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 21, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 21, "", "Giảng viên 1", 6, "Buổi học 21", "Phòng 101", new DateTime(2023, 2, 22, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 22, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 22, "", "Giảng viên 2", 7, "Buổi học 22", "Phòng 102", new DateTime(2023, 2, 23, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 23, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 23, "", "Giảng viên 3", 8, "Buổi học 23", "Phòng 103", new DateTime(2023, 2, 24, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 24, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 24, "", "Giảng viên 1", 9, "Buổi học 24", "Phòng 104", new DateTime(2023, 2, 25, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 25, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 25, "", "Giảng viên 2", 10, "Buổi học 25", "Phòng 100", new DateTime(2023, 2, 26, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 26, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 26, "", "Giảng viên 3", 11, "Buổi học 26", "Phòng 101", new DateTime(2023, 2, 27, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 27, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 27, "", "Giảng viên 1", 12, "Buổi học 27", "Phòng 102", new DateTime(2023, 2, 28, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 28, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 28, "", "Giảng viên 2", 13, "Buổi học 28", "Phòng 103", new DateTime(2023, 3, 1, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 1, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 29, "", "Giảng viên 3", 14, "Buổi học 29", "Phòng 104", new DateTime(2023, 3, 2, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 2, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 30, "", "Giảng viên 1", 15, "Buổi học 30", "Phòng 100", new DateTime(2023, 3, 3, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 3, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" }
                });

            migrationBuilder.InsertData(
                table: "DangKyHocs",
                columns: new[] { "MaDangKy", "GhiChu", "MaHocVien", "MaLop", "NgayDangKy", "NgayDongHocPhi", "NgayXuLy", "NhanXetGiaoVu", "SoTienDaDong", "TrangThai" },
                values: new object[,]
                {
                    { 1, "", 1, 1, new DateTime(2023, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Chờ duyệt" },
                    { 2, "", 2, 2, new DateTime(2023, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã duyệt" },
                    { 3, "", 3, 3, new DateTime(2023, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đã đóng học phí" },
                    { 4, "", 4, 4, new DateTime(2023, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đang học" },
                    { 5, "", 5, 5, new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã hủy" },
                    { 6, "", 6, 6, new DateTime(2023, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Chờ duyệt" },
                    { 7, "", 7, 7, new DateTime(2023, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 1, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã duyệt" },
                    { 8, "", 8, 8, new DateTime(2023, 1, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đã đóng học phí" },
                    { 9, "", 9, 9, new DateTime(2023, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đang học" },
                    { 10, "", 10, 10, new DateTime(2023, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã hủy" },
                    { 11, "", 11, 11, new DateTime(2023, 1, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Chờ duyệt" },
                    { 12, "", 12, 12, new DateTime(2023, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã duyệt" },
                    { 13, "", 13, 13, new DateTime(2023, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đã đóng học phí" },
                    { 14, "", 14, 14, new DateTime(2023, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đang học" },
                    { 15, "", 15, 15, new DateTime(2023, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã hủy" },
                    { 16, "", 16, 1, new DateTime(2023, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Chờ duyệt" },
                    { 17, "", 17, 2, new DateTime(2023, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã duyệt" },
                    { 18, "", 18, 3, new DateTime(2023, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đã đóng học phí" },
                    { 19, "", 19, 4, new DateTime(2023, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đang học" },
                    { 20, "", 20, 5, new DateTime(2023, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã hủy" },
                    { 21, "", 21, 6, new DateTime(2023, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Chờ duyệt" },
                    { 22, "", 22, 7, new DateTime(2023, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã duyệt" },
                    { 23, "", 23, 8, new DateTime(2023, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đã đóng học phí" },
                    { 24, "", 24, 9, new DateTime(2023, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đang học" },
                    { 25, "", 25, 10, new DateTime(2023, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã hủy" },
                    { 26, "", 26, 11, new DateTime(2023, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Chờ duyệt" },
                    { 27, "", 27, 12, new DateTime(2023, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã duyệt" },
                    { 28, "", 28, 13, new DateTime(2023, 2, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đã đóng học phí" },
                    { 29, "", 29, 14, new DateTime(2023, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đang học" },
                    { 30, "", 30, 15, new DateTime(2023, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã hủy" },
                    { 31, "", 1, 1, new DateTime(2023, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Chờ duyệt" },
                    { 32, "", 2, 2, new DateTime(2023, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã duyệt" },
                    { 33, "", 3, 3, new DateTime(2023, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đã đóng học phí" },
                    { 34, "", 4, 4, new DateTime(2023, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đang học" },
                    { 35, "", 5, 5, new DateTime(2023, 2, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã hủy" },
                    { 36, "", 6, 6, new DateTime(2023, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Chờ duyệt" },
                    { 37, "", 7, 7, new DateTime(2023, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã duyệt" },
                    { 38, "", 8, 8, new DateTime(2023, 2, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đã đóng học phí" },
                    { 39, "", 9, 9, new DateTime(2023, 2, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đang học" },
                    { 40, "", 10, 10, new DateTime(2023, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã hủy" },
                    { 41, "", 11, 11, new DateTime(2023, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Chờ duyệt" },
                    { 42, "", 12, 12, new DateTime(2023, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã duyệt" },
                    { 43, "", 13, 13, new DateTime(2023, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đã đóng học phí" },
                    { 44, "", 14, 14, new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 5000000.0, "Đang học" },
                    { 45, "", 15, 15, new DateTime(2023, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2023, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đã kiểm tra", 0.0, "Đã hủy" }
                });

            migrationBuilder.InsertData(
                table: "KetQuaHocTaps",
                columns: new[] { "MaKetQua", "DiemChuyenCan", "DiemCuoiKhoa", "DiemTongKet", "KetQua", "MaDangKy", "NgayCapNhat", "NhanXet" },
                values: new object[,]
                {
                    { 1, 8.0, 10.0, 9.4000000000000004, "Hoàn thành", 3, new DateTime(2023, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 2, 9.0, 7.0, 7.5999999999999996, "Hoàn thành", 4, new DateTime(2023, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 3, 10.0, 7.0, 7.8999999999999995, "Hoàn thành", 8, new DateTime(2023, 5, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 4, 8.0, 8.0, 8.0, "Hoàn thành", 9, new DateTime(2023, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 5, 9.0, 8.0, 8.2999999999999989, "Hoàn thành", 13, new DateTime(2023, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 6, 10.0, 9.0, 9.3000000000000007, "Hoàn thành", 14, new DateTime(2023, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 7, 8.0, 9.0, 8.6999999999999993, "Hoàn thành", 18, new DateTime(2023, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 8, 9.0, 10.0, 9.6999999999999993, "Hoàn thành", 19, new DateTime(2023, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 9, 10.0, 10.0, 10.0, "Hoàn thành", 23, new DateTime(2023, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 10, 8.0, 7.0, 7.2999999999999989, "Hoàn thành", 24, new DateTime(2023, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 11, 9.0, 7.0, 7.5999999999999996, "Hoàn thành", 28, new DateTime(2023, 5, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 12, 10.0, 8.0, 8.5999999999999996, "Hoàn thành", 29, new DateTime(2023, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 13, 8.0, 8.0, 8.0, "Hoàn thành", 33, new DateTime(2023, 6, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 14, 9.0, 9.0, 9.0, "Hoàn thành", 34, new DateTime(2023, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 15, 10.0, 9.0, 9.3000000000000007, "Hoàn thành", 38, new DateTime(2023, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 16, 8.0, 10.0, 9.4000000000000004, "Hoàn thành", 39, new DateTime(2023, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 17, 9.0, 10.0, 9.6999999999999993, "Hoàn thành", 43, new DateTime(2023, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" },
                    { 18, 10.0, 7.0, 7.8999999999999995, "Hoàn thành", 44, new DateTime(2023, 6, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tốt" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuoiHocs_MaLop",
                table: "BuoiHocs",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyHocs_MaHocVien",
                table: "DangKyHocs",
                column: "MaHocVien");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyHocs_MaLop",
                table: "DangKyHocs",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_HocViens_MaTaiKhoan",
                table: "HocViens",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaHocTaps_MaDangKy",
                table: "KetQuaHocTaps",
                column: "MaDangKy");

            migrationBuilder.CreateIndex(
                name: "IX_LopHocs_MaChuongTrinh",
                table: "LopHocs",
                column: "MaChuongTrinh");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_TenDangNhap",
                table: "TaiKhoans",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuoiHocs");

            migrationBuilder.DropTable(
                name: "KetQuaHocTaps");

            migrationBuilder.DropTable(
                name: "DangKyHocs");

            migrationBuilder.DropTable(
                name: "HocViens");

            migrationBuilder.DropTable(
                name: "LopHocs");

            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "ChuongTrinhDaoTaos");
        }
    }
}
