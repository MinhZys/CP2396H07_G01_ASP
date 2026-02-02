using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class PageContent
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime LastUpdated { get; set; }

        public string? SubjectId { get; set; }
        
        [ForeignKey("SubjectId")]
        public Subject? Subject { get; set; }

        public string? CenterId { get; set; }
        
        [ForeignKey("CenterId")]
        public Center? Center { get; set; }
    }
}
