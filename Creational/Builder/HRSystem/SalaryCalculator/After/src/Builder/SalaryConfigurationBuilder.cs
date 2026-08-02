using System;
using System.Collections.Generic;
using SalaryCalculator.After.Configuration;
using SalaryCalculator.After.Models;

namespace SalaryCalculator.After.Builder
{
    /// <summary>
    /// SalaryConfigurationBuilder: Fluent builder for salary configuration
    /// SRP: Only responsible for step-by-step construction and validation
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
            
            return SalaryConfiguration.CreateFromBuilder(
                _employee,
                _baseSalary,
                _bonusPercentage,
                _taxRate,
                _commissionRate,
                _overtimeMultiplier,
                _leaveEncashmentDays,
                _healthInsurance,
                _dentalInsurance,
                _visionInsurance,
                _lifeInsurance,
                _retirementContribution,
                _stockOptions,
                _customComponents
            );
        }
    }
}
