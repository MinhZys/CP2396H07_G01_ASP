# Phân tích Cụm Chức năng và Quy trình Hệ thống

Tài liệu này phân tích các chức năng của hệ thống dựa trên `Requirements.md` và mô tả chi tiết quy trình hoạt động (Workflows).

## I. Phân Cụm Chức năng (Functional Clusters)

Hệ thống được chia thành 5 cụm chức năng chính có liên quan chặt chẽ với nhau:

### 1. Cụm Quản trị Hệ thống & Người dùng (System Administrator & IAM)
Cụm này xử lý việc xác thực và quản lý tài nguyên nền tảng.
*   **Chức năng:**
    *   Đăng ký / Đăng nhập / Quên mật khẩu (Tất cả người dùng).
    *   Quản lý hồ sơ cá nhân (Profile).
    *   **Admin:** Quản lý danh sách tài khoản (Tạo, Khóa, Sửa, Phân quyền cho Giảng viên/Học viên).
    *   **Admin:** Quản lý nội dung tĩnh (CMS): Cập nhật FAQ, Liên hệ, Thông tin Chi nhánh.

### 2. Cụm Tuyển sinh & Khóa học (Admissions & Course Catalog)
Cụm này tập trung vào quy trình đầu phễu: Từ lúc Khách xem thông tin đến lúc Đăng ký thi.
*   **Dữ liệu chính:** Khóa học, Chứng chỉ, Kỳ tuyển sinh.
*   **Chức năng:**
    *   **Admin:** Quản lý danh mục Khóa học & Chứng chỉ.
    *   **Admin:** Tạo và Cấu hình các **Kỳ thi tuyển sinh** (Thời gian, Lệ phí).
    *   **Khách/Học viên:** Xem danh sách khóa học, lịch thi tuyển sinh.
    *   **Khách/Học viên:** Đăng ký dự thi trực tuyến.

### 3. Cụm Đào tạo & Lớp học (Academic & Class Structure)
Đây là lõi của hệ thống, xử lý việc tổ chức giảng dạy sau khi đã có kết quả tuyển sinh.
*   **Dữ liệu chính:** Lớp học, Danh sách lớp, Tài liệu.
*   **Chức năng:**
    *   **Admin:** Tạo Lớp học mới.
    *   **Admin:** **Xếp lớp** (Chức năng quan trọng): Xếp học viên vào lớp dựa trên kết quả thi (Tự động hoặc Thủ công).
    *   **Admin:** Phân công giảng viên phụ trách lớp.
    *   **Giảng viên:** Xem lịch dạy, danh sách lớp, danh sách học viên.
    *   **Học viên:** Xem thông tin lớp mình đang học, lịch học.
    *   **Giảng viên/Học viên:** Quản lý Tài liệu (Upload/Download bài giảng).

### 4. Cụm Khảo thí & Điểm số (Examination & Grading)
Liên kết chặt chẽ với cụm Tuyển sinh (đầu vào) và Đào tạo (đầu ra).
*   **Chức năng:**
    *   **Giảng viên:** Chấm thi (Tuyển sinh & Cuối khóa).
    *   **Giảng viên:** Nhập điểm và Cập nhật điểm lên hệ thống.
    *   **Giảng viên:** Ghi nhận xét/Đánh giá học viên (Feedback).
    *   **Học viên:** Tra cứu/Xem kết quả thi.
    *   **Admin:** Thống kê báo cáo kết quả thi.

### 5. Cụm Tài chính (Finance & Payment)
Xử lý các vấn đề liên quan đến tiền bạc.
*   **Chức năng:**
    *   **Admin:** Thiết lập các loại phí (Học phí, Lệ phí thi, Phí thực hành/Lab).
    *   **Admin:** Xem báo cáo doanh thu.
    *   **Học viên:** Xác nhận nhập học (Đăng ký khóa học chính thức sau khi đỗ).
    *   **Học viên:** Thanh toán online (Học phí, Lệ phí).
    *   **Học viên:** Xem Lịch sử thanh toán.

---

## II. Quy trình Chi tiết (Detailed Workflows)
1. Cụm Quản trị Hệ thống & Người dùng (System Administrator & IAM)
Quy trình chính: Đăng ký, Đăng nhập và Phân quyền.

mermaid
graph TD
    A[Khách (Guest)] -->|Đăng ký Tài khoản| B(Hệ thống)
    B -->|Tạo User (Default Role)| C[Người dùng mới]
    C -->|Đăng nhập| D{Kiểm tra Role}
    D -->|Admin| E[Vào trang Admin]
    D -->|Instructor| F[Vào trang Giảng viên]
    D -->|Student/User| G[Vào trang Cá nhân]
    E -->|1. Tạo tài khoản Instructor| F
    E -->|2. Khóa/Mở khóa User| H[Trạng thái User]



Bước 1 - Đăng ký: Khách truy cập web, điền form đăng ký. Hệ thống tạo tài khoản với quyền mặc định (thường là User/Candidate).
Bước 2 - Phân quyền (Bởi Admin):
Admin tạo tài khoản đặc biệt cho Giảng viên (Instructor).
Admin có quyền khóa (Block) hoặc kích hoạt lại tài khoản nếu người dùng vi phạm.
Bước 3 - Quản trị nội dung: Admin cập nhật các trang tĩnh (FAQ, Liên hệ) để hiển thị cho Khách xem.




2. Cụm Tuyển sinh & Khóa học (Admissions & Course Catalog)
Quy trình chính: Tổ chức thi tuyển sinh đầu vào.

mermaid
graph LR
    Admin -->|1. Tạo Khóa học & Chứng chỉ| DB[(Database)]
    Admin -->|2. Tạo Kỳ tuyển sinh (Ngày, Lệ phí)| Exam[Kỳ tuyển sinh]
    Guest -->|3. Xem thông tin| Exam
    Guest -->|4. Đăng ký dự thi| Form[Form Đăng ký]
    Form -->|5. Lưu thông tin| Candidate[Hồ sơ Thí sinh]





Bước 1 - Chuẩn bị: Admin định nghĩa các Khóa học có trong trường và các Chứng chỉ tương ứng.
Bước 2 - Mở đợt tuyển sinh: Admin tạo các "Kỳ thi tuyển sinh" (Admission Exams), thiết lập ngày thi và lệ phí thi.
Bước 3 - Tiếp cận: Khách (Guest) xem danh sách các kỳ thi sắp mở.
Bước 4 - Ứng tuyển: Khách điền form đăng ký dự thi. Lúc này Khách trở thành Thí sinh (Candidate).



3. Cụm Khảo thí & Điểm số (Examination & Grading)
Quy trình chính: Chấm thi và Xử lý kết quả.


mermaid
graph TD
    Candidate[Thí sinh] -->|1. Tham gia thi| Test[Bài thi]
    Instructor[Giảng viên] -->|2. Chấm thi| Test
    Instructor -->|3. Nhập điểm| Server
    Server -->|4. Lưu kết quả| Result[Kết quả thi]
    Student[Học viên] -->|5. Tra cứu điểm| Result
    Admin -->|6. Xem báo cáo điểm| Report[Báo cáo Thống kê]


    
Bước 1 - Thi: Thí sinh tham gia kỳ thi (có thể là thi offline hoặc online tùy mô hình, nhưng kết quả sẽ được quản lý trên hệ thống).
Bước 2 - Chấm điểm: Giảng viên được phân công sẽ chấm bài và nhập điểm vào hệ thống.
Bước 3 - Công bố: Điểm số được lưu lại. Thí sinh có thể đăng nhập để xem mình Đậu hay Rớt.



4. Cụm Đào tạo & Lớp học (Academic & Class Structure)
Quy trình chính: Xếp lớp và Tổ chức giảng dạy (Diễn ra sau khi có điểm thi).

mermaid
graph TD
    Result[Kết quả thi ĐẬU] -->|1. Admin lọc danh sách| Admin
    Admin -->|2. Tạo Lớp học mới| Class[Lớp học]
    Admin -->|3. Xếp học viên vào lớp| Enrollment[Danh sách lớp]
    Admin -->|4. Phân công Giảng viên| Class
    Class -->|5. Kích hoạt Lớp| ActiveClass[Lớp Đang học]
    Instructor -->|6. Upload Tài liệu| ActiveClass
    Student -->|7. Tải tài liệu & Xem lịch| ActiveClass


Bước 1 - Xếp lớp: Admin dựa trên danh sách các Thí sinh đã ĐẬU -> Tạo lớp mới -> Gán thí sinh vào lớp.
Bước 2 - Phân công: Admin chọn Giảng viên phụ trách cho lớp đó.
Bước 3 - Vận hành:
Lớp bắt đầu hoạt động.
Giảng viên đăng tài liệu lên lớp.
Học viên (lúc này đã chính thức là Student) vào xem lịch học và tải tài liệu.



5. Cụm Tài chính (Finance & Payment)
Quy trình chính: Thu phí (Xen kẽ vào các quy trình trên).

mermaid
graph LR
    Admin -->|1. Cấu hình Học phí/Lệ phí| Settings
    Student -->|2. Xem công nợ| Bill[Hóa đơn]
    Student -->|3. Thanh toán (Online/Tiền mặt)| Payment
    Payment -->|4. Xác nhận thanh toán| System
    System -->|5. Cập nhật trạng thái| Status[Đã đóng tiền]
    Status -->|6. Cho phép nhập học| Permission[Quyền vào lớp]


    
Bước 1 - Quy định: Admin thiết lập giá tiền cho khóa học và lệ phí thi.
Bước 2 - Thanh toán:
Trước khi thi: Thí sinh đóng Lệ phí thi.
Sau khi Đậu & Xếp lớp: Học viên đóng Học phí + Phí thực hành (Lab).
Bước 3 - Kiểm soát: Hệ thống chỉ cho phép Học viên tham gia lớp học đầy đủ sau khi đã hoàn tất nghĩa vụ tài chính (hoặc theo chính sách của Admin).

---

## III. Danh Sách Chức Năng Đã Triển Khai Thực Tế (Implemented Features)

Dưới đây là danh sách các chức năng **đã có và đang hoạt động** trên hệ thống (dựa trên source code hiện tại):

### 1. Phân hệ Quản trị (Admin Module)
*   **Quản lý Khóa học (Course Management):**
    *   Thêm mới, Chỉnh sửa, Xóa khóa học.
    *   Quản lý danh sách Môn học (Subjects) và Bài học (Lessons).
*   **Quản lý Tuyển sinh (Entrance Exam Management):**
    *   Tạo lịch thi tuyển sinh (Exam Schedule).
    *   Xem danh sách hồ sơ đăng ký (`StudentRegistrations`).
    *   **Duyệt hồ sơ (Approve):** Chuyển trạng thái từ Pending -> Approved.
    *   **Từ chối hồ sơ (Reject):** Hủy đơn đăng ký.
*   **Quản lý Người dùng (User Management):**
    *   Xem danh sách toàn bộ tài khoản.
    *   Phân quyền (Admin/Instructor/Student).
*   **CMS (Content Management System):**
    *   Quản lý nội dung trang FAQ.
    *   Quản lý thông tin Trung tâm (Centers).

### 2. Phân hệ Công khai (Public Portal - Guest)
*   **Trang chủ:** Xem danh sách khóa học và thông tin giới thiệu.
*   **Đăng ký Tuyển sinh:** Form đăng ký trực tuyến (lưu thông tin vào bảng `StudentRegistrations`).
*   **Xác thực:** Đăng ký tài khoản, Đăng nhập.

### 3. Phân hệ Giảng viên (Instructor Area)
*   **Instructor Dashboard:** Trang tổng quan cơ bản.
*   **Quản lý Lớp học:** Xem danh sách lớp được phân công.
*   **Chấm điểm (Grading):** Nhập điểm cho học viên (Backend logic đã có).

### 4. Phân hệ Học viên (Student Area)
*   **Student Dashboard:** Trang chủ dành riêng cho học viên.
*   *(Các chức năng học tập chi tiết đang trong quá trình hoàn thiện)*
