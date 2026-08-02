using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace PayrollCalculator.After.Tests
{
    /// <summary>
    /// Tests for all payroll adapter implementations
    /// Ensures each adapter correctly converts incompatible interfaces to IPayrollSystem
    /// </summary>
    [TestFixture]
    public class PayrollAdapterTests_Base
    {

        // ========================================================================
        // LEGACY PAYROLL ADAPTER TESTS (12 tests)
        // ========================================================================

        [TestFixture]
        public class LegacyPayrollAdapterTests
        {
            private IPayrollSystem _adapter;

            [SetUp]
            public void SetUp()
            {
                _adapter = new LegacyPayrollAdapter();
            }

            [Test]
            public void Adapter_HasCorrectSystemId()
            {
                Assert.That(_adapter.SystemId, Is.EqualTo("LEGACY"));
            }

            [Test]
            public void Adapter_HasCorrectSystemName()
            {
                Assert.That(_adapter.SystemName, Is.EqualTo("Legacy Payroll System"));
            }

            [Test]
            public void RegisterPerson_WithValidParameters_Succeeds()
            {
                _adapter.RegisterPerson("EMP001", 25.00m, 40);
                decimal payment = _adapter.GetTotalPayment("EMP001");
                Assert.That(payment, Is.EqualTo(1000m)); // 25 * 40
            }

            [Test]
            public void GetTotalPayment_CalculatesHourlyWageCorrectly()
            {
                _adapter.RegisterPerson("EMP001", 30.00m, 8);
                decimal payment = _adapter.GetTotalPayment("EMP001");
                Assert.That(payment, Is.EqualTo(240m)); // 30 * 8
            }

            [Test]
            public void GetTotalPayment_UnregisteredEmployee_ReturnsZero()
            {
                decimal payment = _adapter.GetTotalPayment("UNKNOWN");
                Assert.That(payment, Is.EqualTo(0m));
            }

            [Test]
            public void GetPaymentDetails_ReturnsFormattedString()
            {
                _adapter.RegisterPerson("EMP001", 20.00m, 40);
                string details = _adapter.GetPaymentDetails("EMP001");
                Assert.That(details, Does.Contain("800.00"));
                Assert.That(details, Does.Contain("Hourly Rate"));
            }

            [Test]
            public void GetAllPayments_ReturnsAllRegisteredEmployees()
            {
                _adapter.RegisterPerson("EMP001", 25.00m, 40);
                _adapter.RegisterPerson("EMP002", 30.00m, 40);
                
                var payments = _adapter.GetAllPayments();
                Assert.That(payments.Count, Is.EqualTo(2));
                Assert.That(payments["EMP001"], Is.EqualTo(1000m));
                Assert.That(payments["EMP002"], Is.EqualTo(1200m));
            }

            [Test]
            public void GenerateReport_ContainsSystemName()
            {
                _adapter.RegisterPerson("EMP001", 25.00m, 40);
                string report = _adapter.GenerateReport();
                Assert.That(report, Does.Contain("Legacy Payroll System"));
            }

            [Test]
            public void GenerateReport_ContainsEmployeeData()
            {
                _adapter.RegisterPerson("EMP001", 25.00m, 40);
                string report = _adapter.GenerateReport();
                Assert.That(report, Does.Contain("EMP001"));
                Assert.That(report, Does.Contain("1000.00"));
            }

            [Test]
            public void RegisterPerson_MissingParameters_ThrowsException()
            {
                Assert.Throws<ArgumentException>(() => _adapter.RegisterPerson("EMP001", 25.00m));
            }

            [Test]
            public void MultipleRegistrations_UpdatesExistingEmployee()
            {
                _adapter.RegisterPerson("EMP001", 25.00m, 40);
                _adapter.RegisterPerson("EMP001", 30.00m, 40);
                
                decimal payment = _adapter.GetTotalPayment("EMP001");
                Assert.That(payment, Is.EqualTo(1200m)); // Updated value
            }

            [Test]
            public void GetAllPayments_EmptyList_ReturnsEmptyDictionary()
            {
                var payments = _adapter.GetAllPayments();
                Assert.That(payments, Is.Empty);
            }
        }

        // ========================================================================
        // MODERN PAYROLL ADAPTER TESTS (12 tests)
        // ========================================================================

        [TestFixture]
        public class ModernPayrollAdapterTests
        {
            private IPayrollSystem _adapter;

            [SetUp]
            public void SetUp()
            {
                _adapter = new ModernPayrollAdapter();
            }

            [Test]
            public void Adapter_HasCorrectSystemId()
            {
                Assert.That(_adapter.SystemId, Is.EqualTo("MODERN"));
            }

            [Test]
            public void Adapter_HasCorrectSystemName()
            {
                Assert.That(_adapter.SystemName, Is.EqualTo("Modern Payroll System"));
            }

            [Test]
            public void RegisterPerson_WithBaseSalaryAndBonus_Succeeds()
            {
                _adapter.RegisterPerson("EMP001", 3000m, 500m);
                decimal payment = _adapter.GetTotalPayment("EMP001");
                Assert.That(payment, Is.EqualTo(3500m)); // 3000 + 500
            }

            [Test]
            public void GetTotalPayment_CalculatesBasePlusBonusCorrectly()
            {
                _adapter.RegisterPerson("EMP001", 4000m, 800m);
                decimal payment = _adapter.GetTotalPayment("EMP001");
                Assert.That(payment, Is.EqualTo(4800m)); // 4000 + 800
            }

            [Test]
            public void GetTotalPayment_UnregisteredEmployee_ReturnsZero()
            {
                decimal payment = _adapter.GetTotalPayment("UNKNOWN");
                Assert.That(payment, Is.EqualTo(0m));
            }

            [Test]
            public void GetPaymentDetails_ReturnsFormattedString()
            {
                _adapter.RegisterPerson("EMP001", 3000m, 500m);
                string details = _adapter.GetPaymentDetails("EMP001");
                Assert.That(details, Does.Contain("3500.00"));
                Assert.That(details, Does.Contain("Base"));
                Assert.That(details, Does.Contain("Bonus"));
            }

            [Test]
            public void GetAllPayments_ReturnsAllEmployees()
            {
                _adapter.RegisterPerson("EMP001", 3000m, 500m);
                _adapter.RegisterPerson("EMP002", 4000m, 800m);
                
                var payments = _adapter.GetAllPayments();
                Assert.That(payments.Count, Is.EqualTo(2));
                Assert.That(payments["EMP001"], Is.EqualTo(3500m));
                Assert.That(payments["EMP002"], Is.EqualTo(4800m));
            }

            [Test]
            public void GenerateReport_ContainsSystemName()
            {
                _adapter.RegisterPerson("EMP001", 3000m, 500m);
                string report = _adapter.GenerateReport();
                Assert.That(report, Does.Contain("Modern Payroll System"));
            }

            [Test]
            public void GenerateReport_ContainsEmployeeData()
            {
                _adapter.RegisterPerson("EMP001", 3000m, 500m);
                string report = _adapter.GenerateReport();
                Assert.That(report, Does.Contain("EMP001"));
                Assert.That(report, Does.Contain("3500.00"));
            }

            [Test]
            public void RegisterPerson_MissingBonus_ThrowsException()
            {
                Assert.Throws<ArgumentException>(() => _adapter.RegisterPerson("EMP001", 3000m));
            }

            [Test]
            public void MultipleRegistrations_UpdatesExistingEmployee()
            {
                _adapter.RegisterPerson("EMP001", 3000m, 500m);
                _adapter.RegisterPerson("EMP001", 4000m, 800m);
                
                decimal payment = _adapter.GetTotalPayment("EMP001");
                Assert.That(payment, Is.EqualTo(4800m)); // Updated value
            }

            [Test]
            public void GetAllPayments_EmptyList_ReturnsEmptyDictionary()
            {
                var payments = _adapter.GetAllPayments();
                Assert.That(payments, Is.Empty);
            }
        }

        // ========================================================================
        // CONTRACTOR PAYMENT ADAPTER TESTS (12 tests)
        // ========================================================================

        [TestFixture]
        public class ContractorPaymentAdapterTests
        {
            private IPayrollSystem _adapter;

            [SetUp]
            public void SetUp()
            {
                _adapter = new ContractorPaymentAdapter();
            }

            [Test]
            public void Adapter_HasCorrectSystemId()
            {
                Assert.That(_adapter.SystemId, Is.EqualTo("CONTRACTOR"));
            }

            [Test]
            public void Adapter_HasCorrectSystemName()
            {
                Assert.That(_adapter.SystemName, Is.EqualTo("Contractor Payment System"));
            }

            [Test]
            public void RegisterPerson_WithRateAndProjectCount_Succeeds()
            {
                _adapter.RegisterPerson("CONT001", 1500m, 3);
                decimal payment = _adapter.GetTotalPayment("CONT001");
                Assert.That(payment, Is.EqualTo(4500m)); // 1500 * 3
            }

            [Test]
            public void GetTotalPayment_CalculatesProjectPaymentCorrectly()
            {
                _adapter.RegisterPerson("CONT001", 2000m, 5);
                decimal payment = _adapter.GetTotalPayment("CONT001");
                Assert.That(payment, Is.EqualTo(10000m)); // 2000 * 5
            }

            [Test]
            public void GetTotalPayment_UnregisteredContractor_ReturnsZero()
            {
                decimal payment = _adapter.GetTotalPayment("UNKNOWN");
                Assert.That(payment, Is.EqualTo(0m));
            }

            [Test]
            public void GetPaymentDetails_ReturnsFormattedString()
            {
                _adapter.RegisterPerson("CONT001", 1500m, 3);
                string details = _adapter.GetPaymentDetails("CONT001");
                Assert.That(details, Does.Contain("4500.00"));
                Assert.That(details, Does.Contain("Rate"));
                Assert.That(details, Does.Contain("Project"));
            }

            [Test]
            public void GetAllPayments_ReturnsAllContractors()
            {
                _adapter.RegisterPerson("CONT001", 1500m, 3);
                _adapter.RegisterPerson("CONT002", 2000m, 5);
                
                var payments = _adapter.GetAllPayments();
                Assert.That(payments.Count, Is.EqualTo(2));
                Assert.That(payments["CONT001"], Is.EqualTo(4500m));
                Assert.That(payments["CONT002"], Is.EqualTo(10000m));
            }

            [Test]
            public void GenerateReport_ContainsSystemName()
            {
                _adapter.RegisterPerson("CONT001", 1500m, 3);
                string report = _adapter.GenerateReport();
                Assert.That(report, Does.Contain("Contractor Payment System"));
            }

            [Test]
            public void GenerateReport_ContainsContractorData()
            {
                _adapter.RegisterPerson("CONT001", 1500m, 3);
                string report = _adapter.GenerateReport();
                Assert.That(report, Does.Contain("CONT001"));
                Assert.That(report, Does.Contain("4500.00"));
            }

            [Test]
            public void RegisterPerson_MissingProjectCount_ThrowsException()
            {
                Assert.Throws<ArgumentException>(() => _adapter.RegisterPerson("CONT001", 1500m));
            }

            [Test]
            public void MultipleRegistrations_UpdatesExistingContractor()
            {
                _adapter.RegisterPerson("CONT001", 1500m, 3);
                _adapter.RegisterPerson("CONT001", 2000m, 5);
                
                decimal payment = _adapter.GetTotalPayment("CONT001");
                Assert.That(payment, Is.EqualTo(10000m)); // Updated value
            }

            [Test]
            public void GetAllPayments_EmptyList_ReturnsEmptyDictionary()
            {
                var payments = _adapter.GetAllPayments();
                Assert.That(payments, Is.Empty);
            }
        }

        // ========================================================================
        // INTERNSHIP PAYMENT ADAPTER TESTS (11 tests)
        // ========================================================================

        [TestFixture]
        public class InternshipPaymentAdapterTests
        {
            private IPayrollSystem _adapter;

            [SetUp]
            public void SetUp()
            {
                _adapter = new InternshipPaymentAdapter();
            }

            [Test]
            public void Adapter_HasCorrectSystemId()
            {
                Assert.That(_adapter.SystemId, Is.EqualTo("INTERN"));
            }

            [Test]
            public void Adapter_HasCorrectSystemName()
            {
                Assert.That(_adapter.SystemName, Is.EqualTo("Internship Payment System"));
            }

            [Test]
            public void RegisterPerson_WithMonthlyStipend_Succeeds()
            {
                _adapter.RegisterPerson("INTERN001", 500m);
                decimal payment = _adapter.GetTotalPayment("INTERN001");
                Assert.That(payment, Is.EqualTo(500m));
            }

            [Test]
            public void GetTotalPayment_ReturnsMonthlyStipend()
            {
                _adapter.RegisterPerson("INTERN001", 750m);
                decimal payment = _adapter.GetTotalPayment("INTERN001");
                Assert.That(payment, Is.EqualTo(750m));
            }

            [Test]
            public void GetTotalPayment_UnregisteredIntern_ReturnsZero()
            {
                decimal payment = _adapter.GetTotalPayment("UNKNOWN");
                Assert.That(payment, Is.EqualTo(0m));
            }

            [Test]
            public void GetPaymentDetails_ReturnsFormattedString()
            {
                _adapter.RegisterPerson("INTERN001", 500m);
                string details = _adapter.GetPaymentDetails("INTERN001");
                Assert.That(details, Does.Contain("500.00"));
                Assert.That(details, Does.Contain("Stipend"));
            }

            [Test]
            public void GetAllPayments_ReturnsAllInterns()
            {
                _adapter.RegisterPerson("INTERN001", 500m);
                _adapter.RegisterPerson("INTERN002", 600m);
                
                var payments = _adapter.GetAllPayments();
                Assert.That(payments.Count, Is.EqualTo(2));
                Assert.That(payments["INTERN001"], Is.EqualTo(500m));
                Assert.That(payments["INTERN002"], Is.EqualTo(600m));
            }

            [Test]
            public void GenerateReport_ContainsSystemName()
            {
                _adapter.RegisterPerson("INTERN001", 500m);
                string report = _adapter.GenerateReport();
                Assert.That(report, Does.Contain("Internship Payment System"));
            }

            [Test]
            public void GenerateReport_ContainsInternData()
            {
                _adapter.RegisterPerson("INTERN001", 500m);
                string report = _adapter.GenerateReport();
                Assert.That(report, Does.Contain("INTERN001"));
                Assert.That(report, Does.Contain("500.00"));
            }

            [Test]
            public void MultipleRegistrations_UpdatesExistingIntern()
            {
                _adapter.RegisterPerson("INTERN001", 500m);
                _adapter.RegisterPerson("INTERN001", 750m);
                
                decimal payment = _adapter.GetTotalPayment("INTERN001");
                Assert.That(payment, Is.EqualTo(750m)); // Updated value
            }

            [Test]
            public void GetAllPayments_EmptyList_ReturnsEmptyDictionary()
            {
                var payments = _adapter.GetAllPayments();
                Assert.That(payments, Is.Empty);
            }
        }
    }
}
