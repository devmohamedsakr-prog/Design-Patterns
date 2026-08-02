using System;
using System.Threading.Tasks;
using ImageProcessing.After.Abstracts;

namespace ImageProcessing.After.Processors
{
    public class FullResolutionProcessor : IImageProcessor
    {
        public string GetProcessorName() => "FullResolution";

        public async Task<ImageResult> ProcessAsync(string imagePath, int width, int height)
        {
            await Task.Delay(300); // Simulate full-res generation (slower)

            return new ImageResult
            {
                Success = true,
                ProcessorName = GetProcessorName(),
                OutputPath = $"{imagePath}_fullres_{width}x{height}.jpg",
                Width = width,
                Height = height,
                FileSize = 2500, // KB (high quality)
                Message = $"Full Resolution ({width}x{height}) generated"
            };
        }
    }
}
