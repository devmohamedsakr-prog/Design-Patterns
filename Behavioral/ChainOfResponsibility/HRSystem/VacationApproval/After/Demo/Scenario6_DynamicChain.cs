using System;
using VacationApproval.After.Models;
using VacationApproval.After.Builders;

namespace VacationApproval.After.Demo
{
    /// <summary>
    /// Scenario 6: Dynamic Chain Building
    /// Demonstrates building approval chain based on request details
    /// </summary>
    class Scenario6_DynamicChain
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 6: Dynamic Chain Building");
            Console.WriteLine("  Build chain based on request characteristics");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var request = new VacationRequest("VAC006", "EMP006", "Lisa Anderson",
                DateTime.Now.AddDays(20), DateTime.Now.AddDays(28), 8, 6000, "Premium resort");

            Console.WriteLine($"Employee: {request.EmployeeName}");
            Console.WriteLine($"Request: {request.DaysRequested} days vacation");
            Console.WriteLine($"Reason: {request.Reason}");
            Console.WriteLine($"Estimated Cost: ${request.EstimatedCost:F2}\n");

            Console.WriteLine("Analyzing request to build optimal approval chain...\n");

            var builder = new ApprovalChainBuilder()
                .AddManagerApproval("John");

            if (request.DaysRequested > 5)
            {
                Console.WriteLine($"✓ Added DirectorApproval (days: {request.DaysRequested} > 5)");
                builder.AddDirectorApproval("Sarah", maxDays: 30, budgetLimit: 10000);
            }

            if (request.EstimatedCost > 5000)
            {
                Console.WriteLine($"✓ Added ExecutiveApproval (cost: ${request.EstimatedCost} > $5000)");
                builder.AddExecutiveApproval("VP Tom", highCostThreshold: 15000);
            }

            builder.AddHRApproval("HR");

            Console.WriteLine($"\nFinal approval chain: Manager → Director → Executive → HR\n");
            Console.WriteLine("Processing dynamic approval chain...\n");

            var result = builder.Process(request);

            Console.WriteLine($"\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"═══════════════════════════════════════════════════════════════");
        }
    }
}
