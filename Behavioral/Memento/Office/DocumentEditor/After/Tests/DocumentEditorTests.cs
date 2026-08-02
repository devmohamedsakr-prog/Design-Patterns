using NUnit.Framework;
using DocumentEditor.After.Context;

namespace DocumentEditor.After.Tests
{
    [TestFixture]
    public class DocumentEditorMementoTests
    {
        private Document _doc;
        private DocumentVersionControl _versionControl;

        [SetUp]
        public void Setup()
        {
            _doc = new Document("Report.docx", "John");
            _versionControl = new DocumentVersionControl();
        }

        [Test]
        public void SaveVersion_Success()
        {
            _doc.AppendText("Introduction paragraph.");
            _versionControl.SaveVersion(_doc, "V1-Draft");
            
            Assert.That(_versionControl.GetVersionCount(), Is.EqualTo(1));
        }

        [Test]
        public void RestoreVersion()
        {
            _doc.AppendText("First draft content.");
            string v1Content = _doc.Content;
            _versionControl.SaveVersion(_doc, "V1");
            
            _doc.AppendText(" More content added.");
            _versionControl.SaveVersion(_doc, "V2");
            
            _versionControl.RestoreVersion(_doc, "V1");
            Assert.That(_doc.Content, Is.EqualTo(v1Content));
        }

        [Test]
        public void MultipleVersions()
        {
            _doc.AppendText("Version 1 text");
            _versionControl.SaveVersion(_doc, "V1");
            
            _doc.ClearContent();
            _doc.AppendText("Version 2 text");
            _versionControl.SaveVersion(_doc, "V2");
            
            _doc.ClearContent();
            _doc.AppendText("Version 3 text");
            _versionControl.SaveVersion(_doc, "V3");
            
            Assert.That(_versionControl.GetVersionCount(), Is.EqualTo(3));
        }

        [Test]
        public void ReplaceText_Restore()
        {
            _doc.AppendText("The quick brown fox jumps over the lazy dog");
            _versionControl.SaveVersion(_doc, "Original");
            
            _doc.ReplaceText("brown", "red");
            _versionControl.RestoreVersion(_doc, "Original");
            
            Assert.That(_doc.Content, Does.Contain("brown"));
            Assert.That(_doc.Content, Does.Not.Contain("red"));
        }

        [Test]
        public void WordCount_Version()
        {
            _doc.AppendText("One two three four five");
            int wordCount = _doc.GetWordCount();
            _versionControl.SaveVersion(_doc, "FiveWords");
            
            _doc.AppendText(" six seven eight");
            _versionControl.RestoreVersion(_doc, "FiveWords");
            
            Assert.That(_doc.GetWordCount(), Is.EqualTo(wordCount));
        }

        [Test]
        public void AutoSave()
        {
            _doc.AppendText("Important content");
            _versionControl.AutoSave(_doc);
            
            Assert.That(_versionControl.GetAutoSaveCount(), Is.EqualTo(1));
        }

        [Test]
        public void RestoreAutoSave()
        {
            _doc.AppendText("Content to preserve");
            string contentToPreserve = _doc.Content;
            _versionControl.AutoSave(_doc);
            
            _doc.ClearContent();
            _versionControl.RestoreAutoSave(_doc);
            
            Assert.That(_doc.Content, Is.EqualTo(contentToPreserve));
        }

        [Test]
        public void VersionHistory()
        {
            _doc.AppendText("Text 1");
            _versionControl.SaveVersion(_doc, "Save1");
            
            _doc.AppendText(" Text 2");
            _versionControl.SaveVersion(_doc, "Save2");
            
            var history = _versionControl.GetVersionHistory();
            Assert.That(history.Count, Is.EqualTo(2));
            Assert.That(history[0].CharacterCount, Is.LessThan(history[1].CharacterCount));
        }

        [Test]
        public void DocumentEvolution()
        {
            // Title
            _doc.AppendText("# My Report\n");
            _versionControl.SaveVersion(_doc, "Title");
            
            // Introduction
            _doc.AppendText("This is an introduction.\n");
            _versionControl.SaveVersion(_doc, "WithIntro");
            
            // Content
            _doc.AppendText("Main content goes here.\n");
            _versionControl.SaveVersion(_doc, "WithContent");
            
            // Back to intro only
            _versionControl.RestoreVersion(_doc, "WithIntro");
            Assert.That(_doc.GetCharacterCount(), Is.LessThan(60));
        }

        [Test]
        public void ComplexEditing()
        {
            _doc.AppendText("Original text");
            _versionControl.SaveVersion(_doc, "Original");
            
            _doc.ReplaceText("Original", "Modified");
            _doc.AppendText(" with additions");
            _versionControl.SaveVersion(_doc, "Modified");
            
            _doc.ReplaceText("additions", "extensive additions");
            _versionControl.SaveVersion(_doc, "Extended");
            
            _versionControl.RestoreVersion(_doc, "Original");
            Assert.That(_doc.Content, Is.EqualTo("Original text"));
        }

        [Test]
        public void GetAvailableVersions()
        {
            _versionControl.SaveVersion(_doc, "V1");
            _versionControl.SaveVersion(_doc, "V2");
            _versionControl.SaveVersion(_doc, "V3");
            
            var versions = _versionControl.GetAvailableVersions();
            Assert.That(versions.Count, Is.EqualTo(3));
            Assert.That(versions, Does.Contain("V1"));
        }

        [Test]
        public void VersionMetadata()
        {
            _doc.AppendText("Sample content");
            _versionControl.SaveVersion(_doc, "WithMetadata");
            
            var history = _versionControl.GetVersionHistory();
            Assert.That(history[0].DocumentName, Is.EqualTo("Report.docx"));
            Assert.That(history[0].AuthorName, Is.EqualTo("John"));
        }
    }
}
