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
        [StringLength(120)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [StringLength(50)]
        public string Phone { get; set; } = string.Empty;

        // NEW: giờ làm việc
        [StringLength(120)]
        public string OpenHours { get; set; } = string.Empty;

        // NEW: dùng cho tab / thứ tự hiển thị
        public int DisplayOrder { get; set; } = 0;

        // NEW: bật/tắt center
        public bool IsActive { get; set; } = true;

        // NEW: toạ độ map
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
