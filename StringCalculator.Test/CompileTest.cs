using ArithmeticString.Compiling;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace ArithmeticString.Test
{
    public class CompileTest
    {
        [Fact]
        public void TestParse()
        {
            var case1 = "x + 1";
            var eq = FloatCompile.Parse(case1);
            Assert.Equal(2, eq.PutVariable("x", 1).Solve());

            var case2 = "- x + 1";
            eq = FloatCompile.Parse(case2);
            Assert.Equal(0, eq.PutVariable("x", 1).Solve());

            var case3 = "x + 1 * 2";
            eq = FloatCompile.Parse(case3);
            Assert.Equal(3, eq.PutVariable("x", 1).Solve());

            var case4 = "(x + 1) * 2";
            eq = FloatCompile.Parse(case4);
            Assert.Equal(4, eq.PutVariable("x", 1).Solve());

            var case5 = "-(-x + 1) * -2";
            eq = FloatCompile.Parse(case5);
            Assert.Equal(0, eq.PutVariable("x", 1).Solve());
        }

        [Fact]
        public void TestParseAsync()
        {
            var case1 = "x + 1";
            var eq = FloatCompile.ParseAsync(case1);
            eq.Wait();
            var err = eq.Exception;
            Assert.Null(err);
            Assert.Equal(2, eq.Result.PutVariable("x", 1).Solve());

            var case2 = "- x + 1";
            eq = FloatCompile.ParseAsync(case2);
            eq.Wait();
            err = eq.Exception;
            Assert.Null(err);
            Assert.Equal(0, eq.Result.PutVariable("x", 1).Solve());

            var case3 = "x + 1 * 2";
            eq = FloatCompile.ParseAsync(case3);
            eq.Wait();
            err = eq.Exception;
            Assert.Null(err);
            Assert.Equal(3, eq.Result.PutVariable("x", 1).Solve());

            var case4 = "(x + 1) * 2";
            eq = FloatCompile.ParseAsync(case4);
            eq.Wait();
            err = eq.Exception;
            Assert.Null(err);
            Assert.Equal(4, eq.Result.PutVariable("x", 1).Solve());

            var case5 = "-(-x + 1) * -2";
            eq = FloatCompile.ParseAsync(case5);
            eq.Wait();
            err = eq.Exception;
            Assert.Null(err);
            Assert.Equal(0, eq.Result.PutVariable("x", 1).Solve());
        }

        [Fact]
        public void TestCompile()
        {
            var case1 = "x + 1";
            var eq = FloatCompile.Compile(case1);
            Assert.Equal(2, eq.PutVariable("x", 1).Solve());

            var case2 = "- x + 1";
            eq = FloatCompile.Compile(case2);
            Assert.Equal(0, eq.PutVariable("x", 1).Solve());

            var case3 = "x + 1 * 2";
            eq = FloatCompile.Compile(case3);
            Assert.Equal(3, eq.PutVariable("x", 1).Solve());

            var case4 = "(x + 1) * 2";
            eq = FloatCompile.Compile(case4);
            Assert.Equal(4, eq.PutVariable("x", 1).Solve());

            var case5 = "-(-x + 1) * -2";
            eq = FloatCompile.Compile(case5);
            Assert.Equal(0, eq.PutVariable("x", 1).Solve());
        }

        [Fact]
        public void TestCompileAsync()
        {
            var case1 = "x + 1";
            var eq = FloatCompile.CompileAsync(case1);
            eq.Wait();
            var err = eq.Exception;
            Assert.Null(err);
            Assert.Equal(2, eq.Result.PutVariable("x", 1).Solve());

            var case2 = "- x + 1";
            eq = FloatCompile.CompileAsync(case2);
            eq.Wait();
            err = eq.Exception;
            Assert.Null(err);
            Assert.Equal(0, eq.Result.PutVariable("x", 1).Solve());

            var case3 = "x + 1 * 2";
            eq = FloatCompile.CompileAsync(case3);
            eq.Wait();
            err = eq.Exception;
            Assert.Null(err);
            Assert.Equal(3, eq.Result.PutVariable("x", 1).Solve());

            var case4 = "(x + 1) * 2";
            eq = FloatCompile.CompileAsync(case4);
            eq.Wait();
            err = eq.Exception;
            Assert.Null(err);
            Assert.Equal(4, eq.Result.PutVariable("x", 1).Solve());

            var case5 = "-(-x + 1) * -2";
            eq = FloatCompile.CompileAsync(case5);
            eq.Wait();
            err = eq.Exception;
            Assert.Null(err);
            Assert.Equal(0, eq.Result.PutVariable("x", 1).Solve());
        }
    }
}
