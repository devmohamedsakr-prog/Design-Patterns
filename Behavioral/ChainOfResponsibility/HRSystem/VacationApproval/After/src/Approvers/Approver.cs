using VacationApproval.After.Models;

namespace VacationApproval.After.Approvers
{
    /// <summary>
    /// Approver: Abstract base for approval chain handlers
    /// SRP: Provides chain interface for approval process
    /// </summary>
    public abstract class Approver
    {
        protected Approver _nextApprover;
        protected string _approverName;
        protected string _approvalLevel;

        public Approver(string approverName, string approvalLevel)
        {
            _approverName = approverName;
            _approvalLevel = approvalLevel;
        }

        /// <summary>
        /// Set the next approver in the chain
        /// </summary>
        public Approver SetNext(Approver nextApprover)
        {
            _nextApprover = nextApprover;
            return nextApprover;
        }

        /// <summary>
        /// Process approval and pass to next approver
        /// </summary>
        public abstract ApprovalResult Process(VacationRequest request);

        /// <summary>
        /// Call next approver or return success
        /// </summary>
        protected ApprovalResult PassToNext(VacationRequest request)
        {
            if (_nextApprover != null)
                return _nextApprover.Process(request);

            return new ApprovalResult(true, _approverName, _approvalLevel, "Final approval granted");
        }
    }
}
