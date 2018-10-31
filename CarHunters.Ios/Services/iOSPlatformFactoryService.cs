using CarHunters.Core.PlatformAbstractions;

namespace CarHunters.Ios.Helpers
{
    public class iOSPlatformFactoryService : IPlatformFactoryService
    {
        public IImageHelpers CreateImageHelpers()
        {
            return new iOSImageHelpers();
        }

        public IMNSSD CreateMNSSD()
        {
            return new iOSMNSSD();
        }

        public IImageClassifier CreateImageClassifier(string modelName, int outSize)
        {
            return new iOSImageClassifier(modelName, outSize);
        }
    }
}