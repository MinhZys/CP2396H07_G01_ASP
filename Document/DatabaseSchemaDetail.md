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
    [ClassCategoryId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_Classes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Classes_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Classes_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Classes_ClassCategories_ClassCategoryId] FOREIGN KEY ([ClassCategoryId]) REFERENCES [ClassCategories] ([Id])
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
    [Status] int NOT NULL,
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

-- 20. GUESTS
CREATE TABLE [Guests] (
    [Id] nvarchar(36) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL, 
    [Password] nvarchar(max) NOT NULL, -- Added
    [PhoneNumber] nvarchar(15) NOT NULL, 
    [Password] nvarchar(max) NOT NULL, -- Added
    [PhoneNumber] nvarchar(15) NOT NULL, 
    [PhoneNumber] nvarchar(15) NOT NULL,
    [Dob] datetime2 NOT NULL,
    [Address] nvarchar(255) NULL,
    [SelectedEntranceExamId] nvarchar(36) NULL,
    [Status] nvarchar(max) NOT NULL, -- Enum
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

 CREATE TABLE [Notifications] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(max) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [IsRead] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UserId] nvarchar(36) NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
```


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

Table Classes {
    Id nvarchar [pk]
    Name nvarchar
    CourseId nvarchar [ref: > Courses.Id]
    InstructorId nvarchar [ref: > Users.Id]
    StartDate datetime
    EndDate datetime
    IsOnline bit
    Room nvarchar
    OfflineFee decimal
    ClassCategoryId nvarchar [ref: > ClassCategories.Id]
}

Table Enrollments {
    Id nvarchar [pk]
    ClassId nvarchar [ref: > Classes.Id]
    StudentId nvarchar [ref: > Users.Id]
    EnrolledDate datetime
    IsApproved bit
    IsPaid bit
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

Table EntranceExams {
    Id nvarchar [pk]
    Title nvarchar
    ExamDate datetime
    Fee decimal
    IsActive bit
}

Table StudentRegistrations {
    Id nvarchar [pk]
    FullName nvarchar(100)
    Email nvarchar
    Gender nvarchar
    DateOfBirth datetime
    Phone nvarchar
    CourseId nvarchar [ref: > Courses.Id]
    CenterId nvarchar [ref: > Centers.Id]
    HasExtraPractice bit
    RegisteredAt datetime
    Status nvarchar
}

Table ExamDetails {
    Id nvarchar [pk]
    RegistrationId nvarchar [ref: > StudentRegistrations.Id]
    ExamTime datetime
    ExamRoom nvarchar
    ExamDescription nvarchar
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
    Amount decimal
    PaymentMethod nvarchar
    PaymentDate datetime
    ReceiptNumber nvarchar
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

Table Assignments {
    Id nvarchar [pk]
    Title nvarchar
    Description nvarchar
    ClassId nvarchar [ref: > Classes.Id]
    DueDate datetime
    AssignmentType nvarchar
    Status nvarchar
}

Table ClassCategories {
    Id nvarchar [pk]
    Name nvarchar
    Description nvarchar
    IsActive bit
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
}

