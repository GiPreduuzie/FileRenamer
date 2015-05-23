using System.Collections.Generic;

namespace FileRenamer
{
    interface IAskUser
    {
        int AboutTrack(FileUpdater fileUpdater);

        string AboutAlbum(IDictionary<string, List<FileUpdater>> candidates, string candidate);

        string AboutArtist(Dictionary<string, List<FileUpdater>> candidates, string candidate);
    }
}