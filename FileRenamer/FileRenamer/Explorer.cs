using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileRenamer
{
    public interface IExplorer
    {
        IEnumerable<SongDirectory> GetDirectories(string rootPath);
        IEnumerable<SongDirectory> ReadAndGetDirectories(string rootPath);
    }

    public class SimpleDirectory
    {
        private readonly string _rootPath;
        private readonly string _relatedLocation;
        private readonly IEnumerable<string> _files;

        public SimpleDirectory(string rootPath, string relatedLocation, IEnumerable<string> files)
        {
            _rootPath = rootPath;
            _relatedLocation = relatedLocation;
            _files = files;
        }

        public IEnumerable<string> Files
        {
            get { return _files; }
        }

        public string RootPath
        {
            get { return _rootPath; }
        }

        public string RelatedLocation
        {
            get { return _relatedLocation; }
        }
    }

    public interface IFileEnumerator
    {
        IEnumerable<SimpleDirectory> GetDirectories(string rootPath);
    }

    public class SimpleFileEnumerator : IFileEnumerator
    {
        public IEnumerable<SimpleDirectory> GetDirectories(string rootPath)
        {
            return GetDirectories(rootPath, string.Empty);
        }

        public IEnumerable<SimpleDirectory> GetDirectories(string rootPath, string relatedLocation)
        {
            return new List<SimpleDirectory>
            {
                new SimpleDirectory(rootPath, relatedLocation, Directory.GetFiles(rootPath, "*.mp3"))
            };
        }
    }

    public class FileEnumerator : IFileEnumerator
    {
        SimpleFileEnumerator _simpleEnumerator;

        public FileEnumerator(SimpleFileEnumerator simpleEnumerator)
        {
            _simpleEnumerator = simpleEnumerator;
        }

        public IEnumerable<SimpleDirectory> GetDirectories(string rootPath)
        {
            return GetDirectories(rootPath, string.Empty);
        }

        private IEnumerable<SimpleDirectory> GetDirectories(string rootPath, string relatedLocation)
        {
            var directories = Directory.GetDirectories(rootPath).ToList();

            var result = _simpleEnumerator.GetDirectories(rootPath, relatedLocation);

            foreach (var directory in directories)
            {
                result.Union(GetDirectories(directory, Path.Combine(relatedLocation, GetDirectoryName(directory))));
            }

            return result;
        }

        private string GetDirectoryName(string absoluteLocation)
        {
            return absoluteLocation.Split(Path.DirectorySeparatorChar).Reverse().Take(1).Single();
        }
    }

    public class SongDirectoryConstructor
    {
        public SongDirectory ConstructFromPreform(SimpleDirectory simpleDirectory)
        {
            return
                new SongDirectory(
                    simpleDirectory.RootPath,
                    simpleDirectory.RelatedLocation,
                    new DirectoryName(simpleDirectory.RootPath),
                    simpleDirectory.Files.Select((file, index) => CreateFileUpdater(index, file, simpleDirectory)).ToList());
        }

        private FileUpdater CreateFileUpdater(int index, string file, SimpleDirectory simpleDirectory)
        {
            return new FileUpdater(
                file,
                index,
                simpleDirectory.RelatedLocation,
                simpleDirectory.Files.Count());
        }
    }


    public class DirectoryProvider : IExplorer
    {
        private readonly IFileEnumerator _fileEnumerator;
        private SongDirectoryConstructor _songDirectoryConstructor;

        public DirectoryProvider(IFileEnumerator fileEnumerator, SongDirectoryConstructor songDirectoryConstructor)
        {
            _fileEnumerator = fileEnumerator;
            _songDirectoryConstructor = songDirectoryConstructor;
        }

        public IEnumerable<SongDirectory> GetDirectories(string rootPath)
        {
            var directories = _fileEnumerator.GetDirectories(rootPath).ToList();
            return directories.Select(_songDirectoryConstructor.ConstructFromPreform);
        }

        public IEnumerable<SongDirectory> ReadAndGetDirectories(string rootPath)
        {
            var directories = GetDirectories(rootPath).ToList();
            foreach (var songDirectory in directories)
            {
                Explore(songDirectory);
            }
            return directories;
        }

        private void Explore(SongDirectory songDirectory)
        {
            int i = 1;
            foreach (var fileUpdater in songDirectory.FileUpdaters)
            {
                try
                {
                    fileUpdater.Initialize();
                }
                catch (Exception)
                {
                    Console.WriteLine("Warning! Fail to read file " + fileUpdater.OriginFile);
                }
                i++;
            }

            foreach (var fileUpdater in songDirectory.FileUpdaters)
            {
                fileUpdater.ExploreModel();
            }
        }
    }


    public class Explorer
    {
        private readonly string _pathToRoot;
        DirectoryProvider _directoryProvider;

        public Explorer(DirectoryProvider directoryProvider)
        {
            _directoryProvider = directoryProvider;
        }

        public void Explore(string pathToRoot, bool usePositionAsTrackNumber, INamePattern namePattern, AskUser userAsker)
        {
            var directories = _directoryProvider.ReadAndGetDirectories(pathToRoot).ToList();

            directories = directories.Where(item => item.Songs.Any()).ToList();

            foreach (var directory in directories.Where(item => item.HasQuestion))
            {
                userAsker.AboutDirectory(directory);
            }

            directories = directories.Where(item => !item.IsIgnored).ToList();

            var artists = directories
                .GroupBy(item => item.ParentFolderName)
                .Select(item => new Artist(item.ToList(), item.First().ParentFolderName))
                .ToList();

            foreach (var artist in artists.Where(item => item.HasQuestions))
            {
                artist.TryToSelectArtistName();
                userAsker.AboutArtist(artist);
            }

            foreach (var directory in directories)
            {
                foreach (var song in directory.Songs)
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


        //private void FixArtist(string file)
        //{
        //    var fileMp3 = new Mp3File(file);
        //    fileMp3.TagHandler.Artist = Encode("М.К. Щербаков");
        //    fileMp3.UpdatePacked();
        //}
    }
}
