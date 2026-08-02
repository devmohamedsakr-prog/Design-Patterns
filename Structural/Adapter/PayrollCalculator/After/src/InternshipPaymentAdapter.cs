using System;
using System.Collections.Generic;

namespace PayrollCalculator.After
{
    /// <summary>
    /// Adapts monthly stipend system to IPayrollSystem interface.
    /// SRP: Single Responsibility - Adapt only Internship system
    /// </summary>
    public class InternshipPaymentAdapter : IPayrollSystem
    {
        private readonly InternshipPaymentSystem _internSystem;
        private Dictionary<string, decimal> _records = new();

        public string SystemId => "INTERN";
        public string SystemName => "Internship Payment System";

        public InternshipPaymentAdapter()
        {
            _internSystem = new InternshipPaymentSystem();
        }

        public void RegisterPerson(string personId, params decimal[] details)
        {
            if (details.Length < 1)
                throw new ArgumentException("Intern adapter requires monthlyStipend");

            decimal monthlyStipend = details[0];
            _records[personId] = monthlyStipend;
            _internSystem.EnrollIntern(personId, monthlyStipend);
        }

        public decimal GetTotalPayment(string personId)
        {
            return _internSystem.CalculateInternPayment(personId);
        }

        public string GetPaymentDetails(string personId)
        {
            if (_records.ContainsKey(personId))
            {
                decimal stipend = _records[personId];
                decimal total = GetTotalPayment(personId);
                return $"Monthly Stipend: ${stipend:F2}, Total: ${total:F2}";
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
            report.AppendLine($"║  {SystemName}   ║");
            report.AppendLine($"╠════════════════════════════════════╣");
            foreach (var kvp in _records)
            {
                decimal total = GetTotalPayment(kvp.Key);
                report.AppendLine($"║ ID: {kvp.Key,-8} | ${total,8:F2}     ║");
            }
            report.AppendLine($"╚════════════════════════════════════╝");
            return report.ToString();
        }

        // ---- Original Internship System ----
        private class InternshipPaymentSystem
        {
            private Dictionary<string, decimal> _internStipends = new();
            private Dictionary<string, bool> _internStatus = new();

            public void EnrollIntern(string internId, decimal monthlyStipend)
            {
                _internStipends[internId] = monthlyStipend;
                _internStatus[internId] = true;
            }

            public decimal CalculateInternPayment(string internId)
            {
                if (_internStatus.ContainsKey(internId) && _internStatus[internId])
                {
                    return _internStipends[internId];
                }
                return 0;
            }
        }
    }
}
