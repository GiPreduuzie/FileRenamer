using System;
using System.IO;
using System.Linq;
using System.Text;
using Id3Lib;
using Mp3Lib;

namespace FileRenamer
{
    public interface INamePattern
    {
        string PatternString { get; }

        FileNamingModel BuildModel(string fileName, FileNamingModel model);
    }

    public class FileNamingModel
    {
        public string Album { get; set; }

        public string Artist { get; set; }

        public string Song { get; set; }

        public string Track { get; set; }

        public DateTime? DateTime { get; set; }

        public FileModel File { get; set; }
    }

    public class FileModel
    {
        public FileModel(string value)
        {
            FullName = value;
            Extention = Path.GetExtension(value);
            Name = Path.GetFileNameWithoutExtension(value);
        }

        public string FullName { get ; private set; }
        public string Name { get; private set; }
        public string Extention { get; private set; }
    }

    internal class FirstPattern : INamePattern
    {
        public string PatternString
        {
            get { return "<yyyy.mm.dd> -- <Name Surname> <TITLE> (<option>).mp3"; }
        }

        public FileNamingModel BuildModel(string fileName, FileNamingModel model)
        {
            var parts = fileName.Split(' ').Select(item => item.Trim()).Where(item => !string.IsNullOrEmpty(item)).ToList();
            var date = parts[0];
            var dateParts = parts[0].Split('.');
            var dateTime = new DateTime(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]));

            var name = parts[2];
            var surname = parts[3];

            string title = parts[4];
            int i = 5;
            var count = parts.Count;
            while (i < count && !parts[i].StartsWith("("))
            {
                title += " " + parts[i];
                i++;
            }

            model.Song = title.Split(' ').Select(item => item[0] + item.Substring(1).ToLowerInvariant()).Aggregate((result, item) => result + " " + item);
            //Artist = name + " " + surname;
            model.DateTime = dateTime;

            return model;
        }
    }

    internal class SecondPattern : INamePattern
    {
        public string PatternString
        {
            get { return "<NN>. <Title>.mp3"; }
        }

        public FileNamingModel BuildModel(string fileName, FileNamingModel model)
        {
            var parts = fileName.Split('.').Select(item => item.Trim()).Where(item => !string.IsNullOrEmpty(item)).ToList();
            var number = int.Parse(parts[0]);
            var title = parts[1];

            model.Song = title;
            model.Track = number.ToString();

            return model;
        }
    }

    internal class ThirdPattern : INamePattern
    {
        public string PatternString
        {
            get { return "<Title>.mp3"; }
        }

        public FileNamingModel BuildModel(string fileName, FileNamingModel model)
        {
            var parts = fileName.Split('.').Select(item => item.Trim()).Where(item => !string.IsNullOrEmpty(item)).ToList();
            var title = parts[0];

            model.Song = title;

            return model;
        }
    }

    internal class GeneralPattern : INamePattern
    {
        public string PatternString
        {
            get { return "Does nothing"; }
        }

        public FileNamingModel BuildModel(string fileName, FileNamingModel model)
        {
            return model;
        }
    }


    public class FileUpdater
    {
        private readonly string _originfileName;
        private readonly Mp3File _originalMp3File;
        private readonly string _relatedPathToFile;
        private readonly int _musicFilesInTheDirectory;
        private Track _trackHelper;
        private FileNamingModel _namingModel;
        private int _index;

        public FileUpdater(string fileName, int index, string relatedPathToFile, int musicFilesInTheDirectory)
        {
            _relatedPathToFile = relatedPathToFile;
            _musicFilesInTheDirectory = musicFilesInTheDirectory;
            _originfileName = fileName;
            _originalMp3File = BuildMp3File(_originfileName);
            _index = index;

            _namingModel = new FileNamingModel();
        }

        public IAskUser UserAsker { get; private set; }

        public string OriginFile { get { return _originfileName; } }

        public FileNamingModel FileNamingModel { get { return _namingModel; } } 

        private Mp3File BuildMp3File(string fileName)
        {
            var fileMp3 = new Mp3File(fileName);

            //foreach (var modelItem in fileMp3.TagModel)
            //{
            //    var type = modelItem.GetType();
            //    if (type == typeof(Id3Lib.Frames.FrameText))
            //    {
            //        var textModel = modelItem as Id3Lib.Frames.FrameText;
            //        textModel.TextCode = TextCode.Ascii;
            //    }
            //}
            return fileMp3;
        }

        public void Initialize()
        {
            _namingModel.Album = Decode(_originalMp3File.TagHandler.Album);
            _namingModel.Artist = Decode(_originalMp3File.TagHandler.Artist);
            _namingModel.Song = Decode(_originalMp3File.TagHandler.Song);
            _namingModel.File = new FileModel(_originfileName);

            var track = _originalMp3File.TagHandler.Track;
            var fileNameHelper = new FileName(Path.GetFileNameWithoutExtension(_originfileName));
            _trackHelper = new Track(track, _musicFilesInTheDirectory);
            UpdateTrackNumber(_trackHelper, fileNameHelper);
        }

        public void ExploreAttributes()
        {
            //Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(1252).GetBytes(fileMp3.TagHandler.Album))

            var fileMp3 = _originalMp3File;
            Console.WriteLine(Path.GetFileNameWithoutExtension(_originfileName));
            Console.WriteLine();
            Console.WriteLine("Album: \t" + Decode(fileMp3.TagHandler.Album));
            Console.WriteLine("Artist:\t" + Decode(fileMp3.TagHandler.Artist));
            Console.WriteLine("Song: \t" + Decode(fileMp3.TagHandler.Song));
            Console.WriteLine("Title: \t" + Decode(fileMp3.TagHandler.Title));
            Console.WriteLine("Track: \t" + Decode(fileMp3.TagHandler.Track));
            Console.WriteLine();
            Console.WriteLine();

            var encoding = Encoding.Unicode;
            var bytes2 = encoding.GetBytes(fileMp3.TagHandler.Album);
        }

        public void ExploreModel()
        {
            var fileMp3 = _originalMp3File;
            //Console.WriteLine("File is: " + Path.GetFileNameWithoutExtension(_originfileName));
            Console.WriteLine("Tag model count: " + fileMp3.TagModel.Count);
            if (!fileMp3.TagModel.Any())
                return;

            foreach (var modelItem in fileMp3.TagModel)
            {
                var type = modelItem.GetType();
               // Console.WriteLine("->" + type);
                if (type == typeof(Id3Lib.Frames.FrameText))
                {
                    var textModel = modelItem as Id3Lib.Frames.FrameText;
                    //Console.WriteLine("-->" + textModel.TextCode);
                    if (textModel.TextCode != TextCode.Ascii)
                    {
                        Console.WriteLine(_originfileName);
                    }
                }
            }
            //Console.WriteLine("Encoded as: " + ((Id3Lib.Frames.FrameText)fileMp3.TagModel[1]).TextCode);
        }

        public void ConvertToV1()
        {
            var fileMp3 = _originalMp3File;
            fileMp3.UpdateNoV2tag();
        }


        private void UpdateTrackNumber(Track track, FileName fileName)
        {
            SetTrack(GetTrackNumber(track, fileName));
        }

        private int GetTracksInAlbumNumber()
        {
            return _trackHelper.TracksInAlbum;
        }

        private int? GetTrackNumber(Track track, FileName fileName)
        {
            var fileMp3 = _originalMp3File;
            
            if (track.IsSet)
            {
                return track.TrackNumber;
            }

            if (fileName.TrackNumber.HasValue)
            {
                return fileName.TrackNumber.Value;
            }

            return null;
        }

        private string Decode(string value)
        {
            //return Encoding.GetEncoding("ISO-8859-5").GetString(Encoding.GetEncoding("CP866").GetBytes(value));
            //return Encoding.GetEncoding("KOI8-R").GetString(Encoding.GetEncoding("KOI8-R").GetBytes(value));
            return Encoding.GetEncoding(1251).GetString(Encoding.GetEncoding(1252).GetBytes(value));
        }

        private string Encode(string value)
        {
            //return value;
            return Encoding.GetEncoding(1252).GetString(Encoding.GetEncoding(1251).GetBytes(value));
        }

        internal void SetTrack(int? trackNumber)
        {
            if (trackNumber == null)
            {
                _namingModel.Track = string.Empty;
                return;
            }

            _namingModel.Track = string.Format("{0}/{1}", trackNumber, GetTracksInAlbumNumber());
        }

        internal void RemoveTags(string newFileLocation)
        {
            var toUpdateMp3File = BuildMp3File(newFileLocation);
            toUpdateMp3File.UpdateNoV2tag();
        }

        internal void RemoveBackups(string newFileLocation)
        {
            foreach (var backupFile in Directory.GetFiles(Path.GetDirectoryName(newFileLocation), "*.bak"))
            {
                System.IO.File.Delete(backupFile);
            }
        }

        internal void Update(string newFileLocation, INamePattern namePattern)
        {
            FixAttributes(namePattern);
            newFileLocation = FixFileName(newFileLocation);

            var toUpdateMp3File = BuildMp3File(newFileLocation);

            toUpdateMp3File.TagHandler.Track = Encode(_namingModel.Track);

            if (_namingModel.DateTime.HasValue)
            {
                toUpdateMp3File.TagHandler.Song = Encode(_namingModel.DateTime.Value.ToShortDateString() + " - " + _namingModel.Song);
                toUpdateMp3File.TagHandler.Year = Encode(_namingModel.DateTime.Value.Year.ToString());
            }
            else
            {
                toUpdateMp3File.TagHandler.Song = Encode(_namingModel.Song);
            }

            toUpdateMp3File.TagHandler.Title = Encode(_namingModel.Song);
            toUpdateMp3File.TagHandler.Album = Encode(_namingModel.Album);
            toUpdateMp3File.TagHandler.Artist = Encode(_namingModel.Artist);

            toUpdateMp3File.Update();
           
            //Check that saved correctly
            var updatedFile = BuildMp3File(newFileLocation);

            Console.WriteLine("Encoded as : " + ((Id3Lib.Frames.FrameText)updatedFile.TagModel[1]).TextCode);
            if (Decode(updatedFile.TagHandler.Title) != _namingModel.Song)
            {
                Console.WriteLine("Decoding\\Encoding fails: " + _namingModel.Song);
            }
        }

        public string Copy(string newRoot)
        {
            var direcotryName = Path.Combine(newRoot, _relatedPathToFile);
            
            if (!Directory.Exists(direcotryName))
            {
                Directory.CreateDirectory(direcotryName);
            }

            var fileName = Path.Combine(direcotryName, Path.GetFileName(_originfileName));
            System.IO.File.Copy(_originalMp3File.FileName, fileName);
            return fileName;
        }

        private void FixAttributes(INamePattern namePattern)
        {
            _namingModel = namePattern.BuildModel(Path.GetFileNameWithoutExtension(_originfileName), _namingModel);
        }

        private string FixFileName(string file)
        {
            var track = _namingModel.Track;

            var newFileName = string.Format("{0:00}", int.Parse(_namingModel.Track.Split('/').First())) + ". " + _namingModel.Song;
            newFileName = Path.Combine(Path.GetDirectoryName(file), newFileName + Path.GetExtension(file));

            System.IO.File.Move(file, newFileName);

            return newFileName;
        }
    }
}