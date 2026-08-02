# ProductFactory - Before (Problem)

## Problem Statement

An e-commerce system needs to handle different product types with different properties and business logic. Without a factory pattern, creating products leads to:

- **Inconsistent Product Creation**: Each service creates products differently
- **Mixed Business Logic**: Shipping, tax, inventory logic scattered everywhere
- **Hard to Add Types**: New product types require changes in multiple places
- **Duplicate Code**: Same creation logic repeated across services
- **Error Prone**: Easy to misconfigure product properties

## Current Issues

```csharp
// ❌ PROBLEM: Manual product creation everywhere

// In ProductService
var shirt = new PhysicalProduct
{
    SKU = "SHIRT-001",
    Name = "T-Shirt",
    Price = 29.99m,
    Weight = 0.5,
    Dimensions = "L x W x H",
    ShippingZones = new[] { "US", "CA", "MX" }
    // But forgot: ReorderLevel, WarehouseLocation, Fragile flag
};

// In OrderService
var ebook = new DigitalProduct
{
    SKU = "EBOOK-001",
    Name = "Learn C#",
    Price = 19.99m,
    FileSize = "5MB",
    Format = "PDF"
    // Missing: License info, Download limit, Expiry logic
};

// In SubscriptionService
var monthly = new SubscriptionProduct
{
    SKU = "SUB-MONTHLY",
    Name = "Premium",
    Price = 9.99m,
    BillingInterval = "Monthly"
    // Missing: AutoRenewal logic, CancellationPolicy, GracePeriod
};

// Result: Products created inconsistently, missing critical properties!
```

## Real-World Impact

- Digital product shipped instead of downloaded → customer confused
- Physical product doesn't calculate shipping correctly → lost money
- Subscription charges continue after cancellation → refund requests
- New product type requires code changes everywhere
- Bug fixes must be replicated in multiple places

## Limitations

| Issue | Impact | Severity |
|-------|--------|----------|
| Inconsistent creation | Wrong product type behavior | 🔴 Critical |
| Duplicate logic | Maintenance nightmare | 🟡 Medium |
| Hard to extend | Long development time | 🟡 Medium |
| Error prone | Revenue loss | 🔴 Critical |
| Tight coupling | Tests impossible | 🟡 Medium |

## Solution Direction

We need a **Factory** that:
1. Centralizes all product creation logic
2. Ensures correct initialization for each product type
3. Hides product type details from clients
4. Makes adding new product types easy

→ **SOLUTION: Factory Pattern**
