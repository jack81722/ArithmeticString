using ArithmeticString.Compiling.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling
{

    public interface IEquationContext<T> : ICloneable, IDisposable
    {
        IEquationContext<T> PutVariable(string varName, T value);

        IEquationContext<T> PutEquation(string eqName, IEquation<T> equation);

        T GetVariable(string varName, T fallback);

        IEquation<T> GetEquation(string eqName);

        IEquationContext<T> Scope();

        T Solve();

        string Explain();
    }

    public class EquationContext<T> : IEquationContext<T>
    {
        protected Node<T> Expr;
        protected Dictionary<string, T> Variables = new Dictionary<string, T>();
        protected Dictionary<string, IEquation<T>> Equations = new Dictionary<string, IEquation<T>>();

        public EquationContext(Node<T> expr)
        {
            Expr = expr;
        }

        public object Clone()
        {
            EquationContext<T> scoped = new EquationContext<T>(Expr);
            scoped.Variables = new Dictionary<string, T>(Variables.Count);
            foreach (var old in Variables)
            {
                scoped.Variables.Add(old.Key, old.Value);
            }
            scoped.Equations = new Dictionary<string, IEquation<T>>(Equations.Count);
            foreach (var old in Equations)
            {
                scoped.Equations.Add(old.Key, old.Value);
            }
            return scoped;
        }

        public void Dispose()
        {
            Expr = null;
            Variables.Clear();
            Equations.Clear();
        }

        public IEquation<T> GetEquation(string eqName)
        {
            if (Equations.TryGetValue(eqName, out IEquation<T> eq))
            {
                return eq;
            }
            return null;
        }

        public T GetVariable(string varName, T fallback)
        {
            if (!Variables.TryGetValue(varName, out T value))
            {
                if (!Equations.TryGetValue(varName, out IEquation<T> eq))
                {
                    return fallback;
                }
                return eq.Solve(this);
            }
            return value;
        }

        public IEquationContext<T> PutVariable(string variable, T value)
        {
            if (!Variables.ContainsKey(variable))
            {
                Variables.Add(variable, value);
                return this;
            }
            Variables[variable] = value;
            return this;
        }

        public IEquationContext<T> PutEquation(string eqName, IEquation<T> equation)
        {
            if (!Equations.ContainsKey(eqName))
            {
                Equations.Add(eqName, equation);
                return this;
            }
            Equations[eqName] = equation;
            return this;
        }

        public IEquationContext<T> Scope()
        {
            return (IEquationContext<T>)Clone();
        }

        public T Solve()
        {
            if (Expr == null)
                return default(T);
            return Expr.Solve(this);
        }

        public string Explain()
        {
            if (Expr == null)
                return "";
            return Expr.Explain(this);
        }
    }
}
