using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling.Nodes
{
    public class FuncNode<T> : GroupNode<T>
    {
        public string Name { get; }
        private Func<T[], T> _func;
        protected List<Node<T>> Inners = new List<Node<T>>();

        public FuncNode(string name, Func<T[], T> func) : base($"{name}(", ")")
        {
            Name = name;
            _func = func;
        }

        public override void Add(Node<T> node)
        {
            Inners.Add(node);
        }

        public override T Solve(IEquationContext<T> ctx = null)
        {
            T[] values;
            if (_func == null)
            {
                var eq = ctx.GetEquation(Name);
                if (eq == null)
                    return default(T);
                T result;
                // transfer parameters
                if (eq.Parameters.Length != Inners.Count)
                    throw new ArgumentException("unmatched parameters");
                using (var scoped = ctx.Scope())
                {
                    for (int i = 0; i < Inners.Count; i++)
                    {
                        var inner = Inners[i];
                        var scopedVarName = eq.Parameters[i];
                        var scopedValue = inner.Solve(ctx);
                        scoped.PutVariable(scopedVarName, scopedValue);
                    }
                    result = eq.Solve(scoped);
                }
                return result;
            }
            values = new T[Inners.Count];
            for (int i = 0; i < Inners.Count; i++)
            {
                values[i] = Inners[i].Solve(ctx);
            }
            return _func(values);
        }

        public override string Explain(IEquationContext<T> ctx = null)
        {
            if (_func == null)
            {
                var eq = ctx.GetEquation(Name);
                if (eq == null)
                    return "";
                // transfer parameters
                if (eq.Parameters.Length != Inners.Count)
                    throw new ArgumentException("unmatched parameters");
                using (var scoped = ctx.Scope())
                {
                    for (int i = 0; i < Inners.Count; i++)
                    {
                        var inner = Inners[i];
                        var scopedVarName = eq.Parameters[i];
                        var scopedValue = inner.Solve(ctx);
                        scoped.PutVariable(scopedVarName, scopedValue);
                    }
                    return $"({eq.Explain(scoped)})";
                }
            }
            string str = Prefix;
            for (int i = 0; i < Inners.Count; i++)
            {
                if (i > 0)
                    str += ", ";
                str += Inners[i].Solve(ctx).ToString();
            }
            str += Suffix;
            return str;
        }

        public override string ToString()
        {
            string str = Prefix;
            for (int i = 0; i < Inners.Count; i++)
            {
                if (i > 0)
                    str += ", ";
                str += Inners[i].ToString();
            }
            str += Suffix;
            return str;
        }

    }
}
