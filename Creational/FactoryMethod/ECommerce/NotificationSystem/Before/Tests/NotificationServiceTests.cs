using NUnit.Framework;
using System.Threading.Tasks;
using NotificationSystem.Before.Src;

namespace NotificationSystem.Before.Tests
{
    [TestFixture]
    public class NotificationServiceTests
    {
        private NotificationService _service;

        [SetUp]
        public void Setup()
        {
            _service = new NotificationService();
        }

        // Email Tests
        [Test]
        public async Task SendEmailNotification_ShouldSucceed()
        {
            var result = await _service.SendNotificationAsync("Email", "user@example.com", "Test message", "Subject");
            Assert.That(result.Success, Is.True);
            Assert.That(result.HandlerName, Is.EqualTo("Email"));
        }

        [Test]
        public async Task SendEmailNotification_InvalidEmail_ShouldFail()
        {
            var result = await _service.SendNotificationAsync("Email", "invalid", "Test");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task SendEmailNotification_GeneratesId()
        {
            var result = await _service.SendNotificationAsync("Email", "user@example.com", "Test");
            Assert.That(result.MessageId, Does.StartWith("email_"));
        }

        [Test]
        public async Task SendEmailNotification_MultipleEmails()
        {
            for (int i = 0; i < 5; i++)
            {
                var result = await _service.SendNotificationAsync("Email", $"user{i}@example.com", "Test");
                Assert.That(result.Success, Is.True);
            }
        }

        [Test]
        public async Task SendEmailNotification_EmptyMessage_ShouldFail()
        {
            var result = await _service.SendNotificationAsync("Email", "user@example.com", "");
            Assert.That(result.Success, Is.False);
        }

        // SMS Tests
        [Test]
        public async Task SendSmsNotification_ShouldSucceed()
        {
            var result = await _service.SendNotificationAsync("SMS", "1234567890", "Test message");
            Assert.That(result.Success, Is.True);
            Assert.That(result.HandlerName, Is.EqualTo("SMS"));
        }

        [Test]
        public async Task SendSmsNotification_InvalidPhone_ShouldFail()
        {
            var result = await _service.SendNotificationAsync("SMS", "abc123", "Test");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task SendSmsNotification_GeneratesId()
        {
            var result = await _service.SendNotificationAsync("SMS", "1234567890", "Test");
            Assert.That(result.MessageId, Does.StartWith("sms_"));
        }

        [Test]
        public async Task SendSmsNotification_DifferentNumbers()
        {
            var phones = new[] { "1234567890", "9876543210", "5555555555" };
            foreach (var phone in phones)
            {
                var result = await _service.SendNotificationAsync("SMS", phone, "Test");
                Assert.That(result.Success, Is.True);
            }
        }

        [Test]
        public async Task SendSmsNotification_EmptyMessage_ShouldFail()
        {
            var result = await _service.SendNotificationAsync("SMS", "1234567890", "");
            Assert.That(result.Success, Is.False);
        }

        // Push Tests
        [Test]
        public async Task SendPushNotification_ShouldSucceed()
        {
            var result = await _service.SendNotificationAsync("Push", "device123456", "Test message");
            Assert.That(result.Success, Is.True);
            Assert.That(result.HandlerName, Is.EqualTo("Push"));
        }

        [Test]
        public async Task SendPushNotification_InvalidDeviceId_ShouldFail()
        {
            var result = await _service.SendNotificationAsync("Push", "short", "Test");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task SendPushNotification_GeneratesId()
        {
            var result = await _service.SendNotificationAsync("Push", "device123456", "Test");
            Assert.That(result.MessageId, Does.StartWith("push_"));
        }

        [Test]
        public async Task SendPushNotification_MultipleDevices()
        {
            for (int i = 0; i < 5; i++)
            {
                var result = await _service.SendNotificationAsync("Push", $"device{i}123456", "Test");
                Assert.That(result.Success, Is.True);
            }
        }

        [Test]
        public async Task SendPushNotification_EmptyMessage_ShouldFail()
        {
            var result = await _service.SendNotificationAsync("Push", "device123456", "");
            Assert.That(result.Success, Is.False);
        }

        // Error Handling
        [Test]
        public async Task SendNotification_UnknownType_ShouldFail()
        {
            var result = await _service.SendNotificationAsync("Telegram", "id", "Test");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task SendNotification_CaseSensitive_LowercaseFails()
        {
            var result = await _service.SendNotificationAsync("email", "user@example.com", "Test");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task SendNotification_NullType_ShouldFail()
        {
            var result = await _service.SendNotificationAsync(null, "user@example.com", "Test");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task SendNotification_Problem_DifficultyAddingNewType()
        {
            // To add WhatsApp, must modify SendNotificationAsync (hard-coded if-else)
            var result = await _service.SendNotificationAsync("WhatsApp", "id", "Test");
            Assert.That(result.Success, Is.False); // Not supported - would need code change!
        }

        [Test]
        public async Task SendNotification_AllTypes_Sequential()
        {
            var r1 = await _service.SendNotificationAsync("Email", "user@example.com", "Test");
            var r2 = await _service.SendNotificationAsync("SMS", "1234567890", "Test");
            var r3 = await _service.SendNotificationAsync("Push", "device123456", "Test");

            Assert.That(r1.Success, Is.True);
            Assert.That(r2.Success, Is.True);
            Assert.That(r3.Success, Is.True);
        }

        [Test]
        public async Task SendNotification_UniqueMessageIds()
        {
            var ids = new System.Collections.Generic.HashSet<string>();
            for (int i = 0; i < 10; i++)
            {
                var result = await _service.SendNotificationAsync("Email", $"user{i}@example.com", "Test");
                ids.Add(result.MessageId);
            }
            Assert.That(ids.Count, Is.EqualTo(10));
        }

        [Test]
        public async Task SendNotification_LongMessage()
        {
            string longMsg = new string('A', 1000);
            var result = await _service.SendNotificationAsync("Email", "user@example.com", longMsg);
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task SendNotification_SpecialCharacters()
        {
            var result = await _service.SendNotificationAsync("Email", "user+tag@example.com", "Test!");
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task SendNotification_NullRecipient()
        {
            var result = await _service.SendNotificationAsync("Email", null, "Test");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task SendNotification_EmptyRecipient()
        {
            var result = await _service.SendNotificationAsync("Email", "", "Test");
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task SendNotification_BulkNotifications()
        {
            int successCount = 0;
            for (int i = 0; i < 15; i++)
            {
                var result = await _service.SendNotificationAsync("Email", $"user{i}@example.com", $"Msg {i}");
                if (result.Success) successCount++;
            }
            Assert.That(successCount, Is.GreaterThanOrEqualTo(13));
        }

        [Test]
        public async Task SendNotification_MixedValidation()
        {
            var validEmail = await _service.SendNotificationAsync("Email", "user@example.com", "Test");
            var invalidSms = await _service.SendNotificationAsync("SMS", "notanumber", "Test");
            Assert.That(validEmail.Success, Is.True);
            Assert.That(invalidSms.Success, Is.False);
        }

        [Test]
        public async Task SendNotification_HandlerNames()
        {
            var types = new[] { "Email", "SMS", "Push" };
            foreach (var type in types)
            {
                var recipient = type == "Email" ? "user@example.com" : type == "SMS" ? "1234567890" : "device123456";
                var result = await _service.SendNotificationAsync(type, recipient, "Test");
                Assert.That(result.HandlerName, Is.EqualTo(type));
            }
        }

        [Test]
        public async Task SendNotification_ConcurrentCalls()
        {
            var tasks = new Task[20];
            for (int i = 0; i < 20; i++)
            {
                tasks[i] = _service.SendNotificationAsync("Email", $"user{i}@example.com", $"Msg {i}");
            }
            await Task.WhenAll(tasks);
            Assert.That(tasks.All(t => t.IsCompleted), Is.True);
        }

        [Test]
        public async Task SendNotification_ErrorHandling()
        {
            var result = await _service.SendNotificationAsync("Email", null, "Test");
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.Not.Null);
        }

        [Test]
        public async Task SendNotification_AsyncBehavior()
        {
            var task = _service.SendNotificationAsync("Email", "user@example.com", "Test");
            Assert.That(task, Is.InstanceOf<Task>());
            var result = await task;
            Assert.That(result, Is.Not.Null);
        }
    }
}
