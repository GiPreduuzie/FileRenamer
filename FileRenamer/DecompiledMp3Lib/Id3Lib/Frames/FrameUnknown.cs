namespace DecompiledMp3Lib.Id3Lib.Frames
{
    public class FrameUnknown : FrameBase
    {
        private byte[] _data;

        internal FrameUnknown(string frameId)
            : base(frameId)
        {
        }

        public override void Parse(byte[] frame)
        {
            this._data = frame;
        }

        public override byte[] Make()
        {
            return this._data;
        }

        public override string ToString()
        {
            return "Unknown ID3 frameId";
        }
    }
}
