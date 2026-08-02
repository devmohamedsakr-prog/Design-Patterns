using NUnit.Framework;
using WorkflowState.After.Context;

namespace WorkflowState.After.Tests
{
    [TestFixture]
    public class WorkflowStateMementoTests
    {
        private WorkflowProcess _process;
        private WorkflowCheckpointManager _manager;

        [SetUp]
        public void Setup()
        {
            _process = new WorkflowProcess("PROC-001");
            _manager = new WorkflowCheckpointManager();
        }

        [Test]
        public void CreateCheckpoint_Success()
        {
            _process.TransitionTo("Submitted");
            _manager.CreateCheckpoint(_process, "CheckpointA");
            
            Assert.That(_manager.GetCheckpointCount(), Is.EqualTo(1));
        }

        [Test]
        public void SaveAndRestoreCheckpoint()
        {
            _process.AddData("OrderID", "12345");
            _process.TransitionTo("Submitted");
            string currentState = _process.CurrentState;
            
            _manager.CreateCheckpoint(_process, "Checkpoint1");
            
            _process.TransitionTo("Approved");
            _process.AddData("OrderID", "99999");
            
            _manager.RestoreCheckpoint(_process, "Checkpoint1");
            Assert.That(_process.CurrentState, Is.EqualTo(currentState));
            Assert.That(_process.GetData("OrderID"), Is.EqualTo("12345"));
        }

        [Test]
        public void WorkflowProgression()
        {
            _process.TransitionTo("Submitted");
            _manager.CreateCheckpoint(_process, "Step1-Submitted");
            
            _process.TransitionTo("Approved");
            _manager.CreateCheckpoint(_process, "Step2-Approved");
            
            _process.TransitionTo("Processing");
            _manager.CreateCheckpoint(_process, "Step3-Processing");
            
            Assert.That(_manager.GetCheckpointCount(), Is.EqualTo(3));
        }

        [Test]
        public void Rollback_ToLastCheckpoint()
        {
            _process.AddData("Status", "InProgress");
            _process.TransitionTo("Submitted");
            _manager.CreateCheckpoint(_process, "Safe");
            
            _process.TransitionTo("Approved");
            _process.AddData("Status", "Corrupted");
            
            _manager.Rollback(_process);
            Assert.That(_process.CurrentState, Is.EqualTo("Submitted"));
            Assert.That(_process.GetData("Status"), Is.EqualTo("InProgress"));
        }

        [Test]
        public void StateData_Persistence()
        {
            _process.AddData("UserID", "USER-101");
            _process.AddData("Department", "Sales");
            _manager.CreateCheckpoint(_process, "WithData");
            
            _process.ClearData();
            
            _manager.RestoreCheckpoint(_process, "WithData");
            Assert.That(_process.GetData("UserID"), Is.EqualTo("USER-101"));
            Assert.That(_process.GetData("Department"), Is.EqualTo("Sales"));
        }

        [Test]
        public void MultipleCheckpoints()
        {
            _manager.CreateCheckpoint(_process, "Checkpoint1");
            _process.TransitionTo("Submitted");
            _manager.CreateCheckpoint(_process, "Checkpoint2");
            _process.TransitionTo("Approved");
            _manager.CreateCheckpoint(_process, "Checkpoint3");
            
            var checkpoints = _manager.GetAvailableCheckpoints();
            Assert.That(checkpoints.Count, Is.EqualTo(3));
        }

        [Test]
        public void RollbackStack()
        {
            _manager.CreateCheckpoint(_process, "CP1");
            _process.TransitionTo("Submitted");
            _manager.CreateCheckpoint(_process, "CP2");
            _process.TransitionTo("Approved");
            _manager.CreateCheckpoint(_process, "CP3");
            
            Assert.That(_manager.GetStackDepth(), Is.EqualTo(3));
            
            _manager.Rollback(_process);
            Assert.That(_manager.GetStackDepth(), Is.EqualTo(2));
            Assert.That(_process.CurrentState, Is.EqualTo("Approved"));
        }

        [Test]
        public void InvalidStateTransition_AllowedForSnapshot()
        {
            _process.TransitionTo("Submitted");
            _manager.CreateCheckpoint(_process, "ValidState");
            
            // Try invalid transition (should fail, but checkpoint still exists)
            bool result = _process.TransitionTo("InvalidState");
            Assert.That(result, Is.False);
            
            _manager.RestoreCheckpoint(_process, "ValidState");
            Assert.That(_process.CurrentState, Is.EqualTo("Submitted"));
        }

        [Test]
        public void ComplexWorkflowScenario()
        {
            // Initial state
            _manager.CreateCheckpoint(_process, "Start");
            
            // First transition
            _process.AddData("RequestID", "REQ-001");
            _process.TransitionTo("Submitted");
            _manager.CreateCheckpoint(_process, "Submitted");
            
            // Approval
            _process.AddData("ApprovedBy", "Manager");
            _process.TransitionTo("Approved");
            _manager.CreateCheckpoint(_process, "Approved");
            
            // Processing
            _process.AddData("ProcessingID", "PROC-123");
            _process.TransitionTo("Processing");
            _manager.CreateCheckpoint(_process, "Processing");
            
            // Restore to approved state
            _manager.RestoreCheckpoint(_process, "Approved");
            Assert.That(_process.StepsCompleted, Is.EqualTo(2));
            Assert.That(_process.GetData("ProcessingID"), Is.Null);
        }

        [Test]
        public void StateDataIsolation()
        {
            _process.AddData("Key1", "Value1");
            _manager.CreateCheckpoint(_process, "Snap1");
            
            _process.AddData("Key1", "ModifiedValue");
            _process.AddData("Key2", "Value2");
            
            _manager.RestoreCheckpoint(_process, "Snap1");
            Assert.That(_process.GetData("Key1"), Is.EqualTo("Value1"));
            Assert.That(_process.GetData("Key2"), Is.Null);
        }

        [Test]
        public void CheckpointTimestamp()
        {
            _manager.CreateCheckpoint(_process, "TimedCP");
            var checkpoint = _manager.GetAvailableCheckpoints().First();
            
            // Just verify checkpoint was created
            Assert.That(checkpoint, Is.EqualTo("TimedCP"));
        }

        [Test]
        public void MultipleRollbacks()
        {
            _process.TransitionTo("Submitted");
            _manager.CreateCheckpoint(_process, "CP1");
            
            _process.TransitionTo("Approved");
            _manager.CreateCheckpoint(_process, "CP2");
            
            _process.TransitionTo("Processing");
            _manager.CreateCheckpoint(_process, "CP3");
            
            // Rollback twice
            _manager.Rollback(_process);
            Assert.That(_process.CurrentState, Is.EqualTo("Processing"));
            
            _manager.Rollback(_process);
            Assert.That(_process.CurrentState, Is.EqualTo("Approved"));
        }
    }
}
