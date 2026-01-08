using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class AdmissionExam
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        public DateTime ExamDate { get; set; }
        
        public decimal Fee { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
