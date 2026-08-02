using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSystem.After.Context
{
    public class SystemEvent
    {
        public string EventType { get; set; } = "";
        public string EventData { get; set; } = "";
        public DateTime Timestamp { get; set; }

        public SystemEvent(string type, string data)
        {
            EventType = type;
            EventData = data;
            Timestamp = DateTime.Now;
        }
    }

    public interface IEventHandler
    {
        void Handle(SystemEvent @event);
        string GetHandlerName();
    }

    public class EventBus
    {
        private Dictionary<string, List<IEventHandler>> _handlers = new();

        public void Subscribe(string eventType, IEventHandler handler)
        {
            if (!_handlers.ContainsKey(eventType))
                _handlers[eventType] = new();

            if (!_handlers[eventType].Contains(handler))
            {
                _handlers[eventType].Add(handler);
                Console.WriteLine($"  ✓ {handler.GetHandlerName()} subscribed to {eventType}");
            }
        }

        public void Publish(SystemEvent @event)
        {
            Console.WriteLine($"📢 Event published: {@event.EventType}");
            if (_handlers.TryGetValue(@event.EventType, out var handlers))
            {
                foreach (var handler in handlers.ToList())
                    handler.Handle(@event);
            }
        }

        public int GetSubscriberCount(string eventType) =>
            _handlers.TryGetValue(eventType, out var handlers) ? handlers.Count : 0;
    }

    public class Logger : IEventHandler
    {
        public string LoggerName { get; set; }
        public List<SystemEvent> LoggedEvents { get; set; } = new();

        public Logger(string name)
        {
            LoggerName = name;
        }

        public void Handle(SystemEvent @event)
        {
            LoggedEvents.Add(@event);
            Console.WriteLine($"    📝 {LoggerName} logged: {@event.EventType} - {@event.EventData}");
        }

        public string GetHandlerName() => LoggerName;
    }

    public class EmailNotifier : IEventHandler
    {
        public string NotifierName { get; set; }
        public List<string> EmailsSent { get; set; } = new();

        public EmailNotifier(string name)
        {
            NotifierName = name;
        }

        public void Handle(SystemEvent @event)
        {
            string email = $"Alert: {@event.EventType} - {@event.EventData}";
            EmailsSent.Add(email);
            Console.WriteLine($"    📧 {NotifierName} sent email: {email}");
        }

        public string GetHandlerName() => NotifierName;
    }

    public class MetricsCollector : IEventHandler
    {
        public string CollectorName { get; set; }
        public Dictionary<string, int> EventCounts { get; set; } = new();

        public MetricsCollector(string name)
        {
            CollectorName = name;
        }

        public void Handle(SystemEvent @event)
        {
            if (!EventCounts.ContainsKey(@event.EventType))
                EventCounts[@event.EventType] = 0;
            EventCounts[@event.EventType]++;
            Console.WriteLine($"    📊 {CollectorName} counted {EventCounts[@event.EventType]} {(@event.EventType)} events");
        }

        public string GetHandlerName() => CollectorName;
    }
}
