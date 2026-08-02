using NUnit.Framework;
using AccountStatus.After.Context;

namespace AccountStatus.After.Tests
{
    [TestFixture]
    public class AccountStatusTests
    {
        private Account _account;

        [SetUp]
        public void Setup()
        {
            _account = new Account("ACC-001", 1000m);
        }

        [Test] public void Account_Initial_Active() => Assert.That(_account.GetCurrentStateName(), Is.EqualTo("Active"));
        [Test] public void Account_Active_CanDeposit() => Assert.That(_account.Deposit(100m), Is.True);
        [Test] public void Account_Deposit_IncreasesBalance()
        {
            _account.Deposit(250m);
            Assert.That(_account.Balance, Is.EqualTo(1250m));
        }
        [Test] public void Account_NegativeDeposit_Fails() => Assert.That(_account.Deposit(-100m), Is.False);
        [Test] public void Account_Active_CanWithdraw() => Assert.That(_account.Withdraw(100m), Is.True);
        [Test] public void Account_Withdraw_DecreasesBalance()
        {
            _account.Withdraw(200m);
            Assert.That(_account.Balance, Is.EqualTo(800m));
        }
        [Test] public void Account_WithdrawMore_Fails() => Assert.That(_account.Withdraw(2000m), Is.False);
        [Test] public void Account_Suspend_Succeeds() => Assert.That(_account.Suspend(), Is.True);
        [Test] public void Account_Suspend_ChangeState()
        {
            _account.Suspend();
            Assert.That(_account.GetCurrentStateName(), Is.EqualTo("Suspended"));
        }
        [Test] public void Account_Suspended_CannotDeposit()
        {
            _account.Suspend();
            Assert.That(_account.Deposit(100m), Is.False);
        }
        [Test] public void Account_Suspended_CannotWithdraw()
        {
            _account.Suspend();
            Assert.That(_account.Withdraw(100m), Is.False);
        }
        [Test] public void Account_Suspended_CanReactivate()
        {
            _account.Suspend();
            Assert.That(_account.Reactivate(), Is.True);
            Assert.That(_account.GetCurrentStateName(), Is.EqualTo("Active"));
        }
        [Test] public void Account_Suspended_CanClose() => 
        {
            _account.Suspend();
            Assert.That(_account.Close(), Is.True);
        }
        [Test] public void Account_Closed_State()
        {
            _account.Suspend();
            _account.Close();
            Assert.That(_account.GetCurrentStateName(), Is.EqualTo("Closed"));
        }
        [Test] public void Account_Closed_CannotDeposit()
        {
            _account.Suspend();
            _account.Close();
            Assert.That(_account.Deposit(100m), Is.False);
        }
        [Test] public void Account_Closed_CannotWithdraw()
        {
            _account.Suspend();
            _account.Close();
            Assert.That(_account.Withdraw(100m), Is.False);
        }
        [Test] public void Account_Closed_CannotReactivate()
        {
            _account.Suspend();
            _account.Close();
            Assert.That(_account.Reactivate(), Is.False);
        }
        [Test] public void Account_Suspended_CannotSuspend() 
        {
            _account.Suspend();
            Assert.That(_account.Suspend(), Is.False);
        }
        [Test] public void Account_MultipleTransactions()
        {
            _account.Deposit(500m);
            _account.Withdraw(300m);
            _account.Deposit(100m);
            Assert.That(_account.Balance, Is.EqualTo(1300m));
        }
        [Test] public void Account_ZeroBalance_CanStillWithdraw()
        {
            _account = new Account("ACC-002", 0m);
            Assert.That(_account.Withdraw(100m), Is.False);
        }
        [Test] public void Account_LargeDeposit()
        {
            _account.Deposit(1000000m);
            Assert.That(_account.Balance, Is.EqualTo(1001000m));
        }
    }
}
