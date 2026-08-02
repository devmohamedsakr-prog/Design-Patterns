using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentEditor.After.Context
{
    /// <summary>
    /// DocumentVersion: Memento - snapshot of document state
    /// </summary>
    public class DocumentVersionMemento
    {
        public string DocumentName { get; set; } = "";
        public string Content { get; set; } = "";
        public int CharacterCount { get; set; }
        public int WordCount { get; set; }
        public DateTime SaveTime { get; set; }
        public string VersionName { get; set; } = "";
        public string AuthorName { get; set; } = "";

        public DocumentVersionMemento(string docName, string content, string versionName, string author)
        {
            DocumentName = docName;
            Content = content;
            CharacterCount = content.Length;
            WordCount = content.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            VersionName = versionName;
            AuthorName = author;
            SaveTime = DateTime.Now;
        }

        public override string ToString() => 
            $"{VersionName} - {CharacterCount} chars, {WordCount} words ({SaveTime:HH:mm:ss})";
    }

    /// <summary>
    /// Document: Originator - manages document content
    /// </summary>
    public class Document
    {
        public string DocumentName { get; set; } = "";
        public string Content { get; set; } = "";
        public string Author { get; set; } = "";

        public Document(string name, string author)
        {
            DocumentName = name;
            Author = author;
        }

        public void AppendText(string text)
        {
            Content += text;
            Console.WriteLine($"  📝 Added {text.Length} characters");
        }

        public void ReplaceText(string oldText, string newText)
        {
            if (Content.Contains(oldText))
            {
                Content = Content.Replace(oldText, newText);
                Console.WriteLine($"  ✎ Replaced '{oldText}' with '{newText}'");
            }
        }

        public void ClearContent()
        {
            Content = "";
            Console.WriteLine($"  🗑️ Document cleared");
        }

        public int GetCharacterCount() => Content.Length;

        public int GetWordCount() => 
            Content.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

        public DocumentVersionMemento SaveVersion(string versionName)
        {
            var memento = new DocumentVersionMemento(DocumentName, Content, versionName, Author);
            Console.WriteLine($"💾 Version saved: {memento}");
            return memento;
        }

        public void RestoreVersion(DocumentVersionMemento memento)
        {
            Content = memento.Content;
            Console.WriteLine($"↶ Restored version: {memento}");
        }

        public override string ToString() => 
            $"{DocumentName} ({GetCharacterCount()} chars, {GetWordCount()} words) by {Author}";
    }

    /// <summary>
    /// DocumentVersionControl: Caretaker - manages document versions
    /// </summary>
    public class DocumentVersionControl
    {
        private Dictionary<string, DocumentVersionMemento> _versions = new();
        private List<DocumentVersionMemento> _versionHistory = new();
        private Stack<DocumentVersionMemento> _autoSaveStack = new();

        public void SaveVersion(Document document, string versionName)
        {
            var memento = document.SaveVersion(versionName);
            _versions[versionName] = memento;
            _versionHistory.Add(memento);
        }

        public void AutoSave(Document document)
        {
            var memento = new DocumentVersionMemento(document.DocumentName, document.Content, 
                $"AutoSave-{DateTime.Now:HH:mm:ss}", document.Author);
            _autoSaveStack.Push(memento);
            Console.WriteLine($"  💾 Auto-saved");
        }

        public void RestoreVersion(Document document, string versionName)
        {
            if (_versions.TryGetValue(versionName, out var memento))
            {
                document.RestoreVersion(memento);
            }
            else
            {
                Console.WriteLine($"✗ Version '{versionName}' not found");
            }
        }

        public void RestoreAutoSave(Document document)
        {
            if (_autoSaveStack.TryPop(out var memento))
            {
                document.RestoreVersion(memento);
            }
        }

        public List<string> GetAvailableVersions() => new(_versions.Keys);
        public List<DocumentVersionMemento> GetVersionHistory() => new(_versionHistory);
        public int GetVersionCount() => _versions.Count;
        public int GetAutoSaveCount() => _autoSaveStack.Count;
    }
}
