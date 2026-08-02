using System;
using System.Collections.Generic;
using System.Linq;

namespace Composite.FileSystem.Directory.Component
{
    /// <summary>
    /// Component interface: File system nodes (files and directories).
    /// Demonstrates: Composite pattern for treating files same as directory hierarchies.
    /// </summary>
    public abstract class FileSystemNode
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        protected FileSystemNode(string name)
        {
            Name = name;
            CreatedAt = DateTime.UtcNow;
            ModifiedAt = DateTime.UtcNow;
        }

        public abstract long GetSize();
        public abstract int CountFiles();
        public abstract void Delete();
        public abstract void Display(int indent = 0);
        public abstract List<string> Search(string pattern);
    }

    /// <summary>
    /// Leaf: File node with no children.
    /// </summary>
    public class File : FileSystemNode
    {
        public long Size { get; set; }
        public string Extension { get; set; }
        public string Content { get; set; }

        public File(string name, long size = 1024) : base(name)
        {
            Size = size;
            Extension = System.IO.Path.GetExtension(name);
        }

        public override long GetSize() => Size;

        public override int CountFiles() => 1;

        public override void Delete()
        {
            Console.WriteLine($"Deleted file: {Name}");
        }

        public override void Display(int indent = 0)
        {
            Console.WriteLine($"{new string(' ', indent)}📄 {Name} ({Size} bytes)");
        }

        public override List<string> Search(string pattern)
        {
            if (Name.Contains(pattern))
                return new List<string> { Name };
            return new List<string>();
        }

        public override string ToString() => $"File({Name}, {Size}b)";
    }

    /// <summary>
    /// Composite: Directory node that can contain files and subdirectories.
    /// </summary>
    public class Directory : FileSystemNode
    {
        private readonly List<FileSystemNode> _children = new List<FileSystemNode>();
        private Directory _parent;

        public Directory(string name) : base(name)
        {
        }

        public void Add(FileSystemNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            _children.Add(node);
        }

        public void Remove(FileSystemNode node)
        {
            _children.Remove(node);
        }

        public IReadOnlyList<FileSystemNode> GetChildren() => _children.AsReadOnly();

        public override long GetSize()
        {
            long totalSize = 0;
            foreach (var child in _children)
            {
                totalSize += child.GetSize();
            }
            return totalSize;
        }

        public override int CountFiles()
        {
            int count = 0;
            foreach (var child in _children)
            {
                count += child.CountFiles();
            }
            return count;
        }

        public override void Delete()
        {
            foreach (var child in _children)
            {
                child.Delete();
            }
            Console.WriteLine($"Deleted directory: {Name}");
        }

        public override void Display(int indent = 0)
        {
            Console.WriteLine($"{new string(' ', indent)}📁 {Name}/ ({GetSize()} bytes)");
            foreach (var child in _children)
            {
                child.Display(indent + 2);
            }
        }

        public override List<string> Search(string pattern)
        {
            var results = new List<string>();
            
            if (Name.Contains(pattern))
                results.Add(Name);

            foreach (var child in _children)
            {
                results.AddRange(child.Search(pattern));
            }

            return results;
        }

        public string GetFullPath()
        {
            if (_parent == null)
                return Name;
            return _parent.GetFullPath() + "/" + Name;
        }

        public override string ToString() => $"Directory({Name}, {_children.Count} items)";
    }
}
