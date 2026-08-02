using System;
using System.Collections.Generic;
using System.Linq;

namespace SalaryCalculator.After
{
    /// <summary>
    /// Employee: Core data for an employee
    /// </summary>
    public class Employee
    {
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Role { get; set; } = "";

        public Employee(string employeeId, string employeeName, string role)
        {
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            Role = role;
        }
    }

    /// <summary>
    /// SalaryComponent: Represents a single component of salary (bonus, tax, etc.)
    /// </summary>
    public class SalaryComponent
    {
        public string Name { get; set; } = "";
        public decimal Amount { get; set; }
        public ComponentType Type { get; set; }

        public SalaryComponent(string name, decimal amount, ComponentType type)
        {
            Name = name;
            Amount = amount;
            Type = type;
        }
    }

    /// <summary>
    /// ComponentType: Types of salary components
    /// </summary>
    public enum ComponentType
    {
        Earning,
        Deduction
    }

    /// <summary>
    /// SalaryConfiguration: Complete salary configuration with fluent builder
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

        public decimal CalculateTotalBenefits()
        {
            return HealthInsurance + DentalInsurance + VisionInsurance + LifeInsurance;
        }

        public decimal CalculateTotalDeductions()
        {
            decimal tax = CalculateGrossSalary() * TaxRate;
            decimal totalDeductions = tax + RetirementContribution + CalculateTotalBenefits();
            totalDeductions += CustomComponents
                .Where(c => c.Type == ComponentType.Deduction)
                .Sum(c => c.Amount);
            return totalDeductions;
        }

        public decimal CalculateNetSalary()
        {
            return CalculateGrossSalary() - CalculateTotalDeductions();
        }

        public override string ToString() => 
            $"Employee: {Employee.EmployeeName} ({Employee.Role}), Gross: ${CalculateGrossSalary():F2}, Deductions: ${CalculateTotalDeductions():F2}, Net: ${CalculateNetSalary():F2}";
    }

    /// <summary>
    /// SalaryConfigurationBuilder: Fluent builder for salary configuration
    /// Supports step-by-step configuration with validation and clear intent
    /// </summary>
    public class SalaryConfigurationBuilder
    {
        private Employee _employee;
        private decimal _baseSalary;
        private decimal _bonusPercentage = 0;
        private decimal _taxRate = 0.15m;
        private decimal _commissionRate = 0;
        private decimal _overtimeMultiplier = 1.0m;
        private decimal _leaveEncashmentDays = 0;
        private decimal _healthInsurance = 0;
        private decimal _dentalInsurance = 0;
        private decimal _visionInsurance = 0;
        private decimal _lifeInsurance = 0;
        private decimal _retirementContribution = 0;
        private decimal _stockOptions = 0;
        private List<SalaryComponent> _customComponents = new();

        public SalaryConfigurationBuilder(Employee employee, decimal baseSalary)
        {
            if (baseSalary <= 0)
                throw new ArgumentException("Base salary must be positive", nameof(baseSalary));
            
            _employee = employee;
            _baseSalary = baseSalary;
        }

        public SalaryConfigurationBuilder WithBonus(decimal bonusPercentage)
        {
            if (bonusPercentage < 0 || bonusPercentage > 1)
                throw new ArgumentException("Bonus percentage must be between 0 and 1", nameof(bonusPercentage));
            
            _bonusPercentage = bonusPercentage;
            return this;
        }

        public SalaryConfigurationBuilder WithTaxRate(decimal taxRate)
        {
            if (taxRate < 0 || taxRate > 1)
                throw new ArgumentException("Tax rate must be between 0 and 1", nameof(taxRate));
            
            _taxRate = taxRate;
            return this;
        }

        public SalaryConfigurationBuilder WithCommission(decimal commissionRate)
        {
            if (commissionRate < 0 || commissionRate > 1)
                throw new ArgumentException("Commission rate must be between 0 and 1", nameof(commissionRate));
            
            _commissionRate = commissionRate;
            return this;
        }

        public SalaryConfigurationBuilder WithOvertime(decimal multiplier)
        {
            if (multiplier < 1)
                throw new ArgumentException("Overtime multiplier must be >= 1", nameof(multiplier));
            
            _overtimeMultiplier = multiplier;
            return this;
        }

        public SalaryConfigurationBuilder WithLeaveEncashment(decimal days)
        {
            if (days < 0)
                throw new ArgumentException("Leave encashment days cannot be negative", nameof(days));
            
            _leaveEncashmentDays = days;
            return this;
        }

        public SalaryConfigurationBuilder WithBenefits(
            decimal health = 0,
            decimal dental = 0,
            decimal vision = 0,
            decimal life = 0)
        {
            if (health < 0 || dental < 0 || vision < 0 || life < 0)
                throw new ArgumentException("Benefit amounts cannot be negative");
            
            _healthInsurance = health;
            _dentalInsurance = dental;
            _visionInsurance = vision;
            _lifeInsurance = life;
            return this;
        }

        public SalaryConfigurationBuilder WithHealthInsurance(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            _healthInsurance = amount;
            return this;
        }

        public SalaryConfigurationBuilder WithDentalInsurance(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            _dentalInsurance = amount;
            return this;
        }

        public SalaryConfigurationBuilder WithVisionInsurance(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            _visionInsurance = amount;
            return this;
        }

        public SalaryConfigurationBuilder WithLifeInsurance(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            _lifeInsurance = amount;
            return this;
        }

        public SalaryConfigurationBuilder WithRetirement(decimal contribution)
        {
            if (contribution < 0)
                throw new ArgumentException("Contribution cannot be negative", nameof(contribution));
            
            _retirementContribution = contribution;
            return this;
        }

        public SalaryConfigurationBuilder WithStockOptions(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            _stockOptions = amount;
            return this;
        }

        public SalaryConfigurationBuilder WithCustomComponent(string name, decimal amount, ComponentType type)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Component name cannot be empty", nameof(name));
            
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));
            
            _customComponents.Add(new SalaryComponent(name, amount, type));
            return this;
        }

        public SalaryConfiguration Build()
        {
            Console.WriteLine($"  ✓ Building salary configuration for {_employee.EmployeeName} ({_employee.Role})");
            
            var config = new SalaryConfiguration(_employee, _baseSalary)
            {
                BonusPercentage = _bonusPercentage,
                TaxRate = _taxRate,
                CommissionRate = _commissionRate,
                OvertimeMultiplier = _overtimeMultiplier,
                LeaveEncashmentDays = _leaveEncashmentDays,
                HealthInsurance = _healthInsurance,
                DentalInsurance = _dentalInsurance,
                VisionInsurance = _visionInsurance,
                LifeInsurance = _lifeInsurance,
                RetirementContribution = _retirementContribution,
                StockOptions = _stockOptions,
                CustomComponents = new List<SalaryComponent>(_customComponents)
            };

            return config;
        }
    }

    /// <summary>
    /// EmployeeRoleTemplate: Predefined templates for different employee roles
    /// Demonstrates how Builder enables template reuse
    /// </summary>
    public static class EmployeeRoleTemplates
    {
        public static SalaryConfigurationBuilder FullTimeEmployeeTemplate(Employee employee, decimal baseSalary)
        {
            return SalaryConfiguration.Builder(employee, baseSalary)
                .WithBonus(0.12m)
                .WithTaxRate(0.22m)
                .WithBenefits(600, 150, 100, 250)
                .WithRetirement(3000)
                .WithStockOptions(1500);
        }

        public static SalaryConfigurationBuilder ManagerTemplate(Employee employee, decimal baseSalary)
        {
            return SalaryConfiguration.Builder(employee, baseSalary)
                .WithBonus(0.20m)
                .WithTaxRate(0.25m)
                .WithBenefits(800, 200, 150, 350)
                .WithRetirement(5000)
                .WithStockOptions(3000);
        }

        public static SalaryConfigurationBuilder ContractorTemplate(Employee employee, decimal baseSalary)
        {
            return SalaryConfiguration.Builder(employee, baseSalary)
                .WithTaxRate(0.20m)
                .WithCommission(0.10m);
        }

        public static SalaryConfigurationBuilder InternTemplate(Employee employee, decimal baseSalary)
        {
            return SalaryConfiguration.Builder(employee, baseSalary)
                .WithTaxRate(0.15m)
                .WithBenefits(200, 50, 25, 75);
        }
    }
}
