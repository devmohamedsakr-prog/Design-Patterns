using System;
using System.Threading.Tasks;
using NotificationSystem.After.Abstracts;

namespace NotificationSystem.After.Handlers
{
    /// <summary>
    /// Push Notification Handler
    /// Concrete product created by PushNotificationCreator factory method
    /// </summary>
    public class PushNotificationHandler : INotificationHandler
    {
        public string GetHandlerName() => "Push";

        public async Task<NotificationResult> SendAsync(string recipientId, string message, string subject)
        {
            // Validate device ID format
            if (string.IsNullOrEmpty(recipientId) || recipientId.Length < 10)
                return new NotificationResult { Success = false, Message = "Invalid device ID" };

            // Simulate push sending (fastest)
            await Task.Delay(25);

            return new NotificationResult
            {
                Success = true,
                MessageId = $"push_{recipientId}_{DateTime.Now.Ticks}",
                Message = $"Push notification sent to {recipientId}",
                HandlerName = GetHandlerName()
            };
        }
    }
}
