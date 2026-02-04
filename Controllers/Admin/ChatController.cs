using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = RoleNames.Admin)]
    [Route("Admin/[controller]/[action]")]
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var adminId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Group messages by SessionId (for guests) or SenderId (for users)
            // We only want to show conversations initiated by others (Guests or Students)
            // and filter out Admin's own replies from creating "self-conversations"
            var recentMessages = await _context.ChatMessages
                .Where(m => m.SenderId != adminId || m.SessionId != null) // Only show if not from admin OR part of a session
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();

            var chatSessions = recentMessages
                .GroupBy(m => m.SessionId ?? m.SenderId ?? "Unknown")
                .Select(g => 
                {
                    var latest = g.First();
                    // Attempt to find the actual user in the conversation (not AI, not Admin)
                    // AI has SenderId = null, Admin is filtered out of recentMessages ideally, but safe to check
                    var originalUser = g.FirstOrDefault(m => m.SenderId != null && m.SenderValidName != "AI Assistant");
                    
                    if (originalUser != null)
                    {
                        // Use the User's name for the conversation title in the sidebar
                        latest.SenderValidName = originalUser.SenderValidName;
                    }
                    return latest;
                })
                .Where(m => m.SenderId != adminId) // Double check to remove Admin's own thread
                .ToList();

            return View(chatSessions);
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory(string id)
        {
            // Id can be SessionId or UserId
            var messages = await _context.ChatMessages
                .Where(m => m.SessionId == id || m.SenderId == id || m.ReceiverId == id)
                .OrderBy(m => m.Timestamp)
                .Select(m => new {
                    m.SenderValidName,
                    m.Content,
                    Time = m.Timestamp.ToString("HH:mm"),
                    IsSelf = false // Logic to be handled by frontend if needed, or refine here
                })
                .ToListAsync();

            return Json(messages);
        }
    }
}
