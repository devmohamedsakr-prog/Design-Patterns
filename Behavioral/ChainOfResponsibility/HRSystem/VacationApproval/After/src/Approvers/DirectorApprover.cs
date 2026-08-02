using System;
using VacationApproval.After.Models;

namespace VacationApproval.After.Approvers
{
    /// <summary>
    /// DirectorApprover: Second approval level - validates extended leave and budget
    /// SRP: Only responsible for director-level validation
    /// </summary>
    public class DirectorApprover : Approver
    {
        private int _maxDaysDirectorApproval;
        private decimal _budgetLimit;

        public DirectorApprover(string approverName, int maxDays = 15, decimal budgetLimit = 5000) 
            : base(approverName, "Director")
        {
            _maxDaysDirectorApproval = maxDays;
            _budgetLimit = budgetLimit;
        }

        public override ApprovalResult Process(VacationRequest request)
        {
            Console.WriteLine($"  [Director {_approverName}] Reviewing extended leave...");

            // Validation 1: Extended leave check
            if (request.DaysRequested > _maxDaysDirectorApproval)
                return new ApprovalResult(false, _approverName, _approvalLevel, 
                    $"Exceeds director approval limit ({_maxDaysDirectorApproval} days)");

            // Validation 2: Budget check for high-cost requests
            if (request.EstimatedCost > _budgetLimit)
                return new ApprovalResult(false, _approverName, _approvalLevel, 
                    $"Estimated cost ${request.EstimatedCost:F2} exceeds budget limit ${_budgetLimit:F2}");

            // Validation 3: Urgent flag check
            if (request.IsUrgent && request.DaysRequested > 10)
                return new ApprovalResult(false, _approverName, _approvalLevel, 
                    "Urgent requests cannot exceed 10 days without executive approval");

            Console.WriteLine($"  ✓ Director approved (budget and timeline verified)");
            return PassToNext(request);
        }
    }
}
