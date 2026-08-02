using NUnit.Framework;
using System.Threading.Tasks;
using ImageProcessing.After.Abstracts;
using ImageProcessing.After.Creators;

namespace ImageProcessing.After.Tests
{
    [TestFixture]
    public class ImageProcessingTests
    {
        [Test] public async Task ThumbnailProcessor_CreatesThumbnail() => Assert.That((await new ThumbnailCreator().ProcessImageAsync("image.jpg", 200, 200)).ProcessorName, Is.EqualTo("Thumbnail"));

        [Test] public async Task PreviewProcessor_CreatesPreview() => Assert.That((await new PreviewCreator().ProcessImageAsync("image.jpg", 600, 400)).ProcessorName, Is.EqualTo("Preview"));

        [Test] public async Task FullResolutionProcessor_CreatesFullRes() => Assert.That((await new FullResolutionCreator().ProcessImageAsync("image.jpg", 1920, 1080)).ProcessorName, Is.EqualTo("FullResolution"));

        [Test] public async Task ThumbnailFileSize_Smallest() => Assert.That((await new ThumbnailCreator().ProcessImageAsync("img.jpg", 200, 200)).FileSize, Is.EqualTo(15));

        [Test] public async Task PreviewFileSize_Medium() => Assert.That((await new PreviewCreator().ProcessImageAsync("img.jpg", 600, 400)).FileSize, Is.EqualTo(150));

        [Test] public async Task FullResFileSize_Largest() => Assert.That((await new FullResolutionCreator().ProcessImageAsync("img.jpg", 1920, 1080)).FileSize, Is.EqualTo(2500));

        [Test] public async Task ThumbnailPath_HasThumbSuffix() => Assert.That((await new ThumbnailCreator().ProcessImageAsync("image.jpg", 200, 200)).OutputPath, Does.Contain("_thumb_"));

        [Test] public async Task PreviewPath_HasPreviewSuffix() => Assert.That((await new PreviewCreator().ProcessImageAsync("image.jpg", 600, 400)).OutputPath, Does.Contain("_preview_"));

        [Test] public async Task FullResPath_HasFullresSuffix() => Assert.That((await new FullResolutionCreator().ProcessImageAsync("image.jpg", 1920, 1080)).OutputPath, Does.Contain("_fullres_"));

        [Test]
        public async Task AllProcessors_ReturnSuccess()
        {
            var t = await new ThumbnailCreator().ProcessImageAsync("img.jpg", 200, 200);
            var p = await new PreviewCreator().ProcessImageAsync("img.jpg", 600, 400);
            var f = await new FullResolutionCreator().ProcessImageAsync("img.jpg", 1920, 1080);
            Assert.That(t.Success && p.Success && f.Success);
        }

        [Test]
        public async Task InvalidWidth_ShouldFail() => Assert.That((await new ThumbnailCreator().ProcessImageAsync("img.jpg", 0, 200)).Success, Is.False);

        [Test]
        public async Task InvalidHeight_ShouldFail() => Assert.That((await new PreviewCreator().ProcessImageAsync("img.jpg", 600, 0)).Success, Is.False);

        [Test]
        public async Task NullImagePath_ShouldFail() => Assert.That((await new FullResolutionCreator().ProcessImageAsync(null, 1920, 1080)).Success, Is.False);

        [Test]
        public async Task DifferentDimensions_AllSucceed()
        {
            var dims = new[] { (200, 200), (600, 400), (1200, 800), (1920, 1080) };
            foreach (var (w, h) in dims)
            {
                var result = await new ThumbnailCreator().ProcessImageAsync("img.jpg", w, h);
                Assert.That(result.Success);
            }
        }

        [Test]
        public async Task FileSize_Comparison_ThumbSmallest()
        {
            var t = await new ThumbnailCreator().ProcessImageAsync("img.jpg", 200, 200);
            var p = await new PreviewCreator().ProcessImageAsync("img.jpg", 600, 400);
            var f = await new FullResolutionCreator().ProcessImageAsync("img.jpg", 1920, 1080);
            
            Assert.That(t.FileSize, Is.LessThan(p.FileSize));
            Assert.That(p.FileSize, Is.LessThan(f.FileSize));
        }

        [Test]
        public async Task OutputPath_IncludesDimensions()
        {
            var result = await new PreviewCreator().ProcessImageAsync("photo.jpg", 600, 400);
            Assert.That(result.OutputPath, Does.Contain("600x400"));
        }

        [Test]
        public async Task SequentialProcessing_AllSucceed()
        {
            var r1 = await new ThumbnailCreator().ProcessImageAsync("photo1.jpg", 200, 200);
            var r2 = await new PreviewCreator().ProcessImageAsync("photo2.jpg", 600, 400);
            var r3 = await new FullResolutionCreator().ProcessImageAsync("photo3.jpg", 1920, 1080);
            
            Assert.That(r1.Success && r2.Success && r3.Success);
        }

        [Test]
        public async Task ProcessorNames_Correct()
        {
            Assert.That((await new ThumbnailCreator().ProcessImageAsync("img.jpg", 200, 200)).ProcessorName, Is.EqualTo("Thumbnail"));
            Assert.That((await new PreviewCreator().ProcessImageAsync("img.jpg", 600, 400)).ProcessorName, Is.EqualTo("Preview"));
            Assert.That((await new FullResolutionCreator().ProcessImageAsync("img.jpg", 1920, 1080)).ProcessorName, Is.EqualTo("FullResolution"));
        }

        [Test]
        public async Task HighResolution_Images()
        {
            var result = await new FullResolutionCreator().ProcessImageAsync("hires.jpg", 4096, 2160);
            Assert.That(result.Success);
            Assert.That(result.FileSize, Is.GreaterThan(1000));
        }

        [Test]
        public async Task Multiple_SameProcessor_DifferentImages()
        {
            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                var result = await new ThumbnailCreator().ProcessImageAsync($"img{i}.jpg", 200, 200);
                if (result.Success) count++;
            }
            Assert.That(count, Is.EqualTo(10));
        }

        [Test]
        public async Task OutputPath_IncludesImageName()
        {
            var result = await new PreviewCreator().ProcessImageAsync("myimage.jpg", 600, 400);
            Assert.That(result.OutputPath, Does.StartWith("myimage.jpg"));
        }

        [Test]
        public async Task Messages_AreDescriptive()
        {
            var t = await new ThumbnailCreator().ProcessImageAsync("img.jpg", 200, 200);
            var p = await new PreviewCreator().ProcessImageAsync("img.jpg", 600, 400);
            var f = await new FullResolutionCreator().ProcessImageAsync("img.jpg", 1920, 1080);
            
            Assert.That(t.Message, Is.Not.Null);
            Assert.That(p.Message, Is.Not.Null);
            Assert.That(f.Message, Is.Not.Null);
        }

        [Test]
        public async Task NegativeWidth_ShouldFail() => Assert.That((await new ThumbnailCreator().ProcessImageAsync("img.jpg", -200, 200)).Success, Is.False);

        [Test]
        public async Task EmptyImagePath_ShouldFail() => Assert.That((await new PreviewCreator().ProcessImageAsync("", 600, 400)).Success, Is.False);

        [Test]
        public async Task FactoryMethod_CreatorsDifferent()
        {
            var t = await new ThumbnailCreator().ProcessImageAsync("img.jpg", 200, 200);
            var p = await new PreviewCreator().ProcessImageAsync("img.jpg", 200, 200);
            
            Assert.That(t.FileSize, Is.Not.EqualTo(p.FileSize));
            Assert.That(t.ProcessorName, Is.Not.EqualTo(p.ProcessorName));
        }

        [Test]
        public async Task VariousImageTypes()
        {
            var extensions = new[] { "photo.jpg", "image.png", "picture.gif", "artwork.bmp" };
            foreach (var img in extensions)
            {
                var result = await new ThumbnailCreator().ProcessImageAsync(img, 200, 200);
                Assert.That(result.Success);
            }
        }

        [Test]
        public async Task Concurrent_Processing()
        {
            var tasks = new Task[15];
            for (int i = 0; i < 15; i++)
            {
                var creator = i % 3 == 0 ? (ImageCreator)new ThumbnailCreator() 
                            : i % 3 == 1 ? new PreviewCreator() 
                            : new FullResolutionCreator();
                tasks[i] = creator.ProcessImageAsync($"img{i}.jpg", 600, 400);
            }
            await Task.WhenAll(tasks);
            Assert.That(tasks.All(t => t.IsCompleted));
        }

        [Test]
        public async Task LargeScale_Processing()
        {
            int successCount = 0;
            for (int i = 0; i < 20; i++)
            {
                var result = await new PreviewCreator().ProcessImageAsync($"bulk{i}.jpg", 600, 400);
                if (result.Success) successCount++;
            }
            Assert.That(successCount, Is.GreaterThanOrEqualTo(18));
        }
    }
}
