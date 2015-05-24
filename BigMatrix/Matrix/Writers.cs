using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix
{
    interface IWriter
    {
        void Write(string value);
        void WriteLine();
    }

    class FileWriter : IWriter
    {
        string _fileName;

        public FileWriter(string fileName)
        {
            _fileName = fileName;

            using (new StreamWriter(fileName, false))
            { }
        }

        public void Write(string value)
        {
            using (var writer = new StreamWriter(_fileName, true))
            {
                writer.Write(value);
            }
        }

        public void WriteLine()
        {
            using (var writer = new StreamWriter(_fileName, true))
            {
                writer.WriteLine();
            }
        }
    }

    class ConsoleWriter : IWriter
    {
        public void Write(string value)
        {
            Console.Write(value);
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }
    }

    class CompositeWriter : IWriter
    {
        IWriter[] _writers;

        public CompositeWriter(params IWriter[] writers)
        {
            _writers = writers;
        }

        public void Write(string value)
        {
            foreach (var writer in _writers)
                writer.Write(value);
        }

        public void WriteLine()
        {
            foreach (var writer in _writers)
                writer.WriteLine();
        }
    }
}
