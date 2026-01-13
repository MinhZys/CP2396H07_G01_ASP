using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class QuizQuestion
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;

        [Required]
        public string CorrectOption { get; set; } = "A"; // A, B, C, D

        public int Points { get; set; } = 1;

        public string QuizId { get; set; } = string.Empty;
        
        [ForeignKey("QuizId")]
        public Quiz? Quiz { get; set; }
    }
}
