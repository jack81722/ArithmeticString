using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling.Nodes
{
    public class QuotaNode<T> : GroupNode<T>
    {
        public QuotaNode() : base("(", ")") { }
    }
}
