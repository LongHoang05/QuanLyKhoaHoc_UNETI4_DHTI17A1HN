-- =========================================================================
-- ĐẠI HỌC KINH TẾ - KỸ THUẬT CÔNG NGHIỆP (UNETI)
-- KHOA CÔNG NGHỆ THÔNG TIN - LỚP DHTI17A1HN
-- BÀI TẬP LỚN: HỆ THỐNG QUẢN LÝ KHÓA HỌC & ĐĂNG KÝ TRỰC TUYẾN (UNETI EDUSPACE)
-- FULL DATABASE SCHEMA SCRIPT (SQL SERVER)
-- =========================================================================

-- 1. TẠO CƠ SỞ DỮ LIỆU
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyKhoaHoc_UNETI4_DB')
BEGIN
    CREATE DATABASE [QuanLyKhoaHoc_UNETI4_DB];
    PRINT N'[OK] Đã tạo cơ sở dữ liệu QuanLyKhoaHoc_UNETI4_DB thành công.';
END
GO

USE [QuanLyKhoaHoc_UNETI4_DB];
GO

-- =========================================================================
-- 2. TẠO CÁC BẢNG DỮ LIỆU (THEO THỨ TỰ PHỤ THUỘC KHÓA NGOẠI)
-- =========================================================================

-- -------------------------------------------------------------
-- BẢNG 1: TaiKhoan (Quản trị viên, Giáo vụ, Giảng viên, Học viên)
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TaiKhoan')
BEGIN
    CREATE TABLE [dbo].[TaiKhoan] (
        [MaTaiKhoan]    INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [TenDangNhap]   NVARCHAR(100) NOT NULL UNIQUE,
        [MatKhau]       NVARCHAR(500) NOT NULL,
        [HoTen]         NVARCHAR(200) NOT NULL,
        [Email]         NVARCHAR(200) NOT NULL,
        [VaiTro]        NVARCHAR(50)  NOT NULL, -- 'Admin', 'GiaoVu', 'GiangVien', 'HocVien'
        [TrangThai]     BIT           NOT NULL DEFAULT 1,
        [NgayTao]       DATETIME2     NOT NULL DEFAULT GETDATE()
    );
    PRINT N'[OK] Đã tạo bảng TaiKhoan.';
END
GO

-- -------------------------------------------------------------
-- BẢNG 2: ChuongTrinhDaoTao (Chương trình đào tạo tổng quát)
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChuongTrinhDaoTao')
BEGIN
    CREATE TABLE [dbo].[ChuongTrinhDaoTao] (
        [MaChuongTrinh]  INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [TenChuongTrinh] NVARCHAR(200)  NOT NULL,
        [MoTa]           NVARCHAR(1000) NULL,
        [EmailLienHe]    NVARCHAR(200)  NULL,
        [TrangThai]      BIT            NOT NULL DEFAULT 1,
        [NgayTao]        DATETIME2      NOT NULL DEFAULT GETDATE()
    );
    PRINT N'[OK] Đã tạo bảng ChuongTrinhDaoTao.';
END
GO

-- -------------------------------------------------------------
-- BẢNG 3: GiangVien (Hồ sơ giảng viên & chuyên ngành)
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GiangVien')
BEGIN
    CREATE TABLE [dbo].[GiangVien] (
        [MaGiangVien]   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MaTaiKhoan]    INT NULL UNIQUE,
        [HoTen]         NVARCHAR(200) NOT NULL,
        [Email]         NVARCHAR(200) NOT NULL,
        [SoDienThoai]   NVARCHAR(20)  NULL,
        [HocVi]         NVARCHAR(100) NULL DEFAULT N'Thạc sĩ',
        [ChuyenNganh]   NVARCHAR(200) NULL DEFAULT N'Công nghệ thông tin',
        [GioiThieu]     NVARCHAR(500) NULL,
        [HinhAnh]       NVARCHAR(200) NULL,
        [TrangThai]     BIT           NOT NULL DEFAULT 1,
        [NgayTao]       DATETIME2     NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_GiangVien_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) 
            REFERENCES [dbo].[TaiKhoan] ([MaTaiKhoan]) ON DELETE SET NULL
    );
    PRINT N'[OK] Đã tạo bảng GiangVien.';
END
GO

-- -------------------------------------------------------------
-- BẢNG 4: LopHoc (Lớp học cụ thể mở tuyển sinh)
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LopHoc')
BEGIN
    CREATE TABLE [dbo].[LopHoc] (
        [MaLopHoc]         INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [TenLop]           NVARCHAR(200)   NOT NULL,
        [MaChuongTrinh]    INT             NOT NULL,
        [MaGiangVien]      INT             NULL,
        [TenGiangVien]     NVARCHAR(200)   NULL,
        [TrinhDoDauVao]    NVARCHAR(100)   NULL, -- Cơ bản, Trung cấp, Nâng cao
        [SiSoToiDa]        INT             NOT NULL DEFAULT 30,
        [SoBuoiHoc]        INT             NOT NULL DEFAULT 12,
        [HocPhi]           DECIMAL(18,2)   NOT NULL DEFAULT 0,
        [NgayBatDauDangKy] DATETIME2       NOT NULL DEFAULT GETDATE(),
        [HanDangKy]        DATETIME2       NOT NULL,
        [NgayKhaiGiang]    DATETIME2       NOT NULL,
        [MoTa]             NVARCHAR(1000)  NULL,
        [YeuCauHocVien]    NVARCHAR(1000)  NULL,
        [TrangThai]        NVARCHAR(50)    NOT NULL DEFAULT N'Đang mở đăng ký',
        [PhongHoc]         NVARCHAR(100)   NULL,
        [NgayTao]          DATETIME2       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_LopHoc_ChuongTrinhDaoTao_MaChuongTrinh] FOREIGN KEY ([MaChuongTrinh]) 
            REFERENCES [dbo].[ChuongTrinhDaoTao] ([MaChuongTrinh]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LopHoc_GiangVien_MaGiangVien] FOREIGN KEY ([MaGiangVien]) 
            REFERENCES [dbo].[GiangVien] ([MaGiangVien]) ON DELETE SET NULL
    );
    PRINT N'[OK] Đã tạo bảng LopHoc.';
END
GO

-- -------------------------------------------------------------
-- BẢNG 5: HocVien (Hồ sơ học viên UNETI)
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HocVien')
BEGIN
    CREATE TABLE [dbo].[HocVien] (
        [MaHocVien]     INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MaTaiKhoan]    INT NULL UNIQUE,
        [HoTen]         NVARCHAR(200) NOT NULL,
        [Email]         NVARCHAR(200) NOT NULL,
        [SoDienThoai]   NVARCHAR(15)  NULL,
        [NgaySinh]      DATETIME2     NULL,
        [GioiTinh]      NVARCHAR(10)  NULL,
        [DiaChi]        NVARCHAR(500) NULL,
        [TrinhDoHocVan] NVARCHAR(100) NULL,
        [NgheNghiep]    NVARCHAR(200) NULL,
        [HinhAnh]       NVARCHAR(200) NULL,
        [TrangThai]     BIT           NOT NULL DEFAULT 1,
        [NgayTao]       DATETIME2     NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_HocVien_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) 
            REFERENCES [dbo].[TaiKhoan] ([MaTaiKhoan]) ON DELETE SET NULL
    );
    PRINT N'[OK] Đã tạo bảng HocVien.';
END
GO

-- -------------------------------------------------------------
-- BẢNG 6: DangKyHoc (Đơn ghi danh, trạng thái xét duyệt & học phí)
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DangKyHoc')
BEGIN
    CREATE TABLE [dbo].[DangKyHoc] (
        [MaDangKy]       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MaHocVien]      INT            NOT NULL,
        [MaLopHoc]       INT            NOT NULL,
        [NgayDangKy]     DATETIME2      NOT NULL DEFAULT GETDATE(),
        [GhiChu]         NVARCHAR(500)  NULL,
        [TrangThai]      NVARCHAR(50)   NOT NULL DEFAULT N'Chờ duyệt', -- 'Chờ duyệt', 'Đã duyệt', 'Đã đóng học phí', 'Đang học', 'Hoàn thành', 'Đã hủy'
        [NgayXuLy]       DATETIME2      NULL,
        [NhanXetGiaoVu]  NVARCHAR(500)  NULL,
        [NgayDongHocPhi] DATETIME2      NULL,
        [SoTienDaDong]   DECIMAL(18,2)  NOT NULL DEFAULT 0,
        CONSTRAINT [FK_DangKyHoc_HocVien_MaHocVien] FOREIGN KEY ([MaHocVien]) 
            REFERENCES [dbo].[HocVien] ([MaHocVien]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DangKyHoc_LopHoc_MaLopHoc] FOREIGN KEY ([MaLopHoc]) 
            REFERENCES [dbo].[LopHoc] ([MaLopHoc]) ON DELETE NO ACTION
    );
    PRINT N'[OK] Đã tạo bảng DangKyHoc.';
END
GO

-- -------------------------------------------------------------
-- BẢNG 7: BuoiHoc (Thời khóa biểu, phòng học, lịch từng buổi)
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BuoiHoc')
BEGIN
    CREATE TABLE [dbo].[BuoiHoc] (
        [MaBuoiHoc]       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MaLopHoc]        INT            NOT NULL,
        [ThoiGianBatDau]  DATETIME2      NOT NULL,
        [ThoiGianKetThuc] DATETIME2      NOT NULL,
        [PhongHoc]        NVARCHAR(100)  NOT NULL,
        [MaGiangVien]     INT            NULL,
        [GiangVien]       NVARCHAR(200)  NOT NULL,
        [NoiDung]         NVARCHAR(1000) NULL,
        [GhiChu]          NVARCHAR(500)  NULL,
        [TrangThai]       NVARCHAR(50)   NOT NULL DEFAULT N'Đã lên lịch', -- 'Đã lên lịch', 'Đã hoàn thành', 'Đã hủy'
        CONSTRAINT [FK_BuoiHoc_LopHoc_MaLopHoc] FOREIGN KEY ([MaLopHoc]) 
            REFERENCES [dbo].[LopHoc] ([MaLopHoc]) ON DELETE CASCADE,
        CONSTRAINT [FK_BuoiHoc_GiangVien_MaGiangVien] FOREIGN KEY ([MaGiangVien]) 
            REFERENCES [dbo].[GiangVien] ([MaGiangVien]) ON DELETE SET NULL
    );
    PRINT N'[OK] Đã tạo bảng BuoiHoc.';
END
GO

-- -------------------------------------------------------------
-- BẢNG 8: KetQuaHocTap (Bảng điểm GPA, chuyên cần, kết quả cuối khóa)
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'KetQuaHocTap')
BEGIN
    CREATE TABLE [dbo].[KetQuaHocTap] (
        [MaKetQua]      INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MaDangKy]      INT          NOT NULL UNIQUE,
        [DiemChuyenCan] FLOAT        NOT NULL DEFAULT 0,
        [DiemCuoiKhoa]  FLOAT        NOT NULL DEFAULT 0,
        [DiemTongKet]   FLOAT        NOT NULL DEFAULT 0,
        [KetQua]        NVARCHAR(50) NOT NULL DEFAULT N'Chưa xét', -- 'Hoàn thành', 'Không hoàn thành'
        [NhanXet]       NVARCHAR(500) NULL,
        [NgayCapNhat]   DATETIME2    NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_KetQuaHocTap_DangKyHoc_MaDangKy] FOREIGN KEY ([MaDangKy]) 
            REFERENCES [dbo].[DangKyHoc] ([MaDangKy]) ON DELETE CASCADE
    );
    PRINT N'[OK] Đã tạo bảng KetQuaHocTap.';
END
GO

-- =========================================================================
-- 3. TẠO INDEX TỐI ƯU HÓA TRUY VẤN
-- =========================================================================
CREATE NONCLUSTERED INDEX [IX_LopHoc_MaChuongTrinh] ON [dbo].[LopHoc] ([MaChuongTrinh]);
CREATE NONCLUSTERED INDEX [IX_LopHoc_MaGiangVien] ON [dbo].[LopHoc] ([MaGiangVien]);
CREATE NONCLUSTERED INDEX [IX_DangKyHoc_MaHocVien] ON [dbo].[DangKyHoc] ([MaHocVien]);
CREATE NONCLUSTERED INDEX [IX_DangKyHoc_MaLopHoc] ON [dbo].[DangKyHoc] ([MaLopHoc]);
CREATE NONCLUSTERED INDEX [IX_BuoiHoc_MaLopHoc] ON [dbo].[BuoiHoc] ([MaLopHoc]);
CREATE NONCLUSTERED INDEX [IX_BuoiHoc_MaGiangVien] ON [dbo].[BuoiHoc] ([MaGiangVien]);
GO

-- =========================================================================
-- 4. NẠP DỮ LIỆU KHỞI TẠO CHUẨN (MOCK SEED DATA)
-- =========================================================================

-- Nạp Tài khoản
IF NOT EXISTS (SELECT * FROM [dbo].[TaiKhoan] WHERE [TenDangNhap] = 'admin')
BEGIN
    INSERT INTO [dbo].[TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
    VALUES ('admin', 'admin123', N'Quản trị viên Hệ thống', 'admin@uneti.edu.vn', 'Admin', 1, '2026-01-01');
END

IF NOT EXISTS (SELECT * FROM [dbo].[TaiKhoan] WHERE [TenDangNhap] = 'giaovu')
BEGIN
    INSERT INTO [dbo].[TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
    VALUES ('giaovu', 'giaovu123', N'Giáo vụ Trung tâm', 'giaovu@uneti.edu.vn', 'GiaoVu', 1, '2026-01-01');
END

IF NOT EXISTS (SELECT * FROM [dbo].[TaiKhoan] WHERE [TenDangNhap] = 'giangvien')
BEGIN
    INSERT INTO [dbo].[TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
    VALUES ('giangvien', '123456', N'ThS. Trần Văn Hùng', 'hung.tran@uneti.edu.vn', 'GiangVien', 1, '2026-01-01');
END

IF NOT EXISTS (SELECT * FROM [dbo].[TaiKhoan] WHERE [TenDangNhap] = 'an.nguyen')
BEGIN
    INSERT INTO [dbo].[TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
    VALUES ('an.nguyen', 'hocvien123', N'Nguyễn Văn An', 'an.nguyen@gmail.com', 'HocVien', 1, '2026-01-01');
END
GO

-- Nạp Chương trình đào tạo
IF NOT EXISTS (SELECT * FROM [dbo].[ChuongTrinhDaoTao] WHERE [TenChuongTrinh] = N'Lập trình Web & Phần mềm Doanh nghiệp')
BEGIN
    INSERT INTO [dbo].[ChuongTrinhDaoTao] ([TenChuongTrinh], [MoTa], [EmailLienHe], [TrangThai], [NgayTao])
    VALUES (N'Lập trình Web & Phần mềm Doanh nghiệp', N'Đào tạo kỹ sư phần mềm thực chiến với ASP.NET Core 10, React, SQL Server', 'cntt@uneti.edu.vn', 1, '2026-01-01');
END

IF NOT EXISTS (SELECT * FROM [dbo].[ChuongTrinhDaoTao] WHERE [TenChuongTrinh] = N'Khoa học Dữ liệu & Trí tuệ Nhân tạo')
BEGIN
    INSERT INTO [dbo].[ChuongTrinhDaoTao] ([TenChuongTrinh], [MoTa], [EmailLienHe], [TrangThai], [NgayTao])
    VALUES (N'Khoa học Dữ liệu & Trí tuệ Nhân tạo', N'Chương trình chuyên sâu Python, Machine Learning, Deep Learning và Power BI', 'ai@uneti.edu.vn', 1, '2026-01-01');
END
GO

-- Nạp Giảng viên
DECLARE @MaTkGv INT = (SELECT TOP 1 [MaTaiKhoan] FROM [dbo].[TaiKhoan] WHERE [TenDangNhap] = 'giangvien');
IF NOT EXISTS (SELECT * FROM [dbo].[GiangVien] WHERE [Email] = 'hung.tran@uneti.edu.vn')
BEGIN
    INSERT INTO [dbo].[GiangVien] ([MaTaiKhoan], [HoTen], [Email], [SoDienThoai], [HocVi], [ChuyenNganh], [GioiThieu], [TrangThai], [NgayTao])
    VALUES (@MaTkGv, N'ThS. Trần Văn Hùng', 'hung.tran@uneti.edu.vn', '0912345678', N'Thạc sĩ', N'Khoa học máy tính & Công nghệ Web', N'Giảng viên khoa CNTT - Đại học UNETI. Hơn 10 năm kinh nghiệm giảng dạy ASP.NET Core và Kiến trúc phần mềm đám mây.', 1, '2026-01-01');
END
GO

-- Nạp Học viên
DECLARE @MaTkHv INT = (SELECT TOP 1 [MaTaiKhoan] FROM [dbo].[TaiKhoan] WHERE [TenDangNhap] = 'an.nguyen');
IF NOT EXISTS (SELECT * FROM [dbo].[HocVien] WHERE [Email] = 'an.nguyen@gmail.com')
BEGIN
    INSERT INTO [dbo].[HocVien] ([MaTaiKhoan], [HoTen], [Email], [SoDienThoai], [NgaySinh], [GioiTinh], [DiaChi], [TrinhDoHocVan], [NgheNghiep], [HinhAnh], [TrangThai], [NgayTao])
    VALUES (@MaTkHv, N'Nguyễn Văn An', 'an.nguyen@gmail.com', '0987654321', '2003-05-20', N'Nam', N'Hà Nội', N'Đại học', N'Sinh viên UNETI', NULL, 1, '2026-01-01');
END
GO

-- Nạp Lớp học
DECLARE @MaCT1 INT = (SELECT TOP 1 [MaChuongTrinh] FROM [dbo].[ChuongTrinhDaoTao] WHERE [TenChuongTrinh] LIKE N'%Lập trình Web%');
DECLARE @MaGV1 INT = (SELECT TOP 1 [MaGiangVien] FROM [dbo].[GiangVien] WHERE [Email] = 'hung.tran@uneti.edu.vn');

IF NOT EXISTS (SELECT * FROM [dbo].[LopHoc] WHERE [TenLop] = 'NETCORE10-K01')
BEGIN
    INSERT INTO [dbo].[LopHoc] (
        [TenLop], [MaChuongTrinh], [MaGiangVien], [TenGiangVien], [TrinhDoDauVao], 
        [SiSoToiDa], [SoBuoiHoc], [HocPhi], [NgayBatDauDangKy], [HanDangKy], 
        [NgayKhaiGiang], [MoTa], [YeuCauHocVien], [TrangThai], [PhongHoc], [NgayTao]
    )
    VALUES (
        'NETCORE10-K01', @MaCT1, @MaGV1, N'ThS. Trần Văn Hùng', N'Cơ bản', 
        30, 15, 3500000, '2026-01-01', '2026-12-31', 
        '2027-01-15', N'Lớp học buổi tối thứ 2-4-6 từ 18:00 - 21:00', N'Đã có kiến thức lập trình C# cơ bản', 
        N'Đang mở đăng ký', N'Lab 402 - Cơ sở Lĩnh Nam', '2026-01-01'
    );
END
GO

PRINT N'========================================================================';
PRINT N'[HOÀN TẤT] SCHEMA CƠ SỞ DỮ LIỆU ĐÃ ĐƯỢC THIẾT LẬP HOÀN CHỈNH & CHUẨN 100%!';
PRINT N'========================================================================';
GO
