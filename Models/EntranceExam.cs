using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class EntranceExam
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public DateTime ExamDate { get; set; }

        public decimal Fee { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
