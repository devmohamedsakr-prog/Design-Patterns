using System;
using System.Collections.Generic;
using System.Linq;
using SalaryCalculator.After.Models;
using SalaryCalculator.After.Builder;

namespace SalaryCalculator.After.Configuration
{
    /// <summary>
    /// SalaryConfiguration: Complete immutable salary configuration
    /// SRP: Only responsible for holding and calculating salary data after build
    /// </summary>
    public class SalaryConfiguration
    {
        public Employee Employee { get; private set; }
        public decimal BaseSalary { get; private set; }
        public decimal BonusPercentage { get; private set; }
        public decimal TaxRate { get; private set; }
        public decimal CommissionRate { get; private set; }
        public decimal OvertimeMultiplier { get; private set; }
        public decimal LeaveEncashmentDays { get; private set; }
        
        // Benefits
        public decimal HealthInsurance { get; private set; }
        public decimal DentalInsurance { get; private set; }
        public decimal VisionInsurance { get; private set; }
        public decimal LifeInsurance { get; private set; }
        
        // Contributions
        public decimal RetirementContribution { get; private set; }
        public decimal StockOptions { get; private set; }
        
        // Custom components
        public List<SalaryComponent> CustomComponents { get; private set; } = new();

        private SalaryConfiguration(Employee employee, decimal baseSalary)
        {
            Employee = employee;
            BaseSalary = baseSalary;
        }

        public static SalaryConfigurationBuilder Builder(Employee employee, decimal baseSalary)
        {
            return new SalaryConfigurationBuilder(employee, baseSalary);
        }

        /// <summary>
        /// Calculate gross salary (base + bonus + commission + custom earnings)
        /// </summary>
        public decimal CalculateGrossSalary()
        {
            decimal gross = BaseSalary;
            gross += BaseSalary * BonusPercentage;
            gross += BaseSalary * CommissionRate;
            gross += CustomComponents
                .Where(c => c.Type == ComponentType.Earning)
                .Sum(c => c.Amount);
            return gross;
        }

        /// <summary>
        /// Calculate total benefits
        /// </summary>
        public decimal CalculateTotalBenefits()
        {
            return HealthInsurance + DentalInsurance + VisionInsurance + LifeInsurance;
        }

        /// <summary>
        /// Calculate total deductions (tax + retirement + benefits + custom deductions)
        /// </summary>
        public decimal CalculateTotalDeductions()
        {
            decimal tax = CalculateGrossSalary() * TaxRate;
            decimal totalDeductions = tax + RetirementContribution + CalculateTotalBenefits();
            totalDeductions += CustomComponents
                .Where(c => c.Type == ComponentType.Deduction)
                .Sum(c => c.Amount);
            return totalDeductions;
        }

        /// <summary>
        /// Calculate net salary (gross - deductions)
        /// </summary>
        public decimal CalculateNetSalary()
        {
            return CalculateGrossSalary() - CalculateTotalDeductions();
        }

        public override string ToString() => 
            $"Employee: {Employee.EmployeeName} ({Employee.Role}), Gross: ${CalculateGrossSalary():F2}, Deductions: ${CalculateTotalDeductions():F2}, Net: ${CalculateNetSalary():F2}";

        // Internal factory method for builder
        internal static SalaryConfiguration CreateFromBuilder(
            Employee employee,
            decimal baseSalary,
            decimal bonusPercentage,
            decimal taxRate,
            decimal commissionRate,
            decimal overtimeMultiplier,
            decimal leaveEncashmentDays,
            decimal healthInsurance,
            decimal dentalInsurance,
            decimal visionInsurance,
            decimal lifeInsurance,
            decimal retirementContribution,
            decimal stockOptions,
            List<SalaryComponent> customComponents)
        {
            return new SalaryConfiguration(employee, baseSalary)
            {
                BonusPercentage = bonusPercentage,
                TaxRate = taxRate,
                CommissionRate = commissionRate,
                OvertimeMultiplier = overtimeMultiplier,
                LeaveEncashmentDays = leaveEncashmentDays,
                HealthInsurance = healthInsurance,
                DentalInsurance = dentalInsurance,
                VisionInsurance = visionInsurance,
                LifeInsurance = lifeInsurance,
                RetirementContribution = retirementContribution,
                StockOptions = stockOptions,
                CustomComponents = new List<SalaryComponent>(customComponents)
            };
        }
    }
}
