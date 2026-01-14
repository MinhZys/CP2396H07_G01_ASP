using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class Material
    {
        [Key]
        [StringLength(36)]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty; // e.g. PDF, MP4

        public DateTime UploadDate { get; set; } = DateTime.Now;

        [Required]
        public string ClassId { get; set; } = string.Empty;

        [ForeignKey("ClassId")]
        public Class? Class { get; set; }
    }
}
