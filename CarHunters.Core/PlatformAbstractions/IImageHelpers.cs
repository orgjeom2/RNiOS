using System.Drawing;
using System.IO;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.PlatformAbstractions
{
    public interface IImageHelpers
    {
        string AppVersion();
        object ToNativeImage(FrameEntry frame);
        byte[] ToJpegBytes(FrameEntry frame);
        Stream GetStreamByPath(string path);
        int DeviceOrientationAsNumber();
        RectangleF RotateBBoxClockwise(RectangleF bb, int rotateTimes);
        FrameEntry RotateImage(FrameEntry frame, int rotateTimes);
        FrameEntry CropImage(FrameEntry frame, Rectangle rect);
    }
}
