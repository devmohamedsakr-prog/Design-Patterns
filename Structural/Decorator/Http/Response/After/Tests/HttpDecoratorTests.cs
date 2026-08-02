using Xunit;
using Decorator.Http.Response.Component;

namespace Decorator.Http.Response.Tests
{
    public class HttpDecoratorTests
    {
        [Fact]
        public void JsonResponse_ShouldInitializeWith200Status()
        {
            var response = new JsonResponse("{\"name\": \"test\"}");
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("application/json", response.Headers["Content-Type"]);
        }

        [Fact]
        public void ValidationDecorator_ShouldAcceptValidResponse()
        {
            var response = new JsonResponse("{\"id\": 1}");
            var validated = new ValidationDecorator(response);
            Assert.True(validated.IsValid);
        }

        [Fact]
        public void ValidationDecorator_ShouldRejectInvalidResponse()
        {
            var response = new JsonResponse("");
            var validated = new ValidationDecorator(response);
            validated.AddValidationError("Empty content");
            Assert.False(validated.IsValid);
        }

        [Fact]
        public void ValidationDecorator_ShouldSetStatus400OnError()
        {
            var response = new JsonResponse("test");
            var validated = new ValidationDecorator(response);
            validated.AddValidationError("Invalid format");
            validated.Send();
            Assert.Equal(400, validated.StatusCode);
        }

        [Fact]
        public void CachingDecorator_ShouldCacheResponse()
        {
            var response = new JsonResponse("{\"data\": \"cached\"}");
            var cached = new CachingDecorator(response, "users_list");
            cached.Send();
            cached.Send();
            Assert.True(response.StatusCode > 0);
        }

        [Fact]
        public void CachingDecorator_ShouldInvalidateCache()
        {
            var response = new JsonResponse("{\"data\": \"fresh\"}");
            var cached = new CachingDecorator(response);
            cached.Send();
            cached.InvalidateCache();
            cached.Send();
            Assert.NotNull(cached.CacheKey);
        }

        [Fact]
        public void CompressionDecorator_ShouldReduceSize()
        {
            var response = new JsonResponse("{\"large\": \"content\"}");
            var compressed = new CompressionDecorator(response);
            Assert.True(compressed.GetSize() < response.GetSize());
            Assert.Equal("gzip", compressed.CompressionType);
        }

        [Fact]
        public void CompressionDecorator_ShouldSetContentEncoding()
        {
            var response = new JsonResponse("test");
            var compressed = new CompressionDecorator(response);
            Assert.Equal("gzip", compressed.Headers["Content-Encoding"]);
        }

        [Fact]
        public void LoggingDecorator_ShouldLogOperations()
        {
            var response = new JsonResponse("{\"log\": true}");
            var logged = new LoggingDecorator(response);
            logged.Send();
            logged.Send();
            Assert.Equal(2, logged.GetLogs().Count);
        }

        [Fact]
        public void ChainedDecorators_ShouldCompose()
        {
            var response = new JsonResponse("{\"chain\": \"test\"}");
            var decorated = new LoggingDecorator(
                new CompressionDecorator(
                    new CachingDecorator(
                        new ValidationDecorator(response))));
            Assert.NotNull(decorated);
            Assert.True(decorated.IsValid);
        }

        [Fact]
        public void DecoratedResponse_ShouldPreserveHeaders()
        {
            var response = new JsonResponse("{\"test\": 1}");
            var decorated = new CompressionDecorator(response);
            Assert.NotEmpty(decorated.Headers);
            Assert.Equal("application/json", decorated.Headers["Content-Type"]);
        }

        [Fact]
        public void MultipleDecorators_ShouldStackCorrectly()
        {
            var response = new JsonResponse("{\"data\": [1,2,3]}");
            var decorated = new ValidationDecorator(response);
            decorated = new CachingDecorator(decorated);
            decorated = new CompressionDecorator(decorated);
            Assert.True(decorated.IsValid);
        }

        [Fact]
        public void ValidationDecorator_ShouldAccumulateErrors()
        {
            var response = new JsonResponse("bad");
            var validated = new ValidationDecorator(response);
            validated.AddValidationError("Error 1");
            validated.AddValidationError("Error 2");
            validated.AddValidationError("Error 3");
            Assert.False(validated.IsValid);
        }
    }
}
