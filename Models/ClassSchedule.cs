using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class ClassSchedule
    {
        public int Id { get; set; }

        [Required]
        public string ClassId { get; set; } = default!;

        public Class? Class { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public ScheduleStatus Status { get; set; } = ScheduleStatus.Draft;

        public bool IsPublished { get; set; } = false;
        public bool IsLocked { get; set; } = false;

        public DateTime? PublishedAt { get; set; }
        public string? PublishedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ClassSession> Sessions { get; set; } = new List<ClassSession>();
    }
}
