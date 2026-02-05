using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    /// <summary>
    /// Tracks each exam attempt by a student for a course.
    /// Supports retake tracking with payment status.
    /// </summary>
    public class ExamAttempt
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string StudentId { get; set; } = string.Empty;
        [ForeignKey("StudentId")]
        public User? Student { get; set; }

        [Required]
        public string CourseId { get; set; } = string.Empty;
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [Display(Name = "Lần thi thứ")]
        public int AttemptNumber { get; set; } = 1;

        [Display(Name = "Điểm trung bình")]
        public double? AverageScore { get; set; }

        public ExamAttemptStatus Status { get; set; } = ExamAttemptStatus.InProgress;

        [Display(Name = "Đã thanh toán phí thi lại")]
        public bool RetakeFeePaid { get; set; } = false;

        [Display(Name = "Mã thanh toán")]
        public string? PaymentReference { get; set; }

        public DateTime AttemptDate { get; set; } = DateTime.Now;
        public DateTime? CompletedDate { get; set; }
    }
}
