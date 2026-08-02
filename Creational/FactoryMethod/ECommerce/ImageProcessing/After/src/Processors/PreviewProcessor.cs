using System;
using System.Threading.Tasks;
using ImageProcessing.After.Abstracts;

namespace ImageProcessing.After.Processors
{
    public class PreviewProcessor : IImageProcessor
    {
        public string GetProcessorName() => "Preview";

        public async Task<ImageResult> ProcessAsync(string imagePath, int width, int height)
        {
            await Task.Delay(150); // Simulate preview generation

            return new ImageResult
            {
                Success = true,
                ProcessorName = GetProcessorName(),
                OutputPath = $"{imagePath}_preview_{width}x{height}.jpg",
                Width = width,
                Height = height,
                FileSize = 150, // KB
                Message = $"Preview ({width}x{height}) generated"
            };
        }
    }
}
