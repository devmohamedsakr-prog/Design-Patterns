using System;
using Bridge.Device.Drivers.Abstraction;

namespace Bridge.Device.Drivers.Implementation
{
    /// <summary>
    /// Implementation interface: Device driver contract.
    /// </summary>
    public interface IDeviceDriver
    {
        DeviceResult Print(string documentName, int copies, string quality);
        DeviceResult Scan(string inputSource, int dpi, string outputFormat);
        DeviceResult Capture(int width, int height, int fps, string format);
        DeviceResult Storage(string operation, string filePath, byte[] data);
    }

    /// <summary>
    /// Implementation: Windows device driver.
    /// </summary>
    public class WindowsDriver : IDeviceDriver
    {
        public DeviceResult Print(string documentName, int copies, string quality)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Printer (Windows)",
                Output = $"Printed {documentName} x{copies} at {quality} quality via GDI+",
                ExecutionTimeMs = 3000
            };
        }

        public DeviceResult Scan(string inputSource, int dpi, string outputFormat)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Scanner (Windows)",
                Output = $"Scanned from {inputSource} at {dpi}DPI as {outputFormat}",
                ExecutionTimeMs = 5000
            };
        }

        public DeviceResult Capture(int width, int height, int fps, string format)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Camera (Windows)",
                Output = $"Captured {width}x{height}@{fps}fps in {format}",
                ExecutionTimeMs = 100
            };
        }

        public DeviceResult Storage(string operation, string filePath, byte[] data)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Storage (Windows NTFS)",
                Output = $"{operation} operation on {filePath}",
                ExecutionTimeMs = 50
            };
        }

        public override string ToString() => "WindowsDriver";
    }

    /// <summary>
    /// Implementation: Linux device driver.
    /// </summary>
    public class LinuxDriver : IDeviceDriver
    {
        public DeviceResult Print(string documentName, int copies, string quality)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Printer (Linux)",
                Output = $"Printed {documentName} x{copies} via CUPS",
                ExecutionTimeMs = 2800
            };
        }

        public DeviceResult Scan(string inputSource, int dpi, string outputFormat)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Scanner (Linux)",
                Output = $"Scanned via SANE: {inputSource} at {dpi}DPI as {outputFormat}",
                ExecutionTimeMs = 4500
            };
        }

        public DeviceResult Capture(int width, int height, int fps, string format)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Camera (Linux)",
                Output = $"Captured via V4L2: {width}x{height}@{fps}fps in {format}",
                ExecutionTimeMs = 120
            };
        }

        public DeviceResult Storage(string operation, string filePath, byte[] data)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Storage (Linux ext4)",
                Output = $"{operation} operation on {filePath}",
                ExecutionTimeMs = 45
            };
        }

        public override string ToString() => "LinuxDriver";
    }

    /// <summary>
    /// Implementation: macOS device driver.
    /// </summary>
    public class MacOSDriver : IDeviceDriver
    {
        public DeviceResult Print(string documentName, int copies, string quality)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Printer (macOS)",
                Output = $"Printed {documentName} x{copies} via macOS printing system",
                ExecutionTimeMs = 3100
            };
        }

        public DeviceResult Scan(string inputSource, int dpi, string outputFormat)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Scanner (macOS)",
                Output = $"Scanned via macOS Image Capture: {inputSource} at {dpi}DPI",
                ExecutionTimeMs = 4800
            };
        }

        public DeviceResult Capture(int width, int height, int fps, string format)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Camera (macOS)",
                Output = $"Captured via AVFoundation: {width}x{height}@{fps}fps",
                ExecutionTimeMs = 110
            };
        }

        public DeviceResult Storage(string operation, string filePath, byte[] data)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Storage (macOS APFS)",
                Output = $"{operation} operation on {filePath}",
                ExecutionTimeMs = 48
            };
        }

        public override string ToString() => "MacOSDriver";
    }

    /// <summary>
    /// Implementation: Android device driver.
    /// </summary>
    public class AndroidDriver : IDeviceDriver
    {
        public DeviceResult Print(string documentName, int copies, string quality)
        {
            return new DeviceResult
            {
                Success = false,
                DeviceType = "Printer (Android)",
                ErrorMessage = "Android: Printing via Print Framework",
                ExecutionTimeMs = 0
            };
        }

        public DeviceResult Scan(string inputSource, int dpi, string outputFormat)
        {
            return new DeviceResult
            {
                Success = false,
                DeviceType = "Scanner (Android)",
                ErrorMessage = "Android: No native scanner support",
                ExecutionTimeMs = 0
            };
        }

        public DeviceResult Capture(int width, int height, int fps, string format)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Camera (Android)",
                Output = $"Captured via Camera2 API: {width}x{height}@{fps}fps in {format}",
                ExecutionTimeMs = 80
            };
        }

        public DeviceResult Storage(string operation, string filePath, byte[] data)
        {
            return new DeviceResult
            {
                Success = true,
                DeviceType = "Storage (Android)",
                Output = $"{operation} operation on {filePath}",
                ExecutionTimeMs = 60
            };
        }

        public override string ToString() => "AndroidDriver";
    }
}
