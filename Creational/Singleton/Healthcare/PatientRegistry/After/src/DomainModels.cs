using System;
using System.Collections.Generic;

namespace PatientRegistry
{
    /// <summary>
    /// Represents a patient in the healthcare system
    /// </summary>
    public class Patient
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string BloodType { get; set; }
        public string Diagnosis { get; set; }
        public List<string> Medications { get; set; } = new();
        public List<MedicalRecord> MedicalRecords { get; set; } = new();
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;

        public override string ToString()
        {
            return $"Patient: {PatientId} - {Name} (Age: {Age})";
        }
    }

    /// <summary>
    /// Represents a medical record entry for a patient
    /// </summary>
    public class MedicalRecord
    {
        public string RecordId { get; set; }
        public string PatientId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public List<string> Medications { get; set; } = new();

        public override string ToString()
        {
            return $"Record: {RecordId} - {Diagnosis} ({DateCreated:yyyy-MM-dd})";
        }
    }
}
