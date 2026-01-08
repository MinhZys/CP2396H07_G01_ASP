# Phân Tích Use Case - Symphony Ltd. Portal

## 1. Tổng quan các Actors

Hệ thống có 4 tác nhân chính (Actors):
1.  **Admin (Quản trị viên):** Người quản lý toàn bộ hệ thống.
2.  **Instructor (Giảng viên):** Người chấm thi và quản lý lớp học chuyên môn.
3.  **Student (Học viên/Thí sinh):** Người đăng ký thi và tham gia học.
4.  **Guest (Khách):** Người xem thông tin công khai.

---

## 2. Biểu đồ Use Case Tổng quát (Mermaid)
flowchart LR

%% ===== ACTORS =====
Guest["👤 Khách"]
Student["👤 Học viên"]
Instructor["👤 Giảng viên"]
Admin["👤 Quản trị viên"]

%% ===== PUBLIC =====
subgraph Public["Website Public"]
    direction TB
    UC_ViewInfo("Trang chủ & Giới thiệu")
    UC_ViewCourses("Danh sách khóa học")
    UC_ViewEntranceExam("Lịch tuyển sinh")
    UC_Register("Đăng ký")
    UC_Login("Đăng nhập")
end

%% ===== STUDENT =====
subgraph StudentModule["Học viên"]
    direction TB
    UC_RegisterExam("ĐK dự thi")
    UC_ViewResult("Xem kết quả")
    UC_EnrollClass("Nhập học")
    UC_Pay("Thanh toán")
    UC_ViewMaterials("Lịch học & Tài liệu")
end

%% ===== INSTRUCTOR =====
subgraph InstructorModule["Giảng viên"]
    direction TB
    UC_ViewMyClasses("Lớp được phân")
    UC_Grade("Chấm điểm")
    UC_UploadDoc("Upload tài liệu")
end

%% ===== ADMIN =====
subgraph AdminModule["Quản trị"]
    direction TB
    UC_ManageUsers("QL Người dùng")
    UC_ManageCourses("QL Khóa học")
    UC_ManageExams("QL Kỳ tuyển sinh")
    UC_AssignClass("Xếp lớp & GV")
    UC_Report("Báo cáo")
    UC_CMS("CMS")
end

%% ===== RELATIONS =====
Guest --> UC_ViewInfo
Guest --> UC_ViewCourses
Guest --> UC_ViewEntranceExam
Guest --> UC_Register
Guest --> UC_Login

Student --> UC_Login
Student --> UC_RegisterExam
Student --> UC_ViewResult
Student --> UC_EnrollClass
Student --> UC_Pay
Student --> UC_ViewMaterials

Instructor --> UC_Login
Instructor --> UC_ViewMyClasses
Instructor --> UC_Grade
Instructor --> UC_UploadDoc

Admin --> UC_Login
Admin --> UC_ManageUsers
Admin --> UC_ManageCourses
Admin --> UC_ManageExams
Admin --> UC_AssignClass
Admin --> UC_Report
Admin --> UC_CMS


## 3. Danh sách Use Case Chi tiết

### 3.1. Nhóm Guest (Khách)
*   **UC01 - Xem thông tin:** Xem thông tin giới thiệu, chi nhánh, FAQ.
*   **UC02 - Tra cứu khóa học:** Xem danh sách và chi tiết các khóa học, chứng chỉ.
*   **UC03 - Đăng ký tài khoản:** Tạo tài khoản mới để trở thành Thí sinh.

### 3.2. Nhóm Student (Học viên / Thí sinh)
*   **UC04 - Đăng ký thi tuyển sinh:** Chọn kỳ thi, điền form đăng ký.
*   **UC05 - Tra cứu kết quả:** Xem điểm thi và xếp loại (Học cơ bản hay vào thẳng chứng chỉ).
*   **UC06 - Đăng ký lớp học:** Chọn lớp học được hệ thống đề xuất dựa trên kết quả thi.
*   **UC07 - Thanh toán:** Thanh toán phí thi, học phí (Tiền mặt/Séc/Online).
*   **UC08 - Xem lịch học:** Xem thời khóa biểu và phòng học.

### 3.3. Nhóm Instructor (Giảng viên)
*   **UC09 - Xem lớp phụ trách:** Xem danh sách các lớp được Admin phân công.
*   **UC10 - Quản lý học viên:** Xem danh sách học viên trong lớp.
*   **UC11 - Chấm thi:** Nhập điểm thi tuyển sinh (được phân công chấm) hoặc điểm thi cuối khóa.
*   **UC12 - Quản lý tài liệu:** Upload/Cập nhật tài liệu cho lớp học.

### 3.4. Nhóm Admin (Quản trị viên)
*   **UC13 - Quản lý Khóa học:** Thêm/Sửa/Xóa khóa học, chứng chỉ. Thiết lập giá, thời lượng.
*   **UC14 - Quản lý Kỳ tuyển sinh:** Tạo kỳ thi mới, thiết lập lệ phí, ngày thi.
*   **UC15 - Xếp lớp (Scheduling):**
    *   Tạo lớp học mới.
    *   Phân công giảng viên.
    *   Xếp học viên vào lớp (dựa trên kết quả thi).
*   **UC16 - Quản lý Tài chính:** Xác nhận thanh toán, xuất biên lai, xem doanh thu.
*   **UC17 - CMS:** Quản lý bài viết, FAQ, thông tin chi nhánh.
