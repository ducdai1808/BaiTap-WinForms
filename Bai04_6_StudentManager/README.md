# Bài 4.6 - Quản lý sinh viên

## Yêu cầu
- Windows và Visual Studio 2022 có workload **.NET desktop development**.
- .NET 8 SDK (Windows).
- Project dùng Windows Forms, target `net8.0-windows`.

## Cách mở và chạy
1. Giải nén toàn bộ file ZIP vào một thư mục.
2. Mở file `StudentManager.csproj` bằng Visual Studio.
3. Chờ Visual Studio tải project.
4. Nhấp phải project `StudentManager` trong Solution Explorer -> **Set as Startup Project**.
5. Chọn **Build -> Build Solution** hoặc nhấn `Ctrl + Shift + B`.
6. Mở **View -> Output**, chọn **Build** để xem thông báo. Nếu báo `Build succeeded`, nhấn `F5`.
7. Nếu báo thiếu .NET 8 SDK, cài .NET 8 SDK hoặc mở Visual Studio Installer và cài workload **.NET desktop development**.

## Chức năng
- Hiển thị danh sách sinh viên bằng DataGridView.
- BindingList<Student> và BindingSource.
- Chọn dòng để hiện thông tin lên các ô nhập.
- Thêm, cập nhật, xóa sinh viên.
- Chọn ảnh JPG/JPEG/PNG/BMP và hiện ảnh đại diện.
- Xuất danh sách ra CSV UTF-8.

## Lưu ý
Giao diện được tạo hoàn toàn bằng code trong `Form1.cs`, không cần kéo thả Designer. Không chép riêng `Form1.cs` vào một project khác; hãy mở file `.csproj` đi kèm ZIP để tránh xung đột.
