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
                new Role { Id = 1, Name = RoleNames.Admin, Description = "System Administrator" },
                new Role { Id = 2, Name = RoleNames.Instructor, Description = "Course Instructor" },
                new Role { Id = 3, Name = RoleNames.Student, Description = "Learner" }
            );

            // Seed Users
            builder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    FullName = "System Admin", 
                    Email = "admin@symphony.local", 
                    Password = "admin", 
                    RoleId = 1,
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    FullName = "Mr. Teacher",
                    Email = "teacher@symphony.local",
                    Password = "123",
                    RoleId = 2,
                    IsActive = true
                },
                new User
                {
                    Id = 3,
                    FullName = "Student One",
                    Email = "student@symphony.local",
                    Password = "123",
                    RoleId = 3,
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
