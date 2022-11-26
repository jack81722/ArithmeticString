using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling.Interfaces
{
    public interface IParser<T>
    {
        T Parse(string str);
    }
}
