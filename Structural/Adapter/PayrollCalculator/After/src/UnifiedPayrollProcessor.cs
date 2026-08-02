using System;
using System.Collections.Generic;

namespace PayrollCalculator.After
{
    /// <summary>
    /// Unified payroll processor that works with any IPayrollSystem.
    /// SRP: Single Responsibility - Process payroll uniformly across all systems
    /// </summary>
    public class UnifiedPayrollProcessor
    {
        private List<IPayrollSystem> _payrollSystems = new();

        public void RegisterPayrollSystem(IPayrollSystem system)
        {
            _payrollSystems.Add(system);
            Console.WriteLine($"✅ Registered: {system.SystemName}");
        }

        public void ProcessAllPayroll()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║         AFTER: Unified Interface via Adapter Pattern       ║");
            Console.WriteLine("║         (All systems conform to IPayrollSystem)            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");

            Console.WriteLine("\n┌─ REGISTERING PEOPLE ACROSS ALL SYSTEMS ─┐\n");

            foreach (var system in _payrollSystems)
            {
                Console.WriteLine($"\n📝 {system.SystemName}:");
                switch (system.SystemId)
                {
                    case "LEGACY":
                        system.RegisterPerson("EMP001", 25.00m, 40);
                        system.RegisterPerson("EMP002", 30.00m, 40);
                        break;

                    case "MODERN":
                        system.RegisterPerson("EMP003", 3000m, 500m);
                        system.RegisterPerson("EMP004", 4000m, 800m);
                        break;

                    case "CONTRACTOR":
                        system.RegisterPerson("CONT001", 1500m, 3);
                        system.RegisterPerson("CONT002", 2000m, 5);
                        break;

                    case "INTERN":
                        system.RegisterPerson("INTERN001", 500m);
                        system.RegisterPerson("INTERN002", 600m);
                        break;
                }
            }

            Console.WriteLine("\n┌─ UNIFIED REPORTING ─┐");
            foreach (var system in _payrollSystems)
            {
                Console.WriteLine(system.GenerateReport());
            }

            Console.WriteLine("\n┌─ PAYMENT SUMMARY (Unified Approach) ─┐\n");
            decimal totalPayroll = 0;
            foreach (var system in _payrollSystems)
            {
                Console.WriteLine($"📊 {system.SystemName}:");
                var payments = system.GetAllPayments();
                foreach (var kvp in payments)
                {
                    Console.WriteLine($"   └─ {kvp.Key}: ${kvp.Value:F2}");
                    totalPayroll += kvp.Value;
                }
            }

            Console.WriteLine($"\n💰 Total Payroll: ${totalPayroll:F2}");

            Console.WriteLine("\n┌─ POLYMORPHIC PROCESSING ─┐\n");
            Console.WriteLine("Processing all systems uniformly:");
            foreach (var system in _payrollSystems)
            {
                Console.WriteLine($"\n   System: {system.SystemName} (ID: {system.SystemId})");
                var allPayments = system.GetAllPayments();
                decimal systemTotal = 0;
                foreach (var payment in allPayments.Values)
                {
                    systemTotal += payment;
                }
                Console.WriteLine($"   Total System Payroll: ${systemTotal:F2}");
            }

            DisplayBenefits();
        }

        private void DisplayBenefits()
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 ADAPTER PATTERN BENEFITS                   ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ ✅ Single unified interface for all payroll systems        ║");
            Console.WriteLine("║ ✅ Polymorphic processing - same code works for all       ║");
            Console.WriteLine("║ ✅ Client code is simple and focused                       ║");
            Console.WriteLine("║ ✅ Easy to add new payroll systems                         ║");
            Console.WriteLine("║ ✅ No code duplication                                     ║");
            Console.WriteLine("║ ✅ Loose coupling - systems don't know about each other   ║");
            Console.WriteLine("║ ✅ Standard reporting across all systems                   ║");
            Console.WriteLine("║ ✅ Uniform payment processing                              ║");
            Console.WriteLine("║ ✅ Easy to test with mocks and stubs                       ║");
            Console.WriteLine("║ ✅ Changes to one system don't affect others               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        }
    }
}
