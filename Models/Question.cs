using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    public class Question
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Display(Name = "Nội dung câu hỏi")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Loại câu hỏi")]
        public QuestionType Type { get; set; } = QuestionType.MultipleChoice;

        [Display(Name = "Môn học")]
        public string? SubjectId { get; set; }
        
        [ForeignKey("SubjectId")]
        public Subject? Subject { get; set; }

        [Display(Name = "Độ khó")]
        public string Difficulty { get; set; } = "Medium"; // Easy, Medium, Hard

        [Display(Name = "Điểm số")]
        public double Score { get; set; } = 1.0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }

    public class QuestionOption
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string QuestionId { get; set; } = string.Empty;
        
        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }

        [Required]
        [Display(Name = "Nội dung đáp án")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Là đáp án đúng")]
        public bool IsCorrect { get; set; } = false;
    }
}
