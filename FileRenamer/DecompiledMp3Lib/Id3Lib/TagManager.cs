using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using DecompiledMp3Lib.Id3Lib.Exceptions;
using DecompiledMp3Lib.Id3Lib.Frames;

namespace DecompiledMp3Lib.Id3Lib
{
    public static class TagManager
    {
        public static TagModel Deserialize(Stream stream)
        {
            TagModel tagModel = new TagModel();
            tagModel.Header.Deserialize(stream);
            if ((int)tagModel.Header.Version != 3 & (int)tagModel.Header.Version != 4)
                throw new NotImplementedException("ID3v2 Version " + (object)tagModel.Header.Version + " is not supported.");
            uint tagSize = tagModel.Header.TagSize;
            if (tagModel.Header.Unsync)
            {
                MemoryStream memoryStream = new MemoryStream();
                tagSize -= Sync.Unsafe(stream, (Stream)memoryStream, tagSize);
                stream = (Stream)memoryStream;
                if (tagSize <= 0U)
                    throw new InvalidTagException("Data is missing after the header.");
            }
            uint num1;
            if (tagModel.Header.ExtendedHeader)
            {
                tagModel.ExtendedHeader.Deserialize(stream);
                num1 = tagSize - tagModel.ExtendedHeader.Size;
                if (tagSize <= 0U)
                    throw new InvalidTagException("Data is missing after the extended header.");
            }
            else
                num1 = tagSize;
            if (num1 <= 0U)
                throw new InvalidTagException("No frames are present in the Tag, there must be at least one present.");
            uint num2 = 0U;
            FrameHelper frameHelper = new FrameHelper(tagModel.Header);
            while (num2 < num1)
            {
                byte[] numArray = new byte[4];
                stream.Read(numArray, 0, 1);
                if ((int)numArray[0] == 0)
                {
                    tagModel.Header.PaddingSize = num1 - num2;
                    stream.Seek((long)(tagModel.Header.PaddingSize - 1U), SeekOrigin.Current);
                    break;
                }
                if (num2 + 10U > num1)
                    throw new InvalidTagException("Tag is corrupt, must be formed of complete frames.");
                stream.Read(numArray, 1, 3);
                uint num3 = num2 + 4U;
                BinaryReader binaryReader = new BinaryReader(stream);
                uint val = Swap.UInt32(binaryReader.ReadUInt32());
                uint num4 = num3 + 4U;
                if ((int)tagModel.Header.Version == 4)
                    val = Sync.Unsafe(val);
                if (val > num1 - num4)
                    throw new InvalidFrameException("A frame is corrupt, it can't be larger than the available space remaining.");
                ushort flags = Swap.UInt16(binaryReader.ReadUInt16());
                uint num5 = num4 + 2U;
                //byte[] buffer = new byte[(IntPtr)val];
                byte[] buffer = new byte[val];
                binaryReader.Read(buffer, 0, (int)val);
                num2 = num5 + val;
                tagModel.Add(frameHelper.Build(Encoding.UTF8.GetString(numArray, 0, 4), flags, buffer));
            }
            return tagModel;
        }

        public static void Serialize(TagModel frameModel, Stream stream)
        {
            if (frameModel.Count <= 0)
                throw new InvalidTagException("Can't serialize a ID3v2 tag without any frames, there must be at least one present.");
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter((Stream)memoryStream);
            FrameHelper frameHelper = new FrameHelper(frameModel.Header);
            foreach (FrameBase frame in (Collection<FrameBase>)frameModel)
            {
                byte[] numArray = new byte[4];
                Encoding.UTF8.GetBytes(frame.FrameId, 0, 4, numArray, 0);
                binaryWriter.Write(numArray);
                ushort flags;
                byte[] buffer = frameHelper.Make(frame, out flags);
                uint val = (uint)buffer.Length;
                if ((int)frameModel.Header.Version == 4)
                    val = Sync.Safe(val);
                binaryWriter.Write(Swap.UInt32(val));
                binaryWriter.Write(Swap.UInt16(flags));
                binaryWriter.Write(buffer);
            }
            uint count = (uint)memoryStream.Position;
            stream.Seek(10L, SeekOrigin.Begin);
            if (frameModel.Header.Unsync)
                count += Sync.Safe((Stream)memoryStream, stream, count);
            else
                memoryStream.WriteTo(stream);
            frameModel.Header.TagSize = count;
            if (frameModel.Header.Padding)
            {
                for (int index = 0; (long)index < (long)frameModel.Header.PaddingSize; ++index)
                    stream.WriteByte((byte)0);
            }
            if (frameModel.Header.Footer)
                frameModel.Header.SerializeFooter(stream);
            long position = stream.Position;
            stream.Seek(0L, SeekOrigin.Begin);
            frameModel.Header.Serialize(stream);
            stream.Position = position;
        }
    }
}
