using ImageProcessing.After.Abstracts;
using ImageProcessing.After.Processors;

namespace ImageProcessing.After.Creators
{
    public class FullResolutionCreator : ImageCreator
    {
        protected override IImageProcessor CreateImageProcessor()
        {
            return new FullResolutionProcessor();
        }
    }
}
