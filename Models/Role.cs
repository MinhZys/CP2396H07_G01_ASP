using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class Role
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty; // e.g., "Admin", "Instructor", "Student"
        
        public string Description { get; set; } = string.Empty;

        // Navigation property
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
