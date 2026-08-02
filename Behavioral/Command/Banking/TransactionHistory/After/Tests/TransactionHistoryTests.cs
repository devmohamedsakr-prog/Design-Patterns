using NUnit.Framework;
using TransactionHistory.After.Context;

namespace TransactionHistory.After.Tests
{
    [TestFixture]
    public class TransactionHistoryTests
    {
        private TransactionInvoker _invoker;
        private Account _account;
        private Account _otherAccount;

        [SetUp]
        public void Setup()
        {
            _invoker = new TransactionInvoker();
            _account = new Account { AccountId = "ACC-001", Balance = 1000m };
            _otherAccount = new Account { AccountId = "ACC-002", Balance = 500m };
        }

        [Test] public void Deposit_Succeeds() => Assert.That(_invoker.ExecuteTransaction(new DepositTransaction(_account, 100m)), Is.True);
        [Test] public void Deposit_IncreasesBalance() 
        { 
            _invoker.ExecuteTransaction(new DepositTransaction(_account, 200m));
            Assert.That(_account.Balance, Is.EqualTo(1200m));
        }
        [Test] public void Deposit_Undo()
        {
            _invoker.ExecuteTransaction(new DepositTransaction(_account, 200m));
            _invoker.Undo();
            Assert.That(_account.Balance, Is.EqualTo(1000m));
        }

        [Test] public void Withdraw_Succeeds() => Assert.That(_invoker.ExecuteTransaction(new WithdrawTransaction(_account, 100m)), Is.True);
        [Test] public void Withdraw_DecreasesBalance()
        {
            _invoker.ExecuteTransaction(new WithdrawTransaction(_account, 300m));
            Assert.That(_account.Balance, Is.EqualTo(700m));
        }
        [Test] public void Withdraw_InsufficientFunds_Fails() => Assert.That(_invoker.ExecuteTransaction(new WithdrawTransaction(_account, 2000m)), Is.False);
        [Test] public void Withdraw_Undo()
        {
            _invoker.ExecuteTransaction(new WithdrawTransaction(_account, 300m));
            _invoker.Undo();
            Assert.That(_account.Balance, Is.EqualTo(1000m));
        }

        [Test] public void Transfer_Succeeds() => Assert.That(_invoker.ExecuteTransaction(new TransferTransaction(_account, _otherAccount, 200m)), Is.True);
        [Test] public void Transfer_UpdatesAccounts()
        {
            _invoker.ExecuteTransaction(new TransferTransaction(_account, _otherAccount, 200m));
            Assert.That(_account.Balance, Is.EqualTo(800m));
            Assert.That(_otherAccount.Balance, Is.EqualTo(700m));
        }
        [Test] public void Transfer_Undo()
        {
            _invoker.ExecuteTransaction(new TransferTransaction(_account, _otherAccount, 200m));
            _invoker.Undo();
            Assert.That(_account.Balance, Is.EqualTo(1000m));
            Assert.That(_otherAccount.Balance, Is.EqualTo(500m));
        }

        [Test] public void Redo_Succeeds()
        {
            _invoker.ExecuteTransaction(new DepositTransaction(_account, 100m));
            _invoker.Undo();
            Assert.That(_invoker.Redo(), Is.True);
            Assert.That(_account.Balance, Is.EqualTo(1100m));
        }

        [Test] public void MultipleTransactions_Undo()
        {
            _invoker.ExecuteTransaction(new DepositTransaction(_account, 100m));
            _invoker.ExecuteTransaction(new DepositTransaction(_account, 200m));
            _invoker.Undo();
            _invoker.Undo();
            Assert.That(_account.Balance, Is.EqualTo(1000m));
        }

        [Test] public void UndoRedo_Sequence()
        {
            _invoker.ExecuteTransaction(new DepositTransaction(_account, 100m));
            _invoker.Undo();
            _invoker.Redo();
            Assert.That(_account.Balance, Is.EqualTo(1100m));
        }

        [Test] public void NegativeDeposit_Fails() => Assert.That(_invoker.ExecuteTransaction(new DepositTransaction(_account, -100m)), Is.False);
        [Test] public void ZeroWithdraw_Fails() => Assert.That(_invoker.ExecuteTransaction(new WithdrawTransaction(_account, 0m)), Is.False);

        [Test] public void TransactionCount_Tracked()
        {
            _invoker.ExecuteTransaction(new DepositTransaction(_account, 100m));
            Assert.That(_invoker.GetUndoCount(), Is.EqualTo(1));
        }
    }
}
