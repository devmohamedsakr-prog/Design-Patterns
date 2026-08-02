using NUnit.Framework;
using TextEditor.After.Context;

namespace TextEditor.After.Tests
{
    [TestFixture]
    public class TextEditorTests
    {
        private EditorInvoker _editor;
        private Document _doc;

        [SetUp]
        public void Setup()
        {
            _editor = new EditorInvoker();
            _doc = new Document();
        }

        [Test] public void InsertText_Succeeds() => Assert.That(_editor.ExecuteEdit(new InsertTextCommand(_doc, "Hello")), Is.True);
        [Test] public void InsertText_UpdatesContent()
        {
            _editor.ExecuteEdit(new InsertTextCommand(_doc, "Test"));
            Assert.That(_doc.Content, Is.EqualTo("Test"));
        }
        [Test] public void InsertText_UpdatesCursor()
        {
            _editor.ExecuteEdit(new InsertTextCommand(_doc, "Hi"));
            Assert.That(_doc.CursorPosition, Is.EqualTo(2));
        }
        [Test] public void InsertText_Undo()
        {
            _editor.ExecuteEdit(new InsertTextCommand(_doc, "Test"));
            _editor.Undo();
            Assert.That(_doc.Content, Is.Empty);
        }

        [Test] public void DeleteText_Succeeds()
        {
            _editor.ExecuteEdit(new InsertTextCommand(_doc, "Hello"));
            _doc.CursorPosition = 5;
            Assert.That(_editor.ExecuteEdit(new DeleteTextCommand(_doc, 2)), Is.False);
        }
        [Test] public void DeleteText_Undo()
        {
            _editor.ExecuteEdit(new InsertTextCommand(_doc, "Hello"));
            _doc.CursorPosition = 0;
            _editor.ExecuteEdit(new DeleteTextCommand(_doc, 2));
            _editor.Undo();
            Assert.That(_doc.Content, Is.EqualTo("Hello"));
        }

        [Test] public void FormatText_ToUpperCase()
        {
            _editor.ExecuteEdit(new InsertTextCommand(_doc, "hello"));
            _editor.ExecuteEdit(new FormatTextCommand(_doc, 0, 5, "uppercase"));
            Assert.That(_doc.Content, Is.EqualTo("HELLO"));
        }
        [Test] public void FormatText_Undo()
        {
            _editor.ExecuteEdit(new InsertTextCommand(_doc, "test"));
            _editor.ExecuteEdit(new FormatTextCommand(_doc, 0, 4, "uppercase"));
            _editor.Undo();
            Assert.That(_doc.Content, Is.EqualTo("test"));
        }

        [Test] public void Redo_Succeeds()
        {
            _editor.ExecuteEdit(new InsertTextCommand(_doc, "A"));
            _editor.Undo();
            Assert.That(_editor.Redo(), Is.True);
            Assert.That(_doc.Content, Is.EqualTo("A"));
        }

        [Test] public void MultipleUndo()
        {
            _editor.ExecuteEdit(new InsertTextCommand(_doc, "A"));
            _editor.ExecuteEdit(new InsertTextCommand(_doc, "B"));
            _editor.Undo();
            _editor.Undo();
            Assert.That(_doc.Content, Is.Empty);
        }

        [Test] public void EmptyInsert_Fails() => Assert.That(_editor.ExecuteEdit(new InsertTextCommand(_doc, "")), Is.False);
        [Test] public void UndoCount_Tracked()
        {
            _editor.ExecuteEdit(new InsertTextCommand(_doc, "Test"));
            Assert.That(_editor.GetUndoCount(), Is.EqualTo(1));
        }
    }
}
