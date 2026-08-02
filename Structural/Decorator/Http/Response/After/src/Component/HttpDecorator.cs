using System;
using System.Collections.Generic;

namespace Decorator.Http.Response.Component
{
    public abstract class HttpResponse
    {
        public string Content { get; set; }
        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public HttpResponse()
        {
            StatusCode = 200;
            Headers = new Dictionary<string, string>();
            Content = "";
        }

        public abstract void Send();
        public abstract int GetSize();
    }

    public class JsonResponse : HttpResponse
    {
        public JsonResponse(string jsonContent)
        {
            Content = jsonContent;
            StatusCode = 200;
            Headers["Content-Type"] = "application/json";
        }

        public override void Send() => Console.WriteLine($"[{StatusCode}] JSON: {Content}");
        public override int GetSize() => Content.Length;
        public override string ToString() => $"JsonResponse(200, {Content.Length} bytes)";
    }

    public abstract class HttpDecorator : HttpResponse
    {
        protected HttpResponse _response;

        public HttpDecorator(HttpResponse response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
            StatusCode = response.StatusCode;
            Headers = new Dictionary<string, string>(response.Headers);
            Content = response.Content;
        }
    }

    public class ValidationDecorator : HttpDecorator
    {
        public bool IsValid { get; set; }
        private List<string> _validationErrors;

        public ValidationDecorator(HttpResponse response) : base(response)
        {
            _validationErrors = new List<string>();
            IsValid = true;
        }

        public override void Send()
        {
            if (!IsValid)
            {
                StatusCode = 400;
                Console.WriteLine($"[400] Validation Failed");
                foreach (var error in _validationErrors)
                    Console.WriteLine($"  - {error}");
            }
            else
            {
                _response.Send();
            }
        }

        public void AddValidationError(string error)
        {
            IsValid = false;
            _validationErrors.Add(error);
        }

        public override int GetSize() => _response.GetSize();
        public override string ToString() => $"ValidationDecorator({_response})";
    }

    public class CachingDecorator : HttpDecorator
    {
        public string CacheKey { get; set; }
        private string _cachedContent;
        private bool _isCached;

        public CachingDecorator(HttpResponse response, string cacheKey = "default") : base(response)
        {
            CacheKey = cacheKey;
            _isCached = false;
        }

        public override void Send()
        {
            if (_isCached)
            {
                Console.WriteLine($"[{StatusCode}] [CACHED] {_cachedContent}");
            }
            else
            {
                _response.Send();
                _cachedContent = Content;
                _isCached = true;
            }
        }

        public override int GetSize() => _response.GetSize();
        public void InvalidateCache() => _isCached = false;
        public override string ToString() => $"CachingDecorator({_response}, key={CacheKey})";
    }

    public class CompressionDecorator : HttpDecorator
    {
        public string CompressionType { get; set; }

        public CompressionDecorator(HttpResponse response) : base(response)
        {
            CompressionType = "gzip";
            Headers["Content-Encoding"] = CompressionType;
        }

        public override void Send()
        {
            Console.WriteLine($"[{StatusCode}] [{CompressionType}] Compressed Response");
            _response.Send();
        }

        public override int GetSize() => (int)(_response.GetSize() * 0.6); // Assume 40% compression
        public override string ToString() => $"CompressionDecorator({_response}, {CompressionType})";
    }

    public class LoggingDecorator : HttpDecorator
    {
        private List<string> _logs;

        public LoggingDecorator(HttpResponse response) : base(response)
        {
            _logs = new List<string>();
        }

        public override void Send()
        {
            _logs.Add($"[SEND] {DateTime.UtcNow:O} - Status: {StatusCode}");
            _response.Send();
        }

        public override int GetSize() => _response.GetSize();
        public IReadOnlyList<string> GetLogs() => _logs.AsReadOnly();
        public override string ToString() => $"LoggingDecorator({_response}, logs={_logs.Count})";
    }
}
