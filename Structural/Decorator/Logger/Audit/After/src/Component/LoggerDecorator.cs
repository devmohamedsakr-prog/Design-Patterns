using System;
using System.Collections.Generic;

namespace Decorator.Logger.Audit.Component
{
    public abstract class Logger
    {
        public string LogLevel { get; set; }

        public Logger()
        {
            LogLevel = "INFO";
        }

        public abstract void Log(string message);
        public abstract IReadOnlyList<string> GetLogs();
    }

    public class SimpleLogger : Logger
    {
        protected List<string> _logs;

        public SimpleLogger()
        {
            _logs = new List<string>();
        }

        public override void Log(string message)
        {
            var logEntry = $"[{LogLevel}] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}";
            _logs.Add(logEntry);
            Console.WriteLine(logEntry);
        }

        public override IReadOnlyList<string> GetLogs() => _logs.AsReadOnly();
        public override string ToString() => $"SimpleLogger({_logs.Count} entries)";
    }

    public abstract class LoggerDecorator : Logger
    {
        protected Logger _logger;

        public LoggerDecorator(Logger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            LogLevel = logger.LogLevel;
        }
    }

    public class FileLoggerDecorator : LoggerDecorator
    {
        public string Filename { get; set; }
        private List<string> _fileBuffer;

        public FileLoggerDecorator(Logger logger, string filename = "app.log") : base(logger)
        {
            Filename = filename;
            _fileBuffer = new List<string>();
        }

        public override void Log(string message)
        {
            _logger.Log(message);
            var fileEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {message}";
            _fileBuffer.Add(fileEntry);
            Console.WriteLine($"  → Written to {Filename}");
        }

        public override IReadOnlyList<string> GetLogs() => _fileBuffer.AsReadOnly();
        public override string ToString() => $"FileLoggerDecorator({_logger}, file={Filename})";
    }

    public class DatabaseLoggerDecorator : LoggerDecorator
    {
        public string ConnectionString { get; set; }
        private List<string> _dbBuffer;

        public DatabaseLoggerDecorator(Logger logger, string connStr = "localhost") : base(logger)
        {
            ConnectionString = connStr;
            _dbBuffer = new List<string>();
        }

        public override void Log(string message)
        {
            _logger.Log(message);
            var dbEntry = $"INSERT INTO logs (level, timestamp, message) VALUES ('{LogLevel}', '{DateTime.UtcNow:O}', '{message}')";
            _dbBuffer.Add(dbEntry);
            Console.WriteLine($"  → Stored in DB: {ConnectionString}");
        }

        public override IReadOnlyList<string> GetLogs() => _dbBuffer.AsReadOnly();
        public override string ToString() => $"DatabaseLoggerDecorator({_logger}, db={ConnectionString})";
    }

    public class MetricsDecorator : LoggerDecorator
    {
        public int TotalLogs { get; set; }
        public int ErrorCount { get; set; }
        public int WarningCount { get; set; }
        private DateTime _startTime;

        public MetricsDecorator(Logger logger) : base(logger)
        {
            TotalLogs = 0;
            ErrorCount = 0;
            WarningCount = 0;
            _startTime = DateTime.UtcNow;
        }

        public override void Log(string message)
        {
            _logger.Log(message);
            TotalLogs++;
            if (LogLevel == "ERROR") ErrorCount++;
            if (LogLevel == "WARN") WarningCount++;
            Console.WriteLine($"  → Metrics: Total={TotalLogs}, Errors={ErrorCount}, Warnings={WarningCount}");
        }

        public override IReadOnlyList<string> GetLogs() => _logger.GetLogs();

        public double GetLogsPerSecond()
        {
            var elapsed = (DateTime.UtcNow - _startTime).TotalSeconds;
            return elapsed > 0 ? TotalLogs / elapsed : 0;
        }

        public override string ToString() => $"MetricsDecorator({_logger}, logs={TotalLogs})";
    }

    public class FilterDecorator : LoggerDecorator
    {
        public HashSet<string> AllowedLevels { get; set; }
        private int _filteredCount;

        public FilterDecorator(Logger logger) : base(logger)
        {
            AllowedLevels = new HashSet<string> { "ERROR", "WARN", "INFO" };
            _filteredCount = 0;
        }

        public override void Log(string message)
        {
            if (AllowedLevels.Contains(LogLevel))
            {
                _logger.Log(message);
            }
            else
            {
                _filteredCount++;
                Console.WriteLine($"  [FILTERED] {LogLevel} message not in allowed levels");
            }
        }

        public override IReadOnlyList<string> GetLogs() => _logger.GetLogs();
        public int GetFilteredCount() => _filteredCount;
        public override string ToString() => $"FilterDecorator({_logger}, allowed={string.Join(",", AllowedLevels)})";
    }
}
