using System.IO;
using System.Linq;

namespace FileRenamer
{
    class DirectoryName
    {
        private readonly string _value;
        public DirectoryName(string value)
        {
            _value = value;

            var directories = value.Split(Path.DirectorySeparatorChar).Reverse().Take(2).ToList();
            Album = directories[0];
            Artist = directories[1];
        }

        public string Album { get; private set; }
        public string Artist { get; private set; }
    }
}