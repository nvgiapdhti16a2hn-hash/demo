# Quản lý khóa học và đăng ký học viên

Ứng dụng mẫu ASP.NET Core 10 MVC, Entity Framework Core 10 và SQL Server. Project này được tạo riêng trong thư mục `QuanLyKhoaHoc`; project Web API .NET 6 hiện có không bị sửa.

## Chạy ứng dụng

1. Cài .NET 10 SDK và SQL Server LocalDB (hoặc thay chuỗi kết nối trong `appsettings.json`).
2. Mở terminal tại thư mục `QuanLyKhoaHoc`.
3. Chạy:

   ```powershell
   dotnet restore
   dotnet run
   ```

Khi khởi động, ứng dụng áp dụng migration `InitialCreate`. Trong môi trường Development, ứng dụng tạo dữ liệu mẫu và ba tài khoản:

| Vai trò | Tên đăng nhập | Mật khẩu |
|---|---|---|
| Admin | `admin` | `Test123!` |
| Nhân viên đào tạo | `staff` | `Test123!` |
| Học viên | `student` | `Test123!` |

Tài khoản mẫu chỉ được tạo ở Development. Không sử dụng chúng trong môi trường triển khai.

## Chức năng đã dựng

- Session login/logout, phân quyền Admin / Nhân viên đào tạo / Học viên; mật khẩu được băm bằng `PasswordHasher`.
- Chương trình đào tạo và tài khoản: tạo, sửa, xem, khóa/mở khóa; không xóa chương trình còn lớp liên quan.
- Lớp học: CRUD, kiểm soát ngày đăng ký và sĩ số; danh sách công khai có tìm kiếm, lọc kết hợp, sắp xếp và phân trang trên EF Core.
- Học viên: đăng ký tài khoản, tự cập nhật hồ sơ, xem danh sách hồ sơ thuộc chính mình, nộp/hủy hồ sơ khi trạng thái cho phép.
- Hồ sơ: nhân viên xét duyệt theo luồng trạng thái; lịch kiểm tra có kiểm tra thời gian kết thúc và lịch trùng của học viên/giáo viên.
- Kết quả xếp lớp: chỉ ghi nhận sau khi hoàn thành lịch kiểm tra; kiểm soát sĩ số bằng truy vấn LINQ.
- Dashboard và một số thống kê nhóm theo chương trình, lớp, trạng thái và tháng.

## Migration

Migration ban đầu và model snapshot được lưu trong thư mục `Migrations`. Để tạo migration khi thay đổi Entity:

```powershell
dotnet tool install --global dotnet-ef --version 10.*
dotnet ef migrations add TenMigrationMoi
dotnet ef database update
```
