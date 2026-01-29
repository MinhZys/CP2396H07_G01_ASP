using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class ClassLesson
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string ClassId { get; set; } = string.Empty;

        [ForeignKey(nameof(ClassId))]
        public Class? Class { get; set; }

        [Required]
        public string LessonId { get; set; } = string.Empty;

        [ForeignKey(nameof(LessonId))]
        public Lesson? Lesson { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.Now;
    }
}
