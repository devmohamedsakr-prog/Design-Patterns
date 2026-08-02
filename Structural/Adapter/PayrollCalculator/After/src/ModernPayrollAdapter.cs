using System;
using System.Collections.Generic;

namespace PayrollCalculator.After
{
    /// <summary>
    /// Adapts salary + bonus system to IPayrollSystem interface.
    /// SRP: Single Responsibility - Adapt only Modern system
    /// </summary>
    public class ModernPayrollAdapter : IPayrollSystem
    {
        private readonly ModernPayrollSystem _modernSystem;
        private Dictionary<string, (decimal baseSalary, decimal bonus)> _records = new();

        public string SystemId => "MODERN";
        public string SystemName => "Modern Payroll System";

        public ModernPayrollAdapter()
        {
            _modernSystem = new ModernPayrollSystem();
        }

        public void RegisterPerson(string personId, params decimal[] details)
        {
            if (details.Length < 2)
                throw new ArgumentException("Modern adapter requires baseSalary and bonus");

            decimal baseSalary = details[0];
            decimal bonus = details[1];

            _records[personId] = (baseSalary, bonus);
            _modernSystem.SetEmployeeSalary(personId, baseSalary, bonus);
        }

        public decimal GetTotalPayment(string personId)
        {
            return _modernSystem.GetTotalCompensation(personId);
        }

        public string GetPaymentDetails(string personId)
        {
            if (_records.ContainsKey(personId))
            {
                var (base_, bonus) = _records[personId];
                decimal total = GetTotalPayment(personId);
                return $"Base: ${base_:F2}, Bonus: ${bonus:F2}, Total: ${total:F2}";
            }
            return "No payment details available";
        }

        public Dictionary<string, decimal> GetAllPayments()
        {
            var payments = new Dictionary<string, decimal>();
            foreach (var personId in _records.Keys)
            {
                payments[personId] = GetTotalPayment(personId);
            }
            return payments;
        }

        public string GenerateReport()
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine($"\n╔════════════════════════════════════╗");
            report.AppendLine($"║  {SystemName}      ║");
            report.AppendLine($"╠════════════════════════════════════╣");
            foreach (var kvp in _records)
            {
                var (base_, bonus) = kvp.Value;
                decimal total = GetTotalPayment(kvp.Key);
                report.AppendLine($"║ ID: {kvp.Key,-8} | ${total,8:F2}     ║");
            }
            report.AppendLine($"╚════════════════════════════════════╝");
            return report.ToString();
        }

        // ---- Original Modern System ----
        private class ModernPayrollSystem
        {
            private Dictionary<string, (decimal baseSalary, decimal bonus)> _employeeData = new();

            public void SetEmployeeSalary(string empId, decimal baseSalary, decimal bonus)
            {
                _employeeData[empId] = (baseSalary, bonus);
            }

            public decimal GetTotalCompensation(string empId)
            {
                if (_employeeData.ContainsKey(empId))
                {
                    var (base_, bonus) = _employeeData[empId];
                    return base_ + bonus;
                }
                return 0;
            }
        }
    }
}
