namespace DecompiledMp3Lib.Mp3Lib
{
    internal class AudioFrameVbriHeader : AudioFrame
    {
        private ushort Version
        {
            get
            {
                return AudioFrame.ParseBigEndianWORD(this._frameBuffer, this._headerBytes + 4U);
            }
        }

        private ushort Delay
        {
            get
            {
                return AudioFrame.ParseBigEndianWORD(this._frameBuffer, this._headerBytes + 6U);
            }
        }

        private ushort QualityIndicator
        {
            get
            {
                return AudioFrame.ParseBigEndianWORD(this._frameBuffer, this._headerBytes + 8U);
            }
        }

        private uint VbriNumStreamBytes
        {
            get
            {
                return AudioFrame.ParseBigEndianDWORD(this._frameBuffer, this._headerBytes + 10U);
            }
        }

        private uint VbriNumStreamFrames
        {
            get
            {
                return AudioFrame.ParseBigEndianDWORD(this._frameBuffer, this._headerBytes + 14U);
            }
        }

        private ushort NumTocEntries
        {
            get
            {
                return AudioFrame.ParseBigEndianWORD(this._frameBuffer, this._headerBytes + 18U);
            }
        }

        private ushort ScaleTocEntries
        {
            get
            {
                return AudioFrame.ParseBigEndianWORD(this._frameBuffer, this._headerBytes + 20U);
            }
        }

        private ushort SizePerTocEntry
        {
            get
            {
                return AudioFrame.ParseBigEndianWORD(this._frameBuffer, this._headerBytes + 22U);
            }
        }

        private ushort FramesPerTocEntry
        {
            get
            {
                return AudioFrame.ParseBigEndianWORD(this._frameBuffer, this._headerBytes + 24U);
            }
        }

        public override string DebugString
        {
            get
            {
                int num1 = (int)this.NumTocEntries * (int)this.SizePerTocEntry;
                int num2 = (int)this._headerBytes + 26 + num1;
                int num3 = (int)this.Header.FrameLengthInBytes.Value;
                return string.Format("{0}\n----VBRI Header----\n  {1} Frames, {2} Bytes\n  Quality: {3}, TocBytes: {4}", (object)base.DebugString, (object)this.VbriNumStreamFrames, (object)this.NumPayloadBytes, (object)this.QualityIndicator, (object)num1);
            }
        }

        public override bool? IsVbr
        {
            get
            {
                return new bool?(true);
            }
        }

        public override uint? NumPayloadBytes
        {
            get
            {
                return new uint?(this.VbriNumStreamBytes);
            }
        }

        public override uint? NumAudioBytes
        {
            get
            {
                uint vbriNumStreamBytes = this.VbriNumStreamBytes;
                uint? frameLengthInBytes = this.Header.FrameLengthInBytes;
                if (!frameLengthInBytes.HasValue)
                    return new uint?();
                return new uint?(vbriNumStreamBytes - frameLengthInBytes.GetValueOrDefault());
            }
        }

        public override uint? NumPayloadFrames
        {
            get
            {
                return new uint?(this.VbriNumStreamFrames);
            }
        }

        public override uint? NumAudioFrames
        {
            get
            {
                uint? numPayloadFrames = this.NumPayloadFrames;
                if (!numPayloadFrames.HasValue)
                    return new uint?();
                return new uint?(numPayloadFrames.GetValueOrDefault() - 1U);
            }
        }

        public override double? Duration
        {
            get
            {
                uint? numAudioFrames = this.NumAudioFrames;
                double secondsPerFrame = this.Header.SecondsPerFrame;
                if (!numAudioFrames.HasValue)
                    return new double?();
                return new double?((double)numAudioFrames.GetValueOrDefault() * secondsPerFrame);
            }
        }

        public override double? BitRateVbr
        {
            get
            {
                double num1 = (double)(this.VbriNumStreamFrames - 1U) * this.Header.SecondsPerFrame;
                uint vbriNumStreamBytes = this.VbriNumStreamBytes;
                uint? frameLengthInBytes = this.Header.FrameLengthInBytes;
                uint? nullable1 = frameLengthInBytes.HasValue ? new uint?(vbriNumStreamBytes - frameLengthInBytes.GetValueOrDefault()) : new uint?();
                double num2 = num1;
                double? nullable2 = nullable1.HasValue ? new double?((double)nullable1.GetValueOrDefault() / num2) : new double?();
                if (!nullable2.HasValue)
                    return new double?();
                return new double?(nullable2.GetValueOrDefault() * 8.0);
            }
        }

        public AudioFrameVbriHeader(AudioFrame baseclass)
            : base(baseclass)
        {
        }
    }
}
