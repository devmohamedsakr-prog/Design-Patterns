using System;
using System.Threading.Tasks;
using NotificationSystem.After.Abstracts;

namespace NotificationSystem.After.Handlers
{
    /// <summary>
    /// Email Notification Handler
    /// Concrete product created by EmailNotificationCreator factory method
    /// </summary>
    public class EmailNotificationHandler : INotificationHandler
    {
        public string GetHandlerName() => "Email";

        public async Task<NotificationResult> SendAsync(string recipientId, string message, string subject)
        {
            // Validate email
            if (!recipientId.Contains("@"))
                return new NotificationResult { Success = false, Message = "Invalid email address" };

            // Simulate email sending (slowest)
            await Task.Delay(100);

            return new NotificationResult
            {
                Success = true,
                MessageId = $"email_{recipientId}_{DateTime.Now.Ticks}",
                Message = $"Email sent to {recipientId}",
                HandlerName = GetHandlerName()
            };
        }
    }
}
