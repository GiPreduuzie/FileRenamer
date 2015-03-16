using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileRenamer
{
    class Explorer
    {
        private readonly string _pathToRoot;
        private IList<SongDirectory> _directories = new List<SongDirectory>();

        public Explorer(string pathToRoot)
        {
            _pathToRoot = pathToRoot;
        }

        public void Explore(bool usePositionAsTrackNumber, INamePattern namePattern, AskUser userAsker)
        {
            Explore(_pathToRoot, usePositionAsTrackNumber);

            _directories = _directories.Where(item => item.Songs.Any()).ToList();

            foreach( var directory in _directories.Where(item => item.HasQuestion))
            {
                userAsker.AboutDirectory(directory);
            }

            _directories = _directories.Where(item => !item.IsIgnored).ToList();

            var artists = _directories
                .GroupBy(item => item.ParentFolderName)
                .Select(item => new Artist(item.ToList(), item.First().ParentFolderName))
                .ToList();

            foreach (var artist in artists.Where(item => item.HasQuestions))
            {
                artist.TryToSelectArtistName();
                userAsker.AboutArtist(artist);
            }

            foreach (var direcortory in _directories)
            {
                foreach (var song in direcortory.Songs)
                {
                    try
                    {
                        var location = Path.Combine(_pathToRoot, "Processed and Corrected");
                        var newLocation = song.Copy(location);
                        song.RemoveTags(newLocation);
                        song.Update(newLocation, namePattern);
                        song.RemoveBackups(newLocation);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Warning! Could not update: " + song.OriginFile);
                    }
                }
            }
        }

        private void Explore(string rootPath, bool usePositionAsTrackNumber)
        {
            var directories = Directory.GetDirectories(rootPath).ToList();

            foreach (var directory in directories)
            {
                Explore(directory, usePositionAsTrackNumber);
            }

            var files = Directory.GetFiles(rootPath, "*.mp3").ToList();
            var fileUpdaters =
                files.Select(
                    item =>
                        new FileUpdater(item,
                            Path.GetDirectoryName(item).Substring(Path.GetDirectoryName(_pathToRoot).Length + 1),
                            files.Count)).ToList();

            int i = 1 + 46;
            foreach (var fileUpdater in fileUpdaters)
            {
                try
                {
                    if (usePositionAsTrackNumber)
                    {
                        fileUpdater.FileNamingModel.Track = i.ToString();
                        fileUpdater.ExploreAttributes();
                    }
                   
                    fileUpdater.Initialize();

                    if (usePositionAsTrackNumber)
                    {
                        fileUpdater.FileNamingModel.Track = i.ToString();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Warning! Fail to read file " + fileUpdater.OriginFile);
                }
                i++;
            }

            foreach (var fileUpdater in fileUpdaters)
            {
                fileUpdater.ExploreModel();
                //fileUpdater.ConvertToV1();
            }

            _directories.Add(new SongDirectory(rootPath, _pathToRoot, new DirectoryName(rootPath), fileUpdaters));
        }
        

        //private void FixArtist(string file)
        //{
        //    var fileMp3 = new Mp3File(file);
        //    fileMp3.TagHandler.Artist = Encode("М.К. Щербаков");
        //    fileMp3.UpdatePacked();
        //}
    }
}