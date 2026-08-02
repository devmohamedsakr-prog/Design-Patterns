# Factory Method Pattern - E-Commerce Payment Gateway

## Problem & Solution

### ❌ BEFORE: Hard-Coded Payment Processing
```
PaymentProcessor class with hard-coded if-else for each payment type:
- if (type == "Stripe") → ProcessStripePayment()
- else if (type == "PayPal") → ProcessPayPalPayment()
- else if (type == "BankTransfer") → ProcessBankTransferPayment()

PROBLEMS:
1. Violates Open-Closed Principle (must modify code to add new payment types)
2. Violates Single Responsibility Principle (all logic in one class)
3. Tight coupling (no abstraction, string-based type checking)
4. Cannot easily mock or swap implementations
5. Not scalable for new payment methods
```

### ✅ AFTER: Factory Method Pattern
```
PaymentGatewayCreator (abstract base)
├── CreatePaymentProcessor() [Factory Method - abstract]
├── ProcessPaymentAsync() [Template method]
└── Common logic (validation, logging)

Concrete Creators:
├── StripePaymentGateway → creates StripeProcessor
├── PayPalPaymentGateway → creates PayPalProcessor
└── BankTransferPaymentGateway → creates BankTransferProcessor

Concrete Products (IPaymentProcessor):
├── StripeProcessor
├── PayPalProcessor
└── BankTransferProcessor

BENEFITS:
1. ✅ Open for extension, closed for modification
2. ✅ Each processor has single responsibility
3. ✅ Loose coupling via interface abstraction
4. ✅ Easy to mock and test
5. ✅ Scalable - add new processors without modifying existing code
```

## Project Structure

```
Creational/FactoryMethod/ECommerce/PaymentGateway/
├── Before/                          ← Problem version
│   ├── src/
│   │   └── PaymentProcessor.cs      (monolithic, hard-coded)
│   ├── Tests/
│   │   └── PaymentProcessorTests.cs (47+ tests showing problems)
│   └── PaymentGateway.csproj
│
└── After/                           ← Solution version
    ├── src/
    │   ├── Abstracts/
    │   │   └── PaymentGateway.cs    (abstract creator + interface)
    │   ├── Creators/
    │   │   ├── StripePaymentGateway.cs
    │   │   ├── PayPalPaymentGateway.cs
    │   │   └── BankTransferPaymentGateway.cs
    │   └── Processors/
    │       ├── StripeProcessor.cs
    │       ├── PayPalProcessor.cs
    │       └── BankTransferProcessor.cs
    ├── Tests/
    │   └── PaymentGatewayTests.cs   (47+ tests)
    └── PaymentGateway.csproj
```

## Factory Method Pattern Components

### 1. Abstract Creator
```csharp
public abstract class PaymentGatewayCreator
{
    // ✅ Factory Method - subclasses implement this
    protected abstract IPaymentProcessor CreatePaymentProcessor();
    
    // ✅ Template Method - uses factory method
    public async Task<PaymentResult> ProcessPaymentAsync(...)
    {
        IPaymentProcessor processor = CreatePaymentProcessor();
        // Use processor
    }
}
```

### 2. Concrete Creators
```csharp
public class StripePaymentGateway : PaymentGatewayCreator
{
    protected override IPaymentProcessor CreatePaymentProcessor()
    {
        return new StripeProcessor();  // ✅ Factory method
    }
}
```

### 3. Product Interface
```csharp
public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessAsync(decimal amount, string currency, string orderId);
    string GetProcessorName();
}
```

### 4. Concrete Products
```csharp
public class StripeProcessor : IPaymentProcessor
{
    public async Task<PaymentResult> ProcessAsync(...)
    {
        // Stripe-specific implementation
    }
}
```

## Key Differences: Before vs After

| Aspect | Before | After |
|--------|--------|-------|
| **Structure** | Single monolithic class | Separate creator/product classes |
| **Extensibility** | Hard-coded if-else | Add new creator, no existing code changes |
| **SRP** | Violates (all in one class) | Follows (one class per processor) |
| **Coupling** | Tight (string-based) | Loose (interface-based) |
| **Testing** | Cannot easily mock | Can mock IPaymentProcessor |
| **New Method** | Modify existing code | Create new creator class |

## Usage Comparison

### ❌ BEFORE
```csharp
var processor = new PaymentProcessor();
var result = await processor.ProcessPaymentAsync("Stripe", 100m, "USD", "ORD-001");
// To add Apple Pay: Must modify PaymentProcessor class!
```

### ✅ AFTER
```csharp
PaymentGatewayCreator gateway = new StripePaymentGateway();
var result = await gateway.ProcessPaymentAsync(100m, "USD", "ORD-001");

// To add Apple Pay: Just create new ApplePayPaymentGateway!
gateway = new ApplePayPaymentGateway();
result = await gateway.ProcessPaymentAsync(100m, "USD", "ORD-002");
```

## Design Principles Applied

1. **Single Responsibility Principle (SRP)**
   - Each processor handles only its payment logic
   - Each creator only knows how to create its processor

2. **Open-Closed Principle**
   - Open for extension (add new creators/processors)
   - Closed for modification (existing code unchanged)

3. **Liskov Substitution Principle**
   - All creators can be used interchangeably
   - All processors implement same interface

4. **Interface Segregation Principle**
   - IPaymentProcessor is focused (only what's needed)
   - Clients depend on abstraction, not concrete classes

5. **Dependency Inversion Principle**
   - Depend on PaymentGatewayCreator (abstract)
   - Not on StripePaymentGateway (concrete)

## Tests Summary

### Before: 47 Tests
- Stripe payment tests (5)
- PayPal payment tests (5)
- Bank transfer tests (4)
- Error handling tests (3)
- Problem demonstration tests (3)
- Additional tests (27+)

### After: 47 Tests
- Stripe gateway tests (5)
- PayPal gateway tests (4)
- Bank transfer gateway tests (4)
- Factory method pattern tests (3)
- Validation tests (4)
- Multiple payment scenarios (3)
- Concrete processor tests (3)
- Additional comprehensive tests (18+)

## Real-World Use Cases

✅ **E-Commerce**: Multiple payment gateways (Stripe, PayPal, Square)
✅ **SaaS Platforms**: Different payment processors for different regions
✅ **Fintech**: Various banking APIs and payment methods
✅ **Healthcare**: Insurance payment processing variations
✅ **Subscription Services**: Different billing systems per region

## Factory Method vs Factory Pattern

| Aspect | Factory Method | Factory (Existing) |
|--------|----------------|-------------------|
| **Creation** | Subclass decides | Type-based decision |
| **Inheritance** | Required | Not required |
| **Flexibility** | Via inheritance | Via configuration |
| **Coupling** | Reduced via abstraction | Still somewhat coupled |
| **Use When** | Family of related creators | Simple type switching |
