using System;
using System.ComponentModel.DataAnnotations;

namespace Symphony.Portal.Web.Models
{
    public class Holiday
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required, MaxLength(120)]
        public string Name { get; set; } = default!;

        public bool IsRecurringAnnual { get; set; } = false;

        [MaxLength(250)]
        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
