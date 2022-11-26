using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling.Nodes
{
    /// <summary>
    /// ExprNode is the root node of expression, it would be instantiate first.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ExprNode<T> : GroupNode<T>
    {
        public ExprNode() : base() { }
    }
}
