using System;
using System.Collections.Generic;

namespace PatientRegistry
{
    /// <summary>
    /// Service for managing patient medications
    /// Uses the Singleton PatientRegistry to ensure medication data is consistent across all services
    /// </summary>
    public class MedicationService
    {
        /// <summary>
        /// Prescribe medication to patient
        /// ✅ Updates medications in the shared PatientRegistry.Instance
        /// </summary>
        public void PrescribeMedication(string patientId, string medication)
        {
            if (string.IsNullOrEmpty(patientId))
                throw new ArgumentException("PatientId cannot be null or empty", nameof(patientId));

            if (string.IsNullOrEmpty(medication))
                throw new ArgumentException("Medication cannot be null or empty", nameof(medication));

            var patient = PatientRegistry.Instance.GetPatient(patientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient {patientId} not found");

            if (!patient.Medications.Contains(medication))
            {
                patient.Medications.Add(medication);
                Console.WriteLine($"✓ Prescribed {medication} to {patient.Name}");
            }
        }

        /// <summary>
        /// Remove medication from patient
        /// </summary>
        public void RemoveMedication(string patientId, string medication)
        {
            if (string.IsNullOrEmpty(patientId))
                throw new ArgumentException("PatientId cannot be null or empty", nameof(patientId));

            var patient = PatientRegistry.Instance.GetPatient(patientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient {patientId} not found");

            if (patient.Medications.Contains(medication))
            {
                patient.Medications.Remove(medication);
                Console.WriteLine($"✓ Removed {medication} from {patient.Name}");
            }
        }

        /// <summary>
        /// Get all medications for patient
        /// ✅ Reads from the shared PatientRegistry.Instance
        /// </summary>
        public List<string> GetMedications(string patientId)
        {
            var patient = PatientRegistry.Instance.GetPatient(patientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient {patientId} not found");

            return new List<string>(patient.Medications);
        }

        /// <summary>
        /// Check if patient is prescribed a medication
        /// </summary>
        public bool IsPrescribed(string patientId, string medication)
        {
            var patient = PatientRegistry.Instance.GetPatient(patientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient {patientId} not found");

            return patient.Medications.Contains(medication);
        }

        /// <summary>
        /// Get medication count for patient
        /// </summary>
        public int GetMedicationCount(string patientId)
        {
            var patient = PatientRegistry.Instance.GetPatient(patientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient {patientId} not found");

            return patient.Medications.Count;
        }
    }
}
