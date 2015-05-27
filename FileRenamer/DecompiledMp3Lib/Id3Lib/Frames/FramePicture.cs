using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using DecompiledMp3Lib.Mp3Lib;

namespace DecompiledMp3Lib.Id3Lib.Frames
{
    [Frame("APIC")]
    public class FramePicture : FrameBase, IFrameDescription
    {
        private TextCode _textEncoding;
        private string _mime;
        private PictureTypeCode _pictureType;
        private string _description;
        private byte[] _pictureData;

        public TextCode TextEncoding
        {
            get
            {
                return this._textEncoding;
            }
            set
            {
                this._textEncoding = value;
            }
        }

        public string Mime
        {
            get
            {
                return this._mime;
            }
            set
            {
                this._mime = value;
            }
        }

        public PictureTypeCode PictureType
        {
            get
            {
                return this._pictureType;
            }
            set
            {
                this._pictureType = value;
            }
        }

        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }

        public byte[] PictureData
        {
            get
            {
                return this._pictureData;
            }
            set
            {
                this._pictureData = value;
            }
        }

        public System.Drawing.Image Picture
        {
            get
            {
                return Image.FromStream((Stream)new MemoryStream(this._pictureData, false));
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                MemoryStream memoryStream = new MemoryStream();
                value.Save((Stream)memoryStream, value.RawFormat);
                this._pictureData = memoryStream.ToArray();
                this._mime = FramePicture.GetMimeType(value);
            }
        }

        public FramePicture(string frameId)
            : base(frameId)
        {
            this._textEncoding = TextCode.Ascii;
        }

        public static string GetMimeType(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");
            foreach (ImageCodecInfo imageCodecInfo in ImageCodecInfo.GetImageDecoders())
            {
                if (imageCodecInfo.FormatID == image.RawFormat.Guid)
                    return imageCodecInfo.MimeType;
            }
            return "image/unknown";
        }

        public override void Parse(byte[] frame)
        {
            int index1 = 0;
            this._textEncoding = (TextCode)frame[index1];
            int index2 = index1 + 1;
            this._mime = TextBuilder.ReadASCII(frame, ref index2);
            this._pictureType = (PictureTypeCode)frame[index2];
            int index3 = index2 + 1;
            this._description = TextBuilder.ReadText(frame, ref index3, this._textEncoding);
            this._pictureData = Memory.Extract(frame, index3, frame.Length - index3);
        }

        public override byte[] Make()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter((Stream)memoryStream);
            binaryWriter.Write((byte)this._textEncoding);
            binaryWriter.Write(TextBuilder.WriteASCII(this._mime));
            binaryWriter.Write((byte)this._pictureType);
            binaryWriter.Write(TextBuilder.WriteText(this._description, this._textEncoding));
            binaryWriter.Write(this._pictureData);
            return memoryStream.ToArray();
        }

        public override string ToString()
        {
            return this._description;
        }
    }
}