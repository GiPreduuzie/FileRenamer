using System;
using System.Diagnostics;
using System.IO;

namespace DecompiledMp3Lib.Mp3Lib
{
    internal abstract class Audio : IAudio
    {
        protected AudioFrame _firstFrame;
        protected TimeSpan? _id3DurationTag;
        private Audio.AudioStats _audioStats;
        private bool _hasInconsistencies;

        public AudioFrameHeader Header
        {
            get
            {
                return this._firstFrame.Header;
            }
        }

        public virtual string DebugString
        {
            get
            {
                string str = string.Format("{0}\n----Audio----\n  Payload: {1} frames in {2} bytes\n  Length: {3:N3} seconds\n  {4:N4} kbit", (object)this._firstFrame.DebugString, (object)this.NumPayloadFrames, (object)this.NumPayloadBytes, (object)this.Duration, (object)this.BitRate);
                if (this._audioStats._numFrames > 0U)
                    str += string.Format("\n  Counted: {0} frames, {1} bytes", (object)this._audioStats._numFrames, (object)this._audioStats._numBytes);
                return str;
            }
        }

        public virtual bool IsVbr
        {
            get
            {
                bool? isVbr = this._firstFrame.IsVbr;
                if (isVbr.HasValue)
                    return isVbr.Value;
                return false;
            }
        }

        public bool HasVbrHeader
        {
            get
            {
                return this._firstFrame.GetType() != typeof(AudioFrame);
            }
        }

        public virtual uint NumPayloadBytes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public uint NumAudioBytes
        {
            get
            {
                uint? numAudioBytes = this._firstFrame.NumAudioBytes;
                if (numAudioBytes.HasValue)
                    return numAudioBytes.Value;
                if (!this.HasVbrHeader)
                    return this.NumPayloadBytes;
                if (!this._firstFrame.Header.FrameLengthInBytes.HasValue)
                    throw new InvalidAudioFrameException("VBR files cannot be 'free' bitrate");
                return this.NumPayloadBytes - this._firstFrame.Header.FrameLengthInBytes.Value;
            }
        }

        public uint NumPayloadFrames
        {
            get
            {
                uint? numPayloadFrames = this._firstFrame.NumPayloadFrames;
                if (numPayloadFrames.HasValue)
                    return numPayloadFrames.Value;
                double? frameLengthInBytes = this.Header.IdealisedFrameLengthInBytes;
                if (frameLengthInBytes.HasValue)
                    return (uint)Math.Round((double)this.NumPayloadBytes / frameLengthInBytes.Value);
                return this.NumPayloadBytes / this._firstFrame.FrameLengthInBytes;
            }
        }

        public uint NumAudioFrames
        {
            get
            {
                if (this.HasVbrHeader)
                    return this.NumPayloadFrames - 1U;
                return this.NumPayloadFrames;
            }
        }

        public double Duration
        {
            get
            {
                double? duration = this._firstFrame.Duration;
                if (duration.HasValue)
                    return duration.Value;
                if (this._id3DurationTag.HasValue)
                    return this._id3DurationTag.Value.TotalSeconds;
                return (double)this.NumAudioFrames * this.Header.SecondsPerFrame;
            }
        }

        public double? BitRateCalc
        {
            get
            {
                if (!this._id3DurationTag.HasValue)
                    return new double?();
                return new double?((double)(this.NumPayloadBytes * 8U) / this._id3DurationTag.Value.TotalSeconds);
            }
        }

        public uint? BitRateMp3
        {
            get
            {
                return this.Header.BitRate;
            }
        }

        public double? BitRateVbr
        {
            get
            {
                return this._firstFrame.BitRateVbr;
            }
        }

        public double BitRate
        {
            get
            {
                return (double)this.NumAudioBytes / this.Duration * 8.0;
            }
        }

        public bool HasInconsistencies
        {
            get
            {
                return this._hasInconsistencies;
            }
        }

        public Exception ParsingError
        {
            get
            {
                uint? numPayloadBytes1 = this._firstFrame.NumPayloadBytes;
                if (numPayloadBytes1.HasValue)
                {
                    uint? nullable = numPayloadBytes1;
                    uint numPayloadBytes2 = this.NumPayloadBytes;
                    if (((int)nullable.GetValueOrDefault() != (int)numPayloadBytes2 ? 1 : (!nullable.HasValue ? 1 : 0)) != 0)
                        return (Exception)new InvalidVbrSizeException(this.NumPayloadBytes, numPayloadBytes1.Value);
                }
                return (Exception)null;
            }
        }

        public Audio(AudioFrame firstFrame, TimeSpan? id3DurationTag)
        {
            if (firstFrame == null)
                throw new InvalidAudioFrameException("MPEG Audio Frame not found");
            this._firstFrame = firstFrame;
            this._id3DurationTag = id3DurationTag;
        }

        protected void CheckConsistency()
        {
            Exception parsingError = this.ParsingError;
            if (parsingError == null)
                return;
            this._hasInconsistencies = true;
            Trace.WriteLine(parsingError.Message);
        }

        public virtual Stream OpenAudioStream()
        {
            throw new NotImplementedException();
        }

        public virtual uint CalculateAudioCRC32()
        {
            throw new NotImplementedException();
        }

        public virtual byte[] CalculateAudioSHA1()
        {
            throw new NotImplementedException();
        }

        public void ScanWholeFile()
        {
            this._audioStats._numFrames = 0U;
            this._audioStats._numBytes = 0U;
            using (Stream stream = this.OpenAudioStream())
            {
                uint num = (uint)stream.Position;
                try
                {
                    while (true)
                    {
                        uint remainingBytes = this.NumPayloadBytes - ((uint)stream.Position - num);
                        AudioFrame frame = AudioFrameFactory.CreateFrame(stream, remainingBytes);
                        if (frame != null)
                        {
                            ++this._audioStats._numFrames;
                            this._audioStats._numBytes += frame.FrameLengthInBytes;
                        }
                        else
                            break;
                    }
                }
                catch (Exception ex)
                {
                    this._hasInconsistencies = true;
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        private struct AudioStats
        {
            public uint _numFrames;
            public uint _numBytes;
        }
    }
}
