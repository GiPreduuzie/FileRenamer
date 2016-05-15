using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneMoreDispatcher
{
    internal interface ICell
    {
        T AcceptVisitor<T>(ICellVisitor<T> visitor);
    }

    internal interface ICellVisitor<out T>
    {
        T Visit(RedCell cell);

        T Visit(BlueCell cell);

        T Visit(GreenCell cell);
    }

    internal class RedCell : ICell
    {
        public string Color
        {
            get { return "red"; }
        }

        public T AcceptVisitor<T>(ICellVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class BlueCell : ICell
    {
        public string Color
        {
            get { return "blue"; }
        }

        public T AcceptVisitor<T>(ICellVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class GreenCell : ICell
    {
        public string Color
        {
            get { return "green"; }
        }

        public T AcceptVisitor<T>(ICellVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
