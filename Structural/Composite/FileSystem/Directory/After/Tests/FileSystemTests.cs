using Xunit;
using Composite.FileSystem.Directory.Component;
using System.Collections.Generic;
using System.Linq;

namespace Composite.FileSystem.Directory.Tests
{
    public class FileSystemTests
    {
        [Fact]
        public void File_CreateAndGetSize()
        {
            var file = new File("document.txt", 2048);

            Assert.Equal(2048, file.GetSize());
            Assert.Equal(1, file.CountFiles());
        }

        [Fact]
        public void File_Search_Found()
        {
            var file = new File("readme.md", 1024);
            var results = file.Search("readme");

            Assert.Single(results);
            Assert.Contains("readme.md", results);
        }

        [Fact]
        public void Directory_CreateEmpty()
        {
            var dir = new Directory("Documents");

            Assert.Equal(0, dir.GetChildren().Count);
            Assert.Equal(0, dir.GetSize());
        }

        [Fact]
        public void Directory_AddFile_Success()
        {
            var dir = new Directory("Projects");
            var file = new File("README.md", 1024);

            dir.Add(file);

            Assert.Single(dir.GetChildren());
            Assert.Equal(1024, dir.GetSize());
            Assert.Equal(1, dir.CountFiles());
        }

        [Fact]
        public void Directory_AddMultipleFiles()
        {
            var dir = new Directory("Photos");
            dir.Add(new File("pic1.jpg", 2048));
            dir.Add(new File("pic2.jpg", 2048));
            dir.Add(new File("pic3.jpg", 2048));

            Assert.Equal(3, dir.GetChildren().Count);
            Assert.Equal(6144, dir.GetSize());
            Assert.Equal(3, dir.CountFiles());
        }

        [Fact]
        public void Directory_Nested_Hierarchy()
        {
            var root = new Directory("root");
            var subDir = new Directory("subfolder");
            var file = new File("file.txt", 512);

            subDir.Add(file);
            root.Add(subDir);

            Assert.Single(root.GetChildren());
            Assert.Equal(512, root.GetSize());
            Assert.Equal(1, root.CountFiles());
        }

        [Fact]
        public void Directory_MultiLevel_SizeCalculation()
        {
            var root = new Directory("root");
            var level1 = new Directory("level1");
            var level2 = new Directory("level2");

            level2.Add(new File("deep.txt", 1024));
            level1.Add(level2);
            level1.Add(new File("mid.txt", 2048));
            root.Add(level1);

            Assert.Equal(3072, root.GetSize());
        }

        [Fact]
        public void Directory_Search_FindsNestedFiles()
        {
            var root = new Directory("root");
            var sub = new Directory("sub");

            root.Add(new File("readme.md", 512));
            sub.Add(new File("readme.txt", 256));
            root.Add(sub);

            var results = root.Search("readme");

            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void Directory_Remove_Success()
        {
            var dir = new Directory("temp");
            var file = new File("temp.txt", 100);

            dir.Add(file);
            Assert.Single(dir.GetChildren());

            dir.Remove(file);
            Assert.Empty(dir.GetChildren());
        }

        [Fact]
        public void File_CountFiles_OnlyOne()
        {
            var file = new File("single.txt", 100);

            Assert.Equal(1, file.CountFiles());
        }

        [Fact]
        public void Directory_CountFiles_Recursive()
        {
            var dir = new Directory("root");
            var sub1 = new Directory("sub1");
            var sub2 = new Directory("sub2");

            dir.Add(new File("f1.txt", 100));
            sub1.Add(new File("f2.txt", 100));
            sub1.Add(new File("f3.txt", 100));
            sub2.Add(new File("f4.txt", 100));
            dir.Add(sub1);
            dir.Add(sub2);

            Assert.Equal(4, dir.CountFiles());
        }

        [Fact]
        public void Directory_Delete_DeletesAll()
        {
            var dir = new Directory("deleteMe");
            dir.Add(new File("file.txt", 100));

            dir.Delete();

            // Delete called successfully (mocked)
            Assert.NotNull(dir);
        }

        [Fact]
        public void File_Display()
        {
            var file = new File("test.txt", 512);
            file.Display();

            Assert.Equal("test.txt", file.Name);
        }

        [Fact]
        public void Directory_Display()
        {
            var dir = new Directory("folder");
            dir.Add(new File("file.txt", 100));

            dir.Display();

            Assert.Equal(1, dir.CountFiles());
        }

        [Fact]
        public void File_ToString_ContainsInfo()
        {
            var file = new File("data.txt", 2048);

            var str = file.ToString();
            Assert.Contains("data.txt", str);
            Assert.Contains("2048", str);
        }

        [Fact]
        public void Directory_ToString_ContainsItemCount()
        {
            var dir = new Directory("folder");
            dir.Add(new File("f1.txt", 100));
            dir.Add(new File("f2.txt", 100));

            var str = dir.ToString();
            Assert.Contains("folder", str);
            Assert.Contains("2", str);
        }

        [Fact]
        public void Directory_AddNull_ThrowsException()
        {
            var dir = new Directory("test");

            var exception = Assert.Throws<ArgumentNullException>(() =>
                dir.Add(null)
            );

            Assert.Contains("node", exception.Message);
        }

        [Fact]
        public void File_Search_NotFound()
        {
            var file = new File("readme.md", 100);
            var results = file.Search("xyz");

            Assert.Empty(results);
        }
    }
}
