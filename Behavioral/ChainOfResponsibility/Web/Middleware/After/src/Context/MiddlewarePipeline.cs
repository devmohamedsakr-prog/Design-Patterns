using System;
using System.Collections.Generic;

namespace MiddlewarePipeline.After.Context
{
    public class HttpRequest
    {
        public string Path { get; set; } = "";
        public string Method { get; set; } = "GET";
        public Dictionary<string, string> Headers { get; set; } = new();
        public bool IsAuthenticated { get; set; } = false;
        public string Body { get; set; } = "";
    }

    public class HttpResponse
    {
        public int StatusCode { get; set; } = 200;
        public string Body { get; set; } = "";
        public Dictionary<string, string> Headers { get; set; } = new();
    }

    public class HttpContext
    {
        public HttpRequest Request { get; set; } = new();
        public HttpResponse Response { get; set; } = new();
        public bool Handled { get; set; } = false;
    }

    public abstract class Middleware
    {
        protected Middleware _next;

        public Middleware SetNext(Middleware next)
        {
            _next = next;
            return next;
        }

        public virtual void Handle(HttpContext context)
        {
            if (_next != null)
                _next.Handle(context);
        }
    }

    public class AuthenticationMiddleware : Middleware
    {
        public override void Handle(HttpContext context)
        {
            Console.WriteLine("🔐 AuthenticationMiddleware: Checking credentials");
            
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Request.IsAuthenticated = true;
                Console.WriteLine("✓ Request authenticated");
            }
            else
                Console.WriteLine("✗ Request not authenticated");

            base.Handle(context);
        }
    }

    public class LoggingMiddleware : Middleware
    {
        public override void Handle(HttpContext context)
        {
            Console.WriteLine($"📝 LoggingMiddleware: {context.Request.Method} {context.Request.Path}");
            base.Handle(context);
            Console.WriteLine($"📊 Response: {context.Response.StatusCode}");
        }
    }

    public class CorsMiddleware : Middleware
    {
        public override void Handle(HttpContext context)
        {
            Console.WriteLine("🌐 CorsMiddleware: Adding CORS headers");
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE";
            base.Handle(context);
        }
    }

    public class CompressionMiddleware : Middleware
    {
        public override void Handle(HttpContext context)
        {
            Console.WriteLine("📦 CompressionMiddleware: Compressing response");
            context.Response.Headers["Content-Encoding"] = "gzip";
            base.Handle(context);
        }
    }

    public class ValidationMiddleware : Middleware
    {
        public override void Handle(HttpContext context)
        {
            Console.WriteLine("✔️ ValidationMiddleware: Validating request");
            
            if (string.IsNullOrEmpty(context.Request.Path))
            {
                context.Response.StatusCode = 400;
                context.Response.Body = "Bad Request";
                context.Handled = true;
                Console.WriteLine("✗ Request validation failed");
                return;
            }

            base.Handle(context);
        }
    }

    public class RequestHandler : Middleware
    {
        public override void Handle(HttpContext context)
        {
            if (!context.Handled)
            {
                Console.WriteLine("⚡ RequestHandler: Processing request");
                context.Response.StatusCode = 200;
                context.Response.Body = "Success";
                context.Handled = true;
            }

            base.Handle(context);
        }
    }

    public class MiddlewarePipeline
    {
        private Middleware _first;

        public MiddlewarePipeline AddMiddleware(Middleware middleware)
        {
            if (_first == null)
                _first = middleware;
            else
            {
                var current = _first;
                while (current._next != null)
                    current = current._next;
                current.SetNext(middleware);
            }
            return this;
        }

        public void Execute(HttpContext context)
        {
            if (_first != null)
                _first.Handle(context);
        }
    }
}
