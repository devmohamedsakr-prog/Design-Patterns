using Xunit;
using Builder.Network.HttpRequest.Context;
using System;

namespace Builder.Network.HttpRequest.Tests
{
    public class HttpRequestTests
    {
        [Fact]
        public void Builder_CreateBasicGetRequest_Success()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com/users")
                .Build();

            Assert.Equal("GET", request.Method);
            Assert.Equal("https://api.example.com/users", request.Url);
            Assert.Equal("application/json", request.ContentType);
            Assert.Equal(30000, request.TimeoutMs);
        }

        [Fact]
        public void Builder_PostRequest_Success()
        {
            var request = HttpRequest.Builder
                .Method("POST")
                .Url("https://api.example.com/users")
                .Body("{\"name\": \"John\"}")
                .Build();

            Assert.Equal("POST", request.Method);
            Assert.Equal("{\"name\": \"John\"}", request.Body);
        }

        [Fact]
        public void Builder_WithHeaders_Success()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com/data")
                .AddHeader("X-Custom-Header", "value1")
                .AddHeader("X-API-Version", "2.0")
                .Build();

            Assert.Equal(2, request.Headers.Count);
            Assert.Equal("value1", request.Headers["X-Custom-Header"]);
            Assert.Equal("2.0", request.Headers["X-API-Version"]);
        }

        [Fact]
        public void Builder_WithQueryParameters_Success()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com/search")
                .AddQueryParameter("q", "builder pattern")
                .AddQueryParameter("limit", "10")
                .AddQueryParameter("offset", "20")
                .Build();

            Assert.Equal(3, request.QueryParameters.Count);
            Assert.Equal("builder pattern", request.QueryParameters["q"]);
            Assert.Equal("10", request.QueryParameters["limit"]);
        }

        [Fact]
        public void Builder_WithContentType_Success()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com/submit")
                .ContentType("application/x-www-form-urlencoded")
                .Build();

            Assert.Equal("application/x-www-form-urlencoded", request.ContentType);
        }

        [Fact]
        public void Builder_WithTimeout_Success()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com/slow")
                .Timeout(60000)
                .Build();

            Assert.Equal(60000, request.TimeoutMs);
        }

        [Fact]
        public void Builder_WithCookies_Success()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com/secure")
                .AddCookie("session=abc123")
                .AddCookie("user_id=42")
                .Build();

            Assert.Equal(2, request.Cookies.Count);
            Assert.Contains("session=abc123", request.Cookies);
        }

        [Fact]
        public void Builder_WithBearerToken_Success()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com/protected")
                .BearerToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9")
                .Build();

            Assert.Equal("Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9", request.Authorization);
        }

        [Fact]
        public void Builder_WithBasicAuth_Success()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com/auth")
                .BasicAuth("username", "password")
                .Build();

            Assert.StartsWith("Basic ", request.Authorization);
            // Basic dXNlcm5hbWU6cGFzc3dvcmQ= is Base64 for username:password
            Assert.Equal("Basic dXNlcm5hbWU6cGFzc3dvcmQ=", request.Authorization);
        }

        [Fact]
        public void Builder_FollowRedirects_Success()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com/redirect")
                .FollowRedirects(false)
                .Build();

            Assert.False(request.FollowRedirects);
        }

        [Fact]
        public void Builder_WithRetries_Success()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com/retry")
                .MaxRetries(3)
                .Build();

            Assert.Equal(3, request.MaxRetries);
        }

        [Fact]
        public void Builder_ComplexRequest_Success()
        {
            var request = HttpRequest.Builder
                .Method("POST")
                .Url("https://api.example.com/complex")
                .AddHeader("X-Request-ID", "12345")
                .AddHeader("Accept", "application/json")
                .AddQueryParameter("version", "v1")
                .AddQueryParameter("format", "json")
                .Body("{\"data\": \"value\"}")
                .ContentType("application/json")
                .Timeout(45000)
                .AddCookie("auth=token123")
                .BearerToken("secret-token")
                .FollowRedirects(true)
                .MaxRetries(2)
                .Build();

            Assert.Equal("POST", request.Method);
            Assert.Equal(2, request.Headers.Count);
            Assert.Equal(2, request.QueryParameters.Count);
            Assert.Single(request.Cookies);
            Assert.Equal("Bearer secret-token", request.Authorization);
            Assert.Equal(2, request.MaxRetries);
        }

        [Fact]
        public void Builder_AllHttpMethods_Success()
        {
            var methods = new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" };

            foreach (var method in methods)
            {
                var request = HttpRequest.Builder
                    .Method(method)
                    .Url("https://api.example.com")
                    .Build();

                Assert.Equal(method, request.Method);
            }
        }

        [Fact]
        public void Builder_MissingUrl_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                HttpRequest.Builder.Build()
            );

            Assert.Contains("URL is required", exception.Message);
        }

        [Fact]
        public void Builder_InvalidMethod_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                HttpRequest.Builder
                    .Method("INVALID")
                    .Url("https://api.example.com")
                    .Build()
            );

            Assert.Contains("Invalid HTTP method", exception.Message);
        }

        [Fact]
        public void Builder_NullUrl_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                HttpRequest.Builder.Url(null)
            );

            Assert.Contains("URL cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_NullContentType_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                HttpRequest.Builder.ContentType(null)
            );

            Assert.Contains("ContentType cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_InvalidTimeout_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                HttpRequest.Builder
                    .Url("https://api.example.com")
                    .Timeout(-1)
                    .Build()
            );

            Assert.Contains("Timeout must be greater than 0", exception.Message);
        }

        [Fact]
        public void Builder_InvalidMaxRetries_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                HttpRequest.Builder
                    .Url("https://api.example.com")
                    .MaxRetries(-1)
                    .Build()
            );

            Assert.Contains("MaxRetries cannot be negative", exception.Message);
        }

        [Fact]
        public void Builder_NullBearerToken_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                HttpRequest.Builder.BearerToken(null)
            );

            Assert.Contains("Token cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_InvalidBasicAuth_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                HttpRequest.Builder.BasicAuth("", "password")
            );

            Assert.Contains("Username and password cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_InvalidHeader_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                HttpRequest.Builder.AddHeader("", "value")
            );

            Assert.Contains("Key and value cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_IsImmutable_Collections()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com")
                .AddHeader("X-Test", "value")
                .AddQueryParameter("q", "test")
                .AddCookie("id=1")
                .Build();

            Assert.Throws<NotSupportedException>(() =>
            {
                ((System.Collections.Generic.Dictionary<string, string>)request.Headers).Add("X-New", "new");
            });
        }

        [Fact]
        public void Builder_FluentChaining_Success()
        {
            var request = HttpRequest.Builder
                .Method("POST")
                .Url("https://api.example.com/users")
                .AddHeader("Content-Type", "application/json")
                .AddQueryParameter("notify", "true")
                .Body("{\"name\": \"Alice\"}")
                .BearerToken("token123")
                .MaxRetries(3)
                .Build();

            Assert.NotNull(request);
            Assert.Equal("POST", request.Method);
        }

        [Fact]
        public void HttpRequest_ToString_ContainsRelevantInfo()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com/data")
                .AddHeader("X-Request-ID", "123")
                .MaxRetries(2)
                .Build();

            var str = request.ToString();
            Assert.Contains("GET", str);
            Assert.Contains("https://api.example.com/data", str);
            Assert.Contains("2", str); // MaxRetries
        }

        [Fact]
        public void Builder_DefaultValues_Applied()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com")
                .Build();

            Assert.Equal("GET", request.Method);
            Assert.Equal("application/json", request.ContentType);
            Assert.Equal(30000, request.TimeoutMs);
            Assert.True(request.FollowRedirects);
            Assert.Equal(0, request.MaxRetries);
        }

        [Fact]
        public void Builder_DuplicateHeaders_Overwrite()
        {
            var request = HttpRequest.Builder
                .Url("https://api.example.com")
                .AddHeader("X-Header", "value1")
                .AddHeader("X-Header", "value2")
                .Build();

            Assert.Single(request.Headers);
            Assert.Equal("value2", request.Headers["X-Header"]);
        }
    }
}
