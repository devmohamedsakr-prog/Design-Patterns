using System;
using System.Collections.Generic;
using Bridge.Notification.Messaging.Implementation;

namespace Bridge.Notification.Messaging.Abstraction
{
    /// <summary>
    /// Abstraction: Notification sending operations.
    /// Demonstrates: Bridge pattern for multi-channel messaging.
    /// </summary>
    public abstract class Notification
    {
        protected IMessagingChannel _channel;

        public Notification(IMessagingChannel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        public abstract MessageResult Send();

        public void SetChannel(IMessagingChannel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }
    }

    /// <summary>
    /// Concrete abstraction: Email notification.
    /// </summary>
    public class EmailNotification : Notification
    {
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> CcEmails { get; set; }

        public EmailNotification(IMessagingChannel channel) : base(channel)
        {
            CcEmails = new List<string>();
        }

        public override MessageResult Send()
        {
            return _channel.SendEmail(RecipientEmail, Subject, Body, CcEmails);
        }

        public override string ToString() =>
            $"Email Notification(To={RecipientEmail}, Subject={Subject})";
    }

    /// <summary>
    /// Concrete abstraction: SMS notification.
    /// </summary>
    public class SMSNotification : Notification
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }

        public SMSNotification(IMessagingChannel channel) : base(channel)
        {
        }

        public override MessageResult Send()
        {
            return _channel.SendSMS(PhoneNumber, Message);
        }

        public override string ToString() =>
            $"SMS Notification(Phone={PhoneNumber}, Length={Message?.Length ?? 0})";
    }

    /// <summary>
    /// Concrete abstraction: Push notification.
    /// </summary>
    public class PushNotification : Notification
    {
        public string DeviceToken { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> Payload { get; set; }

        public PushNotification(IMessagingChannel channel) : base(channel)
        {
            Payload = new Dictionary<string, object>();
        }

        public override MessageResult Send()
        {
            return _channel.SendPush(DeviceToken, Title, Message, Payload);
        }

        public override string ToString() =>
            $"Push Notification(Title={Title}, Device={DeviceToken.Substring(0, 8)}...)";
    }

    /// <summary>
    /// Concrete abstraction: Slack notification.
    /// </summary>
    public class SlackNotification : Notification
    {
        public string ChannelId { get; set; }
        public string Message { get; set; }
        public string IconUrl { get; set; }
        public string Username { get; set; }

        public SlackNotification(IMessagingChannel channel) : base(channel)
        {
        }

        public override MessageResult Send()
        {
            return _channel.SendSlack(ChannelId, Message, IconUrl, Username);
        }

        public override string ToString() =>
            $"Slack Notification(Channel={ChannelId}, From={Username})";
    }

    /// <summary>
    /// Message result.
    /// </summary>
    public class MessageResult
    {
        public bool Success { get; set; }
        public string MessageId { get; set; }
        public DateTime SentAt { get; set; }
        public string ErrorMessage { get; set; }
        public string Channel { get; set; }

        public override string ToString() =>
            $"MessageResult(Success={Success}, Channel={Channel}, MessageId={MessageId})";
    }

    /// <summary>
    /// Notification dispatcher supporting multiple channels.
    /// </summary>
    public class NotificationDispatcher
    {
        private readonly List<Notification> _notifications = new List<Notification>();
        private readonly Dictionary<string, IMessagingChannel> _channels = 
            new Dictionary<string, IMessagingChannel>();

        public void RegisterChannel(string name, IMessagingChannel channel)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Channel name cannot be empty", nameof(name));
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));

            _channels[name] = channel;
        }

        public void AddNotification(Notification notification)
        {
            if (notification == null)
                throw new ArgumentNullException(nameof(notification));
            _notifications.Add(notification);
        }

        public void DispatchAll()
        {
            foreach (var notification in _notifications)
            {
                notification.Send();
            }
        }

        public List<MessageResult> DispatchToChannel(string channelName)
        {
            if (!_channels.ContainsKey(channelName))
                throw new KeyNotFoundException($"Channel {channelName} not registered");

            var results = new List<MessageResult>();
            var channel = _channels[channelName];

            foreach (var notification in _notifications)
            {
                notification.SetChannel(channel);
                results.Add(notification.Send());
            }

            return results;
        }

        public int NotificationCount => _notifications.Count;
        public int ChannelCount => _channels.Count;
    }
}
