using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class Lesson
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ContentLink { get; set; } = string.Empty;

        public string? Image { get; set; }

        public int DurationMinutes { get; set; }

        public string CourseId { get; set; } = string.Empty;
        
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public string SubjectId { get; set; } = string.Empty;
        
        [ForeignKey("SubjectId")]
        public Subject? Subject { get; set; }
        public string ClassId { get; set; } = string.Empty;

        [ForeignKey("ClassId")]
        public Class? Class { get; set; }
    }
}
