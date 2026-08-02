# Chain of Responsibility: Before (Anti-pattern)

## The Problem: Rigid Validation Pipeline

Without Chain of Responsibility, order validation becomes a monolithic if/else chain that is hard to extend and modify.

### Problem Analysis

**Scenario:** eCommerce platform needs to validate orders through multiple stages:
1. Check inventory availability
2. Verify payment method
3. Detect fraud patterns
4. Validate shipping address
5. Apply business rules

**Anti-Pattern Solution:** All validation in one method

```csharp
public void ValidateOrder(Order order)
{
    if (!CheckInventory(order)) throw new Exception("Out of stock");
    if (!ValidatePayment(order)) throw new Exception("Payment failed");
    if (!CheckFraud(order)) throw new Exception("Fraud detected");
    if (!ValidateShipping(order)) throw new Exception("Invalid shipping");
}
```

### Real-World Impact: $2.1M/Year

**Impact Scenario 1: Adding New Validation**
- Need to add loyalty discount check
- Must modify main ValidateOrder method
- Risk of breaking existing validations
- Takes 2-3 hours to test all combinations
- **Cost:** 100+ validation additions/year × 2.5 hours = $500K/year

**Impact Scenario 2: Reordering Validators**
- Business decides to check fraud BEFORE payment
- Must edit ValidateOrder method
- Hard to test if order matters
- Risk of regression bugs
- **Cost:** $200K/year in debugging

**Impact Scenario 3: Extending Validations**
- Payment validator needs 3 sub-checks now
- Can't reuse individual checks
- Must copy validation logic
- **Cost:** $400K/year in code duplication

**Impact Scenario 4: Testing Nightmare**
- Must test all combinations: 2^5 = 32 test cases minimum
- Adding new validator doubles test cases
- Hard to isolate which validator failed
- **Cost:** $600K/year in testing/debugging

## Code Example: The Problem

```csharp
public class Order
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public int Quantity { get; set; }
    public string PaymentMethod { get; set; }
    public string ShippingAddress { get; set; }
    public Customer Customer { get; set; }
}

// Anti-pattern: Monolithic validator
public class OrderProcessor
{
    private InventoryService _inventory;
    private PaymentService _payment;
    private FraudService _fraud;
    private ShippingService _shipping;

    public OrderProcessor()
    {
        _inventory = new InventoryService();
        _payment = new PaymentService();
        _fraud = new FraudService();
        _shipping = new ShippingService();
    }

    public void ValidateOrder(Order order)
    {
        // Problem 1: All validation logic in one method
        // Problem 2: Hard-coded validation order
        // Problem 3: Can't skip validators or reorder them
        
        Console.WriteLine($"Validating order {order.OrderId}...");

        // Step 1: Inventory check
        if (!_inventory.IsInStock(order.Quantity))
        {
            throw new InvalidOperationException($"Out of stock. Only {_inventory.GetAvailable()} available.");
        }
        Console.WriteLine("  ✓ Inventory check passed");

        // Step 2: Payment validation
        if (!_payment.IsValidPaymentMethod(order.PaymentMethod))
        {
            throw new InvalidOperationException("Invalid payment method");
        }
        if (!_payment.HasSufficientFunds(order.Customer, order.Amount))
        {
            throw new InvalidOperationException("Insufficient funds");
        }
        Console.WriteLine("  ✓ Payment check passed");

        // Step 3: Fraud detection
        if (_fraud.IsSuspiciousActivity(order.Customer, order.Amount))
        {
            throw new InvalidOperationException("Suspicious activity detected");
        }
        if (_fraud.IsBlacklisted(order.Customer))
        {
            throw new InvalidOperationException("Customer is blacklisted");
        }
        Console.WriteLine("  ✓ Fraud check passed");

        // Step 4: Shipping validation
        if (!_shipping.IsValidAddress(order.ShippingAddress))
        {
            throw new InvalidOperationException("Invalid shipping address");
        }
        if (!_shipping.CanShipToRegion(order.ShippingAddress))
        {
            throw new InvalidOperationException("Cannot ship to this region");
        }
        Console.WriteLine("  ✓ Shipping check passed");

        Console.WriteLine($"✓ Order {order.OrderId} validation complete!");
    }

    // Problems when business requirements change:

    // Problem 1: Adding a new validator
    // Must edit this method, add another if block, risk regression

    // Problem 2: Reordering validators
    // Must edit this method, reorganize if blocks

    // Problem 3: Conditional validators
    // Can't easily skip a validator based on conditions

    // Problem 4: Extending existing validator
    // Must modify existing if block, hard to maintain

    // Problem 5: Testing
    // Must test entire chain, can't test validators independently
}
```

### Problems This Creates

1. **Tight Coupling**
   - OrderProcessor knows about all validators
   - Adding new validator = modifying OrderProcessor
   - Hard to reuse validators elsewhere

2. **Hard to Extend**
   - New validation type? Add another if block
   - New business rule? Edit existing if block
   - Risk of breaking existing logic

3. **Hard to Reorder**
   - Business says fraud check should come first
   - Must edit method, reorganize if blocks
   - Hard to verify order doesn't break logic

4. **Hard to Skip**
   - Want to skip fraud check for premium customers?
   - Must add nested if blocks
   - Code becomes spaghetti

5. **Hard to Test**
   - Must test entire chain
   - Can't isolate which validator failed
   - 2^n test combinations for n validators

6. **Code Duplication**
   - Similar validation logic across different orders
   - Can't reuse validators in different contexts
   - Copy-paste violations

## Comparison: Before vs After

| Aspect | Before | After |
|--------|--------|-------|
| Validators | Hard-coded in method | Composable chain |
| Adding validator | Modify main method | Add to chain |
| Reordering | Reorganize if blocks | Change chain order |
| Testing | All or nothing | Test each handler |
| Extensibility | Low | High |
| Coupling | Tight | Loose |
| Flexibility | Fixed | Dynamic |
| Maintenance | Hard | Easy |

---

**Problem Type:** Rigid Validation Pipeline / Monolithic Logic  
**Cost Impact:** $2.1M/year in maintenance and bugs  
**Solution:** Chain of Responsibility Pattern (see After/)
