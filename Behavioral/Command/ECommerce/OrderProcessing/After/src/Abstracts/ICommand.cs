using System;

namespace OrderProcessing.After.Abstracts
{
    /// <summary>
    /// Command Interface: Encapsulates order processing steps
    /// </summary>
    public interface ICommand
    {
        bool Execute();
        bool Undo();
        string GetDescription();
    }

    /// <summary>
    /// Order Invoker: Executes commands and maintains history
    /// </summary>
    public class OrderInvoker
    {
        private Stack<ICommand> _commandHistory = new();

        public bool ExecuteCommand(ICommand command)
        {
            if (command.Execute())
            {
                _commandHistory.Push(command);
                Console.WriteLine($"✓ Command executed: {command.GetDescription()}");
                return true;
            }
            Console.WriteLine($"✗ Command failed: {command.GetDescription()}");
            return false;
        }

        public bool UndoCommand()
        {
            if (_commandHistory.Count == 0) return false;
            
            var command = _commandHistory.Pop();
            if (command.Undo())
            {
                Console.WriteLine($"✓ Undo: {command.GetDescription()}");
                return true;
            }
            return false;
        }

        public int GetHistoryCount() => _commandHistory.Count;
        public List<string> GetHistory() => _commandHistory.Select(c => c.GetDescription()).ToList();
    }
}
