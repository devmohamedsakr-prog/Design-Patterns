using Xunit;
using Flyweight.Threading.ThreadPool.Component;
using System.Threading;

namespace Flyweight.Threading.ThreadPool.Tests
{
    public class ThreadPoolFlyweightTests
    {
        [Fact]
        public void ThreadPoolExecutor_ShouldCreateCoreThreads()
        {
            var executor = new ThreadPoolExecutor(5, "MyPool");
            Assert.Equal(5, executor.GetCoreThreadCount());
        }

        [Fact]
        public void ThreadMetadataFactory_ShouldReuseMet adata()
        {
            var factory = new ThreadMetadataFactory();
            var meta1 = factory.GetThreadMetadata(0, "Pool", ThreadPriority.Normal);
            var meta2 = factory.GetThreadMetadata(0, "Pool", ThreadPriority.Normal);
            
            Assert.Same(meta1, meta2);
        }

        [Fact]
        public void ThreadMetadata_ShouldHaveCorrectProperties()
        {
            var factory = new ThreadMetadataFactory();
            var meta = factory.GetThreadMetadata(1, "MyPool", ThreadPriority.AboveNormal);
            
            Assert.Equal(1, meta.ThreadId);
            Assert.Contains("MyPool", meta.ThreadName);
            Assert.Equal(ThreadPriority.AboveNormal, meta.Priority);
        }

        [Fact]
        public void ThreadPoolExecutor_ShouldSubmitWork()
        {
            var executor = new ThreadPoolExecutor(5, "Pool");
            var workId = executor.Submit(() => { }, null);
            
            Assert.NotNull(workId);
            Assert.StartsWith("WORK_", workId);
        }

        [Fact]
        public void ThreadPoolExecutor_ShouldQueueWork()
        {
            var executor = new ThreadPoolExecutor(5, "Pool");
            executor.Submit(() => { }, null);
            executor.Submit(() => { }, null);
            executor.Submit(() => { }, null);
            
            Assert.Equal(3, executor.GetQueuedWorkCount());
        }

        [Fact]
        public void ThreadPoolExecutor_ShouldExecuteWork()
        {
            var executor = new ThreadPoolExecutor(5, "Pool");
            var executed = false;
            executor.Submit(() => { executed = true; }, null);
            
            executor.Execute();
            
            Assert.True(executed);
        }

        [Fact]
        public void ThreadPoolExecutor_ShouldDequeueAfterExecution()
        {
            var executor = new ThreadPoolExecutor(5, "Pool");
            executor.Submit(() => { }, null);
            executor.Submit(() => { }, null);
            
            Assert.Equal(2, executor.GetQueuedWorkCount());
            
            executor.Execute();
            
            Assert.Equal(1, executor.GetQueuedWorkCount());
        }

        [Fact]
        public void ThreadPoolExecutor_ShouldShareMetadata()
        {
            var executor = new ThreadPoolExecutor(10, "SharedPool");
            var metadataList = executor.GetThreadMetadata();
            
            Assert.Equal(10, metadataList.Count);
            
            // All should have same pool name
            foreach (var meta in metadataList)
            {
                Assert.Contains("SharedPool", meta.ThreadName);
            }
        }

        [Fact]
        public void ThreadPoolExecutor_ShouldCalculateMemorySavings()
        {
            var executor = new ThreadPoolExecutor(100, "Pool");
            for (int i = 0; i < 1000; i++)
            {
                executor.Submit(() => { }, null);
            }
            
            var savings = executor.EstimateMemorySavings();
            Assert.True(savings > 0);
        }

        [Fact]
        public void ThreadPoolExecutor_ShouldHandleAsyncWork()
        {
            var executor = new ThreadPoolExecutor(5, "AsyncPool");
            var results = new System.Collections.Generic.List<int>();
            var lockObj = new object();
            
            for (int i = 0; i < 10; i++)
            {
                var value = i;
                executor.Submit((state) => 
                {
                    lock (lockObj)
                    {
                        results.Add(value);
                    }
                }, null);
            }
            
            for (int i = 0; i < 10; i++)
                executor.Execute();
            
            Assert.Equal(10, results.Count);
        }

        [Fact]
        public void LargeThreadPool_ShouldHandleManyThreads()
        {
            var executor = new ThreadPoolExecutor(1000, "LargePool");
            Assert.Equal(1000, executor.GetCoreThreadCount());
            Assert.Equal(1000, executor.GetThreadMetadataPoolSize());
        }

        [Fact]
        public void ThreadMetadataFactory_ShouldMinimizeMemory()
        {
            var factory = new ThreadMetadataFactory();
            
            for (int i = 0; i < 100; i++)
                factory.GetThreadMetadata(i, "Pool");
            
            Assert.Equal(100, factory.GetPoolSize()); // 100 unique threads
        }
    }
}
