using System;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Email.Message.Context
{
    /// <summary>
    /// Product: Immutable email message constructed via builder.
    /// Demonstrates: Fluent builder for complex object with many optional fields.
    /// </summary>
    public class EmailMessage
    {
        public string From { get; }
        public string To { get; }
        public string Subject { get; }
        public string Body { get; }
        public string BodyType { get; } // "Html" or "PlainText"
        public IReadOnlyList<string> CarbonCopy { get; }
        public IReadOnlyList<string> BlindCarbonCopy { get; }
        public IReadOnlyList<string> Attachments { get; }
        public IReadOnlyDictionary<string, string> CustomHeaders { get; }
        public string Priority { get; } // "Low", "Normal", "High"
        public string ReplyTo { get; }
        public bool ReadReceiptRequested { get; }
        public DateTime ScheduledFor { get; }

        // Private constructor - only builder can create instances
        private EmailMessage(
            string from,
            string to,
            string subject,
            string body,
            string bodyType,
            IReadOnlyList<string> cc,
            IReadOnlyList<string> bcc,
            IReadOnlyList<string> attachments,
            IReadOnlyDictionary<string, string> customHeaders,
            string priority,
            string replyTo,
            bool readReceiptRequested,
            DateTime scheduledFor)
        {
            From = from;
            To = to;
            Subject = subject;
            Body = body;
            BodyType = bodyType;
            CarbonCopy = cc;
            BlindCarbonCopy = bcc;
            Attachments = attachments;
            CustomHeaders = customHeaders;
            Priority = priority;
            ReplyTo = replyTo;
            ReadReceiptRequested = readReceiptRequested;
            ScheduledFor = scheduledFor;
        }

        /// <summary>
        /// Static factory to get a new builder instance.
        /// </summary>
        public static EmailBuilder Builder => new EmailBuilder();

        public override string ToString()
        {
            return $"EmailMessage(From={From}, To={To}, Subject={Subject}, Priority={Priority}, " +
                   $"CC={string.Join(", ", CarbonCopy)}, BCC={string.Join(", ", BlindCarbonCopy)}, " +
                   $"Attachments={CarbonCopy.Count}, BodyType={BodyType}, ReadReceipt={ReadReceiptRequested})";
        }

        /// <summary>
        /// Builder class: Fluent API for constructing EmailMessage.
        /// </summary>
        public class EmailBuilder
        {
            private string _from;
            private string _to;
            private string _subject;
            private string _body;
            private string _bodyType = "PlainText";
            private readonly List<string> _cc = new();
            private readonly List<string> _bcc = new();
            private readonly List<string> _attachments = new();
            private readonly Dictionary<string, string> _customHeaders = new();
            private string _priority = "Normal";
            private string _replyTo;
            private bool _readReceiptRequested = false;
            private DateTime _scheduledFor = DateTime.UtcNow;

            /// <summary>
            /// Set sender email address (required).
            /// </summary>
            public EmailBuilder From(string from)
            {
                if (string.IsNullOrWhiteSpace(from))
                    throw new ArgumentException("From cannot be null or empty", nameof(from));
                _from = from;
                return this;
            }

            /// <summary>
            /// Set recipient email address (required).
            /// </summary>
            public EmailBuilder To(string to)
            {
                if (string.IsNullOrWhiteSpace(to))
                    throw new ArgumentException("To cannot be null or empty", nameof(to));
                _to = to;
                return this;
            }

            /// <summary>
            /// Set email subject (required).
            /// </summary>
            public EmailBuilder Subject(string subject)
            {
                if (string.IsNullOrWhiteSpace(subject))
                    throw new ArgumentException("Subject cannot be null or empty", nameof(subject));
                _subject = subject;
                return this;
            }

            /// <summary>
            /// Set email body content (required).
            /// </summary>
            public EmailBuilder Body(string body)
            {
                if (string.IsNullOrWhiteSpace(body))
                    throw new ArgumentException("Body cannot be null or empty", nameof(body));
                _body = body;
                return this;
            }

            /// <summary>
            /// Set body type: "Html" or "PlainText".
            /// </summary>
            public EmailBuilder BodyType(string bodyType)
            {
                if (!new[] { "Html", "PlainText" }.Contains(bodyType))
                    throw new ArgumentException("BodyType must be 'Html' or 'PlainText'", nameof(bodyType));
                _bodyType = bodyType;
                return this;
            }

            /// <summary>
            /// Add carbon copy recipient.
            /// </summary>
            public EmailBuilder AddCarbonCopy(string email)
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be null or empty", nameof(email));
                _cc.Add(email);
                return this;
            }

            /// <summary>
            /// Add blind carbon copy recipient.
            /// </summary>
            public EmailBuilder AddBlindCarbonCopy(string email)
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be null or empty", nameof(email));
                _bcc.Add(email);
                return this;
            }

            /// <summary>
            /// Add attachment file path.
            /// </summary>
            public EmailBuilder AddAttachment(string filePath)
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("FilePath cannot be null or empty", nameof(filePath));
                _attachments.Add(filePath);
                return this;
            }

            /// <summary>
            /// Add custom header.
            /// </summary>
            public EmailBuilder AddCustomHeader(string key, string value)
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Key and value cannot be null or empty");
                _customHeaders[key] = value;
                return this;
            }

            /// <summary>
            /// Set priority: "Low", "Normal", or "High".
            /// </summary>
            public EmailBuilder Priority(string priority)
            {
                if (!new[] { "Low", "Normal", "High" }.Contains(priority))
                    throw new ArgumentException("Priority must be 'Low', 'Normal', or 'High'", nameof(priority));
                _priority = priority;
                return this;
            }

            /// <summary>
            /// Set reply-to address.
            /// </summary>
            public EmailBuilder ReplyTo(string replyTo)
            {
                if (string.IsNullOrWhiteSpace(replyTo))
                    throw new ArgumentException("ReplyTo cannot be null or empty", nameof(replyTo));
                _replyTo = replyTo;
                return this;
            }

            /// <summary>
            /// Request read receipt.
            /// </summary>
            public EmailBuilder RequestReadReceipt()
            {
                _readReceiptRequested = true;
                return this;
            }

            /// <summary>
            /// Schedule email for future delivery.
            /// </summary>
            public EmailBuilder ScheduleFor(DateTime deliveryTime)
            {
                if (deliveryTime <= DateTime.UtcNow)
                    throw new ArgumentException("Delivery time must be in the future", nameof(deliveryTime));
                _scheduledFor = deliveryTime;
                return this;
            }

            /// <summary>
            /// Build the immutable EmailMessage.
            /// </summary>
            public EmailMessage Build()
            {
                if (string.IsNullOrWhiteSpace(_from))
                    throw new InvalidOperationException("From is required");
                if (string.IsNullOrWhiteSpace(_to))
                    throw new InvalidOperationException("To is required");
                if (string.IsNullOrWhiteSpace(_subject))
                    throw new InvalidOperationException("Subject is required");
                if (string.IsNullOrWhiteSpace(_body))
                    throw new InvalidOperationException("Body is required");

                return new EmailMessage(
                    _from,
                    _to,
                    _subject,
                    _body,
                    _bodyType,
                    _cc.AsReadOnly(),
                    _bcc.AsReadOnly(),
                    _attachments.AsReadOnly(),
                    new Dictionary<string, string>(_customHeaders),
                    _priority,
                    _replyTo,
                    _readReceiptRequested,
                    _scheduledFor
                );
            }
        }
    }
}
