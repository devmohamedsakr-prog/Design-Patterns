# ProductFactory - After (Solution)

## Solution: Factory Pattern

The Factory Pattern centralizes product creation, ensuring each type is correctly initialized with proper business logic.

## How It Works

```csharp
// ✅ Solution: Use ProductFactory

public interface IProduct
{
    string SKU { get; }
    string Name { get; }
    decimal Price { get; }
    decimal CalculateShippingCost();
    decimal CalculateTax();
}

public static class ProductFactory
{
    public static IProduct Create(ProductType type, ProductDetails details)
    {
        return type switch
        {
            ProductType.Physical => new PhysicalProduct(details),
            ProductType.Digital => new DigitalProduct(details),
            ProductType.Service => new ServiceProduct(details),
            ProductType.Subscription => new SubscriptionProduct(details),
            _ => throw new ArgumentException($"Unknown product type: {type}")
        };
    }
}

// Usage: Clean and simple
var shirt = ProductFactory.Create(ProductType.Physical, shirtDetails);
var ebook = ProductFactory.Create(ProductType.Digital, ebookDetails);
var subscription = ProductFactory.Create(ProductType.Subscription, subDetails);
```

## Product Types

### Physical Product
- Requires shipping calculation
- Has weight and dimensions
- Inventory tracking
- Warehouse location management
- Requires tax calculation
- Damage/fragile handling

### Digital Product
- No shipping required
- Instant delivery
- File download mechanism
- License management
- Access expiry optional
- No tax on digital goods (varies by jurisdiction)

### Service Product
- Professional services
- No physical product
- Instant/scheduled delivery
- Labor-based pricing
- Consultation booking
- Custom configuration per order

### Subscription Product
- Recurring billing
- Auto-renewal logic
- Cancellation policies
- Billing interval (monthly, yearly, etc.)
- Grace period handling
- Prorated charges

## Architecture

```
IProduct (Interface)
    ↑
    ├── PhysicalProduct
    ├── DigitalProduct
    ├── ServiceProduct
    └── SubscriptionProduct
    
ProductFactory
    └── Create(type) → IProduct
```

## Benefits

| Benefit | Impact |
|---------|--------|
| Centralized Creation | One place to fix bugs |
| Correct Types | Right business logic per type |
| Easy to Extend | New product type = new class |
| Loose Coupling | Clients use IProduct interface |
| Maintainable | Clear, organized code |
| Testable | Mock IProduct in tests |

## Real-World Example

```csharp
public class OrderService
{
    public Order CreateOrder(OrderRequest request)
    {
        var order = new Order();
        
        foreach (var item in request.Items)
        {
            // ✅ Factory handles all product type complexities
            var product = ProductFactory.Create(item.Type, item.Details);
            
            // Product knows its own business rules
            var shipping = product.CalculateShippingCost(); // 0 for digital
            var tax = product.CalculateTax(); // Varies by type
            
            order.AddLineItem(product, item.Quantity, shipping, tax);
        }
        
        return order;
    }
}
```

## Usage Patterns

```csharp
// Physical Product
var book = ProductFactory.Create(
    ProductType.Physical, 
    new { SKU = "BOOK-001", Weight = 1.5m, Dimensions = "6x9x1" }
);

// Digital Product
var software = ProductFactory.Create(
    ProductType.Digital,
    new { SKU = "SOFT-001", FileSize = "100MB", Format = "EXE" }
);

// Service Product
var consulting = ProductFactory.Create(
    ProductType.Service,
    new { SKU = "CONS-001", HourlyRate = 150m, Duration = 2 }
);

// Subscription Product
var premium = ProductFactory.Create(
    ProductType.Subscription,
    new { SKU = "SUB-001", BillingInterval = "Monthly", AutoRenew = true }
);
```

## Files

- `IProduct.cs` - Product interface
- `PhysicalProduct.cs` - Physical products
- `DigitalProduct.cs` - Digital products
- `ServiceProduct.cs` - Services
- `SubscriptionProduct.cs` - Subscriptions
- `ProductFactory.cs` - Factory implementation
- `ProductType.cs` - Enum of types
- `ProductDetails.cs` - Domain model
- `Tests/ProductFactoryTests.cs` - 47+ tests

## Test Coverage

✅ Physical product creation and calculations (12 tests)
✅ Digital product creation and zero shipping (10 tests)
✅ Service product configuration (10 tests)
✅ Subscription billing logic (10 tests)
✅ Error handling and edge cases (5 tests)

Total: 47+ tests, all passing
