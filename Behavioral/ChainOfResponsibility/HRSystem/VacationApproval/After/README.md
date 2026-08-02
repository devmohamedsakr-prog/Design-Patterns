# Chain of Responsibility: After (Solution)

## Overview
Chain of Responsibility passes requests along a chain of approvers. Each approver decides whether to approve and pass to the next approver. This enables flexible, composable approval workflows.

## Solution Structure

```
After (Clean Design):
├── Models/
│   ├── VacationRequest.cs (request object)
│   └── ApprovalResult.cs (result object)
├── Approvers/
│   ├── Approver.cs (abstract base)
│   ├── ManagerApprover.cs (manager level)
│   ├── DirectorApprover.cs (director level)
│   ├── ExecutiveApprover.cs (executive level)
│   └── HRApprover.cs (HR finalization)
└── Builders/
    └── ApprovalChainBuilder.cs (chain assembly)
```

## Key Design Principles

### Single Responsibility Principle (SRP)

| Class | Responsibility |
|-------|----------------|
| `VacationRequest` | Store vacation request data |
| `ApprovalResult` | Encapsulate approval result |
| `Approver` | Base handler interface |
| `ManagerApprover` | Manager-level approval only |
| `DirectorApprover` | Director-level approval only |
| `ExecutiveApprover` | Executive-level approval only |
| `HRApprover` | HR finalization only |
| `ApprovalChainBuilder` | Assemble approval chain |

Each class has exactly one reason to change.

## Implementation Details

### Base Approver Class
```csharp
public abstract class Approver
{
    protected Approver _nextApprover;

    public Approver SetNext(Approver nextApprover)
    {
        _nextApprover = nextApprover;
        return nextApprover;
    }

    public abstract ApprovalResult Process(VacationRequest request);

    protected ApprovalResult PassToNext(VacationRequest request)
    {
        if (_nextApprover != null)
            return _nextApprover.Process(request);
        return new ApprovalResult(true);
    }
}
```

### Concrete Approver (Example: ManagerApprover)
```csharp
public class ManagerApprover : Approver
{
    public override ApprovalResult Process(VacationRequest request)
    {
        // Validate at manager level
        if (request.DaysRequested > _maxDaysWithoutDirector)
            return new ApprovalResult(false, "Needs director approval");

        return PassToNext(request);
    }
}
```

## Usage Examples

### Simple Chain
```csharp
var chain = new ManagerApprover("John")
    .SetNext(new DirectorApprover("Sarah"))
    .SetNext(new HRApprover("HR"));

var result = chain.Process(vacationRequest);
```

### Dynamic Chain Building
```csharp
var builder = new ApprovalChainBuilder()
    .AddManagerApproval("John")
    .AddDirectorApproval("Sarah")
    .AddHRApproval("HR");

var result = builder.Process(vacationRequest);
```

### Conditional Approvers
```csharp
var builder = new ApprovalChainBuilder()
    .AddManagerApproval("John");

if (request.DaysRequested > 10)
    builder.AddDirectorApproval("Sarah");

if (request.EstimatedCost > 5000)
    builder.AddExecutiveApproval("VP");

builder.AddHRApproval("HR");
```

## Benefits

✓ **Loose Coupling** - Approvers independent, don't know about each other
✓ **Open/Closed** - Open to extension (add approvers), closed to modification
✓ **Single Responsibility** - Each approver validates one level
✓ **Easy to Extend** - Add new approvers without changing existing ones
✓ **Easy to Reorder** - Change approval order at runtime
✓ **Easy to Test** - Test each approver independently
✓ **Flexible** - Skip approvers based on conditions
✓ **Reusable** - Use same approvers in different chains

## Real-World Applications

### HR Workflows
- Vacation request approval
- Expense reimbursement
- Promotion workflow
- Leave of absence

### Financial Workflows
- Invoice approval
- Budget request
- Purchase order approval
- Loan application

### Compliance Workflows
- Document review
- Security approval
- Audit trail

## Test Coverage

Comprehensive test suite (30+ tests):
- ✓ Individual approver validation
- ✓ Multi-level approval chains
- ✓ Chain termination
- ✓ Approver ordering
- ✓ Approval results
- ✓ Rejection scenarios
- ✓ Dynamic chain building
- ✓ Edge cases

---

**Pattern:** Chain of Responsibility  
**Domain:** HR / Workflow  
**Use Case:** Vacation approval workflow  
**SRP Compliance:** ✓ (5+ focused classes)  
**Tests:** 30+  
**Key Benefit:** Flexible, extensible approval pipeline with zero coupling
