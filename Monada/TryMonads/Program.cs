using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TryMonads
{
    class A
    {
        public A(IEnumerable<B> bs)
        {
            Bs = bs;
        }

        public IEnumerable<B> Bs { get; private set; }
    }

    class B
    {
        public B(IEnumerable<C> cs)
        {
            Cs = cs;
        }

        public IEnumerable<C> Cs { get; private set; } 
    }

    class C
    {
        private static int _number = 0;

        public C()
        {
            Id = _number;
            Console.WriteLine("Create c: " + _number++);
        }

        public int Id { get; private set; }
    }

    public interface IMonad<TMonad, out T>
    {
        TMonad Create(TOut value);
        TMonad Apply(Func<T, TMonad<TOut>> toApply);
    }

    public class MHelper<T, TOut>
    {
        private readonly Func<Func<T, TOut>, TOut> _func;

        public MHelper(Func<Func<T, TOut>, TOut> func)
        {
            _func = func;
        }

        public TOut Apply(Func<T, TOut> toApply)
        {
            return _func(toApply);
        }
    }

    public class LazyMonad<T>
    {
        private readonly Func<T> _toCreate;

        public LazyMonad(Func<T> toCreate)
        {
            _toCreate = toCreate;
        }

        public LazyMonad<TOut> Create<TOut>(TOut value)
        {
            return new LazyMonad<TOut>(() => value);
        }

        public LazyMonad<TOut> Apply<TOut>(Func<T, TOut> toApply)
        {
            return Apply(x => Create(toApply(x)));
        }
    
        public LazyMonad<TOut> Apply<TOut>(Func<T, LazyMonad<TOut>> toApply)
        {
            return new MHelper<T, LazyMonad<TOut>>(x => new LazyMonad<TOut>(() => x(Resolve()).Resolve())).Apply(toApply);
        }

        public T Resolve()
        {
            return _toCreate();
        }
    }

    //public class SelectModad<T>
    //{
    //    private readonly IEnumerable<T> _items;

    //    public SelectModad(T item)
    //    {
    //        _items = new List<T>{item};
    //    }

    //    public SelectModad<TOut> Apply<TOut>(Func<T, TOut> toApply)
    //    {
    //        return Apply(x => new SelectModad<TOut>(toApply(x)));
    //    }

    //    public SelectModad<TOut> Apply<TOut>(Func<T, SelectModad<TOut>> toApply)
    //    {
    //        var result = new List<TOut>();
    //        foreach (var item in _items)
    //        {
    //            result.AddRange(toApply(item).Resolve());
    //        }

    //        return new SelectModad<TOut>(result);
    //    }

    //    public IEnumerable<T> Resolve()
    //    {
    //        return _items;
    //    }
    //}


    public class SomeMonad<T> where T : class
    {
        private readonly T _value;

        public SomeMonad(T value)
        {
            _value = value;
        }

        public SomeMonad<TOut> Apply<TOut>(Func<T, TOut> toApply) where TOut : class
        {
            return Apply(x => new SomeMonad<TOut>(toApply(x)));
        }

        public SomeMonad<TOut> Apply<TOut>(Func<T, SomeMonad<TOut>> toApply) where TOut : class
        {
            return _value == null ? new SomeMonad<TOut>(null) : toApply(_value);
        }

        public T Resolve(T defaultValue)
        {
            return _value ?? defaultValue;
        }
    }

    public static class GeneralExtensions
    {
        public static SomeMonad<T> Some<T>(this IEnumerable<T> items) where T: class
        {
            var value = items.FirstOrDefault();
            if (value == null)
                return new SomeMonad<T>(null);
            return new SomeMonad<T>(value);
        }
    }

    public static class Monads
    {
        public static SomeMonad<TOut> Apply<T1, T2, TOut>(
            SomeMonad<T1> m1,
            SomeMonad<T2> m2,
            Func<T1, T2, TOut> toApply)
            where T1 : class
            where T2 : class
            where TOut : class 
        {
            return
                m1.Apply(x =>
                    m2.Apply(y => toApply(x, y)));
        }

        public static SomeMonad<TOut> Apply<T1, T2, T3, TOut>(
            SomeMonad<T1> m1,
            SomeMonad<T2> m2,
            SomeMonad<T3> m3,
            Func<T1, T2, T3, TOut> toApply)
            where T1 : class
            where T2 : class
            where T3 : class
            where TOut : class
        {
            return
                m1.Apply(x =>
                    m2.Apply(y =>
                        m3.Apply(z => toApply(x, y, z))));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var m1 = new LazyMonad<C>(() => new C())
                .Apply(x => x.Id + 1)
                .Apply(x => x*x)
                .Apply(x => x.ToString() + ";");

            var m2 = new LazyMonad<C>(() => new C())
                .Apply(x => x.Id + 1)
                .Apply(x => x*x)
                .Apply(x => x.ToString() + ";")
                .Apply(x => 
                    m1.Apply(y => x + y));


            Console.WriteLine("Before resolve;");
            Console.WriteLine(m2.Resolve());


            //var a = new A(new List<B>() {new B(new List<C> {new C(), new C()}), new B(new List<C> {new C(), new C()})});


            //a.Bs.Some().Apply(x => x.Cs.Some().Apply(y => y)).Resolve(new C());

            //var aFirst = a.Bs.Some();
            //var aSecond = a.Bs.Some();

            //aFirst.Apply(x =>
            //    aSecond.Apply(y => string.Format("x:{0}; y:{1}", x, y)));

            //Monads.Apply(aFirst, aSecond, (x, y) => string.Format("x:{0}; y:{1}", x, y)).Resolve(string.Empty);
        }
    }
}
