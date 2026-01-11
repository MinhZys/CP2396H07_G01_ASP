using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class FAQ
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Question { get; set; } = string.Empty;

        [Required]
        public string Answer { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
    }
}
