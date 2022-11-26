using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling.Nodes
{
    public sealed class VariableNode<T> : Node<T>
    {
        public string Name { get; }

        public VariableNode(string name)
        {
            Name = name;
        }

        public override T Solve(IEquationContext<T> ctx = null)
        {
            if (ctx == null)
                return default(T);
            return ctx.GetVariable(Name, default(T));
        }

        public override string Explain(IEquationContext<T> ctx = null)
        {
            if (ctx == null)
                return default(T).ToString();
            return ctx.GetVariable(Name, default(T)).ToString();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
