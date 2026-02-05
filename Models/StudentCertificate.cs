using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    /// <summary>
    /// Tracks certificates issued to students upon passing a course's final exam(s).
    /// </summary>
    public class StudentCertificate
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

        [Required]
        public string CertificateId { get; set; } = string.Empty;
        [ForeignKey("CertificateId")]
        public Certificate? Certificate { get; set; }

        [Display(Name = "Ngày cấp")]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Display(Name = "Ngày hết hạn")]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Mã chứng chỉ")]
        [StringLength(50)]
        public string CertificateCode { get; set; } = string.Empty;

        [Display(Name = "Điểm trung bình")]
        public double AverageScore { get; set; }

        public bool IsValid { get; set; } = true;
    }
}
