using System;
using VacationApproval.After.Models;

namespace VacationApproval.After.Approvers
{
    /// <summary>
    /// HRApprover: Final approval level - records approval in system
    /// SRP: Only responsible for HR finalization
    /// </summary>
    public class HRApprover : Approver
    {
        public HRApprover(string approverName) : base(approverName, "HR")
        {
        }

        public override ApprovalResult Process(VacationRequest request)
        {
            Console.WriteLine($"  [HR {_approverName}] Finalizing approval...");

            // Final validation
            if (string.IsNullOrEmpty(request.RequestId))
                return new ApprovalResult(false, _approverName, _approvalLevel, "Request ID required for recording");

            // Record in system (simulated)
            Console.WriteLine($"  ✓ Recording in HR system...");
            Console.WriteLine($"  ✓ Sending confirmation email to {request.EmployeeName}");

            // Return final approval
            return new ApprovalResult(true, _approverName, _approvalLevel, "Vacation recorded in system");
        }
    }
}
