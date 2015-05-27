using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using DecompiledMp3Lib.Id3Lib.Exceptions;
using DecompiledMp3Lib.Id3Lib.Frames;
using DecompiledMp3Lib.Mp3Lib;

namespace DecompiledMp3Lib.Id3Lib
{
    public class ID3v1
    {
        private static readonly byte[] _id3 = new byte[3]
    {
      (byte) 84,
      (byte) 65,
      (byte) 71
    };
        private static readonly string[] _genres = new string[148]
    {
      "Blues",
      "Classic Rock",
      "Country",
      "Dance",
      "Disco",
      "Funk",
      "Grunge",
      "Hip-Hop",
      "Jazz",
      "Metal",
      "New Age",
      "Oldies",
      "Other",
      "Pop",
      "R&B",
      "Rap",
      "Reggae",
      "Rock",
      "Techno",
      "Industrial",
      "Alternative",
      "Ska",
      "Death Metal",
      "Pranks",
      "Soundtrack",
      "Euro-Techno",
      "Ambient",
      "Trip-Hop",
      "Vocal",
      "Jazz+Funk",
      "Fusion",
      "Trance",
      "Classical",
      "Instrumental",
      "Acid",
      "House",
      "Game",
      "Sound Clip",
      "Gospel",
      "Noise",
      "Alternative Rock",
      "Bass",
      "Soul",
      "Punk",
      "Space",
      "Meditative",
      "Instrumental Pop",
      "Instrumental Rock",
      "Ethnic",
      "Gothic",
      "Darkwave",
      "Techno-Industrial",
      "Electronic",
      "Pop-Folk",
      "Eurodance",
      "Dream",
      "Southern Rock",
      "Comedy",
      "Cult",
      "Gangsta",
      "Top 40",
      "Christian Rap",
      "Pop/Funk",
      "Jungle",
      "Native American",
      "Cabaret",
      "New Wave",
      "Psychadelic",
      "Rave",
      "Showtunes",
      "Trailer",
      "Lo-Fi",
      "Tribal",
      "Acid Punk",
      "Acid Jazz",
      "Polka",
      "Retro",
      "Musical",
      "Rock & Roll",
      "Hard Rock",
      "Folk",
      "Folk/Rock",
      "National Folk",
      "Swing",
      "Fast-Fusion",
      "Bebob",
      "Latin",
      "Revival",
      "Celtic",
      "Bluegrass",
      "Avantgarde",
      "Gothic Rock",
      "Progressive Rock",
      "Psychedelic Rock",
      "Symphonic Rock",
      "Slow Rock",
      "Big Band",
      "Chorus",
      "Easy Listening",
      "Acoustic",
      "Humour",
      "Speech",
      "Chanson",
      "Opera",
      "Chamber Music",
      "Sonata",
      "Symphony",
      "Booty Bass",
      "Primus",
      "Porn Groove",
      "Satire",
      "Slow Jam",
      "Club",
      "Tango",
      "Samba",
      "Folklore",
      "Ballad",
      "Power Ballad",
      "Rhytmic Soul",
      "Freestyle",
      "Duet",
      "Punk Rock",
      "Drum Solo",
      "Acapella",
      "Euro-House",
      "Dance Hall",
      "Goa",
      "Drum & Bass",
      "Club-House",
      "Hardcore",
      "Terror",
      "Indie",
      "BritPop",
      "Negerpunk",
      "Polsk Punk",
      "Beat",
      "Christian Gangsta Rap",
      "Heavy Metal",
      "Black Metal",
      "Crossover",
      "Contemporary Christian",
      "Christian Rock",
      "Merengue",
      "Salsa",
      "Trash Metal",
      "Anime",
      "JPop",
      "SynthPop"
    };
        private string _song;
        private string _artist;
        private string _album;
        private string _year;
        private string _comment;
        private byte _track;
        private byte _genre;

        public static uint TagLength
        {
            get
            {
                return 128U;
            }
        }

        public string Song
        {
            get
            {
                return this._song;
            }
        }

        public string Artist
        {
            get
            {
                return this._artist;
            }
        }

        public string Year
        {
            get
            {
                return this._year;
            }
        }

        public string Album
        {
            get
            {
                return this._album;
            }
        }

        public string Comment
        {
            get
            {
                return this._comment;
            }
        }

        public byte Track
        {
            get
            {
                return this._track;
            }
        }

        public byte Genre
        {
            get
            {
                return this._genre;
            }
        }

        public TagModel FrameModel
        {
            get
            {
                return this.GetFrameModel();
            }
            set
            {
                this.SetFrameModel(value);
            }
        }

        public ID3v1()
        {
            this.Clear();
        }

        private TagModel GetFrameModel()
        {
            TagModel tagModel = new TagModel();
            tagModel.Add((FrameBase)new FrameText("TIT2")
            {
                TextCode = TextCode.Ascii,
                Text = this._song
            });
            tagModel.Add((FrameBase)new FrameText("TPE1")
            {
                TextCode = TextCode.Ascii,
                Text = this._artist
            });
            tagModel.Add((FrameBase)new FrameText("TALB")
            {
                TextCode = TextCode.Ascii,
                Text = this._album
            });
            tagModel.Add((FrameBase)new FrameText("TYER")
            {
                TextCode = TextCode.Ascii,
                Text = this._year
            });
            tagModel.Add((FrameBase)new FrameText("TRCK")
            {
                TextCode = TextCode.Ascii,
                Text = this._track.ToString((IFormatProvider)CultureInfo.InvariantCulture)
            });
            tagModel.Add((FrameBase)new FrameFullText("COMM")
            {
                TextCode = TextCode.Ascii,
                Language = "eng",
                Description = "",
                Text = this._comment
            });
            if ((int)this._genre >= 0 && (int)this._genre < ID3v1._genres.Length)
                tagModel.Add((FrameBase)new FrameText("TCON")
                {
                    TextCode = TextCode.Ascii,
                    Text = ID3v1._genres[(int)this._genre]
                });
            tagModel.Header.TagSize = 0U;
            tagModel.Header.Version = (byte)3;
            tagModel.Header.Revision = (byte)0;
            tagModel.Header.Unsync = false;
            tagModel.Header.Experimental = false;
            tagModel.Header.Footer = false;
            tagModel.Header.ExtendedHeader = false;
            return tagModel;
        }

        private void SetFrameModel(TagModel frameModel)
        {
            this.Clear();
            foreach (FrameBase tag in (Collection<FrameBase>)frameModel)
            {
                switch (tag.FrameId)
                {
                    case "TIT2":
                        this._song = ID3v1.GetTagText(tag);
                        continue;
                    case "TPE1":
                        this._artist = ID3v1.GetTagText(tag);
                        continue;
                    case "TALB":
                        this._album = ID3v1.GetTagText(tag);
                        continue;
                    case "TYER":
                        this._year = ID3v1.GetTagText(tag);
                        continue;
                    case "TRCK":
                        if (!byte.TryParse(ID3v1.GetTagText(tag), out this._track))
                        {
                            this._track = (byte)0;
                            continue;
                        }
                        continue;
                    case "TCON":
                        this._genre = ID3v1.ParseGenre(ID3v1.GetTagText(tag));
                        continue;
                    case "COMM":
                        this._comment = ID3v1.GetTagText(tag);
                        continue;
                    default:
                        continue;
                }
            }
        }

        private static string GetTagText(FrameBase tag)
        {
            FrameText frameText = tag as FrameText;
            if (frameText != null)
                return frameText.Text;
            FrameFullText frameFullText = tag as FrameFullText;
            if (frameFullText != null)
                return frameFullText.Text;
            return (string)null;
        }

        private static byte ParseGenre(string sGenre)
        {
            if (string.IsNullOrEmpty(sGenre))
                return byte.MaxValue;
            byte result;
            if (byte.TryParse(sGenre, out result))
                return result;
            if ((int)sGenre[0] == 40 && (int)sGenre[1] != 40)
            {
                int num = sGenre.IndexOf(')');
                if (num != -1 && byte.TryParse(sGenre.Substring(1, num - 1), out result))
                    return result;
            }
            byte num1 = (byte)0;
            foreach (string str in ID3v1._genres)
            {
                if (string.Equals(str.Trim(), sGenre, StringComparison.OrdinalIgnoreCase))
                    return num1;
                ++num1;
            }
            return (byte)12;
        }

        private void Clear()
        {
            this._song = "";
            this._artist = "";
            this._album = "";
            this._year = "";
            this._comment = "";
            this._track = (byte)0;
            this._genre = byte.MaxValue;
        }

        private static string GetString(Encoding encoding, byte[] tag)
        {
            int @byte = Memory.FindByte(tag, (byte)0, 0);
            if (@byte >= 0)
                return encoding.GetString(tag, 0, @byte);
            return encoding.GetString(tag);
        }

        public void Deserialize(Stream stream)
        {
            BinaryReader binaryReader = new BinaryReader(stream);
            Encoding encoding = Encoding.GetEncoding(1252);
            binaryReader.BaseStream.Seek((long)sbyte.MinValue, SeekOrigin.End);
            byte[] numArray1 = new byte[3];
            binaryReader.Read(numArray1, 0, 3);
            if (!Memory.Compare(ID3v1._id3, numArray1))
                throw new TagNotFoundException("ID3v1 tag was not found");
            byte[] numArray2 = new byte[30];
            binaryReader.Read(numArray2, 0, 30);
            this._song = ID3v1.GetString(encoding, numArray2);
            binaryReader.Read(numArray2, 0, 30);
            this._artist = ID3v1.GetString(encoding, numArray2);
            binaryReader.Read(numArray2, 0, 30);
            this._album = ID3v1.GetString(encoding, numArray2);
            binaryReader.Read(numArray2, 0, 4);
            this._year = (int)numArray2[0] == 0 || (int)numArray2[1] == 0 || ((int)numArray2[2] == 0 || (int)numArray2[3] == 0) ? string.Empty : encoding.GetString(numArray2, 0, 4);
            binaryReader.Read(numArray2, 0, 30);
            if ((int)numArray2[28] == 0)
            {
                this._track = numArray2[29];
                this._comment = encoding.GetString(numArray2, 0, Memory.FindByte(numArray2, (byte)0, 0));
            }
            else
            {
                this._track = (byte)0;
                this._comment = ID3v1.GetString(encoding, numArray2);
            }
            this._genre = binaryReader.ReadByte();
        }

        public void Serialize(Stream stream)
        {
            byte[] numArray = new byte[3];
            BinaryReader binaryReader = new BinaryReader(stream);
            binaryReader.BaseStream.Seek((long)sbyte.MinValue, SeekOrigin.End);
            binaryReader.Read(numArray, 0, 3);
            if (Memory.Compare(ID3v1._id3, numArray))
            {
                stream.Seek((long)sbyte.MinValue, SeekOrigin.End);
                this.Write(stream);
            }
            else
            {
                long position = stream.Position;
                stream.Seek(0L, SeekOrigin.End);
                try
                {
                    this.Write(stream);
                }
                catch (Exception ex)
                {
                    stream.SetLength(position);
                    throw ex;
                }
            }
        }

        public void Write(Stream stream)
        {
            BinaryWriter binaryWriter = new BinaryWriter(stream);
            Encoding encoding = Encoding.GetEncoding(1252);
            binaryWriter.Write(ID3v1._id3, 0, 3);
            byte[] numArray = new byte[30];
            if (this._song.Length > 30)
                this._song = this._song.Substring(0, 30);
            encoding.GetBytes(this._song, 0, this._song.Length, numArray, 0);
            binaryWriter.Write(numArray, 0, 30);
            Memory.Clear(numArray, 0, 30);
            if (this._artist.Length > 30)
                this._artist = this._artist.Substring(0, 30);
            encoding.GetBytes(this._artist, 0, this._artist.Length, numArray, 0);
            binaryWriter.Write(numArray, 0, 30);
            Memory.Clear(numArray, 0, 30);
            if (this._album.Length > 30)
                this._album = this._album.Substring(0, 30);
            encoding.GetBytes(this._album, 0, this._album.Length, numArray, 0);
            binaryWriter.Write(numArray, 0, 30);
            Memory.Clear(numArray, 0, 30);
            if (string.IsNullOrEmpty(this._year))
            {
                Memory.Clear(numArray, 0, 30);
            }
            else
            {
                ushort result;
                if (!ushort.TryParse(this._year, out result))
                    this._year = "0";
                if ((int)result > 9999)
                    this._year = "0";
                encoding.GetBytes(this._year, 0, this._year.Length, numArray, 0);
            }
            binaryWriter.Write(numArray, 0, 4);
            Memory.Clear(numArray, 0, 30);
            if (this._comment.Length > 28)
                this._comment = this._comment.Substring(0, 28);
            encoding.GetBytes(this._comment, 0, this._comment.Length, numArray, 0);
            binaryWriter.Write(numArray, 0, 28);
            Memory.Clear(numArray, 0, 30);
            binaryWriter.Write((byte)0);
            binaryWriter.Write(this._track);
            binaryWriter.Write(this._genre);
        }
    }
}
