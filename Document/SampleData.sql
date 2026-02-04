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

/* =========================================================================================
   ADDITIONAL SAMPLE DATA (Centers, Locations, Content, Exams, Classes, etc.)
   ========================================================================================= */

-- 0. Ensure Roles Exist (if not already seeded)
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Admin')
    INSERT INTO Roles (Id, Name, Description) VALUES ('1', 'Admin', 'System Administrator');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Instructor')
    INSERT INTO Roles (Id, Name, Description) VALUES ('2', 'Instructor', 'Teacher/Instructor');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Student')
    INSERT INTO Roles (Id, Name, Description) VALUES ('3', 'Student', 'Student/Learner');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE Name = 'Guest')
    INSERT INTO Roles (Id, Name, Description) VALUES ('4', 'Guest', 'Guest User');

-- 11. CENTERS (5 Centers)
DECLARE @Center1 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Center2 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Center3 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Center4 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Center5 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));

INSERT INTO [Centers] ([Id], [Name], [Address], [Phone]) VALUES
(@Center1, N'Main Campus', N'123 Education Blvd, New York, NY', N'0212-333-4444'),
(@Center2, N'North Branch', N'456 North Ave, Chicago, IL', N'0312-555-6666'),
(@Center3, N'West Coast Hub', N'789 West St, San Francisco, CA', N'0415-777-8888'),
(@Center4, N'Online Support Center', N'Virtual Headquarters', N'1800-555-0000'),
(@Center5, N'South Campus', N'321 South Rd, Miami, FL', N'0305-999-0000');

-- 12. ROOMS (5 Rooms)
INSERT INTO [Rooms] ([Name], [Type], [Capacity], [IsActive], [LocationNote], [CreatedAt], [UpdatedAt]) VALUES
(N'Room 101', 1, 30, 1, N'Building A, Ground Floor', GETDATE(), GETDATE()),
(N'Room 102', 1, 30, 1, N'Building A, Ground Floor', GETDATE(), GETDATE()),
(N'Lab 201', 2, 25, 1, N'Building B, 2nd Floor (Computer Lab)', GETDATE(), GETDATE()),
(N'Hall A', 3, 100, 1, N'Main Hall', GETDATE(), GETDATE()),
(N'Room 303', 1, 40, 1, N'Building C, 3rd Floor', GETDATE(), GETDATE());

-- 13. HOLIDAYS (5 Holidays)
INSERT INTO [Holidays] ([Date], [Name], [IsRecurringAnnual], [Note], [IsActive], [CreatedAt], [UpdatedAt]) VALUES
('2026-01-01', N'New Year''s Day', 1, N'Happy New Year', 1, GETDATE(), GETDATE()),
('2026-04-30', N'Liberty Day', 1, N'National Holiday', 1, GETDATE(), GETDATE()),
('2026-05-01', N'Labor Day', 1, N'International Workers Day', 1, GETDATE(), GETDATE()),
('2026-09-02', N'Independence Day', 1, N'National Day', 1, GETDATE(), GETDATE()),
('2026-12-25', N'Christmas Day', 1, N'Merry Christmas', 1, GETDATE(), GETDATE());

-- 14. FAQS (5 FAQs)
INSERT INTO [FAQs] ([Id], [Question], [Answer], [DisplayOrder], [IsActive]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), N'How do I register for a course?', N'You can register online via the Courses page.', 1, 1),
(CAST(NEWID() AS NVARCHAR(36)), N'What are the payment methods?', N'We accept Credit Cards and VNPay.', 2, 1),
(CAST(NEWID() AS NVARCHAR(36)), N'Can I get a refund?', N'Yes, refunds are available within the first week of class.', 3, 1),
(CAST(NEWID() AS NVARCHAR(36)), N'Do you offer online classes?', N'Yes, many of our courses are available online.', 4, 1),
(CAST(NEWID() AS NVARCHAR(36)), N'How do I contact support?', N'Use the Contact Us form or call our hotline.', 5, 1);

-- 15. PAGE CONTENTS (5 Pages)
DECLARE @Page1 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Page2 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Page3 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Page4 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Page5 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));

INSERT INTO [PageContents] ([Id], [Slug], [Title], [Content], [LastUpdated], [CenterId]) VALUES
(@Page1, N'about-us', N'About Us', N'<p>Welcome to Symphony Portal...</p>', GETDATE(), @Center1),
(@Page2, N'terms', N'Terms of Service', N'<p>These are our terms...</p>', GETDATE(), @Center1),
(@Page3, N'privacy', N'Privacy Policy', N'<p>We respect your privacy...</p>', GETDATE(), @Center1),
(@Page4, N'careers', N'Careers', N'<p>Join our team!</p>', GETDATE(), @Center2),
(@Page5, N'events', N'Upcoming Events', N'<p>Check out our events...</p>', GETDATE(), @Center3);

-- 16. PAGE IMAGES (5 Images - linked to pages)
INSERT INTO [PageImages] ([Id], [PageContentId], [ImageUrl], [SortOrder], [IsFeatured], [CreatedAt]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), @Page1, N'about-banner.jpg', 1, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @Page1, N'team-photo.jpg', 2, 0, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @Page4, N'office-life.jpg', 1, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @Page5, N'event-2026.jpg', 1, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @Page5, N'workshop.jpg', 2, 0, GETDATE());

-- 17. LESSONS (5 Lessons for a Course)
-- Re-fetch a course and subject to link
DECLARE @RefCourseId NVARCHAR(36); SELECT TOP 1 @RefCourseId = Id FROM Courses;
DECLARE @RefSubjectId NVARCHAR(450); SELECT TOP 1 @RefSubjectId = Id FROM Subjects;

DECLARE @L1 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @L2 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @L3 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @L4 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @L5 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));

INSERT INTO [Lessons] ([Id], [Title], [Description], [ContentLink], [Image], [DurationMinutes], [CourseId], [SubjectId]) VALUES
(@L1, N'Intro to C#', N'Variables and DataTypes', N'https://video.com/csharp1', N'csharp1.jpg', 45, @RefCourseId, @RefSubjectId),
(@L2, N'Control Flow', N'If/Else and Loops', N'https://video.com/csharp2', N'csharp2.jpg', 50, @RefCourseId, @RefSubjectId),
(@L3, N'Methods', N'Functions and Parameters', N'https://video.com/csharp3', N'csharp3.jpg', 55, @RefCourseId, @RefSubjectId),
(@L4, N'OOP Basics', N'Classes and Objects', N'https://video.com/csharp4', N'csharp4.jpg', 60, @RefCourseId, @RefSubjectId),
(@L5, N'Inheritance', N'Base and Derived Classes', N'https://video.com/csharp5', N'csharp5.jpg', 60, @RefCourseId, @RefSubjectId);

-- 18. QUIZZES (5 Quizzes)
DECLARE @Qz1 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Qz2 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Qz3 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Qz4 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @Qz5 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));

INSERT INTO [Quizzes] ([Id], [Name], [Description], [PassScore], [DateCreated], [LessonId]) VALUES
(@Qz1, N'C# Basics Quiz', N'Test your knowledge on basics', 70, GETDATE(), @L1),
(@Qz2, N'Control Flow Quiz', N'Test loops and conditions', 70, GETDATE(), @L2),
(@Qz3, N'Methods Quiz', N'Test functions knowledge', 75, GETDATE(), @L3),
(@Qz4, N'OOP Quiz', N'Classes and Objects test', 80, GETDATE(), @L4),
(@Qz5, N'Inheritance Quiz', N'Polymorphism and Inheritance', 80, GETDATE(), @L5);

-- 19. QUIZ QUESTIONS (5 Questions for Quiz 1)
INSERT INTO [QuizQuestions] ([Id], [QuestionText], [OptionA], [OptionB], [OptionC], [OptionD], [CorrectOption], [Points], [QuizId]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), N'What is int?', N'Integer', N'String', N'Double', N'Boolean', N'A', 10, @Qz1),
(CAST(NEWID() AS NVARCHAR(36)), N'What is var?', N'Explicit type', N'Implicit type', N'Loop', N'Class', N'B', 10, @Qz1),
(CAST(NEWID() AS NVARCHAR(36)), N'C# is developed by?', N'Oracle', N'Microsoft', N'Google', N'Apple', N'B', 10, @Qz1),
(CAST(NEWID() AS NVARCHAR(36)), N'Ends a statement?', N'Dot', N'Comma', N'Semicolon', N'Colon', N'C', 10, @Qz1),
(CAST(NEWID() AS NVARCHAR(36)), N'Main method is?', N'Private', N'Static', N'Virtual', N'Abstract', N'B', 10, @Qz1);

-- 20. QUESTIONS, OPTIONS & EXAM PAPERS (Bank)
DECLARE @QP1 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @QP2 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @QP3 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @QP4 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @QP5 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));

INSERT INTO [Questions] ([Id], [Content], [Type], [SubjectId], [Difficulty], [Score], [CreatedAt]) VALUES
(@QP1, N'Explain DI', 1, @RefSubjectId, N'Hard', 5.0, GETDATE()),
(@QP2, N'Explain MVC', 1, @RefSubjectId, N'Medium', 5.0, GETDATE()),
(@QP3, N'What is Middleware?', 1, @RefSubjectId, N'Medium', 5.0, GETDATE()),
(@QP4, N'What is EF Core?', 1, @RefSubjectId, N'Easy', 5.0, GETDATE()),
(@QP5, N'What is Razor?', 1, @RefSubjectId, N'Easy', 5.0, GETDATE());

-- Options for QP1
INSERT INTO [QuestionOptions] ([Id], [QuestionId], [Content], [IsCorrect]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), @QP1, N'Dependency Injection', 1),
(CAST(NEWID() AS NVARCHAR(36)), @QP1, N'Direct Injection', 0),
(CAST(NEWID() AS NVARCHAR(36)), @QP2, N'Model View Controller', 1),
(CAST(NEWID() AS NVARCHAR(36)), @QP2, N'Make View Code', 0),
(CAST(NEWID() AS NVARCHAR(36)), @QP3, N'Pipeline component', 1);

-- 21. EXAM PAPERS
DECLARE @EP1 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @EP2 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @EP3 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @EP4 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
DECLARE @EP5 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));

INSERT INTO [ExamPapers] ([Id], [Title], [Duration], [SubjectId], [CreatedAt]) VALUES
(@EP1, N'Midterm Exam', 60, @RefSubjectId, GETDATE()),
(@EP2, N'Final Exam', 90, @RefSubjectId, GETDATE()),
(@EP3, N'Quiz 1', 15, @RefSubjectId, GETDATE()),
(@EP4, N'Quiz 2', 15, @RefSubjectId, GETDATE()),
(@EP5, N'Practice Test', 45, @RefSubjectId, GETDATE());

INSERT INTO [ExamPaperQuestions] ([ExamPaperId], [QuestionId], [Order]) VALUES
(@EP1, @QP1, 1), (@EP1, @QP2, 2),
(@EP2, @QP3, 1), (@EP2, @QP4, 2), (@EP2, @QP5, 3);

-- 22. ENROLLMENTS & REGISTRATIONS
-- Get Students and Classes
DECLARE @RefStudent1 NVARCHAR(36); SELECT TOP 1 @RefStudent1 = Id FROM Users WHERE RoleId = '3';
DECLARE @RefClassId NVARCHAR(36); SELECT TOP 1 @RefClassId = Id FROM Classes;

-- Enrollments
INSERT INTO [Enrollments] ([Id], [ClassId], [StudentId], [CourseId], [EnrolledDate], [IsApproved], [IsPaid], [IsCompleted], [PaymentReference]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), @RefClassId, @RefStudent1, @RefCourseId, GETDATE(), 1, 1, 0, N'PAY-001'),
(CAST(NEWID() AS NVARCHAR(36)), @RefClassId, @RefStudent1, @RefCourseId, GETDATE(), 1, 1, 0, N'PAY-002'),
(CAST(NEWID() AS NVARCHAR(36)), @RefClassId, @RefStudent1, @RefCourseId, GETDATE(), 1, 0, 0, NULL),
(CAST(NEWID() AS NVARCHAR(36)), @RefClassId, @RefStudent1, @RefCourseId, GETDATE(), 0, 0, 0, NULL),
(CAST(NEWID() AS NVARCHAR(36)), @RefClassId, @RefStudent1, @RefCourseId, GETDATE(), 1, 1, 1, N'PAY-005');

-- Student Registrations
INSERT INTO [StudentRegistrations] ([Id], [FullName], [Email], [Gender], [DateOfBirth], [Phone], [CourseId], [CenterId], [HasExtraPractice], [RegisteredAt], [Status]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), N'New Student 1', N'ns1@test.com', 1, GETDATE(), N'0999888771', @RefCourseId, @Center1, 1, GETDATE(), 1),
(CAST(NEWID() AS NVARCHAR(36)), N'New Student 2', N'ns2@test.com', 0, GETDATE(), N'0999888772', @RefCourseId, @Center1, 0, GETDATE(), 1),
(CAST(NEWID() AS NVARCHAR(36)), N'New Student 3', N'ns3@test.com', 1, GETDATE(), N'0999888773', @RefCourseId, @Center1, 1, GETDATE(), 1),
(CAST(NEWID() AS NVARCHAR(36)), N'New Student 4', N'ns4@test.com', 0, GETDATE(), N'0999888774', @RefCourseId, @Center1, 0, GETDATE(), 1),
(CAST(NEWID() AS NVARCHAR(36)), N'New Student 5', N'ns5@test.com', 1, GETDATE(), N'0999888775', @RefCourseId, @Center1, 1, GETDATE(), 1);

-- 23. CLASS SCHEDULES & SESSIONS
-- Schedule
DECLARE @SchedId INT;
INSERT INTO [ClassSchedules] ([ClassId], [StartDate], [EndDate], [Status], [IsPublished], [IsLocked], [PublishedAt], [CreatedAt], [UpdatedAt]) VALUES
(@RefClassId, GETDATE(), DATEADD(MONTH, 3, GETDATE()), 1, 1, 0, GETDATE(), GETDATE(), GETDATE());
-- Cannot easily get Identity Key without scope identity immediately.
-- Assuming we just insert generic data or use logic. 
-- For SQL script to be robust usually need 'SCOPE_IDENTITY()' captured in variable.
SET @SchedId = SCOPE_IDENTITY(); 

-- Sessions (5)
DECLARE @RoomRef INT; SELECT TOP 1 @RoomRef = Id FROM Rooms;
DECLARE @InstrRef NVARCHAR(36); SELECT TOP 1 @InstrRef = Id FROM Users WHERE RoleId = '2';

INSERT INTO [ClassSessions] ([ClassScheduleId], [SessionDate], [StartTime], [EndTime], [RoomId], [InstructorId], [SessionType], [Note], [IsCancelled], [CreatedAt]) VALUES
(@SchedId, GETDATE(), '08:00', '10:00', @RoomRef, @InstrRef, 1, N'Lecture 1', 0, GETDATE()),
(@SchedId, DATEADD(DAY, 2, GETDATE()), '08:00', '10:00', @RoomRef, @InstrRef, 1, N'Lecture 2', 0, GETDATE()),
(@SchedId, DATEADD(DAY, 4, GETDATE()), '08:00', '10:00', @RoomRef, @InstrRef, 2, N'Lab 1', 0, GETDATE()),
(@SchedId, DATEADD(DAY, 6, GETDATE()), '08:00', '10:00', @RoomRef, @InstrRef, 1, N'Lecture 3', 0, GETDATE()),
(@SchedId, DATEADD(DAY, 8, GETDATE()), '08:00', '10:00', @RoomRef, @InstrRef, 2, N'Lab 2', 0, GETDATE());

-- 24. MATERIALS (5 Materials)
INSERT INTO [Materials] ([Id], [Title], [Description], [FilePath], [FileType], [UploadDate], [ClassId]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), N'Syllabus', N'Course Syllabus', N'/files/syllabus.pdf', N'.pdf', GETDATE(), @RefClassId),
(CAST(NEWID() AS NVARCHAR(36)), N'Slide Deck 1', N'Intro Slides', N'/files/deck1.pptx', N'.pptx', GETDATE(), @RefClassId),
(CAST(NEWID() AS NVARCHAR(36)), N'Code Sample', N'Week 1 Code', N'/files/code.zip', N'.zip', GETDATE(), @RefClassId),
(CAST(NEWID() AS NVARCHAR(36)), N'Assignment Guide', N'How to submit', N'/files/guide.pdf', N'.pdf', GETDATE(), @RefClassId),
(CAST(NEWID() AS NVARCHAR(36)), N'Reading List', N'Books to read', N'/files/books.pdf', N'.pdf', GETDATE(), @RefClassId);

-- 25. ASSIGNMENTS & CLASS ASSIGNMENTS
INSERT INTO [Assignments] ([Id], [Title], [ClassId], [InstructorId], [AssignmentType], [Status], [Note], [CreatedAt]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), N'Homework 1', @RefClassId, @InstrRef, 1, 1, N'Due next week', GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'Homework 2', @RefClassId, @InstrRef, 1, 1, N'Chapter 2', GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'Midterm Project', @RefClassId, @InstrRef, 2, 1, N'Group work', GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'Essay', @RefClassId, @InstrRef, 1, 1, N'1000 words', GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'Final Presentation', @RefClassId, @InstrRef, 3, 1, N'Slides required', GETDATE());

-- Class Assignments (Distribution)
INSERT INTO [ClassAssignments] ([Id], [StudentId], [ClassId], [AssignedAt]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, @RefClassId, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, @RefClassId, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, @RefClassId, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, @RefClassId, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, @RefClassId, GETDATE());

-- 26. REVISION PACKAGES (5 Packages)
DECLARE @RP1 NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
INSERT INTO [RevisionPackages] ([Id], [Title], [Description], [Fee], [MaxStudents], [CurrentStudents], [Status], [CreatedAt]) VALUES
(@RP1, N'IELTS Prep', N'Intensive English', 500, 20, 5, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'TOEIC Prep', N'Business English', 400, 25, 10, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'N3 Japanese', N'JLPT N3', 600, 15, 2, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'Basic Math', N'Algebra Review', 200, 30, 0, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'Coding Interview', N'DSA Prep', 800, 10, 8, 1, GETDATE());

-- 27. REVISION REGISTRATIONS (5 Registrations)
INSERT INTO [RevisionRegistrations] ([Id], [FullName], [Email], [PhoneNumber], [RevisionPackageId], [ClassId], [Status], [CreatedAt]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), N'John Doe', N'john@example.com', N'0123456789', @RP1, @RefClassId, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'Jane Smith', N'jane@example.com', N'0987654321', @RP1, @RefClassId, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'Bob Wilson', N'bob@example.com', N'0111222333', @RP1, @RefClassId, 0, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'Alice Brown', N'alice@example.com', N'0444555666', @RP1, @RefClassId, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), N'Charlie Day', N'charlie@example.com', N'0777888999', @RP1, @RefClassId, 2, GETDATE());

-- 28. MESSAGING
-- Contact Messages
INSERT INTO [ContactMessages] ([Id], [FullName], [Email], [CenterId], [Message], [CreatedAt], [IsRead]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), N'Visitor 1', N'vis1@email.com', @Center1, N'Open hours?', GETDATE(), 1),
(CAST(NEWID() AS NVARCHAR(36)), N'Visitor 2', N'vis2@email.com', @Center1, N'Location?', GETDATE(), 0),
(CAST(NEWID() AS NVARCHAR(36)), N'Visitor 3', N'vis3@email.com', @Center1, N'Jobs?', GETDATE(), 0),
(CAST(NEWID() AS NVARCHAR(36)), N'Visitor 4', N'vis4@email.com', @Center1, N'Courses?', GETDATE(), 1),
(CAST(NEWID() AS NVARCHAR(36)), N'Visitor 5', N'vis5@email.com', @Center1, N'Price?', GETDATE(), 0);

-- Chat Messages (Mock)
INSERT INTO [ChatMessages] ([SenderId], [ReceiverId], [Content], [Timestamp], [IsRead], [SenderValidName]) VALUES
(@RefStudent1, @InstrRef, N'Hello Professor', GETDATE(), 0, N'Student User'),
(@InstrRef, @RefStudent1, N'Hi there', GETDATE(), 1, N'Instructor'),
(@RefStudent1, @InstrRef, N'Question about HW', GETDATE(), 0, N'Student User'),
(@InstrRef, @RefStudent1, N'Sure go ahead', GETDATE(), 1, N'Instructor'),
(@RefStudent1, @InstrRef, N'When is the deadline?', GETDATE(), 0, N'Student User');

-- 29. PAYMENTS (5 Payments)
INSERT INTO [Payments] ([Id], [StudentId], [GuestId], [Amount], [PaymentMethod], [PaymentDate], [ReceiptNumber], [Status], [Purpose]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, NULL, 500.00, 1, GETDATE(), N'REC-001', 1, 1),
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, NULL, 200.00, 2, GETDATE(), N'REC-002', 1, 2),
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, NULL, 1500.00, 1, GETDATE(), N'REC-003', 1, 1),
(CAST(NEWID() AS NVARCHAR(36)), NULL, NULL, 50.00, 1, GETDATE(), N'REC-004', 1, 3), 
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, NULL, 100.00, 1, GETDATE(), N'REC-005', 0, 1);

-- 30. EXAM RESULTS (5 Results)
DECLARE @EntExamRef NVARCHAR(36); SELECT TOP 1 @EntExamRef = Id FROM EntranceExams;
INSERT INTO [ExamResults] ([Id], [StudentId], [EntranceExamId], [Score], [IsPassed], [ExamDate]) VALUES
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, @EntExamRef, 85.5, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, @EntExamRef, 45.0, 0, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, @EntExamRef, 90.0, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, @EntExamRef, 60.0, 1, GETDATE()),
(CAST(NEWID() AS NVARCHAR(36)), @RefStudent1, @EntExamRef, 75.0, 1, GETDATE());

-- 31. STUDENT EXAM SESSIONS & ANSWERS
DECLARE @SessId NVARCHAR(36) = CAST(NEWID() AS NVARCHAR(36));
INSERT INTO [StudentExamSessions] ([Id], [EntranceExamId], [StudentId], [ExamPaperId], [StartTime], [EndTime], [TotalScore], [GradeLevel], [Status]) VALUES
(@SessId, @EntExamRef, @RefStudent1, @EP1, GETDATE(), DATEADD(HOUR, 1, GETDATE()), 80, N'A', 2);

INSERT INTO [StudentAnswers] ([SessionId], [QuestionId], [SelectedOptionId], [EarnedScore], [IsGraded]) VALUES
(@SessId, @QP1, NULL, 5.0, 1),
(@SessId, @QP2, NULL, 5.0, 1),
(@SessId, @QP3, NULL, 5.0, 1),
(@SessId, @QP4, NULL, 5.0, 1),
(@SessId, @QP5, NULL, 5.0, 1);

PRINT 'Full sample data generated.';
GO
