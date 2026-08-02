using NUnit.Framework;
using ATMMachine.After.Context;

namespace ATMMachine.After.Tests
{
    [TestFixture]
    public class ATMTests
    {
        private ATM _atm;

        [SetUp]
        public void Setup()
        {
            _atm = new ATM("ATM-001", 5000m); // $5000 available
        }

        // ============================================================
        // INITIAL STATE TESTS
        // ============================================================

        [Test]
        public void ATM_InitialState_ShouldBeIdle()
        {
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("Idle"));
        }

        [Test]
        public void ATM_Idle_CanAcceptCard()
        {
            bool result = _atm.InsertCard("CARD-123");
            Assert.That(result, Is.True);
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("CardInserted"));
        }

        [Test]
        public void ATM_Idle_CannotEnterPIN()
        {
            bool result = _atm.EnterPIN("1234");
            Assert.That(result, Is.False);
        }

        // ============================================================
        // CARD INSERTION TESTS
        // ============================================================

        [Test]
        public void ATM_InsertCard_ValidCard_Succeeds()
        {
            bool result = _atm.InsertCard("CARD-001");
            Assert.That(result, Is.True);
            Assert.That(_atm.CurrentCardId, Is.EqualTo("CARD-001"));
        }

        [Test]
        public void ATM_InsertCard_NullCard_Fails()
        {
            bool result = _atm.InsertCard(null);
            Assert.That(result, Is.False);
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("Idle"));
        }

        [Test]
        public void ATM_InsertCard_EmptyCard_Fails()
        {
            bool result = _atm.InsertCard("");
            Assert.That(result, Is.False);
        }

        // ============================================================
        // PIN ENTRY TESTS
        // ============================================================

        [Test]
        public void ATM_CardInserted_CanEnterPIN()
        {
            _atm.InsertCard("CARD-001");
            bool result = _atm.EnterPIN("1234");
            Assert.That(result, Is.True);
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("Authenticated"));
        }

        [Test]
        public void ATM_EnterPIN_ValidPIN_Succeeds()
        {
            _atm.InsertCard("CARD-001");
            bool result = _atm.EnterPIN("5678");
            Assert.That(result, Is.True);
            Assert.That(_atm.CurrentPIN, Is.EqualTo("5678"));
        }

        [Test]
        public void ATM_EnterPIN_ShortPIN_Fails()
        {
            _atm.InsertCard("CARD-001");
            bool result = _atm.EnterPIN("123"); // Only 3 digits
            Assert.That(result, Is.False);
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("CardInserted"));
        }

        [Test]
        public void ATM_EnterPIN_NullPIN_Fails()
        {
            _atm.InsertCard("CARD-001");
            bool result = _atm.EnterPIN(null);
            Assert.That(result, Is.False);
        }

        // ============================================================
        // TRANSACTION SELECTION TESTS
        // ============================================================

        [Test]
        public void ATM_Authenticated_CanSelectTransaction()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            bool result = _atm.SelectTransaction("Withdrawal");
            Assert.That(result, Is.True);
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("TransactionSelection"));
        }

        [Test]
        public void ATM_SelectTransaction_ValidType_Succeeds()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            bool result = _atm.SelectTransaction("CheckBalance");
            Assert.That(result, Is.True);
        }

        // ============================================================
        // WITHDRAWAL TESTS
        // ============================================================

        [Test]
        public void ATM_Withdraw_ValidAmount_Succeeds()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            _atm.SelectTransaction("Withdrawal");
            bool result = _atm.Withdraw(100m);
            Assert.That(result, Is.True);
            Assert.That(_atm.RequestedAmount, Is.EqualTo(100m));
        }

        [Test]
        public void ATM_Withdraw_NegativeAmount_Fails()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            _atm.SelectTransaction("Withdrawal");
            bool result = _atm.Withdraw(-100m);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ATM_Withdraw_ZeroAmount_Fails()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            _atm.SelectTransaction("Withdrawal");
            bool result = _atm.Withdraw(0m);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ATM_Withdraw_ExceedsLimit_Fails()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            _atm.SelectTransaction("Withdrawal");
            bool result = _atm.Withdraw(1500m); // Exceeds $1000 limit
            Assert.That(result, Is.False);
        }

        [Test]
        public void ATM_Withdraw_InsufficientCash_Fails()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            _atm.SelectTransaction("Withdrawal");
            bool result = _atm.Withdraw(6000m); // More than available
            Assert.That(result, Is.False);
        }

        [Test]
        public void ATM_Withdraw_MaximumAmount_Succeeds()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            _atm.SelectTransaction("Withdrawal");
            bool result = _atm.Withdraw(1000m); // Max allowed
            Assert.That(result, Is.True);
        }

        // ============================================================
        // CARD EJECTION TESTS
        // ============================================================

        [Test]
        public void ATM_EjectCard_FromCardInserted_Succeeds()
        {
            _atm.InsertCard("CARD-001");
            bool result = _atm.EjectCard();
            Assert.That(result, Is.True);
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("Idle"));
            Assert.That(_atm.CurrentCardId, Is.Null);
        }

        [Test]
        public void ATM_EjectCard_FromAuthenticated_Succeeds()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            bool result = _atm.EjectCard();
            Assert.That(result, Is.True);
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("Idle"));
        }

        [Test]
        public void ATM_EjectCard_FromFinished_Succeeds()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            _atm.SelectTransaction("Withdrawal");
            _atm.Withdraw(100m);
            _atm.TransitionTo(new FinishedState());
            
            bool result = _atm.EjectCard();
            Assert.That(result, Is.True);
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("Idle"));
        }

        // ============================================================
        // COMPLETE WORKFLOW TESTS
        // ============================================================

        [Test]
        public void ATM_CompleteTransaction_PendingToFinished()
        {
            Assert.That(_atm.InsertCard("CARD-001"), Is.True);
            Assert.That(_atm.EnterPIN("1234"), Is.True);
            Assert.That(_atm.SelectTransaction("Withdrawal"), Is.True);
            Assert.That(_atm.Withdraw(50m), Is.True);
            
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("Processing"));
        }

        [Test]
        public void ATM_CancelTransaction_FromTransactionSelection()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            _atm.SelectTransaction("Withdrawal");
            bool result = _atm.EjectCard();
            
            Assert.That(result, Is.True);
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("Idle"));
        }

        // ============================================================
        // TIMEOUT TESTS
        // ============================================================

        [Test]
        public void ATM_Timeout_FromCardInserted_ReturnsToIdle()
        {
            _atm.InsertCard("CARD-001");
            bool result = _atm.Timeout();
            Assert.That(result, Is.True);
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("Idle"));
            Assert.That(_atm.CurrentCardId, Is.Null);
        }

        [Test]
        public void ATM_Timeout_FromAuthenticated_ReturnsToIdle()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            bool result = _atm.Timeout();
            Assert.That(result, Is.True);
            Assert.That(_atm.GetCurrentStateName(), Is.EqualTo("Idle"));
        }

        // ============================================================
        // STATE PERMISSION TESTS
        // ============================================================

        [Test]
        public void ATM_CardInserted_CannotWithdraw()
        {
            _atm.InsertCard("CARD-001");
            bool result = _atm.Withdraw(100m);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ATM_Authenticated_CannotWithdraw()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            bool result = _atm.Withdraw(100m);
            Assert.That(result, Is.False);
        }

        // ============================================================
        // CASH MANAGEMENT TESTS
        // ============================================================

        [Test]
        public void ATM_InitialCash_Set()
        {
            Assert.That(_atm.CashAvailable, Is.EqualTo(5000m));
        }

        [Test]
        public void ATM_LowCash_CannotWithdraw()
        {
            _atm = new ATM("ATM-002", 50m); // Only $50
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            _atm.SelectTransaction("Withdrawal");
            bool result = _atm.Withdraw(100m);
            Assert.That(result, Is.False);
        }

        // ============================================================
        // MULTIPLE CARDS TESTS
        // ============================================================

        [Test]
        public void ATM_MultipleCards_Sequential()
        {
            _atm.InsertCard("CARD-001");
            _atm.EjectCard();
            
            bool result = _atm.InsertCard("CARD-002");
            Assert.That(result, Is.True);
            Assert.That(_atm.CurrentCardId, Is.EqualTo("CARD-002"));
        }

        [Test]
        public void ATM_CannotInsertMultipleCards()
        {
            _atm.InsertCard("CARD-001");
            bool result = _atm.InsertCard("CARD-002");
            Assert.That(result, Is.False);
        }

        // ============================================================
        // EDGE CASES
        // ============================================================

        [Test]
        public void ATM_SmallWithdrawal_Succeeds()
        {
            _atm.InsertCard("CARD-001");
            _atm.EnterPIN("1234");
            _atm.SelectTransaction("Withdrawal");
            bool result = _atm.Withdraw(1m);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ATM_LongPIN_Succeeds()
        {
            _atm.InsertCard("CARD-001");
            bool result = _atm.EnterPIN("12345678");
            Assert.That(result, Is.True);
        }
    }
}
