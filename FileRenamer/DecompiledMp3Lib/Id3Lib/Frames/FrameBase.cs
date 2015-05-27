using System;
using DecompiledMp3Lib.Id3Lib.Exceptions;

namespace DecompiledMp3Lib.Id3Lib.Frames
{
    public abstract class FrameBase
    {
        private string _frameId;
        private byte? _group;

        public bool TagAlter { get; set; }

        public bool FileAlter { get; set; }

        public bool ReadOnly { get; set; }

        public bool Compression { get; set; }

        public bool Encryption { get; set; }

        public bool Unsynchronisation { get; set; }

        public bool DataLength { get; set; }

        public byte? Group
        {
            get
            {
                return this._group;
            }
            set
            {
                this._group = value;
            }
        }

        public string FrameId
        {
            get
            {
                return this._frameId;
            }
        }

        internal FrameBase(string frameId)
        {
            if (frameId == null)
                throw new ArgumentNullException("frameId");
            if (frameId.Length != 4)
                throw new InvalidTagException("Invalid frame type: '" + frameId + "', it must be 4 characters long.");
            this._frameId = frameId;
        }

        public abstract void Parse(byte[] frame);

        public abstract byte[] Make();
    }
}
