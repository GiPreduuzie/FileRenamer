using System;
using System.IO;
using System.Text;

namespace DecompiledMp3Lib.Id3Lib.Frames
{
    [Frame("COMM")]
    [Frame("USLT")]
    public class FrameFullText : FrameBase, IFrameDescription
    {
        private string _language = "eng";
        private string _contents;
        private string _text;
        private TextCode _textEncoding;

        public TextCode TextCode
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

        public string Description
        {
            get
            {
                return this._contents;
            }
            set
            {
                this._contents = value;
            }
        }

        public string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                this._text = value;
            }
        }

        public string Language
        {
            get
            {
                return this._language;
            }
            set
            {
                this._language = value;
            }
        }

        public FrameFullText(string frameId)
            : base(frameId)
        {
            this._textEncoding = TextCode.Ascii;
        }

        public override void Parse(byte[] frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");
            int index1 = 0;
            this._textEncoding = (TextCode)frame[index1];
            int index2 = index1 + 1;
            if (frame.Length - index2 < 3)
                return;
            this._language = Encoding.UTF8.GetString(frame, index2, 3);
            int index3 = index2 + 3;
            if (frame.Length - index3 < 1)
                return;
            this._contents = TextBuilder.ReadText(frame, ref index3, this._textEncoding);
            this._text = TextBuilder.ReadTextEnd(frame, index3, this._textEncoding);
        }

        public override byte[] Make()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter((Stream)memoryStream);
            binaryWriter.Write((byte)this._textEncoding);
            byte[] buffer = TextBuilder.WriteASCII(this._language);
            if (buffer.Length != 3)
                binaryWriter.Write(new byte[3]
        {
          (byte) 101,
          (byte) 110,
          (byte) 103
        });
            else
                binaryWriter.Write(buffer, 0, 3);
            binaryWriter.Write(TextBuilder.WriteText(this._contents, this._textEncoding));
            binaryWriter.Write(TextBuilder.WriteTextEnd(this._text, this._textEncoding));
            return memoryStream.ToArray();
        }

        public override string ToString()
        {
            return this._text;
        }
    }
}
