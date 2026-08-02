using System;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Network.HttpRequest.Context
{
    /// <summary>
    /// Product: Immutable HTTP request constructed via builder.
    /// Demonstrates: Fluent API for building complex HTTP requests with headers, auth, etc.
    /// </summary>
    public class HttpRequest
    {
        public string Method { get; } // GET, POST, PUT, DELETE, PATCH
        public string Url { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
        public IReadOnlyDictionary<string, string> QueryParameters { get; }
        public string Body { get; }
        public string ContentType { get; }
        public int TimeoutMs { get; }
        public IReadOnlyList<string> Cookies { get; }
        public string Authorization { get; } // Bearer, Basic, etc.
        public bool FollowRedirects { get; }
        public int MaxRetries { get; }

        private HttpRequest(
            string method,
            string url,
            IReadOnlyDictionary<string, string> headers,
            IReadOnlyDictionary<string, string> queryParameters,
            string body,
            string contentType,
            int timeoutMs,
            IReadOnlyList<string> cookies,
            string authorization,
            bool followRedirects,
            int maxRetries)
        {
            Method = method;
            Url = url;
            Headers = headers;
            QueryParameters = queryParameters;
            Body = body;
            ContentType = contentType;
            TimeoutMs = timeoutMs;
            Cookies = cookies;
            Authorization = authorization;
            FollowRedirects = followRedirects;
            MaxRetries = maxRetries;
        }

        public static HttpRequestBuilder Builder => new HttpRequestBuilder();

        public override string ToString()
        {
            return $"HttpRequest(Method={Method}, Url={Url}, Headers={Headers.Count}, " +
                   $"QueryParams={QueryParameters.Count}, TimeoutMs={TimeoutMs}, MaxRetries={MaxRetries})";
        }

        /// <summary>
        /// Builder class: Fluent API for constructing HttpRequest.
        /// </summary>
        public class HttpRequestBuilder
        {
            private string _method = "GET";
            private string _url;
            private readonly Dictionary<string, string> _headers = new();
            private readonly Dictionary<string, string> _queryParameters = new();
            private string _body;
            private string _contentType = "application/json";
            private int _timeoutMs = 30000;
            private readonly List<string> _cookies = new();
            private string _authorization;
            private bool _followRedirects = true;
            private int _maxRetries = 0;

            /// <summary>
            /// Set HTTP method: GET, POST, PUT, DELETE, PATCH.
            /// </summary>
            public HttpRequestBuilder Method(string method)
            {
                if (!new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" }.Contains(method))
                    throw new ArgumentException("Invalid HTTP method", nameof(method));
                _method = method;
                return this;
            }

            /// <summary>
            /// Set request URL (required).
            /// </summary>
            public HttpRequestBuilder Url(string url)
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentException("URL cannot be null or empty", nameof(url));
                _url = url;
                return this;
            }

            /// <summary>
            /// Add HTTP header.
            /// </summary>
            public HttpRequestBuilder AddHeader(string key, string value)
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Key and value cannot be null or empty");
                _headers[key] = value;
                return this;
            }

            /// <summary>
            /// Add query parameter.
            /// </summary>
            public HttpRequestBuilder AddQueryParameter(string key, string value)
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Key and value cannot be null or empty");
                _queryParameters[key] = value;
                return this;
            }

            /// <summary>
            /// Set request body.
            /// </summary>
            public HttpRequestBuilder Body(string body)
            {
                if (string.IsNullOrWhiteSpace(body))
                    throw new ArgumentException("Body cannot be null or empty", nameof(body));
                _body = body;
                return this;
            }

            /// <summary>
            /// Set content type.
            /// </summary>
            public HttpRequestBuilder ContentType(string contentType)
            {
                if (string.IsNullOrWhiteSpace(contentType))
                    throw new ArgumentException("ContentType cannot be null or empty", nameof(contentType));
                _contentType = contentType;
                return this;
            }

            /// <summary>
            /// Set request timeout in milliseconds.
            /// </summary>
            public HttpRequestBuilder Timeout(int milliseconds)
            {
                if (milliseconds <= 0)
                    throw new ArgumentException("Timeout must be greater than 0", nameof(milliseconds));
                _timeoutMs = milliseconds;
                return this;
            }

            /// <summary>
            /// Add cookie.
            /// </summary>
            public HttpRequestBuilder AddCookie(string cookie)
            {
                if (string.IsNullOrWhiteSpace(cookie))
                    throw new ArgumentException("Cookie cannot be null or empty", nameof(cookie));
                _cookies.Add(cookie);
                return this;
            }

            /// <summary>
            /// Set Bearer token authorization.
            /// </summary>
            public HttpRequestBuilder BearerToken(string token)
            {
                if (string.IsNullOrWhiteSpace(token))
                    throw new ArgumentException("Token cannot be null or empty", nameof(token));
                _authorization = $"Bearer {token}";
                return this;
            }

            /// <summary>
            /// Set Basic authentication.
            /// </summary>
            public HttpRequestBuilder BasicAuth(string username, string password)
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Username and password cannot be null or empty");
                var credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{username}:{password}"));
                _authorization = $"Basic {credentials}";
                return this;
            }

            /// <summary>
            /// Enable or disable redirect following.
            /// </summary>
            public HttpRequestBuilder FollowRedirects(bool follow)
            {
                _followRedirects = follow;
                return this;
            }

            /// <summary>
            /// Set maximum number of retries.
            /// </summary>
            public HttpRequestBuilder MaxRetries(int retries)
            {
                if (retries < 0)
                    throw new ArgumentException("MaxRetries cannot be negative", nameof(retries));
                _maxRetries = retries;
                return this;
            }

            public HttpRequest Build()
            {
                if (string.IsNullOrWhiteSpace(_url))
                    throw new InvalidOperationException("URL is required");

                return new HttpRequest(
                    _method,
                    _url,
                    new Dictionary<string, string>(_headers),
                    new Dictionary<string, string>(_queryParameters),
                    _body,
                    _contentType,
                    _timeoutMs,
                    _cookies.AsReadOnly(),
                    _authorization,
                    _followRedirects,
                    _maxRetries
                );
            }
        }
    }
}
