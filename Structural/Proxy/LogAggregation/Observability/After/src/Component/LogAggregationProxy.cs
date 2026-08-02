using System;
using System.Collections.Generic;

namespace Proxy.LogAggregation.Observability.Component
{
    // Subject: Central logging service
    public interface ILogService
    {
        void SendLog(string message);
        int GetLogCount();
    }

    // Real Subject: Remote log service
    public class CentralLogService : ILogService
    {
        private List<string> _logs = new();

        public void SendLog(string message)
        {
            // Simulate network latency
            System.Threading.Thread.Sleep(20);
            _logs.Add($"[{DateTime.UtcNow:O}] {message}");
            Console.WriteLine($"  [CentralLog] Received log: {message}");
        }

        public int GetLogCount() => _logs.Count;
    }

    // Proxy: Batches logs for efficiency
    public class LogAggregationProxy : ILogService
    {
        private ILogService _realService;
        private List<string> _localBuffer = new();
        private int _batchSize = 10;
        private DateTime _lastFlush = DateTime.UtcNow;
        private int _flushIntervalMs = 5000;

        public LogAggregationProxy(ILogService realService, int batchSize = 10)
        {
            _realService = realService;
            _batchSize = batchSize;
            Console.WriteLine($"✓ [LogProxy] Initialized with batch size: {_batchSize}");
        }

        public void SendLog(string message)
        {
            _localBuffer.Add(message);
            Console.WriteLine($"✓ [LogProxy] Buffered log ({_localBuffer.Count}/{_batchSize}): {message}");

            // Flush if batch full
            if (_localBuffer.Count >= _batchSize)
            {
                Flush();
            }
            // Or flush if timeout exceeded
            else if ((DateTime.UtcNow - _lastFlush).TotalMilliseconds > _flushIntervalMs)
            {
                Flush();
            }
        }

        private void Flush()
        {
            if (_localBuffer.Count == 0) return;

            Console.WriteLine($"✓ [LogProxy] Flushing {_localBuffer.Count} logs to remote service");
            foreach (var log in _localBuffer)
            {
                _realService.SendLog(log);
            }
            _localBuffer.Clear();
            _lastFlush = DateTime.UtcNow;
        }

        public int GetLogCount()
        {
            // Return total (remote + buffered)
            return _realService.GetLogCount() + _localBuffer.Count;
        }

        public int GetBufferedCount() => _localBuffer.Count;
        public void FlushNow() => Flush();
    }
}
