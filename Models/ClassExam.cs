using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    /// <summary>
    /// Links an ExamPaper to a specific Class and Subject for final exams.
    /// This enables multi-subject final exams for a course/class.
    /// </summary>
    public class ClassExam
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Display(Name = "Lớp học")]
        public string ClassId { get; set; } = string.Empty;
        [ForeignKey("ClassId")]
        public Class? Class { get; set; }

        [Required]
        [Display(Name = "Khóa học")]
        public string CourseId { get; set; } = string.Empty;
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [Required]
        [Display(Name = "Đề thi")]
        public string ExamPaperId { get; set; } = string.Empty;
        [ForeignKey("ExamPaperId")]
        public ExamPaper? ExamPaper { get; set; }

        [Display(Name = "Ngày thi")]
        public DateTime? ExamDate { get; set; }

        [Display(Name = "Thời gian làm bài (phút)")]
        public int? DurationOverride { get; set; } // Override the ExamPaper duration if needed

        public ClassExamStatus Status { get; set; } = ClassExamStatus.Scheduled;

        [Display(Name = "Công bố điểm")]
        public bool IsScorePublished { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
