using System;
using System.Collections.Generic;

namespace Facade.Banking.System.Component
{
    // Subsystem 1: Account Management
    public class AccountManager
    {
        private Dictionary<string, BankAccount> _accounts = new();

        public BankAccount CreateAccount(string accountId, string customerName, string type)
        {
            var account = new BankAccount { AccountId = accountId, CustomerName = customerName, Type = type, Balance = 0 };
            _accounts[accountId] = account;
            return account;
        }

        public BankAccount GetAccount(string accountId) => _accounts.ContainsKey(accountId) ? _accounts[accountId] : null;
        public decimal GetBalance(string accountId) => _accounts[accountId]?.Balance ?? 0;
    }

    public class BankAccount
    {
        public string AccountId { get; set; }
        public string CustomerName { get; set; }
        public string Type { get; set; }
        public decimal Balance { get; set; }
    }

    // Subsystem 2: Transaction Processing
    public class TransactionProcessor
    {
        private List<Transaction> _transactions = new();

        public bool ProcessTransaction(string fromAccount, string toAccount, decimal amount)
        {
            if (amount <= 0) return false;
            _transactions.Add(new Transaction { From = fromAccount, To = toAccount, Amount = amount, Timestamp = DateTime.UtcNow });
            return true;
        }

        public IReadOnlyList<Transaction> GetTransactionHistory(string accountId) => 
            _transactions.FindAll(t => t.From == accountId || t.To == accountId).AsReadOnly();
    }

    public class Transaction
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // Subsystem 3: Loan Management
    public class LoanManager
    {
        private Dictionary<string, Loan> _loans = new();
        private int _loanCounter = 0;

        public Loan ApplyForLoan(string accountId, decimal amount, int termMonths)
        {
            var loanId = $"LOAN_{++_loanCounter}";
            var loan = new Loan 
            { 
                LoanId = loanId, 
                AccountId = accountId, 
                Amount = amount, 
                TermMonths = termMonths, 
                Status = "Approved",
                InterestRate = 0.05m
            };
            _loans[loanId] = loan;
            return loan;
        }

        public Loan GetLoan(string loanId) => _loans.ContainsKey(loanId) ? _loans[loanId] : null;
    }

    public class Loan
    {
        public string LoanId { get; set; }
        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public int TermMonths { get; set; }
        public decimal InterestRate { get; set; }
        public string Status { get; set; }
    }

    // Subsystem 4: Security & Fraud Detection
    public class FraudDetectionService
    {
        public bool IsTransactionSafe(decimal amount, string accountType)
        {
            if (accountType == "Premium") return amount <= 100000;
            if (accountType == "Standard") return amount <= 10000;
            return amount <= 1000;
        }

        public void LogSecurityEvent(string accountId, string event_type)
        {
            // Log event
        }
    }

    // Subsystem 5: Compliance Reporting
    public class ComplianceReporter
    {
        private List<string> _auditLog = new();

        public void LogAuditEvent(string accountId, string action)
        {
            _auditLog.Add($"[{DateTime.UtcNow:O}] Account: {accountId}, Action: {action}");
        }

        public IReadOnlyList<string> GetAuditLog() => _auditLog.AsReadOnly();
    }

    // FACADE: Simplifies all subsystems
    public class BankingFacade
    {
        private AccountManager _accountManager = new();
        private TransactionProcessor _transactionProcessor = new();
        private LoanManager _loanManager = new();
        private FraudDetectionService _fraudDetection = new();
        private ComplianceReporter _complianceReporter = new();

        public BankAccount OpenAccount(string accountId, string customerName, string accountType)
        {
            var account = _accountManager.CreateAccount(accountId, customerName, accountType);
            _complianceReporter.LogAuditEvent(accountId, $"Account opened - Type: {accountType}");
            return account;
        }

        public bool TransferMoney(string fromAccount, string toAccount, decimal amount)
        {
            if (!_fraudDetection.IsTransactionSafe(amount, "Standard")) return false;
            
            var success = _transactionProcessor.ProcessTransaction(fromAccount, toAccount, amount);
            if (success)
            {
                var from = _accountManager.GetAccount(fromAccount);
                var to = _accountManager.GetAccount(toAccount);
                from.Balance -= amount;
                to.Balance += amount;
                _complianceReporter.LogAuditEvent(fromAccount, $"Transfer {amount} to {toAccount}");
            }
            return success;
        }

        public Loan ApplyForLoan(string accountId, decimal amount, int termMonths)
        {
            var loan = _loanManager.ApplyForLoan(accountId, amount, termMonths);
            _complianceReporter.LogAuditEvent(accountId, $"Loan application: {amount} for {termMonths} months");
            return loan;
        }

        public decimal GetAccountBalance(string accountId)
        {
            return _accountManager.GetBalance(accountId);
        }

        public IReadOnlyList<Transaction> GetAccountTransactions(string accountId)
        {
            return _transactionProcessor.GetTransactionHistory(accountId);
        }

        public IReadOnlyList<string> GenerateAuditReport()
        {
            return _complianceReporter.GetAuditLog();
        }
    }
}
