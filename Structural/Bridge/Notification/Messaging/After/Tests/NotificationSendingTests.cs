using Xunit;
using Bridge.Notification.Messaging.Abstraction;
using Bridge.Notification.Messaging.Implementation;
using System.Collections.Generic;

namespace Bridge.Notification.Messaging.Tests
{
    public class NotificationSendingTests
    {
        [Fact]
        public void EmailNotification_SendWithSendGrid_Success()
        {
            var channel = new SendGridChannel("api_key_123");
            var notification = new EmailNotification(channel)
            {
                RecipientEmail = "john@example.com",
                Subject = "Welcome",
                Body = "Welcome to our service"
            };

            var result = notification.Send();

            Assert.True(result.Success);
            Assert.Equal("SendGrid", result.Channel);
        }

        [Fact]
        public void SMSNotification_SendWithTwilio_Success()
        {
            var channel = new TwilioChannel("account_sid", "auth_token");
            var notification = new SMSNotification(channel)
            {
                PhoneNumber = "+1234567890",
                Message = "Your code is 123456"
            };

            var result = notification.Send();

            Assert.True(result.Success);
            Assert.Equal("Twilio", result.Channel);
        }

        [Fact]
        public void PushNotification_SendWithFirebase_Success()
        {
            var channel = new FirebaseChannel("project_id");
            var notification = new PushNotification(channel)
            {
                DeviceToken = "device_token_12345",
                Title = "New Message",
                Message = "You have a new message"
            };

            var result = notification.Send();

            Assert.True(result.Success);
            Assert.Equal("Firebase", result.Channel);
        }

        [Fact]
        public void SlackNotification_SendWithSlackWebhook_Success()
        {
            var channel = new SlackWebhookChannel("https://hooks.slack.com/...");
            var notification = new SlackNotification(channel)
            {
                ChannelId = "general",
                Message = "System notification",
                Username = "bot"
            };

            var result = notification.Send();

            Assert.True(result.Success);
            Assert.Equal("Slack", result.Channel);
        }

        [Fact]
        public void Notification_SwitchChannel_Success()
        {
            var sendgrid = new SendGridChannel("key1");
            var notification = new EmailNotification(sendgrid)
            {
                RecipientEmail = "john@example.com",
                Subject = "Test",
                Body = "Body"
            };

            var result1 = notification.Send();
            Assert.True(result1.Success);

            var twilioChannel = new TwilioChannel("sid", "token");
            notification.SetChannel(twilioChannel);

            // Note: TwilioChannel doesn't support email, so should fail
            var result2 = notification.Send();
            Assert.False(result2.Success);
        }

        [Fact]
        public void EmailNotification_WithCarbonCopy_Success()
        {
            var channel = new SendGridChannel("key");
            var notification = new EmailNotification(channel)
            {
                RecipientEmail = "john@example.com",
                Subject = "Important",
                Body = "Important message"
            };

            notification.CcEmails.Add("manager@example.com");
            notification.CcEmails.Add("backup@example.com");

            var result = notification.Send();

            Assert.True(result.Success);
            Assert.Equal(2, notification.CcEmails.Count);
        }

        [Fact]
        public void PushNotification_WithPayload_Success()
        {
            var channel = new FirebaseChannel("project");
            var notification = new PushNotification(channel)
            {
                DeviceToken = "token123",
                Title = "Sale",
                Message = "50% off now"
            };

            notification.Payload["action"] = "open_app";
            notification.Payload["target"] = "sales";

            var result = notification.Send();

            Assert.True(result.Success);
            Assert.Equal(2, notification.Payload.Count);
        }

        [Fact]
        public void NotificationDispatcher_SendAllNotifications_Success()
        {
            var sendgrid = new SendGridChannel("key");
            var dispatcher = new NotificationDispatcher();
            dispatcher.RegisterChannel("email", sendgrid);

            dispatcher.AddNotification(new EmailNotification(sendgrid)
            {
                RecipientEmail = "john@example.com",
                Subject = "Welcome",
                Body = "Welcome"
            });

            dispatcher.AddNotification(new EmailNotification(sendgrid)
            {
                RecipientEmail = "jane@example.com",
                Subject = "Welcome",
                Body = "Welcome"
            });

            dispatcher.DispatchAll();

            Assert.Equal(2, dispatcher.NotificationCount);
        }

        [Fact]
        public void NotificationDispatcher_DispatchToSpecificChannel_Success()
        {
            var sendgrid = new SendGridChannel("key");
            var dispatcher = new NotificationDispatcher();
            dispatcher.RegisterChannel("email", sendgrid);

            dispatcher.AddNotification(new EmailNotification(sendgrid)
            {
                RecipientEmail = "user@example.com",
                Subject = "Test",
                Body = "Test"
            });

            var results = dispatcher.DispatchToChannel("email");

            Assert.Single(results);
            Assert.True(results[0].Success);
        }

        [Fact]
        public void SendGridChannel_OnlySupportsEmail()
        {
            var channel = new SendGridChannel("key");

            var emailResult = channel.SendEmail("test@example.com", "Subject", "Body", new List<string>());
            Assert.True(emailResult.Success);

            var smsResult = channel.SendSMS("+1234567890", "Message");
            Assert.False(smsResult.Success);
        }

        [Fact]
        public void TwilioChannel_OnlySupportsSMS()
        {
            var channel = new TwilioChannel("sid", "token");

            var smsResult = channel.SendSMS("+1234567890", "Message");
            Assert.True(smsResult.Success);

            var emailResult = channel.SendEmail("test@example.com", "Subject", "Body", new List<string>());
            Assert.False(emailResult.Success);
        }

        [Fact]
        public void FirebaseChannel_OnlySupportsPush()
        {
            var channel = new FirebaseChannel("project");

            var pushResult = channel.SendPush("token", "Title", "Message", new Dictionary<string, object>());
            Assert.True(pushResult.Success);

            var emailResult = channel.SendEmail("test@example.com", "Subject", "Body", new List<string>());
            Assert.False(emailResult.Success);
        }

        [Fact]
        public void NotificationDispatcher_RegisterMultipleChannels_Success()
        {
            var dispatcher = new NotificationDispatcher();
            dispatcher.RegisterChannel("email", new SendGridChannel("key1"));
            dispatcher.RegisterChannel("sms", new TwilioChannel("sid", "token"));
            dispatcher.RegisterChannel("push", new FirebaseChannel("project"));

            Assert.Equal(3, dispatcher.ChannelCount);
        }

        [Fact]
        public void Notification_WithNullChannel_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new EmailNotification(null)
            );

            Assert.Contains("channel", exception.Message);
        }

        [Fact]
        public void SetChannel_WithNullChannel_ThrowsException()
        {
            var channel = new SendGridChannel("key");
            var notification = new EmailNotification(channel);

            var exception = Assert.Throws<ArgumentNullException>(() =>
                notification.SetChannel(null)
            );

            Assert.Contains("channel", exception.Message);
        }

        [Fact]
        public void NotificationDispatcher_DispatchToUnregisteredChannel_ThrowsException()
        {
            var dispatcher = new NotificationDispatcher();
            dispatcher.AddNotification(new EmailNotification(new SendGridChannel("key")));

            var exception = Assert.Throws<KeyNotFoundException>(() =>
                dispatcher.DispatchToChannel("nonexistent")
            );

            Assert.Contains("nonexistent", exception.Message);
        }

        [Fact]
        public void MessageResult_ToString_ContainsInfo()
        {
            var result = new MessageResult
            {
                Success = true,
                MessageId = "msg_123",
                Channel = "SendGrid"
            };

            var str = result.ToString();
            Assert.Contains("True", str);
            Assert.Contains("msg_123", str);
            Assert.Contains("SendGrid", str);
        }
    }
}
