using System;

namespace ExpenseApproval.After.Context
{
    public class ExpenseRequest
    {
        public string EmployeeId { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
        public string ApprovedBy { get; set; } = "";
    }

    public abstract class ApprovalHandler
    {
        protected ApprovalHandler _nextHandler;
        protected decimal _approvalLimit;

        public void SetNext(ApprovalHandler next) => _nextHandler = next;

        public virtual void ProcessRequest(ExpenseRequest request)
        {
            if (CanApprove(request))
            {
                Approve(request);
            }
            else if (_nextHandler != null)
            {
                Console.WriteLine($"→ Escalating to next approver");
                _nextHandler.ProcessRequest(request);
            }
            else
            {
                Console.WriteLine($"❌ Request denied - no approver available");
                request.Status = "Rejected";
            }
        }

        protected virtual bool CanApprove(ExpenseRequest request) => request.Amount <= _approvalLimit;

        protected virtual void Approve(ExpenseRequest request)
        {
            request.Status = "Approved";
            request.ApprovedBy = this.GetType().Name;
            Console.WriteLine($"✅ {this.GetType().Name} approved expense of ${request.Amount}");
        }
    }

    public class TeamLead : ApprovalHandler
    {
        public TeamLead() => _approvalLimit = 500;
    }

    public class DepartmentManager : ApprovalHandler
    {
        public DepartmentManager() => _approvalLimit = 5000;
    }

    public class Director : ApprovalHandler
    {
        public Director() => _approvalLimit = 50000;
    }

    public class VpFinance : ApprovalHandler
    {
        public VpFinance() => _approvalLimit = decimal.MaxValue;
    }

    public class ExpenseApprovalChain
    {
        private ApprovalHandler _firstApprover;

        public ExpenseApprovalChain()
        {
            var teamLead = new TeamLead();
            var manager = new DepartmentManager();
            var director = new Director();
            var vp = new VpFinance();

            teamLead.SetNext(manager);
            manager.SetNext(director);
            director.SetNext(vp);

            _firstApprover = teamLead;
        }

        public void ProcessExpense(ExpenseRequest request)
        {
            Console.WriteLine($"\n💰 Processing expense: ${request.Amount}");
            _firstApprover.ProcessRequest(request);
        }
    }
}
