using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling.Nodes
{
    public sealed class ValueNode<T> : Node<T>
    {
        private T _value;

        public ValueNode(T value)
        {
            _value = value;
        }

        public override T Solve(IEquationContext<T> ctx = null)
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
