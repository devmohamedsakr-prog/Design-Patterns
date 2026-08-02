using Xunit;
using Bridge.Device.Drivers.Abstraction;
using Bridge.Device.Drivers.Implementation;
using System.Collections.Generic;

namespace Bridge.Device.Drivers.Tests
{
    public class DeviceDriverTests
    {
        [Fact]
        public void Printer_PrintWithWindowsDriver_Success()
        {
            var driver = new WindowsDriver();
            var printer = new Printer(driver)
            {
                DocumentName = "Report.pdf",
                Copies = 2,
                Quality = "High"
            };

            var result = printer.Operate();

            Assert.True(result.Success);
            Assert.Contains("GDI+", result.Output);
        }

        [Fact]
        public void Scanner_ScanWithLinuxDriver_Success()
        {
            var driver = new LinuxDriver();
            var scanner = new Scanner(driver)
            {
                InputSource = "Flatbed",
                DPI = 600,
                OutputFormat = "PDF"
            };

            var result = scanner.Operate();

            Assert.True(result.Success);
            Assert.Contains("SANE", result.Output);
        }

        [Fact]
        public void Camera_CaptureWithMacOSDriver_Success()
        {
            var driver = new MacOSDriver();
            var camera = new Camera(driver)
            {
                Width = 1920,
                Height = 1080,
                FPS = 60,
                Format = "H264"
            };

            var result = camera.Operate();

            Assert.True(result.Success);
            Assert.Contains("AVFoundation", result.Output);
        }

        [Fact]
        public void Storage_WriteWithAndroidDriver_Success()
        {
            var driver = new AndroidDriver();
            var storage = new StorageDevice(driver)
            {
                Operation = "Write",
                FilePath = "/sdcard/file.txt",
                Data = new byte[] { 1, 2, 3 }
            };

            var result = storage.Operate();

            Assert.True(result.Success);
        }

        [Fact]
        public void Device_SwitchDriver_Success()
        {
            var windowsDriver = new WindowsDriver();
            var printer = new Printer(windowsDriver)
            {
                DocumentName = "Test.pdf",
                Copies = 1,
                Quality = "Normal"
            };

            var result1 = printer.Operate();
            Assert.True(result1.Success);

            var linuxDriver = new LinuxDriver();
            printer.SetDriver(linuxDriver);

            var result2 = printer.Operate();
            Assert.True(result2.Success);
        }

        [Fact]
        public void DeviceManager_OperateMultipleDevices_Success()
        {
            var windowsDriver = new WindowsDriver();
            var manager = new DeviceManager();

            manager.AddDevice(new Printer(windowsDriver) { DocumentName = "doc1.pdf" });
            manager.AddDevice(new Scanner(windowsDriver) { DPI = 300 });
            manager.AddDevice(new Camera(windowsDriver) { Width = 1920, Height = 1080 });

            var results = manager.OperateAll();

            Assert.Equal(3, results.Count);
            Assert.All(results, r => Assert.True(r.Success));
        }

        [Fact]
        public void DeviceManager_ChangeDriver_UpdatesAllDevices()
        {
            var windowsDriver = new WindowsDriver();
            var manager = new DeviceManager();
            manager.RegisterDriver("windows", windowsDriver);

            manager.AddDevice(new Printer(windowsDriver));
            manager.AddDevice(new Scanner(windowsDriver));

            var linuxDriver = new LinuxDriver();
            manager.RegisterDriver("linux", linuxDriver);
            manager.ChangeDriver("linux");

            var results = manager.OperateAll();
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void AllPlatformDrivers_PrintSupported()
        {
            var drivers = new IDeviceDriver[]
            {
                new WindowsDriver(),
                new LinuxDriver(),
                new MacOSDriver()
            };

            foreach (var driver in drivers)
            {
                var result = driver.Print("test.pdf", 1, "Normal");
                Assert.True(result.Success);
            }
        }

        [Fact]
        public void AndroidDriver_PrintNotSupported()
        {
            var driver = new AndroidDriver();
            var result = driver.Print("test.pdf", 1, "Normal");

            Assert.False(result.Success);
        }

        [Fact]
        public void AllPlatformDrivers_ScanSupported()
        {
            var drivers = new IDeviceDriver[]
            {
                new WindowsDriver(),
                new LinuxDriver(),
                new MacOSDriver()
            };

            foreach (var driver in drivers)
            {
                var result = driver.Scan("Flatbed", 300, "PDF");
                Assert.True(result.Success);
            }
        }

        [Fact]
        public void AndroidDriver_ScanNotSupported()
        {
            var driver = new AndroidDriver();
            var result = driver.Scan("Flatbed", 300, "PDF");

            Assert.False(result.Success);
        }

        [Fact]
        public void AllPlatformDrivers_CaptureSupported()
        {
            var drivers = new IDeviceDriver[]
            {
                new WindowsDriver(),
                new LinuxDriver(),
                new MacOSDriver(),
                new AndroidDriver()
            };

            foreach (var driver in drivers)
            {
                var result = driver.Capture(1920, 1080, 30, "JPEG");
                Assert.True(result.Success);
            }
        }

        [Fact]
        public void DeviceResult_ToString_ContainsInfo()
        {
            var result = new DeviceResult
            {
                Success = true,
                DeviceType = "Printer",
                ExecutionTimeMs = 3000
            };

            var str = result.ToString();
            Assert.Contains("True", str);
            Assert.Contains("Printer", str);
            Assert.Contains("3000", str);
        }

        [Fact]
        public void Printer_ToString_ContainsInfo()
        {
            var driver = new WindowsDriver();
            var printer = new Printer(driver)
            {
                DocumentName = "Report.pdf",
                Copies = 2,
                Quality = "High"
            };

            var str = printer.ToString();
            Assert.Contains("Report.pdf", str);
            Assert.Contains("2", str);
            Assert.Contains("High", str);
        }

        [Fact]
        public void Scanner_ToString_ContainsInfo()
        {
            var driver = new WindowsDriver();
            var scanner = new Scanner(driver) { DPI = 600, OutputFormat = "PDF" };

            var str = scanner.ToString();
            Assert.Contains("600", str);
            Assert.Contains("PDF", str);
        }

        [Fact]
        public void Camera_ToString_ContainsInfo()
        {
            var driver = new WindowsDriver();
            var camera = new Camera(driver) { Width = 1280, Height = 720, FPS = 24 };

            var str = camera.ToString();
            Assert.Contains("1280", str);
            Assert.Contains("720", str);
            Assert.Contains("24", str);
        }

        [Fact]
        public void Device_WithNullDriver_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Printer(null)
            );

            Assert.Contains("driver", exception.Message);
        }

        [Fact]
        public void SetDriver_WithNullDriver_ThrowsException()
        {
            var driver = new WindowsDriver();
            var printer = new Printer(driver);

            var exception = Assert.Throws<ArgumentNullException>(() =>
                printer.SetDriver(null)
            );

            Assert.Contains("driver", exception.Message);
        }

        [Fact]
        public void DeviceManager_AddNullDevice_ThrowsException()
        {
            var manager = new DeviceManager();

            var exception = Assert.Throws<ArgumentNullException>(() =>
                manager.AddDevice(null)
            );

            Assert.Contains("device", exception.Message);
        }

        [Fact]
        public void DeviceManager_ChangeToUnregisteredDriver_ThrowsException()
        {
            var manager = new DeviceManager();

            var exception = Assert.Throws<KeyNotFoundException>(() =>
                manager.ChangeDriver("nonexistent")
            );

            Assert.Contains("nonexistent", exception.Message);
        }

        [Fact]
        public void WindowsDriver_ToString()
        {
            var driver = new WindowsDriver();
            Assert.Contains("Windows", driver.ToString());
        }

        [Fact]
        public void Printer_DefaultValues()
        {
            var driver = new WindowsDriver();
            var printer = new Printer(driver) { DocumentName = "test.pdf" };

            Assert.Equal(1, printer.Copies);
            Assert.Equal("Normal", printer.Quality);
        }

        [Fact]
        public void Scanner_DefaultValues()
        {
            var driver = new WindowsDriver();
            var scanner = new Scanner(driver);

            Assert.Equal(300, scanner.DPI);
            Assert.Equal("PDF", scanner.OutputFormat);
        }

        [Fact]
        public void Camera_DefaultValues()
        {
            var driver = new WindowsDriver();
            var camera = new Camera(driver);

            Assert.Equal(1920, camera.Width);
            Assert.Equal(1080, camera.Height);
            Assert.Equal(30, camera.FPS);
            Assert.Equal("JPEG", camera.Format);
        }
    }
}
