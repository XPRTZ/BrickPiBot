using System;
using System.ComponentModel;
using System.IO;
using System.Threading;


namespace Iot.Device.BrickPi3.Utils
{
    /// <summary>
    /// Byte utilities
    /// </summary>
    public class ByteUtils
    {
        public static int ByteArrayToInt(byte[] bytes)
        {
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            if(bytes.Length == 2)
            {
                return BitConverter.ToInt16(bytes);
            }
            else if(bytes.Length == 4)
            {
                return BitConverter.ToInt32(bytes);
            }
            else
            {
                return 0;
            }
        }

        public static int SliceByteArrayToInt(ref byte[] bytes, int start, int length)
        {
            if(start + length > bytes.Length)
            {
                return 0;
            }

            var slice = bytes[start..(start + length)];

            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(slice);
            }

            if(length == 1)
            {
                return slice[0];
            }
            else if (length == 2)
            {
                return BitConverter.ToInt16(slice);
            }
            else if(length == 4)
            {
                return BitConverter.ToInt32(slice);
            }
            else
            {
                return 0;
            }

        }

        public static long SliceByteArrayToLong(ref byte[] bytes, int start, int length)
        {
            if (start + length > bytes.Length)
            {
                return 0;
            }

            if (length < 8)
            {
                return SliceByteArrayToInt(ref bytes, start, length);
            }
            else if(length == 8)
            {
                var slice = bytes[start..(start + length)];

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(slice);
                }
                return BitConverter.ToInt64(slice);
            }
            else
            {
                return 0;
            }

        }

    }
}