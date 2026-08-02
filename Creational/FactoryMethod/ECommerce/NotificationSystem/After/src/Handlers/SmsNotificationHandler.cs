using System;
using System.Threading.Tasks;
using NotificationSystem.After.Abstracts;

namespace NotificationSystem.After.Handlers
{
    /// <summary>
    /// SMS Notification Handler
    /// Concrete product created by SmsNotificationCreator factory method
    /// </summary>
    public class SmsNotificationHandler : INotificationHandler
    {
        public string GetHandlerName() => "SMS";

        public async Task<NotificationResult> SendAsync(string recipientId, string message, string subject)
        {
            // Validate phone number (simple check)
            if (!recipientId.All(char.IsDigit))
                return new NotificationResult { Success = false, Message = "Invalid phone number" };

            // Simulate SMS sending (medium speed)
            await Task.Delay(75);

            return new NotificationResult
            {
                Success = true,
                MessageId = $"sms_{recipientId}_{DateTime.Now.Ticks}",
                Message = $"SMS sent to {recipientId}",
                HandlerName = GetHandlerName()
            };
        }
    }
}
