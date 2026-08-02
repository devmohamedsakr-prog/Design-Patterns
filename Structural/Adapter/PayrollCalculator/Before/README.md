# ❌ Before: Payroll Calculator without Adapter Pattern

## Problem Statement

This implementation demonstrates the **problems** when multiple payroll systems with **incompatible interfaces** need to work together without an adaptation layer.

---

## 🔴 Issues Demonstrated

### 1. **No Unified Interface**
Different payroll systems use different method names and signatures:

```csharp
// System 1: Legacy
legacySystem.RecordEmployeeWage("EMP001", 25.00m, 40);

// System 2: Modern
modernSystem.SetEmployeeSalary("EMP003", 3000m, 500m);

// System 3: Contractor
contractorSystem.RegisterContractor("CONT001", 1500m);

// System 4: Intern
internSystem.EnrollIntern("INTERN001", 500m);
```

### 2. **Different Retrieval Methods**
No standard way to get payment amounts:

```csharp
// Each system has different method names
legacySystem.GetEmployeeSalary("EMP001");
modernSystem.GetTotalCompensation("EMP003");
contractorSystem.CalculateContractorPayment("CONT001");
internSystem.CalculateInternPayment("INTERN001");
```

### 3. **Different Report Formats**
Each system prints reports differently:

```csharp
legacySystem.PrintLegacyReport();           // Method: PrintLegacyReport
modernSystem.DisplayModernReport();         // Method: DisplayModernReport
contractorSystem.ShowContractorStatement(); // Method: ShowContractorStatement
internSystem.PrintInternPayrollSheet();     // Method: PrintInternPayrollSheet
```

---

## 📊 Architecture Problems

### No Standard Contract
```
┌─────────────────────────────────────┐
│     Client Code (PayrollProcessor)  │
├─────────────────────────────────────┤
│  ❌ Knows about all system types    │
│  ❌ Calls different methods         │
│  ❌ Handles different data formats  │
│  ❌ Duplicated logic for same ops   │
└─────────────────────────────────────┘
  ↓         ↓         ↓         ↓
┌──────────────────────────────────┐
│ LegacyPayrollSystem | ModernPayroll
│ ContractorPayment  | InternshipPay
│ (ALL DIFFERENT!)   | (ALL DIFFERENT!)
└──────────────────────────────────┘
```

### High Coupling
- Client tightly coupled to each system
- No abstraction layer
- Difficult to add new systems

---

## 🔴 Specific Problems

| Problem | Impact | Example |
|---------|--------|---------|
| **No unified interface** | Must handle each system separately | 4+ different method calls for same operation |
| **Different method names** | Code duplication and confusion | RecordEmployeeWage vs SetEmployeeSalary |
| **Different data structures** | Complex client logic | Tuples vs dictionaries vs simple values |
| **Different report methods** | No standard reporting | 4 different report methods |
| **Hard to extend** | Adding new system = code changes | New payroll system = modify client code |
| **Type safety issues** | Runtime errors possible | Different parameter types/counts |
| **Testing complexity** | Hard to test uniformly | Must test each system separately |
| **Maintenance nightmare** | Future changes risky | Changes to one system affect many places |

---

## 📋 System Interfaces (Incompatible)

### LegacyPayrollSystem
```csharp
public void RecordEmployeeWage(string employeeId, decimal hourlyRate, int hoursWorked)
public decimal GetEmployeeSalary(string employeeId)
public void PrintLegacyReport()
```

### ModernPayrollSystem
```csharp
public void SetEmployeeSalary(string empId, decimal baseSalary, decimal bonus)
public decimal GetTotalCompensation(string empId)
public void DisplayModernReport()
```

### ContractorPaymentSystem
```csharp
public void RegisterContractor(string contractorId, decimal ratePerProject)
public void LogProjectCompletion(string contractorId, int projectCount)
public decimal CalculateContractorPayment(string contractorId)
public void ShowContractorStatement()
```

### InternshipPaymentSystem
```csharp
public void EnrollIntern(string internId, decimal monthlyStipend)
public void DeactivateIntern(string internId)
public decimal CalculateInternPayment(string internId)
public void PrintInternPayrollSheet()
```

---

## 🎯 Pain Points

### 1. **Client Code Complexity**
```csharp
// Client must know about all system types
var legacySystem = new LegacyPayrollSystem();
var modernSystem = new ModernPayrollSystem();
var contractorSystem = new ContractorPaymentSystem();
var internSystem = new InternshipPaymentSystem();

// Each setup is different
legacySystem.RecordEmployeeWage(...);
modernSystem.SetEmployeeSalary(...);
contractorSystem.RegisterContractor(...);
internSystem.EnrollIntern(...);
```

### 2. **No Polymorphism**
```csharp
// Can't do this - no common interface
PayrollSystem[] systems = { legacySystem, modernSystem, ... };
foreach (var system in systems)
{
    system.ProcessPayment(...); // ❌ No such method!
}
```

### 3. **Different Data Representations**
```csharp
// Legacy: Direct calculation
decimal legacy = hourlyRate * hoursWorked;

// Modern: Base + Bonus
decimal modern = baseSalary + bonus;

// Contractor: Rate × Projects
decimal contractor = ratePerProject * projectCount;

// Intern: Monthly stipend
decimal intern = monthlyStipend;
```

### 4. **Reporting Inconsistency**
Each system formats and presents data differently, making unified reporting impossible.

---

## ✅ What's Needed

The **Adapter Pattern** solves these issues by:

1. ✅ Creating a **common interface** for all payroll systems
2. ✅ **Adapting** each system to conform to the interface
3. ✅ Allowing **polymorphic** processing
4. ✅ Reducing **client code complexity**
5. ✅ Making it easy to **add new systems**
6. ✅ Enabling **uniform reporting**

---

## 🏃 Running This Example

```bash
dotnet run
```

**Output shows:**
- 4 incompatible payroll systems
- Different method calls for each
- Inconsistent reporting
- Problems with no unified interface

---

## 📚 Next Step

See the **After** implementation for the Adapter Pattern solution that:
- Creates unified IPayrollSystem interface
- Adapts all systems to common interface
- Enables polymorphic processing
- Simplifies client code
- Makes system easy to extend

---

## 🔑 Key Takeaway

> **The Adapter Pattern bridges the gap between incompatible interfaces, allowing systems that weren't designed to work together to collaborate seamlessly.**
