using NUnit.Framework;
using FileSystemAnalysis.After.Context;

namespace FileSystemAnalysis.After.Tests
{
    [TestFixture]
    public class FileSystemAnalysisTests
    {
        [Test]
        public void SizeCalculator()
        {
            var dir = new Directory { Name = "root" };
            dir.Children.Add(new File { Name = "file1.txt", SizeBytes = 1024 });
            dir.Children.Add(new File { Name = "file2.txt", SizeBytes = 2048 });
            var calc = new SizeCalculator();
            dir.Accept(calc);
            Assert.That(calc.TotalSize, Is.EqualTo(3072));
        }

        [Test]
        public void FileCounter()
        {
            var dir = new Directory { Name = "root" };
            dir.Children.Add(new File { Name = "file1.txt", SizeBytes = 100 });
            dir.Children.Add(new Directory { Name = "subdir" });
            var counter = new FileCounter();
            dir.Accept(counter);
            Assert.That(counter.FileCount, Is.EqualTo(1));
            Assert.That(counter.DirectoryCount, Is.EqualTo(1));
        }

        [Test]
        public void FileTypeCategorizer()
        {
            var dir = new Directory { Name = "root" };
            dir.Children.Add(new File { Name = "file1.txt", SizeBytes = 100 });
            dir.Children.Add(new File { Name = "file2.txt", SizeBytes = 100 });
            dir.Children.Add(new File { Name = "image.jpg", SizeBytes = 100 });
            var cat = new FileTypeCategorizer();
            dir.Accept(cat);
            Assert.That(cat.FileTypeCount[".txt"], Is.EqualTo(2));
            Assert.That(cat.FileTypeCount[".jpg"], Is.EqualTo(1));
        }
    }
}
