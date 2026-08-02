using NUnit.Framework;
using EventSystem.After.Context;

namespace EventSystem.After.Tests
{
    [TestFixture]
    public class EventSystemTests
    {
        private EventBus _bus;
        private Logger _logger;
        private EmailNotifier _notifier;
        private MetricsCollector _metrics;

        [SetUp]
        public void Setup()
        {
            _bus = new EventBus();
            _logger = new Logger("Log");
            _notifier = new EmailNotifier("Email");
            _metrics = new MetricsCollector("Metrics");
        }

        [Test]
        public void Subscribe_Handler() { _bus.Subscribe("UserCreated", _logger); Assert.That(_bus.GetSubscriberCount("UserCreated"), Is.EqualTo(1)); }

        [Test]
        public void Publish_Event() { _bus.Subscribe("UserCreated", _logger); _bus.Publish(new SystemEvent("UserCreated", "User123")); Assert.That(_logger.LoggedEvents.Count, Is.EqualTo(1)); }

        [Test]
        public void MultipleHandlers() { _bus.Subscribe("UserCreated", _logger); _bus.Subscribe("UserCreated", _notifier); _bus.Subscribe("UserCreated", _metrics); _bus.Publish(new SystemEvent("UserCreated", "User123")); Assert.That(_logger.LoggedEvents.Count, Is.EqualTo(1)); Assert.That(_notifier.EmailsSent.Count, Is.EqualTo(1)); }

        [Test]
        public void EventMetrics() { _bus.Subscribe("Error", _metrics); _bus.Publish(new SystemEvent("Error", "E1")); _bus.Publish(new SystemEvent("Error", "E2")); Assert.That(_metrics.EventCounts["Error"], Is.EqualTo(2)); }
    }
}
