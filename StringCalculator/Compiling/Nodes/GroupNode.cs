using ArithmeticString.Compiling.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling.Nodes
{
    public class GroupNode<T> : Node<T>
    {
        protected string Prefix, Suffix;
        public Node<T> Root { get; set; }

        public GroupNode(string prefix = "", string suffix = "")
        {
            Prefix = prefix;
            Suffix = suffix;
        }

        public virtual void Add(Node<T> node)
        {
            if (Root == null)
                Root = node;
            else
            {
                Root.Insert(node);
                while (Root.Parent != null)
                {
                    Root = Root.Parent;
                }
            }
        }

        public override T Solve(IEquationContext<T> ctx = null)
        {
            return Root.Solve(ctx);
        }

        public override string Explain(IEquationContext<T> ctx = null)
        {
            return $"{Prefix}{Root.Explain(ctx)}{Suffix}";
        }

        public override string ToString()
        {
            string str = Prefix;
            str += Root.ToString();
            str += Suffix;
            return str;
        }
    }

}
