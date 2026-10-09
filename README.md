# Hệ thống quản lý đăng ký khóa học và học viên trung tâm đào tạo

Bài tập lớn môn **Thực hành lập trình .NET**, xây dựng bằng ASP.NET Core 10 MVC, Entity Framework Core 10 (Code First) và SQL Server.

- **Project / Solution:** `QuanLyKhoaHoc_UNETI4_DHTI17A1HN`
- **Nhóm:** 4 | **Lớp:** DHTI17A1HN
- **Namespace gốc:** `QuanLyKhoaHoc_UNETI4_DHTI17A1HN`
- **Repository:** https://github.com/LongHoang05/QuanLyKhoaHoc_UNETI4_DHTI17A1HN

---

## 1. Thành viên và phân công

| Module | Thành viên | Mã SV | Nội dung chính |
|:------:|------------|:-----:|----------------|
| 1 | Vũ Đình Trường | 23103100026 | Tài khoản, đăng nhập, phân quyền, quản lý chương trình đào tạo |
| 2 | Nguyễn Đình Kiên | 23103100020 | Quản lý lớp học, tìm kiếm, lọc, sắp xếp, phân trang |
| 3 | Nguyễn Đức Linh | 23103100042 | Quản lý học viên, hồ sơ cá nhân, đăng ký lớp, theo dõi đăng ký (phía học viên) |
| 4 | **Nguyễn Hoàng Long** (nhóm trưởng) | 23103100046 | Tiếp nhận đăng ký, duyệt/từ chối, ghi nhận học phí, trạng thái và sĩ số (phía Giáo vụ) |
| 5 | Nguyễn Tùng Dương | 23103100003 | Lịch học, kết quả học tập, Dashboard, thống kê |

Việc chi tiết của từng người xem **Tài liệu phân công công việc** của nhóm.

### Phạm vi sở hữu file

Mỗi file chỉ có **một chủ**. Chỉ sửa file thuộc module của mình.

| Module | Controller | Thư mục `Services/` và `ViewModels/` | Thư mục `Views/` |
|:------:|------------|---------------------------------------|-------------------|
| 1 | `TaiKhoanController`, `ChuongTrinhDaoTaoController` | `QuanLyTaiKhoan`, `QuanLyChuongTrinh` | `TaiKhoan`, `ChuongTrinhDaoTao` |
| 2 | `LopHocController` (cả Giáo vụ và học viên xem lớp) | `QuanLyLop` | `LopHoc` |
| 3 | `HocVienController`, `DangKyHocController` | `QuanLyHocVien`, `DangKyLop` | `HocVien`, `DangKyHoc` |
| 4 | `QuanLyDangKyController`, `HomeController` | `XetDuyet` | `QuanLyDangKy`, `Home` |
| 5 | `BuoiHocController`, `KetQuaHocTapController`, `ThongKeController` | `LichHoc`, `KetQua`, `ThongKe` | `BuoiHoc`, `KetQuaHocTap`, `ThongKe` |

Ngoài ra:

- Module 1 tạo `Filters/PhanQuyenAttribute.cs` và `Helpers/PhienDangNhapExtensions.cs`.
- Module 2 tạo `Helpers/DanhSachPhanTrang.cs` (phân trang bằng `Skip`/`Take` trên truy vấn).
- Mỗi module tự sửa file dữ liệu mẫu của mình: `Data/Seed/TrinhNapDuLieu.Module<số>.cs` (xem mục 7).
- Module 3 làm phía **học viên** của đăng ký, Module 4 làm phía **Giáo vụ**. Hai module dùng chung Entity `DangKyHoc` nhưng **khác Controller** để không sửa chung file.

### File chỉ nhóm trưởng được sửa

Cần thay đổi thì nhắn nhóm trưởng, **không tự sửa**:

- `Models/Entities/*` (7 Entity)
- `Data/KhoaHocDbContext.cs`, `Data/TrinhNapDuLieu.cs`
- `Migrations/*`
- `Constants/TrangThai.cs`
- `appsettings.json`
- `Views/Shared/_Layout.cshtml`

**Ngoại lệ `Program.cs`:** file này có sẵn 5 vùng `// ===== MODULE N =====`. Mỗi người **chỉ thêm dòng đăng ký dịch vụ vào vùng của mình** (xem mục 10), không sửa phần khác.

---

## 2. Công nghệ và yêu cầu môi trường

| Thành phần | Yêu cầu | Ghi chú |
|------------|---------|---------|
| **IDE** | **Visual Studio 2026** | .NET 10 cần Visual Studio 2026. Workload: **ASP.NET and web development**; component: **SQL Server Express LocalDB** (cài qua Visual Studio Installer) |
| **Framework** | **.NET 10 SDK** | Kiểm tra: `dotnet --list-sdks` (phải có dòng bắt đầu bằng `10.0`) |
| **ORM** | **Entity Framework Core 10** | Ba gói `SqlServer`, `Tools`, `Design` cùng phiên bản `10.0.x` (hiện dùng `10.0.12`) |
| **Database** | **SQL Server LocalDB** | Instance `(localdb)\MSSQLLocalDB`, database `QuanLyKhoaHoc_UNETI4` |
| **Quản lý mã nguồn** | **Git** | Tích hợp trong Visual Studio (cửa sổ *Git Changes*) |

---

## 3. Cài đặt và chạy lần đầu

1. Clone repo (xem mục 8, bước 1) và mở file **`QuanLyKhoaHoc_UNETI4_DHTI17A1HN.slnx`**. Chờ Visual Studio tự restore NuGet.
2. Nhấn **F5** (nút `https` màu xanh). Lần đầu ứng dụng mất vài giây để **tự tạo database `QuanLyKhoaHoc_UNETI4`, áp Migration và nạp dữ liệu mẫu**, sau đó trình duyệt mở trang web. Không cần chạy lệnh nào khác.

Mỗi lần `git pull` mà có file mới trong `Migrations/`, chỉ cần nhấn F5 lại, ứng dụng tự áp Migration.

Connection string dùng LocalDB nên không phải cấu hình gì thêm. "Cơ sở dữ liệu dùng chung" nghĩa là cả nhóm dùng chung một schema (từ Migration trong repo) và cùng bộ dữ liệu mẫu; mỗi máy có một bản database riêng.

> Lệnh `Update-Database` trong Package Manager Console vẫn chạy được nhưng chỉ tạo bảng, **không** nạp dữ liệu mẫu. Dữ liệu mẫu chỉ được nạp khi chạy ứng dụng.

### Tài khoản kiểm thử

Mật khẩu của mọi tài khoản: `123456`.

| Tên đăng nhập | Vai trò | Ghi chú |
|---------------|---------|---------|
| `admin`, `admin2` | Admin | Quản lý tài khoản, chương trình đào tạo |
| `giaovu1`, `giaovu2`, `giaovu3` | Giáo vụ | Lớp học, đăng ký, học phí, lịch học, kết quả, thống kê |
| `hocvien01` đến `hocvien29` | Học viên | `hocvien01` có đăng ký ở nhiều trạng thái; `hocvien29` có hồ sơ chưa khai báo đủ |
| `hocvien30` | Học viên | Tài khoản và hồ sơ **bị khóa**, dùng để thử đăng nhập bị chặn |

### Dữ liệu mẫu có sẵn (nạp lúc chạy ứng dụng, xem mục 7)

| Bảng | Số dòng | Có sẵn các tình huống |
|------|:-------:|-----------------------|
| `TaiKhoans` | 35 | 2 Admin, 3 Giáo vụ, 30 học viên, 1 tài khoản bị khóa |
| `ChuongTrinhDaoTaos` | 5 | "Kỹ năng mềm" đang khóa và chưa có lớp (để thử xóa) |
| `LopHocs` | 15 | Đang mở, hết hạn đăng ký, chưa mở, tạm dừng, đã đóng, lớp **đầy chỗ** (Tiếng Anh giao tiếp A2) |
| `HocViens` | 30 | 1 hồ sơ thiếu thông tin, 1 học viên bị khóa |
| `DangKyHocs` | 64 | Đủ 8 trạng thái; có đăng ký đóng một phần học phí |
| `BuoiHocs` | 34 | Đã lên lịch, đã hoàn thành, đã hủy; không trùng lớp, phòng, giảng viên |
| `KetQuaHocTaps` | 7 | Hoàn thành và Không hoàn thành |

Lớp **Excel cho kế toán** đã học xong mọi buổi và còn 5 học viên *Đang học*: dùng để thử nhập kết quả cuối cùng. Lớp **Thiết kế UI/UX** đã kết thúc và có sẵn kết quả.

Các mốc ngày của dữ liệu mẫu tính theo **ngày hôm nay** lúc nạp, nên lớp, đăng ký và buổi học luôn đúng trạng thái dù chạy vào ngày nào.

---

## 4. Cấu trúc thư mục

```
QuanLyKhoaHoc_UNETI4_DHTI17A1HN/                  ← thư mục gốc repo (có file .slnx)
├── .gitignore, .gitattributes, README.md
├── QuanLyKhoaHoc_UNETI4_DHTI17A1HN.slnx
└── QuanLyKhoaHoc_UNETI4_DHTI17A1HN/               ← project MVC duy nhất
    ├── Constants/            Hằng số vai trò, trạng thái, khóa Session
    ├── Controllers/
    ├── Data/                 KhoaHocDbContext, TrinhNapDuLieu, Seed/ (dữ liệu mẫu từng module)
    ├── Filters/              Bộ lọc phân quyền
    ├── Helpers/              Hàm dùng chung (phân trang, đọc Session...)
    ├── Migrations/
    ├── Models/Entities/      7 Entity
    ├── Services/<nhóm>/      Nghiệp vụ và truy vấn LINQ
    ├── ViewModels/<nhóm>/
    ├── Views/<Controller>/  và  Views/Shared/
    └── wwwroot/
```

**Luồng xử lý:** `Controller → Service → KhoaHocDbContext`

- **Controller:** nhận request, kiểm tra quyền, gọi Service, trả View.
- **Service:** quy tắc nghiệp vụ và truy vấn EF Core/LINQ.
- **ViewModel:** dữ liệu cho View và Validation riêng từng màn hình. Entity gần như không phải sửa.

---

## 5. Mô hình dữ liệu

### Quan hệ giữa các Entity

| Quan hệ | Bản số |
|---------|:------:|
| `TaiKhoan` — `HocVien` | 1 – 0..1 |
| `ChuongTrinhDaoTao` — `LopHoc` | 1 – N |
| `HocVien` — `DangKyHoc` | 1 – N |
| `LopHoc` — `DangKyHoc` | 1 – N |
| `LopHoc` — `BuoiHoc` | 1 – N |
| `DangKyHoc` — `KetQuaHocTap` | 1 – 0..1 |

### Danh sách Entity

| Entity | Khóa chính | Mô tả |
|--------|:----------:|-------|
| `TaiKhoan` | `MaTaiKhoan` | Đăng nhập, vai trò (`Admin` / `GiaoVu` / `HocVien`), trạng thái |
| `ChuongTrinhDaoTao` | `MaChuongTrinh` | Chương trình đào tạo (ví dụ: Lập trình .NET, Tiếng Anh giao tiếp) |
| `LopHoc` | `MaLop` | Lớp học thuộc một chương trình; có sĩ số, học phí, ba mốc ngày (bắt đầu đăng ký, hạn đăng ký, khai giảng) |
| `HocVien` | `MaHocVien` | Hồ sơ học viên, liên kết với một `TaiKhoan` |
| `DangKyHoc` | `MaDangKy` | Đơn đăng ký của học viên vào lớp, kèm trạng thái và học phí đã đóng |
| `BuoiHoc` | `MaBuoiHoc` | Buổi học cụ thể của một lớp |
| `KetQuaHocTap` | `MaKetQua` | Điểm và kết quả của một đăng ký |

---

## 6. Quy ước chung

### Đặt tên

| Loại | Quy ước | Ví dụ |
|------|---------|-------|
| Controller | `<ChứcNăng>Controller` | `LopHocController` |
| Interface Service | `I<ChứcNăng>Service` | `ILopHocService` |
| Service | `<ChứcNăng>Service` | `LopHocService` |
| ViewModel | `<ChứcNăng><Việc>ViewModel` | `LopHocDanhSachViewModel` |
| Namespace | Theo thư mục | `QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services.QuanLyLop` |

> **Lưu ý quan trọng:** không đặt tên thư mục trong `Services/` và `ViewModels/` trùng tên Entity (ví dụ `Services/LopHoc`). Visual Studio đặt namespace theo thư mục, nên tên `LopHoc` bị hiểu là namespace và báo lỗi *"'LopHoc' is a namespace but is used like a type"*. Vì vậy dùng tên như `QuanLyLop`, `XetDuyet`, `LichHoc`.

### Hằng số (thư mục `Constants/`)

Không gõ chuỗi trực tiếp. Luôn dùng hằng số:

| Lớp hằng số | Dùng cho |
|-------------|----------|
| `LoaiVaiTro` | Vai trò: `Admin`, `GiaoVu`, `HocVien` |
| `KhoaSession` | Khóa Session: `MaTaiKhoan`, `HoTen`, `VaiTro` |
| `TrangThaiChung` | Trạng thái tài khoản, học viên, chương trình: `HoatDong`, `Khoa` |
| `TrangThaiLop` | `ChuaMo`, `DangMo`, `TamDung`, `DaDong` |
| `TrangThaiDangKy` | `ChoDuyet`, `DaDuyet`, `DaDongHocPhi`, `DangHoc`, `HoanThanh`, `KhongHoanThanh`, `TuChoi`, `DaHuy` |
| `TrangThaiBuoiHoc` | `DaLenLich`, `DaHoanThanh`, `DaHuy` |

Các trạng thái lưu trong database dưới dạng chuỗi tiếng Việt.

### Quy ước về dữ liệu và Entity

- **Giới tính** học viên: `true` = Nam, `false` = Nữ.
- Navigation property của `LopHoc` tới chương trình đào tạo tên là **`ChuongTrinh`** (không phải `ChuongTrinhDaoTao`), ví dụ `Include(l => l.ChuongTrinh)`.
- Không xóa dây chuyền trong database (mọi khóa ngoại là `Restrict`) để giữ dữ liệu lịch sử. Muốn "bỏ" bản ghi thì đổi `TrangThai` (xóa mềm).
- Validation đặt ở **ViewModel** hoặc **Service**, không sửa Entity.

### Các định nghĩa thống nhất

Để mỗi người không hiểu một kiểu:

- **Sĩ số lớp** = số `DangKyHoc` có `TrangThai` thuộc `TrangThaiDangKy.TinhSiSo` (Đã duyệt, Đã đóng học phí, Đang học). Mọi truy vấn đếm sĩ số dùng chung mảng này.
- **Đăng ký đang xử lý** (để chặn đăng ký trùng) = Chờ duyệt, Đã duyệt, Đã đóng học phí, Đang học.
- **Hồ sơ học viên đủ thông tin** = có giá trị ở `HoTen`, `NgaySinh`, `GioiTinh`, `SoDienThoai`, `Email`, `DiaChi`.
- **Lớp mở cho học viên đăng ký** = `TrangThai` là Đang mở đăng ký, `NgayBatDauDangKy` ≤ hôm nay ≤ `HanDangKy` (so theo ngày), và sĩ số nhỏ hơn `SiSoToiDa`.
- **Lớp được xếp lịch** = có ít nhất một đăng ký Đã đóng học phí hoặc Đang học.
- **Lớp đã học xong** = có ít nhất một buổi chưa hủy và mọi buổi chưa hủy đều Đã hoàn thành.
- **Kết quả được công bố** = đăng ký có bản ghi `KetQuaHocTap` và đang ở trạng thái Hoàn thành hoặc Không hoàn thành.
- **Học phí cộng dồn:** `SoTienDaDong` là tổng đã đóng. Mỗi lần ghi nhận phải lớn hơn 0 và tổng không vượt học phí của lớp.
- **Điểm tổng kết** = 20% điểm chuyên cần + 80% điểm cuối khóa, do hệ thống tính.
- Thời điểm hệ thống dùng `DateTime.Now`.

### Luồng trạng thái đăng ký

Mọi nơi đổi trạng thái đều phải gọi `LuongTrangThaiDangKy.HopLe` (do Module 4 cung cấp).

| Từ | Sang | Ai thực hiện |
|----|------|--------------|
| Chờ duyệt | Đã duyệt | Giáo vụ (Module 4), khi còn sĩ số |
| Chờ duyệt | Từ chối | Giáo vụ (Module 4), có nhận xét |
| Chờ duyệt; Đã duyệt (chưa đóng học phí) | Đã hủy | Học viên (Module 3), chỉ đăng ký của chính mình |
| Chờ duyệt; Đã duyệt; Đã đóng học phí | Đã hủy | Giáo vụ (Module 4), có lý do |
| Đã duyệt | Đã đóng học phí | Giáo vụ (Module 4), khi tổng tiền đã đóng bằng học phí lớp |
| Đã đóng học phí | Đang học | Giáo vụ (Module 4), khi lớp có buổi chưa hủy và đã đến ngày khai giảng |
| Đang học | Hoàn thành hoặc Không hoàn thành | Giáo vụ (Module 5), khi lớp đã học xong và đã nhập điểm |

Trạng thái cuối, không chuyển tiếp: Từ chối, Đã hủy, Hoàn thành, Không hoàn thành.

### Hợp đồng giữa các module

Ai nhận phần nào phải làm đúng hạn vì người khác đang chờ (Ngày N tính từ ngày nhóm trưởng đẩy khung lên GitHub):

| Sản phẩm bàn giao | Người làm | Người dùng | Hạn |
|-------------------|-----------|------------|:---:|
| `Helpers/DanhSachPhanTrang<T>` | Module 2 | Cả nhóm | Ngày 1 |
| Session ghi đủ 3 khóa theo `KhoaSession`; action `DangNhap`, `DangXuat`, `TuChoiTruyCap` trong `TaiKhoanController` | Module 1 | Cả nhóm | Ngày 2 |
| `Filters/PhanQuyenAttribute`, `Helpers/PhienDangNhapExtensions` | Module 1 | Cả nhóm | Ngày 2 |
| Partial `Views/TaiKhoan/_ThongTinDangNhap.cshtml` (họ tên, vai trò, nút Đăng xuất) | Module 1 | Module 4 nhúng vào `_Layout` | Ngày 2 |
| `Services/XetDuyet/LuongTrangThaiDangKy` (hàm `HopLe(tu, den)`) | Module 4 | Module 3, 5 | Ngày 2 |
| `_Layout` có menu theo vai trò, `HomeController`, 5 vùng trong `Program.cs` | Module 4 | Cả nhóm | Ngày 1 |
| `ILichHocService`, `IKetQuaService` (chỉ đọc) | Module 5 | Module 3 | Ngày 3 |

Quy tắc kèm theo:

- Quyền được kiểm tra **tại Controller** bằng `[PhanQuyen(...)]`, không chỉ ẩn menu hoặc nút trên View.
- Học viên chỉ xem và sửa dữ liệu của chính mình. Luôn lấy mã tài khoản từ Session, **không tin mã trên URL hay trên form**.
- Cần dữ liệu hoặc hàm của module khác thì thỏa thuận Interface với chủ module đó, không sửa code của họ.

### Header mỗi file mã nguồn

Theo yêu cầu đề bài (mục 18), mỗi file do bạn viết phải có ở đầu (file `.cshtml` dùng comment Razor `@* ... *@`):

```csharp
// Họ và tên: Nguyễn Văn A
// Mã sinh viên: 23103100001
// Nội dung thực hiện: Quản lý lớp học, tìm kiếm, lọc, sắp xếp và phân trang
```

Khi sửa file của người khác, thêm tên mình và ghi rõ phần mình làm. Không ghi tên vào file bạn không trực tiếp làm.

---

## 7. Migration và dữ liệu mẫu

### Migration

- **Chỉ nhóm trưởng tạo Migration.** Tên có nghĩa, ví dụ `InitialCreate`, `M3_ThemCotLyDoHuy`.
- Cần đổi Entity (thêm cột, thêm bảng): nhắn nhóm trưởng. Nhóm trưởng sửa, tạo Migration, commit, báo cả nhóm `pull` rồi nhấn F5.
- **Không tự ý tạo Migration trên nhánh riêng.** Hai Migration cùng sửa database gây xung đột `ModelSnapshot`.
- Ứng dụng tự áp Migration khi khởi động (lệnh `db.Database.Migrate()` trong `TrinhNapDuLieu.NapDuLieu`), nên không ai phải chạy `Update-Database` bằng tay.

### Dữ liệu mẫu

Dữ liệu mẫu **không đi cùng Migration**. Nó được nạp mỗi lần ứng dụng khởi động, qua `TrinhNapDuLieu.NapDuLieu(db)` gọi trong `Program.cs`. Mỗi module một file riêng trong `Data/Seed/`:

| File | Nạp bảng | Chủ file |
|------|----------|----------|
| `TrinhNapDuLieu.Module1.cs` | `TaiKhoans`, `ChuongTrinhDaoTaos` | Module 1 |
| `TrinhNapDuLieu.Module2.cs` | `LopHocs` | Module 2 |
| `TrinhNapDuLieu.Module3.cs` | `HocViens` | Module 3 |
| `TrinhNapDuLieu.Module4.cs` | `DangKyHocs` | Module 4 |
| `TrinhNapDuLieu.Module5.cs` | `BuoiHocs`, `KetQuaHocTaps` | Module 5 |

Quy tắc:

- **Sửa hay thêm dữ liệu mẫu không cần Migration.** Chỉ sửa file seed của module mình rồi commit.
- Các file chạy theo thứ tự 1 ➔ 5 vì có khóa ngoại. Chỉ nạp dữ liệu của bảng thuộc module mình, không xóa dòng mà module khác đang tham chiếu.
- Mỗi hàm chỉ nạp khi bảng **còn trống** (`if (!db.<Bảng>.Any())`), nên chạy nhiều lần không bị nạp trùng. Hệ quả: máy đã có dữ liệu sẽ **không tự nhận** dữ liệu mẫu mới sau khi pull. Khi đó xóa database (*View ➔ SQL Server Object Explorer ➔ (localdb)\MSSQLLocalDB ➔ Databases ➔ chuột phải `QuanLyKhoaHoc_UNETI4` ➔ Delete*, tick *Close existing connections*) rồi nhấn F5.
- Tên lớp ở `Module2.cs` (các hằng `LopNetCoBan`, `LopAnhA2`...) được Module 4 và Module 5 dùng để tra cứu lớp. Đổi tên lớp thì phải sửa luôn hai file đó và báo nhóm.
- Mốc ngày tính theo `DateTime.Today` lúc nạp.
- Muốn thêm dữ liệu thử riêng cho mình: thêm trực tiếp trong database trên máy bạn và **không commit**. Dữ liệu đó mất khi xóa database.

---

## 8. Quy trình Git

Ở mỗi bước có cách bấm trong Visual Studio và lệnh tương đương.

### Bước 1. Clone repo (làm một lần)
- **Visual Studio:** *Git ➔ Clone Repository...* ➔ dán `https://github.com/LongHoang05/QuanLyKhoaHoc_UNETI4_DHTI17A1HN.git` ➔ chọn thư mục ➔ **Clone**. Visual Studio mở sẵn solution.
- **Lệnh:** `git clone https://github.com/LongHoang05/QuanLyKhoaHoc_UNETI4_DHTI17A1HN.git`

Sau đó làm theo mục 3 để tạo database và chạy thử.

### Bước 2. Tạo nhánh cho việc mình sắp làm
Luôn tạo nhánh từ `main` mới nhất. **Không code trực tiếp trên `main`.** Tên nhánh: `module<số>/<mô-tả-ngắn>`.

- **Visual Studio:** *Git ➔ Pull* (đang ở `main`) ➔ bấm tên nhánh ở góc dưới bên phải ➔ **New Branch...** ➔ nhập tên, ô *Based on* chọn `main` ➔ **Create**.
- **Lệnh:**
  ```bash
  git checkout main
  git pull
  git checkout -b module2/phan-trang-lop-hoc
  ```

| Module | Ví dụ tên nhánh |
|:------:|-----------------|
| 1 | `module1/dang-nhap-phan-quyen` |
| 2 | `module2/crud-lop-hoc` |
| 3 | `module3/ho-so-hoc-vien` |
| 4 | `module4/duyet-dang-ky` |
| 5 | `module5/lich-hoc-buoi-hoc` |

### Bước 3. Commit nhỏ và thường xuyên
Mẫu message: `[Mã SV] [Module] Nội dung`, viết tiếng Việt không dấu. Ví dụ: `[23103100020] [LopHoc] Them chuc nang tim kiem lop`. Không dồn cuối kỳ mới commit một lần.

- **Visual Studio:** mở *Git Changes* (`Ctrl+0, Ctrl+G`) ➔ kiểm tra danh sách file chỉ thuộc module của mình ➔ nhập message ➔ **Commit All** ➔ nút **Push** (mũi tên lên).
- **Lệnh:** `git add .` ➔ `git commit -m "[23103100020] [LopHoc] Them chuc nang tim kiem lop"` ➔ `git push -u origin <tên-nhánh>`

### Bước 4. Cập nhật theo `main` trước khi tạo Pull Request
- **Visual Studio:** *Git ➔ Fetch* ➔ *Git ➔ Manage Branches* ➔ chuột phải `origin/main` ➔ **Merge 'origin/main' into '<nhánh của bạn>'**.
- **Lệnh:** `git fetch origin` rồi `git merge origin/main`

Nếu có conflict, mở file bị báo trong Visual Studio, chọn phần code đúng trong *Merge Editor*, lưu rồi commit. Không chắc thì nhắn nhóm trưởng. Sau đó **build và chạy lại** trước khi push.

### Bước 5. Tạo Pull Request
1. Vào GitHub repo, bấm **Compare & pull request**.
2. Kiểm tra `base: main` ← `compare: <nhánh của bạn>`.
3. Tiêu đề theo mẫu commit. Reviewer: nhóm trưởng (`LongHoang05`).
4. Tick checklist trong mô tả:
   - Build thành công, chạy không lỗi
   - Chỉ sửa file thuộc module của mình
   - Mỗi file mới đều có header họ tên và mã sinh viên
   - Commit message đúng định dạng
5. **Create pull request** rồi báo nhóm trưởng.

Mỗi Pull Request cần thêm một người review chéo theo vòng: Module 1 do Module 2 xem, Module 2 do Module 3 xem, Module 3 do Module 4 xem, Module 4 do Module 5 xem, Module 5 do Module 1 xem.

### Bước 6. Sau khi Pull Request được merge
Nhóm trưởng merge bằng **Create a merge commit** (không dùng Squash) để giữ nguyên lịch sử commit từng người.

1. Chuyển về `main`, bấm **Pull**.
2. Nhấn F5: ứng dụng tự áp Migration mới (nếu có) và nạp dữ liệu mẫu còn thiếu.
3. Tạo nhánh mới cho việc tiếp theo.

### Những điều không được làm

- ❌ Không push thẳng lên `main`.
- ❌ Không `git push --force`.
- ❌ Không commit `bin/`, `obj/`, `.vs/`.
- ❌ Không sửa file ngoài module của mình.
- ❌ Không tự tạo Migration.
- ❌ Không dồn cuối kỳ mới commit một lần.

---

## 9. Thứ tự phụ thuộc giữa các module

Về **luồng dữ liệu**, các bảng có quan hệ theo chuỗi: Module 1 (tài khoản, chương trình) ➔ Module 2 (lớp) ➔ Module 3 (học viên, đăng ký) ➔ Module 4 (duyệt, học phí) ➔ Module 5 (lịch học, kết quả, thống kê). Nhờ dữ liệu mẫu có sẵn, mỗi người bắt đầu làm ngay mà không phải chờ người trước.

Về **việc phải bàn giao sớm**, xem bảng "Hợp đồng giữa các module" ở mục 6. Module 1 (đăng nhập, `PhanQuyen`) và Module 2 (`DanhSachPhanTrang`) cần xong sớm nhất vì cả nhóm dùng. Trong lúc chờ, cứ code và gắn `[PhanQuyen(...)]` sau khi Module 1 đã merge.

---

## 10. Lập trình một chức năng mới (luồng chuẩn)

```
[View .cshtml] ⇄ [Controller] ⇄ [Service (nghiệp vụ + LINQ)] ⇄ [KhoaHocDbContext]
                      ↑
               [ViewModel (Validation)]
```

**Bước 1. ViewModel** trong `ViewModels/<nhóm của bạn>/`, ví dụ `ViewModels/QuanLyLop/LopHocFormViewModel.cs`. Không truyền trực tiếp Entity ra View khi cần dữ liệu tổng hợp hoặc Validation riêng.

**Bước 2. Interface và Service** trong `Services/<nhóm của bạn>/`: `ILopHocService.cs` và `LopHocService.cs`. Mọi truy vấn EF Core/LINQ đặt trong Service, **không viết truy vấn trong Controller**. Phân trang và lọc phải làm trên truy vấn (`Skip`/`Take`), không tải hết dữ liệu rồi lọc bằng C# hay JavaScript.

**Bước 3. Đăng ký Service** trong `Program.cs`, **chỉ ở vùng module của bạn**, và thêm `using` tương ứng ở đầu file:

```csharp
using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Services.QuanLyLop;

// ===== MODULE 2 (Nguyễn Đình Kiên) =====
builder.Services.AddScoped<ILopHocService, LopHocService>();
```

**Bước 4. Controller** trong `Controllers/`: inject Service qua constructor, gắn `[PhanQuyen(...)]`, kiểm tra `ModelState.IsValid` ở action POST, thêm `[ValidateAntiForgeryToken]`. Ví dụ rút gọn (bỏ phần `namespace` và `using`):

```csharp
[PhanQuyen(LoaiVaiTro.GiaoVu, LoaiVaiTro.Admin)]
public class LopHocController : Controller
{
    private readonly ILopHocService _lopHocService;

    public LopHocController(ILopHocService lopHocService) => _lopHocService = lopHocService;

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LopHocFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await _lopHocService.TaoAsync(model);
        return RedirectToAction(nameof(Index));
    }
}
```

`PhanQuyen` nằm ở `QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Filters`, `LoaiVaiTro` ở `...Constants`.

**Bước 5. View** trong `Views/<TênController>/`. Đặt model đầu file bằng **tên đầy đủ**:

```razor
@model QuanLyKhoaHoc_UNETI4_DHTI17A1HN.ViewModels.QuanLyLop.LopHocFormViewModel
```

Dùng Tag Helper `asp-for`, `asp-action`, `asp-validation-for`. Không giữ nguyên View do scaffold sinh ra: ngày dùng `type="date"`, giờ buổi học dùng `datetime-local`, mô tả dùng `textarea`, chương trình và lớp dùng `select` lấy từ database, hiển thị tên thay cho mã khóa ngoại, nút thao tác chỉ hiện khi đúng trạng thái.

> **Khi dùng *Add ➔ New Scaffolded Item*:** scaffold xong, mở *Git Changes* kiểm tra. Nếu `Program.cs` hoặc `KhoaHocDbContext.cs` bị sửa thì **Undo** các file đó. Đây là nguồn conflict hay gặp nhất.

---

## 11. Làm việc với Session và phân quyền

### Trong Controller

```csharp
int? maTaiKhoan = HttpContext.Session.GetInt32(KhoaSession.MaTaiKhoan);
string? hoTen = HttpContext.Session.GetString(KhoaSession.HoTen);
string? vaiTro = HttpContext.Session.GetString(KhoaSession.VaiTro);

if (maTaiKhoan == null)
{
    return RedirectToAction("DangNhap", "TaiKhoan");
}
```

Khi Module 1 đã có `PhienDangNhapExtensions`, dùng `HttpContext.Session.LayMaTaiKhoan()`, `LayHoTen()`, `LayVaiTro()` cho gọn. Phần chặn người chưa đăng nhập hoặc sai vai trò đã có `[PhanQuyen]` lo.

> **Riêng cho Module 3 (Học viên):** tuyệt đối **không** dùng `id` từ URL để sửa hồ sơ hay xem đăng ký. Luôn lấy `MaTaiKhoan` từ Session, truy vấn sang `HocVien` tương ứng, rồi ràng buộc truy vấn theo chủ sở hữu (ví dụ `d.HocVien.MaTaiKhoan == maTaiKhoan`). Không khớp thì trả `NotFound` để không lộ dữ liệu người khác.

### Trong View (`.cshtml`)

```razor
@using Microsoft.AspNetCore.Http
@using QuanLyKhoaHoc_UNETI4_DHTI17A1HN.Constants

@{
    var vaiTro = Context.Session.GetString(KhoaSession.VaiTro);
    var hoTen = Context.Session.GetString(KhoaSession.HoTen);
}

@if (vaiTro == LoaiVaiTro.GiaoVu || vaiTro == LoaiVaiTro.Admin)
{
    <a asp-action="Create" class="btn btn-primary">Thêm lớp học</a>
}
```

Dòng `@using Microsoft.AspNetCore.Http` cần có thì View mới nhận ra `Context.Session.GetString`. Việc ẩn nút trên View chỉ để giao diện gọn; **quyền thật vẫn phải kiểm tra ở Controller**.

---

## 12. Khắc phục các lỗi thường gặp

| Lỗi | Nguyên nhân | Cách khắc phục |
|-----|-------------|----------------|
| Không kết nối được `(localdb)\MSSQLLocalDB` hoặc `Cannot open database` | LocalDB chưa chạy | Mở PowerShell: `sqllocaldb start MSSQLLocalDB`, rồi nhấn F5 lại |
| `'LopHoc' is a namespace but is used like a type` | Tên thư mục trong `Services/` hoặc `ViewModels/` trùng tên Entity | Đổi tên thư mục (`QuanLyLop`, `XetDuyet`, `LichHoc`...), không đặt trùng tên Entity nào |
| `The DELETE statement conflicted with the REFERENCE constraint` | Mọi khóa ngoại là `Restrict` để giữ dữ liệu lịch sử | Không xóa cứng. Đổi `TrangThai` (ví dụ `Đã hủy`, `Khóa`) |
| `PendingModelChangesWarning ... has pending changes` | Entity đã đổi nhưng chưa có Migration tương ứng | Báo nhóm trưởng tạo và commit Migration. Không tự tạo Migration |
| `The name 'X' is used by an existing migration` | Tên Migration trùng một Migration đã có | Dùng tên khác (việc của nhóm trưởng) |
| Lỗi `FOREIGN KEY constraint` khi chạy ứng dụng (lúc nạp dữ liệu mẫu) | Một file seed trỏ tới dòng chưa được nạp hoặc sai thứ tự | Báo nhóm trưởng kèm nguyên thông báo lỗi. Tạm thời xóa database rồi F5 |
| Không thấy dữ liệu mẫu mới, hoặc dữ liệu lạ sau khi pull | Seed chỉ nạp khi bảng còn trống; database trên máy còn dữ liệu cũ | Xóa database trong SQL Server Object Explorer rồi nhấn F5 |
| Merge conflict khi merge `origin/main` | Hai người sửa cùng vùng của một file | Mở *Merge Editor*, chọn phần đúng, lưu, commit. Không chắc thì nhắn nhóm trưởng |
| `Program.cs` hoặc `KhoaHocDbContext.cs` bị sửa sau khi scaffold | Scaffold tự thêm cấu hình | *Git Changes* ➔ **Undo** hai file đó |
| Session bị `null` sau khi chạy lại ứng dụng | Session lưu trong RAM, mất khi dừng ứng dụng | Đăng nhập lại |

---

## 13. Tài liệu tham khảo

- **Đề bài:** Hệ thống quản lý đăng ký khóa học và học viên trung tâm đào tạo, ASP.NET Core 10 MVC (file đề của môn học).
- **Tài liệu phân công công việc** của nhóm: việc chi tiết, mốc thời gian, tiêu chí hoàn thành và kịch bản kiểm thử của từng người.