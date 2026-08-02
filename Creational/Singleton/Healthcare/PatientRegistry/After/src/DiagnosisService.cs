using System;

namespace PatientRegistry
{
    /// <summary>
    /// Service for managing patient diagnoses
    /// Uses the Singleton PatientRegistry to ensure diagnosis updates are visible to all services
    /// </summary>
    public class DiagnosisService
    {
        /// <summary>
        /// Update patient diagnosis
        /// ✅ Updates the patient in the shared PatientRegistry.Instance
        /// </summary>
        public void UpdateDiagnosis(string patientId, string diagnosis)
        {
            if (string.IsNullOrEmpty(patientId))
                throw new ArgumentException("PatientId cannot be null or empty", nameof(patientId));

            var patient = PatientRegistry.Instance.GetPatient(patientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient {patientId} not found");

            patient.Diagnosis = diagnosis;
            Console.WriteLine($"✓ Diagnosis updated for {patient.Name}: {diagnosis}");
        }

        /// <summary>
        /// Get patient diagnosis
        /// ✅ Reads from the shared PatientRegistry.Instance
        /// </summary>
        public string GetDiagnosis(string patientId)
        {
            var patient = PatientRegistry.Instance.GetPatient(patientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient {patientId} not found");

            return patient.Diagnosis ?? "No diagnosis recorded";
        }

        /// <summary>
        /// Add medical record for patient
        /// </summary>
        public void AddMedicalRecord(string patientId, MedicalRecord record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            var patient = PatientRegistry.Instance.GetPatient(patientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient {patientId} not found");

            record.PatientId = patientId;
            patient.MedicalRecords.Add(record);
            Console.WriteLine($"✓ Medical record added for {patient.Name}");
        }

        /// <summary>
        /// Get all medical records for patient
        /// </summary>
        public int GetMedicalRecordCount(string patientId)
        {
            var patient = PatientRegistry.Instance.GetPatient(patientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient {patientId} not found");

            return patient.MedicalRecords.Count;
        }
    }
}
