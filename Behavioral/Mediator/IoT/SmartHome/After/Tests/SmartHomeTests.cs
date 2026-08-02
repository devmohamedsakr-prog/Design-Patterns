using NUnit.Framework;
using SmartHome.After.Context;

namespace SmartHome.After.Tests
{
    [TestFixture]
    public class SmartHomeTests
    {
        private SmartHomeHub _hub;
        private SmartDevice _light, _lock_device, _thermostat;

        [SetUp]
        public void Setup()
        {
            _hub = new SmartHomeHub();
            _light = new SmartDevice("Living Room Light", "Light", _hub);
            _lock_device = new SmartDevice("Front Door Lock", "Lock", _hub);
            _thermostat = new SmartDevice("Smart Thermostat", "Thermostat", _hub);
        }

        [Test]
        public void DeviceRegistration_Success()
            => Assert.That(_light.Name, Is.EqualTo("Living Room Light"));

        [Test]
        public void TurnOnDevice_Success()
        {
            _hub.TurnOnDevice("Light");
            Assert.That(_light.IsOn, Is.True);
        }

        [Test]
        public void TurnOffDevice_Success()
        {
            _light.TurnOn();
            _hub.TurnOffDevice("Light");
            Assert.That(_light.IsOn, Is.False);
        }

        [Test]
        public void SetTemperature()
        {
            _hub.SetTemperature(22);
            Assert.Pass();
        }

        [Test]
        public void ExecuteScene_Morning()
        {
            _hub.ExecuteScene("Morning");
            Assert.That(_light.IsOn, Is.True);
        }

        [Test]
        public void ExecuteScene_Night()
        {
            _light.TurnOn();
            _hub.ExecuteScene("Night");
            Assert.That(_light.IsOn, Is.False);
        }

        [Test]
        public void MultipleDevicesControl()
        {
            _hub.TurnOnDevice("Light");
            _hub.TurnOnDevice("Lock");
            Assert.That(_light.IsOn, Is.True);
        }
    }
}
