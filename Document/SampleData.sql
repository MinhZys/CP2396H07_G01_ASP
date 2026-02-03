-- 1. ADD MORE CERTIFICATES (Total 5)
INSERT INTO [Certificates] ([Id], [Name], [Description], [IsActive]) VALUES
(NEWID(), N'PHP Web Development', N'Advanced PHP and Laravel certificate', 1),
(NEWID(), N'Java Enterprise', N'Java Spring Boot and Microservices', 1),
(NEWID(), N'Data Analytics', N'Python and PowerBI certification', 1),
(NEWID(), N'UI/UX Design', N'Modern Design Principles', 1);

-- 1.1 ADD MORE COURSE CATEGORIES (Total 5 new)
DECLARE @Cat1 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Cat2 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Cat3 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Cat4 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Cat5 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));

INSERT INTO [Categories] ([Id], [Name], [Description]) VALUES
(@Cat1, N'Marketing', N'Digital Marketing and SEO'),
(@Cat2, N'Business', N'Business Management and Finance'),
(@Cat3, N'Languages', N'English, Japanese, and Korean courses'),
(@Cat4, N'Soft Skills', N'Communication and Leadership'),
(@Cat5, N'Health & Fitness', N'Health and Personal Training');

-- 1.2 ADD MORE CLASS CATEGORIES (Total 5 new)
DECLARE @ClsCat1 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @ClsCat2 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @ClsCat3 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @ClsCat4 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @ClsCat5 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));

INSERT INTO [ClassCategories] ([Id], [Name], [Description], [IsActive], [CreatedAt]) VALUES
(@ClsCat1, N'Workshop', N'Short-term intensive practical sessions', 1, GETDATE()),
(@ClsCat2, N'Seminar', N'Academic discussion groups', 1, GETDATE()),
(@ClsCat3, N'Bootcamp', N'Accelerated learning programs', 1, GETDATE()),
(@ClsCat4, N'One-on-One', N'Private tutoring sessions', 1, GETDATE()),
(@ClsCat5, N'Evening Class', N'Classes scheduled after 6 PM', 1, GETDATE());

-- 2. SUBJECTS & ROADMAPS (10 Subjects)
DECLARE @Sub1 NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
DECLARE @Sub2 NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
DECLARE @Sub3 NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
DECLARE @Sub4 NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
DECLARE @Sub5 NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
DECLARE @Sub6 NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
DECLARE @Sub7 NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
DECLARE @Sub8 NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
DECLARE @Sub9 NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
DECLARE @Sub10 NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));

INSERT INTO [Subjects] ([Id], [Name], [StudyTime], [Description], [LearningRoadmap], [Image]) VALUES
(@Sub1, N'C# Basics', 40, N'Introduction to C# programming', N'Week 1: Syntax; Week 2: OOP; Week 3: Collections; Week 4: Final Project', 'https://example.com/csharp.png'),
(@Sub2, N'Advanced SQL', 30, N'Database optimization and complex queries', N'Week 1: Indices; Week 2: Stored Procedures; Week 3: Performance Tuning', 'https://example.com/sql.png'),
(@Sub3, N'ASP.NET Core', 50, N'Building web apps with .NET', N'Module 1: MVC; Module 2: Web API; Module 3: Entity Framework Core', 'https://example.com/aspnet.png'),
(@Sub4, N'React JS', 45, N'Modern frontend development', N'Intro to JSX -> Hooks -> Redux -> Project Implementation', 'https://example.com/react.png'),
(@Sub5, N'Python for Data Science', 60, N'Pandas, Numpy and Matplotlib', N'Data Cleaning -> Analysis -> Visualization -> Machine Learning Intro', 'https://example.com/python.png'),
(@Sub6, N'Java Programming', 40, N'Core Java concepts', N'Basics -> OOP -> Exceptions -> Multithreading', 'https://example.com/java.png'),
(@Sub7, N'HTML/CSS Mastery', 20, N'Responsive web design', N'HTML5 -> CSS3 Flexbox/Grid -> Responsive Web Design', 'https://example.com/htmlcss.png'),
(@Sub8, N'Node.js Backend', 40, N'Server-side Javascript', N'Express.js -> Authentication -> REST APIs -> Deployment', 'https://example.com/node.png'),
(@Sub9, N'UI Architecture', 30, N'Design patterns for interfaces', N'Design Thinking -> Wireframing -> Prototyping in Figma', 'https://example.com/ui.png'),
(@Sub10, N'Cyber Security', 50, N'Basics of network security', N'Threats -> Encryption -> Network Security -> Ethical Hacking', 'https://example.com/security.png');

-- 3. COURSES (5 Courses)
-- Using one of the newly created categories to ensure FK success
DECLARE @CertProf NVARCHAR(36) = '2'; -- This must exist from seed. If not, use one from above.

DECLARE @Course1 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Course2 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Course3 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Course4 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Course5 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));

INSERT INTO [Courses] ([Id], [Title], [Description], [TuitionFee], [DurationMonths], [CertificateId], [IsActive], [Level], [CategoryId]) VALUES
(@Course1, N'Fullstack .NET Developer', N'Become a professional .NET developer', 1500, 6, '2', 1, 'Advanced', @Cat1),
(@Course2, N'Data Science Specialization', N'Master data analysis and ML', 2000, 8, '2', 1, 'Intermediate', @Cat2),
(@Course3, N'Modern Frontend Mastery', N'Master React and modern UI', 1200, 4, '2', 1, 'Beginner', @Cat1),
(@Course4, N'Enterprise Java Development', N'Build scalable Java applications', 1800, 6, '2', 1, 'Advanced', @Cat2),
(@Course5, N'Cyber Security Associate', N'Protect digital assets', 2500, 12, '2', 1, 'Intermediate', @Cat4);

-- Link Subjects to Courses
INSERT INTO [CourseSubjects] ([CourseId], [SubjectId]) VALUES
(@Course1, @Sub1), (@Course1, @Sub2), (@Course1, @Sub3),
(@Course2, @Sub2), (@Course2, @Sub5),
(@Course3, @Sub7), (@Course3, @Sub4), (@Course3, @Sub9),
(@Course4, @Sub6), (@Course4, @Sub2), (@Course4, @Sub8),
(@Course5, @Sub10), (@Course5, @Sub1);

-- 4. USERS & PROFILES (10 Students, 10 Instructors)
DECLARE @i INT = 1;
WHILE @i <= 10
BEGIN
    DECLARE @StudentId NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
    INSERT INTO [Users] ([Id], [FullName], [Email], [Password], [IsActive], [RoleId])
    VALUES (@StudentId, 'Student User ' + CAST(@i AS NVARCHAR), 'student' + CAST(@i AS NVARCHAR) + '@example.com', 'pass123', 1, '3');
    
    INSERT INTO [StudentProfiles] ([Id], [UserId], [FullName], [DateOfBirth], [Gender], [PhoneNumber], [AddressLine], [AvatarUrl])
    VALUES (CAST(NEWID() AS NVARCHAR(36)), @StudentId, 'Student User ' + CAST(@i AS NVARCHAR), DATEADD(YEAR, -20-@i, GETDATE()), CASE WHEN @i % 2 = 0 THEN 'Male' ELSE 'Female' END, '090123456' + CAST(@i-1 AS NVARCHAR), 'Street ' + CAST(@i AS NVARCHAR) + ', City', 'default_avatar.png');
    
    SET @i = @i + 1;
END

SET @i = 1;
WHILE @i <= 10
BEGIN
    DECLARE @InstId NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
    INSERT INTO [Users] ([Id], [FullName], [Email], [Password], [IsActive], [RoleId])
    VALUES (@InstId, 'Instructor ' + CAST(@i AS NVARCHAR), 'instructor' + CAST(@i AS NVARCHAR) + '@example.com', 'pass123', 1, '2');
    
    INSERT INTO [InstructorProfiles] ([Id], [UserId], [FullName], [DateOfBirth], [Gender], [PhoneNumber], [AddressLine], [AvatarUrl], [YearsOfExperience], [Specialization], [Bio], [Certifications], [GithubUrl])
    VALUES (CAST(NEWID() AS NVARCHAR(36)), @InstId, 'Instructor ' + CAST(@i AS NVARCHAR), DATEADD(YEAR, -30-@i, GETDATE()), CASE WHEN @i % 2 = 1 THEN 'Male' ELSE 'Female' END, '09887766' + CAST(@i-1 AS NVARCHAR), 'Tech Lane ' + CAST(@i AS NVARCHAR), 'inst_avatar.png', 5 + @i, N'Expert in Subject ' + CAST(@i AS NVARCHAR), N'Experienced professional with a passion for teaching.', N'Standard teaching license', 'https://github.com/inst' + CAST(@i AS NVARCHAR));
    
    INSERT INTO [CourseInstructors] ([CourseId], [InstructorId]) 
    SELECT TOP 1 Id, @InstId FROM Courses WHERE Id IN (@Course1, @Course2, @Course3, @Course4, @Course5) ORDER BY NEWID();

    SET @i = @i + 1;
END

-- 5. GUEST REGISTRATIONS (10 Guests)
SET @i = 1;
WHILE @i <= 10
BEGIN
    INSERT INTO [Guests] ([Id], [FullName], [Email], [PhoneNumber], [Dob], [Address], [Status], [CreatedAt])
    VALUES (CAST(NEWID() AS NVARCHAR(36)), 'Guest ' + CAST(@i AS NVARCHAR), 'guest' + CAST(@i AS NVARCHAR) + '@gmail.com', '033344455' + CAST(@i-1 AS NVARCHAR), '2000-01-01', 'Visitor Home ' + CAST(@i AS NVARCHAR), 'Pending', GETDATE());
    SET @i = @i + 1;
END

-- 6. ENTRANCE EXAMS (5 Exams)
INSERT INTO [EntranceExams] ([Id], [Title], [ExamDate], [Fee], [MaxCandidates], [Status], [IsRegistrationOpen], [IsActive], [Subjects]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), N'Spring 2026 Entrance - IT', DATEADD(DAY, 30, GETDATE()), 50, 100, 'NotStarted', 1, 1, N'Math, IT'),
(CAST(NEWID() AS NVARCHAR(36)), N'Summer 2026 Entrance - Music', DATEADD(DAY, 60, GETDATE()), 40, 50, 'NotStarted', 1, 1, N'Music Theory'),
(CAST(NEWID() AS NVARCHAR(36)), N'Fall 2026 Entrance - General', DATEADD(DAY, 90, GETDATE()), 30, 200, 'NotStarted', 1, 1, N'General Knowledge'),
(CAST(NEWID() AS NVARCHAR(36)), N'Winter 2026 Scholarship', DATEADD(DAY, 120, GETDATE()), 0, 50, 'NotStarted', 1, 1, N'IQ, Logic'),
(CAST(NEWID() AS NVARCHAR(36)), N'Monthly Placement Test', DATEADD(DAY, 15, GETDATE()), 20, 30, 'Ongoing', 1, 1, N'English');

-- 7. CLASSES (5 Classes)
INSERT INTO [Classes] ([Id], [ClassName], [ClassCategoryId], [NumberOfSeats], [Status], [CreatedAt], [Fee]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), N'NET-CORE-001', @ClsCat1, 30, '1', GETDATE(), 100),
(CAST(NEWID() AS NVARCHAR(36)), N'REACT-JS-002', @ClsCat2, 20, '1', GETDATE(), 120),
(CAST(NEWID() AS NVARCHAR(36)), N'DATA-SCI-003', @ClsCat1, 50, '1', GETDATE(), 150),
(CAST(NEWID() AS NVARCHAR(36)), N'JAVA-ENT-004', @ClsCat3, 25, '1', GETDATE(), 110),
(CAST(NEWID() AS NVARCHAR(36)), N'CYBER-SEC-005', @ClsCat4, 40, '1', GETDATE(), 200);



-- 9. COURSE REVIEWS (5/Course = 25 total)
INSERT INTO [CourseReviews] ([Id], [CourseId], [StudentId], [Rating], [ReviewText], [ReviewDate], [IsApproved])
SELECT TOP 25 CAST(NEWID() AS NVARCHAR(36)), c.Id, s.Id, (ABS(CHECKSUM(NEWID())) % 3) + 3, N'Great course! Highly recommended.', GETDATE(), 1
FROM Courses c, Users s
WHERE s.RoleId = '3'
ORDER BY NEWID();

-- 10. NOTIFICATIONS (5 total: 2 for students, 3 for instructors)
INSERT INTO [Notifications] ([Title], [Message], [IsRead], [CreatedAt], [UserId])
SELECT TOP 2 N'New Assignment', N'You have been assigned to a new module.', 0, GETDATE(), Id
FROM Users WHERE RoleId = '3' ORDER BY NEWID();

INSERT INTO [Notifications] ([Title], [Message], [IsRead], [CreatedAt], [UserId])
SELECT TOP 3 N'New Course Assigned', N'Admin has assigned you to a new course.', 0, GETDATE(), Id
FROM Users WHERE RoleId = '2' ORDER BY NEWID();

PRINT 'Sample data insertion completed successfully.';
GO
