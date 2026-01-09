using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Roles
            builder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "System Administrator" },
                new Role { Id = 2, Name = "Instructor", Description = "Course Instructor" },
                new Role { Id = 3, Name = "Student", Description = "Learner" }
            );

            // Seed Admin User
            builder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    FullName = "System Admin", 
                    Email = "admin@symphony.local", 
                    Password = "admin", // Simple plain text as requested for simplicity
                    RoleId = 1,
                    IsActive = true
                }
            );
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        
        public DbSet<Course> Courses { get; set; }
        public DbSet<AdmissionExam> AdmissionExams { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<PageContent> PageContents { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
    }
}
