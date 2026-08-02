using NUnit.Framework;
using MobileApiFactory.After.Context;

namespace MobileApiFactory.After.Tests
{
    [TestFixture]
    public class MobileApiTests
    {
        [Test]
        public void IosFactory_CreateCamera()
        {
            var factory = new IosFactory();
            var camera = factory.CreateCamera();
            Assert.IsNotNull(camera);
        }

        [Test]
        public void AndroidFactory_CreateStorage()
        {
            var factory = new AndroidFactory();
            var storage = factory.CreateStorage();
            Assert.IsNotNull(storage);
        }

        [Test]
        public void WindowsPhoneFactory_CreateNotification()
        {
            var factory = new WindowsPhoneFactory();
            var notification = factory.CreateNotification();
            Assert.IsNotNull(notification);
        }

        [Test]
        public void ProviderReturnsCorrectFactory_Ios()
        {
            var factory = MobileFactoryProvider.GetFactory("ios");
            Assert.That(factory, Is.InstanceOf<IosFactory>());
        }

        [Test]
        public void ProviderReturnsCorrectFactory_Android()
        {
            var factory = MobileFactoryProvider.GetFactory("android");
            Assert.That(factory, Is.InstanceOf<AndroidFactory>());
        }

        [Test]
        public void Storage_SaveAndRead()
        {
            var factory = new AndroidFactory();
            var storage = factory.CreateStorage();
            storage.SaveFile("test.txt", "test content");
            var content = storage.ReadFile("test.txt");
            Assert.That(content, Is.EqualTo("test content"));
        }

        [Test]
        public void MobileApplication_RunsSuccessfully()
        {
            var factory = new IosFactory();
            var app = new MobileApplication(factory);
            app.RunApp();
            Assert.Pass();
        }
    }
}
