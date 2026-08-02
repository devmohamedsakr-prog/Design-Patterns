using System;

namespace AccountStatus.After.Context
{
    /// <summary>
    /// Account Context: Active → Suspended → Closed
    /// </summary>
    public class Account
    {
        public string AccountId { get; set; }
        public decimal Balance { get; set; }
        public IAccountState CurrentState { get; private set; }

        public Account(string accountId, decimal initialBalance)
        {
            AccountId = accountId;
            Balance = initialBalance;
            CurrentState = new ActiveState();
            Console.WriteLine($"[Account {accountId}] Created - State: Active");
        }

        public void TransitionTo(IAccountState newState)
        {
            string oldState = CurrentState.GetStateName();
            CurrentState = newState;
            Console.WriteLine($"[Account {AccountId}] {oldState} → {newState.GetStateName()}");
        }

        public bool Deposit(decimal amount) => CurrentState.Deposit(this, amount);
        public bool Withdraw(decimal amount) => CurrentState.Withdraw(this, amount);
        public bool Suspend() => CurrentState.Suspend(this);
        public bool Reactivate() => CurrentState.Reactivate(this);
        public bool Close() => CurrentState.Close(this);

        public string GetCurrentStateName() => CurrentState.GetStateName();
    }

    public interface IAccountState
    {
        string GetStateName();
        bool Deposit(Account account, decimal amount);
        bool Withdraw(Account account, decimal amount);
        bool Suspend(Account account);
        bool Reactivate(Account account);
        bool Close(Account account);
    }

    public class ActiveState : IAccountState
    {
        public string GetStateName() => "Active";

        public bool Deposit(Account account, decimal amount)
        {
            if (amount <= 0) return false;
            account.Balance += amount;
            Console.WriteLine($"✓ Deposited ${amount:F2}. Balance: ${account.Balance:F2}");
            return true;
        }

        public bool Withdraw(Account account, decimal amount)
        {
            if (amount <= 0 || amount > account.Balance) return false;
            account.Balance -= amount;
            Console.WriteLine($"✓ Withdrew ${amount:F2}. Balance: ${account.Balance:F2}");
            return true;
        }

        public bool Suspend(Account account)
        {
            account.TransitionTo(new SuspendedState());
            Console.WriteLine($"✓ Account suspended");
            return true;
        }

        public bool Reactivate(Account account) => false;
        public bool Close(Account account) => false;
    }

    public class SuspendedState : IAccountState
    {
        public string GetStateName() => "Suspended";

        public bool Deposit(Account account, decimal amount) => false;
        public bool Withdraw(Account account, decimal amount) => false;

        public bool Suspend(Account account) => false;

        public bool Reactivate(Account account)
        {
            account.TransitionTo(new ActiveState());
            Console.WriteLine($"✓ Account reactivated");
            return true;
        }

        public bool Close(Account account)
        {
            account.TransitionTo(new ClosedState());
            Console.WriteLine($"✓ Account closed");
            return true;
        }
    }

    public class ClosedState : IAccountState
    {
        public string GetStateName() => "Closed";

        public bool Deposit(Account account, decimal amount) => false;
        public bool Withdraw(Account account, decimal amount) => false;
        public bool Suspend(Account account) => false;
        public bool Reactivate(Account account) => false;
        public bool Close(Account account) => false;
    }
}
