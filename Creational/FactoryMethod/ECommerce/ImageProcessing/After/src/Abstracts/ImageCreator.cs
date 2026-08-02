using System;
using System.Threading.Tasks;

namespace ImageProcessing.After.Abstracts
{
    public abstract class ImageCreator
    {
        protected abstract IImageProcessor CreateImageProcessor();

        public async Task<ImageResult> ProcessImageAsync(string imagePath, int width, int height)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath) || width <= 0 || height <= 0)
                    return new ImageResult { Success = false, Message = "Invalid parameters" };

                IImageProcessor processor = CreateImageProcessor();
                ImageResult result = await processor.ProcessAsync(imagePath, width, height);

                LogProcessing(imagePath, processor.GetProcessorName(), result.Success ? "SUCCESS" : "FAILED");
                return result;
            }
            catch (Exception ex)
            {
                return new ImageResult { Success = false, Message = ex.Message };
            }
        }

        protected virtual void LogProcessing(string path, string processor, string status)
        {
            Console.WriteLine($"[LOG] Image: {path}, Processor: {processor}, Status: {status}");
        }
    }

    public interface IImageProcessor
    {
        Task<ImageResult> ProcessAsync(string imagePath, int width, int height);
        string GetProcessorName();
    }

    public class ImageResult
    {
        public bool Success { get; set; }
        public string ProcessorName { get; set; }
        public string OutputPath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int FileSize { get; set; } // KB
        public string Message { get; set; }
    }
}
