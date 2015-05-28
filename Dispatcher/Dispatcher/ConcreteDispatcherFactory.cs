namespace Dispatcher
{
    internal class ConcreteDispatcherFactory
    {
        public ICellVisitor<ICellVisitor<string>> CreateDispatcher()
        {
            return
                new PrototypeFactory<string>(Do)
                    .TakeRed.WithRed(Do)
                    .TakeGreen.WithBlue(Do);
        }

        private string Do(ICell a, ICell b)
        {
            return a.Color + "\t-->\t" + b.Color;
        }

        private string Do(GreenCell a, BlueCell b)
        {
            return "побережье";
        }

        private string Do(RedCell a, RedCell b)
        {
            return "красное на красном";
        }
    }
}