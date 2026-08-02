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
    /// Order Invoker: Executes commands and maintains full undo/redo history
    /// Features: Command execution, undo/redo stacks, history tracking, transaction support
    /// </summary>
    public class OrderInvoker
    {
        private Stack<ICommand> _undoStack = new();
        private Stack<ICommand> _redoStack = new();
        private List<ICommand> _executedHistory = new(); // Full history (never cleared)
        private int _transactionDepth = 0;
        private List<ICommand> _transactionCommands = new();

        /// <summary>
        /// Execute a command and add to undo stack
        /// </summary>
        public bool ExecuteCommand(ICommand command)
        {
            if (command.Execute())
            {
                _undoStack.Push(command);
                _executedHistory.Add(command);
                _redoStack.Clear(); // Clear redo when new command executed
                
                if (_transactionDepth > 0)
                {
                    _transactionCommands.Add(command);
                }
                
                Console.WriteLine($"✓ Command executed: {command.GetDescription()}");
                return true;
            }
            Console.WriteLine($"✗ Command failed: {command.GetDescription()}");
            return false;
        }

        /// <summary>
        /// Undo the last command
        /// </summary>
        public bool UndoCommand()
        {
            if (_undoStack.Count == 0) return false;
            
            var command = _undoStack.Pop();
            if (command.Undo())
            {
                _redoStack.Push(command);
                Console.WriteLine($"↶ Undo: {command.GetDescription()}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Redo the last undone command
        /// </summary>
        public bool RedoCommand()
        {
            if (_redoStack.Count == 0) return false;
            
            var command = _redoStack.Pop();
            if (command.Execute())
            {
                _undoStack.Push(command);
                Console.WriteLine($"↷ Redo: {command.GetDescription()}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Undo multiple commands at once
        /// </summary>
        public int UndoMultiple(int count)
        {
            int undone = 0;
            for (int i = 0; i < count && UndoCommand(); i++)
            {
                undone++;
            }
            return undone;
        }

        /// <summary>
        /// Redo multiple commands at once
        /// </summary>
        public int RedoMultiple(int count)
        {
            int redone = 0;
            for (int i = 0; i < count && RedoCommand(); i++)
            {
                redone++;
            }
            return redone;
        }

        /// <summary>
        /// Begin a transaction - groups commands together
        /// </summary>
        public void BeginTransaction()
        {
            _transactionDepth++;
            _transactionCommands.Clear();
            Console.WriteLine($"▢ Transaction started (depth: {_transactionDepth})");
        }

        /// <summary>
        /// Commit a transaction
        /// </summary>
        public bool CommitTransaction()
        {
            if (_transactionDepth <= 0)
            {
                Console.WriteLine("✗ No active transaction to commit");
                return false;
            }

            _transactionDepth--;
            Console.WriteLine($"✓ Transaction committed ({_transactionCommands.Count} commands, depth: {_transactionDepth})");
            _transactionCommands.Clear();
            return true;
        }

        /// <summary>
        /// Rollback a transaction - undo all commands in transaction
        /// </summary>
        public bool RollbackTransaction()
        {
            if (_transactionDepth <= 0)
            {
                Console.WriteLine("✗ No active transaction to rollback");
                return false;
            }

            int rolled = 0;
            while (_transactionCommands.Count > 0)
            {
                var cmd = _transactionCommands[_transactionCommands.Count - 1];
                if (cmd.Undo())
                {
                    _transactionCommands.RemoveAt(_transactionCommands.Count - 1);
                    rolled++;
                }
            }

            _transactionDepth--;
            Console.WriteLine($"↶ Transaction rolled back ({rolled} commands undone, depth: {_transactionDepth})");
            return true;
        }

        /// <summary>
        /// Get count of commands that can be undone
        /// </summary>
        public int GetUndoCount() => _undoStack.Count;

        /// <summary>
        /// Get count of commands that can be redone
        /// </summary>
        public int GetRedoCount() => _redoStack.Count;

        /// <summary>
        /// Get history of executed commands
        /// </summary>
        public List<string> GetHistory() => _undoStack.Select(c => c.GetDescription()).ToList();

        /// <summary>
        /// Get full execution history (all commands ever executed)
        /// </summary>
        public List<string> GetFullHistory() => _executedHistory.Select(c => c.GetDescription()).ToList();

        /// <summary>
        /// Get pending redo commands
        /// </summary>
        public List<string> GetRedoHistory() => _redoStack.Select(c => c.GetDescription()).ToList();

        /// <summary>
        /// Clear all undo/redo history
        /// </summary>
        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            Console.WriteLine("🗑 History cleared (undo/redo stacks)");
        }

        /// <summary>
        /// Check if undo is available
        /// </summary>
        public bool CanUndo() => _undoStack.Count > 0;

        /// <summary>
        /// Check if redo is available
        /// </summary>
        public bool CanRedo() => _redoStack.Count > 0;

        /// <summary>
        /// Get current undo stack as array for inspection
        /// </summary>
        public ICommand[] GetUndoStackSnapshot() => _undoStack.ToArray();

        /// <summary>
        /// Get current redo stack as array for inspection
        /// </summary>
        public ICommand[] GetRedoStackSnapshot() => _redoStack.ToArray();
    }
}
