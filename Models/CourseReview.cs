using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class CourseReview
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        public string CourseId { get; set; } = string.Empty;
        
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public string StudentId { get; set; } = string.Empty;
        
        [ForeignKey("StudentId")]
        public User? Student { get; set; }

        public int Rating { get; set; }

        public string ReviewText { get; set; } = string.Empty;

        public DateTime ReviewDate { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; }
    }
}
