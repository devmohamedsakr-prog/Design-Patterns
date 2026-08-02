# Chain of Responsibility: After (Solution)

## Overview
Chain of Responsibility passes requests along a chain of handlers. Each handler decides whether to process or pass to the next handler. This enables flexible, composable validation pipelines.

## Solution Structure

```
After (Clean Design):
├── Models/
│   ├── Order.cs (request object)
│   └── ValidationResult.cs (result object)
├── Handlers/
│   ├── ValidationHandler.cs (abstract base)
│   ├── InventoryHandler.cs (inventory validation)
│   ├── PaymentHandler.cs (payment validation)
│   ├── FraudHandler.cs (fraud detection)
│   └── ShippingHandler.cs (shipping validation)
└── Builders/
    └── ValidationChainBuilder.cs (chain assembly)
```

## Key Design Principles

### Single Responsibility Principle (SRP)

| Class | Responsibility | Dependencies |
|-------|----------------|--------------|
| `Order` | Store order data | None |
| `ValidationResult` | Encapsulate validation result | None |
| `ValidationHandler` | Base handler interface | None |
| `InventoryHandler` | Check inventory only | ValidationHandler |
| `PaymentHandler` | Check payment only | ValidationHandler |
| `FraudHandler` | Detect fraud only | ValidationHandler |
| `ShippingHandler` | Validate shipping only | ValidationHandler |
| `ValidationChainBuilder` | Assemble validation chain | ValidationHandler |

Each class has exactly one reason to change.

### Composition Over Inheritance

**Before (Inheritance):**
```csharp
public class OrderValidator
{
    public void ValidateOrder(Order order) { }  // Rigid
}
```

**After (Composition):**
```csharp
var chain = new ValidationChainBuilder()
    .AddInventoryCheck()
    .AddPaymentCheck()
    .AddFraudCheck()
    .AddShippingCheck()
    .Build();

chain.Handle(order);  // Flexible chaining
```

## Implementation Details

### Base Handler Class
```csharp
public abstract class ValidationHandler
{
    protected ValidationHandler _nextHandler;

    public ValidationHandler SetNext(ValidationHandler nextHandler)
    {
        _nextHandler = nextHandler;
        return nextHandler;  // Chain pattern
    }

    public abstract ValidationResult Handle(Order order);
}
```

### Concrete Handler (Example: InventoryHandler)
```csharp
public class InventoryHandler : ValidationHandler
{
    public override ValidationResult Handle(Order order)
    {
        // Validate inventory
        if (order.Quantity <= 0)
            return new ValidationResult(false, "Invalid quantity");

        // Pass to next handler
        if (_nextHandler != null)
            return _nextHandler.Handle(order);

        // Chain complete
        return new ValidationResult(true, "All validations passed");
    }
}
```

## Usage Examples

### Simple Chain
```csharp
var chain = new InventoryHandler()
    .SetNext(new PaymentHandler())
    .SetNext(new FraudHandler())
    .SetNext(new ShippingHandler());

var result = chain.Handle(order);
Console.WriteLine(result.IsValid ? "✓ Order approved" : $"✗ {result.ErrorMessage}");
```

### Dynamic Chain Building
```csharp
var builder = new ValidationChainBuilder();
builder.AddInventoryCheck();

if (order.Amount > 5000)
    builder.AddFraudCheck();

if (!isPremiumCustomer)
    builder.AddPaymentCheck();

builder.AddShippingCheck();
var chain = builder.Build();
```

### Custom Chain Order
```csharp
// Different business rule: Check fraud BEFORE payment
var chain = new FraudHandler()
    .SetNext(new PaymentHandler())
    .SetNext(new InventoryHandler())
    .SetNext(new ShippingHandler());
```

## Benefits

✓ **Loose Coupling** - Handlers independent, don't know about each other
✓ **Open/Closed** - Open to extension (add handlers), closed to modification
✓ **Single Responsibility** - Each handler validates one concern
✓ **Easy to Extend** - Add new handlers without changing existing ones
✓ **Easy to Reorder** - Change validation order at runtime
✓ **Easy to Test** - Test each handler independently
✓ **Flexible** - Skip handlers based on conditions
✓ **Reusable** - Use same handlers in different chains

## Comparison: Before vs After

| Metric | Before | After |
|--------|--------|-------|
| Classes | 1 monolithic | 5 focused |
| Adding validator | Edit main method | Add to chain |
| Reordering | Edit main method | Rearrange chain |
| Extensibility | Low | High |
| Testing | All or nothing | Isolated tests |
| Coupling | Tight | Loose |
| Flexibility | Fixed | Dynamic |
| Code Reuse | None | High |

## Design Patterns Used

1. **Chain of Responsibility** - Core pattern for handler chaining
2. **Builder** - ValidationChainBuilder for flexible chain assembly
3. **Strategy** - Each handler encapsulates validation strategy
4. **Template Method** - Base handler provides validation template

## Real-World Applications

### E-Commerce
- Order validation chains
- Approval workflows
- Request processing pipelines

### Finance
- Expense approval chains
- Credit authorization
- Transaction verification

### Support Systems
- Ticket routing
- Escalation chains
- Request handling

## Test Coverage

Comprehensive test suite (25+ tests):
- ✓ Single handler execution
- ✓ Multiple handler chains
- ✓ Chain termination
- ✓ Handler ordering
- ✓ Validation results
- ✓ Error handling
- ✓ Dynamic chain building
- ✓ Edge cases

## Architecture Diagram

```
Request (Order)
    ↓
┌──────────────────────────┐
│  InventoryHandler        │
│  - Validate quantity     │
│  - Pass to next or fail  │
└──────────────────────────┘
    ↓ (if valid)
┌──────────────────────────┐
│  PaymentHandler          │
│  - Validate payment      │
│  - Pass to next or fail  │
└──────────────────────────┘
    ↓ (if valid)
┌──────────────────────────┐
│  FraudHandler            │
│  - Detect fraud          │
│  - Pass to next or fail  │
└──────────────────────────┘
    ↓ (if valid)
┌──────────────────────────┐
│  ShippingHandler         │
│  - Validate shipping     │
│  - Return result         │
└──────────────────────────┘
    ↓
Result (ValidationResult)
```

---

**Pattern:** Chain of Responsibility  
**Domain:** eCommerce  
**Use Case:** Order validation chain  
**SRP Compliance:** ✓ (5 focused classes)  
**Tests:** 25+  
**Key Benefit:** Flexible, extensible validation pipeline with zero coupling
