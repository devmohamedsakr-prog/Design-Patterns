using System;
using System.Collections.Generic;
using System.Linq;

namespace SalaryCalculator.Before
{
    /// <summary>
    /// BEFORE: Salary Calculator WITHOUT Builder Pattern
    /// Problem: Complex configuration with many parameters
    /// </summary>

    public class SalaryCalculatorBefore
    {
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Role { get; set; } = "";
        
        public decimal BaseSalary { get; set; }
        public decimal BonusPercentage { get; set; }
        public decimal TaxRate { get; set; }
        public decimal InsuranceDeduction { get; set; }
        public decimal RetirementContribution { get; set; }
        public decimal HealthInsurance { get; set; }
        public decimal DentalInsurance { get; set; }
        public decimal VisionInsurance { get; set; }
        public decimal LifeInsurance { get; set; }
        public decimal StockOptions { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal OvertimeMultiplier { get; set; }
        public decimal LeaveEncashmentDays { get; set; }

        public SalaryCalculatorBefore(
            string employeeId,
            string employeeName,
            string role,
            decimal baseSalary,
            decimal bonusPercentage = 0,
            decimal taxRate = 0.15m,
            decimal insuranceDeduction = 0,
            decimal retirementContribution = 0,
            decimal healthInsurance = 0,
            decimal dentalInsurance = 0,
            decimal visionInsurance = 0,
            decimal lifeInsurance = 0,
            decimal stockOptions = 0,
            decimal commissionRate = 0,
            decimal overtimeMultiplier = 1.0m,
            decimal leaveEncashmentDays = 0)
        {
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            Role = role;
            BaseSalary = baseSalary;
            BonusPercentage = bonusPercentage;
            TaxRate = taxRate;
            InsuranceDeduction = insuranceDeduction;
            RetirementContribution = retirementContribution;
            HealthInsurance = healthInsurance;
            DentalInsurance = dentalInsurance;
            VisionInsurance = visionInsurance;
            LifeInsurance = lifeInsurance;
            StockOptions = stockOptions;
            CommissionRate = commissionRate;
            OvertimeMultiplier = overtimeMultiplier;
            LeaveEncashmentDays = leaveEncashmentDays;

            Console.WriteLine($"  ✓ Salary calculator created for {employeeName} ({role})");
        }

        public decimal CalculateGrossSalary()
        {
            decimal bonus = BaseSalary * BonusPercentage;
            decimal commission = BaseSalary * CommissionRate;
            return BaseSalary + bonus + commission;
        }

        public decimal CalculateTotalDeductions()
        {
            decimal tax = CalculateGrossSalary() * TaxRate;
            decimal totalInsurance = HealthInsurance + DentalInsurance + VisionInsurance + LifeInsurance;
            return tax + InsuranceDeduction + RetirementContribution + totalInsurance;
        }

        public decimal CalculateNetSalary()
        {
            return CalculateGrossSalary() - CalculateTotalDeductions();
        }

        public override string ToString() => 
            $"Employee: {EmployeeName} ({Role}), Gross: ${CalculateGrossSalary():F2}, Net: ${CalculateNetSalary():F2}";
    }

    /// <summary>
    /// APPLICATION 1: Creating Different Employee Types WITHOUT Builder (STRUGGLES)
    /// Scenario: HR needs to calculate salaries for different employee types
    /// </summary>
    public class EmployeeCreationWithoutBuilder
    {
        public static void Demo()
        {
            Console.WriteLine("\n=== APPLICATION 1: Creating Employee Types WITHOUT Builder ===");
            Console.WriteLine("Scenario: Creating different employee salary calculators\n");

            Console.WriteLine("1️⃣ Creating Full-Time Employee:");
            // Problem: Many parameters, hard to understand which is which
            var fullTimeEmployee = new SalaryCalculatorBefore(
                employeeId: "EMP001",
                employeeName: "Alice Johnson",
                role: "Senior Developer",
                baseSalary: 80000,
                bonusPercentage: 0.15m,           // 15% bonus
                taxRate: 0.22m,                   // 22% tax
                insuranceDeduction: 500,
                retirementContribution: 3000,
                healthInsurance: 600,
                dentalInsurance: 150,
                visionInsurance: 100,
                lifeInsurance: 250,
                stockOptions: 2000,
                commissionRate: 0,
                overtimeMultiplier: 1.5m,
                leaveEncashmentDays: 30
            );
            Console.WriteLine($"   {fullTimeEmployee}\n");

            Console.WriteLine("2️⃣ Creating Another Full-Time Employee (Similar Config!):");
            // ❌ CODE DUPLICATION: Same parameters, only baseSalary different!
            var anotherFullTime = new SalaryCalculatorBefore(
                employeeId: "EMP002",
                employeeName: "Bob Smith",
                role: "Senior Developer",
                baseSalary: 85000,                // Only this changed!
                bonusPercentage: 0.15m,          // Duplicated
                taxRate: 0.22m,                  // Duplicated
                insuranceDeduction: 500,         // Duplicated
                retirementContribution: 3000,    // Duplicated
                healthInsurance: 600,            // Duplicated
                dentalInsurance: 150,            // Duplicated
                visionInsurance: 100,            // Duplicated
                lifeInsurance: 250,              // Duplicated
                stockOptions: 2000,              // Duplicated
                commissionRate: 0,               // Duplicated
                overtimeMultiplier: 1.5m,        // Duplicated
                leaveEncashmentDays: 30          // Duplicated
            );
            Console.WriteLine($"   {anotherFullTime}\n");

            Console.WriteLine("3️⃣ Creating Contract Employee:");
            var contractor = new SalaryCalculatorBefore(
                employeeId: "EMP003",
                employeeName: "Charlie Brown",
                role: "Contractor",
                baseSalary: 100000,
                bonusPercentage: 0,              // No bonus
                taxRate: 0.20m,                  // Different tax
                insuranceDeduction: 0,           // No insurance deduction
                retirementContribution: 0,       // No retirement
                healthInsurance: 0,              // No benefits!
                dentalInsurance: 0,
                visionInsurance: 0,
                lifeInsurance: 0,
                stockOptions: 0,                 // No stock
                commissionRate: 0.10m,           // Commission instead
                overtimeMultiplier: 1.0m,
                leaveEncashmentDays: 0           // No leave encashment
            );
            Console.WriteLine($"   {contractor}\n");

            Console.WriteLine("❌ PROBLEMS:");
            Console.WriteLine("   - Parameter order is hard to remember");
            Console.WriteLine("   - Many parameters for simple cases");
            Console.WriteLine("   - Massive code duplication for similar roles");
            Console.WriteLine("   - Hard to understand what each number means");
            Console.WriteLine("   - If policy changes, must update all places!\n");
        }
    }

    /// <summary>
    /// APPLICATION 2: Modifying Salary Configuration WITHOUT Builder (STRUGGLES)
    /// Scenario: HR needs to create templates and modify them for different policies
    /// </summary>
    public class SalaryPolicyUpdateWithoutBuilder
    {
        public static void Demo()
        {
            Console.WriteLine("\n=== APPLICATION 2: Salary Policy Updates WITHOUT Builder ===");
            Console.WriteLine("Scenario: Tax rate changes, must update all employees\n");

            Console.WriteLine("1️⃣ Initial Employee Setup (Old Tax Rate 20%):");
            var employee1 = new SalaryCalculatorBefore(
                "EMP001", "Alice", "Manager", 70000, 0.10m, 0.20m,
                500, 3000, 600, 150, 100, 250, 1500, 0, 1.5m, 30
            );
            Console.WriteLine($"   Original Tax Rate: 20%");
            Console.WriteLine($"   Gross Salary: ${employee1.CalculateGrossSalary():F2}");
            Console.WriteLine($"   Net Salary: ${employee1.CalculateNetSalary():F2}\n");

            Console.WriteLine("2️⃣ Tax Rate Changes to 25% (Policy Update):");
            // ❌ PROBLEM: Must manually update everywhere!
            employee1.TaxRate = 0.25m;
            Console.WriteLine($"   Updated Tax Rate: 25%");
            Console.WriteLine($"   Gross Salary: ${employee1.CalculateGrossSalary():F2}");
            Console.WriteLine($"   New Net Salary: ${employee1.CalculateNetSalary():F2}\n");

            Console.WriteLine("3️⃣ If we had 100 employees, would need to update all 100!");
            Console.WriteLine("   Employee 1: tax = 0.25m ✓");
            Console.WriteLine("   Employee 2: tax = 0.20m ❌ (forgot to update!)");
            Console.WriteLine("   Employee 3: tax = 0.25m ✓");
            Console.WriteLine("   ...");
            Console.WriteLine("   Employee 100: tax = ??? (inconsistent!)\n");

            Console.WriteLine("❌ PROBLEMS:");
            Console.WriteLine("   - No way to create reusable templates");
            Console.WriteLine("   - Policy changes require manual updates everywhere");
            Console.WriteLine("   - Risk of inconsistency (some employees wrong rate)");
            Console.WriteLine("   - HR nightmare when updating policies");
            Console.WriteLine("   - No way to manage different employee categories!\n");
        }
    }
}
