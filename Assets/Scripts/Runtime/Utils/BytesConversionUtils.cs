using System;

namespace Game.Utils
{
    public static class BytesConversionUtils
    {
        public const long KB = 1024;
        public const long MB = KB * KB;
        public const long GB = MB * KB;
        public const long TB = GB * KB;

        // TODO: Better use https://github.com/omar/ByteSize
        public static string ToHumanizedString(ulong bytes)
        {
            return bytes switch
            {
                < KB => $"{bytes}B",
                >= KB and < MB => $"{bytes / KB}KB",
                >= MB and < GB => $"{bytes / MB}MB",
                >= GB and < TB => $"{bytes / MB}GB",
                >= TB => $"{bytes / TB}"
            };
        }
        
        // TODO: Better use https://github.com/omar/ByteSize
        public static string ToHumanizedString(long bytes)
        {
            if (bytes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), bytes, "must be greater or equal 0 (Zero)");
            }
            
            return bytes switch
            {
                < KB => $"{bytes}B",
                >= KB and < MB => $"{bytes / KB}KB",
                >= MB and < GB => $"{bytes / MB}MB",
                >= GB and < TB => $"{bytes / MB}GB",
                >= TB => $"{bytes / TB}"
            };
        }
    }
}