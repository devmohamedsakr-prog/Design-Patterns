using System;

namespace VacationApproval.Before
{
    // BEFORE: Anti-pattern - Hard-coded approval workflow
    // All approval logic in one method

    public class VacationRequest
    {
        public string RequestId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLevel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRequested { get; set; }
        public decimal EstimatedCost { get; set; }
        public string Reason { get; set; }
    }

    public class ApprovalProcessor
    {
        public void ApproveVacationRequest(VacationRequest request)
        {
            Console.WriteLine($"\nProcessing vacation request for {request.EmployeeName}...");

            // PROBLEM 1: All approval logic in one method
            // PROBLEM 2: Hard-coded approval order
            // PROBLEM 3: Can't delegate or skip approvers
            // PROBLEM 4: Hard to modify approval rules

            // Step 1: Manager approval
            if (string.IsNullOrEmpty(request.EmployeeId))
            {
                throw new InvalidOperationException("Employee ID required");
            }
            if (request.DaysRequested <= 0)
            {
                throw new InvalidOperationException("Invalid days requested");
            }
            if (request.DaysRequested > 30)
            {
                throw new InvalidOperationException("Manager cannot approve > 30 days");
            }
            Console.WriteLine("  ✓ Manager approved (checked team coverage)");

            // Step 2: Director approval
            if (request.DaysRequested > 10)
            {
                if (request.EstimatedCost > 5000)
                {
                    throw new InvalidOperationException("Director cannot approve > $5000 cost");
                }
                Console.WriteLine("  ✓ Director approved (checked budget for extended leave)");
            }
            else
            {
                Console.WriteLine("  ✓ Director pre-approved (< 10 days, auto-approved)");
            }

            // Step 3: HR approval
            if (request.EstimatedCost < 0)
            {
                throw new InvalidOperationException("Invalid cost estimate");
            }
            Console.WriteLine("  ✓ HR approved (recorded in system)");

            Console.WriteLine($"✓ Vacation request for {request.EmployeeName} approved!");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("  Chain of Responsibility: BEFORE (Anti-pattern)");
            Console.WriteLine("  Hard-Coded Approval Workflow");
            Console.WriteLine("════════════════════════════════════════════════════════════════\n");

            var processor = new ApprovalProcessor();

            // Test 1: Valid vacation request
            Console.WriteLine("--- Test 1: Valid Vacation Request (5 days) ---");
            try
            {
                var request1 = new VacationRequest
                {
                    RequestId = "VAC001",
                    EmployeeId = "EMP001",
                    EmployeeName = "John Smith",
                    EmployeeLevel = "Developer",
                    StartDate = DateTime.Now.AddDays(10),
                    EndDate = DateTime.Now.AddDays(15),
                    DaysRequested = 5,
                    EstimatedCost = 1000,
                    Reason = "Summer vacation"
                };
                processor.ApproveVacationRequest(request1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Request rejected: {ex.Message}");
            }

            // Test 2: Extended leave
            Console.WriteLine("\n--- Test 2: Extended Leave (15 days, $3000) ---");
            try
            {
                var request2 = new VacationRequest
                {
                    RequestId = "VAC002",
                    EmployeeId = "EMP002",
                    EmployeeName = "Jane Doe",
                    EmployeeLevel = "Manager",
                    StartDate = DateTime.Now.AddDays(20),
                    EndDate = DateTime.Now.AddDays(35),
                    DaysRequested = 15,
                    EstimatedCost = 3000,
                    Reason = "International trip"
                };
                processor.ApproveVacationRequest(request2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Request rejected: {ex.Message}");
            }

            // Test 3: Too many days
            Console.WriteLine("\n--- Test 3: Too Many Days (40 days) ---");
            try
            {
                var request3 = new VacationRequest
                {
                    RequestId = "VAC003",
                    EmployeeId = "EMP003",
                    EmployeeName = "Bob Johnson",
                    EmployeeLevel = "Developer",
                    StartDate = DateTime.Now.AddDays(30),
                    EndDate = DateTime.Now.AddDays(70),
                    DaysRequested = 40,
                    EstimatedCost = 8000,
                    Reason = "Sabbatical"
                };
                processor.ApproveVacationRequest(request3);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Request rejected: {ex.Message}");
            }

            // Test 4: High cost
            Console.WriteLine("\n--- Test 4: High Cost (15 days, $8000) ---");
            try
            {
                var request4 = new VacationRequest
                {
                    RequestId = "VAC004",
                    EmployeeId = "EMP004",
                    EmployeeName = "Alice Williams",
                    EmployeeLevel = "Senior Engineer",
                    StartDate = DateTime.Now.AddDays(25),
                    EndDate = DateTime.Now.AddDays(40),
                    DaysRequested = 15,
                    EstimatedCost = 8000,
                    Reason = "World trip"
                };
                processor.ApproveVacationRequest(request4);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Request rejected: {ex.Message}");
            }

            // Show the problem
            Console.WriteLine("\n════════════════════════════════════════════════════════════════");
            Console.WriteLine("  THE PROBLEMS WITH THIS APPROACH");
            Console.WriteLine("════════════════════════════════════════════════════════════════");
            Console.WriteLine("✗ All approval logic in ONE method (ApproveVacationRequest)");
            Console.WriteLine("✗ Hard-coded approval sequence - can't reorder");
            Console.WriteLine("✗ Adding new approver? Must edit this method");
            Console.WriteLine("✗ Manager on vacation? Can't delegate approval");
            Console.WriteLine("✗ Need CFO override? Must add nested if blocks");
            Console.WriteLine("✗ Can't test individual approvers");
            Console.WriteLine("✗ Tight coupling between processor and all approvers");
            Console.WriteLine("✗ Each policy change requires method modification");
            Console.WriteLine();
            Console.WriteLine("SOLUTION: Use Chain of Responsibility Pattern!");
            Console.WriteLine("- Each approver is independent handler");
            Console.WriteLine("- Chain them together dynamically");
            Console.WriteLine("- Easy to add, remove, or reorder handlers");
            Console.WriteLine("- Easy to delegate or override");
            Console.WriteLine("- Test each handler independently");
        }
    }
}
