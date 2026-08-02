using ImageProcessing.After.Abstracts;
using ImageProcessing.After.Processors;

namespace ImageProcessing.After.Creators
{
    public class PreviewCreator : ImageCreator
    {
        protected override IImageProcessor CreateImageProcessor()
        {
            return new PreviewProcessor();
        }
    }
}
