using Microsoft.AspNetCore.SignalR;
using Symphony.Portal.Web.Data;
using Symphony.Portal.Web.Models;
using Symphony.Portal.Web.Services;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Symphony.Portal.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly IOllamaService _ollamaService;
        
        // Removed session intervention tracking to keep AI and Human support independent as requested.

        public ChatHub(AppDbContext context, IOllamaService ollamaService)
        {
            _context = context;
            _ollamaService = ollamaService;
        }

        public async Task SendMessage(string receiverId, string content, string senderName, string sessionId)
        {
            var senderId = Context.UserIdentifier;
            
            // Normalize IDs to handle empty strings from client
            var dbSenderId = string.IsNullOrEmpty(senderId) ? null : senderId;
            var dbReceiverId = string.IsNullOrEmpty(receiverId) ? null : receiverId;

            // Save to DB
            var message = new ChatMessage
            {
                SenderId = dbSenderId,
                ReceiverId = dbReceiverId,
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

        /// <summary>
        /// Ask AI for a response - used by chat widget
        /// </summary>
        public async Task AskAI(string question, string sessionId, string senderName)
        {
            try
            {

                // 1. Save USER's question to DB (so Admin can see it)
                var userId = Context.UserIdentifier;
                var userMsg = new ChatMessage 
                {
                    SenderId = string.IsNullOrEmpty(userId) ? null : userId,
                    ReceiverId = null, // To System/Admin
                    Content = question,
                    Timestamp = DateTime.Now,
                    SessionId = sessionId,
                    SenderValidName = senderName
                };
                _context.ChatMessages.Add(userMsg);
                await _context.SaveChangesAsync();

                // 2. Broadcast User's question to Admins
                await Clients.Group("Admins").SendAsync("ReceiveMessage", userMsg.SenderId, senderName, question, sessionId, userMsg.Timestamp.ToString("HH:mm"));

                // Notify client that AI is thinking
                await Clients.Caller.SendAsync("AITyping", true);

                // Generate AI response
                var response = await _ollamaService.GenerateResponseAsync(question);

                // Save AI response to database
                // Note: SenderId is null for AI messages, we use SenderValidName to identify AI                                                
                var aiMessage = new ChatMessage
                {
                    SenderId = null, // AI doesn't have a user ID
                    ReceiverId = null,
                    Content = response,
                    Timestamp = DateTime.Now,
                    IsRead = false,
                    SessionId = sessionId,
                    SenderValidName = "AI Assistant" // This identifies the message as from AI
                };

                _context.ChatMessages.Add(aiMessage);
                await _context.SaveChangesAsync();

                // Send response to caller (User)
                await Clients.Caller.SendAsync("ReceiveAIMessage", response, aiMessage.Timestamp.ToString("HH:mm"));
                
                // Broadcast AI response to Admins (so they see the thread flow)
                await Clients.Group("Admins").SendAsync("ReceiveMessage", null, "AI Assistant", response, sessionId, aiMessage.Timestamp.ToString("HH:mm"));

            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"AI Chat Error: {ex.Message}");
                await Clients.Caller.SendAsync("ReceiveAIMessage", "Xin lỗi, đã xảy ra lỗi. Vui lòng thử lại sau.", DateTime.Now.ToString("HH:mm"));
            }
            finally
            {
                // Notify client that AI stopped typing
                await Clients.Caller.SendAsync("AITyping", false);
            }
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

