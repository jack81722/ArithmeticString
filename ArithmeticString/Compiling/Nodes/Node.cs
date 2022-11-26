using ArithmeticString.Compiling.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling.Nodes
{
    public abstract class Node<T>
    {
        public Node<T> Parent { get; set; }

        public abstract T Solve(IEquationContext<T> ctx = null);

        public virtual void Insert(Node<T> node)
        {
            node.Insert(this);
        }

        public virtual string Explain(IEquationContext<T> ctx = null)
        {
            return ToString();
        }
    }

    
}

