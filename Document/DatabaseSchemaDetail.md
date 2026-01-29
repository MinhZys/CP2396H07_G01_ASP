# Database Schema (SQL Server/MSSQL)

Dưới đây là schema thực tế đang được triển khai trong Code (Entity Framework Core), sử dụng **SQL Server** (`mssqllocaldb`).

## Chi tiết và Công dụng các Bảng dữ liệu

Hệ thống bao gồm 41 bảng dữ liệu, được chia thành các nhóm chức năng chính như sau:

### 1. Nhóm Xác thực & Hồ sơ (Auth & Profiles)
*   **`Roles`**: Lưu trữ các vai trò trong hệ thống (như Admin, Instructor, Student). Giúp phân quyền truy cập chức năng.
*   **`Users`**: Bảng người dùng chính, chứa thông tin đăng nhập (Email, Password) và liên kết với vai trò (`RoleId`).
*   **`StudentProfiles`**: Chứa thông tin cá nhân chi tiết của Học viên (Ngày sinh, giới tính, số điện thoại, địa chỉ, ảnh đại diện).
*   **`InstructorProfiles`**: Chứa thông tin chuyên sâu của Giảng viên (Số năm kinh nghiệm, chuyên môn, tiểu sử, chứng chỉ, link Github).

### 2. Nhóm Đào tạo & Khóa học (Education Core)
*   **`Categories`**: Danh mục khóa học (ví dụ: Lập trình Web, Mobile, Data Science).
*   **`Certificates`**: Các loại chứng chỉ sẽ được cấp sau khi hoàn thành khóa học.
*   **`Courses`**: Thông tin chính về các khóa học (Tiêu đề, học phí, thời lượng, cấp độ).
*   **`Subjects`**: Các môn học cụ thể (ví dụ: HTML, C#, SQL).
*   **`CourseSubjects`**: Bảng trung gian (nhiều-nhiều) kết nối Môn học vào các Khóa học.
*   **`CourseInstructors`**: Bảng trung gian kết nối Giảng viên với các Khóa học mà họ có thể dạy.

### 3. Nhóm Quản lý Lớp học & Bài giảng (Classes & Lessons)
*   **`ClassCategories`**: Loại hình lớp học (ví dụ: Lớp lý thuyết, Lớp thực hành).
*   **`Classes`**: Thông tin các lớp học thực tế đang mở (Tên lớp, số chỗ ngồi, trạng thái).
*   **`Enrollments`**: Quản lý việc đăng ký lớp học của học viên (Trạng thái đóng tiền, đã duyệt chưa, đã hoàn thành khóa học chưa).
*   **`Lessons`**: Các bài giảng trong hệ thống, thuộc về một Khóa học và Môn học nhất định.
*   **`ClassLessons`**: Quản lý việc bài giảng nào được dạy trong lớp học nào.
*   **`Materials`**: Lưu trữ tài liệu học tập (File PDF, Video, Slide) cho từng lớp học.

### 4. Nhóm Đánh giá & Quiz (Quizzes)
*   **`Quizzes`**: Các bài kiểm tra ngắn (trắc nghiệm) đính kèm sau mỗi bài giảng.
*   **`QuizQuestions`**: Các câu hỏi trong bài Quiz (nội dung câu hỏi, các lựa chọn A, B, C, D và đáp án đúng).

### 5. Hệ thống Thi tuyển đầu vào (Entrance Exam System)
*   **`ExamPapers`**: Đề thi tuyển sinh (Tiêu đề, thời lượng làm bài).
*   **`Questions`**: Ngân hàng câu hỏi cho đề thi (nhiều dạng: trắc nghiệm, tự luận).
*   **`QuestionOptions`**: Các lựa chọn trả lời cho câu hỏi trắc nghiệm.
*   **`ExamPaperQuestions`**: Kết nối câu hỏi vào đề thi và quy định thứ tự xuất hiện.
*   **`EntranceExams`**: Các kỳ thi tuyển sinh được tổ chức (Ngày thi, lệ phí, số lượng thí sinh tối đa).
*   **`StudentRegistrations`**: Thông tin thí sinh đăng ký dự thi.
*   **`ExamDetails`**: Chi tiết ca thi của thí sinh (Phòng thi, giờ thi cụ thể).
*   **`StudentExamSessions`**: Lưu lại phiên làm bài thi thực tế của thí sinh (Thời gian bắt đầu/kết thúc, tổng điểm).
*   **`StudentAnswers`**: Lưu lại các câu trả lời chi tiết của thí sinh trong bài thi để chấm điểm.
*   **`ExamResults`**: Kết quả cuối cùng của kỳ thi (Đạt hay không đạt).

### 6. Nhóm Tài chính (Payments)
*   **`Payments`**: Quản lý các giao dịch thanh toán (Học phí, phí dự thi, phí ôn tập).
*   **`VNPayTransactions`**: Lưu vết chi tiết các giao dịch thực hiện qua cổng thanh toán VNPay.

### 7. Nhóm CMS & Hỗ trợ (CMS & Support)
*   **`Centers`**: Danh sách các cơ sở/trung tâm đào tạo (Địa chỉ, số điện thoại).
*   **`PageContents`**: Quản lý nội dung các trang tĩnh trên website (Trang giới thiệu, trang môn học).
*   **`FAQs`**: Các câu hỏi thường gặp và câu trả lời để hỗ trợ người dùng.
*   **`CourseReviews`**: Đánh giá và nhận xét của học viên về khóa học.

### 8. Nhóm Trao đổi & Thông báo (Communication)
*   **`ChatMessages`**: Lưu lịch sử chat giữa người dùng với nhau hoặc giữa khách với Admin.
*   **`Notifications`**: Các thông báo gửi đến người dùng (Thông báo đăng ký thành công, nhắc lịch thi).

### 9. Nhóm Giao nhiệm vụ (Assignments)
*   **`Assignments`**: Dùng để phân công Giảng viên phụ trách các lớp học hoặc nhiệm vụ nhất định.
*   **`ClassAssignments`**: Dùng để gán Học viên vào một lớp học cụ thể sau khi đã hoàn tất thủ tục đăng ký.

### 10. Nhóm Khách hàng & Ôn thi (Guests & Revision)
*   **`Guests`**: Lưu thông tin khách vãng lai khi họ cần tư vấn hoặc đăng ký thi trực tiếp mà chưa có tài khoản.
*   **`RevisionPackages`**: Các gói ôn tập kiến thức trước kỳ thi (Tên gói, lệ phí).
*   **`RevisionRegistrations`**: Thông tin thí sinh đăng ký mua gói ôn tập.

---

```sql
-- 1. ROLES
CREATE TABLE [Roles] (
    [Id] nvarchar(36) NOT NULL,
    [Name] nvarchar(max) NOT NULL, 
    [Description] nvarchar(max) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

-- 2. USERS
CREATE TABLE [Users] (
    [Id] nvarchar(36) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [RoleId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);

-- 3. PROFILES
CREATE TABLE [StudentProfiles] (
    [Id] nvarchar(36) NOT NULL,
    [UserId] nvarchar(36) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [Gender] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [AddressLine] nvarchar(max) NULL,
    [AvatarUrl] nvarchar(max) NULL,
    CONSTRAINT [PK_StudentProfiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [InstructorProfiles] (
    [Id] nvarchar(36) NOT NULL,
    [UserId] nvarchar(36) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [Gender] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [AddressLine] nvarchar(max) NULL,
    [AvatarUrl] nvarchar(max) NULL,
    [YearsOfExperience] int NOT NULL,
    [Specialization] nvarchar(max) NULL,
    [Bio] nvarchar(max) NULL,
    [Certifications] nvarchar(max) NULL,
    [GithubUrl] nvarchar(max) NULL,
    CONSTRAINT [PK_InstructorProfiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InstructorProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

-- 4. CATEGORIES
CREATE TABLE [Categories] (
    [Id] nvarchar(36) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);

-- 5. CERTIFICATES
CREATE TABLE [Certificates] (
    [Id] nvarchar(36) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Certificates] PRIMARY KEY ([Id])
);

-- 6. COURSES
CREATE TABLE [Courses] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [TuitionFee] decimal(18,2) NOT NULL,
    [DurationMonths] int NOT NULL,
    [CertificateId] nvarchar(36) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [Image] nvarchar(max) NULL,
    [Level] nvarchar(max) NOT NULL, -- Enum
    [CategoryId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Courses_Certificates_CertificateId] FOREIGN KEY ([CertificateId]) REFERENCES [Certificates] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Courses_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE
);

-- 7. SUBJECTS
CREATE TABLE [Subjects] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [StudyTime] int NOT NULL,
    [Description] nvarchar(max) NULL,
    [LearningRoadmap] nvarchar(max) NULL,
    [Image] nvarchar(max) NULL,
    CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id])
);

-- 8. COURSE RELATED (Many-to-Many)
CREATE TABLE [CourseSubjects] (
    [CourseId] nvarchar(36) NOT NULL,
    [SubjectId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_CourseSubjects] PRIMARY KEY ([CourseId], [SubjectId]),
    CONSTRAINT [FK_CourseSubjects_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseSubjects_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [CourseInstructors] (
    [CourseId] nvarchar(36) NOT NULL,
    [InstructorId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_CourseInstructors] PRIMARY KEY ([CourseId], [InstructorId]),
    CONSTRAINT [FK_CourseInstructors_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseInstructors_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

-- 9. CLASSES
CREATE TABLE [ClassCategories] (
    [Id] nvarchar(36) NOT NULL, 
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_ClassCategories] PRIMARY KEY ([Id])
);

CREATE TABLE [Classes] (
    [Id] nvarchar(36) NOT NULL,
    [ClassName] nvarchar(max) NOT NULL,
    [ClassCategoryId] nvarchar(36) NOT NULL,
    [NumberOfSeats] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Classes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Classes_ClassCategories_ClassCategoryId] FOREIGN KEY ([ClassCategoryId]) REFERENCES [ClassCategories] ([Id])
);

-- 10. ENROLLMENTS
CREATE TABLE [Enrollments] (
    [Id] nvarchar(36) NOT NULL,
    [ClassId] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [CourseId] nvarchar(36) NULL,
    [EnrolledDate] datetime2 NOT NULL,
    [IsApproved] bit NOT NULL,
    [IsPaid] bit NOT NULL,
    [IsCompleted] bit NOT NULL DEFAULT 0,
    [PaymentReference] nvarchar(max) NULL,
    CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Enrollments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Enrollments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Enrollments_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id])
);

-- 11. LESSONS
CREATE TABLE [Lessons] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [ContentLink] nvarchar(max) NULL,
    [Image] nvarchar(max) NULL,
    [DurationMinutes] int NOT NULL,
    [CourseId] nvarchar(36) NOT NULL,
    [SubjectId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Lessons] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Lessons_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Lessons_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ClassLessons] (
    [Id] nvarchar(36) NOT NULL,
    [ClassId] nvarchar(36) NOT NULL,
    [LessonId] nvarchar(36) NOT NULL,
    [AssignedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ClassLessons] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClassLessons_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ClassLessons_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([Id]) ON DELETE CASCADE
);

-- 12. QUIZZES
CREATE TABLE [Quizzes] (
    [Id] nvarchar(36) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [PassScore] int NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    [LessonId] nvarchar(36) NULL,
    CONSTRAINT [PK_Quizzes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Quizzes_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([Id])
);

CREATE TABLE [QuizQuestions] (
    [Id] nvarchar(36) NOT NULL,
    [QuestionText] nvarchar(max) NOT NULL,
    [OptionA] nvarchar(max) NULL,
    [OptionB] nvarchar(max) NULL,
    [OptionC] nvarchar(max) NULL,
    [OptionD] nvarchar(max) NULL,
    [CorrectOption] nvarchar(max) NOT NULL,
    [Points] int NOT NULL,
    [QuizId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_QuizQuestions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_QuizQuestions_Quizzes_QuizId] FOREIGN KEY ([QuizId]) REFERENCES [Quizzes] ([Id]) ON DELETE CASCADE
);

-- 13. EXAMS & REGISTRATIONS
CREATE TABLE [ExamPapers] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Duration] int NOT NULL,
    [SubjectId] nvarchar(450) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ExamPapers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamPapers_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id])
);

CREATE TABLE [Questions] (
    [Id] nvarchar(36) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [Type] int NOT NULL, -- Enum QuestionType
    [SubjectId] nvarchar(450) NULL,
    [Difficulty] nvarchar(max) NULL,
    [Score] float NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Questions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Questions_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id])
);

CREATE TABLE [QuestionOptions] (
    [Id] nvarchar(36) NOT NULL,
    [QuestionId] nvarchar(36) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [IsCorrect] bit NOT NULL,
    CONSTRAINT [PK_QuestionOptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_QuestionOptions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ExamPaperQuestions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ExamPaperId] nvarchar(36) NOT NULL,
    [QuestionId] nvarchar(36) NOT NULL,
    [Order] int NOT NULL,
    CONSTRAINT [PK_ExamPaperQuestions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamPaperQuestions_ExamPapers_ExamPaperId] FOREIGN KEY ([ExamPaperId]) REFERENCES [ExamPapers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ExamPaperQuestions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [EntranceExams] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [ExamDate] datetime2 NOT NULL,
    [Fee] decimal(18,2) NOT NULL,
    [MaxCandidates] int NOT NULL,
    [Status] int NOT NULL, -- Enum ExamStatus
    [IsRegistrationOpen] bit NOT NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [Subjects] nvarchar(max) NULL,
    [ExamPaperId] nvarchar(36) NULL,
    CONSTRAINT [PK_EntranceExams] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EntranceExams_ExamPapers_ExamPaperId] FOREIGN KEY ([ExamPaperId]) REFERENCES [ExamPapers] ([Id])
);

CREATE TABLE [StudentRegistrations] (
    [Id] nvarchar(36) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Gender] int NOT NULL, -- Enum
    [DateOfBirth] datetime2 NOT NULL,
    [Phone] nvarchar(max) NULL,
    [CourseId] nvarchar(36) NOT NULL,
    [CenterId] nvarchar(36) NOT NULL,
    [HasExtraPractice] bit NOT NULL,
    [RegisteredAt] datetime2 NOT NULL,
    [Status] int NOT NULL, -- Enum
    CONSTRAINT [PK_StudentRegistrations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentRegistrations_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentRegistrations_Centers_CenterId] FOREIGN KEY ([CenterId]) REFERENCES [Centers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ExamDetails] (
    [Id] nvarchar(36) NOT NULL,
    [RegistrationId] nvarchar(36) NOT NULL,
    [ExamTime] datetime2 NOT NULL,
    [ExamRoom] nvarchar(max) NULL,
    [ExamDescription] nvarchar(max) NULL,
    CONSTRAINT [PK_ExamDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamDetails_StudentRegistrations_RegistrationId] FOREIGN KEY ([RegistrationId]) REFERENCES [StudentRegistrations] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [StudentExamSessions] (
    [Id] nvarchar(36) NOT NULL,
    [EntranceExamId] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [ExamPaperId] nvarchar(36) NOT NULL,
    [StartTime] datetime2 NULL,
    [EndTime] datetime2 NULL,
    [TotalScore] float NOT NULL,
    [GradeLevel] nvarchar(max) NULL,
    [Status] int NOT NULL, -- Enum ExamSessionStatus
    CONSTRAINT [PK_StudentExamSessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentExamSessions_EntranceExams_EntranceExamId] FOREIGN KEY ([EntranceExamId]) REFERENCES [EntranceExams] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentExamSessions_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentExamSessions_ExamPapers_ExamPaperId] FOREIGN KEY ([ExamPaperId]) REFERENCES [ExamPapers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [StudentAnswers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SessionId] nvarchar(36) NOT NULL,
    [QuestionId] nvarchar(36) NOT NULL,
    [SelectedOptionId] nvarchar(max) NULL,
    [EssayContent] nvarchar(max) NULL,
    [EarnedScore] float NOT NULL,
    [IsGraded] bit NOT NULL,
    [ExaminerNote] nvarchar(max) NULL,
    CONSTRAINT [PK_StudentAnswers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentAnswers_StudentExamSessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [StudentExamSessions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentAnswers_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ExamResults] (
    [Id] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [EntranceExamId] nvarchar(36) NOT NULL,
    [Score] float NOT NULL,
    [IsPassed] bit NOT NULL,
    [ExamDate] datetime2 NOT NULL,
    CONSTRAINT [PK_ExamResults] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamResults_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ExamResults_EntranceExams_EntranceExamId] FOREIGN KEY ([EntranceExamId]) REFERENCES [EntranceExams] ([Id]) ON DELETE CASCADE
);

-- 14. PAYMENTS & TRANSACTIONS
CREATE TABLE [Payments] (
    [Id] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NULL,
    [GuestId] nvarchar(36) NULL,
    [Amount] decimal(18,2) NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL, -- Conversion to String
    [PaymentDate] datetime2 NOT NULL,
    [ReceiptNumber] nvarchar(max) NULL,
    [Status] int NOT NULL, -- Enum
    [Purpose] int NOT NULL DEFAULT 0, -- Enum PaymentPurpose
    [CourseId] nvarchar(36) NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Payments_Guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [Guests] ([Id])
);

CREATE TABLE [VNPayTransactions] (
    [Id] nvarchar(450) NOT NULL,
    [PaymentId] nvarchar(36) NOT NULL,
    [VnpTxnRef] nvarchar(max) NOT NULL,
    [VnpAmount] bigint NOT NULL,
    [VnpOrderInfo] nvarchar(max) NOT NULL,
    [VnpCreateDate] nvarchar(max) NOT NULL,
    [VnpResponseCode] nvarchar(max) NULL,
    [VnpTransactionNo] nvarchar(max) NULL,
    [VnpBankCode] nvarchar(max) NULL,
    [VnpPayDate] nvarchar(max) NULL,
    [Status] int NOT NULL, -- Enum VNPayTransactionStatus
    CONSTRAINT [PK_VNPayTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VNPayTransactions_Payments_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payments] ([Id]) ON DELETE CASCADE
);

-- 15. CMS CONTENT
CREATE TABLE [Centers] (
    [Id] nvarchar(36) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NULL,
    [Phone] nvarchar(max) NULL,
    CONSTRAINT [PK_Centers] PRIMARY KEY ([Id])
);

CREATE TABLE [PageContents] (
    [Id] nvarchar(36) NOT NULL,
    [Slug] nvarchar(50) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [LastUpdated] datetime2 NOT NULL,
    [SubjectId] nvarchar(450) NULL,
    [CenterId] nvarchar(36) NULL,
    CONSTRAINT [PK_PageContents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PageContents_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]),
    CONSTRAINT [FK_PageContents_Centers_CenterId] FOREIGN KEY ([CenterId]) REFERENCES [Centers] ([Id])
);

CREATE TABLE [FAQs] (
    [Id] nvarchar(36) NOT NULL,
    [Question] nvarchar(max) NOT NULL,
    [Answer] nvarchar(max) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_FAQs] PRIMARY KEY ([Id])
);

-- 16. EXTRAS
CREATE TABLE [CourseReviews] (
    [Id] nvarchar(36) NOT NULL,
    [CourseId] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [Rating] int NOT NULL,
    [ReviewText] nvarchar(max) NULL,
    [ReviewDate] datetime2 NOT NULL,
    [IsApproved] bit NOT NULL,
    CONSTRAINT [PK_CourseReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CourseReviews_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseReviews_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Materials] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [FileType] nvarchar(max) NULL,
    [UploadDate] datetime2 NOT NULL,
    [ClassId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_Materials] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Materials_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE
);

-- 17. CHAT & MESSAGING
CREATE TABLE [ChatMessages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SenderId] nvarchar(36) NULL,
    [ReceiverId] nvarchar(36) NULL,
    [Content] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    [SessionId] nvarchar(max) NULL,
    [SenderValidName] nvarchar(max) NULL,
    CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChatMessages_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_ChatMessages_Users_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [Notifications] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [IsRead] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UserId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

-- 18. ASSIGNMENTS
CREATE TABLE [Assignments] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [ClassId] nvarchar(36) NOT NULL,
    [InstructorId] nvarchar(36) NOT NULL,
    [AssignmentType] int NOT NULL, -- Enum
    [Status] int NOT NULL, -- Enum
    [Note] nvarchar(max) NULL,
    [CancellationReason] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Assignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Assignments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Assignments_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id])
);

CREATE TABLE [ClassAssignments] (
    [Id] nvarchar(36) NOT NULL,
    [ClassId] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [AssignedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ClassAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClassAssignments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ClassAssignments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

-- 19. GUESTS & REVISION
CREATE TABLE [Guests] (
    [Id] nvarchar(36) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL, 
    [PhoneNumber] nvarchar(15) NOT NULL, 
    [Dob] datetime2 NOT NULL,
    [Address] nvarchar(255) NULL,
    [SelectedEntranceExamId] nvarchar(36) NULL,
    [Status] int NOT NULL, -- Enum GuestRegistrationStatus
    [CreatedAt] datetime2 NOT NULL,
    [UserId] nvarchar(36) NULL,
    [ExamRoom] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [ClassId] nvarchar(36) NULL,
    CONSTRAINT [PK_Guests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Guests_EntranceExams_SelectedEntranceExamId] FOREIGN KEY ([SelectedEntranceExamId]) REFERENCES [EntranceExams] ([Id]),
    CONSTRAINT [FK_Guests_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Guests_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id])
);

CREATE TABLE [RevisionPackages] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Fee] decimal(18,2) NOT NULL,
    [MaxStudents] int NOT NULL,
    [CurrentStudents] int NOT NULL,
    [Status] int NOT NULL, -- Enum RevisionPackageStatus
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_RevisionPackages] PRIMARY KEY ([Id])
);

CREATE TABLE [RevisionRegistrations] (
    [Id] nvarchar(36) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [RevisionPackageId] nvarchar(36) NOT NULL,
    [ClassId] nvarchar(36) NULL,
    [Status] int NOT NULL, -- Enum GuestRegistrationStatus
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_RevisionRegistrations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RevisionRegistrations_RevisionPackages_RevisionPackageId] FOREIGN KEY ([RevisionPackageId]) REFERENCES [RevisionPackages] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RevisionRegistrations_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id])
);
```

---

Table Roles {
    Id nvarchar [pk]
    Name nvarchar
    Description nvarchar
}

Table Users {
    Id nvarchar [pk]
    FullName nvarchar
    Email nvarchar
    Password nvarchar
    IsActive bit
    RoleId nvarchar [ref: > Roles.Id]
}

Table StudentProfiles {
    Id nvarchar [pk]
    UserId nvarchar [ref: > Users.Id]
    FullName nvarchar
    DateOfBirth datetime
    Gender nvarchar
    PhoneNumber nvarchar
    AddressLine nvarchar
    AvatarUrl nvarchar
}

Table InstructorProfiles {
    Id nvarchar [pk]
    UserId nvarchar [ref: > Users.Id]
    FullName nvarchar
    DateOfBirth datetime
    Gender nvarchar
    PhoneNumber nvarchar
    AddressLine nvarchar
    AvatarUrl nvarchar
    YearsOfExperience int
    Specialization nvarchar
    Bio nvarchar
    Certifications nvarchar
    GithubUrl nvarchar
}

Table Categories {
    Id nvarchar [pk]
    Name nvarchar(100)
    Description nvarchar(500)
}

Table Certificates {
    Id nvarchar [pk]
    Name nvarchar(100)
    Description nvarchar(500)
    IsActive bit
}

Table Courses {
    Id nvarchar [pk]
    Title nvarchar
    Description nvarchar
    TuitionFee decimal
    DurationMonths int
    CertificateId nvarchar [ref: > Certificates.Id]
    IsActive bit
    Image nvarchar
    Level nvarchar
    CategoryId nvarchar [ref: > Categories.Id]
}

Table Subjects {
    Id nvarchar [pk]
    Name nvarchar
    StudyTime int
    Description nvarchar
    LearningRoadmap nvarchar
    Image nvarchar
}

Table CourseSubjects {
    CourseId nvarchar [ref: > Courses.Id]
    SubjectId nvarchar [ref: > Subjects.Id]
}

Table CourseInstructors {
    CourseId nvarchar [ref: > Courses.Id]
    InstructorId nvarchar [ref: > Users.Id]
}

Table ClassCategories {
    Id nvarchar [pk]
    Name nvarchar
    Description nvarchar
    IsActive bit
}

Table Classes {
    Id nvarchar [pk]
    ClassName nvarchar
    ClassCategoryId nvarchar [ref: > ClassCategories.Id]
    NumberOfSeats int
    Status int
    CreatedAt datetime
}

Table Enrollments {
    Id nvarchar [pk]
    ClassId nvarchar [ref: > Classes.Id]
    StudentId nvarchar [ref: > Users.Id]
    CourseId nvarchar [ref: > Courses.Id]
    EnrolledDate datetime
    IsApproved bit
    IsPaid bit
    IsCompleted bit
    PaymentReference nvarchar
}

Table Lessons {
    Id nvarchar [pk]
    Title nvarchar
    Description nvarchar
    ContentLink nvarchar
    Image nvarchar
    DurationMinutes int
    CourseId nvarchar [ref: > Courses.Id]
    SubjectId nvarchar [ref: > Subjects.Id]
}

Table ClassLessons {
    Id nvarchar [pk]
    ClassId nvarchar [ref: > Classes.Id]
    LessonId nvarchar [ref: > Lessons.Id]
    AssignedAt datetime
}

Table Quizzes {
    Id nvarchar [pk]
    Name nvarchar
    Description nvarchar
    PassScore int
    DateCreated datetime
    LessonId nvarchar [ref: > Lessons.Id]
}

Table QuizQuestions {
    Id nvarchar [pk]
    QuestionText nvarchar
    OptionA nvarchar
    OptionB nvarchar
    OptionC nvarchar
    OptionD nvarchar
    CorrectOption nvarchar
    Points int
    QuizId nvarchar [ref: > Quizzes.Id]
}

Table Questions {
    Id nvarchar [pk]
    Content nvarchar
    Type int
    SubjectId nvarchar [ref: > Subjects.Id]
    Difficulty nvarchar
    Score float
    CreatedAt datetime
}

Table QuestionOptions {
    Id nvarchar [pk]
    QuestionId nvarchar [ref: > Questions.Id]
    Content nvarchar
    IsCorrect bit
}

Table ExamPapers {
    Id nvarchar [pk]
    Title nvarchar
    Duration int
    SubjectId nvarchar [ref: > Subjects.Id]
    CreatedAt datetime
}

Table ExamPaperQuestions {
    Id int [pk]
    ExamPaperId nvarchar [ref: > ExamPapers.Id]
    QuestionId nvarchar [ref: > Questions.Id]
    Order int
}

Table EntranceExams {
    Id nvarchar [pk]
    Title nvarchar
    ExamDate datetime
    Fee decimal
    MaxCandidates int
    Status int
    IsRegistrationOpen bit
    IsActive bit
    Subjects nvarchar
    ExamPaperId nvarchar [ref: > ExamPapers.Id]
}

Table StudentRegistrations {
    Id nvarchar [pk]
    FullName nvarchar(100)
    Email nvarchar
    Gender int
    DateOfBirth datetime
    Phone nvarchar
    CourseId nvarchar [ref: > Courses.Id]
    CenterId nvarchar [ref: > Centers.Id]
    HasExtraPractice bit
    RegisteredAt datetime
    Status int
}

Table ExamDetails {
    Id nvarchar [pk]
    RegistrationId nvarchar [ref: > StudentRegistrations.Id]
    ExamTime datetime
    ExamRoom nvarchar
    ExamDescription nvarchar
}

Table StudentExamSessions {
    Id nvarchar [pk]
    EntranceExamId nvarchar [ref: > EntranceExams.Id]
    StudentId nvarchar [ref: > Users.Id]
    ExamPaperId nvarchar [ref: > ExamPapers.Id]
    StartTime datetime
    EndTime datetime
    TotalScore float
    GradeLevel nvarchar
    Status int
}

Table StudentAnswers {
    Id int [pk]
    SessionId nvarchar [ref: > StudentExamSessions.Id]
    QuestionId nvarchar [ref: > Questions.Id]
    SelectedOptionId nvarchar
    EssayContent nvarchar
    EarnedScore float
    IsGraded bit
    ExaminerNote nvarchar
}

Table ExamResults {
    Id nvarchar [pk]
    StudentId nvarchar [ref: > Users.Id]
    EntranceExamId nvarchar [ref: > EntranceExams.Id]
    Score float
    IsPassed bit
    ExamDate datetime
}

Table Payments {
    Id nvarchar [pk]
    StudentId nvarchar [ref: > Users.Id]
    GuestId nvarchar [ref: > Guests.Id]
    Amount decimal
    PaymentMethod nvarchar
    PaymentDate datetime
    ReceiptNumber nvarchar
    Status int
    Purpose int
    CourseId nvarchar
}

Table VNPayTransactions {
    Id nvarchar [pk]
    PaymentId nvarchar [ref: > Payments.Id]
    VnpTxnRef nvarchar
    VnpAmount long
    VnpOrderInfo nvarchar
    VnpCreateDate nvarchar
    VnpResponseCode nvarchar
    VnpTransactionNo nvarchar
    VnpBankCode nvarchar
    VnpPayDate nvarchar
    Status int
}

Table Centers {
    Id nvarchar [pk]
    Name nvarchar
    Address nvarchar
    Phone nvarchar
}

Table PageContents {
    Id nvarchar [pk]
    Slug nvarchar(50)
    Title nvarchar
    Content nvarchar
    LastUpdated datetime
    SubjectId nvarchar [ref: > Subjects.Id]
    CenterId nvarchar [ref: > Centers.Id]
}

Table FAQs {
    Id nvarchar [pk]
    Question nvarchar
    Answer nvarchar
    DisplayOrder int
    IsActive bit
}

Table CourseReviews {
    Id nvarchar [pk]
    CourseId nvarchar [ref: > Courses.Id]
    StudentId nvarchar [ref: > Users.Id]
    Rating int
    ReviewText nvarchar
    ReviewDate datetime
    IsApproved bit
}

Table Materials {
    Id nvarchar [pk]
    Title nvarchar
    Description nvarchar
    FilePath nvarchar
    FileType nvarchar
    UploadDate datetime
    ClassId nvarchar [ref: > Classes.Id]
}

Table ChatMessages {
    Id int [pk]
    SenderId nvarchar [ref: > Users.Id]
    ReceiverId nvarchar [ref: > Users.Id]
    Content nvarchar
    Timestamp datetime
    IsRead bit
    SessionId nvarchar
    SenderValidName nvarchar
}

Table Notifications {
    Id int [pk]
    Title nvarchar
    Message nvarchar
    IsRead bit
    CreatedAt datetime
    UserId nvarchar [ref: > Users.Id]
}

Table Assignments {
    Id nvarchar [pk]
    Title nvarchar
    ClassId nvarchar [ref: > Classes.Id]
    InstructorId nvarchar [ref: > Users.Id]
    AssignmentType int
    Status int
    Note nvarchar
    CancellationReason nvarchar
    CreatedAt datetime
}

Table ClassAssignments {
    Id nvarchar [pk]
    ClassId nvarchar [ref: > Classes.Id]
    StudentId nvarchar [ref: > Users.Id]
    AssignedAt datetime
}

Table Guests {
    Id nvarchar [pk]
    FullName nvarchar(100)
    Email nvarchar(100)
    PhoneNumber nvarchar(15)
    Dob datetime
    Address nvarchar(255)
    SelectedEntranceExamId nvarchar [ref: > EntranceExams.Id]
    UserId nvarchar [ref: > Users.Id]
    ExamRoom nvarchar
    Description nvarchar
    ClassId nvarchar [ref: > Classes.Id]
    Status int
    CreatedAt datetime
}

Table RevisionPackages {
    Id nvarchar [pk]
    Title nvarchar
    Description nvarchar
    Fee decimal
    MaxStudents int
    CurrentStudents int
    Status int
    CreatedAt datetime
}

Table RevisionRegistrations {
    Id nvarchar [pk]
    FullName nvarchar
    Email nvarchar
    PhoneNumber nvarchar
    RevisionPackageId nvarchar [ref: > RevisionPackages.Id]
    ClassId nvarchar [ref: > Classes.Id]
    Status int
    CreatedAt datetime
}

