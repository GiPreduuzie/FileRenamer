using System;
using System.IO;
using DecompiledMp3Lib.Id3Lib.Exceptions;

namespace DecompiledMp3Lib.Id3Lib
{
    internal static class Sync
    {
        public static uint Unsafe(uint val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if ((int)bytes[0] > (int)sbyte.MaxValue || (int)bytes[1] > (int)sbyte.MaxValue || ((int)bytes[2] > (int)sbyte.MaxValue || (int)bytes[3] > (int)sbyte.MaxValue))
                throw new InvalidTagException("Sync-safe value corrupted");
            return BitConverter.ToUInt32(new byte[4]
      {
        (byte) ((int) bytes[0] & (int) sbyte.MaxValue | ((int) bytes[1] & 1) << 7),
        (byte) ((int) bytes[1] >> 1 & 63 | ((int) bytes[2] & 3) << 6),
        (byte) ((int) bytes[2] >> 2 & 31 | ((int) bytes[3] & 7) << 5),
        (byte) ((int) bytes[3] >> 3 & 15)
      }, 0);
        }

        public static uint Safe(uint val)
        {
            if (val > 268435456U)
                throw new OverflowException("value is too large for a sync-safe integer");
            byte[] bytes = BitConverter.GetBytes(val);
            return BitConverter.ToUInt32(new byte[4]
      {
        (byte) ((uint) bytes[0] & (uint) sbyte.MaxValue),
        (byte) ((int) bytes[0] >> 7 & 1 | (int) bytes[1] << 1 & (int) sbyte.MaxValue),
        (byte) ((int) bytes[1] >> 6 & 3 | (int) bytes[2] << 2 & (int) sbyte.MaxValue),
        (byte) ((int) bytes[2] >> 5 & 7 | (int) bytes[3] << 3 & (int) sbyte.MaxValue)
      }, 0);
        }

        public static uint UnsafeBigEndian(uint val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if ((int)bytes[0] > (int)sbyte.MaxValue || (int)bytes[1] > (int)sbyte.MaxValue || ((int)bytes[2] > (int)sbyte.MaxValue || (int)bytes[3] > (int)sbyte.MaxValue))
                throw new InvalidTagException("Sync-safe value corrupted");
            byte[] numArray = new byte[4];
            numArray[3] = (byte)((int)bytes[3] & (int)sbyte.MaxValue | ((int)bytes[2] & 1) << 7);
            numArray[2] = (byte)((int)bytes[2] >> 1 & 63 | ((int)bytes[1] & 3) << 6);
            numArray[1] = (byte)((int)bytes[1] >> 2 & 31 | ((int)bytes[0] & 7) << 5);
            numArray[0] = (byte)((int)bytes[0] >> 3 & 15);
            return BitConverter.ToUInt32(numArray, 0);
        }

        public static uint SafeBigEndian(uint val)
        {
            if (val > 268435456U)
                throw new OverflowException("value is too large for a sync-safe integer");
            byte[] bytes = BitConverter.GetBytes(val);
            byte[] numArray = new byte[4];
            numArray[3] = (byte)((uint)bytes[3] & (uint)sbyte.MaxValue);
            numArray[2] = (byte)((int)bytes[3] >> 7 & 1 | (int)bytes[2] << 1 & (int)sbyte.MaxValue);
            numArray[1] = (byte)((int)bytes[2] >> 6 & 3 | (int)bytes[1] << 2 & (int)sbyte.MaxValue);
            numArray[0] = (byte)((int)bytes[1] >> 5 & 7 | (int)bytes[0] << 3 & (int)sbyte.MaxValue);
            return BitConverter.ToUInt32(numArray, 0);
        }

        public static uint Unsafe(Stream src, Stream dst, uint size)
        {
            BinaryWriter binaryWriter = new BinaryWriter(dst);
            BinaryReader binaryReader = new BinaryReader(src);
            byte num1 = (byte)0;
            uint num2 = 0U;
            for (uint index = 0U; index < size; ++index)
            {
                byte num3 = binaryReader.ReadByte();
                if ((int)num1 == (int)byte.MaxValue && (int)num3 == 0)
                    ++num2;
                else
                    binaryWriter.Write(num3);
                num1 = num3;
            }
            if ((int)num1 == (int)byte.MaxValue)
            {
                binaryWriter.Write((byte)0);
                ++num2;
            }
            dst.Seek(0L, SeekOrigin.Begin);
            return num2;
        }

        public static uint Safe(Stream src, Stream dst, uint count)
        {
            BinaryWriter binaryWriter = new BinaryWriter(dst);
            BinaryReader binaryReader = new BinaryReader(src);
            byte num1 = (byte)0;
            uint num2 = 0U;
            for (; count > 0U; --count)
            {
                byte num3 = binaryReader.ReadByte();
                if ((int)num1 == (int)byte.MaxValue && ((int)num3 == 0 || (int)num3 >= 224))
                {
                    binaryWriter.Write((byte)0);
                    ++num2;
                }
                num1 = num3;
                binaryWriter.Write(num3);
            }
            if ((int)num1 == (int)byte.MaxValue)
            {
                binaryWriter.Write((byte)0);
                ++num2;
            }
            return num2;
        }
    }
}
