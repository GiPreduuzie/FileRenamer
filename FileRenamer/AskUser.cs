using System;
using System.Collections.Generic;

namespace FileRenamer
{
    class AskUser : IAskUser
    {
        public int AboutTrack(FileUpdater fileUpdater)
        {
            Console.WriteLine(string.Format("Cannot parse track number of '{0}({1})'. Help me!", fileUpdater.FileNamingModel.Song, fileUpdater.FileNamingModel.File));
            return GetNumber();
        }

        public string AboutAlbum(IDictionary<string, List<FileUpdater>> candidates, string candidate)
        {
            Console.WriteLine("Help me to select album name.");
            Console.WriteLine("Folder is:\t" + candidate);
            foreach (var c in candidates)
            {
                Console.WriteLine();
                Console.WriteLine("Album tag is:\t" + c.Key);
                Console.WriteLine("And songs with this album tag:");
                foreach(var fileUpdater in c.Value)
                {
                    Console.WriteLine("| " + fileUpdater.FileNamingModel.Song);
                }
                
            }
            Console.WriteLine();
            Console.WriteLine("So, what do you like?");
            return Console.ReadLine();
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
            Console.WriteLine(title);
            var i = 1;
            foreach (var item in menuItems)
            {
                Console.WriteLine(i++ + ". " + item);
            }

            var value = GetNumber(1, menuItems.Count) - 1;
            Console.WriteLine("You have selected " + value + " : " + menuItems[value]);
            return value;
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
            Console.WriteLine("Something is not clear for this directory: " + directory.Location);
            Console.WriteLine("Do you want to ignore it?");
            if (YesNoQuestion())
            {
                directory.Ignore();
                return;
            }

            directory.AskQuestions(this);
        }

        internal void AboutArtist(Artist artist)
        {
            Console.WriteLine("Cannot decide which artist should be used for these directories...");
            foreach( var directory in artist.Directories)
            {
                Console.WriteLine("|" + directory.Location + " with artist-> '" + directory.Artist + "'");
            }

            Console.WriteLine("Can you select one artist for all of them?");
            if (YesNoQuestion())
            {
                artist.SetArtist(Console.ReadLine());
            }
        }
    }
}