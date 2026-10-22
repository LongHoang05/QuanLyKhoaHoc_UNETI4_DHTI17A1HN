using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuongTrinhDaoTao",
                columns: table => new
                {
                    MaChuongTrinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChuongTrinh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EmailLienHe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuongTrinhDaoTao", x => x.MaChuongTrinh);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "LopHoc",
                columns: table => new
                {
                    MaLopHoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLop = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaChuongTrinh = table.Column<int>(type: "int", nullable: false),
                    TenGiangVien = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrinhDoDauVao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SiSoToiDa = table.Column<int>(type: "int", nullable: false),
                    SoBuoiHoc = table.Column<int>(type: "int", nullable: false),
                    HocPhi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayBatDauDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HanDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKhaiGiang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    YeuCauHocVien = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhongHoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LopHoc", x => x.MaLopHoc);
                    table.ForeignKey(
                        name: "FK_LopHoc_ChuongTrinhDaoTao_MaChuongTrinh",
                        column: x => x.MaChuongTrinh,
                        principalTable: "ChuongTrinhDaoTao",
                        principalColumn: "MaChuongTrinh",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HocVien",
                columns: table => new
                {
                    MaHocVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrinhDoHocVan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgheNghiep = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocVien", x => x.MaHocVien);
                    table.ForeignKey(
                        name: "FK_HocVien_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BuoiHoc",
                columns: table => new
                {
                    MaBuoiHoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLopHoc = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhongHoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiangVien = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuoiHoc", x => x.MaBuoiHoc);
                    table.ForeignKey(
                        name: "FK_BuoiHoc_LopHoc_MaLopHoc",
                        column: x => x.MaLopHoc,
                        principalTable: "LopHoc",
                        principalColumn: "MaLopHoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DangKyHoc",
                columns: table => new
                {
                    MaDangKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHocVien = table.Column<int>(type: "int", nullable: false),
                    MaLopHoc = table.Column<int>(type: "int", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NhanXetGiaoVu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayDongHocPhi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoTienDaDong = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyHoc", x => x.MaDangKy);
                    table.ForeignKey(
                        name: "FK_DangKyHoc_HocVien_MaHocVien",
                        column: x => x.MaHocVien,
                        principalTable: "HocVien",
                        principalColumn: "MaHocVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DangKyHoc_LopHoc_MaLopHoc",
                        column: x => x.MaLopHoc,
                        principalTable: "LopHoc",
                        principalColumn: "MaLopHoc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaHocTap",
                columns: table => new
                {
                    MaKetQua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDangKy = table.Column<int>(type: "int", nullable: false),
                    DiemChuyenCan = table.Column<double>(type: "float", nullable: false),
                    DiemCuoiKhoa = table.Column<double>(type: "float", nullable: false),
                    DiemTongKet = table.Column<double>(type: "float", nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NhanXet = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaHocTap", x => x.MaKetQua);
                    table.ForeignKey(
                        name: "FK_KetQuaHocTap_DangKyHoc_MaDangKy",
                        column: x => x.MaDangKy,
                        principalTable: "DangKyHoc",
                        principalColumn: "MaDangKy",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ChuongTrinhDaoTao",
                columns: new[] { "MaChuongTrinh", "EmailLienHe", "MoTa", "NgayTao", "TenChuongTrinh", "TrangThai" },
                values: new object[,]
                {
                    { 1, "cntt@uneti.edu.vn", "Đào tạo kỹ sư phần mềm thực chiến với ASP.NET Core 10, React, SQL Server", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lập trình Web & Phần mềm Doanh nghiệp", true },
                    { 2, "ai@uneti.edu.vn", "Chương trình chuyên sâu Python, Machine Learning, Deep Learning và Power BI", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Khoa học Dữ liệu & Trí tuệ Nhân tạo", true }
                });

            migrationBuilder.InsertData(
                table: "TaiKhoan",
                columns: new[] { "MaTaiKhoan", "Email", "HoTen", "MatKhau", "NgayTao", "TenDangNhap", "TrangThai", "VaiTro" },
                values: new object[,]
                {
                    { 1, "admin@uneti.edu.vn", "Quản trị viên Hệ thống", "admin123", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin", true, "Admin" },
                    { 2, "giaovu@uneti.edu.vn", "Giáo vụ Trung tâm", "giaovu123", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "giaovu", true, "GiaoVu" },
                    { 3, "an.nguyen@gmail.com", "Học viên Nguyễn Văn An", "hocvien123", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "hocvien", true, "HocVien" }
                });

            migrationBuilder.InsertData(
                table: "HocVien",
                columns: new[] { "MaHocVien", "DiaChi", "Email", "GioiTinh", "HinhAnh", "HoTen", "MaTaiKhoan", "NgaySinh", "NgayTao", "NgheNghiep", "SoDienThoai", "TrangThai", "TrinhDoHocVan" },
                values: new object[] { 1, "Hà Nội", "an.nguyen@gmail.com", "Nam", null, "Học viên Nguyễn Văn An", 3, new DateTime(2003, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh viên", "0987654321", true, "Đại học" });

            migrationBuilder.InsertData(
                table: "LopHoc",
                columns: new[] { "MaLopHoc", "HanDangKy", "HocPhi", "MaChuongTrinh", "MoTa", "NgayBatDauDangKy", "NgayKhaiGiang", "NgayTao", "PhongHoc", "SiSoToiDa", "SoBuoiHoc", "TenGiangVien", "TenLop", "TrangThai", "TrinhDoDauVao", "YeuCauHocVien" },
                values: new object[] { 1, new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 3500000m, 1, "Lớp học buổi tối thứ 2-4-6 từ 18:00 - 21:00", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lab 402 - Cơ sở Lĩnh Nam", 25, 16, "ThS. Trần Văn Hùng", "NETCORE10-K01", "Đang mở đăng ký", "Cơ bản", "Đã có kiến thức lập trình C# cơ bản" });

            migrationBuilder.InsertData(
                table: "DangKyHoc",
                columns: new[] { "MaDangKy", "GhiChu", "MaHocVien", "MaLopHoc", "NgayDangKy", "NgayDongHocPhi", "NgayXuLy", "NhanXetGiaoVu", "SoTienDaDong", "TrangThai" },
                values: new object[] { 1, "Đăng ký lớp học NETCORE10-K01", 1, 1, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, 0m, "Chờ duyệt" });

            migrationBuilder.CreateIndex(
                name: "IX_BuoiHoc_MaLopHoc",
                table: "BuoiHoc",
                column: "MaLopHoc");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyHoc_MaHocVien",
                table: "DangKyHoc",
                column: "MaHocVien");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyHoc_MaLopHoc",
                table: "DangKyHoc",
                column: "MaLopHoc");

            migrationBuilder.CreateIndex(
                name: "IX_HocVien_MaTaiKhoan",
                table: "HocVien",
                column: "MaTaiKhoan",
                unique: true,
                filter: "[MaTaiKhoan] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaHocTap_MaDangKy",
                table: "KetQuaHocTap",
                column: "MaDangKy",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LopHoc_MaChuongTrinh",
                table: "LopHoc",
                column: "MaChuongTrinh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuoiHoc");

            migrationBuilder.DropTable(
                name: "KetQuaHocTap");

            migrationBuilder.DropTable(
                name: "DangKyHoc");

            migrationBuilder.DropTable(
                name: "HocVien");

            migrationBuilder.DropTable(
                name: "LopHoc");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "ChuongTrinhDaoTao");
        }
    }
}
