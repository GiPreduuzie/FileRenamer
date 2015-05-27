namespace DecompiledMp3Lib.Id3Lib
{
    internal static class Swap
    {
        public static int Int32(int val)
        {
            return (int)Swap.UInt32((uint)val);
        }

        public static uint UInt32(uint val)
        {
            return (uint)(((int)val & (int)byte.MaxValue) << 24) | (uint)(((int)val & 65280) << 8) | (val & 16711680U) >> 8 | (val & 4278190080U) >> 24;
        }

        public static short Int16(short val)
        {
            return (short)Swap.UInt16((ushort)val);
        }

        public static ushort UInt16(ushort val)
        {
            return (ushort)((uint)(((int)val & (int)byte.MaxValue) << 8) | ((uint)val & 65280U) >> 8);
        }
    }
}