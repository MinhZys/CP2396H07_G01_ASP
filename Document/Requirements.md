# Tài liệu Yêu cầu Hệ thống: Cổng thông tin Đào tạo Symphony Ltd.

## 1. Tổng quan & Mục tiêu

**Đề tài:** Xây dựng cổng thông tin trực tuyến (Web Portal) cho học viện đào tạo CNTT Symphony Ltd.

**Mục tiêu cốt lõi:**
*   Số hóa toàn bộ quy trình: Tuyển sinh – Học tập – Quản lý thông tin.
*   Quảng bá khóa học & chứng chỉ.
*   Hỗ trợ quản lý thi đầu vào, xếp lớp, quản lý học viên và học phí.
*   Cung cấp kênh thông tin chính thức (FAQ, lịch trình, kết quả, liên hệ).
*   Thay thế quy trình thủ công hiện tại (tờ rơi, quảng cáo TV, tư vấn trực tiếp).

## 2. Các nhóm người dùng (Actors)

1.  **Admin (Quản trị viên):** Quản lý toàn hệ thống, cấu hình và giám sát hoạt động.
2.  **Giảng viên (Instructor):** Chịu trách nhiệm chuyên môn, chấm thi, đánh giá học viên và quản lý tài liệu.
3.  **Học viên / Thí sinh (Student/Candidate):** Người sử dụng dịch vụ đào tạo, đăng ký thi, học tập và thanh toán.
4.  **Khách chưa đăng nhập (Guest/Visitor):** Khách vãng lai tìm hiểu thông tin và đăng ký tham gia.

---

## 3. Chức năng Hệ thống (Phân quyền chi tiết)

### A. Phân hệ Quản trị viên (Admin) - Quản lý toàn hệ thống

1.  **Quản lý Tài khoản & Phân quyền:**
    *   Đăng nhập hệ thống quản trị.
    *   Quản lý tài khoản người dùng (tạo mới/khóa/sửa): Giảng viên, Học viên.

2.  **Quản lý Khóa học & Tuyển sinh:**
    *   **Khóa học & Chứng chỉ:** Quản lý thông tin khóa học, chứng chỉ cấp phát.
    *   **Kỳ tuyển sinh:** Tạo và quản lý thời gian, trạng thái các kỳ thi tuyển sinh.
    *   **Thiết lập chi phí:** Cập nhật học phí, lệ phí thi, phí thực hành.

3.  **Quản lý Lớp học & Xếp lớp:**
    *   Quản lý danh sách lớp học.
    *   **Phân công giảng viên:** Gán giảng viên phụ trách cho từng lớp.
    *   **Xếp lớp học viên:** Tự động hoặc thủ công xếp học viên vào lớp dựa trên điểm thi đầu vào.

4.  **Quản lý Nội dung Website (CMS):**
    *   Cập nhật trang Giới thiệu, FAQ, Liên hệ.
    *   Quản lý thông tin Chi nhánh.

5.  **Thống kê - Báo cáo:**
    *   Xem các báo cáo tổng hợp về học viên, doanh thu, kết quả thi.

### B. Phân hệ Giảng viên (Instructor) - Chuyên môn & Đánh giá

1.  **Quản lý Giảng dạy:**
    *   Đăng nhập hệ thống.
    *   Xem danh sách các lớp được phân công.
    *   Xem danh sách học viên trong từng lớp.

2.  **Đánh giá & Chấm thi:**
    *   **Chấm bài thi:** Chấm thi tuyển sinh đầu vào và thi cuối khóa.
    *   **Nhập điểm:** Cập nhật điểm số lên hệ thống.
    *   **Đánh giá học viên:** Ghi nhận xét, đánh giá quá trình học tập.

3.  **Quản lý Tài liệu:**
    *   Upload tài liệu học tập (bài giảng, bài tập) cho lớp mình phụ trách.

### C. Phân hệ Học viên / Thí sinh (Student/Candidate)

1.  **Tài khoản & Tuyển sinh:**
    *   Đăng ký tài khoản / Đăng nhập.
    *   Đăng ký dự thi tuyển sinh trực tuyến.
    *   Xem thông tin khóa học và các kỳ tuyển sinh đang mở.

2.  **Học tập & Kết quả:**
    *   **Xem kết quả thi:** Tra cứu điểm thi tuyển sinh/cuối khóa.
    *   **Lớp học:** Xem thông tin lớp được xếp, lịch học, lịch thi.
    *   **Tài liệu:** Tải và xem tài liệu học tập do giảng viên chia sẻ.

3.  **Tài chính & Phản hồi:**
    *   **Đăng ký khóa học:** Xác nhận nhập học.
    *   **Thanh toán:** Thực hiện thanh toán học phí/lệ phí và xem lịch sử thanh toán.
    *   **Hỗ trợ:** Gửi câu hỏi hoặc phản hồi đến nhà trường.

### D. Phân hệ Khách (Guest/Visitor)

*   Xem Trang chủ (Thông tin nổi bật).
*   Xem Danh sách Khóa học & Chứng chỉ.
*   Xem Thông tin Kỳ tuyển sinh (Lịch, lệ phí).
*   Xem FAQ và Thông tin Liên hệ / Chi nhánh.
*   **Hành động chính:** Đăng ký tài khoản mới hoặc Đăng ký dự thi ngay.

---

## 4. Quy tắc Nghiệp vụ & Tài chính

1.  **Học phí & Lệ phí:**
    *   Do Admin quy định, có thể thay đổi theo thời điểm.
    *   **Phí thực hành (Lab Fee):** $1000 (Đăng ký sớm) / $1500 (Bình thường).
    *   **Phí thi lại:** $1500.

2.  **Quy trình:**
    *   **Admin** tạo kỳ thi -> **Khách** đăng ký thi -> **Giảng viên** chấm điểm -> **Admin** xếp lớp & công bố -> **Học viên** xem kết quả & đóng tiền -> **Học viên** vào học.

---

## 5. Yêu cầu Kỹ thuật Phi chức năng

*   **Kiến trúc:** Web Portal (Frontend Public + Backend/Admin Panel).
*   **Cơ sở dữ liệu:** Lưu trữ quan hệ (Khóa học, Thí sinh, Kết quả, Đăng ký, Tài chính...).
*   **Bảo mật:** Phân quyền chặt chẽ (Admin / Instructor / Student).
*   **Tương thích:** Hoạt động tốt trên các trình duyệt phổ biến.

---

## 6. Tóm tắt Đề tài

Xây dựng cổng thông tin đào tạo CNTT cho học viện Symphony Ltd. với 4 nhóm người dùng chính.
  **Admin** quản lý toàn bộ hệ thống, xếp lớp và tài chính. 
  **Giảng viên** phụ trách chuyên môn, chấm thi và cung cấp tài liệu.
  **Học viên** tham gia thi tuyển, theo dõi kết quả, lịch học và thanh toán học phí. 
  **Khách** timg hiểu thông tin. Hệ thống nhằm mục đích số hóa quy trình tuyển sinh và đào tạo, tăng tính minh bạch và hiệu quả quản lý.
