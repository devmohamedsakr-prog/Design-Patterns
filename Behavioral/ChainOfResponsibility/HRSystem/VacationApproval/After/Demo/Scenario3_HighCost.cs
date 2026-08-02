using System;
using VacationApproval.After.Models;
using VacationApproval.After.Builders;

namespace VacationApproval.After.Demo
{
    /// <summary>
    /// Scenario 3: High-Cost Request (needs Executive)
    /// Demonstrates full chain: Manager → Director → Executive → HR
    /// </summary>
    class Scenario3_HighCost
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 3: High-Cost Request (needs Executive)");
            Console.WriteLine("  Approval Chain: Manager → Director → Executive → HR");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var builder = new ApprovalChainBuilder()
                .AddManagerApproval("John")
                .AddDirectorApproval("Sarah", maxDays: 30, budgetLimit: 15000)
                .AddExecutiveApproval("VP Tom", highCostThreshold: 20000)
                .AddHRApproval("HR");

            var request = new VacationRequest("VAC003", "EMP003", "David Lee",
                DateTime.Now.AddDays(25), DateTime.Now.AddDays(40), 15, 12000, "Executive conference + vacation");

            Console.WriteLine($"Employee: {request.EmployeeName}");
            Console.WriteLine($"Request: {request.DaysRequested} days vacation");
            Console.WriteLine($"Reason: {request.Reason}");
            Console.WriteLine($"Estimated Cost: ${request.EstimatedCost:F2}");
            Console.WriteLine($"  → High cost requires Executive approval\n");

            Console.WriteLine("Processing approval chain...");
            Console.WriteLine("  Manager level → Director level → Executive level → HR finalization\n");

            var result = builder.Process(request);

            Console.WriteLine($"\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"═══════════════════════════════════════════════════════════════");
        }
    }
}
