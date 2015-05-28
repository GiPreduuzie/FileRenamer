namespace Dispatcher
{
    internal class Cell
    {
        public Cell(string color)
        {
            Color = color;
        }

        public string Color { get; private set; }
    }

    internal interface ICell
    {
        string Color { get; }

        T AcceptVisitor<T>(ICellVisitor<T> visitor);
    }

    internal class RedCell : ICell
    {
        public RedCell(int identifier)
        {
            Identifier = identifier;
        }

        public string Color
        {
            get { return "красный"; }
        }

        public int Identifier { get; private set; }

        public T AcceptVisitor<T>(ICellVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal class BlueCell : ICell
    {
        public string Color
        {
            get { return "синий"; }
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
            get { return "зелёный"; }
        }

        public T AcceptVisitor<T>(ICellVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }

    internal interface ICellVisitor<out T>
    {
        T Visit(RedCell cell);

        T Visit(BlueCell cell);

        T Visit(GreenCell cell);
    }
}
