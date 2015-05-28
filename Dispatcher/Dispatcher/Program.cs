using System;
using System.Collections.Generic;

namespace Dispatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var targets = new List<ICell>
            {
                new RedCell(1),
                new GreenCell(),
                new BlueCell()
            };

            var sources = new List<ICell>
            {
                new RedCell(2),
                new GreenCell(),
                new BlueCell()
            };


            foreach (var target in targets)
                foreach (var source in sources)
                    Map(target, source);

            Console.ReadKey();
        }

        private static void Map(ICell target, ICell source)
        {
            var dispatcher = new ConcreteDispatcherFactory().CreateDispatcher();
            var result = source.AcceptVisitor(target.AcceptVisitor(dispatcher));
            Console.WriteLine(result);
        }
    }
}
