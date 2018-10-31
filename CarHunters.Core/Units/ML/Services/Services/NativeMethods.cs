using System;
using System.Runtime.InteropServices;

namespace CarHunters.Core.Units.ML.Services.Services
{
    public class NativeMethods
    {
        private const string _libraryName = "__Internal";

        [DllImport(_libraryName, EntryPoint = "downscaleGrayImageTwice")]
        public static extern void DownscaleGrayImageTwiceNative(IntPtr gray, int width, int height, IntPtr output);

        [DllImport(_libraryName, EntryPoint = "bgraToGray")]
        public static extern void BgraToGrayNative(IntPtr bgra, int width, int height, IntPtr gray);

        [DllImport(_libraryName, EntryPoint = "cropImage")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int CropImageNative(IntPtr img, int width, int height, int channels,
                                                 int x1, int y1, int x2, int y2, IntPtr cropped);

        [DllImport(_libraryName, EntryPoint = "rotateImage")]
        public static extern void RotateImageNative(IntPtr img, int width, int height, int channels, int rotateTimes, IntPtr rotated);

        [DllImport(_libraryName, EntryPoint = "normalizePixels")]
        public static extern void NormalizePixelsNative(IntPtr pixels, int pixels_n, float low, float hi, IntPtr normalizedValues);

        [DllImport(_libraryName, EntryPoint = "normalizePixelValues")]
        public static extern void NormalizePixelValuesNative(IntPtr pixelValues, int pixelValues_n, float low, float hi, IntPtr normalizedValues);

        [DllImport(_libraryName, EntryPoint = "softmax")]
        public static extern void SoftMaxNative(IntPtr logProbs, int logProbs_n, IntPtr outProbs);

        [DllImport(_libraryName, EntryPoint = "translateByAnchor")]
        public static extern void TranslateByAnchorNative(IntPtr bbox, int anchorIdx, IntPtr outBB);

        [DllImport(_libraryName, EntryPoint = "translateAndTakeMainBB")]
        public static extern void TranslateAndTakeMainBBNative(
            IntPtr logProbsAll,
            IntPtr bboxes,
            [In] int bboxes_n,
            [In] int labels_n,
            int isOrderTransposed,
            IntPtr vehicleLabelIds, //int
            int vehicleLabelIds_n,
            float minScore,
            float minBBoxWidth,
            float minBBoxHeight,
            IntPtr mainBB);

        [DllImport(_libraryName, EntryPoint = "CreateObjectTracker")]
        public static extern void CreateObjectTracker(int frameWidth, int frameHeight);

        [DllImport(_libraryName, EntryPoint = "ReleaseObjectTracker")]
        public static extern void ReleaseObjectTracker();

        [DllImport(_libraryName, EntryPoint = "NextFrame")]
        public static extern void NextFrameNative(IntPtr frameDataGray, long timestamp);

        [DllImport(_libraryName, EntryPoint = "TrackObject")]
        [return: MarshalAs(UnmanagedType.I8)]
        public static extern long TrackObjectNative(IntPtr frameDataGray, long timestamp, float x1, float y1, float x2, float y2);

        [DllImport(_libraryName, EntryPoint = "ForgetTrackedObject")]
        public static extern void ForgetTrackedObject(long handle);

        [DllImport(_libraryName, EntryPoint = "GetCurrentCorrelation")]
        [return: MarshalAs(UnmanagedType.R4)]
        public static extern float GetCurrentCorrelation(long handle);

        [DllImport(_libraryName, EntryPoint = "GetCurrentTrackedBBox")]
        public static extern void GetCurrentTrackedBBoxNative(long handle, IntPtr outBB);

        public static void BgraToGray(byte[] bgra, int width, int height, byte[] gray)
        {
            IntPtr unmanagedBgra = Marshal.AllocHGlobal(Marshal.SizeOf(byte.MinValue) * bgra.Length);
            IntPtr unmanagedGray = Marshal.AllocHGlobal(Marshal.SizeOf(byte.MinValue) * gray.Length);
            Marshal.Copy(bgra, 0, unmanagedBgra, bgra.Length);

            BgraToGrayNative(unmanagedBgra, width, height, unmanagedGray);

            Marshal.Copy(unmanagedGray, gray, 0, gray.Length);

            Marshal.FreeHGlobal(unmanagedBgra);
            Marshal.FreeHGlobal(unmanagedGray);
        }

        public static int CropImage(byte[] img, int width, int height, int channels,
                                    int x1, int y1, int x2, int y2, byte[] cropped)
        {
            IntPtr unmanagedImg = Marshal.AllocHGlobal(Marshal.SizeOf(byte.MinValue) * img.Length);
            IntPtr unmanagedCropped = Marshal.AllocHGlobal(Marshal.SizeOf(byte.MinValue) * cropped.Length);
            Marshal.Copy(img, 0, unmanagedImg, img.Length);

            int ret = CropImageNative(unmanagedImg, width, height, channels, x1, y1, x2, y2, unmanagedCropped);

            Marshal.Copy(unmanagedCropped, cropped, 0, cropped.Length);

            Marshal.FreeHGlobal(unmanagedImg);
            Marshal.FreeHGlobal(unmanagedCropped);

            return ret;
        }

        public static void RotateImage(byte[] img, int width, int height, int channels, int rotateTimes, byte[] rotated)
        {
            IntPtr unmanagedImg = Marshal.AllocHGlobal(Marshal.SizeOf(byte.MinValue) * img.Length);
            IntPtr unmanagedRotated = Marshal.AllocHGlobal(Marshal.SizeOf(byte.MinValue) * rotated.Length);
            Marshal.Copy(img, 0, unmanagedImg, img.Length);

            RotateImageNative(unmanagedImg, width, height, channels, rotateTimes, unmanagedRotated);

            Marshal.Copy(unmanagedRotated, rotated, 0, rotated.Length);

            Marshal.FreeHGlobal(unmanagedImg);
            Marshal.FreeHGlobal(unmanagedRotated);
        }

        public static void DownscaleGrayImageTwice(byte[] gray, int width, int height, byte[] output)
        {
            IntPtr unmanagedPointerbyte = Marshal.AllocHGlobal(Marshal.SizeOf(byte.MinValue) * gray.Length);
            IntPtr unmanagedPointerfloat = Marshal.AllocHGlobal(Marshal.SizeOf(byte.MinValue) * output.Length);
            Marshal.Copy(gray, 0, unmanagedPointerbyte, gray.Length);

            DownscaleGrayImageTwiceNative(unmanagedPointerbyte, width, height, unmanagedPointerfloat);

            Marshal.Copy(unmanagedPointerfloat, output, 0, output.Length);

            Marshal.FreeHGlobal(unmanagedPointerbyte);
            Marshal.FreeHGlobal(unmanagedPointerfloat);
        }

        public static void NormalizePixelValues(ref int[] pixelValues, ref float[] normalizedValues)
        {
            IntPtr unmanagedPointerbyte = Marshal.AllocHGlobal(Marshal.SizeOf(0) * pixelValues.Length);
            IntPtr unmanagedPointerfloat = Marshal.AllocHGlobal(Marshal.SizeOf(1.0f) * normalizedValues.Length);
            Marshal.Copy(pixelValues, 0, unmanagedPointerbyte, pixelValues.Length);

            NormalizePixelsNative(unmanagedPointerbyte, pixelValues.Length, -1f, 1f, unmanagedPointerfloat);

            Marshal.Copy(unmanagedPointerfloat, normalizedValues, 0, normalizedValues.Length);

            Marshal.FreeHGlobal(unmanagedPointerbyte);
            Marshal.FreeHGlobal(unmanagedPointerfloat);
        }

        public static void SoftMax(ref float[] logProbs, int logProbsN, ref float[] outProbs)
        {
            IntPtr unmanagedPointerfloatIn = Marshal.AllocHGlobal(Marshal.SizeOf(1.0f) * logProbs.Length);
            IntPtr unmanagedPointerfloatOut = Marshal.AllocHGlobal(Marshal.SizeOf(1.0f) * outProbs.Length);
            Marshal.Copy(logProbs, 0, unmanagedPointerfloatIn, logProbs.Length);

            SoftMaxNative(unmanagedPointerfloatIn, logProbsN, unmanagedPointerfloatOut);

            Marshal.Copy(unmanagedPointerfloatOut, outProbs, 0, outProbs.Length);

            Marshal.FreeHGlobal(unmanagedPointerfloatIn);
            Marshal.FreeHGlobal(unmanagedPointerfloatOut);
        }

        public static void TranslateByAnchor(ref float[] bbox, int anchorIdx, ref float[] outBB)
        {
            IntPtr unmanagedPointerfloatIn = Marshal.AllocHGlobal(Marshal.SizeOf(1.0f) * bbox.Length);
            IntPtr unmanagedPointerfloatOut = Marshal.AllocHGlobal(Marshal.SizeOf(1.0f) * outBB.Length);
            Marshal.Copy(bbox, 0, unmanagedPointerfloatIn, bbox.Length);

            TranslateByAnchorNative(unmanagedPointerfloatIn, anchorIdx, unmanagedPointerfloatOut);

            Marshal.Copy(unmanagedPointerfloatOut, outBB, 0, outBB.Length);

            Marshal.FreeHGlobal(unmanagedPointerfloatIn);
            Marshal.FreeHGlobal(unmanagedPointerfloatOut);
        }

        public static void TranslateAndTakeMainBB(
            ref float[] logProbsAll,
            ref float[] bboxes,
            int bboxesN,
            int labelsN,
            int isOrderTransposed,
            ref int[] vehicleLabelIds,
            int vehicleLabelIdsN,
            float minScore,
            float minBBoxWidth,
            float minBBoxHeight,
            ref float[] mainBB)
        {
            IntPtr unmanagedPointerfloatIn = Marshal.AllocHGlobal(Marshal.SizeOf(1.0f) * logProbsAll.Length);
            IntPtr unmanagedPointerfloatInBboxes = Marshal.AllocHGlobal(Marshal.SizeOf(1.0f) * bboxes.Length);
            IntPtr unmanagedPointerintInIds = Marshal.AllocHGlobal(Marshal.SizeOf(0) * vehicleLabelIds.Length);

            IntPtr unmanagedPointerfloatOutMainBB = Marshal.AllocHGlobal(Marshal.SizeOf(1.0f) * mainBB.Length);

            Marshal.Copy(logProbsAll, 0, unmanagedPointerfloatIn, logProbsAll.Length);
            Marshal.Copy(bboxes, 0, unmanagedPointerfloatInBboxes, bboxes.Length);
            Marshal.Copy(vehicleLabelIds, 0, unmanagedPointerintInIds, vehicleLabelIds.Length);

            TranslateAndTakeMainBBNative(unmanagedPointerfloatIn, unmanagedPointerfloatInBboxes, bboxesN, labelsN, isOrderTransposed, unmanagedPointerintInIds, vehicleLabelIdsN, minScore, minBBoxWidth, minBBoxHeight, unmanagedPointerfloatOutMainBB);

            Marshal.Copy(unmanagedPointerfloatOutMainBB, mainBB, 0, mainBB.Length);

            Marshal.FreeHGlobal(unmanagedPointerfloatIn);
            Marshal.FreeHGlobal(unmanagedPointerfloatInBboxes);
            Marshal.FreeHGlobal(unmanagedPointerfloatOutMainBB);
            Marshal.FreeHGlobal(unmanagedPointerintInIds);
        }

        public static void NextFrame(byte[] frameDataGray, long timestamp)
        {
            IntPtr unmanagedPointerFrameDataGray = Marshal.AllocHGlobal(Marshal.SizeOf((byte)0) * frameDataGray.Length);
            Marshal.Copy(frameDataGray, 0, unmanagedPointerFrameDataGray, frameDataGray.Length);
            NextFrameNative(unmanagedPointerFrameDataGray, timestamp);
            Marshal.FreeHGlobal(unmanagedPointerFrameDataGray);
        }

        public static long TrackObject(byte[] frameDataGray, long timestamp, float x1, float y1, float x2, float y2)
        {
            IntPtr unmanagedPointerFrameDataGray = Marshal.AllocHGlobal(Marshal.SizeOf((byte)0) * frameDataGray.Length);
            Marshal.Copy(frameDataGray, 0, unmanagedPointerFrameDataGray, frameDataGray.Length);
            var retValue = TrackObjectNative(unmanagedPointerFrameDataGray, timestamp, x1, y1, x2, y2);
            Marshal.FreeHGlobal(unmanagedPointerFrameDataGray);
            return retValue;
        }

        public static void GetCurrentTrackedBBox(long handle, float[] outBB)
        {
            IntPtr unmanagedPointerfloatOutMainBB = Marshal.AllocHGlobal(Marshal.SizeOf(1.0f) * outBB.Length);

            GetCurrentTrackedBBoxNative(handle, unmanagedPointerfloatOutMainBB);

            Marshal.Copy(unmanagedPointerfloatOutMainBB, outBB, 0, outBB.Length);

            Marshal.FreeHGlobal(unmanagedPointerfloatOutMainBB);
        }
    }
}
