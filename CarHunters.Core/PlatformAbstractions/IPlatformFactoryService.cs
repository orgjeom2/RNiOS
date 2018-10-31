using System;

namespace CarHunters.Core.PlatformAbstractions
{
    public interface IPlatformFactoryService
    {
        IImageHelpers CreateImageHelpers();
        IMNSSD CreateMNSSD();
        IImageClassifier CreateImageClassifier(string modelName, int outSize);
    }
}
