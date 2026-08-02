using System;
using System.Collections.Generic;

namespace WorkflowState.After.Context
{
    /// <summary>
    /// WorkflowStateMemento: Snapshot of workflow progress
    /// </summary>
    public class WorkflowStateMemento
    {
        public string ProcessId { get; set; } = "";
        public string CurrentState { get; set; } = "";
        public int StepsCompleted { get; set; }
        public int TotalSteps { get; set; }
        public Dictionary<string, string> StateData { get; set; } = new();
        public DateTime CheckpointTime { get; set; }
        public string CheckpointName { get; set; } = "";

        public WorkflowStateMemento(string processId, string currentState, int stepsCompleted, 
            int totalSteps, Dictionary<string, string> data, string checkpointName)
        {
            ProcessId = processId;
            CurrentState = currentState;
            StepsCompleted = stepsCompleted;
            TotalSteps = totalSteps;
            StateData = new Dictionary<string, string>(data);
            CheckpointName = checkpointName;
            CheckpointTime = DateTime.Now;
        }

        public override string ToString() => 
            $"{CheckpointName} - {CurrentState} ({StepsCompleted}/{TotalSteps}) at {CheckpointTime:HH:mm:ss}";
    }

    /// <summary>
    /// WorkflowProcess: Originator - manages workflow state
    /// </summary>
    public class WorkflowProcess
    {
        public string ProcessId { get; set; } = "";
        public string CurrentState { get; set; } = "Draft";
        public int StepsCompleted { get; set; } = 0;
        public int TotalSteps { get; set; } = 5;
        public Dictionary<string, string> StateData { get; set; } = new();

        private readonly List<string> _validStates = new() 
        { 
            "Draft", "Submitted", "Approved", "Processing", "Completed", "Failed" 
        };

        public WorkflowProcess(string processId)
        {
            ProcessId = processId;
        }

        public bool TransitionTo(string newState)
        {
            if (!_validStates.Contains(newState))
                return false;

            CurrentState = newState;
            StepsCompleted++;
            Console.WriteLine($"  ➜ Workflow transitioned to: {newState} (Step {StepsCompleted}/{TotalSteps})");
            return true;
        }

        public void AddData(string key, string value)
        {
            StateData[key] = value;
            Console.WriteLine($"  📋 Data added: {key} = {value}");
        }

        public string? GetData(string key) => StateData.TryGetValue(key, out var value) ? value : null;

        public void ClearData()
        {
            StateData.Clear();
            Console.WriteLine($"  🗑️ All data cleared");
        }

        public WorkflowStateMemento CreateCheckpoint(string checkpointName)
        {
            var memento = new WorkflowStateMemento(ProcessId, CurrentState, StepsCompleted, 
                TotalSteps, StateData, checkpointName);
            Console.WriteLine($"📍 Checkpoint created: {memento}");
            return memento;
        }

        public void RestoreFromCheckpoint(WorkflowStateMemento memento)
        {
            CurrentState = memento.CurrentState;
            StepsCompleted = memento.StepsCompleted;
            TotalSteps = memento.TotalSteps;
            StateData = new Dictionary<string, string>(memento.StateData);
            Console.WriteLine($"↶ Restored from checkpoint: {memento}");
        }

        public override string ToString() => 
            $"{ProcessId} - {CurrentState} (Progress: {StepsCompleted}/{TotalSteps})";
    }

    /// <summary>
    /// WorkflowCheckpointManager: Caretaker - manages workflow checkpoints
    /// </summary>
    public class WorkflowCheckpointManager
    {
        private Dictionary<string, WorkflowStateMemento> _checkpoints = new();
        private Stack<WorkflowStateMemento> _checkpointStack = new();

        public void CreateCheckpoint(WorkflowProcess process, string checkpointName)
        {
            var memento = process.CreateCheckpoint(checkpointName);
            _checkpoints[checkpointName] = memento;
            _checkpointStack.Push(memento);
        }

        public void RestoreCheckpoint(WorkflowProcess process, string checkpointName)
        {
            if (_checkpoints.TryGetValue(checkpointName, out var memento))
            {
                process.RestoreFromCheckpoint(memento);
            }
            else
            {
                Console.WriteLine($"✗ Checkpoint '{checkpointName}' not found");
            }
        }

        public bool Rollback(WorkflowProcess process)
        {
            if (_checkpointStack.TryPop(out var memento))
            {
                process.RestoreFromCheckpoint(memento);
                return true;
            }
            return false;
        }

        public List<string> GetAvailableCheckpoints() => new(_checkpoints.Keys);
        public int GetCheckpointCount() => _checkpoints.Count;
        public int GetStackDepth() => _checkpointStack.Count;
    }
}
