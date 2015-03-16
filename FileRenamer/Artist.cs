using System.Collections.Generic;
using System.Linq;

namespace FileRenamer
{
    public class Artist
    {
        private IList<SongDirectory> _directories;
        string _artist;
        private Dictionary<string, List<SongDirectory>> _artistNameVariants;
        private string _defaultArtistName;

        public Artist(IList<SongDirectory> directories, string defaultArtistName)
        {
            _directories = directories;
            _defaultArtistName = defaultArtistName;
        }

        public bool HasQuestions { get { return string.IsNullOrEmpty(_artist); } }


        private string GetArtistName(IList<SongDirectory> directories, string defaultArtistName)
        {
            var artistNameVariants = _directories
                .Where(item => !string.IsNullOrWhiteSpace(item.Artist))
                .GroupBy(item => item.Artist)
                .ToDictionary(item => item.First().Artist, item => item.ToList());

            if (!artistNameVariants.Any())
                return defaultArtistName;

            if (artistNameVariants.Count == 1)
                return artistNameVariants.Keys.First();

            _artistNameVariants = artistNameVariants;

            return string.Empty;
        }

        public IList<SongDirectory> Directories { get { return _directories; } }

        internal void SetArtist(string artistName)
        {
            _artist = artistName;
            foreach(var directory in _directories)
            {
                foreach (var song in directory.Songs)
                {
                    song.FileNamingModel.Artist = _artist;
                }
            }
        }

        internal void TryToSelectArtistName()
        {
            _artist = GetArtistName(_directories, _defaultArtistName);
        }
    }
}