using System;
using System.Runtime.Serialization;

namespace DecompiledMp3Lib.Id3Lib.Exceptions
{
    [Serializable]
    public class InvalidTagException : InvalidStructureException
    {
        protected InvalidTagException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public InvalidTagException()
        {
        }

        public InvalidTagException(string message)
            : base(message)
        {
        }

        public InvalidTagException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}