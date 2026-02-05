using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ClassLesson> ClassLessons { get; set; }
        public DbSet<Center> Centers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassAssignment> ClassAssignments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSubject> CourseSubjects { get; set; }
        public DbSet<CourseInstructor> CourseInstructors { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<EntranceExam> EntranceExams { get; set; }
        public DbSet<ExamDetail> ExamDetails { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<PageContent> PageContents { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<StudentRegistration> StudentRegistrations { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Guest> Guests { get; set; }

        // New Models
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<ClassSession> ClassSessions { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<PageImage> PageImages { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<InstructorProfile> InstructorProfiles { get; set; }

        // Class Management
        public DbSet<ClassCategory> ClassCategories { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // Entrance Exam System
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<ExamPaper> ExamPapers { get; set; }
        public DbSet<ExamPaperQuestion> ExamPaperQuestions { get; set; }
        public DbSet<StudentExamSession> StudentExamSessions { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }

        // Payment & VNPay
        public DbSet<VNPayTransaction> VNPayTransactions { get; set; }

        // Revision System
        public DbSet<RevisionPackage> RevisionPackages { get; set; }
        public DbSet<RevisionRegistration> RevisionRegistrations { get; set; }

        // Course Exam & Certificate System
        public DbSet<StudentCertificate> StudentCertificates { get; set; }
        public DbSet<ClassExam> ClassExams { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // FIX: Decimal Precision
            // =========================
            modelBuilder.Entity<Course>().Property(x => x.TuitionFee).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<EntranceExam>().Property(x => x.Fee).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Payment>().Property(x => x.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<RevisionPackage>().Property(x => x.Fee).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Class>().Property(x => x.Fee).HasColumnType("decimal(18,2)");

            // =========================
            // FIX: ClassLesson Cascade Cycle
            // =========================
            // Sửa lỗi SQL Exception: Multiple cascade paths
            modelBuilder.Entity<ClassLesson>(entity =>
            {
                entity.HasIndex(x => new { x.ClassId, x.LessonId }).IsUnique();

                // Ngắt Cascade xóa từ Class -> ClassLesson
                entity.HasOne(cl => cl.Class)
                      .WithMany() // Hoặc .WithMany(c => c.ClassLessons) nếu có
                      .HasForeignKey(cl => cl.ClassId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Ngắt Cascade xóa từ Lesson -> ClassLesson
                entity.HasOne(cl => cl.Lesson)
                      .WithMany()
                      .HasForeignKey(cl => cl.LessonId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================
            // Composite Keys
            // =========================
            modelBuilder.Entity<CourseSubject>()
                .HasKey(x => new { x.CourseId, x.SubjectId });

            modelBuilder.Entity<CourseInstructor>()
                .HasKey(x => new { x.CourseId, x.InstructorId });

            // =========================
            // Enum as string
            // =========================
            modelBuilder.Entity<Guest>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<Class>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<Assignment>().Property(x => x.AssignmentType).HasConversion<string>();
            modelBuilder.Entity<Assignment>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<StudentRegistration>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<StudentRegistration>().Property(x => x.Gender).HasConversion<string>();
            modelBuilder.Entity<Course>().Property(x => x.Level).HasConversion<string>();
            modelBuilder.Entity<Payment>().Property(x => x.PaymentMethod).HasConversion<string>();
            modelBuilder.Entity<EntranceExam>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<Question>().Property(x => x.Type).HasConversion<string>();
            modelBuilder.Entity<ClassExam>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<ExamAttempt>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<Course>().Property(x => x.RetakeFee).HasColumnType("decimal(18,2)");

            // =========================
            // Course Exam & Certificate System
            // =========================
            modelBuilder.Entity<StudentCertificate>(entity =>
            {
                entity.HasOne(sc => sc.Student)
                      .WithMany()
                      .HasForeignKey(sc => sc.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sc => sc.Course)
                      .WithMany()
                      .HasForeignKey(sc => sc.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sc => sc.Certificate)
                      .WithMany()
                      .HasForeignKey(sc => sc.CertificateId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(sc => sc.CertificateCode).IsUnique();
            });

            modelBuilder.Entity<ClassExam>(entity =>
            {
                entity.HasOne(ce => ce.Class)
                      .WithMany()
                      .HasForeignKey(ce => ce.ClassId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ce => ce.Course)
                      .WithMany()
                      .HasForeignKey(ce => ce.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ce => ce.ExamPaper)
                      .WithMany()
                      .HasForeignKey(ce => ce.ExamPaperId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ExamPaper>(entity =>
            {
                entity.HasOne(ep => ep.Course)
                      .WithMany()
                      .HasForeignKey(ep => ep.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ExamAttempt>(entity =>
            {
                entity.HasOne(ea => ea.Student)
                      .WithMany()
                      .HasForeignKey(ea => ea.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ea => ea.Course)
                      .WithMany()
                      .HasForeignKey(ea => ea.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // =========================
            // Class Schedule
            // =========================
            modelBuilder.Entity<ClassSchedule>()
                .HasOne(x => x.Class)
                .WithMany(c => c.ClassSchedules)
                .HasForeignKey(x => x.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassSession>()
                .HasOne(x => x.ClassSchedule)
                .WithMany(cs => cs.Sessions)
                .HasForeignKey(x => x.ClassScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassSession>()
                .Property(x => x.StartTime)
                .HasColumnType("time");

            modelBuilder.Entity<ClassSession>()
                .Property(x => x.EndTime)
                .HasColumnType("time");

            // =====================================================
            // ===== EntranceExam -> ExamPaper (selected paper) =====
            // =====================================================
            modelBuilder.Entity<EntranceExam>()
                .HasOne(e => e.ExamPaper)
                .WithMany(p => p.EntranceExams)
                .HasForeignKey(e => e.ExamPaperId)
                .OnDelete(DeleteBehavior.SetNull);

            // =====================================================
            // ===== StudentExamSession (explicit mapping) =====
            // =====================================================
            modelBuilder.Entity<StudentExamSession>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Status).HasConversion<string>();

                b.HasOne(x => x.Student)
                    .WithMany(u => u.StudentExamSessions)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.ExamPaper)
                    .WithMany(p => p.StudentExamSessions)
                    .HasForeignKey(x => x.ExamPaperId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.EntranceExam)
                    .WithMany(e => e.StudentExamSessions)
                    .HasForeignKey(x => x.EntranceExamId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.ClassExam)
                    .WithMany()
                    .HasForeignKey(x => x.ClassExamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== StudentAnswer =====
            modelBuilder.Entity<StudentAnswer>(b =>
            {
                b.HasKey(x => x.Id);

                b.HasOne(x => x.Session)
                    .WithMany(s => s.Answers)
                    .HasForeignKey(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.Question)
                    .WithMany()
                    .HasForeignKey(x => x.QuestionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // =========================
            // One-to-one ExamDetail
            // =========================
            modelBuilder.Entity<StudentRegistration>()
                .HasOne(x => x.ExamDetail)
                .WithOne(e => e.StudentRegistration)
                .HasForeignKey<ExamDetail>(x => x.RegistrationId);

            // =========================
            // Seeding
            // =========================

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = "1", Name = "Admin", Description = "System Administrator" },
                new Role { Id = "2", Name = "Instructor", Description = "Course Instructor" },
                new Role { Id = "3", Name = "Student", Description = "Learner" },
                new Role { Id = "4", Name = "Guest", Description = "Prospective Student" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = "1",
                    FullName = "System Admin",
                    Email = "admin@symphony.local",
                    Password = "admin",
                    RoleId = "1",
                    IsActive = true
                },
                new User
                {
                    Id = "2",
                    FullName = "Mr. Teacher",
                    Email = "teacher@symphony.local",
                    Password = "123",
                    RoleId = "2",
                    IsActive = true
                },
                new User
                {
                    Id = "3",
                    FullName = "Student One",
                    Email = "student@symphony.local",
                    Password = "123",
                    RoleId = "3",
                    IsActive = true
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = "1", Name = "Programming", Description = "Software Development Courses" },
                new Category { Id = "2", Name = "Music", Description = "Music Theory and Instruments" },
                new Category { Id = "3", Name = "Art", Description = "Visual Arts and Design" }
            );

            modelBuilder.Entity<Certificate>().HasData(
                new Certificate { Id = "2", Name = "Professional Certification", Description = "Recognized industry standard certification.", IsActive = true }
            );

            modelBuilder.Entity<ClassCategory>().HasData(
                new ClassCategory { Id = "1", Name = "Theory", Description = "Standard classrooms", IsActive = true },
                new ClassCategory { Id = "2", Name = "Lab", Description = "Computer labs", IsActive = true },
                new ClassCategory { Id = "3", Name = "Online", Description = "Virtual classes", IsActive = true }
            );
        }
    }
}