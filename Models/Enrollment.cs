using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class Enrollment
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;
        
        public string ClassId { get; set; } = string.Empty;
        public Class? Class { get; set; }

        public string StudentId { get; set; } = string.Empty;
        [ForeignKey("StudentId")]
        public User? Student { get; set; }

        public DateTime EnrolledDate { get; set; }
        
        public bool IsApproved { get; set; }
        public bool IsPaid { get; set; }
        public string PaymentReference { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;

        public string? CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
    }
}
