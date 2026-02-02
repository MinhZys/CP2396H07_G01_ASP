using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class PageImage
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(36)]
        public string PageContentId { get; set; } = string.Empty;

        [ForeignKey(nameof(PageContentId))]
        public PageContent? PageContent { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public int SortOrder { get; set; } = 0;

        public bool IsFeatured { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
