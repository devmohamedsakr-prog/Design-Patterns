using System;
using System.Collections.Generic;

namespace PatientRegistry
{
    /// <summary>
    /// Service for managing patient registration and retrieval
    /// Uses the Singleton PatientRegistry for centralized patient data
    /// </summary>
    public class PatientService
    {
        /// <summary>
        /// Register a new patient in the system
        /// ✅ Uses PatientRegistry.Instance - guaranteed single instance
        /// </summary>
        public void RegisterPatient(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            PatientRegistry.Instance.Add(patient);
            Console.WriteLine($"✓ Patient registered: {patient.Name}");
        }

        /// <summary>
        /// Get patient by ID
        /// ✅ Accesses the same PatientRegistry instance used by all services
        /// </summary>
        public Patient GetPatient(string patientId)
        {
            return PatientRegistry.Instance.GetPatient(patientId);
        }

        /// <summary>
        /// Get all registered patients
        /// </summary>
        public List<Patient> GetAllPatients()
        {
            return PatientRegistry.Instance.GetAll();
        }

        /// <summary>
        /// Update patient information
        /// </summary>
        public void UpdatePatient(Patient patient)
        {
            PatientRegistry.Instance.Update(patient);
            Console.WriteLine($"✓ Patient updated: {patient.Name}");
        }

        /// <summary>
        /// Check if patient exists
        /// </summary>
        public bool PatientExists(string patientId)
        {
            return PatientRegistry.Instance.Exists(patientId);
        }

        /// <summary>
        /// Get total number of registered patients
        /// ✅ All services see the SAME count
        /// </summary>
        public int GetPatientCount()
        {
            return PatientRegistry.Instance.Count;
        }
    }
}
