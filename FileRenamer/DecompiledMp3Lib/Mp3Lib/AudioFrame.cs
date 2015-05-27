using System.IO;
using System.Text;

namespace DecompiledMp3Lib.Mp3Lib
{
    public class AudioFrame
    {
        protected byte[] _frameBuffer;
        protected AudioFrameHeader _header;
        protected uint _headerBytes;

        public AudioFrameHeader Header
        {
            get
            {
                return this._header;
            }
        }

        public uint FrameLengthInBytes
        {
            get
            {
                return (uint)this._frameBuffer.Length;
            }
        }

        public virtual string DebugString
        {
            get
            {
                return string.Format("{0}\n----AudioFrame----\n  Audio: {1} frames, {2} bytes\n  Length: {3:N3} seconds", (object)this._header.DebugString, (object)this.NumAudioFrames, (object)this.NumAudioBytes, (object)this.Duration);
            }
        }

        public virtual bool? IsVbr
        {
            get
            {
                if (!this.Header.BitRate.HasValue)
                    return new bool?(false);
                return new bool?();
            }
        }

        public virtual uint? NumPayloadBytes
        {
            get
            {
                return new uint?();
            }
        }

        public virtual uint? NumAudioBytes
        {
            get
            {
                return new uint?();
            }
        }

        public virtual uint? NumPayloadFrames
        {
            get
            {
                return new uint?();
            }
        }

        public virtual uint? NumAudioFrames
        {
            get
            {
                return new uint?();
            }
        }

        public virtual double? Duration
        {
            get
            {
                return new double?();
            }
        }

        public uint? BitRateMp3
        {
            get
            {
                return this.Header.BitRate;
            }
        }

        public virtual double? BitRateVbr
        {
            get
            {
                return new double?();
            }
        }

        public bool IsXingHeader
        {
            get
            {
                string @string = Encoding.ASCII.GetString(this._frameBuffer, (int)this._headerBytes, 4);
                if (!(@string == "Xing"))
                    return @string == "Info";
                return true;
            }
        }

        public bool IsLameHeader
        {
            get
            {
                return false;
            }
        }

        public bool IsVbriHeader
        {
            get
            {
                return Encoding.ASCII.GetString(this._frameBuffer, (int)this._headerBytes, 4) == "VBRI";
            }
        }

        public AudioFrame(byte[] frameBuffer)
        {
            this._header = new AudioFrameHeader(frameBuffer);
            this._headerBytes = this._header.HeaderSize;
            this._frameBuffer = frameBuffer;
        }

        public AudioFrame(Stream stream, AudioFrameHeader header, uint frameSize, uint remainingBytes)
        {
            this._header = header;
            //this._frameBuffer = new byte[(IntPtr)frameSize];
            this._frameBuffer = new byte[frameSize];
            this._headerBytes = this._header.HeaderSize;
            int num = stream.Read(this._frameBuffer, 0, (int)frameSize);
            if (num < (int)frameSize)
                throw new InvalidAudioFrameException(string.Format("MPEG Audio AudioFrame: only {0} bytes of frame found when {1} bytes declared", (object)num, (object)frameSize));
        }

        protected AudioFrame(AudioFrame other)
        {
            this._frameBuffer = other._frameBuffer;
            this._header = other._header;
            this._headerBytes = other._headerBytes;
        }

        protected static ushort ParseBigEndianWORD(byte[] buffer, uint index)
        {
            ushort num = (ushort)0;
            for (uint index1 = index; index1 < index + 2U; ++index1)
                //num = (ushort)(((uint)num << 8) + (uint)buffer[(IntPtr)index1]);
                num = (ushort)(((uint)num << 8) + (uint)buffer[index1]);
            return num;
        }

        protected static uint ParseBigEndianDWORD(byte[] buffer, uint index)
        {
            uint num = 0U;
            for (uint index1 = index; index1 < index + 4U; ++index1)
                //num = (num << 8) + (uint)buffer[(IntPtr)index1];
                num = (num << 8) + (uint)buffer[index1];
            return num;
        }
    }
}
