# Factory Pattern Analysis - 4 Domain Applications

## Overview
The Factory Pattern creates objects without specifying their exact classes. Perfect for systems with multiple types that need centralized creation logic.

---

## 🏥 HEALTHCARE - MedicationFactory

**Problem**: Different medications need different initialization based on type
- Tablets, Capsules, Injections, Liquids, Creams
- Each has different administration methods, dosages, interactions
- Creating them manually leads to errors and inconsistency

**Solution**: MedicationFactory creates appropriate medication objects

**Medication Types**:
```csharp
interface IMedication {
    string Name { get; }
    string Dosage { get; }
    void Administer();
}

// Factory creates:
class TabletMedication : IMedication { }
class CapsuleMedication : IMedication { }
class InjectionMedication : IMedication { }
class LiquidMedication : IMedication { }
class CreamMedication : IMedication { }
```

**Real-World Scenario**:
```
Doctor prescribes: "Amoxicillin 500mg tablet"
→ MedicationFactory.Create("Amoxicillin", "500mg", MedicationType.Tablet)
→ Returns TabletMedication with correct initialization
→ System knows how to administer (oral, with water, etc.)
```

**Why Factory?**
- Each medication type has different initialization
- Centralized creation ensures consistency
- Easy to add new medication types
- Encapsulates creation complexity

---

## 💰 FINANCE - InvestmentProductFactory

**Problem**: Different investment products need different configurations
- Stocks, Bonds, Mutual Funds, ETFs, Options, Futures
- Each has different fee structures, risk levels, regulations
- Manual creation leads to misconfiguration

**Solution**: InvestmentProductFactory creates appropriate product objects

**Investment Types**:
```csharp
interface IInvestmentProduct {
    string Symbol { get; }
    decimal Price { get; }
    decimal CalculateFees();
}

// Factory creates:
class Stock : IInvestmentProduct { }
class Bond : IInvestmentProduct { }
class MutualFund : IInvestmentProduct { }
class ETF : IInvestmentProduct { }
class Option : IInvestmentProduct { }
class Future : IInvestmentProduct { }
```

**Real-World Scenario**:
```
Investor wants to buy: "Apple stock"
→ InvestmentProductFactory.Create("AAPL", ProductType.Stock)
→ Returns Stock with correct fee structure and regulations
→ System knows trading rules, tax implications, settlement

Investor wants: "Bond portfolio"
→ InvestmentProductFactory.Create("US Treasury", ProductType.Bond)
→ Returns Bond with coupon payments, maturity date, rating
```

**Why Factory?**
- Each product has different fee models
- Complex initialization logic hidden
- Easy to manage different regulatory requirements
- Encapsulates product-specific logic

---

## 🎓 EDUCATION - CourseFactory

**Problem**: Different course types need different configurations
- Online Courses, Hybrid Courses, In-Person Courses, Asynchronous, Synchronous
- Each has different scheduling, materials, student limits, assessment methods
- Manual creation leads to inconsistent course setup

**Solution**: CourseFactory creates appropriate course objects

**Course Types**:
```csharp
interface ICourse {
    string Title { get; }
    int MaxStudents { get; }
    void StartSession();
}

// Factory creates:
class OnlineCourse : ICourse { }
class HybridCourse : ICourse { }
class InPersonCourse : ICourse { }
class AsyncCourse : ICourse { }
class SyncCourse : ICourse { }
```

**Real-World Scenario**:
```
Professor creates: "Introduction to Python"
→ CourseFactory.Create("Python Intro", CourseType.Online, 100)
→ Returns OnlineCourse with LMS setup, video hosting, etc.
→ System sets up deadlines, notifications, grade posting

Dean starts: "Advanced Chemistry Lab"
→ CourseFactory.Create("Chem Lab", CourseType.InPerson, 20)
→ Returns InPersonCourse with lab scheduling, equipment reservation
→ System knows attendance is mandatory, safety protocols needed
```

**Why Factory?**
- Each course type has different infrastructure needs
- Consistent setup across similar courses
- Easy to modify course type templates
- Encapsulates course-specific configuration

---

## 🏦 BANKING - AccountFactory

**Problem**: Different account types need different configurations
- Checking Accounts, Savings Accounts, Money Market, CD, Money Market, Investment, Business
- Each has different interest rates, fees, withdrawal limits, FDIC coverage
- Manual creation leads to account misconfiguration

**Solution**: AccountFactory creates appropriate account objects

**Account Types**:
```csharp
interface IBankAccount {
    string AccountNumber { get; }
    decimal Balance { get; }
    void Withdraw(decimal amount);
    void Deposit(decimal amount);
}

// Factory creates:
class CheckingAccount : IBankAccount { }
class SavingsAccount : IBankAccount { }
class MoneyMarketAccount : IBankAccount { }
class CDAccount : IBankAccount { }
class InvestmentAccount : IBankAccount { }
class BusinessAccount : IBankAccount { }
```

**Real-World Scenario**:
```
Customer opens: "Checking account"
→ AccountFactory.Create(AccountType.Checking, customerId)
→ Returns CheckingAccount with:
  - No interest
  - Unlimited withdrawals
  - Monthly fees
  - Debit card support
  - Check book provisioning

Customer opens: "Savings account"
→ AccountFactory.Create(AccountType.Savings, customerId)
→ Returns SavingsAccount with:
  - Interest calculation
  - Limited free withdrawals (6 per month)
  - Lower fees
  - No debit card
  - Early withdrawal penalties
```

**Why Factory?**
- Each account type has different fee structures
- Interest calculation differs by type
- Withdrawal rules differ
- Regulatory requirements differ
- Encapsulates account-specific logic

---

## 🏪 E-COMMERCE - ProductFactory (Bonus)

**Problem**: Different product types need different handling
- Physical Products, Digital Products, Services, Subscriptions
- Each has different shipping, tax, fulfillment needs
- Manual creation causes inconsistencies

**Solution**: ProductFactory creates appropriate product objects

**Product Types**:
```csharp
interface IProduct {
    string SKU { get; }
    decimal Price { get; }
    void Ship();
}

// Factory creates:
class PhysicalProduct : IProduct { }
class DigitalProduct : IProduct { }
class ServiceProduct : IProduct { }
class SubscriptionProduct : IProduct { }
```

---

## 📊 Complete Factory Pattern Structure

```
Design-Patterns/
│
└── Creational/
    └── Factory/                    (Object Creation Pattern)
        ├── ProductFactory ✅ (General - to be created first)
        │
        ├── Healthcare/
        │   └── MedicationFactory   (Tablet, Capsule, Injection, Liquid, Cream)
        │
        ├── Finance/
        │   └── InvestmentProductFactory (Stock, Bond, Mutual Fund, ETF, Option, Future)
        │
        ├── Education/
        │   └── CourseFactory        (Online, Hybrid, InPerson, Async, Sync)
        │
        └── Banking/
            └── AccountFactory       (Checking, Savings, MoneyMarket, CD, Investment, Business)
```

---

## 🎯 Factory vs Other Patterns

### Factory vs Singleton
- **Singleton**: ONE instance, global access
- **Factory**: CREATE multiple instances, centralized creation logic

### Factory vs Strategy
- **Strategy**: SELECT algorithm at runtime
- **Factory**: CREATE objects based on type

### Factory vs Adapter
- **Adapter**: CONVERT incompatible interface
- **Factory**: CREATE appropriate object type

---

## 💡 Benefits of Factory Pattern

1. **Centralized Creation**: All creation logic in one place
2. **Encapsulation**: Clients don't know concrete classes
3. **Easy Extension**: Add new types without changing client code
4. **Consistency**: All objects of type X created consistently
5. **Configuration**: Complex initialization hidden in factory
6. **Maintenance**: Changes to creation logic in one place

---

## 📈 Complexity by Domain

| Domain | Complexity | Number of Types | Real-World Value |
|--------|-----------|-----------------|------------------|
| Healthcare | High | 5+ medication types | High (patient safety) |
| Finance | Very High | 6+ product types | Very High (money) |
| Education | Medium | 5 course types | Medium (learning) |
| Banking | Very High | 6+ account types | Very High (money) |

---

## 🔄 Implementation Pattern

Each use case follows:

```
Before/
├── README.md          (Problem: manual creation, errors, inconsistency)
└── app.cs            (Demo: hard-coded object creation)

After/
├── src/
│   ├── IProduct.cs           (Common interface)
│   ├── ConcreteType1.cs      (Product type 1)
│   ├── ConcreteType2.cs      (Product type 2)
│   ├── ...
│   ├── ProductFactory.cs     (Factory implementation)
│   └── Client.cs             (Uses factory)
│
├── Tests/
│   └── ProductFactoryTests.cs (47+ tests)
│
├── docs/
│   └── FACTORY_OVERVIEW.md
│
├── README.md          (Solution: factory pattern benefits)
└── ProductFactory.csproj
```

---

## 📚 Test Categories (47+ tests per use case)

1. **Object Creation** (10 tests)
   - Create each type
   - Verify correct type returned
   - Verify initialization

2. **Configuration** (10 tests)
   - Each type configured correctly
   - Parameters applied properly
   - Defaults work

3. **Encapsulation** (8 tests)
   - Clients don't know concrete classes
   - Can change implementation

4. **Extension** (8 tests)
   - Add new types without breaking
   - Factory updates only

5. **Error Handling** (6 tests)
   - Invalid type handling
   - Missing parameters
   - Error messages

6. **Integration** (5 tests)
   - Factory in context
   - Multiple type creation
   - State management

---

## 🚀 Implementation Priority

### Phase 1 (High Value)
1. **Banking/AccountFactory** - Core financial system
2. **Finance/InvestmentProductFactory** - Complex products
3. **Healthcare/MedicationFactory** - Safety critical

### Phase 2 (Medium Value)
4. **Education/CourseFactory** - Infrastructure heavy

---

## ✅ Key Implementation Points

**For Each Domain Factory**:
- ✅ Create interface (IProduct)
- ✅ Create 5-6 concrete types
- ✅ Implement Factory class with Create() method
- ✅ Hide concrete classes from clients
- ✅ Write 47+ comprehensive tests
- ✅ Document configuration and extensions
- ✅ Show Before/After comparison

---

**Next Step**: Create folder structures and starter documentation for all 4 domain factories
