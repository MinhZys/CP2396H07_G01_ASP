using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        
        public decimal TuitionFee { get; set; }
        
        public int DurationMonths { get; set; }

        public string Certification { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;

        public string? Image { get; set; }

        public string Level { get; set; } = "Beginner";

        
        public ICollection<Class> Classes { get; set; } = new List<Class>();
        public ICollection<CourseSubject> CourseSubjects { get; set; } = new List<CourseSubject>();
    }
}
