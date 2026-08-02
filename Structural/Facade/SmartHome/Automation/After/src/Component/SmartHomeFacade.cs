using System;
using System.Collections.Generic;

namespace Facade.SmartHome.Automation.Component
{
    // Subsystem 1: Lighting System
    public class LightingSystem
    {
        public bool IsEnabled { get; set; }
        public int Brightness { get; set; }
        public string Color { get; set; }

        public void TurnOn()
        {
            IsEnabled = true;
            Brightness = 100;
        }

        public void TurnOff()
        {
            IsEnabled = false;
            Brightness = 0;
        }

        public void SetBrightness(int level)
        {
            Brightness = Math.Max(0, Math.Min(100, level));
        }

        public void SetColor(string color)
        {
            Color = color;
        }
    }

    // Subsystem 2: HVAC Climate Control
    public class ClimateControl
    {
        public decimal TargetTemperature { get; set; }
        public string Mode { get; set; }

        public void SetTemperature(decimal temp)
        {
            TargetTemperature = Math.Max(16, Math.Min(30, temp));
        }

        public void SetMode(string mode)
        {
            Mode = mode; // Heat, Cool, Auto
        }

        public decimal GetCurrentTemperature() => TargetTemperature;
    }

    // Subsystem 3: Audio/Entertainment
    public class AudioSystem
    {
        public bool IsPlaying { get; set; }
        public int Volume { get; set; }
        public string CurrentTrack { get; set; }

        public void Play(string track)
        {
            CurrentTrack = track;
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
        }

        public void SetVolume(int level)
        {
            Volume = Math.Max(0, Math.Min(100, level));
        }

        public void Mute()
        {
            Volume = 0;
        }
    }

    // Subsystem 4: Security System
    public class SecuritySystem
    {
        public bool IsArmed { get; set; }
        public List<string> Alerts { get; set; } = new();

        public void ArmSystem()
        {
            IsArmed = true;
        }

        public void DisarmSystem()
        {
            IsArmed = false;
        }

        public void LockDoors()
        {
            Alerts.Add($"[{DateTime.UtcNow:O}] Doors locked");
        }

        public void UnlockDoors()
        {
            Alerts.Add($"[{DateTime.UtcNow:O}] Doors unlocked");
        }

        public IReadOnlyList<string> GetAlerts() => Alerts.AsReadOnly();
    }

    // Subsystem 5: Window Management
    public class WindowManagement
    {
        public bool BlindsClosed { get; set; }

        public void CloseBlinds()
        {
            BlindsClosed = true;
        }

        public void OpenBlinds()
        {
            BlindsClosed = false;
        }
    }

    // Subsystem 6: Energy Management
    public class EnergyManager
    {
        private List<string> _energyLogs = new();

        public void LogEnergyUsage(string device, decimal kwh)
        {
            _energyLogs.Add($"[{DateTime.UtcNow:O}] {device}: {kwh} kWh");
        }

        public decimal GetTotalConsumption() => _energyLogs.Count * 0.5m;

        public IReadOnlyList<string> GetEnergyLogs() => _energyLogs.AsReadOnly();
    }

    // FACADE: Simplifies home automation
    public class SmartHomeFacade
    {
        private LightingSystem _lighting = new();
        private ClimateControl _climate = new();
        private AudioSystem _audio = new();
        private SecuritySystem _security = new();
        private WindowManagement _windows = new();
        private EnergyManager _energy = new();

        public void ActivateMovieMode()
        {
            _lighting.SetBrightness(10);
            _lighting.SetColor("Warm");
            _windows.CloseBlinds();
            _climate.SetTemperature(21);
            _audio.SetVolume(40);
            _energy.LogEnergyUsage("MovieMode", 0.5m);
        }

        public void ActivateLeaveMode()
        {
            _lighting.TurnOff();
            _climate.SetTemperature(18);
            _security.LockDoors();
            _security.ArmSystem();
            _audio.Stop();
            _energy.LogEnergyUsage("LeaveMode", 0.1m);
        }

        public void ActivateGoodMorningMode()
        {
            _windows.OpenBlinds();
            _climate.SetTemperature(22);
            _climate.SetMode("Heat");
            _lighting.TurnOn();
            _lighting.SetBrightness(80);
            _audio.Play("Morning_News");
            _audio.SetVolume(30);
            _energy.LogEnergyUsage("GoodMorningMode", 0.3m);
        }

        public void ActivateBedtimeMode()
        {
            _security.LockDoors();
            _security.ArmSystem();
            _lighting.TurnOff();
            _climate.SetTemperature(18);
            _climate.SetMode("Cool");
            _audio.Play("Sleep_Sounds");
            _audio.SetVolume(10);
            _energy.LogEnergyUsage("BedtimeMode", 0.2m);
        }

        public void OptimizeEnergy()
        {
            _lighting.SetBrightness(50);
            _climate.SetTemperature(20);
            _audio.SetVolume(20);
            _energy.LogEnergyUsage("Optimization", 0.05m);
        }

        public Dictionary<string, object> GetHomeStatus()
        {
            return new Dictionary<string, object>
            {
                { "Lights", _lighting.IsEnabled },
                { "Temperature", _climate.GetCurrentTemperature() },
                { "Audio", _audio.IsPlaying ? _audio.CurrentTrack : "Off" },
                { "Security", _security.IsArmed },
                { "Blinds", _windows.BlindsClosed },
                { "EnergyUsage", _energy.GetTotalConsumption() }
            };
        }
    }
}
