using System;
using System.Threading.Tasks;
using ImageProcessing.After.Abstracts;

namespace ImageProcessing.After.Processors
{
    public class ThumbnailProcessor : IImageProcessor
    {
        public string GetProcessorName() => "Thumbnail";

        public async Task<ImageResult> ProcessAsync(string imagePath, int width, int height)
        {
            await Task.Delay(100); // Simulate thumbnail generation

            return new ImageResult
            {
                Success = true,
                ProcessorName = GetProcessorName(),
                OutputPath = $"{imagePath}_thumb_{width}x{height}.jpg",
                Width = width,
                Height = height,
                FileSize = 15, // KB
                Message = $"Thumbnail ({width}x{height}) generated"
            };
        }
    }
}
