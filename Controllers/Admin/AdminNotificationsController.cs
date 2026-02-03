using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Symphony.Portal.Web.Services;
using System.Threading.Tasks;

namespace Symphony.Portal.Web.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]/[action]")]
    public class AdminNotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public AdminNotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string title, string message, List<string> selectedRoles)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(message))
            {
                TempData["Error"] = "Title and Message are required.";
                return View();
            }

            if (selectedRoles == null || !selectedRoles.Any() || selectedRoles.Contains("All"))
            {
                await _notificationService.BroadcastNotificationAsync(title, message);
                TempData["Success"] = "Notification sent to all users successfully!";
            }
            else
            {
                await _notificationService.SendNotificationToRolesAsync(selectedRoles, title, message);
                TempData["Success"] = $"Notification sent to {string.Join(", ", selectedRoles)} successfully!";
            }
            
            return RedirectToAction(nameof(Index), "Dashboard");
        }
    }
}
