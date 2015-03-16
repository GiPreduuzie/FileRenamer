using System;
using System.Collections.Generic;
using System.Linq;

namespace FileRenamer
{
    public interface IPatternSelector
    {
        void AddPattern(INamePattern namePattern);
        INamePattern Select();
    }


    public class PatternSelector : IPatternSelector
    {
        readonly IList<INamePattern> _patterns;
        readonly AskUser _askUser;

        public PatternSelector(AskUser askUser)
        {
            _askUser = askUser;
            _patterns = new List<INamePattern>();
        }

        public void AddPattern(INamePattern namePattern)
        {
            _patterns.Add(namePattern);
        }

        public INamePattern Select()
        {
            return _patterns[_askUser.SelectItem("Select a pattern", _patterns.Select(item => item.PatternString).ToList())];
        }
    }

    public class Binder
    {
        private IPatternSelector _patternSelector;

        public Binder(IPatternSelector patternSelector)
        {
            _patternSelector = patternSelector;
        }

        public IExplorer GetExplorer()
        {
            return CreateDirectoryProvider();
        }

        public Explorer GetRenamer()
        {
            return new Explorer(CreateDirectoryProvider());
        }

        public IPatternSelector GetPatternSelector()
        {
            _patternSelector.AddPattern(new FirstPattern());
            _patternSelector.AddPattern(new SecondPattern());
            _patternSelector.AddPattern(new ThirdPattern());
            _patternSelector.AddPattern(new GeneralPattern());

            return _patternSelector;
        }

        private DirectoryProvider CreateDirectoryProvider()
        {
            return new DirectoryProvider(new SimpleFileEnumerator(), new SongDirectoryConstructor());
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input path to root folder:");
            var pathToRoot = Console.ReadLine();

            var userAsker = new AskUser(new ConsoleUserAsker());
            var patternSelector = new Binder(new PatternSelector(userAsker)).GetPatternSelector();
            
            new Explorer(new DirectoryProvider(new FileEnumerator(), new SongDirectoryConstructor())).Explore(pathToRoot, true, patternSelector.Select(), userAsker);
        }
    }
}
