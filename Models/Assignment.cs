using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    public class Assignment
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty; // e.g., "Giảng dạy", "Coi thi"

        [Display(Name = "Term / Exam Name")]
        public string? TermOrExamName { get; set; }

        [Required]
        public string ClassId { get; set; }
        [ForeignKey("ClassId")]
        public Class? Class { get; set; }

        [Required]
        public string InstructorId { get; set; }
        [ForeignKey("InstructorId")]
        public User? Instructor { get; set; }

        public AssignmentType AssignmentType { get; set; }

        public string? Note { get; set; }

        public AssignmentStatus Status { get; set; } = AssignmentStatus.Assigned;

        public string? CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
