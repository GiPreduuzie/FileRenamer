using System;
using System.Runtime.Serialization;

namespace DecompiledMp3Lib.Id3Lib.Exceptions
{
    [Serializable]
    public class InvalidFrameException : InvalidStructureException
    {
        protected InvalidFrameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public InvalidFrameException()
        {
        }

        public InvalidFrameException(string message)
            : base(message)
        {
        }

        public InvalidFrameException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
