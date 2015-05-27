using System;

namespace DecompiledMp3Lib.Id3Lib.Frames
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class FrameAttribute : Attribute
    {
        private string _frameId;

        public string FrameId
        {
            get
            {
                return this._frameId;
            }
        }

        public FrameAttribute(string frameId)
        {
            if (frameId == null)
                throw new ArgumentNullException("frameId");
            this._frameId = frameId;
        }
    }
}