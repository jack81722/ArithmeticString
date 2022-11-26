using ArithmeticString.Compiling.Interfaces;
using System;

namespace ArithmeticString.Compiling.Operators.Float
{
    #region -- Parser --
    public class FloatParser : IParser<float>
    {
        public float Parse(string str)
        {
            return float.Parse(str);
        }
    }
    #endregion

    #region -- General Operator --
    public class PlusOperator : IOperator<float>
    {
        public float Calculate(float x, float y)
        {
            return x + y;
        }
    }

    public class MinusOperator : IOperator<float>
    {
        public float Calculate(float x, float y)
        {
            return x - y;
        }
    }

    public class MultiOperator : IOperator<float>
    {
        public float Calculate(float x, float y)
        {
            return x * y;
        }
    }

    public class DivideOperator : IOperator<float>
    {
        public float Calculate(float x, float y)
        {
            return x / y;
        }
    }

    public class PowerOperator : IOperator<float>
    {
        public float Calculate(float x, float y)
        {
            return MathF.Pow(x, y);
        }
    }
    #endregion

    #region -- Decorator --
    public class NegativeDecorator : IDecorator<float>
    {
        public float Decorate(float x)
        {
            return -x;
        }
    }
    #endregion

    #region -- Logic Gate Operator --
    public class EqualOperator : IOperator<float>
    {
        public float Calculate(float x, float y)
        {
            return x == y ? 1 : 0;
        }
    }
    public class NotEqualOperator : IOperator<float>
    {
        public float Calculate(float x, float y)
        {
            return x != y ? 1 : 0;
        }
    }

    public class GreaterOperator : IOperator<float>
    {
        public float Calculate(float x, float y)
        {
            return x > y ? 1 : 0;
        }
    }

    public class GreaterOrEqualOperator : IOperator<float>
    {
        public float Calculate(float x, float y)
        {
            return x >= y ? 1 : 0;
        }
    }

    public class LessOperator : IOperator<float>
    {
        public float Calculate(float x, float y)
        {
            return x < y ? 1 : 0;
        }
    }

    public class LessOrEqualOperator : IOperator<float>
    {
        public float Calculate(float x, float y)
        {
            return x <= y ? 1 : 0;
        }
    }
    #endregion

    public static class FloatFuncs
    {
        public static float Sum(float[] args)
        {
            float sum = 0;
            foreach (var n in args)
            {
                sum += n;
            }
            return sum;
        }

        public static float Floor(float[] args)
        {
            return MathF.Floor(args[0]);
        }

        public static float Ceil(float[] args)
        {
            return MathF.Ceiling(args[0]);
        }

        public static float Round(float[] args)
        {
            if (args.Length == 1)
                return MathF.Round(args[0]);
            return MathF.Round(args[0], (int)args[1]);
        }

        public static float Clamp01(float[] args)
        {
            var value = MathF.Min(args[0], 0);
            value = MathF.Max(args[0], 1);
            return value;
        }

        public static float Clamp(float[] args)
        {
            var value = MathF.Max(args[0], args[1]);
            value = MathF.Min(value, args[2]);
            return value;
        }

        public static float Log(float[] args)
        {
            return MathF.Log(args[0], args[1]);
        }

        public static float Log10(float[] args)
        {
            return MathF.Log10(args[0]);
        }

        public static float Log2(float[] args)
        {
            return MathF.Log2(args[0]);
        }

        public static float Sin(float[] args)
        {
            return MathF.Sin(args[0]);
        }

        public static float Cos(float[] args)
        {
            return MathF.Cos(args[0]);
        }

        public static float Tan(float[] args)
        {
            return MathF.Tan(args[0]);
        }

        public static float Sinh(float[] args)
        {
            return MathF.Sinh(args[0]);
        }

        public static float Cosh(float[] args)
        {
            return MathF.Cosh(args[0]);
        }

        public static float Tanh(float[] args)
        {
            return MathF.Tanh(args[0]);
        }

        public static float Sqrt(float[] args)
        {
            return MathF.Sqrt(args[0]);
        }

    }

}
