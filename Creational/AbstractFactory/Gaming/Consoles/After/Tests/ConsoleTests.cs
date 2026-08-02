using NUnit.Framework;
using GameConsoleFactory.After.Context;

namespace GameConsoleFactory.After.Tests
{
    [TestFixture]
    public class ConsoleTests
    {
        [Test]
        public void PlayStationFactory_CreateInputHandler()
        {
            var factory = new PlayStationFactory();
            var input = factory.CreateInputHandler();
            Assert.That(input.GetControllerType(), Is.EqualTo("DualSense"));
        }

        [Test]
        public void XboxFactory_CreateGraphicsEngine()
        {
            var factory = new XboxFactory();
            var graphics = factory.CreateGraphicsEngine();
            Assert.That(graphics.GetGraphicsAPI(), Is.EqualTo("DirectX 12"));
        }

        [Test]
        public void NintendoFactory_CreateAudioEngine()
        {
            var factory = new NintendoFactory();
            var audio = factory.CreateAudioEngine();
            Assert.That(audio.GetAudioFormat(), Is.EqualTo("Stereo PCM"));
        }

        [Test]
        public void ProviderReturnsCorrectFactory_PlayStation()
        {
            var factory = ConsoleFactoryProvider.GetFactory("playstation");
            Assert.That(factory, Is.InstanceOf<PlayStationFactory>());
        }

        [Test]
        public void ProviderReturnsCorrectFactory_Xbox()
        {
            var factory = ConsoleFactoryProvider.GetFactory("xbox");
            Assert.That(factory, Is.InstanceOf<XboxFactory>());
        }

        [Test]
        public void AllConsoleComponentsConsistent()
        {
            var factory = new NintendoFactory();
            var input = factory.CreateInputHandler();
            var graphics = factory.CreateGraphicsEngine();
            var audio = factory.CreateAudioEngine();
            
            Assert.IsNotNull(input);
            Assert.IsNotNull(graphics);
            Assert.IsNotNull(audio);
        }

        [Test]
        public void GameEngine_RunsSuccessfully()
        {
            var factory = new XboxFactory();
            var engine = new GameEngine(factory);
            engine.RunGame();
            Assert.Pass();
        }
    }
}
