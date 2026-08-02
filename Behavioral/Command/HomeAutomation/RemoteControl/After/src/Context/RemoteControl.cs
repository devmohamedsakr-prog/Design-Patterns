using System;
using System.Collections.Generic;

namespace RemoteControl.After.Context
{
    public interface IDeviceCommand
    {
        bool Execute();
        string GetDescription();
    }

    public class RemoteInvoker
    {
        private Dictionary<string, IDeviceCommand> _commands = new();

        public void RegisterCommand(string button, IDeviceCommand command)
        {
            _commands[button] = command;
        }

        public bool PressButton(string button)
        {
            if (!_commands.ContainsKey(button)) return false;
            return _commands[button].Execute();
        }

        public List<string> GetRegisteredButtons() => new(_commands.Keys);
    }

    public interface IDevice
    {
        bool TurnOn();
        bool TurnOff();
        bool SetBrightness(int level);
    }

    public class Light : IDevice
    {
        public bool IsOn { get; set; }
        public int Brightness { get; set; } = 100;

        public bool TurnOn()
        {
            IsOn = true;
            Console.WriteLine("💡 Light turned ON");
            return true;
        }

        public bool TurnOff()
        {
            IsOn = false;
            Console.WriteLine("💡 Light turned OFF");
            return true;
        }

        public bool SetBrightness(int level)
        {
            if (level < 0 || level > 100) return false;
            Brightness = level;
            Console.WriteLine($"💡 Light brightness: {level}%");
            return true;
        }
    }

    public class TurnOnCommand : IDeviceCommand
    {
        private IDevice _device;

        public TurnOnCommand(IDevice device) => _device = device;

        public bool Execute() => _device.TurnOn();
        public string GetDescription() => "Turn On";
    }

    public class TurnOffCommand : IDeviceCommand
    {
        private IDevice _device;

        public TurnOffCommand(IDevice device) => _device = device;

        public bool Execute() => _device.TurnOff();
        public string GetDescription() => "Turn Off";
    }

    public class SetBrightnessCommand : IDeviceCommand
    {
        private IDevice _device;
        private int _level;

        public SetBrightnessCommand(IDevice device, int level)
        {
            _device = device;
            _level = level;
        }

        public bool Execute() => _device.SetBrightness(_level);
        public string GetDescription() => $"Set Brightness {_level}%";
    }
}
