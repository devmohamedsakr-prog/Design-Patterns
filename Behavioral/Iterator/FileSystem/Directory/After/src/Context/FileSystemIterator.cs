using System;
using System.Collections.Generic;

namespace FileSystemIterator.After.Context
{
    public interface IFileSystemIterator
    {
        bool HasNext();
        FileSystemItem Next();
    }

    public class FileSystemItem
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = ""; // "File" or "Directory"
        public long Size { get; set; }
        public List<FileSystemItem> Children { get; set; } = new();

        public FileSystemItem(string name, string type, long size = 0)
        {
            Name = name;
            Type = type;
            Size = size;
        }

        public override string ToString() => $"[{Type}] {Name} ({Size} bytes)";
    }

    public class DepthFirstIterator : IFileSystemIterator
    {
        private Stack<FileSystemItem> _stack = new();

        public DepthFirstIterator(FileSystemItem root)
        {
            _stack.Push(root);
        }

        public bool HasNext() => _stack.Count > 0;

        public FileSystemItem Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more items");
            
            var item = _stack.Pop();
            for (int i = item.Children.Count - 1; i >= 0; i--)
                _stack.Push(item.Children[i]);
            return item;
        }
    }

    public class BreadthFirstIterator : IFileSystemIterator
    {
        private Queue<FileSystemItem> _queue = new();

        public BreadthFirstIterator(FileSystemItem root)
        {
            _queue.Enqueue(root);
        }

        public bool HasNext() => _queue.Count > 0;

        public FileSystemItem Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more items");
            
            var item = _queue.Dequeue();
            foreach (var child in item.Children)
                _queue.Enqueue(child);
            return item;
        }
    }

    public class FileOnlyIterator : IFileSystemIterator
    {
        private Queue<FileSystemItem> _fileQueue = new();

        public FileOnlyIterator(FileSystemItem root)
        {
            CollectFiles(root);
        }

        private void CollectFiles(FileSystemItem item)
        {
            if (item.Type == "File")
                _fileQueue.Enqueue(item);
            else
                foreach (var child in item.Children)
                    CollectFiles(child);
        }

        public bool HasNext() => _fileQueue.Count > 0;

        public FileSystemItem Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more files");
            return _fileQueue.Dequeue();
        }
    }

    public class DirectoryOnlyIterator : IFileSystemIterator
    {
        private Queue<FileSystemItem> _dirQueue = new();

        public DirectoryOnlyIterator(FileSystemItem root)
        {
            CollectDirectories(root);
        }

        private void CollectDirectories(FileSystemItem item)
        {
            if (item.Type == "Directory")
                _dirQueue.Enqueue(item);
            foreach (var child in item.Children)
                CollectDirectories(child);
        }

        public bool HasNext() => _dirQueue.Count > 0;

        public FileSystemItem Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more directories");
            return _dirQueue.Dequeue();
        }
    }
}
