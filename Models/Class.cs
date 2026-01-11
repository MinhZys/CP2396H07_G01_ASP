using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class Class
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string CourseId { get; set; } = string.Empty;
        public Course? Course { get; set; }

        public string? InstructorId { get; set; }
        [ForeignKey("InstructorId")]
        public User? Instructor { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public bool IsOnline { get; set; }
        public string? Room { get; set; }
        public decimal OfflineFee { get; set; }
        
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
