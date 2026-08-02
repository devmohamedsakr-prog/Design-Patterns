using System.Collections.Generic;

namespace PatientRegistry
{
    /// <summary>
    /// Contract for patient registry operations in healthcare system
    /// </summary>
    public interface IPatientRegistry
    {
        /// <summary>Add a new patient to the registry</summary>
        void Add(Patient patient);

        /// <summary>Get patient by ID</summary>
        Patient GetPatient(string patientId);

        /// <summary>Get all patients</summary>
        List<Patient> GetAll();

        /// <summary>Update patient information</summary>
        void Update(Patient patient);

        /// <summary>Remove patient from registry</summary>
        bool Remove(string patientId);

        /// <summary>Check if patient exists</summary>
        bool Exists(string patientId);

        /// <summary>Total patient count</summary>
        int Count { get; }

        /// <summary>Clear all patients (for testing)</summary>
        void Clear();
    }
}
