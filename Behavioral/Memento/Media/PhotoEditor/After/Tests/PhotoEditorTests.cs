using NUnit.Framework;
using PhotoEditor.After.Context;

namespace PhotoEditor.After.Tests
{
    [TestFixture]
    public class PhotoEditorMementoTests
    {
        private Photo _photo;
        private PhotoEditorCaretaker _caretaker;

        [SetUp]
        public void Setup()
        {
            _photo = new Photo("vacation.jpg");
            _caretaker = new PhotoEditorCaretaker();
        }

        [Test]
        public void SavePhotoSnapshot()
        {
            _photo.AdjustBrightness(120);
            _caretaker.SaveSnapshot(_photo, "Brightened");
            
            Assert.That(_caretaker.GetSnapshotCount(), Is.EqualTo(1));
        }

        [Test]
        public void RestorePhotoSnapshot()
        {
            _photo.AdjustBrightness(120);
            _photo.AdjustContrast(110);
            int originalBrightness = _photo.Brightness;
            int originalContrast = _photo.Contrast;
            
            _caretaker.SaveSnapshot(_photo, "Original");
            
            _photo.ResetToDefaults();
            _caretaker.RestoreSnapshot(_photo, "Original");
            
            Assert.That(_photo.Brightness, Is.EqualTo(originalBrightness));
            Assert.That(_photo.Contrast, Is.EqualTo(originalContrast));
        }

        [Test]
        public void UndoEdit()
        {
            _photo.AdjustBrightness(100);
            _caretaker.SaveSnapshot(_photo, "Step1");
            
            _photo.AdjustBrightness(150);
            _caretaker.SaveSnapshot(_photo, "Step2");
            
            _caretaker.Undo(_photo);
            Assert.That(_photo.Brightness, Is.EqualTo(100));
        }

        [Test]
        public void MultipleEdits_Snapshot()
        {
            _photo.AdjustBrightness(120);
            _photo.AdjustContrast(110);
            _photo.AdjustSaturation(130);
            _photo.Rotate(45);
            
            _caretaker.SaveSnapshot(_photo, "ComplexEdit");
            
            _photo.ResetToDefaults();
            _caretaker.RestoreSnapshot(_photo, "ComplexEdit");
            
            Assert.That(_photo.Brightness, Is.EqualTo(120));
            Assert.That(_photo.Contrast, Is.EqualTo(110));
            Assert.That(_photo.Saturation, Is.EqualTo(130));
            Assert.That(_photo.Rotation, Is.EqualTo(45));
        }

        [Test]
        public void FlipRestore()
        {
            _photo.Flip();
            bool flippedState = _photo.IsFlipped;
            _caretaker.SaveSnapshot(_photo, "Flipped");
            
            _photo.Flip();
            _caretaker.RestoreSnapshot(_photo, "Flipped");
            
            Assert.That(_photo.IsFlipped, Is.EqualTo(flippedState));
        }

        [Test]
        public void GetAvailableSnapshots()
        {
            _caretaker.SaveSnapshot(_photo, "Snap1");
            _caretaker.SaveSnapshot(_photo, "Snap2");
            _caretaker.SaveSnapshot(_photo, "Snap3");
            
            var snapshots = _caretaker.GetAvailableSnapshots();
            Assert.That(snapshots.Count, Is.EqualTo(3));
        }

        [Test]
        public void EditingSequence()
        {
            _photo.AdjustBrightness(100);
            _caretaker.SaveSnapshot(_photo, "Edit1");
            
            _photo.AdjustContrast(120);
            _caretaker.SaveSnapshot(_photo, "Edit2");
            
            _photo.Rotate(90);
            _caretaker.SaveSnapshot(_photo, "Edit3");
            
            // Undo to Edit1
            _caretaker.Undo(_photo);
            _caretaker.Undo(_photo);
            
            Assert.That(_photo.Brightness, Is.EqualTo(100));
            Assert.That(_photo.Contrast, Is.EqualTo(100)); // Back to default
            Assert.That(_photo.Rotation, Is.EqualTo(0));
        }

        [Test]
        public void BoundaryValues()
        {
            _photo.AdjustBrightness(0);
            Assert.That(_photo.Brightness, Is.EqualTo(0));
            
            _photo.AdjustBrightness(300);
            Assert.That(_photo.Brightness, Is.EqualTo(200)); // Clamped
            
            _caretaker.SaveSnapshot(_photo, "Extremes");
            _photo.ResetToDefaults();
            _caretaker.RestoreSnapshot(_photo, "Extremes");
            
            Assert.That(_photo.Brightness, Is.EqualTo(200));
        }

        [Test]
        public void RotationSnapshot()
        {
            _photo.Rotate(45);
            _photo.Rotate(45);
            int rotation = _photo.Rotation;
            
            _caretaker.SaveSnapshot(_photo, "Rotated");
            _photo.Rotate(180);
            _caretaker.RestoreSnapshot(_photo, "Rotated");
            
            Assert.That(_photo.Rotation, Is.EqualTo(rotation));
        }

        [Test]
        public void RedoAfterUndo()
        {
            _photo.AdjustBrightness(120);
            _caretaker.SaveSnapshot(_photo, "Bright");
            
            _photo.AdjustBrightness(80);
            _caretaker.SaveSnapshot(_photo, "Dark");
            
            _caretaker.Undo(_photo);
            Assert.That(_photo.Brightness, Is.EqualTo(120));
            
            _caretaker.Redo(_photo);
            Assert.That(_photo.Brightness, Is.EqualTo(80));
        }
    }
}
