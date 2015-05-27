using System;
using System.Runtime.Serialization;

namespace DecompiledMp3Lib.Id3Lib.Exceptions
{
    [Serializable]
    public class InvalidStructureException : Exception
    {
        protected InvalidStructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public InvalidStructureException()
        {
        }

        public InvalidStructureException(string message)
            : base(message)
        {
        }

        public InvalidStructureException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
