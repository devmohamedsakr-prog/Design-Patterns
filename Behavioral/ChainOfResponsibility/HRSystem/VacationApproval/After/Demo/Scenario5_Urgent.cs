using System;
using VacationApproval.After.Models;
using VacationApproval.After.Builders;

namespace VacationApproval.After.Demo
{
    /// <summary>
    /// Scenario 5: Urgent Request (Family Emergency)
    /// Demonstrates urgent flag handling through full approval chain
    /// </summary>
    class Scenario5_Urgent
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 5: Urgent Request (Family Emergency)");
            Console.WriteLine("  Priority: HIGH - Fast-track approval");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var builder = new ApprovalChainBuilder()
                .AddManagerApproval("John")
                .AddDirectorApproval("Sarah")
                .AddExecutiveApproval("VP Tom")
                .AddHRApproval("HR");

            var request = new VacationRequest("VAC005", "EMP005", "Iris Kumar",
                DateTime.Now.AddDays(2), DateTime.Now.AddDays(9), 7, 2000, "Family emergency")
            {
                IsUrgent = true
            };

            Console.WriteLine($"Employee: {request.EmployeeName}");
            Console.WriteLine($"Request: {request.DaysRequested} days vacation");
            Console.WriteLine($"Reason: {request.Reason}");
            Console.WriteLine($"Estimated Cost: ${request.EstimatedCost:F2}");
            Console.WriteLine($"Priority: ⚠️  URGENT");
            Console.WriteLine($"  → Fast-track approval process initiated\n");

            Console.WriteLine("Processing urgent approval chain...");
            Console.WriteLine("  All approvers notified - expedited review\n");

            var result = builder.Process(request);

            Console.WriteLine($"\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"═══════════════════════════════════════════════════════════════");
        }
    }
}
