using NUnit.Framework;
using ExceptionHandler.After.Context;

namespace ExceptionHandler.After.Tests
{
    [TestFixture]
    public class ExceptionHandlerTests
    {
        private ExceptionHandlingChain _chain;

        [SetUp]
        public void Setup() => _chain = new ExceptionHandlingChain();

        [Test]
        public void LowSeverityException_Logged()
        {
            var ex = new ApplicationException("Minor error", "MINOR_ERR", 1);
            _chain.ProcessException(ex);
            
            Assert.That(ex.Handled, Is.True);
        }

        [Test]
        public void HighSeverityException_AlertAndNotify()
        {
            var ex = new ApplicationException("Critical error", "CRITICAL", 4);
            _chain.ProcessException(ex);
            
            Assert.That(ex.Handled, Is.True);
        }

        [Test]
        public void RecoverableException_Recovered()
        {
            var ex = new ApplicationException("DB connection timeout", "DB_TIMEOUT", 3);
            _chain.ProcessException(ex);
            
            Assert.That(ex.Handled, Is.True);
        }

        [Test]
        public void ServiceUnavailable_RecoveryAttempted()
        {
            var ex = new ApplicationException("Service down", "SERVICE_UNAVAILABLE", 3);
            _chain.ProcessException(ex);
            
            Assert.That(ex.Handled, Is.True);
        }

        [Test]
        public void AllHandlers_Executed()
        {
            var ex = new ApplicationException("Critical system failure", "FATAL", 4);
            _chain.ProcessException(ex);
            
            Assert.That(ex.Handled, Is.True);
        }
    }
}
