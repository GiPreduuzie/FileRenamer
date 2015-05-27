using System;
using System.Text;

namespace DecompiledMp3Lib.Mp3Lib
{
    internal class AudioFrameXingHeader : AudioFrame
    {
        private static uint[] _offsetFrames = new uint[16]
    {
      0U,
      8U,
      0U,
      8U,
      0U,
      8U,
      0U,
      8U,
      0U,
      8U,
      0U,
      8U,
      0U,
      8U,
      0U,
      8U
    };
        private static uint[] _offsetBytes = new uint[16]
    {
      0U,
      0U,
      8U,
      12U,
      0U,
      0U,
      8U,
      12U,
      0U,
      0U,
      8U,
      12U,
      0U,
      0U,
      8U,
      12U
    };
        private static uint[] _offsetToc = new uint[16]
    {
      0U,
      0U,
      0U,
      0U,
      8U,
      12U,
      12U,
      16U,
      0U,
      0U,
      0U,
      0U,
      8U,
      12U,
      12U,
      16U
    };
        private static uint[] _offsetQuality = new uint[16]
    {
      0U,
      0U,
      0U,
      0U,
      0U,
      0U,
      0U,
      0U,
      8U,
      12U,
      12U,
      16U,
      108U,
      112U,
      112U,
      116U
    };

        private string Tag
        {
            get
            {
                return Encoding.ASCII.GetString(this._frameBuffer, (int)this._headerBytes, 4);
            }
        }

        private AudioFrameXingHeader.FieldsCode Fields
        {
            get
            {
                return (AudioFrameXingHeader.FieldsCode)AudioFrame.ParseBigEndianDWORD(this._frameBuffer, this._headerBytes + 4U);
            }
        }

        private uint? XingNumFileFrames
        {
            get
            {
                uint num = AudioFrameXingHeader._offsetFrames[(int)this.Fields];
                if (num > 0U)
                    return new uint?(AudioFrame.ParseBigEndianDWORD(this._frameBuffer, this._headerBytes + num));
                return new uint?();
            }
        }

        private uint? XingNumFileBytes
        {
            get
            {
                uint num = AudioFrameXingHeader._offsetBytes[(int)this.Fields];
                if (num > 0U)
                    return new uint?(AudioFrame.ParseBigEndianDWORD(this._frameBuffer, this._headerBytes + num));
                return new uint?();
            }
        }

        private uint TocOffset
        {
            get
            {
                return AudioFrameXingHeader._offsetToc[(int)this.Fields];
            }
        }

        private uint? QualityIndicator
        {
            get
            {
                uint num = AudioFrameXingHeader._offsetQuality[(int)this.Fields];
                if (num > 0U)
                    return new uint?(AudioFrame.ParseBigEndianDWORD(this._frameBuffer, this._headerBytes + num));
                return new uint?();
            }
        }

        public override string DebugString
        {
            get
            {
                string str = base.DebugString + "\n----Xing Header----";
                uint? numPayloadFrames = this.NumPayloadFrames;
                if (numPayloadFrames.HasValue)
                    str += string.Format("\n  {0} Frames", (object)numPayloadFrames.Value);
                uint? numPayloadBytes = this.NumPayloadBytes;
                if (numPayloadBytes.HasValue)
                    str += string.Format(", {0} Bytes", (object)numPayloadBytes.Value);
                uint? qualityIndicator = this.QualityIndicator;
                if (qualityIndicator.HasValue)
                    str += string.Format("\n  Quality: {0}", (object)qualityIndicator.Value);
                if (this.IsVbr.HasValue)
                    str += string.Format(", {0}", this.IsVbr.Value ? (object)"VBR" : (object)"CBR");
                return str;
            }
        }

        public override bool? IsVbr
        {
            get
            {
                string tag = this.Tag;
                if (tag == "Xing")
                    return new bool?(true);
                if (tag == "Info")
                    return new bool?(false);
                throw new InvalidAudioFrameException("Xing/Info tag not recognised");
            }
        }

        public override uint? NumPayloadBytes
        {
            get
            {
                return this.XingNumFileBytes;
            }
        }

        public override uint? NumAudioBytes
        {
            get
            {
                uint? xingNumFileBytes = this.XingNumFileBytes;
                if (!xingNumFileBytes.HasValue)
                    return new uint?();
                uint num = xingNumFileBytes.Value;
                uint? frameLengthInBytes = this.Header.FrameLengthInBytes;
                if (!frameLengthInBytes.HasValue)
                    return new uint?();
                return new uint?(num - frameLengthInBytes.GetValueOrDefault());
            }
        }

        public override uint? NumPayloadFrames
        {
            get
            {
                return this.XingNumFileFrames;
            }
        }

        public override uint? NumAudioFrames
        {
            get
            {
                uint? xingNumFileFrames = this.XingNumFileFrames;
                if (xingNumFileFrames.HasValue)
                    return new uint?(xingNumFileFrames.Value);
                return new uint?();
            }
        }

        public override double? Duration
        {
            get
            {
                uint? xingNumFileFrames = this.XingNumFileFrames;
                if (xingNumFileFrames.HasValue)
                    return new double?((double)(xingNumFileFrames.Value - 1U) * this.Header.SecondsPerFrame);
                return base.Duration;
            }
        }

        public override double? BitRateVbr
        {
            get
            {
                uint? numAudioFrames = this.NumAudioFrames;
                if (!numAudioFrames.HasValue)
                    return new double?();
                uint? numAudioBytes = this.NumAudioBytes;
                if (!numAudioBytes.HasValue)
                    return new double?();
                double num = (double)numAudioFrames.Value * this.Header.SecondsPerFrame;
                return new double?((double)numAudioBytes.Value / num * 8.0);
            }
        }

        public AudioFrameXingHeader(AudioFrame baseclass)
            : base(baseclass)
        {
        }

        [Flags]
        private enum FieldsCode
        {
            Frames = 1,
            Bytes = 2,
            Toc = 4,
            Quality = 8,
        }
    }
}
