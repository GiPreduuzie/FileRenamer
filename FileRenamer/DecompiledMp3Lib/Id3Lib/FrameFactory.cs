using System;
using System.Collections.Generic;
using System.Reflection;
using DecompiledMp3Lib.Id3Lib.Exceptions;
using DecompiledMp3Lib.Id3Lib.Frames;

namespace DecompiledMp3Lib.Id3Lib
{
    public static class FrameFactory
    {
        private static Dictionary<string, Type> _frames = new Dictionary<string, Type>();

        static FrameFactory()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (FrameAttribute frameAttribute in type.GetCustomAttributes(typeof(FrameAttribute), false) as FrameAttribute[])
                    FrameFactory._frames.Add(frameAttribute.FrameId, type);
            }
        }

        public static FrameBase Build(string frameId)
        {
            if (frameId == null)
                throw new ArgumentNullException("frameId");
            if (frameId.Length != 4)
                throw new InvalidTagException("Invalid frame type: '" + frameId + "', it must be 4 characters long.");
            Type type = (Type)null;
            if (FrameFactory._frames.TryGetValue(frameId, out type))
                return (FrameBase)Activator.CreateInstance(type, new object[1]
        {
          (object) frameId
        });
            if (!FrameFactory._frames.TryGetValue(frameId.Substring(0, 1), out type))
                return (FrameBase)new FrameUnknown(frameId);
            return (FrameBase)Activator.CreateInstance(type, new object[1]
      {
        (object) frameId
      });
        }
    }
}
