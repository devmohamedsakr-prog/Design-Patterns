namespace VacationApproval.After.Models
{
    /// <summary>
    /// ApprovalResult: Result object from approval chain
    /// SRP: Only encapsulates approval result and audit trail
    /// </summary>
    public class ApprovalResult
    {
        public bool IsApproved { get; set; }
        public string ApproverName { get; set; }
        public string ApprovalLevel { get; set; }
        public string Comments { get; set; }

        public ApprovalResult(bool isApproved, string approverName = "", string approvalLevel = "", string comments = "")
        {
            IsApproved = isApproved;
            ApproverName = approverName;
            ApprovalLevel = approvalLevel;
            Comments = comments;
        }

        public override string ToString() =>
            IsApproved ? $"✓ Approved by {ApproverName} ({ApprovalLevel})" : 
            $"✗ Rejected by {ApproverName} ({ApprovalLevel}): {Comments}";
    }
}
