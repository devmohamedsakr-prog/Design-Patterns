# PatientRegistry - After (Solution)

## Solution: Singleton Pattern

The Singleton pattern ensures only **one instance** of PatientRegistry exists throughout the application, accessible globally with thread-safe access.

## How It Works

### ✅ Single Instance
```csharp
public class PatientRegistry
{
    private static readonly Lazy<PatientRegistry> instance = 
        new Lazy<PatientRegistry>(() => new PatientRegistry());
    
    public static PatientRegistry Instance => instance.Value;
    
    private PatientRegistry() { } // Private constructor
}
```

### ✅ Thread-Safe Access
- Uses `Lazy<T>` for thread-safe lazy initialization
- Private constructor prevents direct instantiation
- All services use the same `PatientRegistry.Instance`

### ✅ Global Consistency
```csharp
public class PatientService
{
    public void RegisterPatient(Patient patient)
    {
        PatientRegistry.Instance.Add(patient); // Same instance!
    }
}

public class DiagnosisService
{
    public void UpdateDiagnosis(string patientId, string diagnosis)
    {
        var patient = PatientRegistry.Instance.GetPatient(patientId); // Same instance!
    }
}
```

## Benefits

| Benefit | Impact |
|---------|--------|
| Single Source of Truth | All services access same data |
| Data Consistency | No fragmentation or conflicts |
| Memory Efficient | Only one instance in memory |
| Thread Safe | Lazy<T> handles synchronization |
| Easy Debugging | Clear data flow |
| Simple API | Access via `Instance` property |

## Key Components

### `IPatientRegistry.cs`
Defines the contract for patient registry operations

### `PatientRegistry.cs`
- Singleton implementation with lazy initialization
- Thread-safe instance management
- Patient CRUD operations

### `DomainModels.cs`
- `Patient`: Patient data structure
- `MedicalRecord`: Patient's medical history

### `PatientService.cs`
- Uses `PatientRegistry.Instance`
- Registers new patients
- Retrieves patient information

### `DiagnosisService.cs`
- Updates patient diagnosis
- Thread-safe access to shared registry

### `MedicationService.cs`
- Prescribes medications
- Ensures data consistency across services

## Architecture

```
┌─────────────────────────────────────┐
│     PatientRegistry (Singleton)     │
│  - Private constructor              │
│  - Lazy initialization              │
│  - Static Instance property         │
│  - Thread-safe access               │
└──────────────────┬──────────────────┘
                   │
        ┌──────────┼──────────┐
        │          │          │
    ┌───▼──┐   ┌──▼───┐  ┌──▼────┐
    │Patient│   │Diagnosis│Medication│
    │Service│   │Service  │Service   │
    └───────┘   └────────┘ └────────┘
```

## Usage Pattern

```csharp
// ✅ All services access the SAME instance
var patientService = new PatientService();
var diagnosisService = new DiagnosisService();
var medicationService = new MedicationService();

// Add patient
var patient = new Patient { PatientId = "P001", Name = "John Doe" };
patientService.RegisterPatient(patient);

// Update diagnosis - sees the patient added above
diagnosisService.UpdateDiagnosis("P001", "Hypertension");

// Prescribe medication - sees the patient and diagnosis
medicationService.PrescribeMedication("P001", "Aspirin");

// All services see consistent data!
```

## Real-World Application

In healthcare systems:
- **Nurse Module**: Registers patients → writes to `PatientRegistry.Instance`
- **Doctor Module**: Updates diagnosis → reads from `PatientRegistry.Instance`
- **Pharmacy Module**: Prescribes meds → reads from `PatientRegistry.Instance`

All modules operate on the same patient data - no fragmentation!

## Test Coverage

- ✅ Single instance verification
- ✅ Lazy initialization
- ✅ Thread-safe concurrent access
- ✅ Data consistency across services
- ✅ Patient CRUD operations
- ✅ Medical record management
- ✅ 47+ comprehensive tests

## Files

- `PatientRegistry.csproj` - Project configuration
- `src/IPatientRegistry.cs` - Interface
- `src/PatientRegistry.cs` - Singleton implementation
- `src/DomainModels.cs` - Data models
- `src/PatientService.cs` - Patient management
- `src/DiagnosisService.cs` - Diagnosis management
- `src/MedicationService.cs` - Medication management
- `Tests/PatientRegistryTests.cs` - 47+ unit tests

## Building & Testing

```bash
dotnet build
dotnet test
```

All tests pass ✅
