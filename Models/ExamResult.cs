using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class ExamResult
    {
        public int Id { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public User? Student { get; set; }

        public int AdmissionExamId { get; set; }
        public AdmissionExam? AdmissionExam { get; set; }

        public double Score { get; set; }
        public bool IsPassed { get; set; }
        public DateTime ExamDate { get; set; }
    }
}
