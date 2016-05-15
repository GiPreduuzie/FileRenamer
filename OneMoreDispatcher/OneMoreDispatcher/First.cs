using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneMoreDispatcher
{
    class PrototypeDispatcher<TResult>
    {
        private readonly Builder<TResult, RedCell, PrototypeDispatcher<TResult>> _redBuilder;
        private readonly Builder<TResult, GreenCell, PrototypeDispatcher<TResult>> _greenBuilder;
        private readonly Builder<TResult, BlueCell, PrototypeDispatcher<TResult>> _blueBuilder;

        public PrototypeDispatcher(Func<ICell, ICell, TResult> generalCase)
        {
            _redBuilder =   CreateBuilder<RedCell,   PrototypeDispatcher<TResult>>(generalCase, this);
            _blueBuilder =  CreateBuilder<BlueCell,  PrototypeDispatcher<TResult>>(generalCase, this);
            _greenBuilder = CreateBuilder<GreenCell, PrototypeDispatcher<TResult>>(generalCase, this);
        }

        private 
            
            BaseBuilder
              <ActionTaker<TA, RedCell, TResult, TReturn>, 
               ActionTaker<TA, BlueCell, TResult, TReturn>,
               ActionTaker<TA, GreenCell, TResult, TReturn>,
               ICellVisitor<TResult>>
            
            CreateBuilder<TA, TReturn>(
            Func<ICell, ICell, TResult> general,
            TReturn returnObject)
            where TA : ICell
        {
            //return new Builder<TResult, TA, TReturn>(
            //    new ActionTaker<TA, RedCell,   TResult, TReturn>(general, returnObject),
            //    new ActionTaker<TA, BlueCell,  TResult, TReturn>(general, returnObject),
            //    new ActionTaker<TA, GreenCell, TResult, TReturn>(general, returnObject));

            var x = new ActionTaker<TA, RedCell,   TResult, TReturn>(general, returnObject);
            var y = new ActionTaker<TA, BlueCell,  TResult, TReturn>(general, returnObject);
            var z = new ActionTaker<TA, GreenCell, TResult, TReturn>(general, returnObject);

            ICellVisitor<TResult> res = new Visitor<TResult>(
              b => x.Build(a, b),
              b => y.Build(a, b),
              b => z.Build(a, b));

              return new BaseBuilder
                <ActionTaker<TA, RedCell, TResult, TReturn>,
                 ActionTaker<TA, BlueCell, TResult, TReturn>,
                 ActionTaker<TA, GreenCell, TResult, TReturn>,
                 ICellVisitor<TResult>>
                 (x, y, z, res);
        }

        public Builder<TResult, RedCell, PrototypeDispatcher<TResult>> TakeRed
        {
            get { return _redBuilder; }
        }

        public Builder<TResult, BlueCell, PrototypeDispatcher<TResult>> TakeBlue
        {
            get { return _blueBuilder; }
        }

        public Builder<TResult, GreenCell, PrototypeDispatcher<TResult>> TakeGreen
        {
            get { return _greenBuilder; }
        }

        public ICellVisitor<ICellVisitor<TResult>> Build()
        {
            return new Visitor<ICellVisitor<TResult>>(
                _redBuilder.Build,
                _blueBuilder.Build,
                _greenBuilder.Build);
        }
    }
}
