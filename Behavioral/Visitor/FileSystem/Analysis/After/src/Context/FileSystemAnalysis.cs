using System;
using System.Collections.Generic;

namespace FileSystemAnalysis.After.Context
{
    public interface IFileSystemNode
    {
        void Accept(IFileSystemVisitor visitor);
    }

    public interface IFileSystemVisitor
    {
        void Visit(File file);
        void Visit(Directory directory);
    }

    public class File : IFileSystemNode
    {
        public string Name { get; set; } = "";
        public long SizeBytes { get; set; }
        public void Accept(IFileSystemVisitor visitor) => visitor.Visit(this);
    }

    public class Directory : IFileSystemNode
    {
        public string Name { get; set; } = "";
        public List<IFileSystemNode> Children { get; set; } = new();
        public void Accept(IFileSystemVisitor visitor)
        {
            visitor.Visit(this);
            foreach (var child in Children)
                child.Accept(visitor);
        }
    }

    public class SizeCalculator : IFileSystemVisitor
    {
        public long TotalSize { get; set; } = 0;

        public void Visit(File file) => TotalSize += file.SizeBytes;
        public void Visit(Directory directory) => Console.WriteLine($"📁 {directory.Name}");
    }

    public class FileCounter : IFileSystemVisitor
    {
        public int FileCount { get; set; } = 0;
        public int DirectoryCount { get; set; } = 0;

        public void Visit(File file) => FileCount++;
        public void Visit(Directory directory) => DirectoryCount++;
    }

    public class FileTypeCategorizer : IFileSystemVisitor
    {
        public Dictionary<string, int> FileTypeCount { get; set; } = new();

        public void Visit(File file)
        {
            var ext = file.Name.Contains(".") ? file.Name.Substring(file.Name.LastIndexOf(".")) : "unknown";
            if (!FileTypeCount.ContainsKey(ext))
                FileTypeCount[ext] = 0;
            FileTypeCount[ext]++;
        }

        public void Visit(Directory directory) { }
    }
}
