using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    public class Course
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Học phí phải lớn hơn 0")]
        public decimal TuitionFee { get; set; }
        
        public int DurationMonths { get; set; }

        [Required]
        public string CertificateId { get; set; } = string.Empty;
        public Certificate? Certificate { get; set; }
        
        public bool IsActive { get; set; } = true;

        public string? Image { get; set; }

        public CourseLevel Level { get; set; } = CourseLevel.Beginner;

        [Required]
        public string CategoryId { get; set; } = string.Empty;
        public Category? Category { get; set; }

        

        public ICollection<CourseSubject> CourseSubjects { get; set; } = new List<CourseSubject>();
        public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
        public ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();
    }
}
