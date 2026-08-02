using VacationApproval.After.Approvers;
using VacationApproval.After.Models;

namespace VacationApproval.After.Builders
{
    /// <summary>
    /// ApprovalChainBuilder: Fluent builder for constructing approval chains
    /// SRP: Only responsible for assembling approver chains
    /// </summary>
    public class ApprovalChainBuilder
    {
        private Approver _firstApprover;
        private Approver _lastApprover;

        public ApprovalChainBuilder AddManagerApproval(string managerName, int maxDaysWithoutDirector = 5)
        {
            var approver = new ManagerApprover(managerName, maxDaysWithoutDirector);
            AppendApprover(approver);
            return this;
        }

        public ApprovalChainBuilder AddDirectorApproval(string directorName, int maxDays = 15, decimal budgetLimit = 5000)
        {
            var approver = new DirectorApprover(directorName, maxDays, budgetLimit);
            AppendApprover(approver);
            return this;
        }

        public ApprovalChainBuilder AddExecutiveApproval(string executiveName, int maxUrgentDays = 20, decimal highCostThreshold = 10000)
        {
            var approver = new ExecutiveApprover(executiveName, maxUrgentDays, highCostThreshold);
            AppendApprover(approver);
            return this;
        }

        public ApprovalChainBuilder AddHRApproval(string hrName)
        {
            var approver = new HRApprover(hrName);
            AppendApprover(approver);
            return this;
        }

        public ApprovalChainBuilder AddApprover(Approver approver)
        {
            AppendApprover(approver);
            return this;
        }

        private void AppendApprover(Approver approver)
        {
            if (_firstApprover == null)
            {
                _firstApprover = approver;
                _lastApprover = approver;
            }
            else
            {
                _lastApprover.SetNext(approver);
                _lastApprover = approver;
            }
        }

        public Approver Build()
        {
            if (_firstApprover == null)
                throw new InvalidOperationException("Approval chain must have at least one approver");
            
            return _firstApprover;
        }

        public ApprovalResult Process(VacationRequest request)
        {
            var chain = Build();
            return chain.Process(request);
        }
    }
}
