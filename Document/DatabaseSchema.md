# Comprehensive Database Schema Documentation

## Overview
This document details the complete database schema for the Symphony Portal application, based on the Entity Framework Core **Code First** models located in the `Models` directory.

---

## 1. Identity & Profiles

### `Users`
Stores all account information for Admins, Instructors, Students, and Guests.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | Unique identifier (GUID). |
| `FullName` | `NVARCHAR(Max)` | `Required` | User's full display name. |
| `Email` | `NVARCHAR(Max)` | `Required`, `EmailAddress` | Unique login email. |
| `Password` | `NVARCHAR(Max)` | `Required` | User password. |
| `IsActive` | `BIT` | Default `true` | Account status. |
| `RoleId` | `NVARCHAR(450)` | **FK** | Links to **Rules** table. |

### `Roles`
Defines user permissions.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | Role ID (e.g., "1", "2"). |
| `Name` | `NVARCHAR(Max)` | | e.g. "Admin", "Instructor", "Student". |
| `Description` | `NVARCHAR(Max)` | | Details about the role. |

### `StudentProfiles`
Extended details for users with the "Student" role.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | Unique Profile ID. |
| `UserId` | `VARCHAR(36)` | **FK**, `Required` | Links to **Users**. |
| `FullName` | `NVARCHAR(Max)` | `Required` | Re-synced full name. |
| `DateOfBirth` | `DATETIME` | | Student's birth date. |
| `Gender` | `NVARCHAR(Max)` | | Student's gender. |
| `PhoneNumber` | `NVARCHAR(Max)` | `Phone` | Contact number. |
| `AddressLine` | `NVARCHAR(Max)` | | Home address. |
| `AvatarUrl` | `NVARCHAR(Max)` | | Profile picture URL. |

### `InstructorProfiles`
Extended details for users with the "Instructor" role.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | Unique Profile ID. |
| `UserId` | `VARCHAR(36)` | **FK**, `Required` | Links to **Users**. |
| `FullName` | `NVARCHAR(Max)` | `Required` | Instructor's name. |
| `DateOfBirth` | `DATETIME` | | |
| `Gender` | `NVARCHAR(Max)` | | |
| `PhoneNumber` | `NVARCHAR(Max)` | `Phone` | |
| `AddressLine` | `NVARCHAR(Max)` | | |
| `AvatarUrl` | `NVARCHAR(Max)` | | |
| `YearsOfExperience` | `INT` | | Years active in field. |
| `Specialization` | `NVARCHAR(Max)` | | Primary teaching focus. |
| `Bio` | `NVARCHAR(Max)` | | Short biography. |
| `Certifications` | `NVARCHAR(Max)` | | List of certs held. |
| `GithubUrl` | `NVARCHAR(Max)` | `Url` | Link to GitHub profile. |

---

## 2. Course Management

### `Categories`
High-level grouping for courses (e.g., Programming, Art).
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | Category ID. |
| `Name` | `NVARCHAR(Max)` | `Required` | Category Name. |
| `Description` | `NVARCHAR(Max)` | | |

### `Courses`
The core educational product.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | Course ID. |
| `Title` | `NVARCHAR(Max)` | `Required` | Course Title. |
| `Description` | `NVARCHAR(Max)` | | Detailed HTML description. |
| `TuitionFee` | `DECIMAL` | | Cost of the course. |
| `DurationMonths` | `INT` | | Length of course. |
| `Level` | `INT` | `Enum` | Beginner, Intermediate, Advanced. |
| `Image` | `NVARCHAR(Max)` | | Cover image URL. |
| `IsActive` | `BIT` | Default `true` | Visibility status. |
| `CategoryId` | `VARCHAR(36)` | **FK**, `Required` | Links to **Categories**. |
| `CertificateId` | `VARCHAR(36)` | **FK**, `Required` | Links to **Certificates**. |

### `Subjects`
Modules or topics that make up a course.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | Subject ID. |
| `Name` | `NVARCHAR(Max)` | `Required` | Subject Name. |
| `StudyTime` | `INT` | | Hours required. |
| `Description` | `NVARCHAR(Max)` | | |
| `LearningRoadmap` | `NVARCHAR(Max)` | `JSON` | Encoded step-by-step roadmap. |
| `Image` | `NVARCHAR(Max)` | | |

### `CourseSubjects` (Join Table)
Many-to-Many relationship between Courses and Subjects.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| `CourseId` | `VARCHAR(36)` | **PK**, **FK** | |
| `SubjectId` | `VARCHAR(36)` | **PK**, **FK** | |

### `CourseInstructors` (Join Table)
Many-to-Many relationship between Courses and Instructors.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| `CourseId` | `VARCHAR(36)` | **PK**, **FK** | |
| `InstructorId` | `VARCHAR(36)` | **PK**, **FK** | (User Table Id). |

---

## 3. Class & Enrollment

### `ClassCategories`
Types of classes (e.g., Theory vs Lab).
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | |
| `Name` | `NVARCHAR(Max)` | `Required` | |
| `Description` | `NVARCHAR(Max)` | | |

### `Classes`
Specific scheduled instances of a Course.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | |
| `ClassName` | `NVARCHAR(Max)` | `Required` | e.g. "C#-001". |
| `NumberOfSeats` | `INT` | | Max capacity. |
| `Status` | `INT` | `Enum` | Active, Finished, Cancelled. |
| `CreatedAt` | `DATETIME` | | Creation timestamp. |
| `ClassCategoryId`| `VARCHAR(36)` | **FK**, `Required` | Links to **ClassCategories**. |
| `CourseId` | `VARCHAR(36)` | **FK** | Links to **Courses**. |
| `InstructorId` | `VARCHAR(36)` | **FK** | Links to **Users**. |
| `RoomName` | `NVARCHAR(Max)` | | Name of the exam room/classroom. |
| `RoomLocation` | `NVARCHAR(Max)` | | Physical location of the room. |
| `CenterId` | `VARCHAR(36)` | **FK** | Links to **Centers**. |
| `Fee` | `DECIMAL(18,2)` | `Default 0` | Tuition or special fee for the class. |

### `Enrollments`
Records of students joining classes.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | |
| `StudentId` | `VARCHAR(36)` | **FK** | Links to **Users**. |
| `ClassId` | `VARCHAR(36)` | **FK** | Links to **Classes**. |
| `CourseId` | `VARCHAR(36)` | **FK** | Historical reference to Course. |
| `EnrolledDate` | `DATETIME` | | |
| `IsApproved` | `BIT` | | Admin approval status. |
| `IsPaid` | `BIT` | | Payment status. |
| `PaymentReference`| `NVARCHAR(Max)` | | Transaction ID. |
| `IsCompleted` | `BIT` | | Course completion status. |

---

## 4. Assessment & Admissions

### `StudentRegistrations`
Applications from prospective students.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | |
| `FullName` | `NVARCHAR(100)` | `Required` | |
| `Email` | `NVARCHAR(Max)` | `Required` | |
| `Phone` | `NVARCHAR(Max)` | | |
| `CourseId` | `VARCHAR(36)` | **FK**, `Required` | Desired course. |
| `CenterId` | `VARCHAR(36)` | **FK** | Preferred location. |
| `Status` | `INT` | `Enum` | Pending, Approved, Rejected. |
| `HasExtraPractice`| `BIT` | | |

### `EntranceExams` & `ExamDetails`
Tables handling the logic for entrance testing, linked typically to Registrations or Courses.

### `ClassExams`
Defines final exam sessions for specific classes.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | Unique ID for the class exam setting. |
| `ClassId` | `VARCHAR(36)` | **FK**, `Required` | Links to **Classes**. |
| `CourseId` | `VARCHAR(36)` | **FK**, `Required` | Links to **Courses**. |
| `ExamPaperId` | `VARCHAR(36)` | **FK**, `Required` | Links to **ExamPapers**. |
| `ExamDate` | `DATETIME` | | Specific date when the exam is held. |
| `DurationOverride` | `INT` | | Custom duration in minutes (overrides paper duration). |
| `Status` | `INT` | `Required` | Scheduled, Taking, Completed, Cancelled. |
| `IsScorePublished` | `BIT` | `Default false`| Whether scores are visible to students. |
| `CreatedAt` | `DATETIME` | `Default Now`| Record creation timestamp. |

### `StudentExamSessions`
Tracks actual exam-taking sessions for both entrance and final exams.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | Session ID. |
| `EntranceExamId` | `VARCHAR(36)` | **FK** | Links to **EntranceExams** (nullable). |
| `ClassExamId` | `VARCHAR(36)` | **FK** | Links to **ClassExams** (nullable). |
| `StudentId` | `VARCHAR(36)` | **FK**, `Required` | Links to **Users**. |
| `ExamPaperId` | `VARCHAR(36)` | **FK**, `Required` | Links to **ExamPapers**. |
| `StartTime` | `DATETIME` | | Actual start timestamp. |
| `EndTime` | `DATETIME` | | Actual submission timestamp. |
| `TotalScore` | `FLOAT` | | Achieved score. |
| `GradeLevel` | `NVARCHAR(Max)` | | e.g., "Pass", "A", "Distinction". |
| `Status` | `INT` | `Enum` | Taking, Finished. |

### `ExamAttempts`
Tracks student progress through multiple exam attempts for course completion.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | Unique identifier for the attempt. |
| `StudentId` | `VARCHAR(36)` | **FK**, `Required` | Links to **Users**. |
| `CourseId` | `VARCHAR(36)` | **FK**, `Required` | Links to **Courses**. |
| `AttemptNumber` | `INT` | `Required` | Attempt count (1, 2, 3...). |
| `AverageScore` | `FLOAT` | | Calculated average score of the attempt. |
| `Status` | `INT` | `Required` | InProgress, Completed, Failed. |
| `RetakeFeePaid` | `BIT` | `Default false`| Whether the fee for retaking was paid. |
| `PaymentReference` | `NVARCHAR(Max)` | | Linked payment transaction reference. |
| `AttemptDate` | `DATETIME` | `Default Now`| Start date of the attempt. |
| `CompletedDate` | `DATETIME` | | Completion date of the attempt. |

---

## 5. System & Interaction

### `ChatMessages`
Stores chat history (SignalR).
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `INT` | **PK** | Auto-increment ID. |
| `SenderId` | `VARCHAR(36)` | **FK** | Nullable (for Guests). |
| `ReceiverId` | `VARCHAR(36)` | **FK** | Nullable (for public/groups). |
| `Content` | `NVARCHAR(Max)` | `Required` | Message text. |
| `SessionId` | `NVARCHAR(Max)` | | Cookie ID for Guest tracking. |
| `IsRead` | `BIT` | | Read receipt. |

### `CourseReviews`
Student feedback.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | |
| `CourseId` | `VARCHAR(36)` | **FK** | |
| `StudentId` | `VARCHAR(36)` | **FK** | |
| `Rating` | `INT` | | 1-5 Stars. |
| `ReviewText` | `NVARCHAR(Max)` | | Text comment. |
| `IsApproved` | `BIT` | | Moderation status. |

### `Guests`
Temporary records for visitors.
*   Similar to incomplete User records, used for tracking session-based interactions.

### `Payments`
Transaction records for tuition fees.
*   Includes Method (Cash/Transfer), Amount, Date, and linked Enrollment.

### `Certificates`
Definitions of available certificates.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | |
| `Name` | `NVARCHAR(100)`| `Required` | |
| `Description` | `NVARCHAR(500)`| | |
| `IsActive` | `BIT` | | |

### `StudentCertificates`
Issued achievements for students who pass courses.
| Column | Type | Constraints | Description |
| :--- | :--- | :--- | :--- |
| **Id** | `VARCHAR(36)` | **PK** | |
| `StudentId` | `VARCHAR(36)` | **FK**, `Required` | |
| `CourseId` | `VARCHAR(36)` | **FK**, `Required` | |
| `CertificateId` | `VARCHAR(36)` | **FK**, `Required` | |
| `IssueDate` | `DATETIME` | `Required` | |
| `ExpiryDate` | `DATETIME` | | |
| `CertificateCode` | `NVARCHAR(50)` | `Required` | Unique code. |
| `AverageScore` | `FLOAT` | `Required` | Final GPA. |
| `IsValid` | `BIT` | `Default true` | |
