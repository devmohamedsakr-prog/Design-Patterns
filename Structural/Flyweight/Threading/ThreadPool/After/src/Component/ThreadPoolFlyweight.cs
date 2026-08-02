using System;
using System.Collections.Generic;
using System.Threading;

namespace Flyweight.Threading.ThreadPool.Component
{
    // Extrinsic State: Per-thread task assignment
    public class WorkItem
    {
        public string WorkId { get; set; }
        public Action<object> Task { get; set; }
        public object State { get; set; }
        public DateTime SubmittedTime { get; set; }
        public bool IsCompleted { get; set; }

        public override string ToString() => $"Work[{WorkId}] Completed={IsCompleted}";
    }

    // Intrinsic State: Shared thread infrastructure (thread name, priority, pool metadata)
    public class ThreadMetadata
    {
        public int ThreadId { get; set; }
        public string ThreadName { get; set; }
        public ThreadPriority Priority { get; set; }
        public bool IsRunning { get; set; }

        public override string ToString() => $"Thread[{ThreadId}] {ThreadName}";
    }

    // Flyweight Factory for thread metadata
    public class ThreadMetadataFactory
    {
        private Dictionary<int, ThreadMetadata> _metadataPool = new();

        public ThreadMetadata GetThreadMetadata(int threadId, string poolName, ThreadPriority priority = ThreadPriority.Normal)
        {
            if (!_metadataPool.ContainsKey(threadId))
            {
                _metadataPool[threadId] = new ThreadMetadata
                {
                    ThreadId = threadId,
                    ThreadName = $"{poolName}-Worker-{threadId}",
                    Priority = priority,
                    IsRunning = true
                };
            }

            return _metadataPool[threadId];
        }

        public int GetPoolSize() => _metadataPool.Count;
        public IReadOnlyDictionary<int, ThreadMetadata> GetPool() => _metadataPool;
    }

    // Thread Pool implementation using Flyweight pattern
    public class ThreadPoolExecutor
    {
        private Queue<WorkItem> _workQueue = new();
        private List<ThreadMetadata> _threadPool = new();
        private ThreadMetadataFactory _metadataFactory = new();
        private int _coreThreads;
        private int _workIdCounter = 0;
        private object _lockObj = new();

        public ThreadPoolExecutor(int coreThreads, string poolName)
        {
            _coreThreads = coreThreads;
            
            // Create core threads with shared metadata
            for (int i = 0; i < _coreThreads; i++)
            {
                var metadata = _metadataFactory.GetThreadMetadata(i, poolName);
                _threadPool.Add(metadata);
            }
        }

        public string Submit(Action<object> task, object state = null)
        {
            var workId = $"WORK_{++_workIdCounter}";
            var workItem = new WorkItem
            {
                WorkId = workId,
                Task = task,
                State = state,
                SubmittedTime = DateTime.UtcNow,
                IsCompleted = false
            };

            lock (_lockObj)
            {
                _workQueue.Enqueue(workItem);
            }

            return workId;
        }

        public void Execute()
        {
            lock (_lockObj)
            {
                if (_workQueue.Count > 0)
                {
                    var workItem = _workQueue.Dequeue();
                    workItem.Task?.Invoke(workItem.State);
                    workItem.IsCompleted = true;
                }
            }
        }

        public int GetCoreThreadCount() => _coreThreads;
        public int GetQueuedWorkCount() => _workQueue.Count;
        public int GetThreadMetadataPoolSize() => _metadataFactory.GetPoolSize();
        public long EstimateMemorySavings() => (long)(_coreThreads + _workQueue.Count) * 1000000 - _metadataFactory.GetPoolSize() * 100;
        public IReadOnlyList<ThreadMetadata> GetThreadMetadata() => _threadPool;
    }
}
