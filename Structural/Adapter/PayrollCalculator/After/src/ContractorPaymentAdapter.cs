using System;
using System.Collections.Generic;

namespace PayrollCalculator.After
{
    /// <summary>
    /// Adapts project-based payment system to IPayrollSystem interface.
    /// SRP: Single Responsibility - Adapt only Contractor system
    /// </summary>
    public class ContractorPaymentAdapter : IPayrollSystem
    {
        private readonly ContractorPaymentSystem _contractorSystem;
        private Dictionary<string, (decimal ratePerProject, int projectCount)> _records = new();

        public string SystemId => "CONTRACTOR";
        public string SystemName => "Contractor Payment System";

        public ContractorPaymentAdapter()
        {
            _contractorSystem = new ContractorPaymentSystem();
        }

        public void RegisterPerson(string personId, params decimal[] details)
        {
            if (details.Length < 2)
                throw new ArgumentException("Contractor adapter requires ratePerProject and projectCount");

            decimal ratePerProject = details[0];
            int projectCount = (int)details[1];

            _records[personId] = (ratePerProject, projectCount);
            _contractorSystem.RegisterContractor(personId, ratePerProject);
            _contractorSystem.LogProjectCompletion(personId, projectCount);
        }

        public decimal GetTotalPayment(string personId)
        {
            return _contractorSystem.CalculateContractorPayment(personId);
        }

        public string GetPaymentDetails(string personId)
        {
            if (_records.ContainsKey(personId))
            {
                var (rate, projects) = _records[personId];
                decimal total = GetTotalPayment(personId);
                return $"Rate per Project: ${rate:F2}, Projects: {projects}, Total: ${total:F2}";
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
            report.AppendLine($"║  {SystemName}  ║");
            report.AppendLine($"╠════════════════════════════════════╣");
            foreach (var kvp in _records)
            {
                var (rate, projects) = kvp.Value;
                decimal total = GetTotalPayment(kvp.Key);
                report.AppendLine($"║ ID: {kvp.Key,-8} | ${total,8:F2}     ║");
            }
            report.AppendLine($"╚════════════════════════════════════╝");
            return report.ToString();
        }

        // ---- Original Contractor System ----
        private class ContractorPaymentSystem
        {
            private Dictionary<string, decimal> _contractorRates = new();
            private Dictionary<string, int> _contractorProjects = new();

            public void RegisterContractor(string contractorId, decimal ratePerProject)
            {
                _contractorRates[contractorId] = ratePerProject;
            }

            public void LogProjectCompletion(string contractorId, int projectCount)
            {
                _contractorProjects[contractorId] = projectCount;
            }

            public decimal CalculateContractorPayment(string contractorId)
            {
                if (_contractorRates.ContainsKey(contractorId) && _contractorProjects.ContainsKey(contractorId))
                {
                    return _contractorRates[contractorId] * _contractorProjects[contractorId];
                }
                return 0;
            }
        }
    }
}
