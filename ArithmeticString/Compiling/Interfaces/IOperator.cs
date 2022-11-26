using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling.Interfaces
{
    public interface IOperator<T>
    {
        T Calculate(T x, T y);
    }
}
