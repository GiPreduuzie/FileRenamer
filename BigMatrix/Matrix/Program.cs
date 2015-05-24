using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix
{
    class Element
    {
        Matrix _matrix;

        public Element(Matrix matrix, int i, int j)
        {
            _matrix = matrix;
            I = i;
            J = j;
        }

        public int I { get; private set; }
        public int J { get; private set; }

        public int GetValue()
        {
            return _matrix[I, J]; 
        }

        public void Set(int value)
        {
            _matrix[I, J] = value;
        }

        public void Read(IReader reader)
        {
            Set(reader.Get(I, J));
        }
    }

    class Matrix
    {
        int[,] _matrix;
        private int _n;
        private int _m;

        public Matrix(int n, int m)
        {
            _matrix = new int[n, m];
            _n = n;
            _m = m;
        }

        public int N { get { return _n; } }
        public int M { get { return _m; } }

        public int this [int i, int j]
        {
            get
            {
                return _matrix[i, j];
            }
            set
            {
                _matrix[i, j] = value;
            }
        }

        public Matrix Transpose()
        {
            return Matrix.Read(GetTransposeReader());
        }

        public static Matrix Read(IReader reader)
        {
            var matrix = new Matrix(reader.GetN(), reader.GetM());
            matrix.Fill(reader);
            return matrix;
        }

        public void Fill(IReader reader)
        {
            Iterate(x => x.Read(reader), i => { });
        }

        public void Print(IWriter writer)
        {
            writer.WriteLine();
            Iterate(x => writer.Write(x.GetValue() + "\t"), i => writer.WriteLine());
            writer.WriteLine();
        }

        private void Iterate(Action<Element> toHandleElement, Action<int> toHandleString)
        {
            for (int i = 0; i < _n; i++)
            {
                for (int j = 0; j < _m; j++)
                {
                    toHandleElement(new Element(this, i, j));
                }
                toHandleString(i);
            }
        }

        public IReader GetReader()
        {
            return new MatrixReader(this);
        }

        public IReader GetTransposeReader()
        {
            return new TrasposeReader(GetReader());
        }
    }

    interface IReader
    {
        int GetN();
        int GetM();
        int Get(int i, int j);
    }

    class TrasposeReader : IReader
    {
        IReader _reader;

        public TrasposeReader(IReader reader)
        {
            _reader = reader;        
        }

        public int GetN()
        {
            return _reader.GetM();
        }

        public int GetM()
        {
            return _reader.GetN();
        }

        public int Get(int i, int j)
        {
            return _reader.Get(j, i);
        }
    }

    class MatrixReader : IReader
    {
        Matrix _matrix;

        public MatrixReader(Matrix matrix)
        {
            _matrix = matrix;
        }

        public int GetN()
        {
            return _matrix.N;
        }

        public int GetM()
        {
            return _matrix.M;
        }

        public int Get(int i, int j)
        {
            return _matrix[i, j];
        }
    }

    class Reader : IReader
    {
        public int GetN ()
        {
            Console.WriteLine("Input n:");
            return Get();
        }

        public int GetM()
        {
            Console.WriteLine("Input m:");
            return Get();
        }

        public int Get(int i, int j)
        {
            Console.Write("[{0},{1}]:", i, j);
            return Get();
        }

        private int Get()
        {
            return int.Parse(Console.ReadLine());
        }
    }

    class FileReader : IReader
    {
        string _fileName;
        int[][] _content;
        int _n;
        int _m;

        public FileReader(string fileName)
        {
            _fileName = fileName;

            var content = File.ReadAllLines(fileName);

            _content =
                content.Skip(1).TakeWhile(x => !string.IsNullOrEmpty(x))
                    //.Select(x => x.Aggregate(new List<string>() {"" }, (result, item) =>
                    //{
                    //    if (item == '\t')
                    //    {
                    //        result.Add("");
                    //        return result;
                    //    }
                    //    result[result.Count -1] += item;
                    //    return result;
                    //}))
                    .Select(x => x.Split(new[] { '\t'}, StringSplitOptions.RemoveEmptyEntries))
                    .Select(y => y.Select(item => int.Parse(item)))
                    .Select(x => x.ToArray())
                    .ToArray();

            _n = _content.Length;
            _m = _content[0].Length;
        }

        public int GetN()
        {
            return _n;
        }

        public int GetM()
        {
            return _m;
        }

        public int Get(int i, int j)
        {
            return _content[i][j];
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var matrix = Matrix.Read(new FileReader(@"c:\matrix\1.txt"));
            var transposed = matrix.Transpose();
            var writer = new CompositeWriter(new ConsoleWriter(), new FileWriter(@"c:\matrix\1.txt"));
            matrix.Print(writer);
            transposed.Print(writer);
        }
    }
}
