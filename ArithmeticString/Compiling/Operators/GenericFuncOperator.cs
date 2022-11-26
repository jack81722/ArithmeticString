using ArithmeticString.Compiling.Interfaces;
using System;

namespace ArithmeticString.Compiling.Operators
{
    public class GenericFuncOperator<T> : IOperator<T>
    {
        private Func<T, T, T> _opFunc;

        public GenericFuncOperator(Func<T, T, T> opFunc)
        {
            _opFunc = opFunc;
        }

        public T Calculate(T x, T y)
        {
            return _opFunc(x, y);
        }
    }
}
