using System;
using System.Collections.Generic;

namespace PayrollCalculator.Before
{
    // ============================================================================
    // BEFORE: Multiple incompatible payroll systems - No adaptation layer
    // ============================================================================
    // PROBLEM: Different payroll systems have different interfaces
    // - Must handle each system separately
    // - No unified interface
    // - Duplicate code for similar operations
    // - Hard to add new payroll systems
    // ============================================================================

    // System 1: LegacyPayrollSystem - Old interface
    public class LegacyPayrollSystem
    {
        private Dictionary<string, decimal> _employeeWages = new();

        public void RecordEmployeeWage(string employeeId, decimal hourlyRate, int hoursWorked)
        {
            decimal totalWage = hourlyRate * hoursWorked;
            _employeeWages[employeeId] = totalWage;
            Console.WriteLine($"[LEGACY] Employee {employeeId}: ${totalWage:F2}");
        }

        public decimal GetEmployeeSalary(string employeeId)
        {
            return _employeeWages.ContainsKey(employeeId) ? _employeeWages[employeeId] : 0;
        }

        public void PrintLegacyReport()
        {
            Console.WriteLine("\n=== LEGACY PAYROLL REPORT ===");
            foreach (var kvp in _employeeWages)
            {
                Console.WriteLine($"Employee: {kvp.Key}, Amount: ${kvp.Value:F2}");
            }
        }
    }

    // System 2: ModernPayrollSystem - Different interface
    public class ModernPayrollSystem
    {
        private Dictionary<string, (decimal baseSalary, decimal bonus)> _employeeData = new();

        public void SetEmployeeSalary(string empId, decimal baseSalary, decimal bonus)
        {
            _employeeData[empId] = (baseSalary, bonus);
            decimal total = baseSalary + bonus;
            Console.WriteLine($"[MODERN] Employee {empId}: ${total:F2} (Base: ${baseSalary}, Bonus: ${bonus})");
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

        public void DisplayModernReport()
        {
            Console.WriteLine("\n=== MODERN PAYROLL REPORT ===");
            foreach (var kvp in _employeeData)
            {
                var (baseSal, bonus) = kvp.Value;
                Console.WriteLine($"Employee: {kvp.Key}, Base: ${baseSal:F2}, Bonus: ${bonus:F2}, Total: ${baseSal + bonus:F2}");
            }
        }
    }

    // System 3: ContractorPaymentSystem - Yet another interface
    public class ContractorPaymentSystem
    {
        private Dictionary<string, decimal> _contractorRates = new();
        private Dictionary<string, int> _contractorHours = new();

        public void RegisterContractor(string contractorId, decimal ratePerProject)
        {
            _contractorRates[contractorId] = ratePerProject;
            Console.WriteLine($"[CONTRACTOR] Registered {contractorId}: ${ratePerProject:F2} per project");
        }

        public void LogProjectCompletion(string contractorId, int projectCount)
        {
            _contractorHours[contractorId] = projectCount;
            decimal totalPayment = _contractorRates[contractorId] * projectCount;
            Console.WriteLine($"[CONTRACTOR] {contractorId} completed {projectCount} projects: ${totalPayment:F2}");
        }

        public decimal CalculateContractorPayment(string contractorId)
        {
            if (_contractorRates.ContainsKey(contractorId) && _contractorHours.ContainsKey(contractorId))
            {
                return _contractorRates[contractorId] * _contractorHours[contractorId];
            }
            return 0;
        }

        public void ShowContractorStatement()
        {
            Console.WriteLine("\n=== CONTRACTOR PAYMENT STATEMENT ===");
            foreach (var contractorId in _contractorRates.Keys)
            {
                if (_contractorHours.ContainsKey(contractorId))
                {
                    decimal total = _contractorRates[contractorId] * _contractorHours[contractorId];
                    Console.WriteLine($"Contractor: {contractorId}, Total Payment: ${total:F2}");
                }
            }
        }
    }

    // System 4: InternshipPaymentSystem - Different again
    public class InternshipPaymentSystem
    {
        private Dictionary<string, decimal> _internStipends = new();
        private Dictionary<string, bool> _internStatus = new();

        public void EnrollIntern(string internId, decimal monthlyStipend)
        {
            _internStipends[internId] = monthlyStipend;
            _internStatus[internId] = true;
            Console.WriteLine($"[INTERN] Enrolled {internId}: ${monthlyStipend:F2}/month");
        }

        public void DeactivateIntern(string internId)
        {
            _internStatus[internId] = false;
            Console.WriteLine($"[INTERN] Deactivated {internId}");
        }

        public decimal CalculateInternPayment(string internId)
        {
            if (_internStatus.ContainsKey(internId) && _internStatus[internId])
            {
                return _internStipends[internId];
            }
            return 0;
        }

        public void PrintInternPayrollSheet()
        {
            Console.WriteLine("\n=== INTERN PAYROLL SHEET ===");
            foreach (var kvp in _internStipends)
            {
                if (_internStatus[kvp.Key])
                {
                    Console.WriteLine($"Intern: {kvp.Key}, Stipend: ${kvp.Value:F2}");
                }
            }
        }
    }

    // ============================================================================
    // PROBLEMS DEMONSTRATED:
    // ============================================================================
    // ❌ No unified interface - Each system has different method names:
    //    - LegacyPayrollSystem.RecordEmployeeWage()
    //    - ModernPayrollSystem.SetEmployeeSalary()
    //    - ContractorPaymentSystem.RegisterContractor()
    //    - InternshipPaymentSystem.EnrollIntern()
    //
    // ❌ Different retrieval methods:
    //    - GetEmployeeSalary() vs GetTotalCompensation() vs CalculateContractorPayment()
    //
    // ❌ Different report methods:
    //    - PrintLegacyReport() vs DisplayModernReport() vs ShowContractorStatement()
    //
    // ❌ Client code must know about each system:
    //    - Must handle each system separately
    //    - Duplicate logic for similar operations
    //    - Hard to maintain and extend
    //
    // ❌ Adding new payroll systems requires:
    //    - New class with different interface
    //    - More client code changes
    //    - No standard way to handle payments
    // ============================================================================

    // Client code - Must handle each system differently
    public class PayrollProcessor
    {
        public void ProcessPayroll()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         BEFORE: Multiple Incompatible Systems              ║");
            Console.WriteLine("║         (No Adapter Pattern - Direct Usage)                ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

            // System 1: Legacy Payroll
            var legacySystem = new LegacyPayrollSystem();
            legacySystem.RecordEmployeeWage("EMP001", 25.00m, 40);
            legacySystem.RecordEmployeeWage("EMP002", 30.00m, 40);

            // System 2: Modern Payroll
            var modernSystem = new ModernPayrollSystem();
            modernSystem.SetEmployeeSalary("EMP003", 3000m, 500m);
            modernSystem.SetEmployeeSalary("EMP004", 4000m, 800m);

            // System 3: Contractor Payments
            var contractorSystem = new ContractorPaymentSystem();
            contractorSystem.RegisterContractor("CONT001", 1500m);
            contractorSystem.LogProjectCompletion("CONT001", 3);

            // System 4: Intern Stipends
            var internSystem = new InternshipPaymentSystem();
            internSystem.EnrollIntern("INTERN001", 500m);
            internSystem.EnrollIntern("INTERN002", 600m);

            // Print reports - Each system has different report method
            legacySystem.PrintLegacyReport();
            modernSystem.DisplayModernReport();
            contractorSystem.ShowContractorStatement();
            internSystem.PrintInternPayrollSheet();

            // Getting payments - Each system has different method names
            Console.WriteLine("\n=== PAYMENT SUMMARY (Manual Approach) ===");
            Console.WriteLine($"Legacy Employee EMP001: ${legacySystem.GetEmployeeSalary("EMP001"):F2}");
            Console.WriteLine($"Modern Employee EMP003: ${modernSystem.GetTotalCompensation("EMP003"):F2}");
            Console.WriteLine($"Contractor CONT001: ${contractorSystem.CalculateContractorPayment("CONT001"):F2}");
            Console.WriteLine($"Intern INTERN001: ${internSystem.CalculateInternPayment("INTERN001"):F2}");

            // PROBLEMS:
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    IDENTIFIED PROBLEMS                      ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ ❌ No unified interface for all payroll systems            ║");
            Console.WriteLine("║ ❌ Different method names for similar operations           ║");
            Console.WriteLine("║ ❌ Client must know about each system's API                ║");
            Console.WriteLine("║ ❌ Hard to process payments uniformly                      ║");
            Console.WriteLine("║ ❌ Adding new payroll systems requires code changes        ║");
            Console.WriteLine("║ ❌ No standard way to handle reporting                     ║");
            Console.WriteLine("║ ❌ Difficult to maintain and extend                        ║");
            Console.WriteLine("║ ❌ High coupling between client and payroll systems        ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        }
    }

    // Entry point
    public class Program
    {
        public static void Main()
        {
            var processor = new PayrollProcessor();
            processor.ProcessPayroll();
        }
    }
}
