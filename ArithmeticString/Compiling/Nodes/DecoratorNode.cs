using ArithmeticString.Compiling.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArithmeticString.Compiling.Nodes
{
    public class DecoratorNode<T> : GroupNode<T>
    {
        protected IDecorator<T> Decorator;

        public DecoratorNode(IDecorator<T> decorator, string prefix = "", string suffix = "") : base(prefix, suffix)
        {
            Decorator = decorator;
        }

        public override T Solve(IEquationContext<T> ctx = null)
        {
            return Decorator.Decorate(base.Solve(ctx));
        }
    }

    public class PrefixDecoratorNode<T> : DecoratorNode<T>
    {
        public PrefixDecoratorNode(IDecorator<T> decorator, char c) : base(decorator, $"{c}(", ")") { }
    }

    public class SuffixDecoratorNode<T> : DecoratorNode<T>
    {
        public SuffixDecoratorNode(IDecorator<T> decorator, char c) : base(decorator, "(", $"{c})") { }
    }
}
