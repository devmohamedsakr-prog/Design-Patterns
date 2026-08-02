using System;

namespace Proxy.ImageLoading.Graphics.Component
{
    // Subject: Image interface
    public interface IImage
    {
        void Display();
        int GetSize();
    }

    // Real Subject: High-resolution image
    public class HighResolutionImage : IImage
    {
        private string _filename;
        private byte[] _imageData;

        public HighResolutionImage(string filename)
        {
            _filename = filename;
            LoadImage();
        }

        private void LoadImage()
        {
            // Simulate expensive image loading
            System.Threading.Thread.Sleep(100);
            _imageData = new byte[5_000_000]; // 5 MB
            Console.WriteLine($"Loaded high-res image: {_filename}");
        }

        public void Display()
        {
            Console.WriteLine($"Displaying high-res image: {_filename} ({GetSize()} bytes)");
        }

        public int GetSize() => _imageData?.Length ?? 0;
    }

    // Proxy: Defers image loading until needed
    public class ImageProxy : IImage
    {
        private string _filename;
        private HighResolutionImage _realImage;
        private bool _isLoaded;

        public ImageProxy(string filename)
        {
            _filename = filename;
            _isLoaded = false;
            Console.WriteLine($"Created proxy for: {_filename} (not loaded yet)");
        }

        public void Display()
        {
            if (!_isLoaded)
            {
                _realImage = new HighResolutionImage(_filename);
                _isLoaded = true;
            }
            _realImage.Display();
        }

        public int GetSize()
        {
            if (!_isLoaded)
                return 0; // Lightweight until needed
            return _realImage.GetSize();
        }

        public bool IsLoaded => _isLoaded;
    }
}
