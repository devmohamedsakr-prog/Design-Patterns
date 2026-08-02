using System;
using VacationApproval.After.Models;
using VacationApproval.After.Approvers;

namespace VacationApproval.After.Demo
{
    /// <summary>
    /// Scenario 4: Rejected Request
    /// Demonstrates rejection at Manager level due to exceeding limits
    /// </summary>
    class Scenario4_Rejected
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 4: Rejected Request (Too Many Days)");
            Console.WriteLine("  Rejection at: Manager Level");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var chain = new ManagerApprover("John", maxDaysWithoutDirector: 100)
                .SetNext(new DirectorApprover("Sarah", maxDays: 20))
                .SetNext(new HRApprover("HR"));

            var request = new VacationRequest("VAC004", "EMP004", "Eve Wilson",
                DateTime.Now.AddDays(10), DateTime.Now.AddDays(40), 30, 8000, "Long sabbatical");

            Console.WriteLine($"Employee: {request.EmployeeName}");
            Console.WriteLine($"Request: {request.DaysRequested} days vacation");
            Console.WriteLine($"Reason: {request.Reason}");
            Console.WriteLine($"Estimated Cost: ${request.EstimatedCost:F2}");
            Console.WriteLine($"  → Request exceeds Manager's approval limit (20 days max)\n");

            Console.WriteLine("Processing approval chain...\n");
            var result = chain.Process(request);

            Console.WriteLine($"\n═══════════════════════════════════════════════════════════════");
            if (!result.IsApproved)
            {
                Console.WriteLine($"✗ REQUEST REJECTED");
                Console.WriteLine($"Rejected by: {result.ApproverName}");
                Console.WriteLine($"Reason: {result.Comments}");
            }
            else
            {
                Console.WriteLine($"Result: {result}");
            }
            Console.WriteLine($"═══════════════════════════════════════════════════════════════");
        }
    }
}
