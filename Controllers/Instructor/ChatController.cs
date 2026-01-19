using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;

namespace Symphony.Portal.Web.Controllers.Instructor
{
    [Authorize(Roles = RoleNames.Instructor)]
    [Route("Instructor/[controller]/[action]")]
    public class ChatController : Controller
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Simplified: Instructor sees all conversations for now, 
            // OR refine to only students in their courses.
            // For MVP: Instructor sees students who messaged them + Admin
            
            // To properly filter, we'd need ChatMessage to differentiate "Type" or infer from ReceiverId.
            // But currently, messages are just Sender/Receiver.
            // Instructor sees messages where ReceiverId == Me OR SenderId == Me.
            
            var myId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var recentMessages = await _context.ChatMessages
                .Where(m => m.SenderId == myId || m.ReceiverId == myId)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();

            var chatSessions = recentMessages
                .GroupBy(m => m.SenderId == myId ? m.ReceiverId : m.SenderId)
                .Select(g => g.First())
                .ToList();

            return View(chatSessions);
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory(string id)
        {
            var myId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var messages = await _context.ChatMessages
                .Where(m => (m.SenderId == myId && m.ReceiverId == id) || (m.SenderId == id && m.ReceiverId == myId))
                .OrderBy(m => m.Timestamp)
                .Select(m => new {
                    m.SenderValidName,
                    m.Content,
                    Time = m.Timestamp.ToString("HH:mm"),
                    IsSelf = m.SenderId == myId
                })
                .ToListAsync();

            return Json(messages);
        }
    }
}
