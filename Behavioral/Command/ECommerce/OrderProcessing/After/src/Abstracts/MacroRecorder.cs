using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace OrderProcessing.After.Abstracts
{
    /// <summary>
    /// Macro Recorder: Records command sequences for replay (Macro Pattern with Command)
    /// Allows recording complex workflows (e.g., Validate→Payment→Reserve) and replaying them as a single macro
    /// </summary>
    public class MacroRecorder
    {
        private List<ICommand> _recordedCommands = new();
        private bool _isRecording = false;
        private string _macroName = "";
        private DateTime _recordingStartTime;

        /// <summary>
        /// Start recording commands into a macro
        /// </summary>
        public void StartRecording(string macroName)
        {
            _recordedCommands.Clear();
            _isRecording = true;
            _macroName = macroName;
            _recordingStartTime = DateTime.Now;
            Console.WriteLine($"🔴 Macro Recording Started: '{macroName}'");
        }

        /// <summary>
        /// Record a command during macro recording
        /// </summary>
        public void RecordCommand(ICommand command)
        {
            if (!_isRecording) return;
            _recordedCommands.Add(command);
            Console.WriteLine($"  ⏺ Recorded: {command.GetDescription()}");
        }

        /// <summary>
        /// Stop recording and return command count
        /// </summary>
        public bool StopRecording()
        {
            if (!_isRecording) return false;
            _isRecording = false;
            var duration = DateTime.Now - _recordingStartTime;
            Console.WriteLine($"⏹ Recording Stopped. Macro '{_macroName}' has {_recordedCommands.Count} steps ({duration.TotalSeconds:F2}s)");
            return true;
        }

        /// <summary>
        /// Replay entire macro sequence
        /// </summary>
        public bool PlayMacro(OrderInvoker invoker)
        {
            if (_recordedCommands.Count == 0)
            {
                Console.WriteLine("⚠ No commands recorded in macro");
                return false;
            }

            Console.WriteLine($"▶️ Playing Macro: '{_macroName}' ({_recordedCommands.Count} steps)");
            int executed = 0;
            
            foreach (var cmd in _recordedCommands)
            {
                if (invoker.ExecuteCommand(cmd))
                {
                    executed++;
                }
            }

            Console.WriteLine($"✓ Macro Complete: {executed}/{_recordedCommands.Count} steps executed");
            return executed == _recordedCommands.Count;
        }

        /// <summary>
        /// Play macro with step-by-step control
        /// </summary>
        public bool PlayMacroStep(OrderInvoker invoker, int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= _recordedCommands.Count)
                return false;

            return invoker.ExecuteCommand(_recordedCommands[stepIndex]);
        }

        /// <summary>
        /// Get total commands in macro
        /// </summary>
        public int GetCommandCount() => _recordedCommands.Count;

        /// <summary>
        /// Check if currently recording
        /// </summary>
        public bool IsRecording => _isRecording;

        /// <summary>
        /// Get macro name
        /// </summary>
        public string GetMacroName() => _macroName;

        /// <summary>
        /// Get all recorded command descriptions
        /// </summary>
        public List<string> GetMacroSummary() => _recordedCommands.Select(c => c.GetDescription()).ToList();

        /// <summary>
        /// Save macro to file
        /// </summary>
        public void SaveMacro(string filePath)
        {
            var lines = new List<string>
            {
                $"Macro: {_macroName}",
                $"Created: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                $"Commands: {_recordedCommands.Count}",
                "---"
            };

            foreach (var cmd in _recordedCommands)
            {
                lines.Add(cmd.GetDescription());
            }

            File.WriteAllLines(filePath, lines);
            Console.WriteLine($"💾 Macro saved to: {filePath}");
        }

        /// <summary>
        /// Clear recorded commands
        /// </summary>
        public void ClearMacro()
        {
            _recordedCommands.Clear();
            _isRecording = false;
            Console.WriteLine($"🗑 Macro '{_macroName}' cleared");
        }
    }

    /// <summary>
    /// Enhanced Order Invoker with Macro Support
    /// </summary>
    public class OrderInvokerWithMacro : OrderInvoker
    {
        private MacroRecorder _macroRecorder = new();

        public MacroRecorder GetMacroRecorder() => _macroRecorder;

        /// <summary>
        /// Override ExecuteCommand to record during macro recording
        /// </summary>
        public new bool ExecuteCommand(ICommand command)
        {
            if (_macroRecorder.IsRecording)
            {
                _macroRecorder.RecordCommand(command);
            }

            return base.ExecuteCommand(command);
        }
    }
}
