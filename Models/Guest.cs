using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Symphony.Portal.Web.Models.Enums;

namespace Symphony.Portal.Web.Models
{
    public class Guest
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime Dob { get; set; }

        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        // The Entrance Exam they want to register for
        public string? SelectedEntranceExamId { get; set; }

        [ForeignKey("SelectedEntranceExamId")]
        public EntranceExam? SelectedEntranceExam { get; set; }

        public GuestRegistrationStatus Status { get; set; } = GuestRegistrationStatus.PendingPayment;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // If approved, this links to the User account
        public string? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User? User { get; set; }

        // Enhancements for Approval
        public string? ExamRoom { get; set; } // Room to study/exam
        public string? Description { get; set; }

        public string? ClassId { get; set; }
        [ForeignKey("ClassId")]
        public Class? Class { get; set; } // Link to school class side
    }
}
