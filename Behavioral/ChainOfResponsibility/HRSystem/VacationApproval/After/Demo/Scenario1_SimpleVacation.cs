using System;
using VacationApproval.After.Models;
using VacationApproval.After.Approvers;

namespace VacationApproval.After.Demo
{
    /// <summary>
    /// Scenario 1: Simple Vacation Request (5 days)
    /// Demonstrates basic approval chain: Manager → HR
    /// </summary>
    class Scenario1_SimpleVacation
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Scenario 1: Simple Vacation Request (5 days)");
            Console.WriteLine("  Approval Chain: Manager → HR");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var chain = new ManagerApprover("John (Manager)")
                .SetNext(new HRApprover("HR Team"));

            var request = new VacationRequest("VAC001", "EMP001", "Alice Smith",
                DateTime.Now.AddDays(10), DateTime.Now.AddDays(15), 5, 1000, "Summer vacation");

            Console.WriteLine($"Employee: {request.EmployeeName}");
            Console.WriteLine($"Request: {request.DaysRequested} days vacation");
            Console.WriteLine($"Reason: {request.Reason}");
            Console.WriteLine($"Estimated Cost: ${request.EstimatedCost:F2}\n");

            Console.WriteLine("Processing approval chain...\n");
            var result = chain.Process(request);

            Console.WriteLine($"\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"═══════════════════════════════════════════════════════════════");
        }
    }
}
