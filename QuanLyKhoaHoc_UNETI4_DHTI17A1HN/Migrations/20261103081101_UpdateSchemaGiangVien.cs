using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaGiangVien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
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
                END;

                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LopHoc') AND name = 'MaGiangVien')
                BEGIN
                    ALTER TABLE [dbo].[LopHoc] ADD [MaGiangVien] INT NULL;
                END;

                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LopHoc_GiangVien_MaGiangVien')
                BEGIN
                    ALTER TABLE [dbo].[LopHoc] ADD CONSTRAINT [FK_LopHoc_GiangVien_MaGiangVien] FOREIGN KEY ([MaGiangVien]) REFERENCES [dbo].[GiangVien] ([MaGiangVien]) ON DELETE SET NULL;
                END;

                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('BuoiHoc') AND name = 'MaGiangVien')
                BEGIN
                    ALTER TABLE [dbo].[BuoiHoc] ADD [MaGiangVien] INT NULL;
                END;

                IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_BuoiHoc_GiangVien_MaGiangVien')
                BEGIN
                    ALTER TABLE [dbo].[BuoiHoc] ADD CONSTRAINT [FK_BuoiHoc_GiangVien_MaGiangVien] FOREIGN KEY ([MaGiangVien]) REFERENCES [dbo].[GiangVien] ([MaGiangVien]) ON DELETE SET NULL;
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LopHoc_MaGiangVien' AND object_id = OBJECT_ID('LopHoc'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_LopHoc_MaGiangVien] ON [dbo].[LopHoc] ([MaGiangVien]);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_BuoiHoc_MaGiangVien' AND object_id = OBJECT_ID('BuoiHoc'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_BuoiHoc_MaGiangVien] ON [dbo].[BuoiHoc] ([MaGiangVien]);
                END;

                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GiangVien_MaTaiKhoan' AND object_id = OBJECT_ID('GiangVien'))
                BEGIN
                    CREATE UNIQUE NONCLUSTERED INDEX [IX_GiangVien_MaTaiKhoan] ON [dbo].[GiangVien] ([MaTaiKhoan]) WHERE [MaTaiKhoan] IS NOT NULL;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_BuoiHoc_GiangVien_MaGiangVien')
                    ALTER TABLE [dbo].[BuoiHoc] DROP CONSTRAINT [FK_BuoiHoc_GiangVien_MaGiangVien];

                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_LopHoc_GiangVien_MaGiangVien')
                    ALTER TABLE [dbo].[LopHoc] DROP CONSTRAINT [FK_LopHoc_GiangVien_MaGiangVien];

                IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GiangVien')
                    DROP TABLE [dbo].[GiangVien];

                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LopHoc') AND name = 'MaGiangVien')
                    ALTER TABLE [dbo].[LopHoc] DROP COLUMN [MaGiangVien];

                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('BuoiHoc') AND name = 'MaGiangVien')
                    ALTER TABLE [dbo].[BuoiHoc] DROP COLUMN [MaGiangVien];
            ");
        }
    }
}
