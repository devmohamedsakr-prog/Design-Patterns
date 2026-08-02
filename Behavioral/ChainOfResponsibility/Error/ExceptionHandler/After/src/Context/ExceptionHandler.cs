using System;

namespace ExceptionHandler.After.Context
{
    public class ApplicationException : Exception
    {
        public string ErrorCode { get; set; } = "";
        public int Severity { get; set; } = 1; // 1=Low, 2=Medium, 3=High, 4=Critical
        public bool Handled { get; set; } = false;

        public ApplicationException(string message, string code, int severity) 
            : base(message)
        {
            ErrorCode = code;
            Severity = severity;
        }
    }

    public abstract class ExceptionHandler
    {
        protected ExceptionHandler _nextHandler;

        public void SetNext(ExceptionHandler next) => _nextHandler = next;

        public virtual void Handle(ApplicationException exception)
        {
            if (CanHandle(exception))
            {
                Process(exception);
            }
            else if (_nextHandler != null)
            {
                Console.WriteLine($"→ Passing to {_nextHandler.GetType().Name}");
                _nextHandler.Handle(exception);
            }
            else
            {
                Console.WriteLine($"⚠️ Exception not handled");
            }
        }

        protected abstract bool CanHandle(ApplicationException exception);
        protected abstract void Process(ApplicationException exception);
    }

    public class LoggingHandler : ExceptionHandler
    {
        protected override bool CanHandle(ApplicationException exception) => true;

        protected override void Process(ApplicationException exception)
        {
            Console.WriteLine($"📝 LoggingHandler: Logging error {exception.ErrorCode}");
            Console.WriteLine($"   Message: {exception.Message}");
            
            if (_nextHandler != null)
                _nextHandler.Handle(exception);
        }
    }

    public class AlertingHandler : ExceptionHandler
    {
        protected override bool CanHandle(ApplicationException exception) 
            => exception.Severity >= 3;

        protected override void Process(ApplicationException exception)
        {
            Console.WriteLine($"🚨 AlertingHandler: Sending alert for {exception.ErrorCode}");
            exception.Handled = true;
            
            if (_nextHandler != null)
                _nextHandler.Handle(exception);
        }
    }

    public class RecoveryHandler : ExceptionHandler
    {
        protected override bool CanHandle(ApplicationException exception) 
            => exception.ErrorCode == "DB_TIMEOUT" || exception.ErrorCode == "SERVICE_UNAVAILABLE";

        protected override void Process(ApplicationException exception)
        {
            Console.WriteLine($"🔧 RecoveryHandler: Attempting recovery for {exception.ErrorCode}");
            exception.Handled = true;
            
            if (_nextHandler != null)
                _nextHandler.Handle(exception);
        }
    }

    public class NotificationHandler : ExceptionHandler
    {
        protected override bool CanHandle(ApplicationException exception) 
            => exception.Severity >= 2;

        protected override void Process(ApplicationException exception)
        {
            Console.WriteLine($"📧 NotificationHandler: Notifying admin about {exception.ErrorCode}");
            
            if (_nextHandler != null)
                _nextHandler.Handle(exception);
        }
    }

    public class FallbackHandler : ExceptionHandler
    {
        protected override bool CanHandle(ApplicationException exception) => true;

        protected override void Process(ApplicationException exception)
        {
            Console.WriteLine($"🛡️ FallbackHandler: Providing fallback response");
            exception.Handled = true;
        }
    }

    public class ExceptionHandlingChain
    {
        private ExceptionHandler _firstHandler;

        public ExceptionHandlingChain()
        {
            var logging = new LoggingHandler();
            var alerting = new AlertingHandler();
            var recovery = new RecoveryHandler();
            var notification = new NotificationHandler();
            var fallback = new FallbackHandler();

            logging.SetNext(alerting);
            alerting.SetNext(recovery);
            recovery.SetNext(notification);
            notification.SetNext(fallback);

            _firstHandler = logging;
        }

        public void ProcessException(ApplicationException exception)
        {
            Console.WriteLine($"\n⚡ Exception occurred: {exception.ErrorCode}");
            _firstHandler.Handle(exception);
            Console.WriteLine($"   Handled: {exception.Handled}\n");
        }
    }
}
