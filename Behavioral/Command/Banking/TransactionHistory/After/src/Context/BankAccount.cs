using System;
using System.Collections.Generic;

namespace TransactionHistory.After.Context
{
    public interface ITransaction
    {
        bool Execute();
        bool Undo();
        string GetDescription();
    }

    public class TransactionInvoker
    {
        private Stack<ITransaction> _undoStack = new();
        private Stack<ITransaction> _redoStack = new();

        public bool ExecuteTransaction(ITransaction transaction)
        {
            if (transaction.Execute())
            {
                _undoStack.Push(transaction);
                _redoStack.Clear();
                Console.WriteLine($"✓ {transaction.GetDescription()}");
                return true;
            }
            return false;
        }

        public bool Undo()
        {
            if (_undoStack.Count == 0) return false;
            var transaction = _undoStack.Pop();
            if (transaction.Undo())
            {
                _redoStack.Push(transaction);
                Console.WriteLine($"✓ Undo: {transaction.GetDescription()}");
                return true;
            }
            return false;
        }

        public bool Redo()
        {
            if (_redoStack.Count == 0) return false;
            var transaction = _redoStack.Pop();
            if (transaction.Execute())
            {
                _undoStack.Push(transaction);
                Console.WriteLine($"✓ Redo: {transaction.GetDescription()}");
                return true;
            }
            return false;
        }

        public int GetUndoCount() => _undoStack.Count;
    }

    public class Account
    {
        public string AccountId { get; set; }
        public decimal Balance { get; set; }
    }

    public class DepositTransaction : ITransaction
    {
        private Account _account;
        private decimal _amount;

        public DepositTransaction(Account account, decimal amount)
        {
            _account = account;
            _amount = amount;
        }

        public bool Execute()
        {
            if (_amount <= 0) return false;
            _account.Balance += _amount;
            return true;
        }

        public bool Undo()
        {
            _account.Balance -= _amount;
            return true;
        }

        public string GetDescription() => $"Deposit ${_amount:F2}";
    }

    public class WithdrawTransaction : ITransaction
    {
        private Account _account;
        private decimal _amount;

        public WithdrawTransaction(Account account, decimal amount)
        {
            _account = account;
            _amount = amount;
        }

        public bool Execute()
        {
            if (_amount <= 0 || _amount > _account.Balance) return false;
            _account.Balance -= _amount;
            return true;
        }

        public bool Undo()
        {
            _account.Balance += _amount;
            return true;
        }

        public string GetDescription() => $"Withdraw ${_amount:F2}";
    }

    public class TransferTransaction : ITransaction
    {
        private Account _fromAccount;
        private Account _toAccount;
        private decimal _amount;

        public TransferTransaction(Account from, Account to, decimal amount)
        {
            _fromAccount = from;
            _toAccount = to;
            _amount = amount;
        }

        public bool Execute()
        {
            if (_amount <= 0 || _amount > _fromAccount.Balance) return false;
            _fromAccount.Balance -= _amount;
            _toAccount.Balance += _amount;
            return true;
        }

        public bool Undo()
        {
            _fromAccount.Balance += _amount;
            _toAccount.Balance -= _amount;
            return true;
        }

        public string GetDescription() => $"Transfer ${_amount:F2}";
    }
}
