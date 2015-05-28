using System;

namespace Dispatcher
{
    internal interface IBuilder<TResult, out TA>
    {
        PrototypeFactory<TResult> WithRed(Func<TA, RedCell, TResult> toDo);
        PrototypeFactory<TResult> WithBlue(Func<TA, BlueCell, TResult> toDo);
        PrototypeFactory<TResult> WithGreen(Func<TA, GreenCell, TResult> toDo);
    }

    internal class Builder<TResult, TA> : IBuilder<TResult, TA>, ICellVisitor<TResult> where TA : ICell
    {
        private Func<TA, RedCell, TResult> _takeRed;
        private Func<TA, BlueCell, TResult> _takeBlue;
        private Func<TA, GreenCell, TResult> _takeGreen;
        private readonly Func<ICell, ICell, TResult> _generalCase;

        private readonly PrototypeFactory<TResult> _factory;
        private TA _target;

        public Builder(PrototypeFactory<TResult> factory, Func<ICell, ICell, TResult> generalCase)
        {
            _factory = factory;
            _generalCase = generalCase;

            _takeRed = (a, b) => _generalCase(a, b);
            _takeBlue = (a, b) => _generalCase(a, b);
            _takeGreen = (a, b) => _generalCase(a, b);
        }

        public PrototypeFactory<TResult> WithRed(Func<TA, RedCell, TResult> toDo)
        {
            _takeRed = toDo;
            return _factory;
        }

        public PrototypeFactory<TResult> WithBlue(Func<TA, BlueCell, TResult> toDo)
        {
            _takeBlue = toDo;
            return _factory;
        }

        public PrototypeFactory<TResult> WithGreen(Func<TA, GreenCell, TResult> toDo)
        {
            _takeGreen = toDo;
            return _factory;
        }

        public TResult Visit(RedCell cell)
        {
            return _takeRed(_target, cell);
        }

        public TResult Visit(BlueCell cell)
        {
            return _takeBlue(_target, cell);
        }

        public TResult Visit(GreenCell cell)
        {
            return _takeGreen(_target, cell);
        }

        public ICellVisitor<TResult> Take(TA a)
        {
            _target = a;
            return this;
        }
    }

    class PrototypeFactory<TResult> : ICellVisitor<ICellVisitor<TResult>>
    {
        private readonly Builder<TResult, RedCell> _redBuilder;
        private readonly Builder<TResult, GreenCell> _greenBuilder;
        private readonly Builder<TResult, BlueCell> _blueBuilder;

        public PrototypeFactory(Func<ICell, ICell, TResult> generalCase)
        {
            _redBuilder = new Builder<TResult, RedCell>(this, generalCase);
            _blueBuilder = new Builder<TResult, BlueCell>(this, generalCase);
            _greenBuilder = new Builder<TResult, GreenCell>(this, generalCase);
        }

        public IBuilder<TResult, RedCell> TakeRed
        {
            get { return _redBuilder; }
        }

        public IBuilder<TResult, BlueCell> TakeBlue
        {
            get { return _blueBuilder; }
        }

        public IBuilder<TResult, GreenCell> TakeGreen
        {
            get { return _greenBuilder; }
        }

        public ICellVisitor<TResult> Visit(RedCell cell)
        {
            return _redBuilder.Take(cell);
        }

        public ICellVisitor<TResult> Visit(BlueCell cell)
        {
            return _blueBuilder.Take(cell);
        }

        public ICellVisitor<TResult> Visit(GreenCell cell)
        {
            return _greenBuilder.Take(cell);
        }
    }
}
