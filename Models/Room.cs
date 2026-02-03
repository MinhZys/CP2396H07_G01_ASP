using Symphony.Portal.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        public RoomType Type { get; set; } = RoomType.Classroom;

        [Range(1, 500)]
        public int Capacity { get; set; } = 30;

        public bool IsActive { get; set; } = true;

        [MaxLength(200)]
        public string? LocationNote { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ClassSession> Sessions { get; set; } = new List<ClassSession>();
    }
}
