using ArithmeticString.Compiling.Operators.Float;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArithmeticString.Compiling
{
    public static class FloatCompile
    {
        static Lazy<Compiler<float>> _floatCompiler = new Lazy<Compiler<float>>(NewFloatCompiler);

        public static Compiler<float> NewFloatCompiler()
        {
            var compiler = new Compiler<float>(new FloatParser());
            compiler.InstallOperator<PlusOperator>("+").
                InstallOperator<MinusOperator>("-").
                InstallOperator<MultiOperator>("*", 1).
                InstallOperator<DivideOperator>("/", 1).
                InstallOperator<PowerOperator>("^", 2).
                InstallOperator<EqualOperator>("==").
                InstallOperator<NotEqualOperator>("!=").
                InstallOperator<GreaterOperator>(">").
                InstallOperator<GreaterOrEqualOperator>(">=").
                InstallOperator<LessOperator>("<").
                InstallOperator<LessOrEqualOperator>("<=").
                InstallPrefix<NegativeDecorator>('-').
                InstallFunc("sum", FloatFuncs.Sum).
                InstallFunc("floor", FloatFuncs.Floor).
                InstallFunc("ceil", FloatFuncs.Ceil).
                InstallFunc("round", FloatFuncs.Round).
                InstallFunc("clamp01", FloatFuncs.Clamp01).
                InstallFunc("clamp", FloatFuncs.Clamp).
                InstallFunc("log", FloatFuncs.Log).
                InstallFunc("log10", FloatFuncs.Log10).
                InstallFunc("log2", FloatFuncs.Log2).
                InstallFunc("sin", FloatFuncs.Sin).
                InstallFunc("cos", FloatFuncs.Cos).
                InstallFunc("tan", FloatFuncs.Tan).
                InstallFunc("sinh", FloatFuncs.Sinh).
                InstallFunc("cosh", FloatFuncs.Cosh).
                InstallFunc("tanh", FloatFuncs.Tanh);

            compiler.SetConstant(3.1415964f, "PI", "pi");
            compiler.SetConstant(2.1782818f, "E", "e");
            return compiler;
        }

        public static Equation<float> Parse(string formula)
        {
            return _floatCompiler.Value.Parse(formula);
        }

        public static Equation<float> Parse(Stream stream)
        {
            return _floatCompiler.Value.Parse(stream);
        }

        public static Task<Equation<float>> ParseAsync(string formula, CancellationTokenSource cts = null)
        {
            return _floatCompiler.Value.ParseAsync(formula, cts);
        }

        public static Task<Equation<float>> ParseAsync(Stream stream, CancellationTokenSource cts = null)
        {
            return _floatCompiler.Value.ParseAsync(stream, cts);
        }

        public static EquationCollection<float> Compile(string script)
        {
            return _floatCompiler.Value.Compile(script);
        }

        public static EquationCollection<float> Compile(Stream stream)
        {
            return _floatCompiler.Value.Compile(stream);
        }

        public static Task<EquationCollection<float>> CompileAsync(string script)
        {
            return _floatCompiler.Value.CompileAsync(script);
        }

        public static Task<EquationCollection<float>> CompileAsync(Stream stream)
        {
            return _floatCompiler.Value.CompileAsync(stream);
        }
    }
}
