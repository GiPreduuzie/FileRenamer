using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneMoreDispatcher
{
    internal class ConcreteDispatcherFactory
    {
        public ICellVisitor<ICellVisitor<string>> CreateDispatcher()
        {
            return
                new PrototypeDispatcher<string>(Do)
                    .TakeRed.WithRed.Do(Do)
                    .TakeGreen.WithBlue.Do(Do)
                    .Build();
        }

        private string Do(ICell a, ICell b)
        {
            var colorRetriever = new ColorRetriever();
            var aColor = a.AcceptVisitor(colorRetriever);
            var bColor = b.AcceptVisitor(colorRetriever);

            return aColor + "\t-->\t" + bColor;
        }

        private string Do(GreenCell a, BlueCell b)
        {
            return "green to blue";
        }

        private string Do(RedCell a, RedCell b)
        {
            return "red to red";
        }
    }

    internal class ColorRetriever : ICellVisitor<string>
    {
        public string Visit(RedCell cell)
        {
            return cell.Color;
        }

        public string Visit(BlueCell cell)
        {
            return cell.Color;
        }

        public string Visit(GreenCell cell)
        {
            return cell.Color;
        }
    }
}
