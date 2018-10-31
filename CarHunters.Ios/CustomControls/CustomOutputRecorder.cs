using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AVFoundation;
using CarHunters.Core.Common.Models;
using CoreMedia;
using CoreVideo;
using Foundation;
using MvvmCross;
using CarHunters.Core.Common.Abstractions;

namespace CarHunters.Ios.CustomControls
{
    public class CustomOutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate, IAVCaptureVideoDataOutputSampleBufferDelegate
    {
        long frameNumber;
        long frameId;
        double initialTime;
        readonly object lockObj = new Object();
        readonly List<byte[]> CVBuffers = new List<byte[]>();
        Action<int> action;

        public CustomOutputRecorder()//Action<int> action
        {
            //this.action = action;
            initialTime = NSProcessInfo.ProcessInfo.SystemUptime;
        }

        public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
            frameNumber++;
            frameId++;
            if (frameNumber % 30 == 0)
            {
                var currentTime = NSProcessInfo.ProcessInfo.SystemUptime;
                var fps = Math.Round(frameNumber / (currentTime - initialTime));
                action?.Invoke((int)fps);
                frameNumber = 0;
                initialTime = NSProcessInfo.ProcessInfo.SystemUptime;
            }

            var buffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer;

            var frameEntry = new FrameEntry
            {
                TimeStamp = DateTimeOffset.UtcNow,
                Width = (int)buffer.Width,
                Height = (int)buffer.Height,
                Frame = CVPixelBufferToByte(buffer)
            };

            Mvx.IoCProvider.Resolve<IInternalNotificationHubService>().NewFrame(frameEntry);

            TryDispose(sampleBuffer);
            TryDispose(buffer);
        }

        public new void Dispose()
        {
            action = null;
        }

        void TryDispose(object obj)
        {
            if (obj is IDisposable disp)
            {
                disp.Dispose();
            }
        }

        private byte[] CVPixelBufferToByte(CVPixelBuffer pixelBuffer)
        {
            pixelBuffer.Lock(CVPixelBufferLock.None);

            CVPixelFormatType ft = pixelBuffer.PixelFormatType;
            var baseAddress = pixelBuffer.BaseAddress;
            var bytesPerRow = (int)pixelBuffer.BytesPerRow;
            var width = (int)pixelBuffer.Width;
            var height = (int)pixelBuffer.Height;
            var lenght = bytesPerRow * height;
            byte[] managedArray = new byte[lenght];
            Marshal.Copy(baseAddress, managedArray, 0, lenght);

            pixelBuffer.Unlock(CVPixelBufferLock.None);
            return managedArray;
        }
    }
}
