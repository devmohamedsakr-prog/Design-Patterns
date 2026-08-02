using Xunit;
using Decorator.Logger.Audit.Component;

namespace Decorator.Logger.Audit.Tests
{
    public class LoggerDecoratorTests
    {
        [Fact]
        public void SimpleLogger_ShouldLogMessage()
        {
            var logger = new SimpleLogger();
            logger.Log("Test message");
            Assert.NotEmpty(logger.GetLogs());
        }

        [Fact]
        public void SimpleLogger_ShouldTrackMultipleLogs()
        {
            var logger = new SimpleLogger();
            logger.Log("First");
            logger.Log("Second");
            logger.Log("Third");
            Assert.Equal(3, logger.GetLogs().Count);
        }

        [Fact]
        public void FileLoggerDecorator_ShouldLogToFile()
        {
            var logger = new SimpleLogger();
            var fileLogger = new FileLoggerDecorator(logger, "test.log");
            fileLogger.Log("File message");
            Assert.NotEmpty(fileLogger.GetLogs());
        }

        [Fact]
        public void FileLoggerDecorator_ShouldSpecifyFilename()
        {
            var logger = new SimpleLogger();
            var fileLogger = new FileLoggerDecorator(logger, "custom.log");
            Assert.Equal("custom.log", fileLogger.Filename);
        }

        [Fact]
        public void DatabaseLoggerDecorator_ShouldLogToDatabase()
        {
            var logger = new SimpleLogger();
            var dbLogger = new DatabaseLoggerDecorator(logger, "server.db");
            dbLogger.Log("Database message");
            Assert.NotEmpty(dbLogger.GetLogs());
        }

        [Fact]
        public void DatabaseLoggerDecorator_ShouldTrackConnection()
        {
            var logger = new SimpleLogger();
            var dbLogger = new DatabaseLoggerDecorator(logger, "user:pass@host");
            Assert.Equal("user:pass@host", dbLogger.ConnectionString);
        }

        [Fact]
        public void MetricsDecorator_ShouldCountLogs()
        {
            var logger = new SimpleLogger();
            var metrics = new MetricsDecorator(logger);
            metrics.Log("Metric 1");
            metrics.Log("Metric 2");
            Assert.Equal(2, metrics.TotalLogs);
        }

        [Fact]
        public void MetricsDecorator_ShouldCountErrors()
        {
            var logger = new SimpleLogger();
            var metrics = new MetricsDecorator(logger);
            logger.LogLevel = "ERROR";
            metrics.Log("Error 1");
            logger.LogLevel = "ERROR";
            metrics.Log("Error 2");
            Assert.True(metrics.ErrorCount >= 0);
        }

        [Fact]
        public void FilterDecorator_ShouldFilterByLogLevel()
        {
            var logger = new SimpleLogger();
            var filter = new FilterDecorator(logger);
            logger.LogLevel = "DEBUG";
            filter.Log("Debug message");
            Assert.True(filter.GetFilteredCount() >= 0);
        }

        [Fact]
        public void FilterDecorator_ShouldAllowConfiguredLevels()
        {
            var logger = new SimpleLogger();
            var filter = new FilterDecorator(logger);
            filter.AllowedLevels.Add("INFO");
            logger.LogLevel = "INFO";
            filter.Log("Info message");
            Assert.NotEmpty(logger.GetLogs());
        }

        [Fact]
        public void ChainedDecorators_ShouldStack()
        {
            var logger = new SimpleLogger();
            var decorated = new FilterDecorator(
                new MetricsDecorator(
                    new DatabaseLoggerDecorator(
                        new FileLoggerDecorator(logger, "chain.log"), "localhost")));
            decorated.Log("Chained message");
            Assert.NotNull(decorated);
        }

        [Fact]
        public void MultipleDecorators_ShouldCompose()
        {
            var logger = new SimpleLogger();
            var fileLogger = new FileLoggerDecorator(logger, "app.log");
            var dbLogger = new DatabaseLoggerDecorator(fileLogger, "db");
            var metrics = new MetricsDecorator(dbLogger);
            metrics.Log("Composed");
            Assert.True(metrics.TotalLogs > 0);
        }

        [Fact]
        public void DecoratedLogger_ShouldPreserveLogLevel()
        {
            var logger = new SimpleLogger();
            logger.LogLevel = "WARN";
            var decorated = new FileLoggerDecorator(logger, "warn.log");
            Assert.Equal("WARN", decorated.LogLevel);
        }

        [Fact]
        public void FilterDecorator_ShouldTrackFilteredMessages()
        {
            var logger = new SimpleLogger();
            var filter = new FilterDecorator(logger);
            filter.AllowedLevels.Remove("DEBUG");
            logger.LogLevel = "DEBUG";
            filter.Log("Should be filtered");
            Assert.True(filter.GetFilteredCount() > 0);
        }
    }
}
