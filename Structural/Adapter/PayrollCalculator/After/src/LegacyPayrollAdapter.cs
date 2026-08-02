using System;
using System.Collections.Generic;

namespace PayrollCalculator.After
{
    /// <summary>
    /// Adapts hourly wage system to IPayrollSystem interface.
    /// SRP: Single Responsibility - Adapt only Legacy system
    /// </summary>
    public class LegacyPayrollAdapter : IPayrollSystem
    {
        private readonly LegacyPayrollSystem _legacySystem;
        private Dictionary<string, (decimal hourlyRate, int hoursWorked)> _records = new();

        public string SystemId => "LEGACY";
        public string SystemName => "Legacy Payroll System";

        public LegacyPayrollAdapter()
        {
            _legacySystem = new LegacyPayrollSystem();
        }

        public void RegisterPerson(string personId, params decimal[] details)
        {
            if (details.Length < 2)
                throw new ArgumentException("Legacy adapter requires hourlyRate and hoursWorked");

            decimal hourlyRate = details[0];
            int hoursWorked = (int)details[1];

            _records[personId] = (hourlyRate, hoursWorked);
            _legacySystem.RecordEmployeeWage(personId, hourlyRate, hoursWorked);
        }

        public decimal GetTotalPayment(string personId)
        {
            return _legacySystem.GetEmployeeSalary(personId);
        }

        public string GetPaymentDetails(string personId)
        {
            if (_records.ContainsKey(personId))
            {
                var (rate, hours) = _records[personId];
                decimal total = GetTotalPayment(personId);
                return $"Hourly Rate: ${rate:F2}, Hours: {hours}, Total: ${total:F2}";
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
            report.AppendLine($"║  {SystemName}    ║");
            report.AppendLine($"╠════════════════════════════════════╣");
            foreach (var kvp in _records)
            {
                var (rate, hours) = kvp.Value;
                decimal total = GetTotalPayment(kvp.Key);
                report.AppendLine($"║ ID: {kvp.Key,-8} | ${total,8:F2}     ║");
            }
            report.AppendLine($"╚════════════════════════════════════╝");
            return report.ToString();
        }

        // ---- Original Legacy System ----
        private class LegacyPayrollSystem
        {
            private Dictionary<string, decimal> _employeeWages = new();

            public void RecordEmployeeWage(string employeeId, decimal hourlyRate, int hoursWorked)
            {
                decimal totalWage = hourlyRate * hoursWorked;
                _employeeWages[employeeId] = totalWage;
            }

            public decimal GetEmployeeSalary(string employeeId)
            {
                return _employeeWages.ContainsKey(employeeId) ? _employeeWages[employeeId] : 0;
            }
        }
    }
}
