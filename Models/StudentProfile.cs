using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class StudentProfile
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(36)]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public string AddressLine { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = string.Empty;
    }
}   
