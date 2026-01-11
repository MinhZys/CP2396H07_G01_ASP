using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class StudentRegistration
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty; // Male/Female/Other

        public DateTime DateOfBirth { get; set; }

        [Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string CourseId { get; set; } = string.Empty;
        
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public string CenterId { get; set; } = string.Empty;
        
        [ForeignKey("CenterId")]
        public Center? Center { get; set; }

        public bool HasExtraPractice { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        // Navigation for Exam Details
        public ExamDetail? ExamDetail { get; set; }
    }
}
