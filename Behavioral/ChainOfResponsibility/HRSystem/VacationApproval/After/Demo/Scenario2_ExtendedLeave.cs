using System;
using VacationApproval.After.Models;
using VacationApproval.After.Approvers;
using VacationApproval.After.Builders;

namespace VacationApproval.After.Demo
{
    /// <summary>
    /// Scenario 2: Extended Leave (10 days, needs Director)
    /// Demonstrates multi-level approval: Manager → Director → HR
    /// </summary>
    class Scenario2_ExtendedLeave
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 2: Extended Leave (10 days)");
            Console.WriteLine("  Approval Chain: Manager → Director → HR");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var builder = new ApprovalChainBuilder()
                .AddManagerApproval("John", maxDaysWithoutDirector: 3)
                .AddDirectorApproval("Sarah", maxDays: 20, budgetLimit: 8000)
                .AddHRApproval("HR");

            var request = new VacationRequest("VAC002", "EMP002", "Carol Davis",
                DateTime.Now.AddDays(20), DateTime.Now.AddDays(30), 10, 3500, "International trip");

            Console.WriteLine($"Employee: {request.EmployeeName}");
            Console.WriteLine($"Request: {request.DaysRequested} days vacation");
            Console.WriteLine($"Reason: {request.Reason}");
            Console.WriteLine($"Estimated Cost: ${request.EstimatedCost:F2}\n");

            Console.WriteLine("Processing approval chain...");
            Console.WriteLine("  Manager level (max 3 days without director approval)\n");

            var result = builder.Process(request);

            Console.WriteLine($"\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"═══════════════════════════════════════════════════════════════");
        }
    }
}
