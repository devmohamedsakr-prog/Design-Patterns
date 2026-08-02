using System;
using System.Collections.Generic;

namespace Facade.Hospital.Management.Component
{
    // Subsystem 1: Patient Registration
    public class PatientRegistry
    {
        private Dictionary<string, Patient> _patients = new();

        public Patient RegisterPatient(string patientId, string name, string medicalHistory)
        {
            var patient = new Patient { PatientId = patientId, Name = name, MedicalHistory = medicalHistory };
            _patients[patientId] = patient;
            return patient;
        }

        public Patient GetPatient(string patientId) => _patients.ContainsKey(patientId) ? _patients[patientId] : null;
    }

    public class Patient
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public string MedicalHistory { get; set; }
    }

    // Subsystem 2: Appointment Scheduling
    public class AppointmentScheduler
    {
        private Dictionary<string, Appointment> _appointments = new();
        private int _appointmentCounter = 0;

        public Appointment ScheduleAppointment(string patientId, string doctorName, DateTime time)
        {
            var appointmentId = $"APT{++_appointmentCounter}";
            var appointment = new Appointment 
            { 
                AppointmentId = appointmentId, 
                PatientId = patientId, 
                DoctorName = doctorName, 
                ScheduledTime = time 
            };
            _appointments[appointmentId] = appointment;
            return appointment;
        }

        public IReadOnlyList<Appointment> GetPatientAppointments(string patientId) =>
            _appointments.Values.Where(a => a.PatientId == patientId).ToList().AsReadOnly();
    }

    public class Appointment
    {
        public string AppointmentId { get; set; }
        public string PatientId { get; set; }
        public string DoctorName { get; set; }
        public DateTime ScheduledTime { get; set; }
    }

    // Subsystem 3: Billing System
    public class BillingManager
    {
        private Dictionary<string, Invoice> _invoices = new();
        private int _invoiceCounter = 0;

        public Invoice CreateInvoice(string patientId, decimal amount, string description)
        {
            var invoiceId = $"INV{++_invoiceCounter}";
            var invoice = new Invoice 
            { 
                InvoiceId = invoiceId, 
                PatientId = patientId, 
                Amount = amount, 
                Description = description,
                Status = "Pending"
            };
            _invoices[invoiceId] = invoice;
            return invoice;
        }

        public bool VerifyInsurance(string patientId)
        {
            return !string.IsNullOrEmpty(patientId);
        }

        public decimal GetOutstandingBalance(string patientId) =>
            _invoices.Values.Where(i => i.PatientId == patientId && i.Status == "Pending").Sum(i => i.Amount);
    }

    public class Invoice
    {
        public string InvoiceId { get; set; }
        public string PatientId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

    // Subsystem 4: Pharmacy Management
    public class PharmacyManager
    {
        private List<Prescription> _prescriptions = new();

        public Prescription IssuePrescription(string patientId, string medication, int quantity)
        {
            var prescription = new Prescription 
            { 
                PatientId = patientId, 
                Medication = medication, 
                Quantity = quantity,
                IssuedDate = DateTime.UtcNow
            };
            _prescriptions.Add(prescription);
            return prescription;
        }

        public IReadOnlyList<Prescription> GetPatientPrescriptions(string patientId) =>
            _prescriptions.FindAll(p => p.PatientId == patientId).AsReadOnly();
    }

    public class Prescription
    {
        public string PatientId { get; set; }
        public string Medication { get; set; }
        public int Quantity { get; set; }
        public DateTime IssuedDate { get; set; }
    }

    // Subsystem 5: Lab Management
    public class LabManager
    {
        private Dictionary<string, LabTest> _tests = new();
        private int _testCounter = 0;

        public LabTest OrderLabTest(string patientId, string testType)
        {
            var testId = $"LAB{++_testCounter}";
            var test = new LabTest 
            { 
                TestId = testId, 
                PatientId = patientId, 
                TestType = testType,
                Status = "Pending"
            };
            _tests[testId] = test;
            return test;
        }

        public void CompleteTest(string testId, string result)
        {
            if (_tests.ContainsKey(testId))
            {
                _tests[testId].Status = "Completed";
                _tests[testId].Result = result;
            }
        }

        public IReadOnlyList<LabTest> GetPatientTests(string patientId) =>
            _tests.Values.Where(t => t.PatientId == patientId).ToList().AsReadOnly();
    }

    public class LabTest
    {
        public string TestId { get; set; }
        public string PatientId { get; set; }
        public string TestType { get; set; }
        public string Status { get; set; }
        public string Result { get; set; }
    }

    // Subsystem 6: Medical Records
    public class MedicalRecordsManager
    {
        private Dictionary<string, MedicalRecord> _records = new();

        public MedicalRecord CreateRecord(string patientId, string content)
        {
            var record = new MedicalRecord 
            { 
                PatientId = patientId, 
                Content = content,
                CreatedDate = DateTime.UtcNow
            };
            _records[patientId] = record;
            return record;
        }

        public MedicalRecord GetRecord(string patientId) =>
            _records.ContainsKey(patientId) ? _records[patientId] : null;
    }

    public class MedicalRecord
    {
        public string PatientId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    // FACADE: Simplifies hospital operations
    public class HospitalFacade
    {
        private PatientRegistry _patientRegistry = new();
        private AppointmentScheduler _appointmentScheduler = new();
        private BillingManager _billingManager = new();
        private PharmacyManager _pharmacyManager = new();
        private LabManager _labManager = new();
        private MedicalRecordsManager _medicalRecords = new();

        public Patient AdmitPatient(string patientId, string name, string medicalHistory)
        {
            var patient = _patientRegistry.RegisterPatient(patientId, name, medicalHistory);
            _billingManager.VerifyInsurance(patientId);
            _medicalRecords.CreateRecord(patientId, $"Admitted: {name}");
            return patient;
        }

        public Appointment ScheduleAppointment(string patientId, string doctorName, DateTime time)
        {
            return _appointmentScheduler.ScheduleAppointment(patientId, doctorName, time);
        }

        public LabTest RequestLabTest(string patientId, string testType)
        {
            return _labManager.OrderLabTest(patientId, testType);
        }

        public Prescription IssuePrescription(string patientId, string medication, int quantity)
        {
            return _pharmacyManager.IssuePrescription(patientId, medication, quantity);
        }

        public void DischargePatient(string patientId)
        {
            var record = _medicalRecords.GetRecord(patientId);
            if (record != null)
                record.Content += $"\nDischarged: {DateTime.UtcNow}";
            
            var balance = _billingManager.GetOutstandingBalance(patientId);
            if (balance > 0)
                _billingManager.CreateInvoice(patientId, balance, "Final discharge invoice");
        }

        public decimal GetPatientBalance(string patientId) =>
            _billingManager.GetOutstandingBalance(patientId);

        public MedicalRecord GetPatientRecord(string patientId) =>
            _medicalRecords.GetRecord(patientId);
    }
}
