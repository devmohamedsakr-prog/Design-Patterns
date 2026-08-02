using Xunit;
using Proxy.ResourceAccess.Enterprise.Component;
using System;

namespace Proxy.ResourceAccess.Enterprise.Tests
{
    public class ResourceAccessProxyTests
    {
        [Fact]
        public void ResourceProxy_ShouldNotLoadImmediately()
        {
            var proxy = new ResourceAccessProxy("dataset.dat", "Admin");
            
            Assert.False(proxy.IsLoaded);
        }

        [Fact]
        public void ResourceProxy_ShouldLazyLoadOnDataAccess()
        {
            var proxy = new ResourceAccessProxy("dataset.dat", "Admin");
            
            var data = proxy.GetData();
            
            Assert.True(proxy.IsLoaded);
        }

        [Fact]
        public void ResourceProxy_ShouldAllowAdminAccess()
        {
            var proxy = new ResourceAccessProxy("dataset.dat", "Admin");
            
            var data = proxy.GetData();
            
            Assert.NotNull(data);
        }

        [Fact]
        public void ResourceProxy_ShouldDenyGuestAccess()
        {
            var proxy = new ResourceAccessProxy("private_dataset.dat", "Guest");
            
            var ex = Assert.Throws<UnauthorizedAccessException>(() => proxy.GetData());
            Assert.NotNull(ex);
        }

        [Fact]
        public void ResourceProxy_ShouldAllowUserAccessToPublic()
        {
            var proxy = new ResourceAccessProxy("public_data.dat", "User");
            
            var data = proxy.GetData();
            
            Assert.NotNull(data);
        }

        [Fact]
        public void ResourceProxy_ShouldDenyUserAccessToPrivate()
        {
            var proxy = new ResourceAccessProxy("private_data.dat", "User");
            
            var ex = Assert.Throws<UnauthorizedAccessException>(() => proxy.GetData());
            Assert.NotNull(ex);
        }

        [Fact]
        public void ResourceProxy_ShouldLogAccessEvents()
        {
            var proxy = new ResourceAccessProxy("dataset.dat", "Admin");
            
            proxy.GetData();
            
            var logs = proxy.GetAccessLog();
            Assert.NotEmpty(logs);
        }

        [Fact]
        public void ResourceProxy_ShouldUnloadResource()
        {
            var proxy = new ResourceAccessProxy("dataset.dat", "Admin");
            
            proxy.GetData();
            Assert.True(proxy.IsLoaded);
            
            proxy.Unload();
            Assert.False(proxy.IsLoaded);
        }

        [Fact]
        public void ResourceProxy_ShouldReturnZeroSizeBeforeLoad()
        {
            var proxy = new ResourceAccessProxy("dataset.dat", "Admin");
            
            Assert.Equal(0, proxy.GetSize());
        }

        [Fact]
        public void ResourceProxy_ShouldReturnCorrectSizeAfterLoad()
        {
            var proxy = new ResourceAccessProxy("dataset.dat", "Admin");
            proxy.GetData();
            
            Assert.True(proxy.GetSize() > 0);
        }

        [Fact]
        public void ResourceProxy_ShouldLogAccessTrail()
        {
            var proxy = new ResourceAccessProxy("dataset.dat", "Admin");
            
            proxy.GetData();
            var size = proxy.GetSize();
            
            var logs = proxy.GetAccessLog();
            Assert.True(logs.Count >= 2);
        }

        [Fact]
        public void HeavyDataset_ShouldHaveCorrectSize()
        {
            var dataset = new HeavyDataset("test.dat");
            
            Assert.Equal(10_000_000, dataset.GetSize());
        }

        [Fact]
        public void ResourceProxy_ShouldHandleMultipleAccessViolations()
        {
            var proxy = new ResourceAccessProxy("private.dat", "Guest");
            
            for (int i = 0; i < 3; i++)
            {
                Assert.Throws<UnauthorizedAccessException>(() => proxy.GetData());
            }
            
            var logs = proxy.GetAccessLog();
            Assert.NotEmpty(logs);
        }

        [Fact]
        public void ResourceProxy_ShouldDeferMemoryUntilAccess()
        {
            var proxy = new ResourceAccessProxy("dataset.dat", "Admin");
            var sizeBefore = proxy.GetSize();
            
            proxy.GetData();
            var sizeAfter = proxy.GetSize();
            
            Assert.Equal(0, sizeBefore);
            Assert.True(sizeAfter > 0);
        }
    }
}
