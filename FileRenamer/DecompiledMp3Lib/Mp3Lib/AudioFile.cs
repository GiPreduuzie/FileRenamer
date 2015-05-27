using System;
using System.IO;
using System.Security.Cryptography;

namespace DecompiledMp3Lib.Mp3Lib
{
    internal class AudioFile : Audio
    {
        private FileInfo _sourceFileInfo;
        private uint _payloadStart;
        private uint _payloadNumBytes;

        public override string DebugString
        {
            get
            {
                return string.Format("{0}\n----AudioFile----\n  Header starts: {1} bytes\n  Payload: {2} bytes", (object)base.DebugString, (object)this._payloadStart, (object)this._payloadNumBytes);
            }
        }

        public override uint NumPayloadBytes
        {
            get
            {
                return this._payloadNumBytes;
            }
        }

        public AudioFile(FileInfo sourceFileInfo, uint audioStart, uint payloadNumBytes, TimeSpan? id3DurationTag)
            : base(AudioFile.ReadFirstFrame(sourceFileInfo, audioStart, payloadNumBytes), id3DurationTag)
        {
            this._sourceFileInfo = sourceFileInfo;
            this._payloadStart = audioStart;
            this._payloadNumBytes = payloadNumBytes;
            this.CheckConsistency();
        }

        private static AudioFrame ReadFirstFrame(FileInfo sourceFileInfo, uint audioStart, uint audioNumBytes)
        {
            using (FileStream fileStream = sourceFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fileStream.Seek((long)audioStart, SeekOrigin.Begin);
                return AudioFrameFactory.CreateFrame((Stream)fileStream, audioNumBytes);
            }
        }

        public override Stream OpenAudioStream()
        {
            FileStream fileStream = this._sourceFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            fileStream.Seek((long)this._payloadStart, SeekOrigin.Begin);
            return (Stream)fileStream;
        }

        public override byte[] CalculateAudioSHA1()
        {
            using (Stream stream = this.OpenAudioStream())
            {
                SHA1 shA1 = (SHA1)new SHA1CryptoServiceProvider();
                uint num1 = this._payloadNumBytes;
                byte[] numArray = new byte[4096];
                while (num1 > 0U)
                {
                    int num2 = stream.Read(numArray, 0, 4096);
                    if ((long)num1 > (long)num2)
                    {
                        shA1.TransformBlock(numArray, 0, 4096, numArray, 0);
                        num1 -= (uint)num2;
                    }
                    else
                        break;
                }
                shA1.TransformFinalBlock(numArray, 0, (int)num1);
                return shA1.Hash;
            }
        }
    }
}
