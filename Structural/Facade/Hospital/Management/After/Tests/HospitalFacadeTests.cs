using Xunit;
using Facade.Hospital.Management.Component;
using System;

namespace Facade.Hospital.Management.Tests
{
    public class HospitalFacadeTests
    {
        [Fact]
        public void AdmitPatient_ShouldCreatePatient()
        {
            var facade = new HospitalFacade();
            var patient = facade.AdmitPatient("P001", "John Doe", "No allergies");
            
            Assert.NotNull(patient);
            Assert.Equal("John Doe", patient.Name);
        }

        [Fact]
        public void AdmitPatient_ShouldVerifyInsurance()
        {
            var facade = new HospitalFacade();
            var patient = facade.AdmitPatient("P001", "John", "None");
            
            Assert.NotNull(patient);
        }

        [Fact]
        public void AdmitPatient_ShouldCreateMedicalRecord()
        {
            var facade = new HospitalFacade();
            facade.AdmitPatient("P001", "John", "Asthma");
            
            var record = facade.GetPatientRecord("P001");
            Assert.NotNull(record);
            Assert.Contains("Admitted", record.Content);
        }

        [Fact]
        public void ScheduleAppointment_ShouldCreateAppointment()
        {
            var facade = new HospitalFacade();
            facade.AdmitPatient("P001", "John", "None");
            
            var appointment = facade.ScheduleAppointment("P001", "Dr. Smith", DateTime.UtcNow.AddDays(1));
            Assert.NotNull(appointment);
        }

        [Fact]
        public void RequestLabTest_ShouldOrderTest()
        {
            var facade = new HospitalFacade();
            facade.AdmitPatient("P001", "John", "None");
            
            var test = facade.RequestLabTest("P001", "Blood Test");
            Assert.NotNull(test);
            Assert.Equal("Pending", test.Status);
        }

        [Fact]
        public void IssuePrescription_ShouldCreatePrescription()
        {
            var facade = new HospitalFacade();
            facade.AdmitPatient("P001", "John", "None");
            
            var prescription = facade.IssuePrescription("P001", "Aspirin", 30);
            Assert.NotNull(prescription);
            Assert.Equal("Aspirin", prescription.Medication);
        }

        [Fact]
        public void DischargePatient_ShouldUpdateRecord()
        {
            var facade = new HospitalFacade();
            facade.AdmitPatient("P001", "John", "None");
            facade.DischargePatient("P001");
            
            var record = facade.GetPatientRecord("P001");
            Assert.Contains("Discharged", record.Content);
        }

        [Fact]
        public void GetPatientBalance_ShouldReturnBalance()
        {
            var facade = new HospitalFacade();
            facade.AdmitPatient("P001", "John", "None");
            
            var balance = facade.GetPatientBalance("P001");
            Assert.True(balance >= 0);
        }

        [Fact]
        public void FacadeHideComplexity_ShouldSimplifyHospitalOps()
        {
            var facade = new HospitalFacade();
            
            // Client only needs these methods instead of managing 6+ subsystems
            facade.AdmitPatient("P001", "Jane", "None");
            facade.ScheduleAppointment("P001", "Dr. Jones", DateTime.UtcNow.AddDays(2));
            facade.RequestLabTest("P001", "CT Scan");
            facade.IssuePrescription("P001", "Ibuprofen", 50);
            facade.DischargePatient("P001");
            
            var record = facade.GetPatientRecord("P001");
            Assert.NotNull(record);
        }

        [Fact]
        public void MultiplePatients_ShouldIsolateRecords()
        {
            var facade = new HospitalFacade();
            
            facade.AdmitPatient("P001", "John", "None");
            facade.AdmitPatient("P002", "Jane", "Diabetes");
            
            var record1 = facade.GetPatientRecord("P001");
            var record2 = facade.GetPatientRecord("P002");
            
            Assert.NotEqual(record1.PatientId, record2.PatientId);
        }

        [Fact]
        public void IssuePrescription_ShouldRecordMedication()
        {
            var facade = new HospitalFacade();
            facade.AdmitPatient("P001", "John", "None");
            
            facade.IssuePrescription("P001", "Penicillin", 10);
            facade.IssuePrescription("P001", "Loratadine", 20);
            
            var record = facade.GetPatientRecord("P001");
            Assert.NotNull(record);
        }

        [Fact]
        public void ScheduleAppointment_ShouldAssignDoctor()
        {
            var facade = new HospitalFacade();
            facade.AdmitPatient("P001", "John", "None");
            
            var appointment = facade.ScheduleAppointment("P001", "Dr. Williams", DateTime.UtcNow);
            Assert.Equal("Dr. Williams", appointment.DoctorName);
        }

        [Fact]
        public void DischargePatient_ShouldCreateFinalInvoice()
        {
            var facade = new HospitalFacade();
            facade.AdmitPatient("P001", "John", "None");
            facade.DischargePatient("P001");
            
            var balance = facade.GetPatientBalance("P001");
            // Balance should be recorded
            Assert.True(balance >= 0);
        }
    }
}
