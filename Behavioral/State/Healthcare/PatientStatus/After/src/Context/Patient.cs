using System;

namespace PatientStatus.After.Context
{
    /// <summary>
    /// Patient Context: CheckIn → InTreatment → Discharged
    /// </summary>
    public class Patient
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public IPatientState CurrentState { get; private set; }
        public string Doctor { get; set; }
        public string Diagnosis { get; set; }

        public Patient(string patientId, string name)
        {
            PatientId = patientId;
            Name = name;
            CurrentState = new CheckInState();
            Console.WriteLine($"[Patient {patientId}] Created - State: CheckIn");
        }

        public void TransitionTo(IPatientState newState)
        {
            string oldState = CurrentState.GetStateName();
            CurrentState = newState;
            Console.WriteLine($"[Patient {PatientId}] {oldState} → {newState.GetStateName()}");
        }

        public bool CheckIn(string doctor) => CurrentState.CheckIn(this, doctor);
        public bool StartTreatment(string diagnosis) => CurrentState.StartTreatment(this, diagnosis);
        public bool Discharge() => CurrentState.Discharge(this);
        public bool CanModifyDiagnosis() => CurrentState.CanModifyDiagnosis(this);

        public string GetCurrentStateName() => CurrentState.GetStateName();
    }

    public interface IPatientState
    {
        string GetStateName();
        bool CheckIn(Patient patient, string doctor);
        bool StartTreatment(Patient patient, string diagnosis);
        bool Discharge(Patient patient);
        bool CanModifyDiagnosis(Patient patient);
    }

    public class CheckInState : IPatientState
    {
        public string GetStateName() => "CheckIn";

        public bool CheckIn(Patient patient, string doctor)
        {
            if (string.IsNullOrEmpty(doctor)) return false;
            patient.Doctor = doctor;
            patient.TransitionTo(new InTreatmentState());
            Console.WriteLine($"✓ Patient assigned to Dr. {doctor}");
            return true;
        }

        public bool StartTreatment(Patient patient, string diagnosis) => false;
        public bool Discharge(Patient patient) => false;
        public bool CanModifyDiagnosis(Patient patient) => false;
    }

    public class InTreatmentState : IPatientState
    {
        public string GetStateName() => "InTreatment";

        public bool CheckIn(Patient patient, string doctor) => false;

        public bool StartTreatment(Patient patient, string diagnosis)
        {
            if (string.IsNullOrEmpty(diagnosis)) return false;
            patient.Diagnosis = diagnosis;
            Console.WriteLine($"✓ Treatment started: {diagnosis}");
            return true;
        }

        public bool Discharge(Patient patient)
        {
            if (string.IsNullOrEmpty(patient.Diagnosis)) return false;
            patient.TransitionTo(new DischargedState());
            Console.WriteLine($"✓ Patient discharged");
            return true;
        }

        public bool CanModifyDiagnosis(Patient patient) => true;
    }

    public class DischargedState : IPatientState
    {
        public string GetStateName() => "Discharged";

        public bool CheckIn(Patient patient, string doctor) => false;
        public bool StartTreatment(Patient patient, string diagnosis) => false;
        public bool Discharge(Patient patient) => false;
        public bool CanModifyDiagnosis(Patient patient) => false;
    }
}
