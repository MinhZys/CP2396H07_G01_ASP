IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Centers] (
    [Id] nvarchar(26) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Centers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Courses] (
    [Id] nvarchar(26) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [TuitionFee] decimal(18,2) NOT NULL,
    [DurationMonths] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EntranceExams] (
    [Id] nvarchar(26) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [ExamDate] datetime2 NOT NULL,
    [Fee] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_EntranceExams] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FAQs] (
    [Id] nvarchar(26) NOT NULL,
    [Question] nvarchar(max) NOT NULL,
    [Answer] nvarchar(max) NOT NULL,
    [DisplayOrder] int NOT NULL,
    CONSTRAINT [PK_FAQs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Roles] (
    [Id] nvarchar(26) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Subjects] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [StudyTime] int NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] nvarchar(26) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [RoleId] nvarchar(26) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [CourseSubjects] (
    [CourseId] nvarchar(26) NOT NULL,
    [SubjectId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_CourseSubjects] PRIMARY KEY ([CourseId], [SubjectId]),
    CONSTRAINT [FK_CourseSubjects_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseSubjects_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PageContents] (
    [Id] nvarchar(26) NOT NULL,
    [Slug] nvarchar(50) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [LastUpdated] datetime2 NOT NULL,
    [SubjectId] nvarchar(450) NULL,
    [CenterId] nvarchar(26) NULL,
    CONSTRAINT [PK_PageContents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PageContents_Centers_CenterId] FOREIGN KEY ([CenterId]) REFERENCES [Centers] ([Id]),
    CONSTRAINT [FK_PageContents_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id])
);
GO

CREATE TABLE [Classes] (
    [Id] nvarchar(26) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [CourseId] nvarchar(26) NOT NULL,
    [InstructorId] nvarchar(26) NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [IsOnline] bit NOT NULL,
    [Room] nvarchar(max) NULL,
    [OfflineFee] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Classes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Classes_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Classes_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id])
);
GO

CREATE TABLE [ExamResults] (
    [Id] nvarchar(26) NOT NULL,
    [StudentId] nvarchar(26) NOT NULL,
    [EntranceExamId] nvarchar(26) NOT NULL,
    [Score] float NOT NULL,
    [IsPassed] bit NOT NULL,
    [ExamDate] datetime2 NOT NULL,
    CONSTRAINT [PK_ExamResults] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamResults_EntranceExams_EntranceExamId] FOREIGN KEY ([EntranceExamId]) REFERENCES [EntranceExams] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ExamResults_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Payments] (
    [Id] nvarchar(26) NOT NULL,
    [StudentId] nvarchar(26) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL,
    [PaymentDate] datetime2 NOT NULL,
    [ReceiptNumber] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Enrollments] (
    [Id] nvarchar(26) NOT NULL,
    [ClassId] nvarchar(26) NOT NULL,
    [StudentId] nvarchar(26) NOT NULL,
    [EnrolledDate] datetime2 NOT NULL,
    [IsApproved] bit NOT NULL,
    [IsPaid] bit NOT NULL,
    [PaymentReference] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Enrollments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Enrollments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([Id], [Description], [Name])
VALUES (N'1', N'System Administrator', N'Admin'),
(N'2', N'Course Instructor', N'Instructor'),
(N'3', N'Learner', N'Student');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'IsActive', N'Password', N'RoleId') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [Email], [FullName], [IsActive], [Password], [RoleId])
VALUES (N'1', N'admin@symphony.local', N'System Admin', CAST(1 AS bit), N'admin', N'1'),
(N'2', N'teacher@symphony.local', N'Mr. Teacher', CAST(1 AS bit), N'123', N'2'),
(N'3', N'student@symphony.local', N'Student One', CAST(1 AS bit), N'123', N'3');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Email', N'FullName', N'IsActive', N'Password', N'RoleId') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;
GO

CREATE INDEX [IX_Classes_CourseId] ON [Classes] ([CourseId]);
GO

CREATE INDEX [IX_Classes_InstructorId] ON [Classes] ([InstructorId]);
GO

CREATE INDEX [IX_CourseSubjects_SubjectId] ON [CourseSubjects] ([SubjectId]);
GO

CREATE INDEX [IX_Enrollments_ClassId] ON [Enrollments] ([ClassId]);
GO

CREATE INDEX [IX_Enrollments_StudentId] ON [Enrollments] ([StudentId]);
GO

CREATE INDEX [IX_ExamResults_EntranceExamId] ON [ExamResults] ([EntranceExamId]);
GO

CREATE INDEX [IX_ExamResults_StudentId] ON [ExamResults] ([StudentId]);
GO

CREATE INDEX [IX_PageContents_CenterId] ON [PageContents] ([CenterId]);
GO

CREATE INDEX [IX_PageContents_SubjectId] ON [PageContents] ([SubjectId]);
GO

CREATE INDEX [IX_Payments_StudentId] ON [Payments] ([StudentId]);
GO

CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111084134_UpdateIdToString', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Courses] ADD [Certification] nvarchar(max) NOT NULL DEFAULT N'';
GO

CREATE TABLE [StudentRegistrations] (
    [Id] nvarchar(26) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    [CourseId] nvarchar(26) NOT NULL,
    [CenterId] nvarchar(26) NOT NULL,
    [HasExtraPractice] bit NOT NULL,
    [RegisteredAt] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_StudentRegistrations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentRegistrations_Centers_CenterId] FOREIGN KEY ([CenterId]) REFERENCES [Centers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentRegistrations_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ExamDetails] (
    [Id] nvarchar(26) NOT NULL,
    [RegistrationId] nvarchar(26) NOT NULL,
    [ExamTime] datetime2 NOT NULL,
    [ExamRoom] nvarchar(max) NOT NULL,
    [ExamDescription] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ExamDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamDetails_StudentRegistrations_RegistrationId] FOREIGN KEY ([RegistrationId]) REFERENCES [StudentRegistrations] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_ExamDetails_RegistrationId] ON [ExamDetails] ([RegistrationId]);
GO

CREATE INDEX [IX_StudentRegistrations_CenterId] ON [StudentRegistrations] ([CenterId]);
GO

CREATE INDEX [IX_StudentRegistrations_CourseId] ON [StudentRegistrations] ([CourseId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111095203_AddStudentRegistration', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Users] DROP CONSTRAINT [FK_Users_Roles_RoleId];
GO

ALTER TABLE [StudentRegistrations] DROP CONSTRAINT [FK_StudentRegistrations_Courses_CourseId];
GO

ALTER TABLE [StudentRegistrations] DROP CONSTRAINT [FK_StudentRegistrations_Centers_CenterId];
GO

ALTER TABLE [Payments] DROP CONSTRAINT [FK_Payments_Users_StudentId];
GO

ALTER TABLE [PageContents] DROP CONSTRAINT [FK_PageContents_Centers_CenterId];
GO

ALTER TABLE [ExamResults] DROP CONSTRAINT [FK_ExamResults_Users_StudentId];
GO

ALTER TABLE [ExamResults] DROP CONSTRAINT [FK_ExamResults_EntranceExams_EntranceExamId];
GO

ALTER TABLE [Enrollments] DROP CONSTRAINT [FK_Enrollments_Users_StudentId];
GO

ALTER TABLE [Enrollments] DROP CONSTRAINT [FK_Enrollments_Classes_ClassId];
GO

ALTER TABLE [CourseSubjects] DROP CONSTRAINT [FK_CourseSubjects_Courses_CourseId];
GO

ALTER TABLE [Classes] DROP CONSTRAINT [FK_Classes_Courses_CourseId];
GO

ALTER TABLE [Classes] DROP CONSTRAINT [FK_Classes_Users_InstructorId];
GO

ALTER TABLE [ExamDetails] DROP CONSTRAINT [FK_ExamDetails_StudentRegistrations_RegistrationId];
GO

ALTER TABLE [Users] DROP CONSTRAINT [PK_Users];
GO

ALTER TABLE [Roles] DROP CONSTRAINT [PK_Roles];
GO

ALTER TABLE [StudentRegistrations] DROP CONSTRAINT [PK_StudentRegistrations];
GO

ALTER TABLE [Payments] DROP CONSTRAINT [PK_Payments];
GO

ALTER TABLE [PageContents] DROP CONSTRAINT [PK_PageContents];
GO

ALTER TABLE [FAQs] DROP CONSTRAINT [PK_FAQs];
GO

ALTER TABLE [ExamResults] DROP CONSTRAINT [PK_ExamResults];
GO

ALTER TABLE [ExamDetails] DROP CONSTRAINT [PK_ExamDetails];
GO

ALTER TABLE [EntranceExams] DROP CONSTRAINT [PK_EntranceExams];
GO

ALTER TABLE [Enrollments] DROP CONSTRAINT [PK_Enrollments];
GO

ALTER TABLE [CourseSubjects] DROP CONSTRAINT [PK_CourseSubjects];
GO

ALTER TABLE [Courses] DROP CONSTRAINT [PK_Courses];
GO

ALTER TABLE [Classes] DROP CONSTRAINT [PK_Classes];
GO

ALTER TABLE [Centers] DROP CONSTRAINT [PK_Centers];
GO

DROP INDEX [IX_Users_RoleId] ON [Users];
DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'RoleId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Users] ALTER COLUMN [RoleId] nvarchar(36) NOT NULL;
CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'Id');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Users] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DROP INDEX [IX_StudentRegistrations_CourseId] ON [StudentRegistrations];
DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StudentRegistrations]') AND [c].[name] = N'CourseId');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [StudentRegistrations] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [StudentRegistrations] ALTER COLUMN [CourseId] nvarchar(36) NOT NULL;
CREATE INDEX [IX_StudentRegistrations_CourseId] ON [StudentRegistrations] ([CourseId]);
GO

DROP INDEX [IX_StudentRegistrations_CenterId] ON [StudentRegistrations];
DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StudentRegistrations]') AND [c].[name] = N'CenterId');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [StudentRegistrations] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [StudentRegistrations] ALTER COLUMN [CenterId] nvarchar(36) NOT NULL;
CREATE INDEX [IX_StudentRegistrations_CenterId] ON [StudentRegistrations] ([CenterId]);
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StudentRegistrations]') AND [c].[name] = N'Id');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [StudentRegistrations] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [StudentRegistrations] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Roles]') AND [c].[name] = N'Id');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Roles] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Roles] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DROP INDEX [IX_Payments_StudentId] ON [Payments];
DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payments]') AND [c].[name] = N'StudentId');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Payments] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [Payments] ALTER COLUMN [StudentId] nvarchar(36) NOT NULL;
CREATE INDEX [IX_Payments_StudentId] ON [Payments] ([StudentId]);
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payments]') AND [c].[name] = N'Id');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Payments] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [Payments] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DROP INDEX [IX_PageContents_CenterId] ON [PageContents];
DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PageContents]') AND [c].[name] = N'CenterId');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [PageContents] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [PageContents] ALTER COLUMN [CenterId] nvarchar(36) NULL;
CREATE INDEX [IX_PageContents_CenterId] ON [PageContents] ([CenterId]);
GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PageContents]') AND [c].[name] = N'Id');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [PageContents] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [PageContents] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FAQs]') AND [c].[name] = N'Id');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [FAQs] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [FAQs] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DROP INDEX [IX_ExamResults_StudentId] ON [ExamResults];
DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamResults]') AND [c].[name] = N'StudentId');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [ExamResults] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [ExamResults] ALTER COLUMN [StudentId] nvarchar(36) NOT NULL;
CREATE INDEX [IX_ExamResults_StudentId] ON [ExamResults] ([StudentId]);
GO

DROP INDEX [IX_ExamResults_EntranceExamId] ON [ExamResults];
DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamResults]') AND [c].[name] = N'EntranceExamId');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [ExamResults] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [ExamResults] ALTER COLUMN [EntranceExamId] nvarchar(36) NOT NULL;
CREATE INDEX [IX_ExamResults_EntranceExamId] ON [ExamResults] ([EntranceExamId]);
GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamResults]') AND [c].[name] = N'Id');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [ExamResults] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [ExamResults] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DROP INDEX [IX_ExamDetails_RegistrationId] ON [ExamDetails];
DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamDetails]') AND [c].[name] = N'RegistrationId');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [ExamDetails] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [ExamDetails] ALTER COLUMN [RegistrationId] nvarchar(36) NOT NULL;
CREATE UNIQUE INDEX [IX_ExamDetails_RegistrationId] ON [ExamDetails] ([RegistrationId]);
GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamDetails]') AND [c].[name] = N'Id');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [ExamDetails] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [ExamDetails] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EntranceExams]') AND [c].[name] = N'Id');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [EntranceExams] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [EntranceExams] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DROP INDEX [IX_Enrollments_StudentId] ON [Enrollments];
DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Enrollments]') AND [c].[name] = N'StudentId');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [Enrollments] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [Enrollments] ALTER COLUMN [StudentId] nvarchar(36) NOT NULL;
CREATE INDEX [IX_Enrollments_StudentId] ON [Enrollments] ([StudentId]);
GO

DROP INDEX [IX_Enrollments_ClassId] ON [Enrollments];
DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Enrollments]') AND [c].[name] = N'ClassId');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [Enrollments] DROP CONSTRAINT [' + @var18 + '];');
ALTER TABLE [Enrollments] ALTER COLUMN [ClassId] nvarchar(36) NOT NULL;
CREATE INDEX [IX_Enrollments_ClassId] ON [Enrollments] ([ClassId]);
GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Enrollments]') AND [c].[name] = N'Id');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [Enrollments] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [Enrollments] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CourseSubjects]') AND [c].[name] = N'CourseId');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [CourseSubjects] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [CourseSubjects] ALTER COLUMN [CourseId] nvarchar(36) NOT NULL;
GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Courses]') AND [c].[name] = N'Id');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Courses] DROP CONSTRAINT [' + @var21 + '];');
ALTER TABLE [Courses] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DROP INDEX [IX_Classes_InstructorId] ON [Classes];
DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Classes]') AND [c].[name] = N'InstructorId');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [Classes] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [Classes] ALTER COLUMN [InstructorId] nvarchar(36) NULL;
CREATE INDEX [IX_Classes_InstructorId] ON [Classes] ([InstructorId]);
GO

DROP INDEX [IX_Classes_CourseId] ON [Classes];
DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Classes]') AND [c].[name] = N'CourseId');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Classes] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [Classes] ALTER COLUMN [CourseId] nvarchar(36) NOT NULL;
CREATE INDEX [IX_Classes_CourseId] ON [Classes] ([CourseId]);
GO

DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Classes]') AND [c].[name] = N'Id');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Classes] DROP CONSTRAINT [' + @var24 + '];');
ALTER TABLE [Classes] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Centers]') AND [c].[name] = N'Id');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [Centers] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [Centers] ALTER COLUMN [Id] nvarchar(36) NOT NULL;
GO

ALTER TABLE [Users] ADD CONSTRAINT [PK_Users] PRIMARY KEY ([Id]);
GO

ALTER TABLE [Roles] ADD CONSTRAINT [PK_Roles] PRIMARY KEY ([Id]);
GO

ALTER TABLE [StudentRegistrations] ADD CONSTRAINT [PK_StudentRegistrations] PRIMARY KEY ([Id]);
GO

ALTER TABLE [Payments] ADD CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]);
GO

ALTER TABLE [PageContents] ADD CONSTRAINT [PK_PageContents] PRIMARY KEY ([Id]);
GO

ALTER TABLE [FAQs] ADD CONSTRAINT [PK_FAQs] PRIMARY KEY ([Id]);
GO

ALTER TABLE [ExamResults] ADD CONSTRAINT [PK_ExamResults] PRIMARY KEY ([Id]);
GO

ALTER TABLE [ExamDetails] ADD CONSTRAINT [PK_ExamDetails] PRIMARY KEY ([Id]);
GO

ALTER TABLE [EntranceExams] ADD CONSTRAINT [PK_EntranceExams] PRIMARY KEY ([Id]);
GO

ALTER TABLE [Enrollments] ADD CONSTRAINT [PK_Enrollments] PRIMARY KEY ([Id]);
GO

ALTER TABLE [CourseSubjects] ADD CONSTRAINT [PK_CourseSubjects] PRIMARY KEY ([CourseId], [SubjectId]);
GO

ALTER TABLE [Courses] ADD CONSTRAINT [PK_Courses] PRIMARY KEY ([Id]);
GO

ALTER TABLE [Classes] ADD CONSTRAINT [PK_Classes] PRIMARY KEY ([Id]);
GO

ALTER TABLE [Centers] ADD CONSTRAINT [PK_Centers] PRIMARY KEY ([Id]);
GO

ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [StudentRegistrations] ADD CONSTRAINT [FK_StudentRegistrations_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [StudentRegistrations] ADD CONSTRAINT [FK_StudentRegistrations_Centers_CenterId] FOREIGN KEY ([CenterId]) REFERENCES [Centers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [PageContents] ADD CONSTRAINT [FK_PageContents_Centers_CenterId] FOREIGN KEY ([CenterId]) REFERENCES [Centers] ([Id]);
GO

ALTER TABLE [ExamResults] ADD CONSTRAINT [FK_ExamResults_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [ExamResults] ADD CONSTRAINT [FK_ExamResults_EntranceExams_EntranceExamId] FOREIGN KEY ([EntranceExamId]) REFERENCES [EntranceExams] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrollments] ADD CONSTRAINT [FK_Enrollments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Enrollments] ADD CONSTRAINT [FK_Enrollments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [CourseSubjects] ADD CONSTRAINT [FK_CourseSubjects_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Classes] ADD CONSTRAINT [FK_Classes_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Classes] ADD CONSTRAINT [FK_Classes_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id]);
GO

ALTER TABLE [ExamDetails] ADD CONSTRAINT [FK_ExamDetails_StudentRegistrations_RegistrationId] FOREIGN KEY ([RegistrationId]) REFERENCES [StudentRegistrations] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111120719_FixIdLengths', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Subjects] ADD [Image] nvarchar(max) NULL;
GO

ALTER TABLE [Courses] ADD [Image] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111151950_AddImageColumns', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Courses] ADD [Level] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260111153300_AddLevelToCourse', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [CourseReviews] (
    [Id] nvarchar(36) NOT NULL,
    [CourseId] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [Rating] int NOT NULL,
    [ReviewText] nvarchar(max) NOT NULL,
    [ReviewDate] datetime2 NOT NULL,
    [IsApproved] bit NOT NULL,
    CONSTRAINT [PK_CourseReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CourseReviews_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseReviews_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Lessons] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [ContentLink] nvarchar(max) NOT NULL,
    [Image] nvarchar(max) NULL,
    [DurationMinutes] int NOT NULL,
    [CourseId] nvarchar(36) NOT NULL,
    [SubjectId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Lessons] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Lessons_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Lessons_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Quizzes] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [TotalQuestions] int NOT NULL,
    [MaxScore] float NOT NULL,
    [DateCreated] datetime2 NOT NULL,
    [CourseId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_Quizzes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Quizzes_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [QuizQuestions] (
    [Id] nvarchar(36) NOT NULL,
    [QuestionText] nvarchar(max) NOT NULL,
    [Answer] nvarchar(max) NOT NULL,
    [QuizId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_QuizQuestions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_QuizQuestions_Quizzes_QuizId] FOREIGN KEY ([QuizId]) REFERENCES [Quizzes] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_CourseReviews_CourseId] ON [CourseReviews] ([CourseId]);
GO

CREATE INDEX [IX_CourseReviews_StudentId] ON [CourseReviews] ([StudentId]);
GO

CREATE INDEX [IX_Lessons_CourseId] ON [Lessons] ([CourseId]);
GO

CREATE INDEX [IX_Lessons_SubjectId] ON [Lessons] ([SubjectId]);
GO

CREATE INDEX [IX_QuizQuestions_QuizId] ON [QuizQuestions] ([QuizId]);
GO

CREATE INDEX [IX_Quizzes_CourseId] ON [Quizzes] ([CourseId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112182812_AddNewModels_Lesson_Quiz_Review', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Quizzes] DROP CONSTRAINT [FK_Quizzes_Courses_CourseId];
GO

DROP INDEX [IX_Quizzes_CourseId] ON [Quizzes];
GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Quizzes]') AND [c].[name] = N'CourseId');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [Quizzes] DROP CONSTRAINT [' + @var26 + '];');
ALTER TABLE [Quizzes] DROP COLUMN [CourseId];
GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Quizzes]') AND [c].[name] = N'MaxScore');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [Quizzes] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [Quizzes] DROP COLUMN [MaxScore];
GO

EXEC sp_rename N'[Quizzes].[TotalQuestions]', N'PassScore', N'COLUMN';
GO

EXEC sp_rename N'[Quizzes].[Title]', N'Name', N'COLUMN';
GO

EXEC sp_rename N'[QuizQuestions].[Answer]', N'OptionD', N'COLUMN';
GO

ALTER TABLE [Quizzes] ADD [LessonId] nvarchar(36) NULL;
GO

ALTER TABLE [QuizQuestions] ADD [CorrectOption] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [QuizQuestions] ADD [OptionA] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [QuizQuestions] ADD [OptionB] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [QuizQuestions] ADD [OptionC] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [QuizQuestions] ADD [Points] int NOT NULL DEFAULT 0;
GO

CREATE INDEX [IX_Quizzes_LessonId] ON [Quizzes] ([LessonId]);
GO

ALTER TABLE [Quizzes] ADD CONSTRAINT [FK_Quizzes_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260112192222_RefactorQuizAndQuestions', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Courses] ADD [CategoryId] nvarchar(36) NOT NULL DEFAULT N'1';
GO

CREATE TABLE [Categories] (
    [Id] nvarchar(36) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Materials] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [FileType] nvarchar(max) NOT NULL,
    [UploadDate] datetime2 NOT NULL,
    [ClassId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_Materials] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Materials_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([Id], [Description], [Name])
VALUES (N'1', N'Software Development Courses', N'Programming'),
(N'2', N'Music Theory and Instruments', N'Music'),
(N'3', N'Visual Arts and Design', N'Art');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] OFF;
GO

CREATE INDEX [IX_Courses_CategoryId] ON [Courses] ([CategoryId]);
GO

CREATE INDEX [IX_Materials_ClassId] ON [Materials] ([ClassId]);
GO

ALTER TABLE [Courses] ADD CONSTRAINT [FK_Courses_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260114065213_AddCategories', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Courses]') AND [c].[name] = N'Certification');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [Courses] DROP CONSTRAINT [' + @var28 + '];');
ALTER TABLE [Courses] DROP COLUMN [Certification];
GO

ALTER TABLE [Courses] ADD [CertificateId] nvarchar(36) NOT NULL DEFAULT N'1';
GO

CREATE TABLE [Certificates] (
    [Id] nvarchar(36) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Certificates] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Certificates]'))
    SET IDENTITY_INSERT [Certificates] ON;
INSERT INTO [Certificates] ([Id], [Description], [IsActive], [Name])
VALUES (N'1', N'Awarded upon completing all course requirements.', CAST(1 AS bit), N'Certificate of Completion'),
(N'2', N'Recognized industry standard certification.', CAST(1 AS bit), N'Professional Certification');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Certificates]'))
    SET IDENTITY_INSERT [Certificates] OFF;
GO

CREATE INDEX [IX_Courses_CertificateId] ON [Courses] ([CertificateId]);
GO

ALTER TABLE [Courses] ADD CONSTRAINT [FK_Courses_Certificates_CertificateId] FOREIGN KEY ([CertificateId]) REFERENCES [Certificates] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260114071625_AddCertificates', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Subjects] ADD [LearningRoadmap] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260114121639_AddLearningRoadmapToSubject', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [CourseInstructors] (
    [CourseId] nvarchar(36) NOT NULL,
    [InstructorId] nvarchar(36) NOT NULL,
    CONSTRAINT [PK_CourseInstructors] PRIMARY KEY ([CourseId], [InstructorId]),
    CONSTRAINT [FK_CourseInstructors_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseInstructors_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_CourseInstructors_InstructorId] ON [CourseInstructors] ([InstructorId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260114145624_AddCourseInstructors', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260114150529_AddCourseInstructorsSchema', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [InstructorProfiles] (
    [Id] nvarchar(36) NOT NULL,
    [UserId] nvarchar(36) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [Gender] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [AddressLine] nvarchar(max) NOT NULL,
    [AvatarUrl] nvarchar(max) NOT NULL,
    [YearsOfExperience] int NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [Bio] nvarchar(max) NOT NULL,
    [Certifications] nvarchar(max) NOT NULL,
    [GithubUrl] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_InstructorProfiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_InstructorProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [StudentProfiles] (
    [Id] nvarchar(36) NOT NULL,
    [UserId] nvarchar(36) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [DateOfBirth] datetime2 NULL,
    [Gender] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [AddressLine] nvarchar(max) NOT NULL,
    [AvatarUrl] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_StudentProfiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_InstructorProfiles_UserId] ON [InstructorProfiles] ([UserId]);
GO

CREATE INDEX [IX_StudentProfiles_UserId] ON [StudentProfiles] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260114162727_AddUserProfiles', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Payments] DROP CONSTRAINT [FK_Payments_Users_StudentId];
GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payments]') AND [c].[name] = N'StudentId');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [Payments] DROP CONSTRAINT [' + @var29 + '];');
ALTER TABLE [Payments] ALTER COLUMN [StudentId] nvarchar(36) NULL;
GO

ALTER TABLE [Payments] ADD [GuestId] nvarchar(36) NULL;
GO

ALTER TABLE [Enrollments] ADD [CourseId] nvarchar(36) NULL;
GO

ALTER TABLE [Enrollments] ADD [IsCompleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

CREATE TABLE [Guests] (
    [Id] nvarchar(36) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [Dob] datetime2 NOT NULL,
    [Address] nvarchar(255) NOT NULL,
    [SelectedEntranceExamId] nvarchar(36) NULL,
    [Status] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UserId] nvarchar(36) NULL,
    CONSTRAINT [PK_Guests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Guests_EntranceExams_SelectedEntranceExamId] FOREIGN KEY ([SelectedEntranceExamId]) REFERENCES [EntranceExams] ([Id]),
    CONSTRAINT [FK_Guests_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);
GO

CREATE INDEX [IX_Payments_GuestId] ON [Payments] ([GuestId]);
GO

CREATE INDEX [IX_Enrollments_CourseId] ON [Enrollments] ([CourseId]);
GO

CREATE INDEX [IX_Guests_SelectedEntranceExamId] ON [Guests] ([SelectedEntranceExamId]);
GO

CREATE INDEX [IX_Guests_UserId] ON [Guests] ([UserId]);
GO

ALTER TABLE [Enrollments] ADD CONSTRAINT [FK_Enrollments_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]);
GO

ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_Guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [Guests] ([Id]);
GO

ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260116144230_AddGuestRegistration', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([Id], [Description], [Name])
VALUES (N'4', N'Prospective Student', N'Guest');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Description', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260116150555_AddGuestRole', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Guests] ADD [ClassId] nvarchar(36) NULL;
GO

ALTER TABLE [Guests] ADD [Description] nvarchar(max) NULL;
GO

ALTER TABLE [Guests] ADD [ExamRoom] nvarchar(max) NULL;
GO

CREATE INDEX [IX_Guests_ClassId] ON [Guests] ([ClassId]);
GO

ALTER TABLE [Guests] ADD CONSTRAINT [FK_Guests_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260116173453_UpdateGuestSchema', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DELETE FROM [Certificates]
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

ALTER TABLE [Classes] ADD [ClassCategoryId] nvarchar(36) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Classes] ADD [ClassroomId] nvarchar(36) NULL;
GO

CREATE TABLE [ClassCategories] (
    [Id] nvarchar(36) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [RequiredRoomType] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_ClassCategories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Classrooms] (
    [Id] nvarchar(36) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [RoomType] nvarchar(max) NOT NULL,
    [Capacity] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Classrooms] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsActive', N'Name', N'RequiredRoomType') AND [object_id] = OBJECT_ID(N'[ClassCategories]'))
    SET IDENTITY_INSERT [ClassCategories] ON;
INSERT INTO [ClassCategories] ([Id], [IsActive], [Name], [RequiredRoomType])
VALUES (N'1', CAST(1 AS bit), N'Theory', N'TheoryRoom'),
(N'2', CAST(1 AS bit), N'Lab', N'LabRoom'),
(N'3', CAST(1 AS bit), N'Online', N'Online');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsActive', N'Name', N'RequiredRoomType') AND [object_id] = OBJECT_ID(N'[ClassCategories]'))
    SET IDENTITY_INSERT [ClassCategories] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'IsActive', N'Name', N'RoomType') AND [object_id] = OBJECT_ID(N'[Classrooms]'))
    SET IDENTITY_INSERT [Classrooms] ON;
INSERT INTO [Classrooms] ([Id], [Capacity], [IsActive], [Name], [RoomType])
VALUES (N'1', 30, CAST(1 AS bit), N'Room 101', N'TheoryRoom'),
(N'2', 20, CAST(1 AS bit), N'Lab A', N'LabRoom');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'IsActive', N'Name', N'RoomType') AND [object_id] = OBJECT_ID(N'[Classrooms]'))
    SET IDENTITY_INSERT [Classrooms] OFF;
GO

UPDATE Classes SET ClassCategoryId = '1' WHERE ClassCategoryId = '' OR ClassCategoryId IS NULL
GO

CREATE INDEX [IX_Classes_ClassCategoryId] ON [Classes] ([ClassCategoryId]);
GO

CREATE INDEX [IX_Classes_ClassroomId] ON [Classes] ([ClassroomId]);
GO

ALTER TABLE [Classes] ADD CONSTRAINT [FK_Classes_ClassCategories_ClassCategoryId] FOREIGN KEY ([ClassCategoryId]) REFERENCES [ClassCategories] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Classes] ADD CONSTRAINT [FK_Classes_Classrooms_ClassroomId] FOREIGN KEY ([ClassroomId]) REFERENCES [Classrooms] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260116180121_UpdateClassSchemaWithCategories', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Classes] DROP CONSTRAINT [FK_Classes_Classrooms_ClassroomId];
GO

ALTER TABLE [Classes] DROP CONSTRAINT [FK_Classes_Courses_CourseId];
GO

ALTER TABLE [Classes] DROP CONSTRAINT [FK_Classes_Users_InstructorId];
GO

DROP TABLE [Classrooms];
GO

DROP INDEX [IX_Classes_ClassroomId] ON [Classes];
GO

DROP INDEX [IX_Classes_CourseId] ON [Classes];
GO

DROP INDEX [IX_Classes_InstructorId] ON [Classes];
GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Classes]') AND [c].[name] = N'ClassroomId');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [Classes] DROP CONSTRAINT [' + @var30 + '];');
ALTER TABLE [Classes] DROP COLUMN [ClassroomId];
GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Classes]') AND [c].[name] = N'CourseId');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [Classes] DROP CONSTRAINT [' + @var31 + '];');
ALTER TABLE [Classes] DROP COLUMN [CourseId];
GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Classes]') AND [c].[name] = N'EndDate');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [Classes] DROP CONSTRAINT [' + @var32 + '];');
ALTER TABLE [Classes] DROP COLUMN [EndDate];
GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Classes]') AND [c].[name] = N'InstructorId');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [Classes] DROP CONSTRAINT [' + @var33 + '];');
ALTER TABLE [Classes] DROP COLUMN [InstructorId];
GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Classes]') AND [c].[name] = N'IsOnline');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [Classes] DROP CONSTRAINT [' + @var34 + '];');
ALTER TABLE [Classes] DROP COLUMN [IsOnline];
GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Classes]') AND [c].[name] = N'OfflineFee');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [Classes] DROP CONSTRAINT [' + @var35 + '];');
ALTER TABLE [Classes] DROP COLUMN [OfflineFee];
GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Classes]') AND [c].[name] = N'Room');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [Classes] DROP CONSTRAINT [' + @var36 + '];');
ALTER TABLE [Classes] DROP COLUMN [Room];
GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassCategories]') AND [c].[name] = N'RequiredRoomType');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [ClassCategories] DROP CONSTRAINT [' + @var37 + '];');
ALTER TABLE [ClassCategories] DROP COLUMN [RequiredRoomType];
GO

EXEC sp_rename N'[Classes].[StartDate]', N'CreatedAt', N'COLUMN';
GO

EXEC sp_rename N'[Classes].[Name]', N'Status', N'COLUMN';
GO

ALTER TABLE [Classes] ADD [ClassName] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Classes] ADD [NumberOfSeats] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [ClassCategories] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [ClassCategories] ADD [Description] nvarchar(max) NULL;
GO

CREATE TABLE [Assignments] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [TermOrExamName] nvarchar(max) NULL,
    [ClassId] nvarchar(36) NOT NULL,
    [InstructorId] nvarchar(36) NOT NULL,
    [AssignmentType] nvarchar(max) NOT NULL,
    [Note] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    [CancellationReason] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Assignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Assignments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Assignments_Users_InstructorId] FOREIGN KEY ([InstructorId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-17T02:42:34.5865455+07:00', [Description] = N'Standard classrooms'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-17T02:42:34.5865470+07:00', [Description] = N'Computer labs'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-17T02:42:34.5865471+07:00', [Description] = N'Virtual classes'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_Assignments_ClassId] ON [Assignments] ([ClassId]);
GO

CREATE INDEX [IX_Assignments_InstructorId] ON [Assignments] ([InstructorId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260116194235_RefactorClassAndAssignments', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE Classes SET Status = 'Active' WHERE Status = N'Thi thử'
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-17T02:46:17.9001555+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-17T02:46:17.9001570+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-17T02:46:17.9001571+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260116194618_FixClassStatusData', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ChatMessages] (
    [Id] int NOT NULL IDENTITY,
    [SenderId] nvarchar(36) NULL,
    [ReceiverId] nvarchar(36) NULL,
    [Content] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    [SessionId] nvarchar(max) NULL,
    [SenderValidName] nvarchar(max) NULL,
    CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChatMessages_Users_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_ChatMessages_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id])
);
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-20T00:06:30.8533121+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-20T00:06:30.8533191+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-20T00:06:30.8533192+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_ChatMessages_ReceiverId] ON [ChatMessages] ([ReceiverId]);
GO

CREATE INDEX [IX_ChatMessages_SenderId] ON [ChatMessages] ([SenderId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260119170632_AddChatFeature', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-20T08:49:53.1830822+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-20T08:49:53.1830837+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-20T08:49:53.1830839+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260120014954_InitDb', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Payments] ADD [Status] int NOT NULL DEFAULT 0;
GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Guests]') AND [c].[name] = N'PhoneNumber');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [Guests] DROP CONSTRAINT [' + @var38 + '];');
ALTER TABLE [Guests] ALTER COLUMN [PhoneNumber] nvarchar(15) NOT NULL;
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-20T10:15:06.2080623+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-20T10:15:06.2080641+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-20T10:15:06.2080644+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260120031507_AddPaymentStatus', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Guests] ADD [Password] nvarchar(100) NOT NULL DEFAULT N'';
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-21T01:57:56.8130254+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-21T01:57:56.8130279+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-21T01:57:56.8130280+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260120185758_AddPasswordToUser', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Guests]') AND [c].[name] = N'Password');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [Guests] DROP CONSTRAINT [' + @var39 + '];');
ALTER TABLE [Guests] DROP COLUMN [Password];
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-21T02:09:57.3704567+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-21T02:09:57.3704582+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-21T02:09:57.3704583+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260120190957_RemovePasswordFromGuest', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

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
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-22T15:15:16.4839678+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-22T15:15:16.4839695+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-22T15:15:16.4839696+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260122081517_AddNotificationTable', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ClassAssignments] (
    [Id] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [ClassId] nvarchar(36) NOT NULL,
    [AssignedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ClassAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClassAssignments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ClassAssignments_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-27T17:32:42.3032037+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-27T17:32:42.3032050+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-27T17:32:42.3032052+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_ClassAssignments_ClassId] ON [ClassAssignments] ([ClassId]);
GO

CREATE INDEX [IX_ClassAssignments_StudentId] ON [ClassAssignments] ([StudentId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260127103242_xeplop', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [FAQs] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T09:39:42.1279049+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T09:39:42.1279077+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T09:39:42.1279081+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260128023943_new', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [VNPayTransactions] (
    [Id] nvarchar(450) NOT NULL,
    [PaymentId] nvarchar(36) NOT NULL,
    [VnpTxnRef] nvarchar(50) NOT NULL,
    [VnpAmount] bigint NOT NULL,
    [VnpOrderInfo] nvarchar(255) NOT NULL,
    [VnpOrderType] nvarchar(50) NOT NULL,
    [VnpCreateDate] nvarchar(14) NOT NULL,
    [VnpResponseCode] nvarchar(10) NOT NULL,
    [VnpTransactionNo] nvarchar(50) NOT NULL,
    [VnpBankCode] nvarchar(20) NOT NULL,
    [VnpBankTranNo] nvarchar(50) NOT NULL,
    [VnpCardType] nvarchar(20) NOT NULL,
    [VnpPayDate] nvarchar(14) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_VNPayTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VNPayTransactions_Payments_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payments] ([Id]) ON DELETE CASCADE
);
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T10:19:00.5740010+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T10:19:00.5740036+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T10:19:00.5740037+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_VNPayTransactions_PaymentId] ON [VNPayTransactions] ([PaymentId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260128031902_UpdatePayment', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VNPayTransactions]') AND [c].[name] = N'CreatedAt');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [VNPayTransactions] DROP CONSTRAINT [' + @var40 + '];');
ALTER TABLE [VNPayTransactions] DROP COLUMN [CreatedAt];
GO

DECLARE @var41 sysname;
SELECT @var41 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VNPayTransactions]') AND [c].[name] = N'VnpBankTranNo');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [VNPayTransactions] DROP CONSTRAINT [' + @var41 + '];');
ALTER TABLE [VNPayTransactions] DROP COLUMN [VnpBankTranNo];
GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VNPayTransactions]') AND [c].[name] = N'VnpCardType');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [VNPayTransactions] DROP CONSTRAINT [' + @var42 + '];');
ALTER TABLE [VNPayTransactions] DROP COLUMN [VnpCardType];
GO

DECLARE @var43 sysname;
SELECT @var43 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VNPayTransactions]') AND [c].[name] = N'VnpOrderType');
IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [VNPayTransactions] DROP CONSTRAINT [' + @var43 + '];');
ALTER TABLE [VNPayTransactions] DROP COLUMN [VnpOrderType];
GO

DECLARE @var44 sysname;
SELECT @var44 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VNPayTransactions]') AND [c].[name] = N'VnpTxnRef');
IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [VNPayTransactions] DROP CONSTRAINT [' + @var44 + '];');
ALTER TABLE [VNPayTransactions] ALTER COLUMN [VnpTxnRef] nvarchar(max) NOT NULL;
GO

DECLARE @var45 sysname;
SELECT @var45 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VNPayTransactions]') AND [c].[name] = N'VnpTransactionNo');
IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [VNPayTransactions] DROP CONSTRAINT [' + @var45 + '];');
ALTER TABLE [VNPayTransactions] ALTER COLUMN [VnpTransactionNo] nvarchar(max) NULL;
GO

DECLARE @var46 sysname;
SELECT @var46 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VNPayTransactions]') AND [c].[name] = N'VnpResponseCode');
IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [VNPayTransactions] DROP CONSTRAINT [' + @var46 + '];');
ALTER TABLE [VNPayTransactions] ALTER COLUMN [VnpResponseCode] nvarchar(max) NULL;
GO

DECLARE @var47 sysname;
SELECT @var47 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VNPayTransactions]') AND [c].[name] = N'VnpPayDate');
IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [VNPayTransactions] DROP CONSTRAINT [' + @var47 + '];');
ALTER TABLE [VNPayTransactions] ALTER COLUMN [VnpPayDate] nvarchar(max) NULL;
GO

DECLARE @var48 sysname;
SELECT @var48 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VNPayTransactions]') AND [c].[name] = N'VnpOrderInfo');
IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [VNPayTransactions] DROP CONSTRAINT [' + @var48 + '];');
ALTER TABLE [VNPayTransactions] ALTER COLUMN [VnpOrderInfo] nvarchar(max) NOT NULL;
GO

DECLARE @var49 sysname;
SELECT @var49 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VNPayTransactions]') AND [c].[name] = N'VnpCreateDate');
IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [VNPayTransactions] DROP CONSTRAINT [' + @var49 + '];');
ALTER TABLE [VNPayTransactions] ALTER COLUMN [VnpCreateDate] nvarchar(max) NOT NULL;
GO

DECLARE @var50 sysname;
SELECT @var50 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VNPayTransactions]') AND [c].[name] = N'VnpBankCode');
IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [VNPayTransactions] DROP CONSTRAINT [' + @var50 + '];');
ALTER TABLE [VNPayTransactions] ALTER COLUMN [VnpBankCode] nvarchar(max) NULL;
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T10:45:20.7509178+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T10:45:20.7509203+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T10:45:20.7509205+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260128034522_AddVNPayTransaction', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [EntranceExams] ADD [IsRegistrationOpen] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [EntranceExams] ADD [MaxCandidates] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [EntranceExams] ADD [Status] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [EntranceExams] ADD [Subjects] nvarchar(max) NOT NULL DEFAULT N'';
GO

CREATE TABLE [ExamPapers] (
    [Id] nvarchar(36) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Duration] int NOT NULL,
    [SubjectId] nvarchar(450) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [EntranceExamId] nvarchar(36) NULL,
    CONSTRAINT [PK_ExamPapers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamPapers_EntranceExams_EntranceExamId] FOREIGN KEY ([EntranceExamId]) REFERENCES [EntranceExams] ([Id]),
    CONSTRAINT [FK_ExamPapers_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id])
);
GO

CREATE TABLE [Questions] (
    [Id] nvarchar(36) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [SubjectId] nvarchar(450) NULL,
    [Difficulty] nvarchar(max) NOT NULL,
    [Score] float NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Questions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Questions_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id])
);
GO

CREATE TABLE [StudentExamSessions] (
    [Id] nvarchar(36) NOT NULL,
    [EntranceExamId] nvarchar(36) NOT NULL,
    [StudentId] nvarchar(36) NOT NULL,
    [ExamPaperId] nvarchar(36) NOT NULL,
    [StartTime] datetime2 NULL,
    [EndTime] datetime2 NULL,
    [TotalScore] float NOT NULL,
    [GradeLevel] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_StudentExamSessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentExamSessions_EntranceExams_EntranceExamId] FOREIGN KEY ([EntranceExamId]) REFERENCES [EntranceExams] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentExamSessions_ExamPapers_ExamPaperId] FOREIGN KEY ([ExamPaperId]) REFERENCES [ExamPapers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentExamSessions_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ExamPaperQuestions] (
    [Id] int NOT NULL IDENTITY,
    [ExamPaperId] nvarchar(36) NOT NULL,
    [QuestionId] nvarchar(36) NOT NULL,
    [Order] int NOT NULL,
    CONSTRAINT [PK_ExamPaperQuestions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamPaperQuestions_ExamPapers_ExamPaperId] FOREIGN KEY ([ExamPaperId]) REFERENCES [ExamPapers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ExamPaperQuestions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [QuestionOptions] (
    [Id] nvarchar(36) NOT NULL,
    [QuestionId] nvarchar(36) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [IsCorrect] bit NOT NULL,
    CONSTRAINT [PK_QuestionOptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_QuestionOptions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [StudentAnswers] (
    [Id] int NOT NULL IDENTITY,
    [SessionId] nvarchar(36) NOT NULL,
    [QuestionId] nvarchar(36) NOT NULL,
    [SelectedOptionId] nvarchar(max) NULL,
    [EssayContent] nvarchar(max) NULL,
    [EarnedScore] float NOT NULL,
    [IsGraded] bit NOT NULL,
    [ExaminerNote] nvarchar(max) NULL,
    CONSTRAINT [PK_StudentAnswers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentAnswers_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudentAnswers_StudentExamSessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [StudentExamSessions] ([Id]) ON DELETE CASCADE
);
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T21:14:29.2007563+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T21:14:29.2007607+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-28T21:14:29.2007608+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_ExamPaperQuestions_ExamPaperId] ON [ExamPaperQuestions] ([ExamPaperId]);
GO

CREATE INDEX [IX_ExamPaperQuestions_QuestionId] ON [ExamPaperQuestions] ([QuestionId]);
GO

CREATE INDEX [IX_ExamPapers_EntranceExamId] ON [ExamPapers] ([EntranceExamId]);
GO

CREATE INDEX [IX_ExamPapers_SubjectId] ON [ExamPapers] ([SubjectId]);
GO

CREATE INDEX [IX_QuestionOptions_QuestionId] ON [QuestionOptions] ([QuestionId]);
GO

CREATE INDEX [IX_Questions_SubjectId] ON [Questions] ([SubjectId]);
GO

CREATE INDEX [IX_StudentAnswers_QuestionId] ON [StudentAnswers] ([QuestionId]);
GO

CREATE INDEX [IX_StudentAnswers_SessionId] ON [StudentAnswers] ([SessionId]);
GO

CREATE INDEX [IX_StudentExamSessions_EntranceExamId] ON [StudentExamSessions] ([EntranceExamId]);
GO

CREATE INDEX [IX_StudentExamSessions_ExamPaperId] ON [StudentExamSessions] ([ExamPaperId]);
GO

CREATE INDEX [IX_StudentExamSessions_StudentId] ON [StudentExamSessions] ([StudentId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260128141430_UpdateEntranceExamFieldsV2', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [EntranceExams] ADD [ExamPaperId] nvarchar(36) NULL;
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T00:32:07.5429413+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T00:32:07.5429427+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T00:32:07.5429428+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_EntranceExams_ExamPaperId] ON [EntranceExams] ([ExamPaperId]);
GO

ALTER TABLE [EntranceExams] ADD CONSTRAINT [FK_EntranceExams_ExamPapers_ExamPaperId] FOREIGN KEY ([ExamPaperId]) REFERENCES [ExamPapers] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260128173208_AddExamPaperToEntranceExam', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [RevisionPackages] (
    [Id] nvarchar(450) NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Fee] decimal(18,2) NOT NULL,
    [MaxStudents] int NOT NULL,
    [CurrentStudents] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_RevisionPackages] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [RevisionRegistrations] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [RevisionPackageId] nvarchar(450) NOT NULL,
    [ClassId] nvarchar(36) NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_RevisionRegistrations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RevisionRegistrations_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]),
    CONSTRAINT [FK_RevisionRegistrations_RevisionPackages_RevisionPackageId] FOREIGN KEY ([RevisionPackageId]) REFERENCES [RevisionPackages] ([Id]) ON DELETE CASCADE
);
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T08:42:23.3027261+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T08:42:23.3027284+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T08:42:23.3027286+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_RevisionRegistrations_ClassId] ON [RevisionRegistrations] ([ClassId]);
GO

CREATE INDEX [IX_RevisionRegistrations_RevisionPackageId] ON [RevisionRegistrations] ([RevisionPackageId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260129014225_RevisionGuests', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Payments] ADD [CourseId] nvarchar(36) NULL;
GO

ALTER TABLE [Payments] ADD [Purpose] int NOT NULL DEFAULT 0;
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T13:30:42.7931313+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T13:30:42.7931338+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T13:30:42.7931340+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_Payments_CourseId] ON [Payments] ([CourseId]);
GO

ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_Courses_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [Courses] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260129063044_RegisterCours', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ClassLessons] (
    [Id] nvarchar(36) NOT NULL,
    [ClassId] nvarchar(36) NOT NULL,
    [LessonId] nvarchar(36) NOT NULL,
    [AssignedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ClassLessons] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClassLessons_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ClassLessons_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([Id]) ON DELETE CASCADE
);
GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T19:41:19.1231187+07:00'
WHERE [Id] = N'1';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T19:41:19.1231199+07:00'
WHERE [Id] = N'2';
SELECT @@ROWCOUNT;

GO

UPDATE [ClassCategories] SET [CreatedAt] = '2026-01-29T19:41:19.1231200+07:00'
WHERE [Id] = N'3';
SELECT @@ROWCOUNT;

GO

CREATE UNIQUE INDEX [IX_ClassLessons_ClassId_LessonId] ON [ClassLessons] ([ClassId], [LessonId]);
GO

CREATE INDEX [IX_ClassLessons_LessonId] ON [ClassLessons] ([LessonId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260129124119_AddCancelReasonAndClassLesson', N'8.0.0');
GO

COMMIT;
GO

