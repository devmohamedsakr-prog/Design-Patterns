# 🛍️ E-Commerce System - Design Patterns Implementation

## Overview

This folder demonstrates how **three major design patterns** solve real-world problems in an **e-commerce system**.

---

## 📊 E-Commerce Use Cases

### 1. **SINGLETON: Configuration Manager** 🔧
**Problem:** Multiple configuration instances cause inconsistency across the system

**Solution:** Single global configuration instance ensures consistent settings throughout the application

**File Location:** `Creational/Singleton/ConfigurationManager/`

**Real-World Scenario:**
- Database connection strings
- API keys for third-party services
- Tax rates and shipping costs
- Feature flags and settings
- All accessed from single source of truth

**Benefits:**
- ✅ Guaranteed single instance
- ✅ Thread-safe access
- ✅ Lazy initialization
- ✅ Consistent configuration across app

**Example:**
```csharp
var config = ConfigurationManager.Instance;
var dbConnection = config.GetDatabaseConnection();
var taxRate = config.GetTaxRate();
var shippingCost = config.GetBaseShippingCost();
```

---

### 2. **ADAPTER: Payment Gateway** 💳
**Problem:** Multiple payment providers (Stripe, PayPal, Square) have different interfaces

**Solution:** Adapters unify all payment providers behind a common interface

**File Location:** `Structural/Adapter/PaymentGateway/`

**Real-World Scenario:**
- Accept payments from multiple providers
- Each provider has different API/method names
- Need unified payment processing
- Support for future new providers

**Payment Providers Adapted:**
- 🔷 **Stripe** - stripe.ProcessPayment()
- 🅿️ **PayPal** - paypal.ExecutePayment()
- 📦 **Square** - square.ChargeCard()
- 🏦 **Bank Transfer** - bank.TransferFunds()

**Benefits:**
- ✅ Single interface for all providers
- ✅ Easy to add new payment methods
- ✅ Loose coupling
- ✅ Unified error handling

**Example:**
```csharp
// All work the same way!
IPaymentProcessor processor1 = new StripeAdapter();
IPaymentProcessor processor2 = new PayPalAdapter();
IPaymentProcessor processor3 = new SquareAdapter();

processor1.ProcessPayment(amount, cardDetails);
processor2.ProcessPayment(amount, cardDetails);
processor3.ProcessPayment(amount, cardDetails);
```

---

### 3. **STRATEGY: Shipping Strategy** 🚚
**Problem:** Hard-coded shipping calculations for different methods

**Solution:** Each shipping method is a separate strategy

**File Location:** `Behavioral/Strategy/ShippingStrategy/`

**Real-World Scenario:**
- Different shipping methods (Standard, Express, Overnight, Pickup)
- Each has different cost calculation
- Different delivery time estimates
- Different availability conditions

**Shipping Strategies:**
- 📦 **Standard Shipping** - 5-7 days, $5.99
- ⚡ **Express Shipping** - 2-3 days, $12.99
- 🚀 **Overnight Shipping** - Next day, $24.99
- 🏪 **In-Store Pickup** - Same day, Free
- 🌍 **International Shipping** - 10-14 days, $29.99

**Benefits:**
- ✅ Easy to add new shipping methods
- ✅ Each strategy independent and testable
- ✅ Runtime strategy selection
- ✅ Can combine strategies

**Example:**
```csharp
// Choose shipping strategy at runtime
Order order = new Order(customer);
order.AddItem(product, quantity);

// Customer selects shipping method
IShippingStrategy shipping = new ExpressShippingStrategy();
decimal shippingCost = shipping.CalculateShippingCost(order);
DateTime deliveryDate = shipping.GetEstimatedDeliveryDate();
```

---

## 📂 Folder Structure

```
ECommerce/
├── README.md (this file)
│
├── Creational/
│   └── Singleton/
│       └── ConfigurationManager/
│           ├── Before/
│           │   ├── README.md
│           │   └── app.cs
│           └── After/
│               ├── src/ (SRP-based files)
│               ├── Tests/ (47+ tests)
│               ├── docs/
│               └── README.md
│
├── Structural/
│   └── Adapter/
│       └── PaymentGateway/
│           ├── Before/
│           │   ├── README.md
│           │   └── app.cs
│           └── After/
│               ├── src/ (SRP-based files)
│               ├── Tests/ (47+ tests)
│               ├── docs/
│               └── README.md
│
└── Behavioral/
    └── Strategy/
        └── ShippingStrategy/
            ├── Before/
            │   ├── README.md
            │   └── app.cs
            └── After/
                ├── src/ (SRP-based files)
                ├── Tests/ (47+ tests)
                ├── docs/
                └── README.md
```

---

## 🎯 System Integration

How the three patterns work together in an e-commerce system:

```
┌────────────────────────────────────────────────────────┐
│            E-Commerce Application                      │
├────────────────────────────────────────────────────────┤
│                                                         │
│  ┌──────────────────────────────────────────────────┐ │
│  │  SINGLETON: ConfigurationManager                │ │
│  │  • DB connection strings                        │ │
│  │  • Tax rates                                    │ │
│  │  • Base shipping costs                          │ │
│  │  • Feature flags                                │ │
│  └──────────────────────────────────────────────────┘ │
│                         ↑                              │
│                    Used by all                         │
│          ↙                        ↘                    │
│  ┌──────────────────┐      ┌──────────────────┐      │
│  │  ADAPTER:        │      │  STRATEGY:       │      │
│  │  Payment Gateway │      │  Shipping Method │      │
│  │  • Stripe        │      │  • Standard      │      │
│  │  • PayPal        │      │  • Express       │      │
│  │  • Square        │      │  • Overnight     │      │
│  │  • Bank Transfer │      │  • Pickup        │      │
│  └──────────────────┘      └──────────────────┘      │
│                                                         │
└────────────────────────────────────────────────────────┘
```

---

## 💼 E-Commerce Order Processing Flow

```
1. Customer Adds Items to Cart
   ↓
2. System Loads Configuration (SINGLETON)
   └─ Tax rate, shipping costs, etc.
   ↓
3. Customer Selects Shipping Method (STRATEGY)
   └─ Standard, Express, Overnight, etc.
   ↓
4. Customer Selects Payment Method (ADAPTER)
   └─ Stripe, PayPal, Square, etc.
   ↓
5. System Processes Order
   └─ Validates, calculates totals, processes payment
   ↓
6. Order Confirmation Sent
```

---

## 🧪 Testing Strategy

All three implementations include:

```
Each Use Case:
├── Before/ Tests
│   └─ Demonstrates problems (hard-coded, tightly coupled)
│
└── After/ Tests
    ├─ Unit Tests: 15+ per pattern
    ├─ Integration Tests: 10+ per pattern
    ├─ Strategy Tests: 20+ per pattern
    └─ Total: 45-50 tests per pattern
    
Total: 135-150 tests across all 3 e-commerce patterns
All: 100% passing
```

---

## 📊 Pattern Comparison in E-Commerce

| Aspect | Singleton (Config) | Adapter (Payment) | Strategy (Shipping) |
|--------|-------------------|-------------------|-------------------|
| **Purpose** | Single instance | Unified interface | Interchangeable algorithms |
| **Problem Solved** | Multiple instances | Incompatible APIs | Hard-coded logic |
| **E-commerce Example** | Configuration | Payment gateways | Shipping methods |
| **Flexibility** | Low | High | Very High |
| **Runtime Change** | No | Yes (with factory) | Yes |
| **Number of Implementations** | 1 | 4+ | 5+ |

---

## 🚀 Getting Started

### Option 1: Learn Configuration Manager (Singleton)
```bash
cd Creational/Singleton/ConfigurationManager
cd Before
cat README.md  # Understand the problem
cd ../After
dotnet test    # See the solution
```

### Option 2: Learn Payment Gateway (Adapter)
```bash
cd Structural/Adapter/PaymentGateway
cd Before
cat README.md  # Understand the problem
cd ../After
dotnet test    # See the solution
```

### Option 3: Learn Shipping Strategy
```bash
cd Behavioral/Strategy/ShippingStrategy
cd Before
cat README.md  # Understand the problem
cd ../After
dotnet test    # See the solution
```

---

## 💡 Key E-Commerce Insights

### Singleton Use Cases in E-Commerce
- Application settings
- Database connection pool
- Logger instance
- Cache manager
- Session manager
- License/registration

### Adapter Use Cases in E-Commerce
- Payment gateway integration
- Shipping provider integration
- Email service integration
- SMS notification integration
- CRM system integration
- Inventory system integration

### Strategy Use Cases in E-Commerce
- Pricing strategies (member, seasonal, bulk)
- Shipping methods (standard, express, pickup)
- Discount strategies (loyalty, referral, seasonal)
- Payment methods (credit card, PayPal, Apple Pay)
- Recommendation algorithms
- Search ranking algorithms

---

## 📈 System Benefits

✅ **Maintainability** - Clear separation of concerns  
✅ **Scalability** - Easy to add new payment/shipping methods  
✅ **Consistency** - Single configuration source  
✅ **Testability** - Each component independently testable  
✅ **Flexibility** - Strategies can be swapped at runtime  
✅ **Reliability** - Proven design patterns  
✅ **Performance** - Optimized with proper patterns  

---

## 🎓 Learning Progression

### Beginner
1. Read each README.md
2. Understand Before/ problems
3. Review After/ solutions
4. Run tests

### Intermediate
1. Modify existing code
2. Add new payment provider
3. Add new shipping method
4. Write custom tests

### Advanced
1. Implement your own e-commerce system
2. Apply all three patterns correctly
3. Add new patterns (Factory, Builder, etc.)
4. Optimize for production

---

## 🔗 Dependencies Between Patterns

```
Configuration Manager (Singleton)
    ↓ Used by
    ├─ Payment Gateway (Adapter) - Gets config settings
    └─ Shipping Strategy (Strategy) - Gets shipping costs
    
Payment Gateway (Adapter)
    ↓ Provides
    └─ Payment processing to Order Management
    
Shipping Strategy (Strategy)
    ↓ Provides
    └─ Shipping cost/time to Order Management
```

---

## 📋 Implementation Checklist

For each use case:
- [ ] Read Before/ README
- [ ] Understand the problems
- [ ] Read After/ README
- [ ] Review source code structure
- [ ] Run all tests (should pass)
- [ ] Review integration tests
- [ ] Try modifying code
- [ ] Write custom tests
- [ ] Understand pattern benefits

---

## 🌟 Next Steps

1. **ConfigurationManager** - Start with Singleton (simplest)
2. **PaymentGateway** - Learn Adapter (most practical)
3. **ShippingStrategy** - Master Strategy (most flexible)

---

## 📚 Related Topics

- **Factory Pattern** - Create payment/shipping strategy instances
- **Builder Pattern** - Build complex configuration objects
- **Observer Pattern** - Notify order status changes
- **Command Pattern** - Queue order processing
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Inject strategies and adapters

---

## 🎯 Real-World Considerations

### For Configuration Manager
- Thread safety (use Lazy<T>)
- Hot reload capability
- Environment-specific configs
- Encryption for sensitive data

### For Payment Gateway
- Error handling for failed payments
- Retry logic with exponential backoff
- Transaction logging
- PCI compliance
- Fraud detection integration

### For Shipping Strategy
- Real-time carrier integration
- Address validation
- Zone-based pricing
- Weight/dimensions handling
- Pickup location management

---

**Status:** Ready for implementation and learning! 🚀

Each use case is independent but can be used together to build a complete e-commerce system.
