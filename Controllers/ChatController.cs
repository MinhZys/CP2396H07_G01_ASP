using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers
{
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) return BadRequest("Session ID required");

            // For guests, we rely on SessionId
            // If user is logged in, we might want to check SenderId, but for now Widget uses logic.
            // Let's allow fetching by SessionId for guests.
            
            var messages = await _context.ChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.Timestamp)
                .Select(m => new {
                    m.Content,
                    Time = m.Timestamp.ToString("HH:mm"),
                    // If SessionId matches, then "Self" is the Guest (SenderId is null or stored in cookie, but effectively checking if SenderId is NULL usually implies Guest, 
                    // BUT Guest messages have SenderId as NULL? Let's check ChatHub/Model.
                    // Actually ChatMessage has SenderId. For Guest it might be null? 
                    // Let's check ChatHub.
                    // If SenderId is null, it's a guest? No, ChatHub takes 'senderId'.
                    // Guest logic: SenderId = "Guest" or Name?
                    // Let's check how ChatMessage is saved in Hub.
                    
                    // Actually, simpler:
                    // If the message has the SessionID, it belongs to this conversation.
                    // If it was sent BY the guest -> Self.
                    // If it was sent TO the guest (from Admin) -> Other.
                    // How to distinguish?
                    // Admin reply: SenderId = "AdminID", ReceiverId = "Session_..." (Group) or invalid?
                    // Wait, Admin sends to Group "Session_...".
                    // The message saved in DB: SenderId = Admin, ReceiverId = "Session_..."?
                    // OR SenderId = Admin, ReceiverId = NULL?
                    
                    // I need to verify ChatHub save logic.
                    // Assuming I can figure out 'IsSelf' in Client or here.
                    // Let's return SenderId too.
                    m.SenderId,
                    m.SenderValidName
                })
                .ToListAsync();

            return Json(messages);
        }
    }
}
