using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling.Interfaces
{
    public interface IDecorator<T>
    {
        T Decorate(T x);
    }
}
