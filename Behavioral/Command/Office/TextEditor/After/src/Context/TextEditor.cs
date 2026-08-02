using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TextEditor.After.Context
{
    public interface IEditCommand
    {
        bool Execute();
        bool Undo();
        string GetDescription();
    }

    public class EditorInvoker
    {
        private Stack<IEditCommand> _undoStack = new();
        private Stack<IEditCommand> _redoStack = new();

        public bool ExecuteEdit(IEditCommand command)
        {
            if (command.Execute())
            {
                _undoStack.Push(command);
                _redoStack.Clear();
                return true;
            }
            return false;
        }

        public bool Undo()
        {
            if (_undoStack.Count == 0) return false;
            var command = _undoStack.Pop();
            if (command.Undo())
            {
                _redoStack.Push(command);
                return true;
            }
            return false;
        }

        public bool Redo()
        {
            if (_redoStack.Count == 0) return false;
            var command = _redoStack.Pop();
            if (command.Execute())
            {
                _undoStack.Push(command);
                return true;
            }
            return false;
        }

        public int GetUndoCount() => _undoStack.Count;
    }

    /// <summary>
    /// Macro Recorder Extension: Records sequences of commands for playback
    /// </summary>
    public class MacroRecorder
    {
        private List<IEditCommand> _recordedCommands = new();
        private bool _isRecording = false;
        private string _macroName = "";

        public void StartRecording(string macroName)
        {
            _recordedCommands.Clear();
            _isRecording = true;
            _macroName = macroName;
        }

        public void RecordCommand(IEditCommand command)
        {
            if (_isRecording)
            {
                _recordedCommands.Add(command);
            }
        }

        public bool StopRecording()
        {
            if (!_isRecording) return false;
            _isRecording = false;
            return true;
        }

        public bool PlayMacro(EditorInvoker invoker)
        {
            if (_recordedCommands.Count == 0) return false;

            foreach (var cmd in _recordedCommands)
            {
                invoker.ExecuteEdit(cmd);
            }

            return true;
        }

        public int GetCommandCount() => _recordedCommands.Count;
        public bool IsRecording => _isRecording;
        public string GetMacroName() => _macroName;

        public void SaveMacro(string filePath)
        {
            var lines = new List<string>
            {
                $"Macro: {_macroName}",
                $"Commands: {_recordedCommands.Count}",
                "---"
            };

            foreach (var cmd in _recordedCommands)
            {
                lines.Add(cmd.GetDescription());
            }

            File.WriteAllLines(filePath, lines);
        }

        public List<string> GetMacroSummary()
        {
            return _recordedCommands.Select(c => c.GetDescription()).ToList();
        }
    }

    public class Document
    {
        public string Content { get; set; } = "";
        public int CursorPosition { get; set; } = 0;
    }

    public class InsertTextCommand : IEditCommand
    {
        private Document _doc;
        private string _text;

        public InsertTextCommand(Document doc, string text)
        {
            _doc = doc;
            _text = text;
        }

        public bool Execute()
        {
            if (string.IsNullOrEmpty(_text)) return false;
            _doc.Content = _doc.Content.Insert(_doc.CursorPosition, _text);
            _doc.CursorPosition += _text.Length;
            return true;
        }

        public bool Undo()
        {
            _doc.Content = _doc.Content.Remove(_doc.CursorPosition - _text.Length, _text.Length);
            _doc.CursorPosition -= _text.Length;
            return true;
        }

        public string GetDescription() => $"Insert '{_text}'";
    }

    public class DeleteTextCommand : IEditCommand
    {
        private Document _doc;
        private int _length;
        private string _deletedText;

        public DeleteTextCommand(Document doc, int length)
        {
            _doc = doc;
            _length = length;
        }

        public bool Execute()
        {
            if (_doc.CursorPosition + _length > _doc.Content.Length) return false;
            _deletedText = _doc.Content.Substring(_doc.CursorPosition, _length);
            _doc.Content = _doc.Content.Remove(_doc.CursorPosition, _length);
            return true;
        }

        public bool Undo()
        {
            _doc.Content = _doc.Content.Insert(_doc.CursorPosition, _deletedText);
            return true;
        }

        public string GetDescription() => $"Delete {_length} chars";
    }

    public class FormatTextCommand : IEditCommand
    {
        private Document _doc;
        private int _start;
        private int _length;
        private string _originalText;

        public FormatTextCommand(Document doc, int start, int length, string format)
        {
            _doc = doc;
            _start = start;
            _length = length;
            _originalText = _doc.Content.Substring(start, length);
        }

        public bool Execute()
        {
            var text = _doc.Content.Substring(_start, _length);
            var formatted = text.ToUpper();
            _doc.Content = _doc.Content.Remove(_start, _length).Insert(_start, formatted);
            return true;
        }

        public bool Undo()
        {
            _doc.Content = _doc.Content.Remove(_start, _length).Insert(_start, _originalText);
            return true;
        }

        public string GetDescription() => "Format text";
    }
}
