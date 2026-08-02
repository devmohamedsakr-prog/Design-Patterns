using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PayrollCalculator.After.Tests
{
    /// <summary>
    /// Integration tests for the Adapter Pattern implementation
    /// Tests complete workflows and system interactions
    /// </summary>
    [TestFixture]
    public class IntegrationTests
    {
        private List<IPayrollSystem> _payrollSystems;

        [SetUp]
        public void SetUp()
        {
            _payrollSystems = new List<IPayrollSystem>
            {
                new LegacyPayrollAdapter(),
                new ModernPayrollAdapter(),
                new ContractorPaymentAdapter(),
                new InternshipPaymentAdapter()
            };
        }

        // ========================================================================
        // INTEGRATION TESTS (13+ tests)
        // ========================================================================

        [Test]
        public void AllAdapters_ImplementIPayrollSystem()
        {
            foreach (var system in _payrollSystems)
            {
                Assert.IsInstanceOf<IPayrollSystem>(system);
            }
        }

        [Test]
        public void AllAdapters_HaveUniqueSystemIds()
        {
            var ids = _payrollSystems.Select(s => s.SystemId).ToList();
            var uniqueIds = ids.Distinct().ToList();
            Assert.That(uniqueIds.Count, Is.EqualTo(ids.Count));
        }

        [Test]
        public void AllAdapters_HaveUniqueSystemNames()
        {
            var names = _payrollSystems.Select(s => s.SystemName).ToList();
            var uniqueNames = names.Distinct().ToList();
            Assert.That(uniqueNames.Count, Is.EqualTo(names.Count));
        }

        [Test]
        public void UnifiedPayroll_ProcessesAllSystemsPolymorphically()
        {
            foreach (var system in _payrollSystems)
            {
                system.RegisterPerson("TEST001", 1000m, 100m);
                Assert.That(system.GetTotalPayment("TEST001"), Is.GreaterThan(0));
            }
        }

        [Test]
        public void UnifiedPayroll_CalculatesCorrectTotalPayroll()
        {
            var processor = new UnifiedPayrollProcessor();
            foreach (var system in _payrollSystems)
            {
                processor.RegisterPayrollSystem(system);
            }

            // Register test data
            _payrollSystems[0].RegisterPerson("E1", 50m, 40);      // Legacy: 2000
            _payrollSystems[1].RegisterPerson("E2", 2000m, 200m);  // Modern: 2200
            _payrollSystems[2].RegisterPerson("C1", 1000m, 2);     // Contractor: 2000
            _payrollSystems[3].RegisterPerson("I1", 500m);         // Intern: 500

            decimal totalPayroll = 0;
            foreach (var system in _payrollSystems)
            {
                var payments = system.GetAllPayments();
                totalPayroll += payments.Values.Sum();
            }

            Assert.That(totalPayroll, Is.EqualTo(6700m));
        }

        [Test]
        public void UnifiedPayroll_GeneratesReportsForAllSystems()
        {
            foreach (var system in _payrollSystems)
            {
                var testId = $"TEST_{system.SystemId}";
                
                if (system.SystemId == "LEGACY")
                    system.RegisterPerson(testId, 1000m, 1);
                else if (system.SystemId == "MODERN")
                    system.RegisterPerson(testId, 1000m, 100m);
                else if (system.SystemId == "CONTRACTOR")
                    system.RegisterPerson(testId, 1000m, 1);
                else
                    system.RegisterPerson(testId, 1000m);

                string report = system.GenerateReport();
                
                Assert.That(report, Does.Contain(system.SystemName));
                Assert.That(report, Does.Contain(testId));
            }
        }

        [Test]
        public void UnifiedPayroll_GetAllPaymentsConsistency()
        {
            var system = _payrollSystems[0];
            system.RegisterPerson("E1", 100m, 10);
            system.RegisterPerson("E2", 200m, 10);

            var payments = system.GetAllPayments();
            var totalFromDictionary = payments.Values.Sum();
            var totalFromIndividualCalls = 
                system.GetTotalPayment("E1") + system.GetTotalPayment("E2");

            Assert.That(totalFromDictionary, Is.EqualTo(totalFromIndividualCalls));
        }

        [Test]
        public void Adapters_RespectPolymorphism()
        {
            // Test that all systems can be treated the same way
            foreach (var system in _payrollSystems)
            {
                // Register 2 people with different parameter counts
                var testId1 = $"ID_{system.SystemId}_1";
                var testId2 = $"ID_{system.SystemId}_2";

                if (system.SystemId == "LEGACY")
                {
                    system.RegisterPerson(testId1, 20m, 40);
                    system.RegisterPerson(testId2, 25m, 40);
                }
                else if (system.SystemId == "MODERN")
                {
                    system.RegisterPerson(testId1, 3000m, 500m);
                    system.RegisterPerson(testId2, 4000m, 600m);
                }
                else if (system.SystemId == "CONTRACTOR")
                {
                    system.RegisterPerson(testId1, 1000m, 5);
                    system.RegisterPerson(testId2, 1500m, 4);
                }
                else if (system.SystemId == "INTERN")
                {
                    system.RegisterPerson(testId1, 500m);
                    system.RegisterPerson(testId2, 600m);
                }

                var payments = system.GetAllPayments();
                Assert.That(payments.Count, Is.EqualTo(2));
                Assert.That(payments[testId1], Is.GreaterThan(0));
                Assert.That(payments[testId2], Is.GreaterThan(0));
            }
        }

        [Test]
        public void Processors_CanWorkWithListOfAdapters()
        {
            var processor = new UnifiedPayrollProcessor();
            
            foreach (var system in _payrollSystems)
            {
                processor.RegisterPayrollSystem(system);
            }

            // Processor should handle all without error
            Assert.DoesNotThrow(() => processor.ProcessAllPayroll());
        }

        [Test]
        public void PaymentDetails_ProvidedForAllSystems()
        {
            foreach (var system in _payrollSystems)
            {
                var testId = $"TEST_{system.SystemId}";
                
                if (system.SystemId == "LEGACY")
                    system.RegisterPerson(testId, 25m, 40);
                else if (system.SystemId == "MODERN")
                    system.RegisterPerson(testId, 3000m, 500m);
                else if (system.SystemId == "CONTRACTOR")
                    system.RegisterPerson(testId, 1500m, 3);
                else
                    system.RegisterPerson(testId, 500m);

                string details = system.GetPaymentDetails(testId);
                Assert.That(details, Is.Not.Empty);
                Assert.That(details, Does.Contain("$"));
            }
        }

        [Test]
        public void Reports_FormattedConsistently()
        {
            foreach (var system in _payrollSystems)
            {
                var testId = $"TEST_{system.SystemId}";
                
                if (system.SystemId == "LEGACY")
                    system.RegisterPerson(testId, 50m, 20);
                else if (system.SystemId == "MODERN")
                    system.RegisterPerson(testId, 2000m, 200m);
                else if (system.SystemId == "CONTRACTOR")
                    system.RegisterPerson(testId, 1000m, 1);
                else
                    system.RegisterPerson(testId, 500m);

                string report = system.GenerateReport();

                // All reports should have consistent formatting
                Assert.That(report, Does.Contain("╔"));
                Assert.That(report, Does.Contain("║"));
                Assert.That(report, Does.Contain("╚"));
                Assert.That(report, Does.Contain(system.SystemName));
            }
        }

        [Test]
        public void MultipleEmployees_TrackedIndependently()
        {
            var system = _payrollSystems[0];
            
            system.RegisterPerson("E1", 25m, 40);
            system.RegisterPerson("E2", 30m, 40);
            system.RegisterPerson("E3", 35m, 40);

            var payments = system.GetAllPayments();
            
            Assert.That(payments.Count, Is.EqualTo(3));
            Assert.That(payments["E1"], Is.EqualTo(1000m));
            Assert.That(payments["E2"], Is.EqualTo(1200m));
            Assert.That(payments["E3"], Is.EqualTo(1400m));
        }

        [Test]
        public void UnifiedInterface_EliminatesCodeDuplication()
        {
            // This test validates that the same code works for all systems
            var testIds = new[] { "TEST1", "TEST2", "TEST3", "TEST4" };
            var testData = new decimal[][] 
            { 
                new[] { 25m, 40m },
                new[] { 3000m, 500m },
                new[] { 1500m, 3m },
                new[] { 500m }
            };

            for (int i = 0; i < _payrollSystems.Count; i++)
            {
                var system = _payrollSystems[i];
                system.RegisterPerson(testIds[i], testData[i]);
                
                decimal payment = system.GetTotalPayment(testIds[i]);
                Assert.That(payment, Is.GreaterThan(0));
                
                string details = system.GetPaymentDetails(testIds[i]);
                Assert.That(details, Does.Not.Contain("available"));
                
                string report = system.GenerateReport();
                Assert.That(report, Does.Contain(testIds[i]));
            }
        }
    }
}
