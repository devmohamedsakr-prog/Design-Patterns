# PatientRegistry - Before (Problem)

## Problem Statement

A healthcare management system needs to access patient records globally across the application. Without proper synchronization, multiple patient registry instances are created, causing:

- **Data Inconsistency**: Different parts of the app read/write to different patient lists
- **Memory Waste**: Multiple copies of the same patient data in memory
- **Concurrency Issues**: Race conditions when updating patient records
- **Maintenance Nightmare**: Hard to track which registry instance is the "source of truth"

## Current Implementation Issues

```csharp
// ❌ PROBLEM: Each service creates its own registry
public class PatientService {
    private PatientRegistry registry = new PatientRegistry(); // Instance 1
    
    public void AddPatient(Patient p) {
        registry.Add(p);
    }
}

public class DiagnosisService {
    private PatientRegistry registry = new PatientRegistry(); // Instance 2 - DIFFERENT!
    
    public void UpdateDiagnosis(string patientId, string diagnosis) {
        var patient = registry.GetPatient(patientId); // Gets from WRONG registry
        patient.Diagnosis = diagnosis;
    }
}

// Result: PatientService and DiagnosisService operate on different patient lists!
```

## Real-World Impact

- Nurse adds patient to system → saved in PatientService.registry
- Doctor searches for patient → looks in DiagnosisService.registry (empty!)
- Patient data appears to be lost
- Medical records are fragmented
- System reliability decreases

## Limitations of Current Approach

| Issue | Impact | Severity |
|-------|--------|----------|
| Multiple instances | Data fragmentation | 🔴 Critical |
| No synchronization | Inconsistent state | 🔴 Critical |
| Memory overhead | Wasted resources | 🟡 Medium |
| Hard to debug | Difficult root cause analysis | 🟡 Medium |
| No thread safety | Race conditions | 🔴 Critical |

## Solution Direction

We need a way to ensure **only ONE instance** of PatientRegistry exists application-wide, accessible from anywhere, and thread-safe for concurrent access.

→ **SOLUTION: Singleton Pattern**
