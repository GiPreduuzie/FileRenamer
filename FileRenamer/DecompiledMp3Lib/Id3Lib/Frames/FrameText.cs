using System.IO;

namespace DecompiledMp3Lib.Id3Lib.Frames
{
    [Frame("T")]
    public class FrameText : FrameBase
    {
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

        public FrameText(string frameId)
            : base(frameId)
        {
            this._textEncoding = TextCode.Ascii;
        }

        public override void Parse(byte[] frame)
        {
            int index1 = 0;
            this._textEncoding = (TextCode)frame[index1];
            int index2 = index1 + 1;
            this._text = TextBuilder.ReadTextEnd(frame, index2, this._textEncoding);
        }

        public override byte[] Make()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter((Stream)memoryStream);
            binaryWriter.Write((byte)this._textEncoding);
            binaryWriter.Write(TextBuilder.WriteTextEnd(this._text, this._textEncoding));
            return memoryStream.ToArray();
        }

        public override string ToString()
        {
            return this._text;
        }
    }
}
