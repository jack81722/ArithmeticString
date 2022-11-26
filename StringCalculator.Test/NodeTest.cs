using ArithmeticString.Compiling;
using ArithmeticString.Compiling.Nodes;
using ArithmeticString.Compiling.Operators.Float;
using System;
using Xunit;

namespace StringCalculator.Test
{
    public class NodeTest
    {
        [Fact]
        public void TestOperatorNodeInsert()
        {
            ValueNode<float> n1 = new ValueNode<float>(1);
            ValueNode<float> n2 = new ValueNode<float>(2);
            OperatorNode<float> op1 = new OperatorNode<float>(new PlusOperator(), "+");
            op1.Insert(n1);
            op1.Insert(n2);

            Assert.Equal(n1, op1.Left);
            Assert.Equal(n2, op1.Right);
            Assert.Equal(op1, n1.Parent);
            Assert.Equal(op1, n2.Parent);
            Assert.Equal(3, op1.Solve());
            Assert.Equal("1 + 2", op1.Explain());
            Assert.Equal("1 + 2", op1.ToString());

            OperatorNode<float> op2 = new OperatorNode<float>(new PlusOperator(), "*", 1);
            ValueNode<float> n3 = new ValueNode<float>(2);
            op1.Insert(op2);
            op1.Insert(n3);
            Assert.Equal(n1, op1.Left);
            Assert.Equal(op2, op1.Right);
            Assert.Equal(n2, op2.Left);
            Assert.Equal(op1, n1.Parent);
            Assert.Equal(op2, n2.Parent);
            Assert.Equal(n3, op2.Right);
            Assert.Equal(5, op1.Solve());
            Assert.Equal("1 + 2 * 2", op1.Explain());
            Assert.Equal("1 + 2 * 2", op1.ToString());
            Assert.Equal(4, op2.Solve());
            Assert.Equal("2 * 2", op2.Explain());
            Assert.Equal("2 * 2", op2.ToString());
        }

        [Fact]
        public void TestValueNodeInsert()
        {
            ValueNode<float> n = new ValueNode<float>(1);
            OperatorNode<float> op = new OperatorNode<float>(new PlusOperator(), "+");

            n.Insert(op);

            Assert.Equal(op, n.Parent);
        }

        [Fact]
        public void TestQuotaNode()
        {
            var qn = new QuotaNode<float>();
            var n1 = new ValueNode<float>(1);
            var n2 = new ValueNode<float>(2);
            var op = new OperatorNode<float>(new PlusOperator(), "+");
            qn.Add(n1);
            qn.Add(op);
            qn.Add(n2);

            Assert.Equal(op, qn.Root);
            Assert.Equal(3f, qn.Solve());
        }

        [Fact]
        public void TestVariableNode()
        {
            var n = new VariableNode<float>("x");
            var ctx = new EquationContext<float>(null);
            ctx.PutVariable("x", 1);

            Assert.Equal(1, n.Solve(ctx));

            var eq = FloatCompile.Parse("x = 2");
            ctx = new EquationContext<float>(null);
            ctx.PutEquation("x", eq);

            Assert.Equal(2, n.Solve(ctx));
        }

    }
}
