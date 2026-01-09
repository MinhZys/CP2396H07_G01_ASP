using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class Role
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty; // e.g., "Admin", "Instructor", "Student"
        
        public string Description { get; set; } = string.Empty;

        // Navigation property
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
