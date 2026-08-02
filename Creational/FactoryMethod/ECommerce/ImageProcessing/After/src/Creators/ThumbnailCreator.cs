using ImageProcessing.After.Abstracts;
using ImageProcessing.After.Processors;

namespace ImageProcessing.After.Creators
{
    public class ThumbnailCreator : ImageCreator
    {
        protected override IImageProcessor CreateImageProcessor()
        {
            return new ThumbnailProcessor();
        }
    }
}
