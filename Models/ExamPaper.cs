using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class ExamPaper
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Display(Name = "Tiêu đề đề thi")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Thời gian làm bài (phút)")]
        public int Duration { get; set; }

        [Display(Name = "Môn học")]
        public string? SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public Subject? Subject { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<ExamPaperQuestion> ExamPaperQuestions { get; set; } = new List<ExamPaperQuestion>();

        // ✅ NEW: sessions làm bài của học viên theo đề nà
        public virtual ICollection<StudentExamSession> StudentExamSessions { get; set; }
            = new List<StudentExamSession>();

        // ✅ NEW: các kỳ thi đang chọn đề này qua EntranceExam.ExamPaperId
        public virtual ICollection<EntranceExam> EntranceExams { get; set; }
            = new List<EntranceExam>();
    }

    public class ExamPaperQuestion
    {
        [Key]
        public int Id { get; set; }

        public string ExamPaperId { get; set; } = string.Empty;

        [ForeignKey("ExamPaperId")]
        public ExamPaper? ExamPaper { get; set; }

        public string QuestionId { get; set; } = string.Empty;

        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }

        public int Order { get; set; }
    }
}
