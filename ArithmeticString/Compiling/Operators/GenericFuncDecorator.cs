using ArithmeticString.Compiling.Interfaces;
using System;

namespace ArithmeticString.Compiling.Operators
{
    public class GenericFuncDecorator<T> : IDecorator<T>
    {
        private Func<T, T> _decorator;

        public GenericFuncDecorator(Func<T, T> decorator)
        {
            _decorator = decorator;
        }

        public T Decorate(T x)
        {
            return _decorator(x);
        }
    }
}
