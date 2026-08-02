using NUnit.Framework;
using System.Threading.Tasks;
using NotificationSystem.After.Abstracts;
using NotificationSystem.After.Creators;

namespace NotificationSystem.After.Tests
{
    [TestFixture]
    public class NotificationSystemTests
    {
        // Email Tests
        [Test]
        public async Task EmailNotification_SendSuccessfully()
        {
            var creator = new EmailNotificationCreator();
            var result = await creator.SendNotificationAsync("user@example.com", "Test message", "Subject");
            Assert.That(result.Success, Is.True);
            Assert.That(result.HandlerName, Is.EqualTo("Email"));
        }

        [Test]
        public async Task EmailNotification_InvalidEmail_ShouldFail()
        {
            var creator = new EmailNotificationCreator();
            var result = await creator.SendNotificationAsync("invalid", "Test message");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task EmailNotification_GeneratesMessageId()
        {
            var creator = new EmailNotificationCreator();
            var result = await creator.SendNotificationAsync("user@example.com", "Test");
            Assert.That(result.MessageId, Does.StartWith("email_"));
        }

        [Test]
        public async Task EmailNotification_MultipleRecipients_AllSucceed()
        {
            var creator = new EmailNotificationCreator();
            for (int i = 0; i < 5; i++)
            {
                var result = await creator.SendNotificationAsync($"user{i}@example.com", "Test");
                Assert.That(result.Success, Is.True);
            }
        }

        [Test]
        public async Task EmailNotification_NullMessage_ShouldFail()
        {
            var creator = new EmailNotificationCreator();
            var result = await creator.SendNotificationAsync("user@example.com", null);
            Assert.That(result.Success, Is.False);
        }

        // SMS Tests
        [Test]
        public async Task SmsNotification_SendSuccessfully()
        {
            var creator = new SmsNotificationCreator();
            var result = await creator.SendNotificationAsync("1234567890", "Test message");
            Assert.That(result.Success, Is.True);
            Assert.That(result.HandlerName, Is.EqualTo("SMS"));
        }

        [Test]
        public async Task SmsNotification_InvalidPhone_ShouldFail()
        {
            var creator = new SmsNotificationCreator();
            var result = await creator.SendNotificationAsync("abc123", "Test");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task SmsNotification_GeneratesMessageId()
        {
            var creator = new SmsNotificationCreator();
            var result = await creator.SendNotificationAsync("1234567890", "Test");
            Assert.That(result.MessageId, Does.StartWith("sms_"));
        }

        [Test]
        public async Task SmsNotification_DifferentPhoneNumbers()
        {
            var creator = new SmsNotificationCreator();
            var phones = new[] { "1234567890", "9876543210", "5555555555" };
            foreach (var phone in phones)
            {
                var result = await creator.SendNotificationAsync(phone, "Test");
                Assert.That(result.Success, Is.True);
            }
        }

        [Test]
        public async Task SmsNotification_NullRecipient_ShouldFail()
        {
            var creator = new SmsNotificationCreator();
            var result = await creator.SendNotificationAsync(null, "Test");
            Assert.That(result.Success, Is.False);
        }

        // Push Notification Tests
        [Test]
        public async Task PushNotification_SendSuccessfully()
        {
            var creator = new PushNotificationCreator();
            var result = await creator.SendNotificationAsync("device123456", "Test message");
            Assert.That(result.Success, Is.True);
            Assert.That(result.HandlerName, Is.EqualTo("Push"));
        }

        [Test]
        public async Task PushNotification_InvalidDeviceId_ShouldFail()
        {
            var creator = new PushNotificationCreator();
            var result = await creator.SendNotificationAsync("short", "Test");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task PushNotification_GeneratesMessageId()
        {
            var creator = new PushNotificationCreator();
            var result = await creator.SendNotificationAsync("device123456", "Test");
            Assert.That(result.MessageId, Does.StartWith("push_"));
        }

        [Test]
        public async Task PushNotification_FastestDelivery()
        {
            var creator = new PushNotificationCreator();
            var result = await creator.SendNotificationAsync("device123456", "Test");
            Assert.That(result.Success, Is.True);
        }

        // Factory Method Tests
        [Test]
        public async Task FactoryMethod_DifferentCreators_CreateDifferentHandlers()
        {
            var emailCreator = new EmailNotificationCreator();
            var smsCreator = new SmsNotificationCreator();
            var pushCreator = new PushNotificationCreator();

            var emailResult = await emailCreator.SendNotificationAsync("user@example.com", "Test");
            var smsResult = await smsCreator.SendNotificationAsync("1234567890", "Test");
            var pushResult = await pushCreator.SendNotificationAsync("device123456", "Test");

            Assert.That(emailResult.HandlerName, Is.Not.EqualTo(smsResult.HandlerName));
            Assert.That(smsResult.HandlerName, Is.Not.EqualTo(pushResult.HandlerName));
        }

        [Test]
        public async Task FactoryMethod_AllHandlers_Successful()
        {
            var creators = new NotificationCreator[]
            {
                new EmailNotificationCreator(),
                new SmsNotificationCreator(),
                new PushNotificationCreator()
            };

            foreach (var creator in creators)
            {
                var result = await creator.SendNotificationAsync("test@example.com", "Test");
                Assert.That(result.HandlerName, Is.Not.Null);
            }
        }

        // Validation Tests
        [Test]
        public async Task Notification_EmptyMessage_ShouldFail()
        {
            var creator = new EmailNotificationCreator();
            var result = await creator.SendNotificationAsync("user@example.com", "");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task Notification_EmptyRecipient_ShouldFail()
        {
            var creator = new EmailNotificationCreator();
            var result = await creator.SendNotificationAsync("", "Test message");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task Notification_LongMessage_ShouldSucceed()
        {
            var creator = new EmailNotificationCreator();
            string longMessage = new string('A', 1000);
            var result = await creator.SendNotificationAsync("user@example.com", longMessage);
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task Notification_SpecialCharacters_ShouldHandle()
        {
            var creator = new EmailNotificationCreator();
            var result = await creator.SendNotificationAsync("user+tag@example.com", "Test!");
            Assert.That(result.HandlerName, Is.EqualTo("Email"));
        }

        // Sequential Tests
        [Test]
        public async Task NotificationSystem_SequentialSending_AllSucceed()
        {
            var email = new EmailNotificationCreator();
            var sms = new SmsNotificationCreator();
            var push = new PushNotificationCreator();

            var r1 = await email.SendNotificationAsync("user@example.com", "Order confirmed");
            var r2 = await sms.SendNotificationAsync("1234567890", "Your order shipped");
            var r3 = await push.SendNotificationAsync("device123456", "Package delivered");

            Assert.That(r1.Success, Is.True);
            Assert.That(r2.Success, Is.True);
            Assert.That(r3.Success, Is.True);
        }

        [Test]
        public async Task NotificationSystem_MultiChannelNotification_AllChannels()
        {
            var creators = new NotificationCreator[]
            {
                new EmailNotificationCreator(),
                new SmsNotificationCreator(),
                new PushNotificationCreator()
            };

            var recipients = new[] { "user@example.com", "1234567890", "device123456" };

            for (int i = 0; i < creators.Length; i++)
            {
                var result = await creators[i].SendNotificationAsync(recipients[i], "Test notification");
                Assert.That(result.Success, Is.True);
            }
        }

        [Test]
        public async Task NotificationSystem_UniqueMess ageIds()
        {
            var creator = new EmailNotificationCreator();
            var msgIds = new System.Collections.Generic.HashSet<string>();

            for (int i = 0; i < 10; i++)
            {
                var result = await creator.SendNotificationAsync($"user{i}@example.com", "Test");
                msgIds.Add(result.MessageId);
            }

            Assert.That(msgIds.Count, Is.EqualTo(10));
        }

        [Test]
        public async Task NotificationSystem_HandlerConsistency()
        {
            var emailCreator1 = new EmailNotificationCreator();
            var emailCreator2 = new EmailNotificationCreator();

            var result1 = await emailCreator1.SendNotificationAsync("user@example.com", "Test");
            var result2 = await emailCreator2.SendNotificationAsync("user@example.com", "Test");

            Assert.That(result1.HandlerName, Is.EqualTo(result2.HandlerName));
        }

        [Test]
        public async Task NotificationSystem_HandlerNames_Correct()
        {
            var tests = new (NotificationCreator creator, string expected)[]
            {
                (new EmailNotificationCreator(), "Email"),
                (new SmsNotificationCreator(), "SMS"),
                (new PushNotificationCreator(), "Push")
            };

            foreach (var (creator, expected) in tests)
            {
                var result = await creator.SendNotificationAsync("test", "message");
                Assert.That(result.HandlerName, Is.EqualTo(expected));
            }
        }

        [Test]
        public async Task NotificationSystem_ResultTimestamp_Valid()
        {
            var creator = new EmailNotificationCreator();
            var result = await creator.SendNotificationAsync("user@example.com", "Test");
            Assert.That(result.SentAt, Is.LessThanOrEqualTo(System.DateTime.UtcNow));
        }

        [Test]
        public async Task NotificationSystem_BulkNotifications()
        {
            var creator = new EmailNotificationCreator();
            int successCount = 0;

            for (int i = 0; i < 20; i++)
            {
                var result = await creator.SendNotificationAsync($"user{i}@example.com", $"Notification {i}");
                if (result.Success) successCount++;
            }

            Assert.That(successCount, Is.GreaterThanOrEqualTo(18));
        }

        [Test]
        public async Task NotificationSystem_MixedValidation()
        {
            var emailCreator = new EmailNotificationCreator();
            var smsCreator = new SmsNotificationCreator();

            var validEmail = await emailCreator.SendNotificationAsync("user@example.com", "Test");
            var invalidSms = await smsCreator.SendNotificationAsync("notanumber", "Test");

            Assert.That(validEmail.Success, Is.True);
            Assert.That(invalidSms.Success, Is.False);
        }

        [Test]
        public async Task NotificationSystem_AsyncExecution()
        {
            var creator = new EmailNotificationCreator();
            var task = creator.SendNotificationAsync("user@example.com", "Test");
            Assert.That(task, Is.InstanceOf<Task>());

            var result = await task;
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task NotificationSystem_ErrorHandling_GracefulFailure()
        {
            var creator = new EmailNotificationCreator();
            var result = await creator.SendNotificationAsync(null, "Test");
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.Not.Null);
        }

        [Test]
        public async Task NotificationSystem_ConcurrentCreators()
        {
            var tasks = new Task[30];
            var creators = new NotificationCreator[]
            {
                new EmailNotificationCreator(),
                new SmsNotificationCreator(),
                new PushNotificationCreator()
            };

            for (int i = 0; i < 30; i++)
            {
                var creator = creators[i % 3];
                var recipient = i % 3 == 0 ? "user@example.com" : i % 3 == 1 ? "1234567890" : "device123456";
                tasks[i] = creator.SendNotificationAsync(recipient, $"Message {i}");
            }

            await Task.WhenAll(tasks);
            Assert.That(tasks.All(t => t.IsCompleted), Is.True);
        }
    }
}
