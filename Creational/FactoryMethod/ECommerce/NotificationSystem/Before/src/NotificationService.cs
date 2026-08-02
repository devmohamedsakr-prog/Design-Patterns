using System;
using System.Threading.Tasks;

namespace NotificationSystem.Before.Src
{
    /// <summary>
    /// BEFORE: Tightly coupled notification service
    /// ❌ PROBLEMS: Hard-coded if-else for each notification type
    /// </summary>
    public class NotificationService
    {
        public async Task<NotificationResult> SendNotificationAsync(string notificationType, string recipientId, 
            string message, string subject = "")
        {
            try
            {
                // ❌ PROBLEM: Hard-coded if-else
                if (notificationType == "Email")
                {
                    return await SendEmailNotification(recipientId, message, subject);
                }
                else if (notificationType == "SMS")
                {
                    return await SendSmsNotification(recipientId, message);
                }
                else if (notificationType == "Push")
                {
                    return await SendPushNotification(recipientId, message);
                }
                else
                {
                    return new NotificationResult { Success = false, Message = "Unknown notification type" };
                }
            }
            catch (Exception ex)
            {
                return new NotificationResult { Success = false, Message = ex.Message };
            }
        }

        // ❌ PROBLEM: All notification logic in single class
        private async Task<NotificationResult> SendEmailNotification(string email, string message, string subject)
        {
            if (!email.Contains("@"))
                return new NotificationResult { Success = false, Message = "Invalid email" };
            if (string.IsNullOrEmpty(message))
                return new NotificationResult { Success = false, Message = "Empty message" };

            await Task.Delay(100);

            return new NotificationResult
            {
                Success = true,
                MessageId = $"email_{email}_{DateTime.Now.Ticks}",
                Message = $"Email sent to {email}",
                HandlerName = "Email"
            };
        }

        private async Task<NotificationResult> SendSmsNotification(string phone, string message)
        {
            if (!phone.All(char.IsDigit))
                return new NotificationResult { Success = false, Message = "Invalid phone" };
            if (string.IsNullOrEmpty(message))
                return new NotificationResult { Success = false, Message = "Empty message" };

            await Task.Delay(75);

            return new NotificationResult
            {
                Success = true,
                MessageId = $"sms_{phone}_{DateTime.Now.Ticks}",
                Message = $"SMS sent to {phone}",
                HandlerName = "SMS"
            };
        }

        private async Task<NotificationResult> SendPushNotification(string deviceId, string message)
        {
            if (string.IsNullOrEmpty(deviceId) || deviceId.Length < 10)
                return new NotificationResult { Success = false, Message = "Invalid device ID" };
            if (string.IsNullOrEmpty(message))
                return new NotificationResult { Success = false, Message = "Empty message" };

            await Task.Delay(25);

            return new NotificationResult
            {
                Success = true,
                MessageId = $"push_{deviceId}_{DateTime.Now.Ticks}",
                Message = $"Push sent to {deviceId}",
                HandlerName = "Push"
            };
        }
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
    }
}
