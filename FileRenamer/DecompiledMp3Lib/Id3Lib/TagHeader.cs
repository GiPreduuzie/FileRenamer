using System.IO;
using DecompiledMp3Lib.Id3Lib.Exceptions;
using DecompiledMp3Lib.Mp3Lib;

namespace DecompiledMp3Lib.Id3Lib
{
    public class TagHeader
    {
        private static readonly byte[] _id3 = new byte[3]
    {
      (byte) 73,
      (byte) 68,
      (byte) 51
    };
        private static readonly byte[] _3di = new byte[3]
    {
      (byte) 51,
      (byte) 68,
      (byte) 73
    };
        private byte _id3Version = (byte)4;
        private byte _id3Revision;
        private byte _id3Flags;
        private uint _id3RawSize;
        private uint _paddingSize;

        public static uint HeaderSize
        {
            get
            {
                return 10U;
            }
        }

        public byte Version
        {
            get
            {
                return this._id3Version;
            }
            set
            {
                this._id3Version = value;
            }
        }

        public byte Revision
        {
            get
            {
                return this._id3Revision;
            }
            set
            {
                this._id3Revision = value;
            }
        }

        public uint TagSize
        {
            get
            {
                return this._id3RawSize;
            }
            set
            {
                this._id3RawSize = value;
            }
        }

        public uint TagSizeWithHeaderFooter
        {
            get
            {
                return (uint)((int)this._id3RawSize + (int)TagHeader.HeaderSize + (this.Footer ? (int)TagHeader.HeaderSize : 0));
            }
        }

        public bool Unsync
        {
            get
            {
                return ((int)this._id3Flags & 128) > 0;
            }
            set
            {
                if (value)
                    // ?? (sbyte.MinValue)
                    this._id3Flags |= (byte)byte.MinValue;
                else
                    this._id3Flags &= (byte)127;
            }
        }

        public bool ExtendedHeader
        {
            get
            {
                return ((int)this._id3Flags & 64) > 0;
            }
            set
            {
                if (value)
                    this._id3Flags |= (byte)64;
                else
                    this._id3Flags &= (byte)191;
            }
        }

        public bool Experimental
        {
            get
            {
                return ((int)this._id3Flags & 32) > 0;
            }
            set
            {
                if (value)
                    this._id3Flags |= (byte)32;
                else
                    this._id3Flags &= (byte)223;
            }
        }

        public bool Footer
        {
            get
            {
                return ((int)this._id3Flags & 16) > 0;
            }
            set
            {
                if (value)
                {
                    this._id3Flags |= (byte)16;
                    this._paddingSize = 0U;
                }
                else
                    this._id3Flags &= (byte)239;
            }
        }

        public bool Padding
        {
            get
            {
                return this._paddingSize > 0U;
            }
        }

        public uint PaddingSize
        {
            get
            {
                return this._paddingSize;
            }
            set
            {
                if (value > 0U)
                    this.Footer = false;
                this._paddingSize = value;
            }
        }

        public void Serialize(Stream stream)
        {
            BinaryWriter binaryWriter = new BinaryWriter(stream);
            binaryWriter.Write(TagHeader._id3);
            binaryWriter.Write(this._id3Version);
            binaryWriter.Write(this._id3Revision);
            binaryWriter.Write(this._id3Flags);
            binaryWriter.Write(Swap.UInt32(Sync.Safe(this._id3RawSize + this._paddingSize)));
        }

        public void SerializeFooter(Stream stream)
        {
            BinaryWriter binaryWriter = new BinaryWriter(stream);
            binaryWriter.Write(TagHeader._3di);
            binaryWriter.Write(this._id3Version);
            binaryWriter.Write(this._id3Revision);
            binaryWriter.Write(this._id3Flags);
            binaryWriter.Write(Swap.UInt32(Sync.Safe(this._id3RawSize)));
        }

        public void Deserialize(Stream stream)
        {
            BinaryReader binaryReader = new BinaryReader(stream);
            byte[] numArray = new byte[3];
            binaryReader.Read(numArray, 0, 3);
            if (!Memory.Compare(TagHeader._id3, numArray))
                throw new TagNotFoundException("ID3v2 tag identifier was not found");
            this._id3Version = binaryReader.ReadByte();
            if ((int)this._id3Version == (int)byte.MaxValue)
                throw new InvalidTagException("Corrupt header, invalid ID3v2 version.");
            this._id3Revision = binaryReader.ReadByte();
            if ((int)this._id3Revision == (int)byte.MaxValue)
                throw new InvalidTagException("Corrupt header, invalid ID3v2 revision.");
            this._id3Flags = (byte)(240U & (uint)binaryReader.ReadByte());
            this._id3RawSize = Swap.UInt32(Sync.UnsafeBigEndian(binaryReader.ReadUInt32()));
            if ((int)this._id3RawSize == 0)
                throw new InvalidTagException("Corrupt header, tag size can't be zero.");
        }
    }
}
