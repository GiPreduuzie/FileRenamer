using System;
using DecompiledMp3Lib.Id3Lib.Exceptions;

namespace DecompiledMp3Lib.Mp3Lib
{
    [Serializable]
    public class InvalidAudioFrameException : InvalidStructureException
    {
        public InvalidAudioFrameException()
        {
        }

        public InvalidAudioFrameException(string message)
            : base(message)
        {
        }

        public InvalidAudioFrameException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}