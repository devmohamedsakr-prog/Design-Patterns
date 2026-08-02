using System;
using System.Collections.Generic;

namespace Proxy.Authentication.Security.Component
{
    // Subject: Banking service interface
    public interface IBankingService
    {
        decimal GetBalance();
        bool Transfer(string toAccount, decimal amount);
        bool ApplyForLoan(decimal amount);
    }

    // Real Subject: Actual banking service
    public class BankingService : IBankingService
    {
        private string _accountId;
        private decimal _balance;

        public BankingService(string accountId, decimal initialBalance)
        {
            _accountId = accountId;
            _balance = initialBalance;
        }

        public decimal GetBalance()
        {
            Console.WriteLine($"  [BankingService] Returning balance: {_balance}");
            return _balance;
        }

        public bool Transfer(string toAccount, decimal amount)
        {
            if (amount > _balance) return false;
            _balance -= amount;
            Console.WriteLine($"  [BankingService] Transferred {amount} to {toAccount}");
            return true;
        }

        public bool ApplyForLoan(decimal amount)
        {
            Console.WriteLine($"  [BankingService] Approved loan of {amount}");
            return true;
        }
    }

    // Proxy: Protects service with authentication/authorization
    public class AuthenticatedBankingProxy : IBankingService
    {
        private IBankingService _realService;
        private string _userRole;
        private List<string> _auditLog;
        private bool _isAuthenticated;

        public AuthenticatedBankingProxy(IBankingService realService, string userRole)
        {
            _realService = realService;
            _userRole = userRole;
            _isAuthenticated = false;
            _auditLog = new List<string>();
        }

        public bool Authenticate(string password)
        {
            // Simulate authentication
            _isAuthenticated = (password == "secure123");
            if (_isAuthenticated)
                _auditLog.Add($"[{DateTime.UtcNow:O}] AUTHENTICATED: {_userRole}");
            else
                _auditLog.Add($"[{DateTime.UtcNow:O}] AUTH FAILED: {_userRole}");
            return _isAuthenticated;
        }

        private bool CheckPermission(string action)
        {
            if (!_isAuthenticated)
            {
                Console.WriteLine($"  ❌ [Proxy] Access DENIED: Not authenticated");
                _auditLog.Add($"[{DateTime.UtcNow:O}] DENIED (not auth): {action}");
                return false;
            }

            if (action == "Loan" && _userRole != "Premium")
            {
                Console.WriteLine($"  ❌ [Proxy] Access DENIED: Insufficient role for {action}");
                _auditLog.Add($"[{DateTime.UtcNow:O}] DENIED (role): {action} by {_userRole}");
                return false;
            }

            _auditLog.Add($"[{DateTime.UtcNow:O}] ALLOWED: {action} by {_userRole}");
            return true;
        }

        public decimal GetBalance()
        {
            if (!CheckPermission("GetBalance")) return 0;
            Console.WriteLine($"✓ [Proxy] Forwarding GetBalance");
            return _realService.GetBalance();
        }

        public bool Transfer(string toAccount, decimal amount)
        {
            if (!CheckPermission("Transfer")) return false;
            Console.WriteLine($"✓ [Proxy] Forwarding Transfer");
            return _realService.Transfer(toAccount, amount);
        }

        public bool ApplyForLoan(decimal amount)
        {
            if (!CheckPermission("Loan")) return false;
            Console.WriteLine($"✓ [Proxy] Forwarding ApplyForLoan");
            return _realService.ApplyForLoan(amount);
        }

        public IReadOnlyList<string> GetAuditLog() => _auditLog;
    }
}
