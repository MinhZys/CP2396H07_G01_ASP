using Microsoft.AspNetCore.Mvc;
using Symphony.Portal.Web.Models.ViewModels;
using Symphony.Portal.Web.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Symphony.Portal.Web.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;

        public NotificationBellViewComponent(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return View(new NotificationBellVM());
            }

            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            
            // Take top 5 for dropdown
            var vm = new NotificationBellVM
            {
                UnreadCount = unreadCount,
                Notifications = notifications.Take(5).ToList()
            };

            return View(vm);
        }
    }
}
