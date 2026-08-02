# MedicationFactory - Before (Problem)

## Problem Statement

A healthcare system needs to manage different types of medications with their specific properties and administration methods. Without proper factory pattern, creating medications leads to:

- **Inconsistent Initialization**: Each part of code creates medications differently
- **Tight Coupling**: Client code knows about all medication types
- **Error-Prone**: Easy to forget required properties for specific types
- **Hard to Extend**: Adding new medication types requires changes everywhere
- **Duplicate Logic**: Creation logic scattered throughout codebase

## Current Issues

```csharp
// ❌ PROBLEM: Manual creation scattered everywhere

// In Doctor Service
var tablet = new TabletMedication 
{ 
    Name = "Aspirin", 
    Dosage = "500mg",
    Color = "White",
    Shape = "Round"
    // But forgot: MaxPillsPerDay, InteractionChecks
};

// In Pharmacy Service
var injection = new Needle 
{ 
    Medication = "Penicillin",
    Volume = "10ml"
    // Missing: SterileHandling, StorageTemp, ExpiryDate
};

// In Nurse Station
var liquid = new LiquidMedication
{ 
    Name = "Cough Syrup",
    Volume = "100ml"
    // Missing: ShakeBeforeUse flag, DispensingCup requirement
};

// Result: Medications created inconsistently, properties missing, bugs in production!
```

## Real-World Impact

- Patient gets incomplete medication information
- Nurse forgets injection storage temperature → medication spoils
- Pharmacy dispenses wrong volume → incorrect dosage
- System crashes because required medication property is null
- Each medication type initialization is different

## Limitations

| Issue | Impact | Severity |
|-------|--------|----------|
| Inconsistent creation | Data errors | 🔴 Critical |
| Duplicate logic | Maintenance nightmare | 🟡 Medium |
| Hard to extend | New types require code changes | 🟡 Medium |
| Tight coupling | Tests difficult | 🟡 Medium |
| Error prone | Patient safety risk | 🔴 Critical |

## Solution Direction

We need a **Factory** that:
1. Centralizes medication creation logic
2. Ensures consistent initialization for each type
3. Hides concrete medication types from clients
4. Makes extending with new types easy

→ **SOLUTION: Factory Pattern**
