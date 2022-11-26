using ArithmeticString.Compiling.Interfaces;

namespace ArithmeticString.Compiling.Nodes
{
    public interface IOperatorNode<T>
    {
        string Symbol { get; }
        int Priority { get; }
        Node<T> Left { get; set; }
        Node<T> Right { get; set; }
    }

    public class OperatorNode<T> : Node<T>, IOperatorNode<T>
    {
        protected IOperator<T> Operator;
        public string Symbol { get; }
        public int Priority { get; }
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }

        public OperatorNode(IOperator<T> @operator, string symbol)
        {
            Operator = @operator;
            Symbol = symbol;
        }

        public OperatorNode(IOperator<T> @operator, string symbol, int priority)
        {
            Operator = @operator;
            Symbol = symbol;
            Priority = priority;
        }

        public override void Insert(Node<T> node)
        {
            var opNode = node as IOperatorNode<T>;
            if (opNode != null)
            {
                // is operator
                if (opNode.Priority > Priority)
                {
                    opNode.Left = Right;
                    Right.Parent = node;
                    Right = node;
                    node.Parent = this;
                }
                else
                {
                    opNode.Left = this;
                    Parent = node;
                }
            }
            else
            {
                if (Left == null)
                    Left = node;
                else
                {
                    var cur = this;
                    while (cur.Right != null)
                        cur = (OperatorNode<T>)cur.Right;
                    cur.Right = node;
                }
                node.Parent = this;
            }
        }

        public override T Solve(IEquationContext<T> ctx = null)
        {
            var left = Left.Solve(ctx);
            var right = Right.Solve(ctx);
            return Operator.Calculate(left, right);
        }

        public override string Explain(IEquationContext<T> ctx = null)
        {
            return $"{Left.Explain(ctx)} {Symbol} {Right.Explain(ctx)}";
        }

        public override string ToString()
        {
            return $"{Left} {Symbol} {Right}";
        }
    }
}
