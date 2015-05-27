using System;
using System.Runtime.Serialization;

namespace DecompiledMp3Lib.Id3Lib.Exceptions
{
    [Serializable]
    public class TagNotFoundException : Exception
    {
        protected TagNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public TagNotFoundException()
        {
        }

        public TagNotFoundException(string message)
            : base(message)
        {
        }

        public TagNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}