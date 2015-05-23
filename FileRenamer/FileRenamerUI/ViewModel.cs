using FileRenamer;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace FileRenamerUI
{
    public class ViewModel : IPatternSelector
    {
        private string _rootFolder;
        private Binder _binder;
        readonly IDictionary<string, INamePattern> _patterns;

        public event Action<string> OnRootFolderChanged;
        public event Action<IEnumerable<string>> OnPatternCollectionChanged;

        public ViewModel()
        {
            _binder = new Binder(this);
            _patterns = new Dictionary<string, INamePattern>();
        }

        public string RootFolder
        {
            get { return _rootFolder; }
            set
            {
                if (_rootFolder == value)
                    return;

                _rootFolder = value;
                OnRootFolderChanged(_rootFolder);
            }
        }

        public string Artist { get { return CurrentDirectory.Artist; } }

        public string Album { get { return CurrentDirectory.Album; } }

        public void LoadFolder(DataTable table, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            table.Clear();
            table.Columns.Clear();
            table.Rows.Clear();

            var directories = _binder.GetExplorer().ReadAndGetDirectories(value);
            CurrentDirectory = directories.First();

            table.Columns.Add("File");
            table.Columns.Add("Song");
            table.Columns.Add("Track");
            table.Columns.Add("Albom");
            table.Columns.Add("Artist");

            foreach (var fileUpdater in CurrentDirectory.FileUpdaters)
            {
                var namingModel = fileUpdater.FileNamingModel;
                table.Rows.Add(namingModel.File.Name, namingModel.Song, namingModel.Track, namingModel.Album, namingModel.Artist);
            }
        }

        public void AddPattern(INamePattern namePattern)
        {
            _patterns.Add(namePattern.PatternString, namePattern);
            OnPatternCollectionChanged(_patterns.Keys);
        }

        public INamePattern SelectedPattern { get; private set; }

        public INamePattern Select()
        {
            return SelectedPattern;
        }

        internal void Rename()
        {
            _binder.GetRenamer().Explore(_rootFolder, false, SelectedPattern, new AskUser(new UiUserAsker(this)));
        }

        private SongDirectory CurrentDirectory { get; set; }

        internal void SelectPattern(string pattern)
        {
            SelectedPattern = _patterns[pattern];
        }
    }

    public class UiUserAsker : IUserAsker
    {
        private readonly ViewModel _viewModel;

        public UiUserAsker(ViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool IgnoreDirectory(string directoryLocation)
        {
            return false;
        }

        public string AboutAlbum(IDictionary<string, List<string>> candidates, string folderName)
        {
            return _viewModel.Album;
        }

        public string AboutArtist(Artist artist)
        {
            return _viewModel.Artist;
        }

        public int SelectFromList(string title, IEnumerable<string> menuItems)
        {
            throw new NotImplementedException();
        }
    }
}