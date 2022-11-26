using ArithmeticString.Compiling;
using ArithmeticString.Compiling.Operators.Float;
using System;
using System.Collections.Generic;

namespace StringCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Compiler<float> compiler = new Compiler<float>(new FloatParser());
            // operators
            compiler.InstallOperator<PlusOperator>("+");
            compiler.InstallOperator<MinusOperator>("-");
            compiler.InstallOperator<MultiOperator>("*", 1);
            compiler.InstallOperator<DivideOperator>("/", 1);
            compiler.InstallOperator<PowerOperator>("^", 2);
            compiler.InstallOperator<GreaterOrEqualOperator>(">=");

            compiler.InstallPrefix<NegativeDecorator>('-');

            compiler.InstallFunc("sum", FloatFuncs.Sum);
            compiler.InstallFunc("floor", FloatFuncs.Floor);
            compiler.InstallFunc("ceil", FloatFuncs.Ceil);
            compiler.InstallFunc("round", FloatFuncs.Round);
            compiler.InstallFunc("clamp01", FloatFuncs.Clamp01);
            compiler.InstallFunc("clamp", FloatFuncs.Clamp);

            compiler.SetConstant(3.14f, "PI", "pi");

            Console.WriteLine();
            //var eq1 = compiler.Parse("x + y * 2 + pi");
            //var eq2 = compiler.Parse("2 * f(x, y)");
            //var eq3 = compiler.Parse("f(a, b) + h(b, a)");
            //Console.WriteLine(eq1);
            //Console.WriteLine(eq2);
            //Console.WriteLine(eq3);
            //Console.WriteLine($"result = {eq3.Put("f", eq1).PutEquation("h", eq2).PutVariable("a", 1).PutVariable("b", 2).Solve()}");

            var eq = compiler.Parse("-x + 1");
            Console.WriteLine(eq.PutVariable("x", 1).Solve());

            Console.WriteLine();

            var eqs = compiler.Compile(
                "b = 2;" +
                "a = 1;" +
                "f(x, y) = x + y * 2;" +
                "h(x, y) = 2 * f(x, y * 1) ;" +
                "f(a, b) + h(b, a);");
            Console.WriteLine(eqs);
            Console.WriteLine($"result = {eqs.Solve()}");
            Console.WriteLine($"result = {eqs.Explain()}");

            eqs = compiler.Compile(
                "a = 1;" +
                "f(x) = 2 * x;" +
                "1 + f(a * 1)");
            Console.WriteLine(eqs);
            Console.WriteLine($"result = {eqs.Solve()}");
            Console.WriteLine($"result = {eqs.Explain()}");

            //Console.WriteLine(eqs[3].PutEquation("f", eqs[2]).PutVariable("x", 2).PutVariable("y", 1).Explain());

            /*
             * f(x) = x * 2;
             * h(x) = 1 + f(x);
             */

            /*
             * atk(str) = str * 3;
             * dmg(hp) = hp - atk(str);
            */

            // node.Solve("hp": 100);
        }


    }
}
