using System;

namespace DecompiledMp3Lib.Mp3Lib
{
    internal static class Memory
    {
        public static bool Compare(byte[] b1, byte[] b2)
        {
            if (b1 == null)
                throw new ArgumentNullException("b1");
            if (b2 == null)
                throw new ArgumentNullException("b2");
            if (b1.Length != b2.Length)
                return false;
            for (int index = 0; index < b1.Length; ++index)
            {
                if ((int)b1[index] != (int)b2[index])
                    return false;
            }
            return true;
        }

        public static byte[] Extract(byte[] src, int srcIndex, int count)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (src == null || srcIndex < 0 || count < 0)
                throw new InvalidOperationException();
            if (src.Length - srcIndex < count)
                throw new InvalidOperationException();
            byte[] numArray = new byte[count];
            Array.Copy((Array)src, srcIndex, (Array)numArray, 0, count);
            return numArray;
        }

        public static int FindByte(byte[] src, byte val, int index)
        {
            int length = src.Length;
            if (index > length)
                throw new InvalidOperationException();
            for (int index1 = index; index1 < length; ++index1)
            {
                if ((int)src[index1] == (int)val)
                    return index1 - index;
            }
            return -1;
        }

        public static int FindShort(byte[] src, short val, int index)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            int length = src.Length;
            if (index > length)
                throw new InvalidOperationException();
            int startIndex = index;
            while (startIndex < length)
            {
                if ((int)BitConverter.ToInt16(src, startIndex) == (int)val)
                    return startIndex - index;
                startIndex += 2;
            }
            return -1;
        }

        public static void Clear(byte[] dst, int begin, int end)
        {
            if (dst == null)
                throw new ArgumentNullException("dst");
            if (begin > end || begin > dst.Length || end > dst.Length)
                throw new InvalidOperationException();
            Array.Clear((Array)dst, begin, end - begin);
        }

        public static ulong ToInt64(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (value.Length > 8)
                throw new InvalidOperationException("The count is to large to be stored");
            return BitConverter.ToUInt64(value, 0);
        }

        public static byte[] GetBytes(ulong value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
