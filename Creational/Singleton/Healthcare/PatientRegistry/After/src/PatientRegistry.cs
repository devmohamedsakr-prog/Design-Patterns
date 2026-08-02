using System;
using System.Collections.Generic;
using System.Linq;

namespace PatientRegistry
{
    /// <summary>
    /// Singleton implementation for centralized patient registry.
    /// Ensures only one instance manages all patient records application-wide.
    /// Thread-safe using Lazy<T> initialization.
    /// </summary>
    public class PatientRegistry : IPatientRegistry
    {
        // ✅ Lazy initialization ensures thread-safe singleton
        private static readonly Lazy<PatientRegistry> instance = 
            new Lazy<PatientRegistry>(() => new PatientRegistry());

        /// <summary>Get the singleton instance</summary>
        public static PatientRegistry Instance => instance.Value;

        // Private constructor prevents direct instantiation
        private PatientRegistry()
        {
            patients = new List<Patient>();
        }

        private readonly List<Patient> patients;

        /// <summary>Add a new patient to the registry</summary>
        public void Add(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            if (Exists(patient.PatientId))
                throw new InvalidOperationException($"Patient {patient.PatientId} already exists");

            patients.Add(patient);
        }

        /// <summary>Get patient by ID</summary>
        public Patient GetPatient(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
                throw new ArgumentException("PatientId cannot be null or empty", nameof(patientId));

            return patients.FirstOrDefault(p => p.PatientId == patientId);
        }

        /// <summary>Get all patients</summary>
        public List<Patient> GetAll()
        {
            return new List<Patient>(patients); // Return copy to prevent external modification
        }

        /// <summary>Update patient information</summary>
        public void Update(Patient patient)
        {
            if (patient == null)
                throw new ArgumentNullException(nameof(patient));

            var existing = GetPatient(patient.PatientId);
            if (existing == null)
                throw new KeyNotFoundException($"Patient {patient.PatientId} not found");

            existing.Name = patient.Name;
            existing.Age = patient.Age;
            existing.Diagnosis = patient.Diagnosis;
            existing.Medications = patient.Medications;
        }

        /// <summary>Remove patient from registry</summary>
        public bool Remove(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
                throw new ArgumentException("PatientId cannot be null or empty", nameof(patientId));

            var patient = GetPatient(patientId);
            if (patient != null)
            {
                patients.Remove(patient);
                return true;
            }
            return false;
        }

        /// <summary>Check if patient exists</summary>
        public bool Exists(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
                return false;

            return patients.Any(p => p.PatientId == patientId);
        }

        /// <summary>Total patient count</summary>
        public int Count => patients.Count;

        /// <summary>Clear all patients (for testing)</summary>
        public void Clear()
        {
            patients.Clear();
        }
    }
}
