using ArithmeticString.Compiling.Interfaces;
using System;

namespace ArithmeticString.Compiling.Operators
{
    public class GenericFuncFunction<T> : IFunction<T>
    {
        private Func<T[], T> _func;

        public GenericFuncFunction(Func<T[], T> func)
        {
            _func = func;
        }

        public T Calculate(T[] parameters)
        {
            return _func(parameters);
        }
    }
}
