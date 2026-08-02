using Xunit;
using Proxy.ImageLoading.Graphics.Component;

namespace Proxy.ImageLoading.Graphics.Tests
{
    public class ImageLoadingProxyTests
    {
        [Fact]
        public void ImageProxy_ShouldNotLoadImmediately()
        {
            var proxy = new ImageProxy("photo.jpg");
            
            Assert.False(proxy.IsLoaded);
        }

        [Fact]
        public void ImageProxy_ShouldLazyLoadOnDisplay()
        {
            var proxy = new ImageProxy("photo.jpg");
            
            proxy.Display();
            
            Assert.True(proxy.IsLoaded);
        }

        [Fact]
        public void ImageProxy_ShouldReturnZeroSizeBeforeLoad()
        {
            var proxy = new ImageProxy("photo.jpg");
            
            Assert.Equal(0, proxy.GetSize());
        }

        [Fact]
        public void ImageProxy_ShouldReturnCorrectSizeAfterLoad()
        {
            var proxy = new ImageProxy("photo.jpg");
            proxy.Display();
            
            Assert.True(proxy.GetSize() > 0);
        }

        [Fact]
        public void RealImage_ShouldLoadWithFilename()
        {
            var image = new HighResolutionImage("test.jpg");
            
            Assert.True(image.GetSize() > 0);
        }

        [Fact]
        public void ImageProxy_ShouldSameInterfaceAsReal()
        {
            IImage proxy = new ImageProxy("photo.jpg");
            IImage real = new HighResolutionImage("photo.jpg");
            
            Assert.NotNull(proxy);
            Assert.NotNull(real);
        }

        [Fact]
        public void ImageProxy_ShouldDeferMemoryAllocation()
        {
            var proxy = new ImageProxy("large.jpg");
            
            // Before display, no memory allocated
            var sizeBefore = proxy.GetSize();
            
            proxy.Display();
            
            // After display, memory allocated
            var sizeAfter = proxy.GetSize();
            
            Assert.Equal(0, sizeBefore);
            Assert.True(sizeAfter > 0);
        }

        [Fact]
        public void MultipleProxies_ShouldNotLoadUntilNeeded()
        {
            var proxy1 = new ImageProxy("photo1.jpg");
            var proxy2 = new ImageProxy("photo2.jpg");
            var proxy3 = new ImageProxy("photo3.jpg");
            
            Assert.False(proxy1.IsLoaded);
            Assert.False(proxy2.IsLoaded);
            Assert.False(proxy3.IsLoaded);
            
            proxy1.Display();
            
            Assert.True(proxy1.IsLoaded);
            Assert.False(proxy2.IsLoaded);
            Assert.False(proxy3.IsLoaded);
        }

        [Fact]
        public void ImageProxy_ShouldOnlyLoadOnce()
        {
            var proxy = new ImageProxy("photo.jpg");
            
            proxy.Display();
            proxy.Display();
            proxy.Display();
            
            Assert.True(proxy.IsLoaded);
        }

        [Fact]
        public void ImageProxy_ShouldHandleMultipleDisplayCalls()
        {
            var proxy = new ImageProxy("photo.jpg");
            
            proxy.Display();
            proxy.Display();
            
            var size = proxy.GetSize();
            Assert.True(size > 0);
        }

        [Fact]
        public void HighResolutionImage_ShouldHaveCorrectSize()
        {
            var image = new HighResolutionImage("test.jpg");
            
            Assert.Equal(5_000_000, image.GetSize());
        }
    }
}
