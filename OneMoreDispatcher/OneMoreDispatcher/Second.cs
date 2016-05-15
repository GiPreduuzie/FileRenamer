using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneMoreDispatcher
{
    internal class BaseBuilder<TRed, TBlue, TGreen, TResult>
    {
        TResult _toDo;

        public BaseBuilder(TRed red, TBlue blue, TGreen green, TResult toDo)
        {
            Red = red;
            Blue = blue;
            Green = green;
            _toDo = toDo;
        }

        public TRed Red { get; private set; }

        public TBlue Blue { get; private set; }

        public TGreen Green { get; private set; }

        public TResult Build()
        {
            return _toDo;
        }
    }

    internal class Visitor<TResult> : ICellVisitor<TResult>
    {
        Func<RedCell, TResult> _takeRed;
        Func<BlueCell, TResult> _takeBlue;
        Func<GreenCell, TResult> _takeGreen;

        public Visitor(
            Func<RedCell, TResult> takeRed,
            Func<BlueCell, TResult> takeBlue,
            Func<GreenCell, TResult> takeGreen)
        {
            _takeRed = takeRed;
            _takeBlue = takeBlue;
            _takeGreen = takeGreen;
        }

        public TResult Visit(RedCell cell)
        {
            return _takeRed(cell);
        }

        public TResult Visit(BlueCell cell)
        {
            return _takeBlue(cell);
        }

        public TResult Visit(GreenCell cell)
        {
            return _takeGreen(cell);
        }
    }

    internal class ActionTaker<TA, TB, TResult, TReturn> 
        where TA : ICell
        where TB : ICell
    {
        private TReturn _returnObject;
        private Func<TA, TB, TResult> _toDo;

        public ActionTaker(Func<ICell, ICell, TResult> general, TReturn returnObject)
        {
            _returnObject = returnObject;
            _toDo = (a, b) => general(a, b);
        }

        public TReturn Do(Func<TA, TB, TResult> toDo)
        {
            _toDo = toDo;
            return _returnObject;
        }

        public Func<TA, TB, TResult> Build
        {
            get
            {
                return _toDo;
            }
        }
    }

    internal class Builder<TResult, TA, TReturn> where TA : ICell
    {
        private ActionTaker<TA, RedCell, TResult, TReturn> _takeRed;
        private ActionTaker<TA, BlueCell, TResult, TReturn> _takeBlue;
        private ActionTaker<TA, GreenCell, TResult, TReturn> _takeGreen;

        public Builder(
              ActionTaker<TA, RedCell, TResult, TReturn> takeRed,
              ActionTaker<TA, BlueCell, TResult, TReturn> takeBlue,
              ActionTaker<TA, GreenCell, TResult, TReturn> takeGreen)
        {
            _takeRed = takeRed;
            _takeBlue = takeBlue;
            _takeGreen = takeGreen;
        }

        public ActionTaker<TA, RedCell, TResult, TReturn> WithRed
        {
            get
            {
                return _takeRed;
            }
        }

        public ActionTaker<TA, BlueCell, TResult, TReturn> WithBlue
        {
            get
            {
                return _takeBlue;
            }
        }

        public ActionTaker<TA, GreenCell, TResult, TReturn> WithGreen
        {
            get
            {
                return _takeGreen;
            }
        }

        public ICellVisitor<TResult> Build(TA a)
        {
            return new Visitor<TResult>(
                b => _takeRed.Build(a, b),
                b => _takeBlue.Build(a, b),
                b => _takeGreen.Build(a, b));
        }
    }
}
