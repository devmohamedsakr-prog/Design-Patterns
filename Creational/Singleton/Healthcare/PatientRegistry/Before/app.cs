using System;
using System.Collections.Generic;
using System.Linq;

// ❌ PROBLEM: Multiple instances cause data inconsistency

public class Patient
{
    public string PatientId { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Diagnosis { get; set; }
    public List<string> Medications { get; set; } = new();
}

public class PatientRegistry
{
    private List<Patient> patients = new();

    public void Add(Patient patient)
    {
        if (!patients.Any(p => p.PatientId == patient.PatientId))
        {
            patients.Add(patient);
            Console.WriteLine($"✓ Added patient: {patient.Name}");
        }
    }

    public Patient GetPatient(string patientId)
    {
        return patients.FirstOrDefault(p => p.PatientId == patientId);
    }

    public List<Patient> GetAll()
    {
        return patients;
    }

    public int Count => patients.Count;
}

public class PatientService
{
    // ❌ Creates its own instance
    private PatientRegistry registry = new PatientRegistry();

    public void RegisterPatient(Patient patient)
    {
        registry.Add(patient);
    }

    public int GetPatientCount()
    {
        return registry.Count;
    }
}

public class DiagnosisService
{
    // ❌ Creates its own instance (DIFFERENT from PatientService!)
    private PatientRegistry registry = new PatientRegistry();

    public void UpdateDiagnosis(string patientId, string diagnosis)
    {
        var patient = registry.GetPatient(patientId);
        if (patient != null)
        {
            patient.Diagnosis = diagnosis;
            Console.WriteLine($"✓ Updated diagnosis for: {patient.Name}");
        }
        else
        {
            Console.WriteLine($"✗ Patient {patientId} not found!"); // ← PROBLEM!
        }
    }

    public int GetPatientCount()
    {
        return registry.Count;
    }
}

public class MedicationService
{
    // ❌ Creates its own instance (DIFFERENT from both!)
    private PatientRegistry registry = new PatientRegistry();

    public void PrescribeMedication(string patientId, string medication)
    {
        var patient = registry.GetPatient(patientId);
        if (patient != null)
        {
            patient.Medications.Add(medication);
            Console.WriteLine($"✓ Prescribed: {medication}");
        }
        else
        {
            Console.WriteLine($"✗ Cannot prescribe - patient {patientId} not found!");
        }
    }

    public int GetPatientCount()
    {
        return registry.Count;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("❌ PROBLEM: Multiple PatientRegistry Instances\n");

        var patientService = new PatientService();
        var diagnosisService = new DiagnosisService();
        var medicationService = new MedicationService();

        // Add patient through PatientService
        var patient = new Patient
        {
            PatientId = "P001",
            Name = "John Doe",
            Age = 45,
            Diagnosis = "Initial"
        };

        patientService.RegisterPatient(patient);
        Console.WriteLine($"Patient count in PatientService: {patientService.GetPatientCount()}");

        // Try to access from DiagnosisService
        Console.WriteLine($"Patient count in DiagnosisService: {diagnosisService.GetPatientCount()}");
        Console.WriteLine("⚠️  PROBLEM: Different services see different patient counts!\n");

        // Try to update diagnosis
        diagnosisService.UpdateDiagnosis("P001", "Hypertension");
        Console.WriteLine("⚠️  Patient not found - data is fragmented!\n");

        // Try to prescribe medication
        medicationService.PrescribeMedication("P001", "Aspirin");
        Console.WriteLine("⚠️  Cannot find patient for medication prescription!\n");

        Console.WriteLine("Summary:");
        Console.WriteLine($"  PatientService sees: {patientService.GetPatientCount()} patient(s)");
        Console.WriteLine($"  DiagnosisService sees: {diagnosisService.GetPatientCount()} patient(s)");
        Console.WriteLine($"  MedicationService sees: {medicationService.GetPatientCount()} patient(s)");
        Console.WriteLine("\n❌ ISSUE: Three different views of the same data!");
    }
}
