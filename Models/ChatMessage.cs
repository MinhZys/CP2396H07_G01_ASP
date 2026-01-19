using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Symphony.Portal.Web.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        public string? SenderId { get; set; } // Null if Guest
        public string? ReceiverId { get; set; } // Null if broadcasting to Admins

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;

        // For Guests
        public string? SessionId { get; set; } // Guid stored in cookie for guests
        public string? SenderValidName { get; set; } // Display name (e.g., "Guest 123" or User.FullName)

        // Navigation properties
        [ForeignKey("SenderId")]
        public virtual User? Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual User? Receiver { get; set; }
    }
}
