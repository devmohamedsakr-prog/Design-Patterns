using Xunit;
using Proxy.LogAggregation.Observability.Component;

namespace Proxy.LogAggregation.Observability.Tests
{
    public class LogAggregationProxyTests
    {
        [Fact]
        public void LogAggregationProxy_ShouldBuffer Logs()
        {
            var real = new CentralLogService();
            var proxy = new LogAggregationProxy(real, 10);
            
            proxy.SendLog("Log 1");
            
            Assert.Equal(1, proxy.GetBufferedCount());
        }

        [Fact]
        public void LogAggregationProxy_ShouldFlushWhenBatchFull()
        {
            var real = new CentralLogService();
            var proxy = new LogAggregationProxy(real, 3);
            
            proxy.SendLog("Log 1");
            proxy.SendLog("Log 2");
            proxy.SendLog("Log 3");
            
            Assert.Equal(0, proxy.GetBufferedCount()); // Flushed
            Assert.Equal(3, real.GetLogCount());
        }

        [Fact]
        public void LogAggregationProxy_ShouldNotFlushPrematurely()
        {
            var real = new CentralLogService();
            var proxy = new LogAggregationProxy(real, 5);
            
            proxy.SendLog("Log 1");
            proxy.SendLog("Log 2");
            
            Assert.Equal(2, proxy.GetBufferedCount());
            Assert.Equal(0, real.GetLogCount());
        }

        [Fact]
        public void LogAggregationProxy_ShouldAllowManualFlush()
        {
            var real = new CentralLogService();
            var proxy = new LogAggregationProxy(real, 10);
            
            proxy.SendLog("Log 1");
            proxy.SendLog("Log 2");
            
            proxy.FlushNow();
            
            Assert.Equal(0, proxy.GetBufferedCount());
            Assert.Equal(2, real.GetLogCount());
        }

        [Fact]
        public void LogAggregationProxy_ShouldBatchMultipleLogs()
        {
            var real = new CentralLogService();
            var proxy = new LogAggregationProxy(real, 5);
            
            for (int i = 0; i < 15; i++)
            {
                proxy.SendLog($"Log {i}");
            }
            
            proxy.FlushNow();
            Assert.True(real.GetLogCount() >= 15);
        }

        [Fact]
        public void LogAggregationProxy_ShouldReduceNetworkCalls()
        {
            var real = new CentralLogService();
            var proxy = new LogAggregationProxy(real, 10);
            
            // 100 logs with batching of 10 = 10 network calls instead of 100
            for (int i = 0; i < 100; i++)
            {
                proxy.SendLog($"Log {i}");
            }
            
            // Most should be batched
            Assert.True(proxy.GetBufferedCount() < 10);
        }

        [Fact]
        public void CentralLogService_ShouldReceiveLogs()
        {
            var service = new CentralLogService();
            
            service.SendLog("Test message");
            
            Assert.Equal(1, service.GetLogCount());
        }

        [Fact]
        public void LogAggregationProxy_ShouldTrackTotalLogs()
        {
            var real = new CentralLogService();
            var proxy = new LogAggregationProxy(real, 10);
            
            proxy.SendLog("Log 1");
            proxy.SendLog("Log 2");
            proxy.SendLog("Log 3");
            
            var total = proxy.GetLogCount();
            Assert.Equal(3, total);
        }

        [Fact]
        public void LogAggregationProxy_ShouldHandleConsecutiveBatches()
        {
            var real = new CentralLogService();
            var proxy = new LogAggregationProxy(real, 5);
            
            // First batch
            for (int i = 0; i < 5; i++)
                proxy.SendLog($"Batch1-Log{i}");
            
            // Second batch
            for (int i = 0; i < 5; i++)
                proxy.SendLog($"Batch2-Log{i}");
            
            proxy.FlushNow();
            Assert.True(real.GetLogCount() >= 10);
        }

        [Fact]
        public void LogAggregationProxy_ShouldAllowCustomBatchSize()
        {
            var real = new CentralLogService();
            var proxy = new LogAggregationProxy(real, 20);
            
            for (int i = 0; i < 19; i++)
                proxy.SendLog($"Log {i}");
            
            // Should not flush yet
            Assert.Equal(19, proxy.GetBufferedCount());
        }
    }
}
