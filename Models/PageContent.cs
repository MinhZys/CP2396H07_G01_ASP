using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class PageContent
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Slug { get; set; } // e.g. "about-us", "contact"

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; } // HTML Content

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
