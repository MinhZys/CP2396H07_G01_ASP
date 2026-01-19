# Database Schema (SQL Server/MSSQL)

Dưới đây là schema thực tế đang được triển khai trong Code (Entity Framework Core), sử dụng **SQL Server** (`mssqllocaldb`).

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
    [Id] nvarchar(max) NOT NULL, -- Note: Model says string but no length attrib, likely nvarchar(max) or 450 key
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
    [SubjectId] nvarchar(36) NOT NULL, -- Assuming SubjectId matches Subject.Id type
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
CREATE TABLE [Classes] (
    [Id] nvarchar(36) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [CourseId] nvarchar(36) NOT NULL,
    [InstructorId] nvarchar(36) NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [IsOnline] bit NOT NULL,
    [Room] nvarchar(max) NULL,
    [OfflineFee] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Classes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Classes_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Classes_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id])
);

-- 10. ENROLLMENTS
CREATE TABLE [Enrollments] (
    [Id] nvarchar(36) NOT NULL,
    [ClassId] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [EnrolledDate] datetime2 NOT NULL,
    [IsApproved] bit NOT NULL,
    [IsPaid] bit NOT NULL,
    [PaymentReference] nvarchar(max) NULL,
    CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Enrollments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Enrollments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
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
    [SubjectId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_Lessons] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Lessons_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE, -- Assuming cascade, verify if NO ACTION
    CONSTRAINT [FK_Lessons_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE CASCADE
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
CREATE TABLE [EntranceExams] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [ExamDate] datetime2 NOT NULL,
    [Fee] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_EntranceExams] PRIMARY KEY ([Id])
);

CREATE TABLE [StudentRegistrations] (
    [Id] nvarchar(36) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Gender] nvarchar(max) NOT NULL, -- Enum
    [DateOfBirth] datetime2 NOT NULL,
    [Phone] nvarchar(max) NULL,
    [CourseId] nvarchar(36) NOT NULL,
    [CenterId] nvarchar(36) NOT NULL,
    [HasExtraPractice] bit NOT NULL,
    [RegisteredAt] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL, -- Enum
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

-- 14. PAYMENTS
CREATE TABLE [Payments] (
    [Id] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL, -- Enum
    [PaymentDate] datetime2 NOT NULL,
    [ReceiptNumber] nvarchar(max) NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
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
    [SubjectId] nvarchar(max) NULL,
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
    CONSTRAINT [FK_CourseReviews_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION -- Prevent cycles?
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

-- 18. ASSIGNMENTS
CREATE TABLE [Assignments] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [ClassId] nvarchar(36) NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [AssignmentType] nvarchar(max) NOT NULL, -- Enum
    [Status] nvarchar(max) NOT NULL, -- Enum
    CONSTRAINT [PK_Assignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Assignments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE
);

-- 19. CLASS CATEGORIES
CREATE TABLE [ClassCategories] (
    [Id] nvarchar(36) NOT NULL, 
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    CONSTRAINT [PK_ClassCategories] PRIMARY KEY ([Id])
);
```
