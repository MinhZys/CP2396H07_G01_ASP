using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Models.Identity;

namespace Symphony.Portal.Web.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<AdmissionExam> AdmissionExams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
    }
}
