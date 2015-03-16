using System;
using System.Linq;

namespace FileRenamer
{
    class FileName
    {
        public FileName(string value)
        {
            TrackNumber = null;
            OriginalName = value;

            value = value.Replace("_", " ");
            var trackNumberCandidate = value.Split(' ', '-').First().Replace(".", "");

            int trackNumber;
            if (int.TryParse(trackNumberCandidate, out trackNumber))
            {
                TrackNumber = trackNumber;
            }
        }

        public string OriginalName{ get; private set; }

        public int? TrackNumber { get; private set; }

        public string FormattedTrack
        {
            get { throw new NotImplementedException(); }
        }
    }
}