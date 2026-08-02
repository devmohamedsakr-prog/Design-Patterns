using NUnit.Framework;
using FileSystemIterator.After.Context;

namespace FileSystemIterator.After.Tests
{
    [TestFixture]
    public class FileSystemIteratorTests
    {
        private FileSystemItem _root;

        [SetUp]
        public void Setup()
        {
            _root = new FileSystemItem("root", "Directory");
            _root.AddChild(new FileSystemItem("file1.txt", "File", 100));
            _root.AddChild(new FileSystemItem("subdir", "Directory"));
            _root.Children[1].AddChild(new FileSystemItem("file2.txt", "File", 200));
        }

        [Test]
        public void DepthFirst_HasNext() => Assert.That(new DepthFirstIterator(_root).HasNext(), Is.True);

        [Test]
        public void DepthFirst_First()
        {
            var iter = new DepthFirstIterator(_root);
            var item = iter.Next();
            Assert.That(item.Name, Is.EqualTo("root"));
        }

        [Test]
        public void BreadthFirst_Traversal()
        {
            var iter = new BreadthFirstIterator(_root);
            int count = 0;
            while (iter.HasNext())
            {
                iter.Next();
                count++;
            }
            Assert.That(count, Is.GreaterThan(0));
        }

        [Test]
        public void FileOnlyIterator()
        {
            var iter = new FileOnlyIterator(_root);
            int fileCount = 0;
            while (iter.HasNext())
            {
                var item = iter.Next();
                Assert.That(item.Type, Is.EqualTo("File"));
                fileCount++;
            }
            Assert.That(fileCount, Is.GreaterThan(0));
        }

        [Test]
        public void DirectoryOnlyIterator()
        {
            var iter = new DirectoryOnlyIterator(_root);
            int dirCount = 0;
            while (iter.HasNext())
            {
                var item = iter.Next();
                Assert.That(item.Type, Is.EqualTo("Directory"));
                dirCount++;
            }
            Assert.That(dirCount, Is.GreaterThan(0));
        }
    }
}
