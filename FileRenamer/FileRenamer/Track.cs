using System.Linq;

namespace FileRenamer
{
    class Track
    {
        public Track(string value, int defaultTracksInAlbum)
        {
            TracksInAlbum = defaultTracksInAlbum;
            IsSet = false;
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var parts = value.Split(new[] { '/', '\\' }).Select(item => item.Trim()).Where(item => !string.IsNullOrWhiteSpace(item)).ToList();
            int trackNumber;
            if (!int.TryParse(parts[0], out trackNumber))
            {
                return;
            }

            IsSet = true;
            TrackNumber = trackNumber;

            if (parts.Count == 2 && int.TryParse(parts[1], out trackNumber))
            {
                TracksInAlbum = trackNumber;
            }
        }

        public bool IsSet { get; private set; }

        public int TrackNumber { get; private set; }
        public int TracksInAlbum { get; private set; }
    }
}