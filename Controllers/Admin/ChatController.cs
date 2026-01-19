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
            // Group messages by SessionId (for guests) or SenderId (for users)
            // This is a simplified "Recent Chats" list
            var recentMessages = await _context.ChatMessages
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();

            var chatSessions = recentMessages
                .GroupBy(m => m.SessionId ?? m.SenderId ?? "Unknown")
                .Select(g => g.First())
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
