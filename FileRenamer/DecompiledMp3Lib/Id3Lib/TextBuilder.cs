using System.IO;
using System.Text;
using DecompiledMp3Lib.Id3Lib.Exceptions;
using DecompiledMp3Lib.Mp3Lib;

namespace DecompiledMp3Lib.Id3Lib
{
  internal static class TextBuilder
  {
    public static string ReadText(byte[] frame, ref int index, TextCode code)
    {
      switch (code)
      {
        case TextCode.Ascii:
          return TextBuilder.ReadASCII(frame, ref index);
        case TextCode.Utf16:
          return TextBuilder.ReadUTF16(frame, ref index);
        case TextCode.Utf16BE:
          return TextBuilder.ReadUTF16BE(frame, ref index);
        case TextCode.Utf8:
          return TextBuilder.ReadUTF8(frame, ref index);
        default:
          throw new InvalidFrameException("Invalid text code string type.");
      }
    }

    public static string ReadTextEnd(byte[] frame, int index, TextCode code)
    {
      switch (code)
      {
        case TextCode.Ascii:
          return TextBuilder.ReadASCIIEnd(frame, index);
        case TextCode.Utf16:
          return TextBuilder.ReadUTF16End(frame, index);
        case TextCode.Utf16BE:
          return TextBuilder.ReadUTF16BEEnd(frame, index);
        case TextCode.Utf8:
          return TextBuilder.ReadUTF8End(frame, index);
        default:
          throw new InvalidFrameException("Invalid text code string type.");
      }
    }

    public static string ReadASCII(byte[] frame, ref int index)
    {
      string str = (string) null;
      int @byte = Memory.FindByte(frame, (byte) 0, index);
      if (@byte == -1)
        throw new InvalidFrameException("Invalid ASCII string size");
      if (@byte > 0)
      {
        str = Encoding.GetEncoding(1252).GetString(frame, index, @byte);
        index += @byte;
      }
      ++index;
      return str;
    }

    public static string ReadUTF16(byte[] frame, ref int index)
    {
      if (index >= frame.Length - 2)
        throw new InvalidFrameException("ReadUTF16: string must be terminated");
      if ((int) frame[index] == 254 && (int) frame[index + 1] == (int) byte.MaxValue)
      {
        index += 2;
        return TextBuilder.ReadUTF16BE(frame, ref index);
      }
      if ((int) frame[index] == (int) byte.MaxValue && (int) frame[index + 1] == 254)
      {
        index += 2;
        return TextBuilder.ReadUTF16LE(frame, ref index);
      }
      if ((int) frame[index] != 0 || (int) frame[index + 1] != 0)
        throw new InvalidFrameException("Invalid UTF16 string.");
      index += 2;
      return "";
    }

    public static string ReadUTF16BE(byte[] frame, ref int index)
    {
      UnicodeEncoding unicodeEncoding = new UnicodeEncoding(true, false);
      int @short = Memory.FindShort(frame, (short) 0, index);
      if (@short == -1)
        throw new InvalidFrameException("Invalid UTF16BE string size");
      string @string = unicodeEncoding.GetString(frame, index, @short);
      index += @short;
      index += 2;
      return @string;
    }

    private static string ReadUTF16LE(byte[] frame, ref int index)
    {
      UnicodeEncoding unicodeEncoding = new UnicodeEncoding(false, false);
      int @short = Memory.FindShort(frame, (short) 0, index);
      if (@short == -1)
        throw new InvalidFrameException("Invalid UTF16LE string size");
      string @string = unicodeEncoding.GetString(frame, index, @short);
      index += @short;
      index += 2;
      return @string;
    }

    public static string ReadUTF8(byte[] frame, ref int index)
    {
      string str = (string) null;
      int @byte = Memory.FindByte(frame, (byte) 0, index);
      if (@byte == -1)
        throw new InvalidFrameException("Invalid UTF8 string size");
      if (@byte > 0)
      {
        str = Encoding.UTF8.GetString(frame, index, @byte);
        index += @byte;
      }
      ++index;
      return str;
    }

    public static string ReadASCIIEnd(byte[] frame, int index)
    {
      return Encoding.GetEncoding(1252).GetString(frame, index, frame.Length - index);
    }

    public static string ReadUTF16End(byte[] frame, int index)
    {
      if (index >= frame.Length - 2)
        return "";
      if ((int) frame[index] == 254 && (int) frame[index + 1] == (int) byte.MaxValue)
        return TextBuilder.ReadUTF16BEEnd(frame, index + 2);
      if ((int) frame[index] == (int) byte.MaxValue && (int) frame[index + 1] == 254)
        return TextBuilder.ReadUTF16LEEnd(frame, index + 2);
      throw new InvalidFrameException("Invalid UTF16 string.");
    }

    public static string ReadUTF16BEEnd(byte[] frame, int index)
    {
      return new UnicodeEncoding(true, false).GetString(frame, index, frame.Length - index);
    }

    private static string ReadUTF16LEEnd(byte[] frame, int index)
    {
      return new UnicodeEncoding(false, false).GetString(frame, index, frame.Length - index);
    }

    public static string ReadUTF8End(byte[] frame, int index)
    {
      return Encoding.UTF8.GetString(frame, index, frame.Length - index);
    }

    public static byte[] WriteText(string text, TextCode code)
    {
      switch (code)
      {
        case TextCode.Ascii:
          return TextBuilder.WriteASCII(text);
        case TextCode.Utf16:
          return TextBuilder.WriteUTF16(text);
        case TextCode.Utf16BE:
          return TextBuilder.WriteUTF16BE(text);
        case TextCode.Utf8:
          return TextBuilder.WriteUTF8(text);
        default:
          throw new InvalidFrameException("Invalid text code string type.");
      }
    }

    public static byte[] WriteTextEnd(string text, TextCode code)
    {
      switch (code)
      {
        case TextCode.Ascii:
          return TextBuilder.WriteASCIIEnd(text);
        case TextCode.Utf16:
          return TextBuilder.WriteUTF16End(text);
        case TextCode.Utf16BE:
          return TextBuilder.WriteUTF16BEEnd(text);
        case TextCode.Utf8:
          return TextBuilder.WriteUTF8End(text);
        default:
          throw new InvalidFrameException("Invalid text code string type.");
      }
    }

    public static byte[] WriteASCII(string text)
    {
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      if (string.IsNullOrEmpty(text))
      {
        binaryWriter.Write((byte) 0);
        return memoryStream.ToArray();
      }
      Encoding encoding = Encoding.GetEncoding(1252);
      binaryWriter.Write(encoding.GetBytes(text));
      binaryWriter.Write((byte) 0);
      return memoryStream.ToArray();
    }

    public static byte[] WriteUTF16(string text)
    {
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      if (string.IsNullOrEmpty(text))
      {
        binaryWriter.Write((ushort) 0);
        return memoryStream.ToArray();
      }
      binaryWriter.Write(byte.MaxValue);
      binaryWriter.Write((byte) 254);
      UnicodeEncoding unicodeEncoding = new UnicodeEncoding(false, false);
      binaryWriter.Write(unicodeEncoding.GetBytes(text));
      binaryWriter.Write((ushort) 0);
      return memoryStream.ToArray();
    }

    public static byte[] WriteUTF16BE(string text)
    {
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      UnicodeEncoding unicodeEncoding = new UnicodeEncoding(true, false);
      if (string.IsNullOrEmpty(text))
      {
        binaryWriter.Write((ushort) 0);
        return memoryStream.ToArray();
      }
      binaryWriter.Write(unicodeEncoding.GetBytes(text));
      binaryWriter.Write((ushort) 0);
      return memoryStream.ToArray();
    }

    public static byte[] WriteUTF8(string text)
    {
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      if (string.IsNullOrEmpty(text))
      {
        binaryWriter.Write((byte) 0);
        return memoryStream.ToArray();
      }
      binaryWriter.Write(Encoding.UTF8.GetBytes(text));
      binaryWriter.Write((byte) 0);
      return memoryStream.ToArray();
    }

    public static byte[] WriteASCIIEnd(string text)
    {
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      if (string.IsNullOrEmpty(text))
        return memoryStream.ToArray();
      Encoding encoding = Encoding.GetEncoding(1252);
      binaryWriter.Write(encoding.GetBytes(text));
      return memoryStream.ToArray();
    }

    public static byte[] WriteUTF16End(string text)
    {
      MemoryStream memoryStream = new MemoryStream(text.Length + 2);
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      if (string.IsNullOrEmpty(text))
        return memoryStream.ToArray();
      binaryWriter.Write(byte.MaxValue);
      binaryWriter.Write((byte) 254);
      UnicodeEncoding unicodeEncoding = new UnicodeEncoding(false, false);
      binaryWriter.Write(unicodeEncoding.GetBytes(text));
      return memoryStream.ToArray();
    }

    public static byte[] WriteUTF16BEEnd(string text)
    {
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      if (string.IsNullOrEmpty(text))
        return memoryStream.ToArray();
      UnicodeEncoding unicodeEncoding = new UnicodeEncoding(true, false);
      binaryWriter.Write(unicodeEncoding.GetBytes(text));
      return memoryStream.ToArray();
    }

    public static byte[] WriteUTF8End(string text)
    {
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      if (string.IsNullOrEmpty(text))
        return memoryStream.ToArray();
      binaryWriter.Write(Encoding.UTF8.GetBytes(text));
      return memoryStream.ToArray();
    }
  }
}
