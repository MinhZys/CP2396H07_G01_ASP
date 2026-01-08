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
