using Xunit;
using Facade.Banking.System.Component;

namespace Facade.Banking.System.Tests
{
    public class BankingFacadeTests
    {
        [Fact]
        public void OpenAccount_ShouldCreateAccount()
        {
            var facade = new BankingFacade();
            var account = facade.OpenAccount("ACC001", "John Doe", "Savings");
            Assert.NotNull(account);
            Assert.Equal("ACC001", account.AccountId);
        }

        [Fact]
        public void OpenAccount_ShouldLogAuditEvent()
        {
            var facade = new BankingFacade();
            facade.OpenAccount("ACC001", "Jane", "Checking");
            var logs = facade.GenerateAuditReport();
            Assert.NotEmpty(logs);
        }

        [Fact]
        public void TransferMoney_ShouldUpdateBalances()
        {
            var facade = new BankingFacade();
            facade.OpenAccount("ACC001", "John", "Savings");
            facade.OpenAccount("ACC002", "Jane", "Savings");
            
            var success = facade.TransferMoney("ACC001", "ACC002", 100);
            Assert.True(success);
        }

        [Fact]
        public void TransferMoney_ShouldRejectNegativeAmount()
        {
            var facade = new BankingFacade();
            facade.OpenAccount("ACC001", "John", "Savings");
            facade.OpenAccount("ACC002", "Jane", "Savings");
            
            var success = facade.TransferMoney("ACC001", "ACC002", -50);
            Assert.False(success);
        }

        [Fact]
        public void ApplyForLoan_ShouldCreateLoan()
        {
            var facade = new BankingFacade();
            facade.OpenAccount("ACC001", "John", "Premium");
            var loan = facade.ApplyForLoan("ACC001", 10000, 24);
            Assert.NotNull(loan);
            Assert.Equal(10000, loan.Amount);
        }

        [Fact]
        public void GetAccountBalance_ShouldReturnBalance()
        {
            var facade = new BankingFacade();
            facade.OpenAccount("ACC001", "John", "Savings");
            var balance = facade.GetAccountBalance("ACC001");
            Assert.Equal(0, balance);
        }

        [Fact]
        public void GetAccountTransactions_ShouldReturnHistory()
        {
            var facade = new BankingFacade();
            facade.OpenAccount("ACC001", "John", "Savings");
            facade.OpenAccount("ACC002", "Jane", "Savings");
            facade.TransferMoney("ACC001", "ACC002", 50);
            
            var transactions = facade.GetAccountTransactions("ACC001");
            Assert.NotEmpty(transactions);
        }

        [Fact]
        public void GenerateAuditReport_ShouldContainAllEvents()
        {
            var facade = new BankingFacade();
            facade.OpenAccount("ACC001", "John", "Savings");
            facade.OpenAccount("ACC002", "Jane", "Savings");
            facade.TransferMoney("ACC001", "ACC002", 100);
            
            var report = facade.GenerateAuditReport();
            Assert.True(report.Count >= 3);
        }

        [Fact]
        public void FacadeHideComplexity_ShouldSimplifyBanking()
        {
            var facade = new BankingFacade();
            
            // Client only needs to call 3-4 methods instead of managing 5+ subsystems
            facade.OpenAccount("ACC001", "John", "Savings");
            facade.TransferMoney("ACC001", "ACC001", 0);
            var loan = facade.ApplyForLoan("ACC001", 5000, 12);
            
            Assert.NotNull(loan);
        }

        [Fact]
        public void MultipleTransfers_ShouldMaintainAuditTrail()
        {
            var facade = new BankingFacade();
            facade.OpenAccount("ACC001", "John", "Savings");
            facade.OpenAccount("ACC002", "Jane", "Savings");
            
            for (int i = 0; i < 5; i++)
                facade.TransferMoney("ACC001", "ACC002", 10);
            
            var report = facade.GenerateAuditReport();
            Assert.True(report.Count >= 5);
        }

        [Fact]
        public void LoanInterestRate_ShouldBeCorrect()
        {
            var facade = new BankingFacade();
            facade.OpenAccount("ACC001", "John", "Premium");
            var loan = facade.ApplyForLoan("ACC001", 50000, 60);
            
            Assert.Equal(0.05m, loan.InterestRate);
        }
    }
}
