using System;

namespace VacationApproval.After.Models
{
    /// <summary>
    /// VacationRequest: Request object flowing through approval chain
    /// SRP: Only stores vacation request data
    /// </summary>
    public class VacationRequest
    {
        public string RequestId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeLevel { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRequested { get; set; }
        public decimal EstimatedCost { get; set; }
        public string Reason { get; set; }
        public bool IsUrgent { get; set; }

        public VacationRequest(string requestId, string employeeId, string employeeName,
            DateTime startDate, DateTime endDate, int daysRequested, decimal estimatedCost, string reason = "")
        {
            RequestId = requestId;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
            StartDate = startDate;
            EndDate = endDate;
            DaysRequested = daysRequested;
            EstimatedCost = estimatedCost;
            Reason = reason;
            IsUrgent = false;
        }

        public override string ToString() =>
            $"Request {RequestId} | Employee: {EmployeeName} | Days: {DaysRequested} | Cost: ${EstimatedCost:F2}";
    }
}
