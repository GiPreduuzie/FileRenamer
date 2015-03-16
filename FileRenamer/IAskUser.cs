using System.Collections.Generic;

namespace FileRenamer
{
    public interface IAskUser
    {
        int AboutTrack(FileUpdater fileUpdater);

        string AboutAlbum(IDictionary<string, List<string>> candidates, string candidate);

        string AboutArtist(Dictionary<string, List<FileUpdater>> candidates, string candidate);
    }
}