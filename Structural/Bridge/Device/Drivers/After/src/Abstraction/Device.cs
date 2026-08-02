using System;
using System.Collections.Generic;
using Bridge.Device.Drivers.Implementation;

namespace Bridge.Device.Drivers.Abstraction
{
    /// <summary>
    /// Abstraction: Device operations.
    /// Demonstrates: Bridge pattern for platform-independent device drivers.
    /// </summary>
    public abstract class Device
    {
        protected IDeviceDriver _driver;

        public Device(IDeviceDriver driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        public abstract DeviceResult Operate();

        public void SetDriver(IDeviceDriver driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }
    }

    /// <summary>
    /// Concrete abstraction: Printer device.
    /// </summary>
    public class Printer : Device
    {
        public string DocumentName { get; set; }
        public int Copies { get; set; }
        public string Quality { get; set; } // Draft, Normal, High

        public Printer(IDeviceDriver driver) : base(driver)
        {
            Copies = 1;
            Quality = "Normal";
        }

        public override DeviceResult Operate()
        {
            return _driver.Print(DocumentName, Copies, Quality);
        }

        public override string ToString() =>
            $"Printer(Document={DocumentName}, Copies={Copies}, Quality={Quality})";
    }

    /// <summary>
    /// Concrete abstraction: Scanner device.
    /// </summary>
    public class Scanner : Device
    {
        public string InputSource { get; set; } // Flatbed, ADF
        public int DPI { get; set; }
        public string OutputFormat { get; set; } // PDF, JPEG, TIFF

        public Scanner(IDeviceDriver driver) : base(driver)
        {
            DPI = 300;
            OutputFormat = "PDF";
        }

        public override DeviceResult Operate()
        {
            return _driver.Scan(InputSource, DPI, OutputFormat);
        }

        public override string ToString() =>
            $"Scanner(DPI={DPI}, Format={OutputFormat}, Source={InputSource})";
    }

    /// <summary>
    /// Concrete abstraction: Camera device.
    /// </summary>
    public class Camera : Device
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int FPS { get; set; }
        public string Format { get; set; } // JPEG, RAW, H264

        public Camera(IDeviceDriver driver) : base(driver)
        {
            Width = 1920;
            Height = 1080;
            FPS = 30;
            Format = "JPEG";
        }

        public override DeviceResult Operate()
        {
            return _driver.Capture(Width, Height, FPS, Format);
        }

        public override string ToString() =>
            $"Camera({Width}x{Height}@{FPS}fps, Format={Format})";
    }

    /// <summary>
    /// Concrete abstraction: Storage device.
    /// </summary>
    public class StorageDevice : Device
    {
        public string Operation { get; set; } // Read, Write, Delete, List
        public string FilePath { get; set; }
        public byte[] Data { get; set; }

        public StorageDevice(IDeviceDriver driver) : base(driver)
        {
        }

        public override DeviceResult Operate()
        {
            return _driver.Storage(Operation, FilePath, Data);
        }

        public override string ToString() =>
            $"StorageDevice(Op={Operation}, Path={FilePath})";
    }

    /// <summary>
    /// Device operation result.
    /// </summary>
    public class DeviceResult
    {
        public bool Success { get; set; }
        public string DeviceType { get; set; }
        public string Output { get; set; }
        public long ExecutionTimeMs { get; set; }
        public string ErrorMessage { get; set; }

        public override string ToString() =>
            $"DeviceResult(Success={Success}, Device={DeviceType}, Time={ExecutionTimeMs}ms)";
    }

    /// <summary>
    /// Device manager for multiple devices and drivers.
    /// </summary>
    public class DeviceManager
    {
        private readonly List<Device> _devices = new List<Device>();
        private readonly Dictionary<string, IDeviceDriver> _drivers =
            new Dictionary<string, IDeviceDriver>();

        public void RegisterDriver(string name, IDeviceDriver driver)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Driver name cannot be empty", nameof(name));
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));

            _drivers[name] = driver;
        }

        public void AddDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));
            _devices.Add(device);
        }

        public void ChangeDriver(string driverName)
        {
            if (!_drivers.ContainsKey(driverName))
                throw new KeyNotFoundException($"Driver {driverName} not found");

            var driver = _drivers[driverName];
            foreach (var device in _devices)
            {
                device.SetDriver(driver);
            }
        }

        public List<DeviceResult> OperateAll()
        {
            var results = new List<DeviceResult>();
            foreach (var device in _devices)
            {
                results.Add(device.Operate());
            }
            return results;
        }

        public int DeviceCount => _devices.Count;
        public int DriverCount => _drivers.Count;
    }
}
