using NUnit.Framework;
using MiddlewarePipeline.After.Context;

namespace MiddlewarePipeline.After.Tests
{
    [TestFixture]
    public class MiddlewareTests
    {
        private MiddlewarePipeline _pipeline;
        private HttpContext _context;

        [SetUp]
        public void Setup()
        {
            _pipeline = new MiddlewarePipeline();
            _context = new HttpContext 
            { 
                Request = new HttpRequest { Path = "/api/users", Method = "GET" } 
            };
        }

        [Test]
        public void SingleMiddleware_Execution()
        {
            _pipeline.AddMiddleware(new LoggingMiddleware());
            _pipeline.Execute(_context);
            Assert.That(_context.Response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void MultipleMiddlewares_ChainedExecution()
        {
            _pipeline.AddMiddleware(new AuthenticationMiddleware())
                    .AddMiddleware(new LoggingMiddleware())
                    .AddMiddleware(new RequestHandler());
            
            _context.Request.Headers["Authorization"] = "Bearer token";
            _pipeline.Execute(_context);
            
            Assert.That(_context.Request.IsAuthenticated, Is.True);
            Assert.That(_context.Handled, Is.True);
        }

        [Test]
        public void ValidationMiddleware_InvalidRequest()
        {
            _pipeline.AddMiddleware(new ValidationMiddleware())
                    .AddMiddleware(new RequestHandler());
            
            _context.Request.Path = "";
            _pipeline.Execute(_context);
            
            Assert.That(_context.Response.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void CorsMiddleware_HeadersAdded()
        {
            _pipeline.AddMiddleware(new CorsMiddleware());
            _pipeline.Execute(_context);
            
            Assert.That(_context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"), Is.True);
        }

        [Test]
        public void CompressionMiddleware_EncodingSet()
        {
            _pipeline.AddMiddleware(new CompressionMiddleware());
            _pipeline.Execute(_context);
            
            Assert.That(_context.Response.Headers["Content-Encoding"], Is.EqualTo("gzip"));
        }
    }
}
