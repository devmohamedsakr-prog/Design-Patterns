using NUnit.Framework;
using RemoteControl.After.Context;

namespace RemoteControl.After.Tests
{
    [TestFixture]
    public class RemoteControlTests
    {
        private RemoteInvoker _remote;
        private Light _light;

        [SetUp]
        public void Setup()
        {
            _remote = new RemoteInvoker();
            _light = new Light();
        }

        [Test] public void RegisterCommand_Succeeds()
        {
            _remote.RegisterCommand("on", new TurnOnCommand(_light));
            Assert.That(_remote.GetRegisteredButtons().Contains("on"), Is.True);
        }

        [Test] public void PressButton_Succeeds()
        {
            _remote.RegisterCommand("on", new TurnOnCommand(_light));
            Assert.That(_remote.PressButton("on"), Is.True);
        }

        [Test] public void TurnOn_Command()
        {
            _remote.RegisterCommand("on", new TurnOnCommand(_light));
            _remote.PressButton("on");
            Assert.That(_light.IsOn, Is.True);
        }

        [Test] public void TurnOff_Command()
        {
            _light.IsOn = true;
            _remote.RegisterCommand("off", new TurnOffCommand(_light));
            _remote.PressButton("off");
            Assert.That(_light.IsOn, Is.False);
        }

        [Test] public void SetBrightness_Command()
        {
            _remote.RegisterCommand("bright", new SetBrightnessCommand(_light, 50));
            _remote.PressButton("bright");
            Assert.That(_light.Brightness, Is.EqualTo(50));
        }

        [Test] public void InvalidBrightness_Fails()
        {
            _remote.RegisterCommand("invalid", new SetBrightnessCommand(_light, 150));
            Assert.That(_remote.PressButton("invalid"), Is.False);
        }

        [Test] public void PressUnregisteredButton_Fails() => Assert.That(_remote.PressButton("invalid"), Is.False);

        [Test] public void MultipleButtons()
        {
            _remote.RegisterCommand("on", new TurnOnCommand(_light));
            _remote.RegisterCommand("off", new TurnOffCommand(_light));
            _remote.RegisterCommand("dim", new SetBrightnessCommand(_light, 25));

            _remote.PressButton("on");
            _remote.PressButton("dim");
            Assert.That(_light.Brightness, Is.EqualTo(25));
        }

        [Test] public void RegisteredButtonsCount()
        {
            _remote.RegisterCommand("1", new TurnOnCommand(_light));
            _remote.RegisterCommand("2", new TurnOffCommand(_light));
            Assert.That(_remote.GetRegisteredButtons().Count, Is.EqualTo(2));
        }
    }
}
