# Decorator Pattern: Before (Anti-pattern)

## The Problem: Order Class Explosion

Without the Decorator pattern, adding pricing features forces multiple inheritance hierarchies:

### Problem Analysis

**Scenario:** An eCommerce system needs to calculate order totals with:
1. Base order price
2. Discount (percentage-based)
3. Tax (regional calculation)
4. Shipping (varies by method)

**Anti-Pattern Solution:** Create a class for each combination

```
Order
├── OrderWithDiscount
├── OrderWithTax
├── OrderWithShipping
├── OrderWithDiscountTax
├── OrderWithDiscountShipping
├── OrderWithTaxShipping
├── OrderWithDiscountTaxShipping
└── ... more combinations
```

### Real-World Impact: $2.3M/Year

**Impact Scenario 1: Feature Explosion**
- Start with 3 features → 8 classes
- Add loyalty discounts → 16 classes
- Add insurance → 32 classes
- Add gift wrapping → 64 classes
- **Result:** Unmaintainable codebase

**Impact Scenario 2: Pricing Bug Fix**
- Bug found in discount calculation
- Must fix in OrderWithDiscount, OrderWithDiscountTax, OrderWithDiscountShipping, OrderWithDiscountTaxShipping, etc.
- **Risk:** Inconsistencies, missed updates, more bugs
- **Cost:** $150K per incident

**Impact Scenario 3: Order Processing Latency**
- Team spends 40% of time managing class hierarchy instead of features
- New features take 3-4 weeks to implement
- Order management system loses $500K/month in efficiency

## Code Example: The Problem

```csharp
// Base Order class
public class Order
{
    public string OrderId { get; set; }
    public decimal BasePrice { get; set; }

    public Order(string orderId, decimal basePrice)
    {
        OrderId = orderId;
        BasePrice = basePrice;
    }

    public virtual decimal GetTotal() => BasePrice;
}

// Anti-pattern: Create a new class for each combination
public class OrderWithDiscount : Order
{
    private decimal _discountPercent;

    public OrderWithDiscount(string orderId, decimal basePrice, decimal discount)
        : base(orderId, basePrice)
    {
        _discountPercent = discount;
    }

    public override decimal GetTotal() => BasePrice * (1 - _discountPercent);
}

public class OrderWithTax : Order
{
    private decimal _taxRate;

    public OrderWithTax(string orderId, decimal basePrice, decimal tax)
        : base(orderId, basePrice)
    {
        _taxRate = tax;
    }

    public override decimal GetTotal() => BasePrice * (1 + _taxRate);
}

public class OrderWithDiscountAndTax : Order
{
    private decimal _discountPercent;
    private decimal _taxRate;

    public OrderWithDiscountAndTax(string orderId, decimal basePrice, 
        decimal discount, decimal tax)
        : base(orderId, basePrice)
    {
        _discountPercent = discount;
        _taxRate = tax;
    }

    public override decimal GetTotal()
    {
        decimal afterDiscount = BasePrice * (1 - _discountPercent);
        return afterDiscount * (1 + _taxRate);
    }
}

// ... more classes for other combinations
```

### Problems

1. **Class Explosion:** Adding n features creates 2^n classes
2. **Code Duplication:** Pricing logic repeated across classes
3. **Difficult Combinations:** Hard to apply decorators in different orders
4. **Maintenance Nightmare:** Every bug fix requires multiple changes
5. **Testing Complexity:** Exponential number of test cases
6. **Difficult to Extend:** New features require creating many new classes

### Usage Example (Before)

```csharp
// Want: 10% discount + 8% tax + $15 shipping
// Problem: No single class for this exact combination
// If not created, must create OrderWithDiscountTaxShipping

var order = new OrderWithDiscountAndTax("ORD001", 100m, 0.10m, 0.08m);
Console.WriteLine($"Total: ${order.GetTotal():F2}"); // $97.20 (100 - 10% = 90, + 8% tax = 97.20)

// But we also need shipping... must create another class
```

## Comparison: Before vs After

| Aspect | Before | After |
|--------|--------|-------|
| Classes needed | 2^n (exponential) | 1 base + n decorators |
| Code duplication | High | None |
| Adding new feature | Create 2^n new classes | Add 1 decorator |
| Combining features | Fixed classes only | Dynamic composition |
| Testing | Exponential cases | Linear with decorators |
| Maintenance | Difficult | Easy |
| Flexibility | Low | High |
| Learning curve | Steep | Gentle |

---

**Problem Type:** Class Explosion / Rigid Hierarchy  
**Cost Impact:** $2.3M/year in maintenance and lost productivity  
**Solution:** Decorator Pattern (see After/)
