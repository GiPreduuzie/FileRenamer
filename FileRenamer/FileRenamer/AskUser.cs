using System;
using System.Collections.Generic;
using System.Linq;

namespace FileRenamer
{
    public interface IUserAsker
    {

        bool IgnoreDirectory(string directoryLocation);
        string AboutAlbum(IDictionary<string, List<string>> candidates, string folderName);
        string AboutArtist(Artist artist);
        int SelectFromList(string title, IEnumerable<string> menuItems);
    }

    public class ConsoleUserAsker : IUserAsker
    {
        public bool IgnoreDirectory(string directoryLocation)
        {
            Console.WriteLine("Something is not clear for this directory: " + directoryLocation);
            Console.WriteLine("Do you want to ignore it?");
            return YesNoQuestion();
        }

        public string AboutAlbum(IDictionary<string, List<string>> candidates, string folderName)
        {
            Console.WriteLine("Help me to select album name.");
            Console.WriteLine("Folder is:\t" + folderName);
            foreach (var candidate in candidates)
            {
                Console.WriteLine();
                Console.WriteLine("Album tag is:\t" + candidate.Key);
                Console.WriteLine("And songs with this album tag:");
                foreach (var song in candidate.Value)
                {
                    Console.WriteLine("| " + song);
                }

            }
            Console.WriteLine();
            Console.WriteLine("So, what do you like?");
            return Console.ReadLine();
        }

        public string AboutArtist(Artist artist)
        {
            Console.WriteLine("Cannot decide which artist should be used for these directories...");
            foreach (var directory in artist.Directories)
            {
                Console.WriteLine("|" + directory.Location + " with artist-> '" + directory.Artist + "'");
            }

            Console.WriteLine("Can you select one artist for all of them?");
            if (YesNoQuestion())
            {
                return Console.ReadLine();
            }

            return string.Empty;
        }

        public int SelectFromList(string title, IEnumerable<string> menuItems)
        {
            var menuList = menuItems.ToList();
            Console.WriteLine(title);
            var i = 1;
            foreach (var item in menuList)
            {
                Console.WriteLine(i++ + ". " + item);
            }

            var value = GetNumber(1, menuList.Count) - 1;
            Console.WriteLine("You have selected " + value + " : " + menuList[value]);
            return value;
        }

        private bool YesNoQuestion()
        {
            Console.WriteLine("(ä/í)...");
            var input = Console.ReadLine();
            while (input != "ä" && input != "í")
            {
                Console.WriteLine("You should answer 'ä' or 'í'");
                input = Console.ReadLine();
            }

            return input == "ä";
        }


        private int GetNumber(int minValue, int maxValue)
        {
            var input = Console.ReadLine();
            int number;
            while (!(int.TryParse(input, out number) && (number <= maxValue) && (number >= minValue)))
            {
                Console.WriteLine("I want a number from " + minValue + " to " + maxValue);
                input = Console.ReadLine();
            }

            return number;
        }

    }



    public class AskUser : IAskUser
    {
        IUserAsker _userAsker;

        public AskUser(IUserAsker userAsker)
        {
            _userAsker = userAsker;
        }

        public int AboutTrack(FileUpdater fileUpdater)
        {
            Console.WriteLine(string.Format("Cannot parse track number of '{0}({1})'. Help me!", fileUpdater.FileNamingModel.Song, fileUpdater.FileNamingModel.File));
            return GetNumber();
        }

        public string AboutAlbum(IDictionary<string, List<string>> candidates, string folderName)
        {
            return _userAsker.AboutAlbum(candidates, folderName);
        }

        public string AboutArtist(Dictionary<string, List<FileUpdater>> candidates, string candidate)
        {
            Console.WriteLine("Help me to select artist name.");
            Console.WriteLine("Folder is:\t" + candidate);
            foreach (var c in candidates)
            {
                Console.WriteLine();
                Console.WriteLine("Album tag is:\t" + c.Key);
                Console.WriteLine("And songs with this artist tag:");
                foreach (var fileUpdater in c.Value)
                {
                    Console.WriteLine("| " + fileUpdater.FileNamingModel.Song);
                }

            }
            Console.WriteLine();
            Console.WriteLine("So, what do you like?");
            return Console.ReadLine();
        }

        public int SelectItem(string title, IList<string> menuItems)
        {
            return _userAsker.SelectFromList(title, menuItems);
        }

        private int GetNumber(int minValue, int maxValue)
        {
            var input = Console.ReadLine();
            int number;
            while (!(int.TryParse(input, out number) && (number <= maxValue) && (number >= minValue)))
            {
                Console.WriteLine("I want a number from " + minValue + " to " + maxValue);
                input = Console.ReadLine();
            }

            return number;
        }


        private int GetNumber()
        {
            var input = Console.ReadLine();
            int number;
            while (!int.TryParse(input, out number))
            {
                Console.WriteLine("I want a number!");
                input = Console.ReadLine();
            }

            return number;
        }

       

        private bool YesNoQuestion()
        {
            Console.WriteLine("(ä/í)...");
            var input = Console.ReadLine();
            while (input != "ä" && input != "í")
            {
                Console.WriteLine("You should answer 'ä' or 'í'");
                input = Console.ReadLine();
            }

            return input == "ä";
        }

        internal void AboutDirectory(SongDirectory directory)
        {
            if (_userAsker.IgnoreDirectory(directory.Location))
            {
                directory.Ignore();
                return;
            }

            directory.AskQuestions(this);
        }

        internal void AboutArtist(Artist artist)
        {
            var selectedArtist = _userAsker.AboutArtist(artist);
            if (string.IsNullOrWhiteSpace(selectedArtist))
                return;

            artist.SetArtist(selectedArtist);
        }
    }
}