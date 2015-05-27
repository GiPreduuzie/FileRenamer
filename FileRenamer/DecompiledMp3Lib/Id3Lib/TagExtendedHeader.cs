using System;
using System.IO;
using DecompiledMp3Lib.Id3Lib.Exceptions;

namespace DecompiledMp3Lib.Id3Lib
{
    public class TagExtendedHeader
    {
        private uint _size;
        private byte[] _extendedHeader;

        public uint Size
        {
            get
            {
                return this._size;
            }
        }

        public void Deserialize(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            this._size = Swap.UInt32(Sync.UnsafeBigEndian(new BinaryReader(stream).ReadUInt32()));
            if (this._size < 6U)
                throw new InvalidFrameException("Corrupt id3 extended header.");
            //this._extendedHeader = new byte[(IntPtr)this._size];
            this._extendedHeader = new byte[_size];
            stream.Read(this._extendedHeader, 0, (int)this._size);
        }

        public void Serialize(Stream stream)
        {
            new BinaryWriter(stream).Write(this._extendedHeader);
        }
    }
}
