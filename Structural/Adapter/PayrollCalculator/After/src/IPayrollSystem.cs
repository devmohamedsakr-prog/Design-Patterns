using System;
using System.Collections.Generic;

namespace PayrollCalculator.After
{
    /// <summary>
    /// Unified interface for all payroll systems.
    /// This contract enables polymorphic processing regardless of underlying implementation.
    /// SRP: Single Responsibility - Define the contract only
    /// </summary>
    public interface IPayrollSystem
    {
        string SystemId { get; }
        string SystemName { get; }
        void RegisterPerson(string personId, params decimal[] details);
        decimal GetTotalPayment(string personId);
        string GetPaymentDetails(string personId);
        Dictionary<string, decimal> GetAllPayments();
        string GenerateReport();
    }
}
