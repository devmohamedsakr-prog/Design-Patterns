using NUnit.Framework;
using SalaryCalculator.After;

namespace SalaryCalculator.After.Tests
{
    [TestFixture]
    public class SalaryConfigurationBuilderTests
    {
        private Employee _employee;

        [SetUp]
        public void Setup()
        {
            _employee = new Employee("EMP001", "John Doe", "Developer");
        }

        [Test]
        public void BuildBasicSalaryConfiguration()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .Build();

            Assert.That(config.BaseSalary, Is.EqualTo(50000));
            Assert.That(config.CalculateGrossSalary(), Is.EqualTo(50000));
        }

        [Test]
        public void BuildWithBonus()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .WithBonus(0.10m)
                .Build();

            Assert.That(config.BonusPercentage, Is.EqualTo(0.10m));
            Assert.That(config.CalculateGrossSalary(), Is.EqualTo(55000)); // 50000 + (50000 * 0.10)
        }

        [Test]
        public void BuildWithTaxRate()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .WithTaxRate(0.20m)
                .Build();

            Assert.That(config.TaxRate, Is.EqualTo(0.20m));
            decimal expectedDeduction = 50000 * 0.20m;
            Assert.That(config.CalculateTotalDeductions(), Is.EqualTo(expectedDeduction));
        }

        [Test]
        public void BuildWithCommission()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .WithCommission(0.05m)
                .Build();

            Assert.That(config.CommissionRate, Is.EqualTo(0.05m));
            Assert.That(config.CalculateGrossSalary(), Is.EqualTo(52500)); // 50000 + (50000 * 0.05)
        }

        [Test]
        public void BuildWithBenefits()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .WithBenefits(500, 150, 100, 250)
                .Build();

            Assert.That(config.HealthInsurance, Is.EqualTo(500));
            Assert.That(config.DentalInsurance, Is.EqualTo(150));
            Assert.That(config.VisionInsurance, Is.EqualTo(100));
            Assert.That(config.LifeInsurance, Is.EqualTo(250));
            Assert.That(config.CalculateTotalBenefits(), Is.EqualTo(1000));
        }

        [Test]
        public void BuildWithRetirement()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .WithRetirement(3000)
                .Build();

            Assert.That(config.RetirementContribution, Is.EqualTo(3000));
            Assert.That(config.CalculateTotalDeductions(), Is.GreaterThanOrEqualTo(3000));
        }

        [Test]
        public void BuildFullTimeEmployee()
        {
            var config = SalaryConfiguration.Builder(_employee, 80000)
                .WithBonus(0.12m)
                .WithTaxRate(0.22m)
                .WithBenefits(600, 150, 100, 250)
                .WithRetirement(3000)
                .WithStockOptions(1500)
                .Build();

            Assert.That(config.CalculateGrossSalary(), Is.EqualTo(81500)); // 80000 + bonus
            Assert.That(config.CalculateTotalBenefits(), Is.EqualTo(1100));
        }

        [Test]
        public void BuildMultipleBenefitsIndividually()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .WithHealthInsurance(600)
                .WithDentalInsurance(150)
                .WithVisionInsurance(100)
                .WithLifeInsurance(250)
                .Build();

            Assert.That(config.HealthInsurance, Is.EqualTo(600));
            Assert.That(config.DentalInsurance, Is.EqualTo(150));
            Assert.That(config.VisionInsurance, Is.EqualTo(100));
            Assert.That(config.LifeInsurance, Is.EqualTo(250));
        }

        [Test]
        public void FluentInterfaceChaining()
        {
            var config = SalaryConfiguration.Builder(_employee, 60000)
                .WithBonus(0.15m)
                .WithTaxRate(0.25m)
                .WithCommission(0.05m)
                .WithRetirement(2000)
                .WithBenefits(500, 150, 100, 250)
                .WithStockOptions(1000)
                .Build();

            Assert.That(config.BonusPercentage, Is.EqualTo(0.15m));
            Assert.That(config.TaxRate, Is.EqualTo(0.25m));
            Assert.That(config.CommissionRate, Is.EqualTo(0.05m));
            Assert.That(config.RetirementContribution, Is.EqualTo(2000));
        }

        [Test]
        public void CalculateNetSalary()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .WithBonus(0.10m)
                .WithTaxRate(0.20m)
                .WithBenefits(500, 100, 50, 150)
                .WithRetirement(1000)
                .Build();

            decimal gross = config.CalculateGrossSalary(); // 55000
            decimal deductions = config.CalculateTotalDeductions();
            decimal net = config.CalculateNetSalary();

            Assert.That(net, Is.EqualTo(gross - deductions));
        }

        [Test]
        public void InvalidBaseSalary_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                SalaryConfiguration.Builder(_employee, -50000).Build();
            });
        }

        [Test]
        public void InvalidBonusPercentage_TooHigh_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                SalaryConfiguration.Builder(_employee, 50000)
                    .WithBonus(1.5m)
                    .Build();
            });
        }

        [Test]
        public void InvalidTaxRate_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                SalaryConfiguration.Builder(_employee, 50000)
                    .WithTaxRate(1.5m)
                    .Build();
            });
        }

        [Test]
        public void InvalidCommissionRate_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                SalaryConfiguration.Builder(_employee, 50000)
                    .WithCommission(-0.05m)
                    .Build();
            });
        }

        [Test]
        public void InvalidBenefitAmount_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                SalaryConfiguration.Builder(_employee, 50000)
                    .WithHealthInsurance(-100)
                    .Build();
            });
        }

        [Test]
        public void CustomComponent_Earning()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .WithCustomComponent("Bonus Pool", 2000, ComponentType.Earning)
                .Build();

            decimal gross = config.CalculateGrossSalary();
            Assert.That(gross, Is.EqualTo(52000)); // 50000 + 2000
        }

        [Test]
        public void CustomComponent_Deduction()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .WithCustomComponent("Union Dues", 500, ComponentType.Deduction)
                .Build();

            decimal deductions = config.CalculateTotalDeductions();
            Assert.That(deductions, Is.GreaterThanOrEqualTo(500));
        }

        [Test]
        public void MultipleCustomComponents()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .WithCustomComponent("Bonus Pool", 1000, ComponentType.Earning)
                .WithCustomComponent("Union Dues", 200, ComponentType.Deduction)
                .WithCustomComponent("Equipment", 300, ComponentType.Deduction)
                .Build();

            Assert.That(config.CustomComponents.Count, Is.EqualTo(3));
        }

        [Test]
        public void DefaultValues()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000).Build();

            Assert.That(config.BonusPercentage, Is.EqualTo(0));
            Assert.That(config.TaxRate, Is.EqualTo(0.15m)); // Default
            Assert.That(config.CommissionRate, Is.EqualTo(0));
            Assert.That(config.OvertimeMultiplier, Is.EqualTo(1.0m));
        }

        [Test]
        public void BuildMultipleConfigurations()
        {
            var emp1 = SalaryConfiguration.Builder(_employee, 50000)
                .WithBonus(0.10m)
                .WithTaxRate(0.20m)
                .Build();

            var emp2 = new Employee("EMP002", "Jane Doe", "Manager");
            var managerConfig = SalaryConfiguration.Builder(emp2, 70000)
                .WithBonus(0.20m)
                .WithTaxRate(0.25m)
                .Build();

            Assert.That(emp1.CalculateGrossSalary(), Is.EqualTo(55000));
            Assert.That(managerConfig.CalculateGrossSalary(), Is.EqualTo(84000));
        }

        [Test]
        public void FullTimeEmployeeTemplate()
        {
            var config = EmployeeRoleTemplates.FullTimeEmployeeTemplate(_employee, 60000).Build();

            Assert.That(config.BonusPercentage, Is.EqualTo(0.12m));
            Assert.That(config.TaxRate, Is.EqualTo(0.22m));
            Assert.That(config.CalculateTotalBenefits(), Is.GreaterThan(0));
        }

        [Test]
        public void ManagerTemplate()
        {
            var manager = new Employee("EMP003", "Boss Person", "Manager");
            var config = EmployeeRoleTemplates.ManagerTemplate(manager, 80000).Build();

            Assert.That(config.BonusPercentage, Is.EqualTo(0.20m));
            Assert.That(config.TaxRate, Is.EqualTo(0.25m));
        }

        [Test]
        public void ContractorTemplate()
        {
            var contractor = new Employee("EMP004", "Contractor", "Contractor");
            var config = EmployeeRoleTemplates.ContractorTemplate(contractor, 100000).Build();

            Assert.That(config.TaxRate, Is.EqualTo(0.20m));
            Assert.That(config.CommissionRate, Is.EqualTo(0.10m));
            Assert.That(config.CalculateTotalBenefits(), Is.EqualTo(0)); // No benefits for contractor
        }

        [Test]
        public void InternTemplate()
        {
            var intern = new Employee("EMP005", "Intern", "Intern");
            var config = EmployeeRoleTemplates.InternTemplate(intern, 25000).Build();

            Assert.That(config.TaxRate, Is.EqualTo(0.15m));
            Assert.That(config.CalculateTotalBenefits(), Is.GreaterThan(0));
        }

        [Test]
        public void TemplateCanBeExtended()
        {
            var config = EmployeeRoleTemplates.FullTimeEmployeeTemplate(_employee, 60000)
                .WithBenefits(800, 200, 150, 350) // Override template benefits
                .Build();

            Assert.That(config.HealthInsurance, Is.EqualTo(800)); // Overridden
            Assert.That(config.BonusPercentage, Is.EqualTo(0.12m)); // From template
        }

        [Test]
        public void BuilderIsReusable()
        {
            var builder = SalaryConfiguration.Builder(_employee, 50000)
                .WithBonus(0.10m)
                .WithTaxRate(0.20m);

            var config1 = builder.Build();
            var config2 = builder.Build();

            Assert.That(config1.CalculateGrossSalary(), Is.EqualTo(config2.CalculateGrossSalary()));
        }

        [Test]
        public void ComplexSalaryConfiguration()
        {
            var executive = new Employee("EMP006", "CEO", "Executive");
            var config = SalaryConfiguration.Builder(executive, 200000)
                .WithBonus(0.50m)
                .WithCommission(0.02m)
                .WithTaxRate(0.30m)
                .WithBenefits(1000, 300, 200, 500)
                .WithRetirement(10000)
                .WithStockOptions(50000)
                .WithCustomComponent("Car Allowance", 1500, ComponentType.Earning)
                .WithCustomComponent("Travel", 2000, ComponentType.Deduction)
                .Build();

            decimal gross = config.CalculateGrossSalary();
            decimal net = config.CalculateNetSalary();

            Assert.That(gross, Is.GreaterThan(200000));
            Assert.That(net, Is.LessThan(gross));
            Assert.That(net, Is.GreaterThan(0));
        }

        [Test]
        public void SalaryComponentDetails()
        {
            var config = SalaryConfiguration.Builder(_employee, 50000)
                .WithCustomComponent("Bonus", 5000, ComponentType.Earning)
                .Build();

            var earnings = config.CustomComponents.Where(c => c.Type == ComponentType.Earning).ToList();
            Assert.That(earnings.Count, Is.EqualTo(1));
            Assert.That(earnings[0].Name, Is.EqualTo("Bonus"));
            Assert.That(earnings[0].Amount, Is.EqualTo(5000));
        }
    }
}
