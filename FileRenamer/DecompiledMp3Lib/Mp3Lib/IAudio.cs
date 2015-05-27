using System;
using System.IO;

namespace DecompiledMp3Lib.Mp3Lib
{
    public interface IAudio
    {
        AudioFrameHeader Header { get; }

        string DebugString { get; }

        bool IsVbr { get; }

        uint NumPayloadBytes { get; }

        uint NumAudioBytes { get; }

        uint NumPayloadFrames { get; }

        uint NumAudioFrames { get; }

        double Duration { get; }

        double? BitRateCalc { get; }

        uint? BitRateMp3 { get; }

        double? BitRateVbr { get; }

        double BitRate { get; }

        bool HasInconsistencies { get; }

        Exception ParsingError { get; }

        Stream OpenAudioStream();

        byte[] CalculateAudioSHA1();

        void ScanWholeFile();
    }
}
