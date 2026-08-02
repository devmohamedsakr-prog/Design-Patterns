# ❌ Before: Customer Discount without Strategy Pattern

## Problem Statement

This implementation demonstrates the **problems** when discount calculation logic is **hard-coded** and **tightly coupled** to the Order class.

---

## 🔴 Issues Demonstrated

### 1. **Tightly Coupled Logic**
Discount calculations are embedded directly in the Order class:

```csharp
public decimal CalculateDiscount()
{
    decimal subtotal = GetSubtotal();
    decimal discount = 0;

    if (Customer.Type == CustomerType.Regular)
    {
        discount = 0;
    }
    else if (Customer.Type == CustomerType.Premium)
    {
        discount = subtotal * 0.10m;
    }
    else if (Customer.Type == CustomerType.VIP)
    {
        discount = subtotal * 0.20m;
    }
    // ... and more
}
```

### 2. **Difficult to Add New Discount Types**
Every new discount type requires modifying the CalculateDiscount() method:

```csharp
// Want to add SeasonalDiscount? Must edit existing code!
// Want to add CombinedDiscount? Must edit existing code!
// Want to add BirthdayDiscount? Must edit existing code!
```

### 3. **Hard to Test**
Testing individual discount strategies requires:
- Creating full Order objects
- Creating Customer objects
- Setting specific conditions
- No way to test discount in isolation

### 4. **No Reusability**
If another class needs discount logic:
- Must duplicate code
- Or create complex inheritance hierarchy
- No way to share and reuse strategies

### 5. **Violates SOLID Principles**
- ❌ **Open/Closed**: Not open for extension, only for modification
- ❌ **Single Responsibility**: Order class has too many reasons to change
- ❌ **Dependency Inversion**: Depends on concrete implementations

---

## 📊 Architecture Problems

```
┌──────────────────────────────┐
│       Order Class            │
├──────────────────────────────┤
│ - Customer data              │
│ - Order items                │
│ - Regular discount logic     │
│ - Premium discount logic     │
│ - VIP discount logic         │
│ - Loyal discount logic       │
│ - Future discount logic (?) │
└──────────────────────────────┘
       ❌ Too many responsibilities
```

---

## 🔴 Specific Problems

| Problem | Impact | Example |
|---------|--------|---------|
| **Tightly coupled** | Hard to change | Modifying discount % requires changing Order class |
| **Adding new types** | Code modification | SeasonalDiscount needs CalculateDiscount() edit |
| **Testing difficulty** | Complex test setup | Can't test discount formula in isolation |
| **No composition** | Can't combine strategies | Can't apply multiple discounts together |
| **Code duplication** | Repeated logic | If other classes need discounts, must copy code |
| **Maintenance nightmare** | High risk | Changes to one discount affect all others |
| **Runtime inflexibility** | Fixed at design time | Can't change discount strategy at runtime |
| **Scalability issues** | Grows linearly | More discounts = more if/else statements |

---

## 📋 Current Discount Types (Hard-Coded)

```csharp
// 1. Regular Customer - No discount
discount = 0;

// 2. Premium Customer - 10% discount
discount = subtotal * 0.10m;

// 3. VIP Customer - 20% discount
discount = subtotal * 0.20m;

// 4. Loyal Customer - 5% + 1% per year (capped at 25%)
discount = subtotal * (0.05m + (Customer.YearsAsCustomer * 0.01m));
```

---

## ❌ What If We Need...

| Scenario | Problem |
|----------|---------|
| **Seasonal Discount** (20% in summer) | Need to add if/else + date logic to CalculateDiscount() |
| **Birthday Discount** (15% on birthday) | Need to check dates, add to Order class |
| **Volume Discount** (5% per 10 items) | Need to count items, modify calculation |
| **First-time Customer** (25% off) | Need to track customer history |
| **Combine Discounts** (VIP + Seasonal) | No way to compose strategies |
| **Change discounts at runtime** | Must recompile code |

---

## ✅ What We Need

The **Strategy Pattern** solves this by:

1. ✅ **Extracting** discount logic into separate strategy classes
2. ✅ **Defining** a common interface for all strategies
3. ✅ **Encapsulating** each discount type independently
4. ✅ **Allowing** runtime strategy selection
5. ✅ **Enabling** strategy composition
6. ✅ **Making** each strategy testable in isolation

---

## 🏃 Running This Example

```bash
dotnet run
```

**Output shows:**
- 4 different discount calculations
- Hard-coded if/else chain
- Problems with no flexibility

---

## 📚 Next Step

See the **After** implementation for the Strategy Pattern solution that:
- Extracts each discount into a separate strategy
- Uses a common IDiscountStrategy interface
- Allows runtime strategy selection
- Enables strategy composition
- Makes each strategy easily testable

---

## 🔑 Key Takeaway

> **The Strategy Pattern encapsulates a family of algorithms, making them interchangeable. It lets the algorithm vary independently from the clients that use it.**
