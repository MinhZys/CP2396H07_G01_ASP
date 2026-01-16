using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class ClassCategory
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty; // e.g., Theory, Lab

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
