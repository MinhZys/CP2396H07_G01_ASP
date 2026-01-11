# Final Project Database Schema (SQL)

Dưới đây là kịch bản tạo cơ sở dữ liệu ("Schema Script") chính thức cho dự án, đã được chốt theo yêu cầu:
1.  **Primary Keys**: Giữ nguyên `VARCHAR(26)` (dùng cho ULID/CUID).
2.  **Column Names**: Giữ nguyên `track`, `duration_weeks` theo SQL mẫu.
3.  **Bổ sung**: Thêm `registration_code` (SBD) vào bảng đăng ký thi để in danh sách cho tiện.

```sql
/* =========================================================
   DATABASE + DEFAULT CHARSET/COLLATION
   ========================================================= */
CREATE DATABASE IF NOT EXISTS symphony_portal
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_general_ci;

USE symphony_portal;

SET FOREIGN_KEY_CHECKS = 0;

/* =========================================================
   1) USERS + ROLES (Admin / Instructor / Student)
   ========================================================= */
CREATE TABLE IF NOT EXISTS users (
  id              VARCHAR(26)  NOT NULL, -- ULID
  email           VARCHAR(190) NOT NULL,
  password_hash   VARCHAR(255) NOT NULL,
  full_name       VARCHAR(120) NOT NULL,
  phone           VARCHAR(30)  DEFAULT NULL,
  role            ENUM('ADMIN','INSTRUCTOR','STUDENT') NOT NULL,
  status          ENUM('ACTIVE','SUSPENDED') NOT NULL DEFAULT 'ACTIVE',
  created_at      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uq_users_email (email)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS instructors (
  user_id         VARCHAR(26) NOT NULL,
  bio             TEXT DEFAULT NULL,
  expertise       VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (user_id),
  CONSTRAINT fk_instructors_user
    FOREIGN KEY (user_id) REFERENCES users(id)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS students (
  user_id         VARCHAR(26) NOT NULL,
  dob             DATE DEFAULT NULL,
  address         VARCHAR(255) DEFAULT NULL,
  PRIMARY KEY (user_id),
  CONSTRAINT fk_students_user
    FOREIGN KEY (user_id) REFERENCES users(id)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

/* =========================================================
   2) BRANCHES + CONTACT/FAQ (Public content)
   ========================================================= */
CREATE TABLE IF NOT EXISTS branches (
  id              VARCHAR(26) NOT NULL,
  name            VARCHAR(120) NOT NULL,
  address         VARCHAR(255) NOT NULL,
  phone           VARCHAR(30)  NOT NULL,
  email           VARCHAR(190) DEFAULT NULL,
  is_active       TINYINT(1) NOT NULL DEFAULT 1,
  created_at      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS faqs (
  id              VARCHAR(26) NOT NULL,
  question        VARCHAR(255) NOT NULL,
  answer          TEXT NOT NULL,
  is_active       TINYINT(1) NOT NULL DEFAULT 1,
  sort_order      INT NOT NULL DEFAULT 0,
  created_by      VARCHAR(26) DEFAULT NULL,
  created_at      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  KEY idx_faqs_active_sort (is_active, sort_order),
  CONSTRAINT fk_faqs_created_by
    FOREIGN KEY (created_by) REFERENCES users(id)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

/* =========================================================
   3) COURSES + CERTIFICATES
   ========================================================= */
CREATE TABLE IF NOT EXISTS certificates (
  id              VARCHAR(26) NOT NULL,
  code            VARCHAR(40)  NOT NULL,
  name            VARCHAR(160) NOT NULL,
  description     TEXT DEFAULT NULL,
  is_active       TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (id),
  UNIQUE KEY uq_cert_code (code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS courses (
  id                      VARCHAR(26) NOT NULL,
  code                    VARCHAR(40)  NOT NULL,
  name                    VARCHAR(160) NOT NULL,
  description             TEXT DEFAULT NULL,
  duration_weeks          INT NOT NULL DEFAULT 0,
  has_basic_knowledge     TINYINT(1) NOT NULL DEFAULT 0,
  base_tuition_fee        DECIMAL(12,2) NOT NULL DEFAULT 0,
  practice_fee_early      DECIMAL(12,2) NOT NULL DEFAULT 1000,
  practice_fee_late       DECIMAL(12,2) NOT NULL DEFAULT 1500,
  retake_exam_fee         DECIMAL(12,2) NOT NULL DEFAULT 1500,
  is_active               TINYINT(1) NOT NULL DEFAULT 1,
  created_at              DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uq_course_code (code),
  KEY idx_courses_active (is_active)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS course_certificates (
  course_id        VARCHAR(26) NOT NULL,
  certificate_id   VARCHAR(26) NOT NULL,
  PRIMARY KEY (course_id, certificate_id),
  CONSTRAINT fk_cc_course
    FOREIGN KEY (course_id) REFERENCES courses(id)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT fk_cc_certificate
    FOREIGN KEY (certificate_id) REFERENCES certificates(id)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

/* =========================================================
   4) ADMISSION CYCLES + ENTRANCE EXAM
   ========================================================= */
CREATE TABLE IF NOT EXISTS admission_cycles (
  id              VARCHAR(26) NOT NULL,
  name            VARCHAR(120) NOT NULL,
  start_date      DATE NOT NULL,
  end_date        DATE NOT NULL,
  exam_fee        DECIMAL(12,2) NOT NULL DEFAULT 0,
  status          ENUM('DRAFT','OPEN','CLOSED') NOT NULL DEFAULT 'DRAFT',
  created_by      VARCHAR(26) DEFAULT NULL,
  created_at      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS exam_sessions (
  id              VARCHAR(26) NOT NULL,
  cycle_id        VARCHAR(26) NOT NULL,
  branch_id       VARCHAR(26) DEFAULT NULL,
  exam_datetime   DATETIME NOT NULL,
  duration_min    INT NOT NULL DEFAULT 60,
  capacity        INT NOT NULL DEFAULT 0,
  PRIMARY KEY (id),
  CONSTRAINT fk_exam_session_cycle
    FOREIGN KEY (cycle_id) REFERENCES admission_cycles(id)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS exam_registrations (
  id              VARCHAR(26) NOT NULL,
  cycle_id        VARCHAR(26) NOT NULL,
  session_id      VARCHAR(26) DEFAULT NULL,
  student_id      VARCHAR(26) NOT NULL,
  registration_code VARCHAR(20) DEFAULT NULL, -- [ADDED] SBD (Số báo danh)
  status          ENUM('PENDING','CONFIRMED','CANCELLED','ATTENDED','NO_SHOW') NOT NULL DEFAULT 'PENDING',
  registered_at   DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uq_exam_reg_unique (cycle_id, student_id),
  KEY idx_exam_reg_code (registration_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS exam_results (
  id                VARCHAR(26) NOT NULL,
  registration_id   VARCHAR(26) NOT NULL,
  graded_by         VARCHAR(26) DEFAULT NULL,
  score             DECIMAL(5,2) NOT NULL DEFAULT 0,
  passed            TINYINT(1) NOT NULL DEFAULT 0,
  track             ENUM('BASIC_TO_CERT','DIRECT_CERT') NOT NULL, -- [KEPT]
  recommended_course_id VARCHAR(26) DEFAULT NULL,
  note              VARCHAR(255) DEFAULT NULL,
  graded_at         DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uq_exam_result_reg (registration_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

/* =========================================================
   5) CLASSES + ENROLLMENT
   ========================================================= */
CREATE TABLE IF NOT EXISTS classes (
  id              VARCHAR(26) NOT NULL,
  course_id       VARCHAR(26) NOT NULL,
  branch_id       VARCHAR(26) DEFAULT NULL,
  code            VARCHAR(40) NOT NULL,
  name            VARCHAR(160) NOT NULL,
  start_date      DATE NOT NULL,
  end_date        DATE NOT NULL,
  schedule_text   VARCHAR(255) DEFAULT NULL,
  capacity        INT NOT NULL DEFAULT 0,
  status          ENUM('PLANNED','ONGOING','COMPLETED','CANCELLED') NOT NULL DEFAULT 'PLANNED',
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS class_instructors (
  class_id        VARCHAR(26) NOT NULL,
  instructor_id   VARCHAR(26) NOT NULL,
  role            ENUM('MAIN','ASSISTANT') NOT NULL DEFAULT 'MAIN',
  PRIMARY KEY (class_id, instructor_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS enrollments (
  id              VARCHAR(26) NOT NULL,
  class_id        VARCHAR(26) NOT NULL,
  student_id      VARCHAR(26) NOT NULL,
  admission_cycle_id VARCHAR(26) DEFAULT NULL,
  exam_result_id  VARCHAR(26) DEFAULT NULL,
  status          ENUM('PENDING_PAYMENT','ENROLLED','CANCELLED','COMPLETED') NOT NULL DEFAULT 'PENDING_PAYMENT',
  enrolled_at     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uq_enroll_unique (class_id, student_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

/* =========================================================
   6) PAYMENTS + RECEIPTS
   ========================================================= */
CREATE TABLE IF NOT EXISTS payments (
  id              VARCHAR(26) NOT NULL,
  student_id      VARCHAR(26) NOT NULL,
  purpose         ENUM('EXAM_FEE','TUITION','PRACTICE_FEE','RETAKE_FEE','OTHER') NOT NULL,
  method          ENUM('CASH','CHEQUE') NOT NULL,
  amount          DECIMAL(12,2) NOT NULL,
  status          ENUM('PENDING','PAID','FAILED','REFUNDED') NOT NULL DEFAULT 'PENDING',
  reference_code  VARCHAR(60) DEFAULT NULL, -- [ADDED] Mã biên lai
  paid_at         DATETIME DEFAULT NULL,
  created_at      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

/* =========================================================
   7) EXTRAS (Materials, Notifications, Feedback)
   ========================================================= */
CREATE TABLE IF NOT EXISTS materials (
  id              VARCHAR(26) NOT NULL,
  class_id        VARCHAR(26) NOT NULL,
  uploaded_by     VARCHAR(26) DEFAULT NULL,
  title           VARCHAR(160) NOT NULL,
  file_url        VARCHAR(500) NOT NULL,
  created_at      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS notifications (
  id              VARCHAR(26) NOT NULL,
  user_id         VARCHAR(26) NOT NULL,
  title           VARCHAR(160) NOT NULL,
  message         TEXT NOT NULL,
  type            ENUM('SYSTEM','EXAM','CLASS','PAYMENT') NOT NULL DEFAULT 'SYSTEM',
  is_read         TINYINT(1) NOT NULL DEFAULT 0,
  created_at      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE IF NOT EXISTS course_reviews (
  id              VARCHAR(26) NOT NULL,
  course_id       VARCHAR(26) NOT NULL,
  student_id      VARCHAR(26) NOT NULL,
  rating          TINYINT NOT NULL,
  comment         TEXT DEFAULT NULL,
  created_at      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

SET FOREIGN_KEY_CHECKS = 1;
```

---

# Actual Implementation Schema (SQL Server/MSSQL)

Dưới đây là schema thực tế đang được triển khai trong Code (Entity Framework Core), sử dụng **SQL Server** (`mssqllocaldb`).

```sql
-- 1. ROLES
CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL, -- "Admin", "Instructor", "Student"
    [Description] nvarchar(max) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

-- 2. USERS
CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);

-- 3. COURSES
CREATE TABLE [Courses] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(100) NOT NULL,
    [Description] nvarchar(max) NULL,
    [TuitionFee] decimal(18,2) NOT NULL,
    [DurationMonths] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY ([Id])
);

-- 4. SUBJECTS (Updated: Int ID)
CREATE TABLE [Subjects] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    [StudyTime] int NOT NULL,
    [Description] nvarchar(max) NULL,
    CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id])
);

-- 5. COURSE_SUBJECTS (Many-to-Many Linking Table)
CREATE TABLE [CourseSubjects] (
    [CourseId] int NOT NULL,
    [SubjectId] int NOT NULL,
    CONSTRAINT [PK_CourseSubjects] PRIMARY KEY ([CourseId], [SubjectId]),
    CONSTRAINT [FK_CourseSubjects_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseSubjects_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE CASCADE
);

-- 6. CLASSES
CREATE TABLE [Classes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [CourseId] int NOT NULL,
    [InstructorId] int NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [IsOnline] bit NOT NULL DEFAULT 0,
    [Room] nvarchar(max) NULL,
    [OfflineFee] decimal(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Classes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Classes_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Classes_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id])
);

-- 7. ENTRANCE EXAMS (Renamed from AdmissionExams)
CREATE TABLE [EntranceExams] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [ExamDate] datetime2 NOT NULL,
    [Fee] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_EntranceExams] PRIMARY KEY ([Id])
);

-- 8. EXAM RESULTS
CREATE TABLE [ExamResults] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [EntranceExamId] int NOT NULL,
    [Score] float NOT NULL,
    [IsPassed] bit NOT NULL,
    [ExamDate] datetime2 NOT NULL,
    CONSTRAINT [PK_ExamResults] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamResults_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ExamResults_EntranceExams_EntranceExamId] FOREIGN KEY ([EntranceExamId]) REFERENCES [EntranceExams] ([Id]) ON DELETE CASCADE
);

-- 9. ENROLLMENTS
CREATE TABLE [Enrollments] (
    [Id] int NOT NULL IDENTITY,
    [ClassId] int NOT NULL,
    [StudentId] int NOT NULL,
    [EnrolledDate] datetime2 NOT NULL,
    [IsApproved] bit NOT NULL DEFAULT 0,
    [IsPaid] bit NOT NULL DEFAULT 0,
    [PaymentReference] nvarchar(max) NULL,
    CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Enrollments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Enrollments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

-- 10. FAQs
CREATE TABLE [FAQs] (
    [Id] int NOT NULL IDENTITY,
    [Question] nvarchar(max) NOT NULL,
    [Answer] nvarchar(max) NOT NULL,
    [DisplayOrder] int NOT NULL,
    CONSTRAINT [PK_FAQs] PRIMARY KEY ([Id])
);

-- 11. CENTERS (New)
CREATE TABLE [Centers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NULL,
    [Phone] nvarchar(max) NULL,
    CONSTRAINT [PK_Centers] PRIMARY KEY ([Id])
);

-- 12. PAGE CONTENTS
CREATE TABLE [PageContents] (
    [Id] int NOT NULL IDENTITY,
    [Slug] nvarchar(50) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [LastUpdated] datetime2 NOT NULL,
    [SubjectId] int NULL, -- Linked via CourseSubjects logic in requirement
    [CenterId] int NULL,
    CONSTRAINT [PK_PageContents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PageContents_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]),
    CONSTRAINT [FK_PageContents_Centers_CenterId] FOREIGN KEY ([CenterId]) REFERENCES [Centers] ([Id])
);

-- 13. PAYMENTS (New)
CREATE TABLE [Payments] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL,
    [PaymentDate] datetime2 NOT NULL,
    [ReceiptNumber] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
```
