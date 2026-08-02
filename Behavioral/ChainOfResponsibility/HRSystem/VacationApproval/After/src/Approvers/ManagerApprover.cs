using System;
using VacationApproval.After.Models;

namespace VacationApproval.After.Approvers
{
    /// <summary>
    /// ManagerApprover: First approval level - validates basic request
    /// SRP: Only responsible for manager-level validation
    /// </summary>
    public class ManagerApprover : Approver
    {
        private int _maxDaysWithoutDirector;

        public ManagerApprover(string approverName, int maxDaysWithoutDirector = 5) 
            : base(approverName, "Manager")
        {
            _maxDaysWithoutDirector = maxDaysWithoutDirector;
        }

        public override ApprovalResult Process(VacationRequest request)
        {
            Console.WriteLine($"  [Manager {_approverName}] Reviewing request...");

            // Validation 1: Basic request validation
            if (request.DaysRequested <= 0)
                return new ApprovalResult(false, _approverName, _approvalLevel, "Invalid days requested");

            // Validation 2: Team coverage check
            if (request.DaysRequested > 20)
                return new ApprovalResult(false, _approverName, _approvalLevel, "Exceeds manager approval limit (20 days)");

            // Validation 3: Immediate rejection threshold
            if (request.DaysRequested <= _maxDaysWithoutDirector)
            {
                Console.WriteLine($"  ✓ Manager approved (≤ {_maxDaysWithoutDirector} days, auto-approved)");
                return PassToNext(request);
            }

            Console.WriteLine($"  ✓ Manager approved (passes to Director)");
            return PassToNext(request);
        }
    }
}
