using ArithmeticString.Compiling.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling
{
    public interface IEquation<T>
    {
        Node<T> Expr { get; }
        string[] Parameters { get; }
        IEquationContext<T> NewContext();
        T Solve(IEquationContext<T> ctx = null);

        string Explain(IEquationContext<T> ctx = null);
    }

    public class Equation<T> : IEquation<T>
    {
        public string Name { get; }
        public Node<T> Expr { get; }
        public string[] Parameters { get; }

        public Equation(string name, Node<T> expr, string[] parameters)
        {
            Name = name;
            Expr = expr;
            Parameters = parameters;
        }

        public IEquationContext<T> NewContext()
        {
            return new EquationContext<T>(Expr);
        }

        public T Solve(IEquationContext<T> ctx = null)
        {
            if (ctx == null)
                ctx = new EquationContext<T>(Expr);
            return Expr.Solve(ctx);
        }

        public string Explain(IEquationContext<T> ctx = null)
        {
            if (ctx == null)
                ctx = new EquationContext<T>(Expr);
            return Expr.Explain(ctx);
        }

        public override string ToString()
        {
            string paramStr = "";
            for (int i = 0; i < Parameters.Length; i++)
            {
                if (i > 0)
                    paramStr += ", ";
                paramStr += Parameters[i];
            }
            var name = string.IsNullOrEmpty(Name) ? "(anonymous)" : Name;
            return $"{name}({paramStr}) = {Expr}";
        }

    }

    /// <summary>
    /// The equation of multi-equation 
    /// </summary>
    /// <remarks>
    /// The Expr & Parameter property would return last equation of the script.
    /// </remarks>
    public class EquationCollection<T> : IEquation<T>, IEnumerable<IEquation<T>>
    {
        private Equation<T>[] _equations;

        public Node<T> Expr => Last.Expr;

        public string[] Parameters => Last.Parameters;

        private Equation<T> Last => _equations[_equations.Length - 1];

        public IEquation<T> this[int key] => _equations[key];

        public EquationCollection(Equation<T>[] eqs)
        {
            _equations = eqs;
        }

        public IEquationContext<T> NewContext()
        {
            var ctx = new EquationContext<T>(Last.Expr);
            foreach (var eq in _equations)
            {
                ctx.PutEquation(eq.Name, eq);
            }
            return ctx;
        }

        public T Solve(IEquationContext<T> ctx = null)
        {
            if (ctx == null)
                ctx = NewContext();
            return ctx.Solve();
        }

        public string Explain(IEquationContext<T> ctx = null)
        {
            if (ctx == null)
                ctx = NewContext();
            return ctx.Explain();
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < _equations.Length; i++)
            {
                if (i > 0)
                    str += "\n";
                str += _equations[i].ToString() + ";";
            }
            return str;
        }

        #region -- IEnumerable --
        public IEnumerator<IEquation<T>> GetEnumerator()
        {
            foreach(var eq in _equations)
            {
                yield return eq;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }

    public static class EquationExtension
    {
        public static IEquationContext<T> PutVariable<T>(this IEquation<T> equation, string varName, T value)
        {
            return equation.NewContext().PutVariable(varName, value);
        }

        public static IEquationContext<T> PutEquation<T>(this IEquation<T> equation, string eqName, IEquation<T> eq)
        {
            return equation.NewContext().PutEquation(eqName, eq);
        }

        public static IEquationContext<T> PutVariable<T>(this EquationCollection<T> equation, string varName, T value)
        {
            return equation.NewContext().PutVariable(varName, value);
        }

        public static IEquationContext<T> PutEquation<T>(this EquationCollection<T> equation, string eqName, IEquation<T> eq)
        {
            return equation.NewContext().PutEquation(eqName, eq);
        }
    }
}
