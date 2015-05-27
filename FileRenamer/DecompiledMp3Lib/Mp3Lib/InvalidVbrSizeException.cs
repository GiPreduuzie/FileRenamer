using System;
using DecompiledMp3Lib.Id3Lib.Exceptions;

namespace DecompiledMp3Lib.Mp3Lib
{
    [Serializable]
    public class InvalidVbrSizeException : InvalidStructureException
    {
        private uint _measured;
        private uint _specified;

        public uint Measured
        {
            get
            {
                return this._measured;
            }
        }

        public uint Specified
        {
            get
            {
                return this._specified;
            }
        }

        public override string Message
        {
            get
            {
                if (this._specified > this._measured)
                    return string.Format("VBR header states the audio size is {0} bytes, but the payload is only {1} bytes, so {2} bytes have been lost from the end", (object)this._specified, (object)this._measured, (object)(uint)((int)this._specified - (int)this._measured));
                return string.Format("VBR header states audio size is only {0} bytes, but the payload is {1} bytes, so {2} bytes have been added to the end", (object)this._specified, (object)this._measured, (object)(uint)((int)this._measured - (int)this._specified));
            }
        }

        public InvalidVbrSizeException(uint measured, uint specified)
            : base("VBR header claims audio size is not payload size.")
        {
            this._measured = measured;
            this._specified = specified;
        }
    }
}
