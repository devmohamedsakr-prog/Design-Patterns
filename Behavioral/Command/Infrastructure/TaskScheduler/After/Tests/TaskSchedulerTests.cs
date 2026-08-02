using NUnit.Framework;
using TaskScheduler.After.Context;

namespace TaskScheduler.After.Tests
{
    [TestFixture]
    public class TaskSchedulerTests
    {
        private Scheduler _scheduler;
        private bool _jobExecuted;

        [SetUp]
        public void Setup()
        {
            _scheduler = new Scheduler();
            _jobExecuted = false;
        }

        [Test] 
        public void QueueJob_Succeeds() 
        {
            _scheduler.QueueJob(new ExecuteJobCommand("test", () => _jobExecuted = true));
            Assert.That(_scheduler.GetQueuedCount(), Is.EqualTo(1));
        }

        [Test] 
        public void ExecuteNextJob_Succeeds() 
        {
            _scheduler.QueueJob(new ExecuteJobCommand("test", () => _jobExecuted = true));
            Assert.That(_scheduler.ExecuteNextJob(), Is.True);
            Assert.That(_jobExecuted, Is.True);
        }

        [Test] 
        public void ExecuteNextJob_UpdatesStats()
        {
            _scheduler.QueueJob(new ExecuteJobCommand("test", () => { }));
            _scheduler.ExecuteNextJob();
            Assert.That(_scheduler.GetCompletedCount(), Is.EqualTo(1));
            Assert.That(_scheduler.GetQueuedCount(), Is.EqualTo(0));
        }

        [Test] 
        public void ExecuteNextJob_Empty_Fails() 
        {
            Assert.That(_scheduler.ExecuteNextJob(), Is.False);
        }

        [Test] 
        public void ExecuteAllJobs()
        {
            int count = 0;
            _scheduler.QueueJob(new ExecuteJobCommand("job1", () => count++));
            _scheduler.QueueJob(new ExecuteJobCommand("job2", () => count++));
            _scheduler.QueueJob(new ExecuteJobCommand("job3", () => count++));
            
            _scheduler.ExecuteAllJobs();
            Assert.That(count, Is.EqualTo(3));
            Assert.That(_scheduler.GetCompletedCount(), Is.EqualTo(3));
        }

        [Test] 
        public void FailedJob_Tracked()
        {
            _scheduler.QueueJob(new ExecuteJobCommand("fail", () => throw new Exception("test")));
            _scheduler.ExecuteNextJob();
            Assert.That(_scheduler.GetFailedCount(), Is.EqualTo(1));
        }

        [Test] 
        public void MultipleJobs_Queue()
        {
            _scheduler.QueueJob(new ExecuteJobCommand("j1", () => { }));
            _scheduler.QueueJob(new ExecuteJobCommand("j2", () => { }));
            _scheduler.QueueJob(new ExecuteJobCommand("j3", () => { }));
            Assert.That(_scheduler.GetQueuedCount(), Is.EqualTo(3));
        }

        [Test] 
        public void RetryableJob_Succeeds()
        {
            int attempts = 0;
            _scheduler.QueueJob(new RetryableJob("retry", () => 
            {
                attempts++;
                if (attempts < 2) throw new Exception("First attempt fails");
            }, 3));
            
            _scheduler.ExecuteNextJob();
            Assert.That(_scheduler.GetCompletedCount(), Is.EqualTo(1));
        }

        [Test] 
        public void ScheduledJob_NotReady()
        {
            _scheduler.QueueJob(new ScheduledJob("future", () => { }, DateTime.UtcNow.AddHours(1)));
            Assert.That(_scheduler.ExecuteNextJob(), Is.False);
        }

        [Test] 
        public void ScheduledJob_Ready()
        {
            _scheduler.QueueJob(new ScheduledJob("past", () => _jobExecuted = true, DateTime.UtcNow.AddSeconds(-1)));
            Assert.That(_scheduler.ExecuteNextJob(), Is.True);
            Assert.That(_jobExecuted, Is.True);
        }

        [Test] 
        public void QueueOrder_FIFO()
        {
            var order = new List<int>();
            _scheduler.QueueJob(new ExecuteJobCommand("1", () => order.Add(1)));
            _scheduler.QueueJob(new ExecuteJobCommand("2", () => order.Add(2)));
            _scheduler.QueueJob(new ExecuteJobCommand("3", () => order.Add(3)));
            
            _scheduler.ExecuteAllJobs();
            Assert.That(order, Is.EqualTo(new[] { 1, 2, 3 }));
        }
    }
}
