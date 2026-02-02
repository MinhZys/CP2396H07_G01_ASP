using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class ContactMessage
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, StringLength(120)]
        public string Email { get; set; } = string.Empty;

        [StringLength(36)]
        public string? CenterId { get; set; }

        [ForeignKey("CenterId")]
        public Center? Center { get; set; }

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}
