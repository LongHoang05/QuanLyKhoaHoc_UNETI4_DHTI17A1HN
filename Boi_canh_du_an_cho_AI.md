# BỐI CẢNH DỰ ÁN DÀNH CHO AI TIẾP QUẢN

Tài liệu này để dán vào đầu một cuộc trò chuyện mới với bất kỳ AI nào. Mục đích: AI đọc xong hiểu dự án đang ở đâu, đã quyết định những gì và nên hỗ trợ người dùng theo cách nào, rồi tiếp tục đúng hướng thay vì làm lại từ đầu.

Ngày lập: 09/10/2026. Người dùng: Nguyễn Hoàng Long, nhóm trưởng nhóm 4, kiêm Module 4.

---

## 0. Cách làm việc AI cần tuân theo

1. Trả lời bằng **tiếng Việt**, giọng một Senior .NET Developer đang hướng dẫn một sinh viên làm nhóm trưởng.
2. Hướng dẫn **từng bước**, cụ thể: tên menu và nút trong **Visual Studio 2026** (giữ tên tiếng Anh như giao diện), lệnh PowerShell hoặc Package Manager Console, và **nơi cần đứng**. Người dùng đã từng chạy script nhầm thư mục (thư mục solution thay vì thư mục project), nên trước mỗi lệnh tạo hoặc xóa thư mục hãy nhắc kiểm tra `ls *.csproj` (đang ở thư mục project) hoặc `ls *.slnx` (đang ở thư mục solution).
3. **Không đoán.** Khi cần biết nội dung file thật hoặc nguyên nhân lỗi, yêu cầu người dùng dán code, nội dung thông báo lỗi, hoặc ảnh chụp cửa sổ *Error List* / *Output* / *Git Changes*.
4. **Trung thực về giới hạn.** AI không chạy được build nên mọi đoạn code là **chưa kiểm thử**. Hãy nói rõ điều đó, và nhờ người dùng build, chạy rồi báo kết quả. Điều gì không chắc thì nói là không chắc.
5. **Giữ nhất quán** với các quyết định ở mục 3 và quy ước ở mục 8. Muốn đổi thì nêu lý do và ảnh hưởng tới các module khác **trước khi** đổi.
6. Đề bài yêu cầu sinh viên **hiểu và giải thích được từng đoạn code** của mình (mục 20 của đề). Khi viết code, kèm giải thích ngắn tại sao làm vậy.
7. Câu trả lời gọn, có thứ tự, đánh số các việc người dùng phải làm. Chỉ hỏi lại khi thật sự thiếu thông tin.
8. Người dùng **không cần dữ liệu do AI tự bịa**. Mọi con số, tên, quy tắc lấy từ tài liệu này hoặc từ đề bài.

---

## 1. Dự án

- **Đề tài:** Hệ thống quản lý đăng ký khóa học và học viên trung tâm đào tạo. Bài tập lớn môn Thực hành lập trình .NET.
- **Công nghệ bắt buộc:** ASP.NET Core 10 MVC, **một project duy nhất**, Entity Framework Core 10 Code First, SQL Server (LocalDB), Visual Studio (không dùng VS Code).
- **Tên project / solution / namespace gốc:** `QuanLyKhoaHoc_UNETI4_DHTI17A1HN`. Lớp DHTI17A1HN, nhóm 4. Solution dạng `.slnx`.
- **Repository:** https://github.com/LongHoang05/QuanLyKhoaHoc_UNETI4_DHTI17A1HN
- **Hạn nộp:** chưa điền.
- Bài giảng tham khảo dùng VS Code, .NET 6, SQLite. Dự án thật dùng Visual Studio, .NET 10, SQL Server nên giữ **ý tưởng** của bài giảng (Controller và Service, ViewModel, Dependency Injection, Session, LINQ) và đổi phần công cụ.
- Đề ghi "04 sinh viên" ở mục 3 nhưng mục 4 liệt kê 5 module. Nhóm có 5 người, chia đủ 5 module.

## 2. Nhóm và phân công

| Module | Thành viên | Mã SV | Nội dung chính |
|:---:|---|---|---|
| 1 | Vũ Đình Trường | 23103100026 | Tài khoản, đăng nhập, phân quyền, quản lý chương trình đào tạo |
| 2 | Nguyễn Đình Kiên | 23103100020 | Quản lý lớp học; tìm kiếm, lọc, sắp xếp, phân trang |
| 3 | Nguyễn Đức Linh | 23103100042 | Quản lý học viên, hồ sơ cá nhân, đăng ký lớp, theo dõi đăng ký (phía học viên) |
| 4 | Nguyễn Hoàng Long (nhóm trưởng) | 23103100046 | Tiếp nhận đăng ký, duyệt hoặc từ chối, ghi nhận học phí, trạng thái và sĩ số (phía Giáo vụ) |
| 5 | Nguyễn Tùng Dương | 23103100003 | Lịch học, kết quả học tập, Dashboard, thống kê |

Ba vai trò người dùng của hệ thống: **Admin**, **Giáo vụ**, **Học viên**.

## 3. Các quyết định thiết kế đã chốt (và lý do)

| Quyết định | Lý do |
|---|---|
| 1 Solution, 1 Project, chia module bằng thư mục | Đề bắt buộc một project |
| Luồng `Controller → Service → KhoaHocDbContext`; **không** dùng lớp Repository | Giảm mã lặp, DbContext đã là Unit of Work. Thêm Repository sau nếu giảng viên yêu cầu (chưa xác nhận) |
| Validation ở ViewModel, Entity gần như không sửa | Tránh nhiều người cùng sửa Entity và xung đột Migration |
| Thư mục con của `Services/` và `ViewModels/` **không trùng tên Entity** (dùng `QuanLyLop`, `XetDuyet`, `LichHoc`...) | Visual Studio đặt namespace theo thư mục; trùng tên gây lỗi `'LopHoc' is a namespace but is used like a type` |
| Đăng ký học tách hai Controller: `DangKyHocController` (Module 3, học viên) và `QuanLyDangKyController` (Module 4, Giáo vụ) | Cùng dùng Entity `DangKyHoc` nhưng khác Controller để không sửa chung file |
| Phân quyền kiểm tra **tại Controller** bằng `[PhanQuyen(...)]`, Session ghi 3 khóa | Đề yêu cầu, không chỉ ẩn nút trên View |
| Mọi khóa ngoại `DeleteBehavior.Restrict`; muốn «bỏ» bản ghi thì đổi `TrangThai` | Giữ dữ liệu lịch sử |
| Trạng thái lưu chuỗi tiếng Việt, luôn dùng hằng số trong `Constants/` | Tránh gõ sai dấu |
| `Program.cs` có 5 vùng comment `// ===== MODULE N =====`; mỗi người chỉ thêm dòng `AddScoped` vào vùng của mình | Giảm conflict khi merge |
| **Dữ liệu mẫu nạp lúc chạy ứng dụng**, 5 file theo module (không dùng `HasData`) | Đổi dữ liệu không cần Migration; mỗi người tự sửa phần mình; mốc ngày tính theo hôm nay |
| Git: nhánh `module<số>/<mô-tả>`, Pull Request vào `main`, merge bằng «Create a merge commit» (không Squash) | Đề mục 21: giữ lịch sử commit từng người |
| Chỉ nhóm trưởng tạo Migration | Tránh xung đột `ModelSnapshot` |

## 4. Cấu trúc thư mục

```
QuanLyKhoaHoc_UNETI4_DHTI17A1HN/                  <- thư mục gốc repo (có file .slnx)
|-- .gitignore, .gitattributes, README.md
|-- QuanLyKhoaHoc_UNETI4_DHTI17A1HN.slnx
`-- QuanLyKhoaHoc_UNETI4_DHTI17A1HN/               <- project MVC duy nhất (có file .csproj)
    |-- Constants/            TrangThai.cs
    |-- Controllers/
    |-- Data/                 KhoaHocDbContext.cs, TrinhNapDuLieu.cs, Seed/TrinhNapDuLieu.Module1..5.cs
    |-- Filters/              PhanQuyenAttribute.cs   (Module 1 tạo)
    |-- Helpers/              PhienDangNhapExtensions.cs (Module 1), DanhSachPhanTrang.cs (Module 2)
    |-- Migrations/
    |-- Models/Entities/      7 Entity
    |-- Services/<nhóm>/
    |-- ViewModels/<nhóm>/
    |-- Views/<Controller>/   và Views/Shared/
    `-- wwwroot/
```

**Phạm vi sở hữu file** (mỗi file chỉ một chủ):

| Module | Controller | Thư mục Services và ViewModels | Thư mục Views |
|:---:|---|---|---|
| 1 | `TaiKhoanController`, `ChuongTrinhDaoTaoController` | `QuanLyTaiKhoan`, `QuanLyChuongTrinh` | `TaiKhoan`, `ChuongTrinhDaoTao` |
| 2 | `LopHocController` (cả Giáo vụ và học viên xem lớp) | `QuanLyLop` | `LopHoc` |
| 3 | `HocVienController`, `DangKyHocController` | `QuanLyHocVien`, `DangKyLop` | `HocVien`, `DangKyHoc` |
| 4 | `QuanLyDangKyController`, `HomeController` | `XetDuyet` | `QuanLyDangKy`, `Home` |
| 5 | `BuoiHocController`, `KetQuaHocTapController`, `ThongKeController` | `LichHoc`, `KetQua`, `ThongKe` | `BuoiHoc`, `KetQuaHocTap`, `ThongKe` |

Mỗi module cũng sở hữu file seed `Data/Seed/TrinhNapDuLieu.Module<số>.cs` của mình.

**Chỉ nhóm trưởng được sửa:** `Models/Entities/*`, `Data/KhoaHocDbContext.cs`, `Data/TrinhNapDuLieu.cs`, `Migrations/*`, `Constants/TrangThai.cs`, `appsettings.json`, `Views/Shared/_Layout.cshtml`. Ngoại lệ: mọi người thêm dòng đăng ký dịch vụ vào vùng module của mình trong `Program.cs`.

## 5. Mô hình dữ liệu (7 Entity, namespace `...Models.Entities`)

Mỗi file Entity có header ba dòng (họ tên, mã sinh viên, nội dung). Quan hệ: `TaiKhoan` 1–0..1 `HocVien`; `ChuongTrinhDaoTao` 1–N `LopHoc`; `HocVien` 1–N `DangKyHoc`; `LopHoc` 1–N `DangKyHoc`; `LopHoc` 1–N `BuoiHoc`; `DangKyHoc` 1–0..1 `KetQuaHocTap`.

| Entity | Thuộc tính chính (kiểu) |
|---|---|
| `TaiKhoan` | `MaTaiKhoan` (khóa), `TenDangNhap` (≤50, duy nhất), `MatKhau` (≤200), `HoTen` (≤100), `Email?`, `VaiTro` (≤20), `TrangThai` (chuỗi ≤30); nav `HocVien?` |
| `ChuongTrinhDaoTao` | `MaChuongTrinh`, `TenChuongTrinh` (≤150, duy nhất), `MoTa?`, `EmailLienHe?`, `TrangThai`; nav `LopHocs` |
| `LopHoc` | `MaLop`, `TenLop`, `MaChuongTrinh`, `GiangVien`, `TrinhDoDauVao?`, `SiSoToiDa`, `SoBuoiHoc`, `HocPhi` (decimal 18,0), `NgayBatDauDangKy`, `HanDangKy`, `NgayKhaiGiang`, `MoTa?`, `YeuCauHocVien?`, `TrangThai`; nav **`ChuongTrinh?`**, `DangKyHocs`, `BuoiHocs` |
| `HocVien` | `MaHocVien`, `MaTaiKhoan`, `HoTen`, `NgaySinh`, `GioiTinh` (bool: true = Nam, false = Nữ), `SoDienThoai?` (regex `^(0|\+84)(3|5|7|8|9)\d{8}$`), `Email?`, `DiaChi?`, `TrinhDoHocVan?`, `NgheNghiep?`, `TrangThai`; nav `TaiKhoan?`, `DangKyHocs` |
| `DangKyHoc` | `MaDangKy`, `MaHocVien`, `MaLop`, `NgayDangKy` (hệ thống gán), `GhiChu?`, `TrangThai`, `NgayXuLy?`, `NhanXetGiaoVu?`, `NgayDongHocPhi?`, `SoTienDaDong?` (decimal 18,0); nav `HocVien?`, `LopHoc?`, `KetQuaHocTap?` |
| `BuoiHoc` | `MaBuoiHoc`, `MaLop`, `ThoiGianBatDau`, `ThoiGianKetThuc`, `PhongHoc` (≤50), `GiangVien` (≤100), `NoiDung?`, `GhiChu?`, `TrangThai`; nav `LopHoc?` |
| `KetQuaHocTap` | `MaKetQua`, `MaDangKy`, `DiemChuyenCan`, `DiemCuoiKhoa`, `DiemTongKet` (decimal 4,2, khoảng 0–10), `KetQua?`, `NhanXet?`, `NgayCapNhat` |

Lưu ý: file Entity thật có thể khác bảng này đôi chút (người dùng có thể đã áp dụng thêm: giá trị mặc định lấy từ hằng số, `HocVien.NgaySinh` và `GioiTinh` cho phép null). **Muốn viết code đụng tới Entity, hãy yêu cầu người dùng dán file thật.**

`KhoaHocDbContext` (`Data/`): 7 `DbSet`, chỉ mục duy nhất cho `TaiKhoan.TenDangNhap` và `ChuongTrinhDaoTao.TenChuongTrinh`, vòng lặp đặt mọi khóa ngoại thành `Restrict`.

`appsettings.json`: `DefaultConnection` = `Server=(localdb)\MSSQLLocalDB;Database=QuanLyKhoaHoc_UNETI4;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true`.

`Program.cs` (dạng `class Program` có `Main`): `AddControllersWithViews`, `AddDbContext<KhoaHocDbContext>` (SQL Server), `AddDistributedMemoryCache` và `AddSession` (30 phút, HttpOnly), 5 vùng module, sau `Build()` gọi `TrinhNapDuLieu.NapDuLieu(db)` trong một scope, `UseSession()` đặt sau `UseRouting()` và trước `UseAuthorization()`.

## 6. Hằng số (`Constants/TrangThai.cs`, namespace `...Constants`)

| Lớp | Giá trị |
|---|---|
| `LoaiVaiTro` | `Admin` = "Admin", `GiaoVu` = "GiaoVu", `HocVien` = "HocVien" |
| `KhoaSession` | `MaTaiKhoan`, `HoTen`, `VaiTro` (khóa Session) |
| `TrangThaiChung` | `HoatDong` = "Hoạt động", `Khoa` = "Khóa" |
| `TrangThaiLop` | `ChuaMo` = "Chưa mở", `DangMo` = "Đang mở đăng ký", `TamDung` = "Tạm dừng", `DaDong` = "Đã đóng đăng ký"; mảng `TatCa` |
| `TrangThaiDangKy` | `ChoDuyet`, `DaDuyet`, `DaDongHocPhi`, `DangHoc`, `HoanThanh`, `KhongHoanThanh`, `TuChoi`, `DaHuy` (chuỗi: "Chờ duyệt", "Đã duyệt", "Đã đóng học phí", "Đang học", "Hoàn thành", "Không hoàn thành", "Từ chối", "Đã hủy"); mảng `TinhSiSo` = {DaDuyet, DaDongHocPhi, DangHoc} |
| `TrangThaiBuoiHoc` | `DaLenLich` = "Đã lên lịch", `DaHoanThanh` = "Đã hoàn thành", `DaHuy` = "Đã hủy" |

Lưu ý lịch sử: ban đầu lớp vai trò tên `VaiTro` trùng với thuộc tính `TaiKhoan.VaiTro` nên đã đổi thành `LoaiVaiTro`.

## 7. Dữ liệu mẫu (nạp lúc chạy ứng dụng)

- `TrinhNapDuLieu.NapDuLieu(db)` gọi `db.Database.Migrate()` (tự tạo database và áp Migration) rồi nạp theo thứ tự Module 1 đến 5 (có khóa ngoại): `NapModule1` (tài khoản, chương trình), `NapModule2` (lớp), `NapModule3` (học viên), `NapModule4` (đăng ký), `NapModule5` (buổi học, kết quả).
- Mỗi hàm chỉ nạp khi bảng còn trống (`if (!db.<Bảng>.Any())`) nên chạy nhiều lần không trùng. Hệ quả: máy đã có dữ liệu sẽ **không tự nhận** dữ liệu mẫu mới; muốn nhận phải **xóa database** (SQL Server Object Explorer, tick *Close existing connections*) rồi F5.
- Mốc ngày tính theo `DateTime.Today`. `Update-Database` chỉ tạo bảng; dữ liệu chỉ nạp khi chạy ứng dụng.
- Số dòng: `TaiKhoans` 35, `ChuongTrinhDaoTaos` 5, `LopHocs` 15, `HocViens` 30, `DangKyHocs` 64, `BuoiHocs` 34, `KetQuaHocTaps` 7.
- Tài khoản (mật khẩu `123456`): `admin`, `admin2`, `giaovu1` đến `giaovu3`, `hocvien01` đến `hocvien30`. `hocvien30` bị khóa (tài khoản và hồ sơ). `hocvien29` có hồ sơ thiếu số điện thoại, địa chỉ, trình độ.
- Tình huống có sẵn: chương trình «Kỹ năng mềm» đang khóa và chưa có lớp; lớp đang mở, hết hạn đăng ký (C# nâng cao, vẫn ghi Đang mở), chưa mở, tạm dừng (Kế toán thuế), đã đóng, lớp **đầy chỗ** (Tiếng Anh giao tiếp A2, sĩ số 5); đăng ký đủ 8 trạng thái, có đăng ký đóng 50% và 30% học phí; lớp «Excel cho kế toán» đã học xong mọi buổi và còn 5 học viên Đang học (để thử nhập kết quả); lớp «Thiết kế UI/UX» đã kết thúc và có kết quả Hoàn thành lẫn Không hoàn thành.
- `Module2.cs` khai báo hằng tên lớp (`LopNetCoBan`, `LopAnhA2`...) mà Module 4 và 5 dùng để tra lớp. Học viên số N trong seed ứng với tài khoản `hocvienNN` (thứ tự `MaHocVien` theo tên đăng nhập).

## 8. Quy ước chung

**Đặt tên:** Controller `<ChứcNăng>Controller`; Service `I<ChứcNăng>Service` và `<ChứcNăng>Service`; ViewModel `<ChứcNăng><Việc>ViewModel`; namespace theo thư mục.

**Header mỗi file mã nguồn (đề mục 18):** ba dòng comment ở đầu: `// Họ và tên: ...`, `// Mã sinh viên: ...`, `// Nội dung thực hiện: ...` (file `.cshtml` dùng `@* ... *@`). Chỉ ghi tên người thực sự làm file đó.

**Git (đề mục 21):** nhánh `module<số>/<mô-tả-ngắn>` tạo từ `main` mới nhất; commit nhỏ, thường xuyên, mẫu `[Mã SV] [Module] Nội dung` (tiếng Việt không dấu); trước khi push merge `origin/main`; Pull Request vào `main`, reviewer là nhóm trưởng, thêm một người review chéo theo vòng (M1 do M2 xem, M2 do M3, M3 do M4, M4 do M5, M5 do M1); merge bằng «Create a merge commit». Không push thẳng `main`, không `push --force`, không commit `bin/`, `obj/`, `.vs/`.

**Migration:** chỉ nhóm trưởng tạo, tên có nghĩa (`InitialCreate`, `M3_ThemCotLyDoHuy`). Thành viên không tự tạo Migration trên nhánh riêng.

**Code (đề mục 14, 15):** View không giữ nguyên bản scaffold (ngày `type="date"`, giờ `datetime-local`, mô tả `textarea`, chương trình và lớp dùng `select` lấy từ database, hiển thị tên thay cho mã khóa ngoại, nút chỉ hiện đúng trạng thái). Tìm kiếm, lọc, sắp xếp, phân trang bằng LINQ trên truy vấn (`Skip`/`Take`), **không** tải hết rồi lọc bằng C# hay JavaScript. Thứ tự truy vấn danh sách: truy vấn → tìm kiếm → lọc → sắp xếp → phân trang → hiển thị.

**Session và dữ liệu cá nhân:** đăng nhập ghi 3 khóa (`KhoaSession.MaTaiKhoan` kiểu int, `HoTen`, `VaiTro`). Học viên chỉ xem và sửa dữ liệu của chính mình: luôn lấy mã tài khoản từ Session, **không tin mã trên URL hay form**, ràng buộc truy vấn theo chủ sở hữu và trả `NotFound` nếu không khớp. Trong View, đọc Session cần `@using Microsoft.AspNetCore.Http`.

**Các định nghĩa thống nhất:**
- Sĩ số lớp = số `DangKyHoc` có trạng thái thuộc `TrangThaiDangKy.TinhSiSo`.
- Đăng ký đang xử lý (chặn đăng ký trùng) = Chờ duyệt, Đã duyệt, Đã đóng học phí, Đang học.
- Hồ sơ học viên đủ thông tin = có giá trị ở HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi.
- Lớp mở cho học viên đăng ký = trạng thái Đang mở đăng ký, `NgayBatDauDangKy` ≤ hôm nay ≤ `HanDangKy` (so theo ngày), và sĩ số < `SiSoToiDa`.
- Lớp được xếp lịch = có ít nhất một đăng ký Đã đóng học phí hoặc Đang học (đề chỉ nêu Đã đóng học phí; nhóm hiểu thêm Đang học; chưa xác nhận với giảng viên).
- Lớp đã học xong = có ít nhất một buổi chưa hủy và mọi buổi chưa hủy đều Đã hoàn thành.
- Kết quả được công bố = đăng ký có bản ghi `KetQuaHocTap` và ở trạng thái Hoàn thành hoặc Không hoàn thành.
- Học phí cộng dồn: `SoTienDaDong` là tổng đã đóng; mỗi lần ghi nhận > 0 và tổng không vượt học phí lớp.
- Điểm tổng kết = 20% chuyên cần + 80% cuối khóa, do hệ thống tính. Đề xuất: tổng kết ≥ 5 gợi ý Hoàn thành, Giáo vụ xác nhận cuối cùng.

**Luồng trạng thái đăng ký** (mọi nơi đổi trạng thái phải gọi `LuongTrangThaiDangKy.HopLe(tu, den)`, do Module 4 cung cấp):

| Từ | Sang | Ai thực hiện |
|---|---|---|
| Chờ duyệt | Đã duyệt | Giáo vụ, khi còn sĩ số |
| Chờ duyệt | Từ chối | Giáo vụ, có nhận xét |
| Chờ duyệt; Đã duyệt (chưa đóng học phí) | Đã hủy | Học viên, chỉ đăng ký của chính mình |
| Chờ duyệt; Đã duyệt; Đã đóng học phí | Đã hủy | Giáo vụ, có lý do |
| Đã duyệt | Đã đóng học phí | Giáo vụ, khi tổng tiền đã đóng bằng học phí lớp |
| Đã đóng học phí | Đang học | Giáo vụ, khi lớp có buổi chưa hủy và đã đến ngày khai giảng |
| Đang học | Hoàn thành hoặc Không hoàn thành | Giáo vụ (Module 5), khi lớp đã học xong và đã nhập điểm |

Trạng thái cuối, không chuyển tiếp: Từ chối, Đã hủy, Hoàn thành, Không hoàn thành.

**Bàn giao giữa các module** («Ngày N» tính từ ngày nhóm trưởng đẩy khung lên GitHub):

| Sản phẩm | Người làm | Hạn |
|---|---|---|
| `Helpers/DanhSachPhanTrang<T>` (`TaoAsync` dùng `CountAsync`, `Skip`, `Take`) | Module 2 | Ngày 1 |
| Session ghi đủ 3 khóa; action `DangNhap`, `DangXuat`, `TuChoiTruyCap` trong `TaiKhoanController` | Module 1 | Ngày 2 |
| `Filters/PhanQuyenAttribute` (cách dùng `[PhanQuyen(LoaiVaiTro.Admin, LoaiVaiTro.GiaoVu)]`; không tham số = chỉ cần đăng nhập) và `Helpers/PhienDangNhapExtensions` (`LayMaTaiKhoan`, `LayHoTen`, `LayVaiTro`) | Module 1 | Ngày 2 |
| Partial `Views/TaiKhoan/_ThongTinDangNhap.cshtml` | Module 1 | Ngày 2 |
| `Services/XetDuyet/LuongTrangThaiDangKy` | Module 4 | Ngày 2 |
| `_Layout` có menu theo vai trò, `HomeController`, 5 vùng trong `Program.cs` | Module 4 | Ngày 1 |
| `ILichHocService.LayLichChuaHuyAsync(maLop)`, `IKetQuaService.LayKetQuaAsync(maDangKy)` (chỉ đọc) | Module 5 | Ngày 3 |

## 9. Yêu cầu nghiệp vụ chính từ đề (tóm tắt theo module)

**Module 1.** Đăng nhập có ba thông báo riêng (không tồn tại, sai mật khẩu, tài khoản bị khóa); thành công thì ghi Session và chuyển hướng theo vai trò (Admin và Giáo vụ về `ThongKe/Index`, Học viên về trang chủ; returnUrl chỉ nhận URL nội bộ). Đăng xuất bằng POST và `Session.Clear()`. Quản lý tài khoản (Admin): tìm, lọc, thêm, sửa, khóa hoặc mở khóa; tên đăng nhập bắt buộc, ≤50, không trùng (không phân biệt hoa thường); mật khẩu bắt buộc khi tạo, để trống khi sửa nghĩa là giữ nguyên; vai trò chọn từ `LoaiVaiTro`; Admin không tự khóa hoặc tự hạ quyền chính mình; không hiển thị mật khẩu. Chương trình đào tạo (Admin): tên bắt buộc ≤150, không trùng, email đúng định dạng; không xóa nếu còn lớp, gợi ý đổi sang Khóa. Băm mật khẩu và đổi mật khẩu là chức năng nâng cao tùy chọn.

**Module 2.** Lớp học: tên bắt buộc; sĩ số, số buổi > 0; học phí ≥ 0; hạn đăng ký không trước ngày bắt đầu đăng ký; khai giảng không trước hạn đăng ký (dùng `IValidatableObject` trên ViewModel). Không chuyển sang Đang mở nếu dữ liệu bắt buộc chưa hợp lệ; không giảm sĩ số dưới sĩ số hiện tại; không xóa lớp đã có đăng ký hoặc buổi học. Tìm theo tên lớp, tên chương trình, giảng viên (`Contains`, không phân biệt hoa thường). Lọc kết hợp: chương trình, trình độ đầu vào, trạng thái, khoảng học phí, còn hạn hoặc hết hạn, còn chỗ hoặc hết chỗ. Sắp xếp: tên A–Z và Z–A, hạn đăng ký tăng và giảm, học phí tăng và giảm, khai giảng gần nhất và xa nhất. Phân trang giữ nguyên từ khóa, bộ lọc, kiểu sắp xếp khi chuyển trang; tính sĩ số bằng subquery trong cùng truy vấn (tránh N+1). Học viên chỉ thấy lớp đang mở đăng ký, kèm nhãn còn hạn hoặc hết hạn, còn chỗ hoặc hết chỗ; nút Đăng ký chỉ bật khi đủ điều kiện.

**Module 3.** Hồ sơ học viên (`HoSoViewModel` chỉ chứa trường được sửa, chống overposting): họ tên bắt buộc, ngày sinh hợp lệ, số điện thoại đúng mẫu, email đúng định dạng, giới tính radio. Quản lý học viên (Admin và Giáo vụ): tìm, lọc, thêm hồ sơ cho tài khoản học viên chưa có hồ sơ, khóa hoặc mở khóa, không xóa nếu đã có đăng ký. Đăng ký lớp kiểm tra theo thứ tự, mỗi lỗi một thông báo: học viên tồn tại và hoạt động; lớp tồn tại và đang mở đăng ký; hôm nay trong khoảng đăng ký; còn chỗ; không có đăng ký đang xử lý cùng lớp; hồ sơ đủ thông tin (thiếu thì chuyển về trang hồ sơ). `NgayDangKy` hệ thống gán, trạng thái ban đầu Chờ duyệt; kiểm tra lại sĩ số ngay trước khi lưu. Hủy đăng ký: POST, chỉ của mình, chỉ khi Chờ duyệt hoặc Đã duyệt chưa đóng học phí. Theo dõi: danh sách và chi tiết đăng ký của mình (học phí đã đóng, còn lại), lịch học (buổi chưa hủy) chỉ khi đăng ký ở Đã đóng học phí, Đang học, Hoàn thành, Không hoàn thành; kết quả khi đã công bố.

**Module 4.** Danh sách đăng ký: tìm theo tên học viên và tên lớp, lọc theo chương trình, trạng thái, khoảng ngày đăng ký; sắp xếp; phân trang. Chi tiết: thông tin học viên, lớp, yêu cầu đầu vào, sĩ số X/Y; nút theo đúng trạng thái. Duyệt: chỉ Chờ duyệt; kiểm tra đăng ký tồn tại và chưa hủy, lớp tồn tại, học viên còn hoạt động, chưa xử lý trước đó, sĩ số chưa đạt tối đa; kiểm tra lại trong transaction. Từ chối: bắt buộc nhận xét. Ghi nhận học phí: chỉ Đã duyệt; số tiền > 0; tổng đã đóng không vượt học phí; `NgayDongHocPhi` do hệ thống gán; đủ học phí thì chuyển Đã đóng học phí. Chuyển Đang học: chỉ Đã đóng học phí, lớp có buổi chưa hủy và đã đến ngày khai giảng. Hủy bởi Giáo vụ: từ Chờ duyệt, Đã duyệt, Đã đóng học phí, bắt buộc lý do. Khi lớp đầy: cảnh báo, chặn duyệt thêm, cho đóng đăng ký lớp (chỉ đặt `LopHoc.TrangThai` = Đã đóng đăng ký).

**Module 5.** Xếp lịch kiểm tra theo thứ tự (LINQ, chỉ trên các buổi chưa hủy): lớp được xếp lịch; kết thúc sau bắt đầu; bắt đầu không ở quá khứ; số buổi chưa hủy không vượt `SoBuoiHoc`; không trùng khung giờ với buổi khác cùng lớp, cùng phòng (`Trim`, không phân biệt hoa thường), cùng giảng viên. Công thức trùng: `b.TrangThai != DaHuy && b.MaBuoiHoc != id && b.ThoiGianBatDau < ketThuc && b.ThoiGianKetThuc > batDau`. Hoàn thành buổi chỉ khi đã qua giờ kết thúc; hủy buổi chỉ khi Đã lên lịch, ghi lý do. Kết quả: chỉ đăng ký Đang học thuộc lớp đã học xong; điểm 0–10; tổng kết hệ thống tính; Giáo vụ xác nhận Hoàn thành hoặc Không hoàn thành rồi chuyển trạng thái đăng ký qua `LuongTrangThaiDangKy`. Dashboard 9 chỉ số: tổng chương trình, tổng lớp, lớp đang mở đăng ký, tổng học viên, tổng đăng ký, đăng ký Chờ duyệt, đăng ký Đã duyệt, buổi học sắp tới, số học viên Hoàn thành (distinct). Thống kê 11 truy vấn LINQ: số lớp theo chương trình, số đăng ký theo lớp, theo trạng thái, lớp nhiều đăng ký nhất (xử lý đồng hạng), doanh thu theo tháng, doanh thu theo chương trình, đăng ký theo tháng, buổi học theo tháng, điểm trung bình theo lớp, tỷ lệ lấp đầy sĩ số, tỷ lệ Hoàn thành (chia cho 0 trả 0).

**Kiểm thử theo đề (mục 22):** 22.1 tài khoản và phân quyền; 22.2 CRUD và Validation; 22.3 tìm kiếm, lọc, sắp xếp, phân trang; 22.4 đăng ký và xử lý đăng ký; 22.5 học phí và sĩ số; 22.6 lịch học; 22.7 kết quả học tập.

## 10. Trạng thái hiện tại (09/10/2026)

**Xong (người dùng đã xác nhận làm theo hướng dẫn):** project được tạo, 3 gói EF Core 10.0.12 (`SqlServer`, `Tools`, `Design`), 7 Entity, Constants, `KhoaHocDbContext`, connection string, cấu hình Session, Migration, bộ nạp dữ liệu mẫu lúc chạy (5 file seed).

**Đã soạn, người dùng cần đưa vào repo:** `README.md` hoàn chỉnh (13 mục); Tài liệu phân công công việc chi tiết từng người (11 mục) dạng Claude Doc.

**Chưa xác nhận hoặc chưa làm:** khởi tạo Git và push lên GitHub; thử clone sạch; mời 4 thành viên (Settings, Collaborators); bật quy tắc bắt buộc Pull Request cho `main` (repo private tài khoản Free có thể không bật được); bật «Allow merge commits», tắt «Allow squash merging»; thêm 5 vùng module vào `Program.cs` (nếu chưa); điền header cho `Constants/TrangThai.cs`; các bàn giao của Module 4 (`LuongTrangThaiDangKy`, `_Layout` có menu theo vai trò, `HomeController` và trang chủ); điền hạn nộp.

**File cũ không còn dùng:** `KhungDuLieuMau.zip`, `Data/DuLieuMau.cs` (bản `HasData`).

## 11. Việc tiếp theo (theo thứ tự)

1. Kiểm tra lần cuối: build 0 lỗi, F5 chạy được, bảng đủ số dòng ở mục 7.
2. Chép `README.md` vào thư mục solution; tạo `.gitattributes` (một dòng `* text=auto`); có `.gitignore` chuẩn Visual Studio.
3. `git init -b main`, `git add .`, kiểm tra `git status` không có `bin/`, `obj/`, `.vs/`, commit, thêm remote, `git push -u origin main`. Đặt `user.name` và `user.email` đúng tài khoản GitHub.
4. **Thử clone sạch**: xóa database cũ, clone sang thư mục khác, mở `.slnx`, F5, kiểm tra có đủ dữ liệu. Sửa lỗi trước khi báo nhóm.
5. Cài đặt GitHub như mục 10, rồi gửi nhóm link repo và tài liệu phân công.
6. Trong 2 ngày đầu, làm các bàn giao của Module 4: `LuongTrangThaiDangKy` (hàm `HopLe` theo bảng luồng trạng thái), `_Layout` có menu theo vai trò (nhúng `_ThongTinDangNhap` của Module 1), `HomeController`, trang chủ.
7. Lộ trình: Giai đoạn 1 (Ngày 1–3) nền tảng; Giai đoạn 2 (Ngày 4–8) hoàn thiện nghiệp vụ từng module; Giai đoạn 3 (Ngày 9–11) tích hợp và kiểm thử chéo theo mục 22; Giai đoạn 4 (Ngày 12–14) hoàn thiện, báo cáo, sơ đồ ERD, đối chiếu lịch sử commit với header, chuẩn bị bảo vệ.

## 12. Lỗi đã gặp và bài học (đừng lặp lại)

| Tình huống | Nguyên nhân | Cách xử lý |
|---|---|---|
| `'LopHoc' is a namespace but is used like a type` | Thư mục `Services/LopHoc` trùng tên Entity | Đặt tên thư mục khác tên Entity |
| Lỗi tên lớp `VaiTro` | Trùng thuộc tính `TaiKhoan.VaiTro` | Đổi thành `LoaiVaiTro` |
| Script tạo thư mục sinh nhầm ở thư mục solution | Terminal mặc định ở thư mục solution | Xóa phần thừa, chạy lại ở thư mục project; kiểm tra `ls *.csproj` trước |
| `Program.cs` dạng `class Program` + `Main` | Khác dạng top-level statements | Các dòng `using` đặt trên cùng, trước `namespace` |
| README mâu thuẫn tài liệu phân công về `Program.cs` | Một bên cho nhóm trưởng sửa, một bên cho mỗi người tự thêm | Thống nhất dùng 5 vùng module |
| `PendingModelChangesWarning` khi `Update-Database` | Thêm `HasData` nhưng chưa tạo Migration | Tạo Migration mới |
| `The name 'InitialCreate' is used by an existing migration` | Thư mục `Migrations` chưa xóa hẳn | Xóa lại hoặc đặt tên khác |
| Mỗi lần đổi dữ liệu mẫu phải tạo Migration | Đặc điểm của `HasData` | Chuyển sang nạp dữ liệu lúc chạy |
| Scaffold tự sửa `Program.cs` hoặc `KhoaHocDbContext.cs` | Scaffold thêm cấu hình | Mở *Git Changes* và **Undo** hai file đó |

## 13. Điều chưa chắc và cần hỏi giảng viên

1. Lớp có học viên Đang học nhưng không còn ai ở Đã đóng học phí có được xếp thêm buổi không.
2. Học phí ghi nhận cộng dồn nhiều lần hay mỗi lần là toàn bộ số tiền.
3. Ngưỡng Hoàn thành có phải điểm tổng kết từ 5 trở lên.
4. Có yêu cầu dùng Repository Pattern không (dự án hiện không dùng).
5. Mục 3 của đề ghi 04 sinh viên trong khi nhóm có 5 người.
6. Một nhận định của AI trước đó: .NET 10 cần Visual Studio 2026 (VS 2022 không build được). Người dùng nên tự kiểm tra trên máy.

## 14. Tài liệu đi kèm (nếu AI cần, hãy yêu cầu người dùng dán nội dung)

- `README.md` của repo: quy ước, cách chạy, quy trình Git, lỗi thường gặp.
- Tài liệu phân công công việc: việc chi tiết từng người (mã việc M1-01 ... M5-08), mốc, tiêu chí hoàn thành, kịch bản tự kiểm thử, commit mẫu, checklist bảo vệ.
- File đề bài của môn học (người dùng giữ).

---

## Câu mở đầu gợi ý để dán sau tài liệu này

> Hãy đọc kỹ tài liệu bối cảnh ở trên và làm việc theo mục 0. Dự án đang ở trạng thái mục 10. Việc tôi đang làm là: [ghi việc cụ thể, ví dụ: «đẩy khung lên GitHub và thử clone sạch» hoặc «viết LuongTrangThaiDangKy cho Module 4»]. Hãy hướng dẫn từng bước trên Visual Studio 2026. Nếu cần xem file thật hoặc thông báo lỗi, hãy yêu cầu tôi dán vào.
