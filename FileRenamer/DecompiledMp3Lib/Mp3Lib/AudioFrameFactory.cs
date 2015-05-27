using System.Diagnostics;
using System.IO;

namespace DecompiledMp3Lib.Mp3Lib
{
    internal static class AudioFrameFactory
    {
        public static AudioFrame CreateFrame(Stream stream, uint remainingBytes)
        {
            long position = stream.Position;
            AudioFrameHeader header = AudioFrameFactory.CreateHeader(stream, remainingBytes);
            if (header == null)
                return (AudioFrame)null;
            uint frameSize;
            if (header.IsFreeBitRate)
            {
                frameSize = (uint)(stream.Position - position) + AudioFrameFactory.GetNextFrameOffset(stream, remainingBytes - (uint)(stream.Position - position));
            }
            else
            {
                Trace.Assert(header.FrameLengthInBytes.HasValue);
                frameSize = header.FrameLengthInBytes.Value;
            }
            stream.Position = position;
            return AudioFrameFactory.CreateSpecialisedHeaderFrame(new AudioFrame(stream, header, frameSize, remainingBytes));
        }

        public static AudioFrame CreateFrame(byte[] sourceBuffer)
        {
            return AudioFrameFactory.CreateSpecialisedHeaderFrame(new AudioFrame(sourceBuffer));
        }

        private static uint GetNextFrameOffset(Stream stream, uint remainingBytes)
        {
            long position = stream.Position;
            AudioFrameHeader header = AudioFrameFactory.CreateHeader(stream, remainingBytes);
            if (header == null)
                return 0U;
            Trace.WriteLine(string.Format("next frame is {0} bytes further", (object)(stream.Position - position)));
            Trace.WriteLine(header.DebugString);
            return (uint)(stream.Position - position);
        }

        private static AudioFrame CreateSpecialisedHeaderFrame(AudioFrame firstTry)
        {
            if (firstTry.IsXingHeader)
                return (AudioFrame)new AudioFrameXingHeader(firstTry);
            if (firstTry.IsVbriHeader)
                return (AudioFrame)new AudioFrameVbriHeader(firstTry);
            return firstTry;
        }

        private static AudioFrameHeader CreateHeader(Stream stream, uint remainingBytes)
        {
            long position1 = stream.Position;
            int num1 = 0;
            if (remainingBytes > 65536U)
                remainingBytes = 65536U;
            long num2 = position1 + (long)remainingBytes;
            long position2;
            AudioFrameHeader audioFrameHeader;
            while (true)
            {
                long remainingBytes1 = num2 - (long)(uint)stream.Position;
                if (AudioFrameFactory.Seek(stream, remainingBytes1) >= 0)
                {
                    position2 = stream.Position;
                    audioFrameHeader = new AudioFrameHeader(AudioFrameFactory.ReadHeader(stream));
                    if (!audioFrameHeader.Valid)
                    {
                        stream.Position = position2;
                        ++num1;
                        stream.ReadByte();
                    }
                    else
                        goto label_6;
                }
                else
                    break;
            }
            return (AudioFrameHeader)null;
        label_6:
            if (position2 > position1)
            {
                if (num1 > 0)
                    Trace.WriteLine(string.Format("total {0} bytes and {1} invalid frame headers skipped to get to the start of a valid frame at stream offset {2}", (object)(position2 - position1), (object)num1, (object)position2));
                else
                    Trace.WriteLine(string.Format("total {0} bytes skipped to get to the start of a valid frame at stream offset {1}", (object)(position2 - position1), (object)position2));
            }
            return audioFrameHeader;
        }

        private static int Seek(Stream stream, long remainingBytes)
        {
            byte num1 = (byte)0;
            int num2 = 0;
            while (remainingBytes > 0L && stream.Position < stream.Length)
            {
                byte num3 = num1;
                num1 = (byte)stream.ReadByte();
                ++num2;
                --remainingBytes;
                if ((int)num3 == (int)byte.MaxValue && ((int)num1 & 224) == 224)
                {
                    stream.Seek(-2L, SeekOrigin.Current);
                    return num2 - 2;
                }
            }
            return -1;
        }

        private static byte[] ReadHeader(Stream stream)
        {
            byte[] buffer = new byte[4];
            int num = stream.Read(buffer, 0, 4);
            if (num < 4)
                throw new InvalidAudioFrameException(string.Format("MPEG Audio Frame: only {0} bytes of header left at offset {1}", (object)num, (object)(stream.Position - (long)num)));
            return buffer;
        }
    }
}
