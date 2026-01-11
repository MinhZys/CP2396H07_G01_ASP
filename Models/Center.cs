using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class Center
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
    }
}
