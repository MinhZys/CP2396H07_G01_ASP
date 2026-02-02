using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    public class EntranceExam
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Display(Name = "Tên kỳ thi")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Ngày giờ tổ chức")]
        public DateTime ExamDate { get; set; }

        [Display(Name = "Lệ phí")]
        public decimal Fee { get; set; }

        [Display(Name = "Số lượng thí sinh tối đa")]
        public int MaxCandidates { get; set; }

        [Display(Name = "Trạng thái")]
        public ExamStatus Status { get; set; } = ExamStatus.NotStarted;

        [Display(Name = "Mở đăng ký")]
        public bool IsRegistrationOpen { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [Display(Name = "Môn thi")]
        public string Subjects { get; set; } = string.Empty;

        [Display(Name = "Đề thi áp dụng")]
        public string? ExamPaperId { get; set; }

        [ForeignKey("ExamPaperId")]
        public ExamPaper? ExamPaper { get; set; }

        public ICollection<ExamPaper> ExamPapers { get; set; } = new List<ExamPaper>();

        public virtual ICollection<StudentExamSession> StudentExamSessions { get; set; }
            = new List<StudentExamSession>();

    }
}
