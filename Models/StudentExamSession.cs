using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    public class StudentExamSession
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? EntranceExamId { get; set; }
        
        [ForeignKey("EntranceExamId")]
        public EntranceExam? EntranceExam { get; set; }

        public string? ClassExamId { get; set; }
        
        [ForeignKey("ClassExamId")]
        public ClassExam? ClassExam { get; set; }

        [Required]
        public string StudentId { get; set; } = string.Empty;
        
        [ForeignKey("StudentId")]
        public User? Student { get; set; }

        [Required]
        public string ExamPaperId { get; set; } = string.Empty;
        
        [ForeignKey("ExamPaperId")]
        public ExamPaper? ExamPaper { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public double TotalScore { get; set; }
        
        [Display(Name = "Xếp loại")]
        public string? GradeLevel { get; set; } // Level A, B, C

        public ExamSessionStatus Status { get; set; } = ExamSessionStatus.Taking;

        public virtual ICollection<StudentAnswer> Answers { get; set; } = new List<StudentAnswer>();
    }

    public class StudentAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SessionId { get; set; } = string.Empty;
        
        [ForeignKey("SessionId")]
        public StudentExamSession? Session { get; set; }

        [Required]
        public string QuestionId { get; set; } = string.Empty;
        
        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }

        // For Multiple Choice
        public string? SelectedOptionId { get; set; }
        
        // For Essay
        public string? EssayContent { get; set; }

        public double EarnedScore { get; set; }

        public bool IsGraded { get; set; } = false;

        [Display(Name = "Ghi chú của giám khảo")]
        public string? ExaminerNote { get; set; }
    }
}
