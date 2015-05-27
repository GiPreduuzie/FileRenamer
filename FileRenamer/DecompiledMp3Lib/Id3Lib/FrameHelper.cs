using System;
using System.IO;
using DecompiledMp3Lib.Id3Lib.Exceptions;
using DecompiledMp3Lib.Id3Lib.Frames;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace DecompiledMp3Lib.Id3Lib
{
    internal class FrameHelper
    {
        private byte _version;
        private byte _revison;

        public byte Version
        {
            get
            {
                return this._version;
            }
        }

        public byte Revision
        {
            get
            {
                return this._revison;
            }
        }

        public FrameHelper(TagHeader header)
        {
            this._version = header.Version;
            this._revison = header.Revision;
        }

        public FrameBase Build(string frameId, ushort flags, byte[] buffer)
        {
            FrameBase frame = FrameFactory.Build(frameId);
            this.SetFlags(frame, flags);
            uint num = 0U;
            uint size = (uint)buffer.Length;
            Stream stream = (Stream)new MemoryStream(buffer, false);
            BinaryReader binaryReader = new BinaryReader(stream);
            if (this.GetGrouping(flags))
            {
                frame.Group = new byte?(binaryReader.ReadByte());
                ++num;
            }
            if (frame.Compression)
            {
                switch (this.Version)
                {
                    case (byte)3:
                        size = Swap.UInt32(binaryReader.ReadUInt32());
                        break;
                    case (byte)4:
                        size = Swap.UInt32(Sync.UnsafeBigEndian(binaryReader.ReadUInt32()));
                        break;
                    default:
                        throw new NotImplementedException("ID3v2 Version " + (object)this.Version + " is not supported.");
                }
                num = 0U;
                stream = (Stream)new InflaterInputStream(stream);
            }
            if (frame.Encryption)
                throw new NotImplementedException("Encryption is not implemented, consequently it is not supported.");
            if (frame.Unsynchronisation)
            {
                MemoryStream memoryStream = new MemoryStream();
                size = Sync.Unsafe(stream, (Stream)memoryStream, size);
                num = 0U;
                memoryStream.Seek(0L, SeekOrigin.Begin);
                stream = (Stream)memoryStream;
            }
            //byte[] numArray = new byte[(IntPtr)(size - num)];
            byte[] numArray = new byte[(size - num)];
            stream.Read(numArray, 0, (int)size - (int)num);
            frame.Parse(numArray);
            return frame;
        }

        public byte[] Make(FrameBase frame, out ushort flags)
        {
            flags = this.GetFlags(frame);
            byte[] numArray1 = frame.Make();
            MemoryStream memoryStream1 = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter((Stream)memoryStream1);
            if (frame.Group.HasValue)
                binaryWriter.Write(frame.Group.Value);
            if (frame.Compression)
            {
                switch (this.Version)
                {
                    case (byte)3:
                        binaryWriter.Write(Swap.Int32(numArray1.Length));
                        break;
                    case (byte)4:
                        binaryWriter.Write(Sync.UnsafeBigEndian(Swap.UInt32((uint)numArray1.Length)));
                        break;
                    default:
                        throw new NotImplementedException("ID3v2 Version " + (object)this.Version + " is not supported.");
                }
                byte[] numArray2 = new byte[2048];
                Deflater deflater = new Deflater(9);
                deflater.SetInput(numArray1, 0, numArray1.Length);
                deflater.Finish();
                while (!deflater.IsNeedingInput)
                {
                    int count = deflater.Deflate(numArray2, 0, numArray2.Length);
                    if (count > 0)
                        memoryStream1.Write(numArray2, 0, count);
                    else
                        break;
                }
                if (!deflater.IsNeedingInput)
                    throw new InvalidFrameException("Can't decompress frame '" + frame.FrameId + "' missing data");
            }
            else
                memoryStream1.Write(numArray1, 0, numArray1.Length);
            if (frame.Encryption)
                throw new NotImplementedException("Encryption is not implemented, consequently it is not supported.");
            if (frame.Unsynchronisation)
            {
                MemoryStream memoryStream2 = new MemoryStream();
                int num = (int)Sync.Unsafe((Stream)memoryStream1, (Stream)memoryStream2, (uint)memoryStream1.Position);
                memoryStream1 = memoryStream2;
            }
            return memoryStream1.ToArray();
        }

        private void SetFlags(FrameBase frame, ushort flags)
        {
            frame.TagAlter = this.GetTagAlter(flags);
            frame.FileAlter = this.GetFileAlter(flags);
            frame.ReadOnly = this.GetReadOnly(flags);
            frame.Compression = this.GetCompression(flags);
            frame.Encryption = this.GetEncryption(flags);
            frame.Unsynchronisation = this.GetUnsynchronisation(flags);
            frame.DataLength = this.GetDataLength(flags);
        }

        private bool GetTagAlter(ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    return ((int)flags & 32768) > 0;
                case (byte)4:
                    return ((int)flags & 16384) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private bool GetFileAlter(ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    return ((int)flags & 16384) > 0;
                case (byte)4:
                    return ((int)flags & 8192) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private bool GetReadOnly(ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    return ((int)flags & 8192) > 0;
                case (byte)4:
                    return ((int)flags & 4096) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private bool GetGrouping(ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    return ((int)flags & 32) > 0;
                case (byte)4:
                    return ((int)flags & 64) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private bool GetCompression(ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    return ((int)flags & 128) > 0;
                case (byte)4:
                    return ((int)flags & 8) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private bool GetEncryption(ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    return ((int)flags & 64) > 0;
                case (byte)4:
                    return ((int)flags & 4) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private bool GetUnsynchronisation(ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    return false;
                case (byte)4:
                    return ((int)flags & 2) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private bool GetDataLength(ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    return false;
                case (byte)4:
                    return ((int)flags & 1) > 0;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private ushort GetFlags(FrameBase frame)
        {
            ushort flags = (ushort)0;
            this.SetTagAlter(frame.TagAlter, ref flags);
            this.SetFileAlter(frame.FileAlter, ref flags);
            this.SetReadOnly(frame.ReadOnly, ref flags);
            this.SetGrouping(frame.Group.HasValue, ref flags);
            this.SetCompression(frame.Compression, ref flags);
            this.SetEncryption(frame.Encryption, ref flags);
            this.SetUnsynchronisation(frame.Unsynchronisation, ref flags);
            this.SetDataLength(frame.DataLength, ref flags);
            return flags;
        }

        private void SetTagAlter(bool value, ref ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    flags = value ? (ushort)((uint)flags | 32768U) : (ushort)((uint)flags & (uint)short.MaxValue);
                    break;
                case (byte)4:
                    flags = value ? (ushort)((uint)flags | 16384U) : (ushort)((uint)flags & 49151U);
                    break;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private void SetFileAlter(bool value, ref ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    flags = value ? (ushort)((uint)flags | 16384U) : (ushort)((uint)flags & 49151U);
                    break;
                case (byte)4:
                    flags = value ? (ushort)((uint)flags | 8192U) : (ushort)((uint)flags & 57343U);
                    break;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private void SetReadOnly(bool value, ref ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    flags = value ? (ushort)((uint)flags | 8192U) : (ushort)((uint)flags & 57343U);
                    break;
                case (byte)4:
                    flags = value ? (ushort)((uint)flags | 4096U) : (ushort)((uint)flags & 61439U);
                    break;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private void SetGrouping(bool value, ref ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    flags = value ? (ushort)((uint)flags | 32U) : (ushort)((uint)flags & 65503U);
                    break;
                case (byte)4:
                    flags = value ? (ushort)((uint)flags | 64U) : (ushort)((uint)flags & 65471U);
                    break;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private void SetCompression(bool value, ref ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    flags = value ? (ushort)((uint)flags | 128U) : (ushort)((uint)flags & 65407U);
                    break;
                case (byte)4:
                    flags = value ? (ushort)((uint)flags | 8U) : (ushort)((uint)flags & 65527U);
                    break;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private void SetEncryption(bool value, ref ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    flags = value ? (ushort)((uint)flags | 64U) : (ushort)((uint)flags & 65471U);
                    break;
                case (byte)4:
                    flags = value ? (ushort)((uint)flags | 4U) : (ushort)((uint)flags & 65531U);
                    break;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private void SetUnsynchronisation(bool value, ref ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    break;
                case (byte)4:
                    flags = value ? (ushort)((uint)flags | 2U) : (ushort)((uint)flags & 65533U);
                    break;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }

        private void SetDataLength(bool value, ref ushort flags)
        {
            switch (this._version)
            {
                case (byte)3:
                    break;
                case (byte)4:
                    flags = value ? (ushort)((uint)flags | 1U) : (ushort)((uint)flags & 65534U);
                    break;
                default:
                    throw new InvalidOperationException("ID3v2 Version " + (object)this._version + " is not supported.");
            }
        }
    }
}
