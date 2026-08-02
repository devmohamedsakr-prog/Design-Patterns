using System;
using System.Collections.Generic;
using Bridge.Notification.Messaging.Abstraction;

namespace Bridge.Notification.Messaging.Implementation
{
    /// <summary>
    /// Implementation interface: Messaging channel contract.
    /// </summary>
    public interface IMessagingChannel
    {
        MessageResult SendEmail(string recipient, string subject, string body, List<string> cc);
        MessageResult SendSMS(string phoneNumber, string message);
        MessageResult SendPush(string deviceToken, string title, string message, Dictionary<string, object> payload);
        MessageResult SendSlack(string channelId, string message, string iconUrl, string username);
    }

    /// <summary>
    /// Implementation: SendGrid email channel.
    /// </summary>
    public class SendGridChannel : IMessagingChannel
    {
        private readonly string _apiKey;

        public SendGridChannel(string apiKey)
        {
            _apiKey = apiKey;
        }

        public MessageResult SendEmail(string recipient, string subject, string body, List<string> cc)
        {
            return new MessageResult
            {
                Success = true,
                MessageId = $"sendgrid_{Guid.NewGuid().ToString().Substring(0, 8)}",
                SentAt = DateTime.UtcNow,
                Channel = "SendGrid"
            };
        }

        public MessageResult SendSMS(string phoneNumber, string message)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "SendGrid: SMS not supported",
                Channel = "SendGrid"
            };
        }

        public MessageResult SendPush(string deviceToken, string title, string message, Dictionary<string, object> payload)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "SendGrid: Push notification not supported",
                Channel = "SendGrid"
            };
        }

        public MessageResult SendSlack(string channelId, string message, string iconUrl, string username)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "SendGrid: Slack not supported",
                Channel = "SendGrid"
            };
        }

        public override string ToString() => "SendGridChannel";
    }

    /// <summary>
    /// Implementation: Twilio SMS channel.
    /// </summary>
    public class TwilioChannel : IMessagingChannel
    {
        private readonly string _accountSid;
        private readonly string _authToken;

        public TwilioChannel(string accountSid, string authToken)
        {
            _accountSid = accountSid;
            _authToken = authToken;
        }

        public MessageResult SendEmail(string recipient, string subject, string body, List<string> cc)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "Twilio: Email not supported",
                Channel = "Twilio"
            };
        }

        public MessageResult SendSMS(string phoneNumber, string message)
        {
            return new MessageResult
            {
                Success = true,
                MessageId = $"twilio_{Guid.NewGuid().ToString().Substring(0, 8)}",
                SentAt = DateTime.UtcNow,
                Channel = "Twilio"
            };
        }

        public MessageResult SendPush(string deviceToken, string title, string message, Dictionary<string, object> payload)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "Twilio: Push notification not supported",
                Channel = "Twilio"
            };
        }

        public MessageResult SendSlack(string channelId, string message, string iconUrl, string username)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "Twilio: Slack not supported",
                Channel = "Twilio"
            };
        }

        public override string ToString() => "TwilioChannel";
    }

    /// <summary>
    /// Implementation: Firebase Cloud Messaging channel.
    /// </summary>
    public class FirebaseChannel : IMessagingChannel
    {
        private readonly string _projectId;

        public FirebaseChannel(string projectId)
        {
            _projectId = projectId;
        }

        public MessageResult SendEmail(string recipient, string subject, string body, List<string> cc)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "Firebase: Email not supported",
                Channel = "Firebase"
            };
        }

        public MessageResult SendSMS(string phoneNumber, string message)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "Firebase: SMS not supported",
                Channel = "Firebase"
            };
        }

        public MessageResult SendPush(string deviceToken, string title, string message, Dictionary<string, object> payload)
        {
            return new MessageResult
            {
                Success = true,
                MessageId = $"firebase_{Guid.NewGuid().ToString().Substring(0, 8)}",
                SentAt = DateTime.UtcNow,
                Channel = "Firebase"
            };
        }

        public MessageResult SendSlack(string channelId, string message, string iconUrl, string username)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "Firebase: Slack not supported",
                Channel = "Firebase"
            };
        }

        public override string ToString() => "FirebaseChannel";
    }

    /// <summary>
    /// Implementation: Slack webhook channel.
    /// </summary>
    public class SlackWebhookChannel : IMessagingChannel
    {
        private readonly string _webhookUrl;

        public SlackWebhookChannel(string webhookUrl)
        {
            _webhookUrl = webhookUrl;
        }

        public MessageResult SendEmail(string recipient, string subject, string body, List<string> cc)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "Slack: Email not supported",
                Channel = "Slack"
            };
        }

        public MessageResult SendSMS(string phoneNumber, string message)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "Slack: SMS not supported",
                Channel = "Slack"
            };
        }

        public MessageResult SendPush(string deviceToken, string title, string message, Dictionary<string, object> payload)
        {
            return new MessageResult
            {
                Success = false,
                ErrorMessage = "Slack: Push notification not supported",
                Channel = "Slack"
            };
        }

        public MessageResult SendSlack(string channelId, string message, string iconUrl, string username)
        {
            return new MessageResult
            {
                Success = true,
                MessageId = $"slack_{Guid.NewGuid().ToString().Substring(0, 8)}",
                SentAt = DateTime.UtcNow,
                Channel = "Slack"
            };
        }

        public override string ToString() => "SlackWebhookChannel";
    }
}
