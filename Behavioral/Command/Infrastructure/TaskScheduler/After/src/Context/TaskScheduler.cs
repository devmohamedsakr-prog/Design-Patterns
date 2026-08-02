using System;
using System.Collections.Generic;

namespace TaskScheduler.After.Context
{
    public interface IJob
    {
        bool Execute();
        bool Cancel();
        string GetDescription();
    }

    public class Scheduler
    {
        private Queue<IJob> _jobQueue = new();
        private List<IJob> _completedJobs = new();
        private List<IJob> _failedJobs = new();

        public void QueueJob(IJob job)
        {
            _jobQueue.Enqueue(job);
            Console.WriteLine($"✓ Job queued: {job.GetDescription()}");
        }

        public bool ExecuteNextJob()
        {
            if (_jobQueue.Count == 0) return false;
            
            var job = _jobQueue.Dequeue();
            if (job.Execute())
            {
                _completedJobs.Add(job);
                Console.WriteLine($"✓ Job completed: {job.GetDescription()}");
                return true;
            }
            else
            {
                _failedJobs.Add(job);
                Console.WriteLine($"✗ Job failed: {job.GetDescription()}");
                return false;
            }
        }

        public void ExecuteAllJobs()
        {
            while (_jobQueue.Count > 0)
            {
                ExecuteNextJob();
            }
        }

        public int GetQueuedCount() => _jobQueue.Count;
        public int GetCompletedCount() => _completedJobs.Count;
        public int GetFailedCount() => _failedJobs.Count;
    }

    public class ExecuteJobCommand : IJob
    {
        private Action _action;
        private string _name;

        public ExecuteJobCommand(string name, Action action)
        {
            _name = name;
            _action = action;
        }

        public bool Execute()
        {
            try
            {
                _action.Invoke();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Cancel() => true;

        public string GetDescription() => $"Execute {_name}";
    }

    public class RetryableJob : IJob
    {
        private Action _action;
        private string _name;
        private int _maxRetries;
        private int _retries = 0;

        public RetryableJob(string name, Action action, int maxRetries = 3)
        {
            _name = name;
            _action = action;
            _maxRetries = maxRetries;
        }

        public bool Execute()
        {
            try
            {
                _action.Invoke();
                return true;
            }
            catch
            {
                _retries++;
                return _retries <= _maxRetries ? Execute() : false;
            }
        }

        public bool Cancel() => true;

        public string GetDescription() => $"Retry {_name} (attempt {_retries})";
    }

    public class ScheduledJob : IJob
    {
        private Action _action;
        private string _name;
        private DateTime _scheduledTime;

        public ScheduledJob(string name, Action action, DateTime scheduledTime)
        {
            _name = name;
            _action = action;
            _scheduledTime = scheduledTime;
        }

        public bool Execute()
        {
            if (DateTime.UtcNow < _scheduledTime) return false;
            try
            {
                _action.Invoke();
                return true;
            }
            catch { return false; }
        }

        public bool Cancel() => true;

        public string GetDescription() => $"Scheduled {_name} at {_scheduledTime:yyyy-MM-dd HH:mm:ss}";
    }
}
