using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class Quiz
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int PassScore { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public string? LessonId { get; set; }
        
        [ForeignKey("LessonId")]
        public Lesson? Lesson { get; set; }

        public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    }
}
