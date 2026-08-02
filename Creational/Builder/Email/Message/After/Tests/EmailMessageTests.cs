using Xunit;
using Builder.Email.Message.Context;
using System;

namespace Builder.Email.Message.Tests
{
    public class EmailMessageTests
    {
        [Fact]
        public void Builder_CreateBasicEmail_Success()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .Subject("Test Subject")
                .Body("Test Body")
                .Build();

            Assert.Equal("sender@example.com", email.From);
            Assert.Equal("recipient@example.com", email.To);
            Assert.Equal("Test Subject", email.Subject);
            Assert.Equal("Test Body", email.Body);
            Assert.Equal("PlainText", email.BodyType);
            Assert.Equal("Normal", email.Priority);
        }

        [Fact]
        public void Builder_WithHtmlBody_Success()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .Subject("HTML Email")
                .Body("<h1>Hello</h1>")
                .BodyType("Html")
                .Build();

            Assert.Equal("Html", email.BodyType);
        }

        [Fact]
        public void Builder_WithMultipleRecipients_Success()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .AddCarbonCopy("cc@example.com")
                .AddCarbonCopy("cc2@example.com")
                .AddBlindCarbonCopy("bcc@example.com")
                .Subject("Test")
                .Body("Body")
                .Build();

            Assert.Equal(2, email.CarbonCopy.Count);
            Assert.Equal(1, email.BlindCarbonCopy.Count);
            Assert.Contains("cc@example.com", email.CarbonCopy);
        }

        [Fact]
        public void Builder_WithAttachments_Success()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .Subject("With Attachments")
                .Body("See attachments")
                .AddAttachment("document.pdf")
                .AddAttachment("image.jpg")
                .Build();

            Assert.Equal(2, email.Attachments.Count);
            Assert.Contains("document.pdf", email.Attachments);
        }

        [Fact]
        public void Builder_WithCustomHeaders_Success()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .Subject("Custom Headers")
                .Body("Body")
                .AddCustomHeader("X-Custom-Header", "CustomValue")
                .AddCustomHeader("X-Priority", "1")
                .Build();

            Assert.Equal(2, email.CustomHeaders.Count);
            Assert.Equal("CustomValue", email.CustomHeaders["X-Custom-Header"]);
        }

        [Fact]
        public void Builder_WithHighPriority_Success()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .Subject("Urgent")
                .Body("Urgent message")
                .Priority("High")
                .Build();

            Assert.Equal("High", email.Priority);
        }

        [Fact]
        public void Builder_WithReplyTo_Success()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .Subject("Reply Test")
                .Body("Please reply to support")
                .ReplyTo("support@example.com")
                .Build();

            Assert.Equal("support@example.com", email.ReplyTo);
        }

        [Fact]
        public void Builder_WithReadReceipt_Success()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .Subject("Confirm Receipt")
                .Body("Please confirm receipt")
                .RequestReadReceipt()
                .Build();

            Assert.True(email.ReadReceiptRequested);
        }

        [Fact]
        public void Builder_WithScheduling_Success()
        {
            var futureTime = DateTime.UtcNow.AddHours(1);
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .Subject("Scheduled Email")
                .Body("Send later")
                .ScheduleFor(futureTime)
                .Build();

            Assert.Equal(futureTime, email.ScheduledFor);
        }

        [Fact]
        public void Builder_ComplexEmail_Success()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("main@example.com")
                .Subject("Complex Email")
                .Body("<p>Complex HTML email</p>")
                .BodyType("Html")
                .AddCarbonCopy("cc@example.com")
                .AddBlindCarbonCopy("bcc@example.com")
                .AddAttachment("file1.pdf")
                .AddAttachment("file2.docx")
                .AddCustomHeader("X-Important", "true")
                .Priority("High")
                .ReplyTo("reply@example.com")
                .RequestReadReceipt()
                .Build();

            Assert.Equal("sender@example.com", email.From);
            Assert.Equal("main@example.com", email.To);
            Assert.Equal("Html", email.BodyType);
            Assert.Single(email.CarbonCopy);
            Assert.Single(email.BlindCarbonCopy);
            Assert.Equal(2, email.Attachments.Count);
            Assert.True(email.ReadReceiptRequested);
            Assert.Equal("High", email.Priority);
        }

        [Fact]
        public void Builder_MissingFrom_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                EmailMessage.Builder
                    .To("recipient@example.com")
                    .Subject("Test")
                    .Body("Body")
                    .Build()
            );

            Assert.Contains("From is required", exception.Message);
        }

        [Fact]
        public void Builder_MissingTo_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                EmailMessage.Builder
                    .From("sender@example.com")
                    .Subject("Test")
                    .Body("Body")
                    .Build()
            );

            Assert.Contains("To is required", exception.Message);
        }

        [Fact]
        public void Builder_MissingSubject_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                EmailMessage.Builder
                    .From("sender@example.com")
                    .To("recipient@example.com")
                    .Body("Body")
                    .Build()
            );

            Assert.Contains("Subject is required", exception.Message);
        }

        [Fact]
        public void Builder_MissingBody_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                EmailMessage.Builder
                    .From("sender@example.com")
                    .To("recipient@example.com")
                    .Subject("Test")
                    .Build()
            );

            Assert.Contains("Body is required", exception.Message);
        }

        [Fact]
        public void Builder_InvalidBodyType_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                EmailMessage.Builder
                    .From("sender@example.com")
                    .To("recipient@example.com")
                    .Subject("Test")
                    .Body("Body")
                    .BodyType("InvalidType")
                    .Build()
            );

            Assert.Contains("BodyType must be", exception.Message);
        }

        [Fact]
        public void Builder_InvalidPriority_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                EmailMessage.Builder
                    .From("sender@example.com")
                    .To("recipient@example.com")
                    .Subject("Test")
                    .Body("Body")
                    .Priority("Urgent")
                    .Build()
            );

            Assert.Contains("Priority must be", exception.Message);
        }

        [Fact]
        public void Builder_ScheduleInPast_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                EmailMessage.Builder
                    .From("sender@example.com")
                    .To("recipient@example.com")
                    .Subject("Test")
                    .Body("Body")
                    .ScheduleFor(DateTime.UtcNow.AddHours(-1))
                    .Build()
            );

            Assert.Contains("future", exception.Message);
        }

        [Fact]
        public void Builder_NullFrom_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                EmailMessage.Builder.From(null)
            );

            Assert.Contains("From cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_EmptyTo_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                EmailMessage.Builder.To("")
            );

            Assert.Contains("To cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_IsImmutable_ReturnsReadOnlyCollections()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .Subject("Immutable Test")
                .Body("Body")
                .AddAttachment("file.pdf")
                .Build();

            Assert.Throws<NotSupportedException>(() =>
            {
                ((System.Collections.Generic.List<string>)email.Attachments).Add("another.pdf");
            });
        }

        [Fact]
        public void Builder_FluentChaining_Success()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .AddCarbonCopy("cc@example.com")
                .AddBlindCarbonCopy("bcc@example.com")
                .Subject("Fluent Test")
                .Body("Fluent chaining")
                .Priority("High")
                .RequestReadReceipt()
                .ReplyTo("reply@example.com")
                .Build();

            Assert.NotNull(email);
            Assert.Equal("sender@example.com", email.From);
        }

        [Fact]
        public void EmailMessage_ToString_ContainsRelevantInfo()
        {
            var email = EmailMessage.Builder
                .From("sender@example.com")
                .To("recipient@example.com")
                .Subject("Test")
                .Body("Body")
                .Priority("High")
                .Build();

            var str = email.ToString();
            Assert.Contains("sender@example.com", str);
            Assert.Contains("High", str);
        }

        [Fact]
        public void Builder_MultipleBuilds_CreateIndependentInstances()
        {
            var builder = EmailMessage.Builder
                .From("sender@example.com")
                .Subject("Test")
                .Body("Body");

            var email1 = builder.To("recipient1@example.com").Build();
            var email2 = builder.To("recipient2@example.com").Build();

            Assert.Equal("recipient2@example.com", email2.To);
        }
    }
}
