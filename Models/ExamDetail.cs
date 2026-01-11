using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class ExamDetail
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string RegistrationId { get; set; } = string.Empty;

        [ForeignKey("RegistrationId")]
        public StudentRegistration? StudentRegistration { get; set; }

        public DateTime ExamTime { get; set; }

        public string ExamRoom { get; set; } = string.Empty;

        public string ExamDescription { get; set; } = string.Empty;
    }
}
