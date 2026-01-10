using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class Subject
    {
        [Key]
        [StringLength(26)]
        [Display(Name = "Subject ID")]
        public string Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Subject Name")]
        public string Name { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Study Time (Hours)")]
        public int StudyTime { get; set; }

        public string Description { get; set; }
    }
}
