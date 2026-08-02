using System;

namespace ATMMachine.After.Context
{
    /// <summary>
    /// ATM Context: Manages ATM state and transitions
    /// States: Idle → CardInserted → Authenticated → TransactionSelection → Processing → Finished → Idle
    /// </summary>
    public class ATM
    {
        public string ATMId { get; set; }
        public decimal CashAvailable { get; set; }
        public IATMState CurrentState { get; private set; }
        public string CurrentCardId { get; set; }
        public string CurrentPIN { get; set; }
        public decimal RequestedAmount { get; set; }

        public ATM(string atmId, decimal initialCash)
        {
            ATMId = atmId;
            CashAvailable = initialCash;
            CurrentState = new IdleState();
            Console.WriteLine($"[ATM {atmId}] Initialized in Idle state with ${initialCash:F2}");
        }

        /// <summary>
        /// Transition to new state
        /// </summary>
        public void TransitionTo(IATMState newState)
        {
            string oldState = CurrentState.GetStateName();
            CurrentState = newState;
            Console.WriteLine($"[ATM {ATMId}] State: {oldState} → {newState.GetStateName()}");
        }

        // Delegate operations to current state
        public bool InsertCard(string cardId) => CurrentState.InsertCard(this, cardId);
        public bool EnterPIN(string pin) => CurrentState.EnterPIN(this, pin);
        public bool SelectTransaction(string transactionType) => CurrentState.SelectTransaction(this, transactionType);
        public bool Withdraw(decimal amount) => CurrentState.Withdraw(this, amount);
        public bool EjectCard() => CurrentState.EjectCard(this);
        public bool Timeout() => CurrentState.Timeout(this);

        public string GetCurrentStateName() => CurrentState.GetStateName();
    }

    /// <summary>
    /// ATM State Interface: Defines operations per state
    /// </summary>
    public interface IATMState
    {
        string GetStateName();
        bool InsertCard(ATM atm, string cardId);
        bool EnterPIN(ATM atm, string pin);
        bool SelectTransaction(ATM atm, string transactionType);
        bool Withdraw(ATM atm, decimal amount);
        bool EjectCard(ATM atm);
        bool Timeout(ATM atm);
    }

    // ============================================================
    // CONCRETE STATES
    // ============================================================

    /// <summary>State 1: Idle - Waiting for card insertion</summary>
    public class IdleState : IATMState
    {
        public string GetStateName() => "Idle";

        public bool InsertCard(ATM atm, string cardId)
        {
            if (string.IsNullOrEmpty(cardId)) return false;
            atm.CurrentCardId = cardId;
            atm.TransitionTo(new CardInsertedState());
            Console.WriteLine($"✓ Card inserted: {cardId}");
            return true;
        }

        public bool EnterPIN(ATM atm, string pin) => false;
        public bool SelectTransaction(ATM atm, string transactionType) => false;
        public bool Withdraw(ATM atm, decimal amount) => false;
        public bool EjectCard(ATM atm) => false;
        public bool Timeout(ATM atm) => false;
    }

    /// <summary>State 2: CardInserted - Waiting for PIN</summary>
    public class CardInsertedState : IATMState
    {
        public string GetStateName() => "CardInserted";

        public bool InsertCard(ATM atm, string cardId) => false; // Already inserted

        public bool EnterPIN(ATM atm, string pin)
        {
            if (string.IsNullOrEmpty(pin) || pin.Length < 4) return false;
            atm.CurrentPIN = pin;
            atm.TransitionTo(new AuthenticatedState());
            Console.WriteLine($"✓ PIN entered (length: {pin.Length})");
            return true;
        }

        public bool SelectTransaction(ATM atm, string transactionType) => false;
        public bool Withdraw(ATM atm, decimal amount) => false;

        public bool EjectCard(ATM atm)
        {
            atm.CurrentCardId = null;
            atm.TransitionTo(new IdleState());
            Console.WriteLine($"✓ Card ejected");
            return true;
        }

        public bool Timeout(ATM atm)
        {
            atm.CurrentCardId = null;
            atm.TransitionTo(new IdleState());
            Console.WriteLine($"✓ Timeout: Card ejected automatically");
            return true;
        }
    }

    /// <summary>State 3: Authenticated - PIN verified, ready for transaction</summary>
    public class AuthenticatedState : IATMState
    {
        public string GetStateName() => "Authenticated";

        public bool InsertCard(ATM atm, string cardId) => false;
        public bool EnterPIN(ATM atm, string pin) => false; // Already authenticated

        public bool SelectTransaction(ATM atm, string transactionType)
        {
            if (string.IsNullOrEmpty(transactionType)) return false;
            atm.TransitionTo(new TransactionSelectionState());
            Console.WriteLine($"✓ Transaction selected: {transactionType}");
            return true;
        }

        public bool Withdraw(ATM atm, decimal amount) => false;

        public bool EjectCard(ATM atm)
        {
            atm.CurrentCardId = null;
            atm.CurrentPIN = null;
            atm.TransitionTo(new IdleState());
            Console.WriteLine($"✓ Card ejected");
            return true;
        }

        public bool Timeout(ATM atm)
        {
            atm.CurrentCardId = null;
            atm.CurrentPIN = null;
            atm.TransitionTo(new IdleState());
            Console.WriteLine($"✓ Timeout: Card ejected automatically");
            return true;
        }
    }

    /// <summary>State 4: TransactionSelection - Customer selecting withdrawal amount</summary>
    public class TransactionSelectionState : IATMState
    {
        public string GetStateName() => "TransactionSelection";

        public bool InsertCard(ATM atm, string cardId) => false;
        public bool EnterPIN(ATM atm, string pin) => false;
        public bool SelectTransaction(ATM atm, string transactionType) => false;

        public bool Withdraw(ATM atm, decimal amount)
        {
            if (amount <= 0 || amount > 1000) return false; // Validate amount
            if (amount > atm.CashAvailable) return false; // Insufficient cash

            atm.RequestedAmount = amount;
            atm.TransitionTo(new ProcessingState());
            Console.WriteLine($"✓ Withdrawal requested: ${amount:F2}");
            return true;
        }

        public bool EjectCard(ATM atm)
        {
            atm.CurrentCardId = null;
            atm.CurrentPIN = null;
            atm.TransitionTo(new IdleState());
            Console.WriteLine($"✓ Transaction cancelled. Card ejected");
            return true;
        }

        public bool Timeout(ATM atm)
        {
            atm.CurrentCardId = null;
            atm.CurrentPIN = null;
            atm.TransitionTo(new IdleState());
            Console.WriteLine($"✓ Timeout: Transaction cancelled");
            return true;
        }
    }

    /// <summary>State 5: Processing - Dispensing cash</summary>
    public class ProcessingState : IATMState
    {
        public string GetStateName() => "Processing";

        public bool InsertCard(ATM atm, string cardId) => false;
        public bool EnterPIN(ATM atm, string pin) => false;
        public bool SelectTransaction(ATM atm, string transactionType) => false;

        public bool Withdraw(ATM atm, decimal amount) => false; // Already processing

        public bool EjectCard(ATM atm) => false; // Cannot eject during processing

        public bool Timeout(ATM atm)
        {
            // Timeout during processing - cancel transaction
            atm.CurrentCardId = null;
            atm.CurrentPIN = null;
            atm.RequestedAmount = 0;
            atm.TransitionTo(new IdleState());
            Console.WriteLine($"✓ Timeout: Transaction failed, returning to Idle");
            return true;
        }

        /// <summary>Complete withdrawal</summary>
        public bool CompleteWithdrawal(ATM atm)
        {
            atm.CashAvailable -= atm.RequestedAmount;
            Console.WriteLine($"✓ Cash dispensed: ${atm.RequestedAmount:F2}. Remaining: ${atm.CashAvailable:F2}");
            atm.TransitionTo(new FinishedState());
            return true;
        }
    }

    /// <summary>State 6: Finished - Transaction complete, returning card</summary>
    public class FinishedState : IATMState
    {
        public string GetStateName() => "Finished";

        public bool InsertCard(ATM atm, string cardId) => false;
        public bool EnterPIN(ATM atm, string pin) => false;
        public bool SelectTransaction(ATM atm, string transactionType) => false;
        public bool Withdraw(ATM atm, decimal amount) => false;

        public bool EjectCard(ATM atm)
        {
            atm.CurrentCardId = null;
            atm.CurrentPIN = null;
            atm.RequestedAmount = 0;
            atm.TransitionTo(new IdleState());
            Console.WriteLine($"✓ Card returned. ATM returning to Idle state");
            return true;
        }

        public bool Timeout(ATM atm)
        {
            // Timeout in Finished state - return card automatically
            atm.CurrentCardId = null;
            atm.CurrentPIN = null;
            atm.RequestedAmount = 0;
            atm.TransitionTo(new IdleState());
            Console.WriteLine($"✓ Timeout: Card ejected automatically");
            return true;
        }
    }
}
