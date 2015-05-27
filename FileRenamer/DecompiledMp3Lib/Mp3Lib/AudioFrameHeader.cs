namespace DecompiledMp3Lib.Mp3Lib
{
    public class AudioFrameHeader
    {
        private static uint[] V1L1 = new uint[16]
    {
      0U,
      32U,
      64U,
      96U,
      128U,
      160U,
      192U,
      224U,
      256U,
      288U,
      320U,
      352U,
      384U,
      416U,
      448U,
      uint.MaxValue
    };
        private static uint[] V1L2 = new uint[16]
    {
      0U,
      32U,
      48U,
      56U,
      64U,
      80U,
      96U,
      112U,
      128U,
      160U,
      192U,
      224U,
      256U,
      320U,
      384U,
      uint.MaxValue
    };
        private static uint[] V1L3 = new uint[16]
    {
      0U,
      32U,
      40U,
      48U,
      56U,
      64U,
      80U,
      96U,
      112U,
      128U,
      160U,
      192U,
      224U,
      256U,
      320U,
      uint.MaxValue
    };
        private static uint[] V2L1 = new uint[16]
    {
      0U,
      32U,
      48U,
      56U,
      64U,
      80U,
      96U,
      112U,
      128U,
      144U,
      160U,
      176U,
      192U,
      224U,
      256U,
      uint.MaxValue
    };
        private static uint[] V2L2L3 = new uint[16]
    {
      0U,
      8U,
      16U,
      24U,
      32U,
      40U,
      48U,
      56U,
      64U,
      80U,
      96U,
      112U,
      128U,
      144U,
      160U,
      uint.MaxValue
    };
        private static uint[] MPEG1 = new uint[4]
    {
      44100U,
      48000U,
      32000U,
      uint.MaxValue
    };
        private static uint[] MPEG2 = new uint[4]
    {
      22050U,
      24000U,
      16000U,
      uint.MaxValue
    };
        private static uint[] MPEG25 = new uint[4]
    {
      11025U,
      12000U,
      8000U,
      uint.MaxValue
    };
        private static uint[] V1Size = new uint[4]
    {
      uint.MaxValue,
      1152U,
      1152U,
      384U
    };
        private static uint[] V2Size = new uint[4]
    {
      uint.MaxValue,
      576U,
      1152U,
      384U
    };
        private static string[] MPEGChannelMode = new string[4]
    {
      "Stereo",
      "Joint Stereo",
      "Dual channel (2 mono channels)",
      "Single channel (Mono)"
    };
        private static string[] MPEGLayer = new string[4]
    {
      "Reserved",
      "Layer III",
      "Layer II",
      "Layer I"
    };
        private static string[] MPEGEmphasis = new string[4]
    {
      "None",
      "50/15 ms",
      "Reserved",
      "CCIT J.17"
    };
        protected byte[] _headerBuffer;

        private byte RawMpegVersion
        {
            get
            {
                return (byte)(((int)this._headerBuffer[1] & 24) >> 3);
            }
        }

        private byte RawLayer
        {
            get
            {
                return (byte)(((int)this._headerBuffer[1] & 6) >> 1);
            }
        }

        private bool RawProtection
        {
            get
            {
                return ((int)this._headerBuffer[1] & 1) > 0;
            }
        }

        private byte RawBitRate
        {
            get
            {
                return (byte)(((int)this._headerBuffer[2] & 240) >> 4);
            }
        }

        private byte RawSampleFreq
        {
            get
            {
                return (byte)(((int)this._headerBuffer[2] & 12) >> 2);
            }
        }

        private bool RawPadding
        {
            get
            {
                return ((int)this._headerBuffer[2] & 2) > 0;
            }
        }

        private bool RawPrivate
        {
            get
            {
                return ((int)this._headerBuffer[2] & 1) > 0;
            }
        }

        private byte RawChannelMode
        {
            get
            {
                return (byte)(((int)this._headerBuffer[3] & 192) >> 6);
            }
        }

        private byte RawModeExtension
        {
            get
            {
                return (byte)(((int)this._headerBuffer[3] & 48) >> 4);
            }
        }

        private bool RawCopyright
        {
            get
            {
                return ((int)this._headerBuffer[3] & 8) > 0;
            }
        }

        private bool RawOriginal
        {
            get
            {
                return ((int)this._headerBuffer[3] & 4) > 0;
            }
        }

        private byte RawEmphasis
        {
            get
            {
                return (byte)((uint)this._headerBuffer[3] & 3U);
            }
        }

        public bool Valid
        {
            get
            {
                if ((int)this.RawMpegVersion != 1 && (int)this.RawLayer != 0 && ((int)this.RawBitRate != 15 && (int)this.RawSampleFreq != 3))
                    return (int)this.RawBitRate != 15;
                return false;
            }
        }

        public string VersionLayer
        {
            get
            {
                switch (this.RawMpegVersion)
                {
                    case (byte)0:
                        switch (this.RawLayer)
                        {
                            case (byte)1:
                                return "MPEG-2.5 layer 3";
                            case (byte)2:
                                return "MPEG-2.5 layer 2";
                            case (byte)3:
                                return "MPEG-2.5 layer 1";
                            default:
                                throw new InvalidAudioFrameException("MPEG Version 2.5 Layer not recognised");
                        }
                    case (byte)2:
                        switch (this.RawLayer)
                        {
                            case (byte)1:
                                return "MPEG-2 layer 3";
                            case (byte)2:
                                return "MPEG-2 layer 2";
                            case (byte)3:
                                return "MPEG-2 layer 1";
                            default:
                                throw new InvalidAudioFrameException("MPEG Version 2 (ISO/IEC 13818-3) Layer not recognised");
                        }
                    case (byte)3:
                        switch (this.RawLayer)
                        {
                            case (byte)1:
                                return "MPEG-1 layer 3";
                            case (byte)2:
                                return "MPEG-1 layer 2";
                            case (byte)3:
                                return "MPEG-1 layer 1";
                            default:
                                throw new InvalidAudioFrameException("MPEG Version 1 (ISO/IEC 11172-3) Layer not recognised");
                        }
                    default:
                        throw new InvalidAudioFrameException("MPEG Version not recognised");
                }
            }
        }

        public uint Layer
        {
            get
            {
                switch (this.RawLayer)
                {
                    case (byte)1:
                        return 3U;
                    case (byte)2:
                        return 2U;
                    case (byte)3:
                        return 1U;
                    default:
                        throw new InvalidAudioFrameException("MPEG Layer not recognised");
                }
            }
        }

        public uint? BitRate
        {
            get
            {
                uint num;
                switch (this.RawMpegVersion)
                {
                    case 0:
                        switch (this.RawLayer)
                        {
                            case (byte)1:
                                num = AudioFrameHeader.V2L2L3[(int)this.RawBitRate];
                                break;
                            case (byte)2:
                                num = AudioFrameHeader.V2L2L3[(int)this.RawBitRate];
                                break;
                            case (byte)3:
                                num = AudioFrameHeader.V2L1[(int)this.RawBitRate];
                                break;
                            default:
                                throw new InvalidAudioFrameException("MPEG Version 2.5 BitRate not recognised");
                        }
                        // ??
                        break;
                    case 2:
                        switch (this.RawLayer)
                        {
                            case (byte)1:
                                num = AudioFrameHeader.V2L2L3[(int)this.RawBitRate];
                                break;
                            case (byte)2:
                                num = AudioFrameHeader.V2L2L3[(int)this.RawBitRate];
                                break;
                            case (byte)3:
                                num = AudioFrameHeader.V2L1[(int)this.RawBitRate];
                                break;
                            default:
                                throw new InvalidAudioFrameException("MPEG Version 2 (ISO/IEC 13818-3) BitRate not recognised");
                        }
                        // ??
                        break;
                    case 3:
                        switch (this.RawLayer)
                        {
                            case (byte)1:
                                num = AudioFrameHeader.V1L3[(int)this.RawBitRate];
                                break;
                            case (byte)2:
                                num = AudioFrameHeader.V1L2[(int)this.RawBitRate];
                                break;
                            case (byte)3:
                                num = AudioFrameHeader.V1L1[(int)this.RawBitRate];
                                break;
                            default:
                                throw new InvalidAudioFrameException("MPEG Version 1 (ISO/IEC 11172-3) BitRate not recognised");
                        }
                        // ??
                        break;
                    default:
                        throw new InvalidAudioFrameException("MPEG Version not recognised");
                }
                if ((int)num == -1)
                    throw new InvalidAudioFrameException("MPEG BitRate not recognised");
                if ((int)num == 0)
                    return new uint?();
                return new uint?(num * 1000U);
            }
        }

        public uint SamplesPerSecond
        {
            get
            {
                uint num;
                switch (this.RawMpegVersion)
                {
                    case (byte)0:
                        num = AudioFrameHeader.MPEG25[(int)this.RawSampleFreq];
                        break;
                    case (byte)2:
                        num = AudioFrameHeader.MPEG2[(int)this.RawSampleFreq];
                        break;
                    case (byte)3:
                        num = AudioFrameHeader.MPEG1[(int)this.RawSampleFreq];
                        break;
                    default:
                        throw new InvalidAudioFrameException("MPEG Version not recognised");
                }
                if ((int)num == -1)
                    throw new InvalidAudioFrameException("MPEG SampleFreq not recognised");
                return num;
            }
        }

        public uint SamplesPerFrame
        {
            get
            {
                uint num;
                switch (this.RawMpegVersion)
                {
                    case (byte)0:
                    case (byte)2:
                        num = AudioFrameHeader.V2Size[(int)this.RawLayer];
                        break;
                    case (byte)3:
                        num = AudioFrameHeader.V1Size[(int)this.RawLayer];
                        break;
                    default:
                        throw new InvalidAudioFrameException("MPEG Version not recognised");
                }
                if ((int)num == -1)
                    throw new InvalidAudioFrameException("MPEG Layer not recognised");
                return num;
            }
        }

        public double SecondsPerFrame
        {
            get
            {
                return (double)this.SamplesPerFrame / (double)this.SamplesPerSecond;
            }
        }

        public bool IsFreeBitRate
        {
            get
            {
                return (int)this.RawBitRate == 0;
            }
        }

        public uint PaddingSize
        {
            get
            {
                if (!this.RawPadding)
                    return 0U;
                switch (this.RawMpegVersion)
                {
                    case (byte)0:
                    case (byte)2:
                        return 1U;
                    case (byte)3:
                        return 4U;
                    default:
                        throw new InvalidAudioFrameException("MPEG Version not recognised");
                }
            }
        }

        public uint? FrameLengthInBytes
        {
            get
            {
                uint? bitRate = this.BitRate;
                if (!bitRate.HasValue)
                    return new uint?();
                switch (this.RawMpegVersion)
                {
                    case (byte)0:
                    case (byte)2:
                        switch (this.RawLayer)
                        {
                            case (byte)1:
                                return new uint?(72U * bitRate.Value / this.SamplesPerSecond + (this.RawPadding ? 1U : 0U));
                            case (byte)2:
                                return new uint?(144U * bitRate.Value / this.SamplesPerSecond + (this.RawPadding ? 1U : 0U));
                            case (byte)3:
                                return new uint?((uint)(((int)(12U * bitRate.Value / this.SamplesPerSecond) + (this.RawPadding ? 1 : 0)) * 4));
                            default:
                                throw new InvalidAudioFrameException("MPEG 2 Layer not recognised");
                        }
                    case (byte)3:
                        switch (this.RawLayer)
                        {
                            case (byte)1:
                                return new uint?(144U * bitRate.Value / this.SamplesPerSecond + (this.RawPadding ? 1U : 0U));
                            case (byte)2:
                                return new uint?(144U * bitRate.Value / this.SamplesPerSecond + (this.RawPadding ? 1U : 0U));
                            case (byte)3:
                                return new uint?((uint)(((int)(12U * bitRate.Value / this.SamplesPerSecond) + (this.RawPadding ? 1 : 0)) * 4));
                            default:
                                throw new InvalidAudioFrameException("MPEG 1 Layer not recognised");
                        }
                    default:
                        throw new InvalidAudioFrameException("MPEG Version not recognised");
                }
            }
        }

        public double? IdealisedFrameLengthInBytes
        {
            get
            {
                uint? bitRate = this.BitRate;
                if (!bitRate.HasValue)
                    return new double?();
                switch (this.RawMpegVersion)
                {
                    case (byte)0:
                    case (byte)2:
                        switch (this.RawLayer)
                        {
                            case (byte)1:
                                return new double?(72.0 * (double)bitRate.Value / (double)this.SamplesPerSecond);
                            case (byte)2:
                                return new double?(144.0 * (double)bitRate.Value / (double)this.SamplesPerSecond);
                            case (byte)3:
                                return new double?(12.0 * (double)bitRate.Value / (double)this.SamplesPerSecond * 4.0);
                            default:
                                throw new InvalidAudioFrameException("MPEG 2 Layer not recognised");
                        }
                    case (byte)3:
                        switch (this.RawLayer)
                        {
                            case (byte)1:
                                return new double?(144.0 * (double)bitRate.Value / (double)this.SamplesPerSecond);
                            case (byte)2:
                                return new double?(144.0 * (double)bitRate.Value / (double)this.SamplesPerSecond);
                            case (byte)3:
                                return new double?(12.0 * (double)bitRate.Value / (double)this.SamplesPerSecond * 4.0);
                            default:
                                throw new InvalidAudioFrameException("MPEG 1 Layer not recognised");
                        }
                    default:
                        throw new InvalidAudioFrameException("MPEG Version not recognised");
                }
            }
        }

        public uint ChecksumSize
        {
            get
            {
                return !this.RawProtection ? 2U : 0U;
            }
        }

        public uint SideInfoSize
        {
            get
            {
                switch (this.RawMpegVersion)
                {
                    case (byte)0:
                    case (byte)2:
                        return this.IsMono ? 9U : 17U;
                    case (byte)3:
                        return this.IsMono ? 17U : 32U;
                    default:
                        throw new InvalidAudioFrameException("MPEG Version not recognised");
                }
            }
        }

        public uint HeaderSize
        {
            get
            {
                return 4U + this.ChecksumSize + this.SideInfoSize;
            }
        }

        public bool CRCs
        {
            get
            {
                return !this.RawProtection;
            }
        }

        public bool Copyright
        {
            get
            {
                return this.RawCopyright;
            }
        }

        public bool Original
        {
            get
            {
                return this.RawOriginal;
            }
        }

        public bool Private
        {
            get
            {
                return this.RawPrivate;
            }
        }

        public bool IsMono
        {
            get
            {
                return (int)this.RawChannelMode == 3;
            }
        }

        public AudioFrameHeader.ChannelModeCode ChannelMode
        {
            get
            {
                return (AudioFrameHeader.ChannelModeCode)this.RawChannelMode;
            }
        }

        public AudioFrameHeader.EmphasisCode Emphasis
        {
            get
            {
                return (AudioFrameHeader.EmphasisCode)this.RawEmphasis;
            }
        }

        public virtual string DebugString
        {
            get
            {
                return string.Format("----MP3 header----\n  {0}\n  {1}Hz {2}\n  Frame: {3} bytes\n  CRCs: {4}, Copyrighted: {5}\n  Original: {6}, Emphasis: {7}", (object)this.VersionLayer, (object)this.SamplesPerSecond, (object)AudioFrameHeader.MPEGChannelMode[(int)this.ChannelMode], (object)this.FrameLengthInBytes, this.CRCs ? (object)"No" : (object)"Yes", this.Copyright ? (object)"No" : (object)"Yes", this.Original ? (object)"No" : (object)"Yes", (object)AudioFrameHeader.MPEGEmphasis[(int)this.Emphasis]);
            }
        }

        public AudioFrameHeader(byte[] frameHeader)
        {
            this._headerBuffer = frameHeader;
        }

        private bool IsCompatible(AudioFrameHeader destHeader)
        {
            return (int)destHeader.RawMpegVersion == (int)this.RawMpegVersion && (int)destHeader.RawLayer == (int)this.RawLayer && ((int)destHeader.RawSampleFreq == (int)this.RawSampleFreq && destHeader.IsMono == this.IsMono) && (int)destHeader.RawEmphasis == (int)this.RawEmphasis;
        }

        public enum ChannelModeCode
        {
            Stereo,
            JointStereo,
            DualMono,
            Mono,
        }

        public enum EmphasisCode
        {
            None,
            E5015,
            Reserved,
            CCIT,
        }
    }
}
