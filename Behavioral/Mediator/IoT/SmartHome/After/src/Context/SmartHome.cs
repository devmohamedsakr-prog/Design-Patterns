using System;
using System.Collections.Generic;

namespace SmartHome.After.Context
{
    public interface ISmartHomeMediator
    {
        void RegisterDevice(SmartDevice device);
        void TurnOnDevice(string deviceType);
        void TurnOffDevice(string deviceType);
        void SetTemperature(int temp);
        void ExecuteScene(string sceneName);
    }

    public class SmartHomeHub : ISmartHomeMediator
    {
        private List<SmartDevice> _devices = new();
        private Dictionary<string, Action> _scenes = new();

        public SmartHomeHub()
        {
            SetupScenes();
        }

        public void RegisterDevice(SmartDevice device)
        {
            _devices.Add(device);
            Console.WriteLine($"🔌 {device.Name} ({device.Type}) connected to hub");
        }

        public void TurnOnDevice(string deviceType)
        {
            foreach (var device in _devices)
                if (device.Type == deviceType)
                    device.TurnOn();
        }

        public void TurnOffDevice(string deviceType)
        {
            foreach (var device in _devices)
                if (device.Type == deviceType)
                    device.TurnOff();
        }

        public void SetTemperature(int temp)
        {
            foreach (var device in _devices)
                if (device.Type == "Thermostat")
                    device.SetTemperature(temp);
        }

        public void ExecuteScene(string sceneName)
        {
            if (_scenes.TryGetValue(sceneName, out var scene))
            {
                Console.WriteLine($"🎬 Executing scene: {sceneName}");
                scene.Invoke();
            }
        }

        private void SetupScenes()
        {
            _scenes["Morning"] = () =>
            {
                TurnOnDevice("Light");
                SetTemperature(22);
            };
            _scenes["Night"] = () =>
            {
                TurnOffDevice("Light");
                SetTemperature(18);
            };
        }
    }

    public class SmartDevice
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsOn { get; set; } = false;
        private ISmartHomeMediator _hub;

        public SmartDevice(string name, string type, ISmartHomeMediator hub)
        {
            Name = name;
            Type = type;
            _hub = hub;
            _hub.RegisterDevice(this);
        }

        public void TurnOn()
        {
            IsOn = true;
            Console.WriteLine($"💡 {Name} turned ON");
        }

        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine($"⚫ {Name} turned OFF");
        }

        public void SetTemperature(int temp)
        {
            Console.WriteLine($"🌡️  {Name} set to {temp}°C");
        }
    }
}
