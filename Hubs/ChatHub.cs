using Microsoft.AspNetCore.SignalR;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using System;
using System.Threading.Tasks;

namespace Symphony.Portal.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string receiverId, string content, string senderName, string sessionId)
        {
            var senderId = Context.UserIdentifier;
            
            // Save to DB
            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.Now,
                IsRead = false,
                SessionId = sessionId,
                SenderValidName = senderName
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            // Broadcast to Receiver
            if (!string.IsNullOrEmpty(receiverId))
            {
                if (receiverId.StartsWith("Session_"))
                {
                     await Clients.Group(receiverId).SendAsync("ReceiveMessage", senderId, senderName, content, sessionId, message.Timestamp.ToString("HH:mm"));
                }
                else
                {
                    await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, senderName, content, sessionId, message.Timestamp.ToString("HH:mm"));
                }
            }
            else
            {
                // Send to Admin Group (if receiver is null/empty, assumed support request)
                await Clients.Group("Admins").SendAsync("ReceiveMessage", senderId, senderName, content, sessionId, message.Timestamp.ToString("HH:mm"));
            }

            // Send back to Sender (for UI update consistency, optional if UI handles optimistic)
            await Clients.Caller.SendAsync("MessageSent", content);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole(RoleNames.Admin))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }
            await base.OnConnectedAsync();
        }
    }
}
