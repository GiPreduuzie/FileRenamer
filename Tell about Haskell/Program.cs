using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication10
{
    class SomeWrapper <T>
    {
        private readonly T _item;

        public SomeWrapper(T item)
        {
            _item = item;
        }
    }

    interface Factory<T<_>>
    {
        T<TValue> Create<TValue>(TValue value);
    }
    
    class ListFactory : Factory<List>
    {
        public List<TValue> Create<TValue>(TValue value)
        {
            return new List(value);
        }
    }
    
    class SomeWrapperFactory : Factory<SomeWrapper>
    {
        public SomeWrapper<TValue> Create<TValue>(TValue value)
        {
            return new SomeWrapper(value);
        }
    }
    
    class SomeClass<T<_>> 
    {
        Factory<T> _factory;
    
        public SomeClass(Factory<T> factory)
        {
            _factory = factory;
        }
    
        public Do<TNew>(TNew value)
        {
            factory.Create<TNew>(value);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var someClass = new SomeClass<List>(new ListFactory());

            List<int> l1 = someClass.Do(1);
            List<String> l2 = someClass.Do("2");

            var someClass2 = new SomeClass<SomeWrapper>(new SomeWrapperFactory());

            SomeWrapper<Int> s1 = someClass2.Do(1);
            SomeWrapper<String> s2 = someClass2.Do("2");

            someClass.Do("2");
        }
    }
}
