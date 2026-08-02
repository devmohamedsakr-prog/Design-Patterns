using System;
using System.Collections.Generic;

namespace PhotoEditor.After.Context
{
    /// <summary>
    /// PhotoState: Memento - snapshot of photo edits
    /// </summary>
    public class PhotoStateMemento
    {
        public string FileName { get; set; } = "";
        public int Brightness { get; set; }
        public int Contrast { get; set; }
        public int Saturation { get; set; }
        public int Rotation { get; set; }
        public bool IsFlipped { get; set; }
        public DateTime EditTime { get; set; }
        public string SnapshotName { get; set; } = "";

        public PhotoStateMemento(string fileName, int brightness, int contrast, int saturation, 
            int rotation, bool isFlipped, string snapshotName)
        {
            FileName = fileName;
            Brightness = brightness;
            Contrast = contrast;
            Saturation = saturation;
            Rotation = rotation;
            IsFlipped = isFlipped;
            SnapshotName = snapshotName;
            EditTime = DateTime.Now;
        }

        public override string ToString() => 
            $"{SnapshotName} - B:{Brightness} C:{Contrast} S:{Saturation} R:{Rotation}° ({EditTime:HH:mm:ss})";
    }

    /// <summary>
    /// Photo: Originator - manages photo state
    /// </summary>
    public class Photo
    {
        public string FileName { get; set; } = "";
        public int Brightness { get; set; } = 100;
        public int Contrast { get; set; } = 100;
        public int Saturation { get; set; } = 100;
        public int Rotation { get; set; } = 0;
        public bool IsFlipped { get; set; } = false;

        public Photo(string fileName)
        {
            FileName = fileName;
        }

        public void AdjustBrightness(int level)
        {
            Brightness = Math.Clamp(level, 0, 200);
            Console.WriteLine($"  ☀️ Brightness: {Brightness}%");
        }

        public void AdjustContrast(int level)
        {
            Contrast = Math.Clamp(level, 0, 200);
            Console.WriteLine($"  🎨 Contrast: {Contrast}%");
        }

        public void AdjustSaturation(int level)
        {
            Saturation = Math.Clamp(level, 0, 200);
            Console.WriteLine($"  🌈 Saturation: {Saturation}%");
        }

        public void Rotate(int degrees)
        {
            Rotation = (Rotation + degrees) % 360;
            Console.WriteLine($"  🔄 Rotation: {Rotation}°");
        }

        public void Flip()
        {
            IsFlipped = !IsFlipped;
            Console.WriteLine($"  ↔️ Flipped: {IsFlipped}");
        }

        public void ResetToDefaults()
        {
            Brightness = Contrast = Saturation = 100;
            Rotation = 0;
            IsFlipped = false;
            Console.WriteLine($"  ⟳ Reset to defaults");
        }

        public PhotoStateMemento SaveSnapshot(string snapshotName)
        {
            var memento = new PhotoStateMemento(FileName, Brightness, Contrast, Saturation, 
                Rotation, IsFlipped, snapshotName);
            Console.WriteLine($"📸 Photo snapshot: {memento}");
            return memento;
        }

        public void RestoreSnapshot(PhotoStateMemento memento)
        {
            Brightness = memento.Brightness;
            Contrast = memento.Contrast;
            Saturation = memento.Saturation;
            Rotation = memento.Rotation;
            IsFlipped = memento.IsFlipped;
            Console.WriteLine($"↶ Restored: {memento}");
        }

        public override string ToString() => 
            $"{FileName} (B:{Brightness}% C:{Contrast}% S:{Saturation}% R:{Rotation}°)";
    }

    /// <summary>
    /// PhotoEditorCaretaker: Manages edit history
    /// </summary>
    public class PhotoEditorCaretaker
    {
        private Dictionary<string, PhotoStateMemento> _snapshots = new();
        private Stack<PhotoStateMemento> _undoStack = new();
        private Stack<PhotoStateMemento> _redoStack = new();

        public void SaveSnapshot(Photo photo, string snapshotName)
        {
            var memento = photo.SaveSnapshot(snapshotName);
            _snapshots[snapshotName] = memento;
            _undoStack.Push(memento);
            _redoStack.Clear();
        }

        public void Undo(Photo photo)
        {
            if (_undoStack.Count < 2) return; // Keep at least one
            var current = _undoStack.Pop();
            _redoStack.Push(current);
            
            if (_undoStack.TryPeek(out var previous))
            {
                photo.RestoreSnapshot(previous);
            }
        }

        public void Redo(Photo photo)
        {
            if (_redoStack.TryPop(out var memento))
            {
                photo.RestoreSnapshot(memento);
                _undoStack.Push(memento);
            }
        }

        public void RestoreSnapshot(Photo photo, string snapshotName)
        {
            if (_snapshots.TryGetValue(snapshotName, out var memento))
            {
                photo.RestoreSnapshot(memento);
            }
        }

        public List<string> GetAvailableSnapshots() => new(_snapshots.Keys);
        public int GetSnapshotCount() => _snapshots.Count;
    }
}
