using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class ClassAssignment
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [ForeignKey("StudentId")]
        public User? Student { get; set; }

        [Required]
        public string ClassId { get; set; } = string.Empty;

        [ForeignKey("ClassId")]
        public Class? Class { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.Now;
    }
}
