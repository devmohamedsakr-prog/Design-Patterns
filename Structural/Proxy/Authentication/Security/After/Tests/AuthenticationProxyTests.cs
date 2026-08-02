using Xunit;
using Proxy.Authentication.Security.Component;

namespace Proxy.Authentication.Security.Tests
{
    public class AuthenticationProxyTests
    {
        [Fact]
        public void AuthenticatedProxy_ShouldDenyUnauthenticatedAccess()
        {
            var real = new BankingService("ACC001", 1000);
            var proxy = new AuthenticatedBankingProxy(real, "User");
            
            var balance = proxy.GetBalance();
            
            Assert.Equal(0, balance);
        }

        [Fact]
        public void AuthenticatedProxy_ShouldAllowAccessAfterAuthentication()
        {
            var real = new BankingService("ACC001", 1000);
            var proxy = new AuthenticatedBankingProxy(real, "User");
            
            proxy.Authenticate("secure123");
            var balance = proxy.GetBalance();
            
            Assert.Equal(1000, balance);
        }

        [Fact]
        public void AuthenticatedProxy_ShouldRejectWrongPassword()
        {
            var real = new BankingService("ACC001", 1000);
            var proxy = new AuthenticatedBankingProxy(real, "User");
            
            var authenticated = proxy.Authenticate("wrongpass");
            
            Assert.False(authenticated);
        }

        [Fact]
        public void AuthenticatedProxy_ShouldLogAuditTrail()
        {
            var real = new BankingService("ACC001", 1000);
            var proxy = new AuthenticatedBankingProxy(real, "User");
            
            proxy.GetBalance();
            
            var logs = proxy.GetAuditLog();
            Assert.NotEmpty(logs);
        }

        [Fact]
        public void AuthenticatedProxy_ShouldRejectLoanForNonPremium()
        {
            var real = new BankingService("ACC001", 1000);
            var proxy = new AuthenticatedBankingProxy(real, "User");
            
            proxy.Authenticate("secure123");
            var approved = proxy.ApplyForLoan(5000);
            
            Assert.False(approved);
        }

        [Fact]
        public void AuthenticatedProxy_ShouldApproveLoanForPremium()
        {
            var real = new BankingService("ACC001", 1000);
            var proxy = new AuthenticatedBankingProxy(real, "Premium");
            
            proxy.Authenticate("secure123");
            var approved = proxy.ApplyForLoan(5000);
            
            Assert.True(approved);
        }

        [Fact]
        public void AuthenticatedProxy_ShouldAllowTransfer()
        {
            var real = new BankingService("ACC001", 1000);
            var proxy = new AuthenticatedBankingProxy(real, "User");
            
            proxy.Authenticate("secure123");
            var success = proxy.Transfer("ACC002", 100);
            
            Assert.True(success);
        }

        [Fact]
        public void AuthenticatedProxy_ShouldDenyInsufficientFunds()
        {
            var real = new BankingService("ACC001", 50);
            var proxy = new AuthenticatedBankingProxy(real, "User");
            
            proxy.Authenticate("secure123");
            var success = proxy.Transfer("ACC002", 100);
            
            Assert.False(success);
        }

        [Fact]
        public void AuthenticatedProxy_ShouldTrackMultipleOperations()
        {
            var real = new BankingService("ACC001", 1000);
            var proxy = new AuthenticatedBankingProxy(real, "Premium");
            
            proxy.Authenticate("secure123");
            proxy.GetBalance();
            proxy.Transfer("ACC002", 100);
            proxy.ApplyForLoan(5000);
            
            var logs = proxy.GetAuditLog();
            Assert.True(logs.Count >= 3);
        }

        [Fact]
        public void BankingService_ShouldMaintainBalance()
        {
            var service = new BankingService("ACC001", 1000);
            
            service.Transfer("ACC002", 100);
            var balance = service.GetBalance();
            
            Assert.Equal(900, balance);
        }
    }
}
