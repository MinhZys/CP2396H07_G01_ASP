using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class ExamResult
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;
        
        public string StudentId { get; set; } = string.Empty;
        [ForeignKey("StudentId")]
        public User? Student { get; set; }

        public string EntranceExamId { get; set; } = string.Empty;
        [ForeignKey("EntranceExamId")]
        public EntranceExam? EntranceExam { get; set; }

        public double Score { get; set; }
        public bool IsPassed { get; set; }
        public DateTime ExamDate { get; set; }
    }
}
