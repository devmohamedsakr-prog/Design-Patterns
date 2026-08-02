using System;
using System.Collections.Generic;

namespace MobileApiFactory.After.Context
{
    // Abstract product families
    public interface ICamera
    {
        void TakePhoto();
        void RecordVideo();
    }

    public interface IStorage
    {
        void SaveFile(string filename, string content);
        string ReadFile(string filename);
    }

    public interface INotification
    {
        void SendPushNotification(string title, string message);
    }

    // Abstract factory
    public interface IMobileFactory
    {
        ICamera CreateCamera();
        IStorage CreateStorage();
        INotification CreateNotification();
    }

    // iOS implementations
    public class IosCamera : ICamera
    {
        public void TakePhoto() => Console.WriteLine("📷 iOS: Capturing photo with AVFoundation");
        public void RecordVideo() => Console.WriteLine("🎥 iOS: Recording video with AVFoundation");
    }

    public class IosStorage : IStorage
    {
        private Dictionary<string, string> _files = new();
        public void SaveFile(string filename, string content)
        {
            _files[filename] = content;
            Console.WriteLine($"💾 iOS: Saved to Documents directory: {filename}");
        }
        public string ReadFile(string filename) => _files.ContainsKey(filename) ? _files[filename] : "";
    }

    public class IosNotification : INotification
    {
        public void SendPushNotification(string title, string message)
            => Console.WriteLine($"🔔 iOS: Push notification via APNs - {title}: {message}");
    }

    // Android implementations
    public class AndroidCamera : ICamera
    {
        public void TakePhoto() => Console.WriteLine("📷 Android: Capturing photo with Camera API");
        public void RecordVideo() => Console.WriteLine("🎥 Android: Recording video with Camera API");
    }

    public class AndroidStorage : IStorage
    {
        private Dictionary<string, string> _files = new();
        public void SaveFile(string filename, string content)
        {
            _files[filename] = content;
            Console.WriteLine($"💾 Android: Saved to internal storage: {filename}");
        }
        public string ReadFile(string filename) => _files.ContainsKey(filename) ? _files[filename] : "";
    }

    public class AndroidNotification : INotification
    {
        public void SendPushNotification(string title, string message)
            => Console.WriteLine($"🔔 Android: Push notification via Firebase - {title}: {message}");
    }

    // Windows Phone implementations
    public class WindowsPhoneCamera : ICamera
    {
        public void TakePhoto() => Console.WriteLine("📷 Windows: Capturing photo with MediaCapture");
        public void RecordVideo() => Console.WriteLine("🎥 Windows: Recording video with MediaCapture");
    }

    public class WindowsPhoneStorage : IStorage
    {
        private Dictionary<string, string> _files = new();
        public void SaveFile(string filename, string content)
        {
            _files[filename] = content;
            Console.WriteLine($"💾 Windows: Saved to LocalFolder: {filename}");
        }
        public string ReadFile(string filename) => _files.ContainsKey(filename) ? _files[filename] : "";
    }

    public class WindowsPhoneNotification : INotification
    {
        public void SendPushNotification(string title, string message)
            => Console.WriteLine($"🔔 Windows: Push notification via WNS - {title}: {message}");
    }

    // Concrete factories
    public class IosFactory : IMobileFactory
    {
        public ICamera CreateCamera() => new IosCamera();
        public IStorage CreateStorage() => new IosStorage();
        public INotification CreateNotification() => new IosNotification();
    }

    public class AndroidFactory : IMobileFactory
    {
        public ICamera CreateCamera() => new AndroidCamera();
        public IStorage CreateStorage() => new AndroidStorage();
        public INotification CreateNotification() => new AndroidNotification();
    }

    public class WindowsPhoneFactory : IMobileFactory
    {
        public ICamera CreateCamera() => new WindowsPhoneCamera();
        public IStorage CreateStorage() => new WindowsPhoneStorage();
        public INotification CreateNotification() => new WindowsPhoneNotification();
    }

    // Factory provider
    public class MobileFactoryProvider
    {
        public static IMobileFactory GetFactory(string platform)
        {
            return platform.ToLower() switch
            {
                "ios" => new IosFactory(),
                "android" => new AndroidFactory(),
                "windows" => new WindowsPhoneFactory(),
                _ => throw new ArgumentException($"Unknown platform: {platform}")
            };
        }
    }

    // Mobile app
    public class MobileApplication
    {
        private ICamera _camera;
        private IStorage _storage;
        private INotification _notification;

        public MobileApplication(IMobileFactory factory)
        {
            _camera = factory.CreateCamera();
            _storage = factory.CreateStorage();
            _notification = factory.CreateNotification();
        }

        public void RunApp()
        {
            Console.WriteLine($"\n📱 Mobile App started");
            _camera.TakePhoto();
            _storage.SaveFile("photo.jpg", "image_data");
            _notification.SendPushNotification("Photo Saved", "Your photo has been saved");
        }
    }
}
