using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileRenamer
{
    public class SongDirectory
    {
        private string _absoluteLocation;
        private readonly string _relativeLocation;
        private readonly List<FileUpdater> _fileUpdaters;
        readonly DirectoryName _directoryName;

        string _album;
        private Dictionary<string, List<string>> _albumNameVariants;
        string _artist;
        private Dictionary<string, List<FileUpdater>> _aritstNameVariants;
       

        public SongDirectory(string absoluteLocation, string relatedLocation, DirectoryName directoryName, List<FileUpdater> fileUpdaters)
        {
            Songs = fileUpdaters;
            _relativeLocation = relatedLocation;
            this._absoluteLocation = absoluteLocation;
            this._fileUpdaters = fileUpdaters;
            _directoryName = directoryName;

            ParentFolderName = absoluteLocation.Split(Path.DirectorySeparatorChar).Reverse().Skip(1).First();

            _album = GetAlbumName(fileUpdaters, _directoryName.Album);
            foreach(var song in Songs)
            {
                song.FileNamingModel.Album = _album;
            }
            _artist = GetArtistName(fileUpdaters, _directoryName.Artist);
        }

        public IReadOnlyList<FileUpdater> Songs { get; private set; }

        public bool HasQuestion
        {
            get
            {
                return AlbumIsNotSet || TrackIsNotSet;
            }
        }

        public string ParentFolderName { get; private set; }
        public IList<FileUpdater> FileUpdaters { get { return _fileUpdaters; } }

        public string Location { get { return _relativeLocation; } }

        private bool ArtistIsNotSet { get { return string.IsNullOrEmpty(_artist); } }
        private bool AlbumIsNotSet { get { return string.IsNullOrEmpty(_album);  } }
        private bool TrackIsNotSet { get { return _fileUpdaters.Any(item => string.IsNullOrEmpty(item.FileNamingModel.Track)); } }

        public void AskQuestions(IAskUser askUser)
        {
            if (AlbumIsNotSet)
            {
                _album = askUser.AboutAlbum(_albumNameVariants, _directoryName.Album);
                foreach (var song in Songs)
                {
                    song.FileNamingModel.Album = _album;
                }
            }

            if (TrackIsNotSet)
            {
                foreach (var song in Songs.Where(item => string.IsNullOrEmpty(item.FileNamingModel.Track)))
                {
                    song.SetTrack(askUser.AboutTrack(song));
                }
            }
            //if (ArtistIsNotSet)
            //{
            //    _artist = askUser.AboutArtist(_albumNameVariants, _directoryName.Artist);
            //}
        }

        private string GetAlbumName(IList<FileUpdater> fileUpdaters, string albumNameCandidate)
        {
            var albumNameVariants = fileUpdaters
                .Where(item => !string.IsNullOrWhiteSpace(item.FileNamingModel.Album))
                .GroupBy(item => item.FileNamingModel.Album)
                .ToDictionary(item => item.First().FileNamingModel.Album, item => item.Select(song => song.FileNamingModel.Song).ToList());

            if (!albumNameVariants.Any())
                return albumNameCandidate;

            if (albumNameVariants.Count == 1 && albumNameCandidate.Contains(albumNameVariants.Keys.First()))
                return albumNameVariants.Keys.First();

            _albumNameVariants = albumNameVariants;

            return string.Empty;
        }

        private string GetArtistName(IList<FileUpdater> fileUpdaters, string artistNameCandidate)
        {
            var aritstNameVariants = fileUpdaters
                .Where(item => !string.IsNullOrWhiteSpace(item.FileNamingModel.Artist))
                .GroupBy(item => item.FileNamingModel.Artist)
                .ToDictionary(item => item.First().FileNamingModel.Artist, item => item.ToList());

            if (!aritstNameVariants.Any())
                return artistNameCandidate;

            if (aritstNameVariants.Count == 1)
                return aritstNameVariants.Keys.First();

            _aritstNameVariants = aritstNameVariants;

            return string.Empty;
        }

        public string Artist
        {
            get
            {
                return _artist;
            }
        }

        public string Album
        {
            get
            {
                return _album;
            }
        }

        internal void Ignore()
        {
            IsIgnored = true;
        }

        public bool IsIgnored { get; private set; }
    }

}