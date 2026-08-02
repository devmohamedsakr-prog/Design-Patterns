using System;
using VacationApproval.After.Models;

namespace VacationApproval.After.Approvers
{
    /// <summary>
    /// ExecutiveApprover: Optional approval level - validates high-value or urgent requests
    /// SRP: Only responsible for executive-level validation
    /// </summary>
    public class ExecutiveApprover : Approver
    {
        private int _maxUrgentDays;
        private decimal _highCostThreshold;

        public ExecutiveApprover(string approverName, int maxUrgentDays = 20, decimal highCostThreshold = 10000) 
            : base(approverName, "Executive")
        {
            _maxUrgentDays = maxUrgentDays;
            _highCostThreshold = highCostThreshold;
        }

        public override ApprovalResult Process(VacationRequest request)
        {
            Console.WriteLine($"  [Executive {_approverName}] Reviewing high-priority request...");

            // Validation 1: High-cost request
            if (request.EstimatedCost > _highCostThreshold)
                return new ApprovalResult(false, _approverName, _approvalLevel, 
                    $"High-cost request ${request.EstimatedCost:F2} requires CFO approval, not available");

            // Validation 2: Urgent request time limit
            if (request.IsUrgent && request.DaysRequested > _maxUrgentDays)
                return new ApprovalResult(false, _approverName, _approvalLevel, 
                    $"Urgent request cannot exceed {_maxUrgentDays} days");

            Console.WriteLine($"  ✓ Executive approved (high-priority request validated)");
            return PassToNext(request);
        }
    }
}
