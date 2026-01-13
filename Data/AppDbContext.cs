using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Center> Centers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSubject> CourseSubjects { get; set; }
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
        
        // New Models
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Enums as Strings
            modelBuilder.Entity<StudentRegistration>()
                .Property(e => e.Status)
                .HasConversion<string>();

            modelBuilder.Entity<StudentRegistration>()
                .Property(e => e.Gender)
                .HasConversion<string>();

            modelBuilder.Entity<Course>()
                .Property(e => e.Level)
                .HasConversion<string>();

            modelBuilder.Entity<Payment>()
                .Property(e => e.PaymentMethod)
                .HasConversion<string>();

            // Composite Key for CourseSubject
            modelBuilder.Entity<CourseSubject>()
                .HasKey(cs => new { cs.CourseId, cs.SubjectId });

            // Seeding Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = "1", Name = "Admin", Description = "System Administrator" },
                new Role { Id = "2", Name = "Instructor", Description = "Course Instructor" },
                new Role { Id = "3", Name = "Student", Description = "Learner" }
            );

            // Seeding Users
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
        }
    }
}
