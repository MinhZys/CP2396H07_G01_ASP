using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Identity;

namespace Symphony.Portal.Web.Models
{
    public class ExamResult
    {
        public int Id { get; set; }
        
        [Required]
        public string StudentId { get; set; } = string.Empty;
        [ForeignKey("StudentId")]
        public ApplicationUser? Student { get; set; }

        public int AdmissionExamId { get; set; }
        public AdmissionExam? AdmissionExam { get; set; }

        public double Score { get; set; }
        public bool IsPassed { get; set; }
        public DateTime ExamDate { get; set; }
    }
}
