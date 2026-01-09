using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class FAQ
    {
        public int Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        public int DisplayOrder { get; set; }
    }
}
