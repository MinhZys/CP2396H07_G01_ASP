using Microsoft.AspNetCore.Mvc;
using Symphony.Portal.Web.Services;
using System.Security.Claims;

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
                return View(0);
            }

            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
            return View(unreadCount);
        }
    }
}
