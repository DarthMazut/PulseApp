using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Represents a quantity of memory space.
    /// </summary>
    public struct MemorySpace
    {
        private readonly long _bytesSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySpace"/> struct.
        /// </summary>
        /// <param name="byteSize">Size of space to be represented in bytes.</param>
        public MemorySpace(long byteSize)
        {
            _bytesSize = byteSize;
        }

        /// <summary>
        /// Returns the number of bytes of the represented memory fragment.
        /// </summary>
        public long Bytes => _bytesSize;

        /// <summary>
        /// Returns the number of kilobytes of the represented memory fragment.
        /// </summary>
        public double Kilobytes => _bytesSize / (double)MemoryUnit.Kilobyte;

        /// <summary>
        /// Returns the number of megabytes of the represented memory fragment.
        /// </summary>
        public double Megabytes => _bytesSize / (double)MemoryUnit.Megabyte;

        /// <summary>
        /// Returns the number of gigabytes of the represented memory fragment.
        /// </summary>
        public double Gigabytes => _bytesSize / (double)MemoryUnit.Gigabyte;

        public MemoryUnit GetSizeWithMostSuitableUnit(out double value)
        {
            if (Bytes >= (long)MemoryUnit.Gigabyte)
            {
                value = Gigabytes;
                return MemoryUnit.Gigabyte;
            }

            if (Bytes >= (long)MemoryUnit.Megabyte)
            {
                value = Megabytes;
                return MemoryUnit.Megabyte;
            }


            if (Bytes >= (long)MemoryUnit.Kilobyte)
            {
                value = Kilobytes;
                return MemoryUnit.Kilobyte;
            }

            value = Bytes;
            return MemoryUnit.Byte;
        }

        public override string ToString()
        {
            MemoryUnit unit = GetSizeWithMostSuitableUnit(out double value);
            string unitString = unit switch
            {
                MemoryUnit.Byte => "B",
                MemoryUnit.Kilobyte => "KB",
                MemoryUnit.Megabyte => "MB",
                MemoryUnit.Gigabyte => "GB",
                _ => "[ - ]"
            };
            return $"{Math.Round(value, 2)} {unitString}";
        }

        public static MemorySpace FromBytes(long byteSize)
        {
            return new MemorySpace(byteSize);
        }

        public static MemorySpace FromKilobytes(double kilobytes)
        {
            return new MemorySpace((long)(kilobytes * (long)MemoryUnit.Kilobyte));
        }

        public static MemorySpace FromMegabytes(double megabytes)
        {
            return new MemorySpace((long)(megabytes * (long)MemoryUnit.Megabyte));
        }
        public static MemorySpace FromGigabytes(double gigabytes)
        {
            return new MemorySpace((long)(gigabytes * (long)MemoryUnit.Gigabyte));
        }

        public static MemorySpace operator +(MemorySpace x, MemorySpace y)
        {
            return new MemorySpace(x.Bytes + y.Bytes);
        }

        public static MemorySpace operator -(MemorySpace x, MemorySpace y)
        {
            return new MemorySpace(x.Bytes - y.Bytes);
        }

        public static bool operator >(MemorySpace x, MemorySpace y)
        {
            return x.Bytes > y.Bytes;
        }

        public static bool operator <(MemorySpace x, MemorySpace y)
        {
            return (x.Bytes < y.Bytes);
        }
    }

    public enum MemoryUnit
    {
        Byte = 1,
        Kilobyte = 1024,
        Megabyte = 1024 * 1024,
        Gigabyte = 1024 * 1024 * 1024
    }
}
