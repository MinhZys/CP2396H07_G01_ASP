using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class Subject
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int StudyTime { get; set; }

        public string Description { get; set; } = string.Empty;

        public string? Image { get; set; }
        
        public ICollection<CourseSubject> CourseSubjects { get; set; } = new List<CourseSubject>();
    }
}
