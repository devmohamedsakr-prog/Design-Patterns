using SalaryCalculator.After.Builder;
using SalaryCalculator.After.Models;

namespace SalaryCalculator.After.Templates
{
    /// <summary>
    /// EmployeeRoleTemplates: Predefined templates for different employee roles
    /// SRP: Only responsible for providing role-based configuration templates
    /// </summary>
    public static class EmployeeRoleTemplates
    {
        /// <summary>
        /// Full-Time Employee Template
        /// Includes benefits, retirement, stock options
        /// </summary>
        public static SalaryConfigurationBuilder FullTimeEmployeeTemplate(Employee employee, decimal baseSalary)
        {
            return Configuration.SalaryConfiguration.Builder(employee, baseSalary)
                .WithBonus(0.12m)
                .WithTaxRate(0.22m)
                .WithBenefits(600, 150, 100, 250)
                .WithRetirement(3000)
                .WithStockOptions(1500);
        }

        /// <summary>
        /// Manager Template
        /// Higher bonus, benefits, and stock options than full-time employees
        /// </summary>
        public static SalaryConfigurationBuilder ManagerTemplate(Employee employee, decimal baseSalary)
        {
            return Configuration.SalaryConfiguration.Builder(employee, baseSalary)
                .WithBonus(0.20m)
                .WithTaxRate(0.25m)
                .WithBenefits(800, 200, 150, 350)
                .WithRetirement(5000)
                .WithStockOptions(3000);
        }

        /// <summary>
        /// Contractor Template
        /// Commission-based, minimal benefits
        /// </summary>
        public static SalaryConfigurationBuilder ContractorTemplate(Employee employee, decimal baseSalary)
        {
            return Configuration.SalaryConfiguration.Builder(employee, baseSalary)
                .WithTaxRate(0.20m)
                .WithCommission(0.10m);
        }

        /// <summary>
        /// Intern Template
        /// Basic benefits, no stock options
        /// </summary>
        public static SalaryConfigurationBuilder InternTemplate(Employee employee, decimal baseSalary)
        {
            return Configuration.SalaryConfiguration.Builder(employee, baseSalary)
                .WithTaxRate(0.15m)
                .WithBenefits(200, 50, 25, 75);
        }
    }
}
