using NUnit.Framework;
using PatientStatus.After.Context;

namespace PatientStatus.After.Tests
{
    [TestFixture]
    public class PatientStatusTests
    {
        private Patient _patient;

        [SetUp]
        public void Setup()
        {
            _patient = new Patient("PAT-001", "John Doe");
        }

        [Test] public void Patient_Initial_CheckIn() => Assert.That(_patient.GetCurrentStateName(), Is.EqualTo("CheckIn"));
        [Test] public void Patient_CheckIn_Succeeds() => Assert.That(_patient.CheckIn("Dr. Smith"), Is.True);
        [Test] public void Patient_CheckIn_TransitionsToTreatment() 
        { 
            _patient.CheckIn("Dr. Smith");
            Assert.That(_patient.GetCurrentStateName(), Is.EqualTo("InTreatment"));
        }
        [Test] public void Patient_CheckIn_NullDoctor_Fails() => Assert.That(_patient.CheckIn(null), Is.False);
        [Test] public void Patient_InTreatment_CanStartTreatment()
        {
            _patient.CheckIn("Dr. Smith");
            Assert.That(_patient.StartTreatment("Flu"), Is.True);
        }
        [Test] public void Patient_InTreatment_CanDischarge()
        {
            _patient.CheckIn("Dr. Smith");
            _patient.StartTreatment("Flu");
            Assert.That(_patient.Discharge(), Is.True);
        }
        [Test] public void Patient_Discharge_TransitionsToDischarged()
        {
            _patient.CheckIn("Dr. Smith");
            _patient.StartTreatment("COVID");
            _patient.Discharge();
            Assert.That(_patient.GetCurrentStateName(), Is.EqualTo("Discharged"));
        }
        [Test] public void Patient_CannotDischargeWithoutDiagnosis()
        {
            _patient.CheckIn("Dr. Johnson");
            Assert.That(_patient.Discharge(), Is.False);
        }
        [Test] public void Patient_InTreatment_CanModifyDiagnosis()
        {
            _patient.CheckIn("Dr. Smith");
            Assert.That(_patient.CanModifyDiagnosis(), Is.True);
        }
        [Test] public void Patient_Discharged_CannotModify()
        {
            _patient.CheckIn("Dr. Smith");
            _patient.StartTreatment("Flu");
            _patient.Discharge();
            Assert.That(_patient.CanModifyDiagnosis(), Is.False);
        }
        [Test] public void Patient_FullWorkflow()
        {
            Assert.That(_patient.CheckIn("Dr. Brown"), Is.True);
            Assert.That(_patient.StartTreatment("Pneumonia"), Is.True);
            Assert.That(_patient.Discharge(), Is.True);
            Assert.That(_patient.GetCurrentStateName(), Is.EqualTo("Discharged"));
        }
        [Test] public void Patient_Multiple_Diagnoses()
        {
            _patient.CheckIn("Dr. Smith");
            _patient.StartTreatment("Fever");
            _patient.StartTreatment("Cough"); // Can update
            Assert.That(_patient.Diagnosis, Is.EqualTo("Cough"));
        }
        [Test] public void Patient_DoctorAssigned()
        {
            _patient.CheckIn("Dr. Anderson");
            Assert.That(_patient.Doctor, Is.EqualTo("Dr. Anderson"));
        }
    }
}
