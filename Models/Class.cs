using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class Class
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public int? InstructorId { get; set; }
        [ForeignKey("InstructorId")]
        public User? Instructor { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public bool IsOnline { get; set; } = false;
        public string? Room { get; set; }
        public decimal OfflineFee { get; set; } = 0; // Extra fee for offline
        
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
