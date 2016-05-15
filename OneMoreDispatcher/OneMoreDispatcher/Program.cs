using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneMoreDispatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var colors = new List<ICell> { new RedCell(), new GreenCell(), new BlueCell() };

            var visitor = new ConcreteDispatcherFactory().CreateDispatcher();
            foreach (var a in colors)
                foreach (var b in colors)
                    Console.WriteLine(b.AcceptVisitor(a.AcceptVisitor(visitor)));
        }
    }
}
