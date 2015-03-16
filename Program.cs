using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FileRenamer
{
    internal class PatternSelector
    {
        IList<INamePattern> _patterns;
        AskUser _askUser;

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


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input path to root folder:");
            var pathToRoot = Console.ReadLine();
            var userAsker = new AskUser();
            var patternSelector = new PatternSelector(userAsker);
            patternSelector.AddPattern(new FirstPattern());
            patternSelector.AddPattern(new SecondPattern());
            patternSelector.AddPattern(new ThirdPattern());
            patternSelector.AddPattern(new GeneralPattern());
            new Explorer(pathToRoot).Explore(true, patternSelector.Select(), userAsker);
        }
    }
}
