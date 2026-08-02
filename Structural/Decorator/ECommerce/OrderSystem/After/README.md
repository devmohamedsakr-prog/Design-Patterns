# Decorator Pattern: After (Solution)

## Overview
The Decorator Pattern solves the class explosion problem by using composition instead of inheritance. Each pricing feature is a separate decorator that wraps the order.

## Solution Structure

```
After (Clean Design):
├── Models/
│   └── Order.cs (base order, single responsibility)
├── Decorators/
│   ├── OrderDecorator.cs (abstract base)
│   ├── DiscountDecorator.cs (discount logic only)
│   ├── TaxDecorator.cs (tax logic only)
│   ├── ShippingDecorator.cs (shipping logic only)
│   └── InsuranceDecorator.cs (insurance logic only)
```

## Key Design Principles

### Single Responsibility Principle (SRP)

| Class | Responsibility | Dependencies |
|-------|----------------|--------------|
| `Order` | Store order identity and base price | None |
| `OrderDecorator` | Provide base decorator interface | Order |
| `DiscountDecorator` | Apply percentage discounts | Order |
| `TaxDecorator` | Apply tax calculations | Order |
| `ShippingDecorator` | Add shipping costs | Order |
| `InsuranceDecorator` | Calculate insurance fees | Order |

Each class has exactly one reason to change.

### Composition Over Inheritance

**Before (Inheritance):**
```csharp
public class OrderWithDiscountAndTax : Order { }  // Rigid hierarchy
```

**After (Composition):**
```csharp
Order order = new Order("ORD001", 100m);
Order withDiscount = new DiscountDecorator(order, 0.10m);
Order withTax = new TaxDecorator(withDiscount, 0.08m);  // Flexible chaining
```

## Implementation Details

### Base Order Class
```csharp
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
```

### Base Decorator Class
```csharp
public abstract class OrderDecorator : Order
{
    protected Order _wrappedOrder;

    public OrderDecorator(Order order) : base(order.OrderId, order.BasePrice)
    {
        _wrappedOrder = order;
    }

    public override decimal GetTotal() => _wrappedOrder.GetTotal();
}
```

### Concrete Decorators (Example: DiscountDecorator)
```csharp
public class DiscountDecorator : OrderDecorator
{
    private decimal _discountPercent;

    public DiscountDecorator(Order order, decimal discountPercent) : base(order)
    {
        _discountPercent = discountPercent;
    }

    public override decimal GetTotal()
    {
        decimal baseTotal = _wrappedOrder.GetTotal();
        return baseTotal - (baseTotal * _discountPercent);
    }
}
```

## Usage Examples

### Simple Decoration
```csharp
// Create base order
var order = new Order("ORD001", 100m);

// Apply discount
var discounted = new DiscountDecorator(order, 0.10m);
Console.WriteLine(discounted.GetTotal()); // $90

// Apply tax
var withTax = new TaxDecorator(discounted, 0.08m);
Console.WriteLine(withTax.GetTotal()); // $97.20
```

### Complex Composition
```csharp
// Complex order: Discount → Tax → Shipping → Insurance
var order = new Order("ORD002", 200m)
    |> (o => new DiscountDecorator(o, 0.15m))      // 15% discount
    |> (o => new TaxDecorator(o, 0.08m))           // 8% tax
    |> (o => new ShippingDecorator(o, 15m))        // $15 shipping
    |> (o => new InsuranceDecorator(o, 0.02m));    // 2% insurance

Console.WriteLine(order.GetTotal()); // Fully composed price
```

### Dynamic Composition
```csharp
// Apply decorators based on conditions
var order = new Order("ORD003", 150m);

if (isLoyaltyMember)
    order = new DiscountDecorator(order, 0.20m);

if (requiresShipping)
    order = new ShippingDecorator(order, 12m);

if (orderValue > 500)
    order = new InsuranceDecorator(order, 0.01m);

Console.WriteLine(order.GetTotal());
```

## Benefits

✓ **No Class Explosion** - 1 base + 4 decorators instead of 16 classes
✓ **Flexible Composition** - Mix and match decorators dynamically
✓ **No Code Duplication** - Each decorator handles one concern
✓ **Easy to Test** - Test decorators independently
✓ **Easy to Extend** - Add new decorators without modifying existing ones
✓ **Clear Responsibilities** - Each class has single concern
✓ **Runtime Flexibility** - Apply decorators based on conditions

## Comparison: Before vs After

| Metric | Before | After |
|--------|--------|-------|
| Classes | 8+ | 5 |
| Code Duplication | High | None |
| Pricing Logic Locations | 8+ | 4 |
| Max Decorator Depth | Fixed | Dynamic |
| Adding Feature | Create multiple classes | Add 1 decorator |
| Testing | Exponential | Linear |
| Maintenance | Difficult | Easy |

## Design Patterns Used

1. **Decorator** - Core pattern for adding responsibilities
2. **Composite** - Treating individual and composed objects uniformly
3. **Strategy** - Each decorator encapsulates pricing strategy
4. **Template Method** - Base decorator provides template

## Real-World Applications

### E-Commerce
- Discounts, taxes, shipping, insurance
- Gift wrapping, express processing
- Loyalty rewards

### Food Delivery
- Base meal → Add toppings → Apply taxes → Add delivery fee
- Dynamic pricing based on conditions

### Travel Booking
- Base flight price → Add seat upgrades → Add insurance → Add lounge access
- Flexible package composition

## Test Coverage

Comprehensive test suite (23+ tests):
- ✓ Single decorator application
- ✓ Multiple decorator chaining
- ✓ Complex combinations
- ✓ Pricing accuracy
- ✓ Validation and error cases
- ✓ Edge cases

## Architecture Diagram

```
                    ┌─────────────┐
                    │   Order     │
                    └─────────────┘
                          ▲
                          │
                    ┌─────────────┐
                    │  Decorator  │ (abstract)
                    └─────────────┘
                          ▲
          ┌───────────────┼───────────────┬──────────────┐
          │               │               │              │
    ┌──────────────┐ ┌──────────┐ ┌──────────────┐ ┌────────────┐
    │  Discount    │ │   Tax    │ │  Shipping    │ │ Insurance  │
    │ Decorator    │ │Decorator │ │  Decorator   │ │ Decorator  │
    └──────────────┘ └──────────┘ └──────────────┘ └────────────┘

Each decorator wraps another Order and adds its own pricing logic.
```

---

**Pattern:** Decorator  
**Domain:** eCommerce  
**Use Case:** Order pricing system  
**SRP Compliance:** ✓ (5 focused classes)  
**Tests:** 23+  
**Key Benefit:** Unlimited pricing combinations with zero code duplication
