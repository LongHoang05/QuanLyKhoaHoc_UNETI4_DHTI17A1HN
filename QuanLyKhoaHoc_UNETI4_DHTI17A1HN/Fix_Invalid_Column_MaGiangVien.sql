-- =========================================================================
-- SQL SCRIPT SỬA LỖI: SqlException: Invalid column name 'MaGiangVien'
-- Dự án: Quản lý Khóa học UNETI - Nhóm 4 DHTI17A1HN
-- Chức năng: Tự động bổ sung bảng GiangVien và cột MaGiangVien vào LopHoc, BuoiHoc
-- =========================================================================

USE [QuanLyKhoaHoc_UNETI4_DB];
GO

-- 1. TẠO BẢNG GiangVien NẾU CHƯA TỒN TẠI
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GiangVien')
BEGIN
    CREATE TABLE [dbo].[GiangVien] (
        [MaGiangVien] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [MaTaiKhoan] INT NULL,
        [HoTen] NVARCHAR(200) NOT NULL,
        [Email] NVARCHAR(200) NOT NULL,
        [SoDienThoai] NVARCHAR(20) NULL,
        [HocVi] NVARCHAR(100) NULL,
        [ChuyenNganh] NVARCHAR(200) NULL,
        [GioiThieu] NVARCHAR(500) NULL,
        [TrangThai] BIT NOT NULL DEFAULT 1,
        [NgayTao] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_GiangVien_TaiKhoan_MaTaiKhoan] FOREIGN KEY ([MaTaiKhoan]) REFERENCES [dbo].[TaiKhoan] ([MaTaiKhoan]) ON DELETE SET NULL
    );
    PRINT N'[OK] Đã tạo bảng GiangVien thành công.';
END
ELSE
BEGIN
    PRINT N'[INFO] Bảng GiangVien đã tồn tại.';
END
GO

-- 2. BỔ SUNG CỘT MaGiangVien VÀO BẢNG LopHoc
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[LopHoc]') AND name = 'MaGiangVien')
BEGIN
    ALTER TABLE [dbo].[LopHoc] ADD [MaGiangVien] INT NULL;
    PRINT N'[OK] Đã thêm cột MaGiangVien vào bảng LopHoc.';
END
ELSE
BEGIN
    PRINT N'[INFO] Cột MaGiangVien trong bảng LopHoc đã tồn tại.';
END
GO

-- Khóa ngoại LopHoc -> GiangVien
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LopHoc_GiangVien_MaGiangVien')
BEGIN
    ALTER TABLE [dbo].[LopHoc] ADD CONSTRAINT [FK_LopHoc_GiangVien_MaGiangVien] 
        FOREIGN KEY ([MaGiangVien]) REFERENCES [dbo].[GiangVien] ([MaGiangVien]) ON DELETE SET NULL;
    PRINT N'[OK] Đã tạo khóa ngoại FK_LopHoc_GiangVien_MaGiangVien.';
END
GO

-- 3. BỔ SUNG CỘT MaGiangVien VÀO BẢNG BuoiHoc
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[BuoiHoc]') AND name = 'MaGiangVien')
BEGIN
    ALTER TABLE [dbo].[BuoiHoc] ADD [MaGiangVien] INT NULL;
    PRINT N'[OK] Đã thêm cột MaGiangVien vào bảng BuoiHoc.';
END
ELSE
BEGIN
    PRINT N'[INFO] Cột MaGiangVien trong bảng BuoiHoc đã tồn tại.';
END
GO

-- Khóa ngoại BuoiHoc -> GiangVien
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_BuoiHoc_GiangVien_MaGiangVien')
BEGIN
    ALTER TABLE [dbo].[BuoiHoc] ADD CONSTRAINT [FK_BuoiHoc_GiangVien_MaGiangVien] 
        FOREIGN KEY ([MaGiangVien]) REFERENCES [dbo].[GiangVien] ([MaGiangVien]) ON DELETE SET NULL;
    PRINT N'[OK] Đã tạo khóa ngoại FK_BuoiHoc_GiangVien_MaGiangVien.';
END
GO

-- 4. THÊM TÀI KHOẢN VÀ DỮ LIỆU GIẢNG VIÊN MẪU (NẾU CHƯA CÓ)
IF NOT EXISTS (SELECT * FROM [dbo].[TaiKhoan] WHERE [TenDangNhap] = 'giangvien')
BEGIN
    INSERT INTO [dbo].[TaiKhoan] ([TenDangNhap], [MatKhau], [HoTen], [Email], [VaiTro], [TrangThai], [NgayTao])
    VALUES ('giangvien', '123456', N'ThS. Trần Văn Hùng', 'hung.tran@uneti.edu.vn', 'GiangVien', 1, GETDATE());
END
GO

DECLARE @MaTkGv INT = (SELECT TOP 1 [MaTaiKhoan] FROM [dbo].[TaiKhoan] WHERE [TenDangNhap] = 'giangvien');

IF NOT EXISTS (SELECT * FROM [dbo].[GiangVien] WHERE [Email] = 'hung.tran@uneti.edu.vn')
BEGIN
    INSERT INTO [dbo].[GiangVien] ([MaTaiKhoan], [HoTen], [Email], [SoDienThoai], [HocVi], [ChuyenNganh], [GioiThieu], [TrangThai], [NgayTao])
    VALUES (@MaTkGv, N'ThS. Trần Văn Hùng', 'hung.tran@uneti.edu.vn', '0912345678', N'Thạc sĩ', N'Khoa học máy tính & Công nghệ Web', N'Giảng viên khoa CNTT - Đại học UNETI. Hơn 10 năm kinh nghiệm giảng dạy ASP.NET Core và Kiến trúc phần mềm đám mây.', 1, GETDATE());
END
GO

DECLARE @GvId INT = (SELECT TOP 1 [MaGiangVien] FROM [dbo].[GiangVien] WHERE [Email] = 'hung.tran@uneti.edu.vn');
UPDATE [dbo].[LopHoc] SET [MaGiangVien] = @GvId WHERE [MaGiangVien] IS NULL;
UPDATE [dbo].[BuoiHoc] SET [MaGiangVien] = @GvId WHERE [MaGiangVien] IS NULL;
GO

PRINT N'===================================================================';
PRINT N'[THÀNH CÔNG] Cơ sở dữ liệu đã được cập nhật đầy đủ cột MaGiangVien!';
PRINT N'===================================================================';
GO
