using System;
using System.Threading.Tasks;

namespace NotificationSystem.After.Abstracts
{
    /// <summary>
    /// Notification Creator: Abstract base class for factory method pattern
    /// Each subclass implements CreateNotificationHandler() to create specific notification type
    /// </summary>
    public abstract class NotificationCreator
    {
        /// <summary>
        /// Factory Method: Abstract - subclasses must implement
        /// </summary>
        protected abstract INotificationHandler CreateNotificationHandler();

        /// <summary>
        /// Template Method: Uses factory method to send notifications
        /// </summary>
        public async Task<NotificationResult> SendNotificationAsync(string recipientId, string message, string subject = "")
        {
            try
            {
                if (!ValidateNotification(recipientId, message))
                    return new NotificationResult { Success = false, Message = "Validation failed" };

                INotificationHandler handler = CreateNotificationHandler();
                NotificationResult result = await handler.SendAsync(recipientId, message, subject);

                if (result.Success)
                    LogNotification(recipientId, handler.GetHandlerName(), "SUCCESS");
                else
                    LogNotification(recipientId, handler.GetHandlerName(), "FAILED");

                return result;
            }
            catch (Exception ex)
            {
                LogNotification(recipientId, "UNKNOWN", $"ERROR: {ex.Message}");
                return new NotificationResult { Success = false, Message = ex.Message };
            }
        }

        protected virtual bool ValidateNotification(string recipientId, string message)
        {
            return !string.IsNullOrEmpty(recipientId) && !string.IsNullOrEmpty(message);
        }

        protected virtual void LogNotification(string recipientId, string handler, string status)
        {
            Console.WriteLine($"[LOG] Recipient: {recipientId}, Handler: {handler}, Status: {status}");
        }
    }

    /// <summary>
    /// Notification Handler Interface: Product interface
    /// </summary>
    public interface INotificationHandler
    {
        Task<NotificationResult> SendAsync(string recipientId, string message, string subject);
        string GetHandlerName();
    }

    /// <summary>
    /// Notification Result Model
    /// </summary>
    public class NotificationResult
    {
        public bool Success { get; set; }
        public string MessageId { get; set; }
        public string Message { get; set; }
        public string HandlerName { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
