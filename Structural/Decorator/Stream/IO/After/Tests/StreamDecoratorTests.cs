using Xunit;
using Decorator.Stream.IO.Component;

namespace Decorator.Stream.IO.Tests
{
    public class StreamDecoratorTests
    {
        [Fact]
        public void FileStream_ShouldReadAndWrite()
        {
            var stream = new FileStream("test.txt");
            var content = stream.Read();
            Assert.NotNull(content);
            Assert.Contains("test.txt", content);
        }

        [Fact]
        public void BufferDecorator_ShouldWrapStream()
        {
            var stream = new FileStream("data.txt");
            var buffered = new BufferDecorator(stream, 2048);
            buffered.Write("Hello");
            Assert.Equal(2048, buffered.BufferSize);
        }

        [Fact]
        public void CompressionDecorator_ShouldReduceSize()
        {
            var stream = new FileStream("large.dat");
            var compressed = new CompressionDecorator(stream);
            Assert.Equal("GZIP", compressed.CompressionType);
            Assert.True(compressed.GetSize() < stream.GetSize());
        }

        [Fact]
        public void EncryptionDecorator_ShouldEncryptData()
        {
            var stream = new FileStream("secret.txt");
            var encrypted = new EncryptionDecorator(stream, "mykey");
            Assert.Equal("AES-256", encrypted.Algorithm);
            var read = encrypted.Read();
            Assert.Contains("Decrypted", read);
        }

        [Fact]
        public void LoggingDecorator_ShouldTrackOperations()
        {
            var stream = new FileStream("test.log");
            var logged = new LoggingDecorator(stream);
            logged.Read();
            logged.Write("test");
            logged.Read();
            Assert.Equal(3, logged.GetLogs().Count);
        }

        [Fact]
        public void CachingDecorator_ShouldCacheData()
        {
            var stream = new FileStream("cache.txt");
            var cached = new CachingDecorator(stream);
            var first = cached.Read();
            var second = cached.Read();
            Assert.Contains("Cached", second);
        }

        [Fact]
        public void ChainedDecorators_ShouldCompose()
        {
            var stream = new FileStream("data.bin");
            var decorated = new LoggingDecorator(
                new CachingDecorator(
                    new EncryptionDecorator(
                        new CompressionDecorator(
                            new BufferDecorator(stream)))));
            decorated.Read();
            Assert.Contains("Logging", decorated.ToString());
        }

        [Fact]
        public void MultipleDecorators_ShouldStackCorrectly()
        {
            var base_stream = new FileStream("multi.dat");
            var decorated = new BufferDecorator(base_stream);
            Assert.Contains("BufferDecorator", decorated.ToString());
            Assert.NotNull(decorated.Read());
        }

        [Fact]
        public void CachingDecorator_ShouldInvalidateCache()
        {
            var stream = new FileStream("inv.txt");
            var cached = new CachingDecorator(stream);
            cached.Read();
            cached.InvalidateCache();
            var result = cached.Read();
            Assert.DoesNotContain("Cached", result);
        }

        [Fact]
        public void LoggingDecorator_ShouldRecordAllOperations()
        {
            var stream = new FileStream("ops.log");
            var logged = new LoggingDecorator(stream);
            for (int i = 0; i < 5; i++)
                logged.Write($"message {i}");
            Assert.Equal(5, logged.GetLogs().Count);
        }

        [Fact]
        public void AllDecorators_ShouldNotThrowOnNullStream()
        {
            var stream = new FileStream("safe.txt");
            Assert.NotNull(new BufferDecorator(stream));
            Assert.NotNull(new CompressionDecorator(stream));
            Assert.NotNull(new EncryptionDecorator(stream));
            Assert.NotNull(new LoggingDecorator(stream));
            Assert.NotNull(new CachingDecorator(stream));
        }
    }
}
