using System;

namespace CarHunters.Core.Common.Models
{
    public class FrameEntry
    {
        public byte[] Frame { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}