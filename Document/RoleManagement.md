# Quy trình Quản lý Tài khoản và Phân quyền (Role Management)

Tài liệu này giải thích cơ chế tạo tài khoản và phân quyền cho người dùng trong hệ thống **Symphony Portal**, dựa trên code xử lý tại `Controllers/AccountController.cs` và cấu hình tại `Data/SeedData.cs`.

## 1. Các Vai trò (Roles) trong Hệ thống

Hệ thống hiện tại có 3 vai trò chính:

1.  **Admin (Quản trị viên)**
    *   **Quyền hạn:** Cao nhất. Truy cập được vào toàn bộ trang Quản trị (`/Admin/Dashboard`). Quản lý Khóa học, Kỳ thi, và Tài khoản người dùng khác.
    *   **Cách tạo:** Được tạo tự động khi khởi chạy ứng dụng (Seed Data). Không thể đăng ký từ bên ngoài.

2.  **Instructor (Giảng viên)**
    *   **Quyền hạn:** Truy cập khu vực Giảng viên (`/Instructor/Classes`). Xem danh sách lớp được phân công, chấm điểm.
    *   **Cách tạo:** **Không thể tự đăng ký.** Tài khoản Giảng viên phải do **Admin tạo** (thông qua chức năng Quản lý User trong Admin Dashboard - *tính năng này sẽ được phát triển tiếp*).

3.  **Student (Học viên)**
    *   **Quyền hạn:** Truy cập cổng thông tin học viên (`/Student/Home`). Đăng ký thi, xem kết quả, đóng học phí.
    *   **Cách tạo:** Có thể **tự đăng ký** công khai thông qua trang Đăng ký (`/Account/Register`).

---

## 2. Chi tiết Quy trình Đăng ký (Technical Workflow)

### A. Đối với Học viên (Student) - Tự Đăng ký
1.  Người dùng truy cập trang `/Account/Register`.
2.  Nhập thông tin: Email, Họ tên, Mật khẩu.
3.  Hệ thống xử lý (trong `AccountController.Register`):
    *   Tạo người dùng mới trong database.
    *   Tự động gán role **"Student"** cho tài khoản này.
    ```csharp
    // Code snippet từ AccountController.cs
    await _userManager.AddToRoleAsync(user, "Student");
    ```
4.  Người dùng đăng nhập và được chuyển hướng vào trang Student.

### B. Đối với Giảng viên (Instructor) - Admin Cấp phát
*Hiện tại hệ thống chặn việc tự đăng ký Instructor để đảm bảo bảo mật.*

Để cấp tài khoản cho Giảng viên mới, quy trình sẽ là:
1.  Admin đăng nhập vào hệ thống.
2.  Truy cập module **User Management** (Quản lý người dùng).
3.  Chọn "Create New User".
4.  Nhập thông tin Giảng viên và chọn Role là **"Instructor"**.
5.  Hệ thống gửi thông tin đăng nhập cho Giảng viên.

### C. Đối với Admin - Khởi tạo Mặc định
Khi chạy ứng dụng lần đầu tiên, hệ thống tự động kiểm tra và tạo tài khoản Admin mặc định nếu chưa có:
*   Email: `admin@symphony.local`
*   Password: `Admin@12345`

---
*Lưu ý: Việc phân chia này nhằm đảm bảo tính bảo mật và toàn vẹn dữ liệu, tránh trường hợp người lạ tự đăng ký làm Giảng viên hoặc Admin phá hoại hệ thống.*
