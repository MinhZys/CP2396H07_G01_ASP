using Microsoft.EntityFrameworkCore;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Symphony.Portal.Web.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string title, string message);
        Task BroadcastNotificationAsync(string title, string message);
        Task<List<Notification>> GetUserNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task SendNotificationToRolesAsync(List<string> roleNames, string title, string message);
    }

    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(string userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task BroadcastNotificationAsync(string title, string message)
        {
            var users = await _context.Users.Where(u => u.IsActive).ToListAsync();
            var notifications = users.Select(u => new Notification
            {
                UserId = u.Id,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            }).ToList();

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
        {
            // Get latest notifications first
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (unreadNotifications.Any())
            {
                foreach (var n in unreadNotifications)
                {
                    n.IsRead = true;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendNotificationToRolesAsync(List<string> roleNames, string title, string message)
        {
            if (roleNames == null || !roleNames.Any()) return;

            var users = await _context.Users
                .Include(u => u.Role)
                .Where(u => u.IsActive && u.Role != null && roleNames.Contains(u.Role.Name))
                .ToListAsync();

            if (!users.Any()) return;

            var notifications = users.Select(u => new Notification
            {
                UserId = u.Id,
                Title = title,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            }).ToList();

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();
        }
    }
}
