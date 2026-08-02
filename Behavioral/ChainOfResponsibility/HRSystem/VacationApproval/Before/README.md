# Chain of Responsibility: Before (Anti-pattern)

## The Problem: Hard-coded Approval Workflow

Without Chain of Responsibility, vacation approval becomes a monolithic method with hard-coded approval levels.

### Problem Analysis

**Scenario:** HR system needs to process vacation requests through approval hierarchy:
1. Manager approves (checks team coverage)
2. Director approves (checks budget)
3. HR finalizes (records in system)

**Anti-Pattern Solution:** All approval logic in one method

```csharp
public void ApproveVacationRequest(VacationRequest request)
{
    // Problem: Hard-coded approval sequence
    if (!ManagerApproves(request)) throw new Exception("Manager rejected");
    if (!DirectorApproves(request)) throw new Exception("Director rejected");
    if (!HRApproves(request)) throw new Exception("HR rejected");
}
```

### Real-World Impact: $1.8M/Year

**Impact Scenario 1: Policy Changes**
- HR decides managers can't approve > 10 days alone
- Must add nested if blocks
- Must test all combinations
- **Cost:** $300K/year in policy update delays

**Impact Scenario 2: New Approval Levels**
- Add executive approval for > $10K cost
- Must edit main method
- Must integrate with existing approvers
- **Cost:** $400K/year in new workflow requests

**Impact Scenario 3: Approval Delegation**
- Manager on vacation, delegate to senior team lead
- Can't easily replace approver
- Must create manager wrapper class
- **Cost:** $350K/year in manual workarounds

**Impact Scenario 4: Process Audits**
- Need to track who approved and when
- Approval logic scattered across methods
- Hard to audit complete approval chain
- **Cost:** $450K/year in compliance overhead

## Code Example: The Problem

```csharp
public class VacationRequest
{
    public string EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DaysRequested { get; set; }
    public decimal EstimatedCost { get; set; }
    public string Reason { get; set; }
}

// Anti-pattern: Monolithic approval workflow
public class ApprovalProcessor
{
    private ManagerService _manager;
    private DirectorService _director;
    private HRService _hr;

    public ApprovalProcessor()
    {
        _manager = new ManagerService();
        _director = new DirectorService();
        _hr = new HRService();
    }

    public void ApproveVacationRequest(VacationRequest request)
    {
        // PROBLEM 1: All approval logic in one method
        // PROBLEM 2: Hard-coded approval order
        // PROBLEM 3: Can't delegate or skip approvers
        // PROBLEM 4: Hard to modify approval rules

        Console.WriteLine($"Processing vacation request for {request.EmployeeName}...");

        // Step 1: Manager approval
        if (!_manager.CanApprove(request))
        {
            throw new InvalidOperationException("Manager cannot approve this request");
        }
        if (!_manager.HasTeamCoverage(request))
        {
            throw new InvalidOperationException("Insufficient team coverage");
        }
        Console.WriteLine("  ✓ Manager approved");

        // Step 2: Director approval
        if (!_director.CanApprove(request))
        {
            throw new InvalidOperationException("Director cannot approve this request");
        }
        if (request.DaysRequested > 10)
        {
            if (!_director.HasBudget(request))
            {
                throw new InvalidOperationException("Insufficient budget for extended vacation");
            }
        }
        Console.WriteLine("  ✓ Director approved");

        // Step 3: HR approval
        if (!_hr.CanApprove(request))
        {
            throw new InvalidOperationException("HR cannot process this request");
        }
        _hr.RecordApproval(request);
        Console.WriteLine("  ✓ HR approved");

        Console.WriteLine($"✓ Vacation request for {request.EmployeeName} approved!");
    }

    // Problems when requirements change:

    // Problem 1: Adding CFO approval for large budgets
    // Must edit this method, add new if block
    // Risk of breaking existing logic

    // Problem 2: Manager is on vacation
    // Can't easily delegate to team lead
    // Must bypass or create special case

    // Problem 3: Director disapproves
    // Should still allow HR to override in special cases
    // But current logic throws exception immediately

    // Problem 4: Need approval audit trail
    // Must track each approver separately
    // Can't easily reconstruct approval chain
}
```

### Problems This Creates

1. **Hard-Coded Approval Sequence**
   - Can't reorder approvers
   - Can't skip levels
   - Can't add conditional approvers

2. **Tight Coupling**
   - ApprovalProcessor knows about all approvers
   - Adding new approver = editing ApprovalProcessor
   - Risk of breaking existing approvers

3. **Hard to Extend**
   - Need to add CFO approval? Edit method
   - Need to add compliance check? Edit method
   - Each change risks regression

4. **Hard to Delegate**
   - Manager on vacation? Can't delegate approval
   - Must create special handling code
   - Adds complexity and error-prone logic

5. **Hard to Override**
   - CFO should be able to override director
   - Can't express this in current model
   - Must add nested if blocks

6. **Hard to Test**
   - Must test entire approval chain
   - Can't test individual approvers
   - 2^n test combinations

## Comparison: Before vs After

| Aspect | Before | After |
|--------|--------|-------|
| Approvers | Hard-coded in method | Composable chain |
| Adding approver | Modify main method | Add to chain |
| Delegating approver | Must create special case | Replace in chain |
| Override capability | Hard to express | Built-in via chain |
| Testing | All or nothing | Test each approver |
| Extensibility | Low | High |
| Coupling | Tight | Loose |
| Flexibility | Fixed | Dynamic |

---

**Problem Type:** Hard-Coded Approval Workflow / Rigid Hierarchy  
**Cost Impact:** $1.8M/year in process management  
**Solution:** Chain of Responsibility Pattern (see After/)
