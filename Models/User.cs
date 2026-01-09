using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; // Storing plain text or simple hash as requested

        public bool IsActive { get; set; } = true;

        // Foreign Key
        public int RoleId { get; set; }
        
        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
    }
}
