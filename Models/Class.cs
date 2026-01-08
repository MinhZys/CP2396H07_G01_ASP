using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Identity;

namespace Symphony.Portal.Web.Models
{
    public class Class
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public string? InstructorId { get; set; }
        [ForeignKey("InstructorId")]
        public ApplicationUser? Instructor { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
