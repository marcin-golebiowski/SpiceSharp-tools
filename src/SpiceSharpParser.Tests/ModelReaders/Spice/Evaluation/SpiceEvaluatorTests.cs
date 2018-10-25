using SpiceSharpParser.Common.Evaluation;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Evaluation;
using System;
using Xunit;

namespace SpiceSharpParser.Tests.ModelReaders.Spice.Evaluation
{
    public class SpiceEvaluatorTests
    {
        [Fact]
        public void ParentEvaluator()
        {
            // arrange
            var p = new SpiceEvaluator();
            p.SetParameter("a", 1);

            // act and assert
            var v = p.CreateChildEvaluator("child", null);

            v.SetParameter("xyz", 13.0);
            Assert.Equal(1, v.GetParameterValue("a"));

            v.SetParameter("a", 2);
            Assert.Equal(2, v.GetParameterValue("a"));
            Assert.Equal(1, p.GetParameterValue("a"));
        }

        [Fact]
        public void AddActionExpression()
        {
            // arrange
            var v = new SpiceEvaluator();
            v.SetParameter("xyz", 13.0);

            double expressionValue = 0;

            // act
            v.AddAction("noname", "xyz + 1", (newValue) => { expressionValue = newValue; });
            v.SetParameter("xyz", 14);

            var val = v.GetParameterValue("xyz");

            // assert
            Assert.Equal(15, expressionValue);
        }

        [Fact]
        public void EvaluateFailsWhenThereCurrlyBraces()
        {
            Evaluator v = new SpiceEvaluator();
            Assert.Throws<Exception>(() => v.EvaluateDouble("{1}"));
        }

        [Fact]
        public void EvaluateParameter()
        {
            Evaluator v = new SpiceEvaluator();
            v.SetParameter("xyz", 13.0);

            Assert.Equal(14, v.EvaluateDouble("xyz + 1"));
        }

        [Fact]
        public void GetVariables()
        {
            // prepare
            Evaluator v = new SpiceEvaluator();
            v.SetParameter("xyz", 13.0);
            v.SetParameter("a", 1.0);

            // act
            var parameters = v.GetParametersFromExpression("xyz + 1 + a");

            // assert
            Assert.Contains("a", parameters);
            Assert.Contains("xyz", parameters);
        }

        [Fact]
        public void EvaluateSuffix()
        {
            Evaluator v = new SpiceEvaluator();
            Assert.Equal(2, v.EvaluateDouble("1V + 1"));
        }

        [Fact]
        public void TableBasic()
        {
            SpiceEvaluator v = new SpiceEvaluator();
            v.SetParameter("N", 1.0);
            Assert.Equal(10, v.EvaluateDouble("table(N, 1, pow(10, 1), 2 + 0, 20, 3, 30)"));
        }

        [Fact]
        public void TableInterpolation()
        {
            SpiceEvaluator v = new SpiceEvaluator();

            v.SetParameter("N", 1.5);
            Assert.Equal(-5, v.EvaluateDouble("table(N, 1, 0, 2, -10)"));

            v.SetParameter("N", 3);
            Assert.Equal(-10, v.EvaluateDouble("table(N, 1, 0, 2, -10)"));

            v.SetParameter("N", 0);
            Assert.Equal(0, v.EvaluateDouble("table(N, 1, 0, 2, -10)"));

            v.SetParameter("N", -1);
            Assert.Equal(0, v.EvaluateDouble("table(N, 1, 0, 2, -10)"));
        }

        [Fact]
        public void TableAdvanced()
        {
            SpiceEvaluator v = new SpiceEvaluator();
            v.SetParameter("N", 1.0);
            Assert.Equal(10, v.EvaluateDouble("table(N, 1, pow(10, 1), 2 + 0, 20, 3, 30)"));
        }

        [Fact]
        public void PowerInfix()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(8, evaluator.EvaluateDouble("2**3"));
        }

        [Fact]
        public void PowerInfixPrecedence()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(7, evaluator.EvaluateDouble("2**3-1"));
        }

        [Fact]
        public void PowerInfixSecondPrecedence()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(8, evaluator.EvaluateDouble("1+2**3-1"));
        }

        [Fact]
        public void PowerInfixThirdPrecedence()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(17, evaluator.EvaluateDouble("1+2**3*2"));
        }

        [Fact]
        public void Round()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(1, evaluator.EvaluateDouble("round(1.2)"));
            Assert.Equal(2, evaluator.EvaluateDouble("round(1.9)"));
        }

        [Fact]
        public void PowMinusLtSpice()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.LtSpice);
            // act and assert
            Assert.Equal(0, evaluator.EvaluateDouble("pow(-2,1.5)"));
        }

        [Fact]
        public void PwrLtSpice()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.LtSpice);
            // act and assert
            Assert.Equal(8, evaluator.EvaluateDouble("pwr(-2,3)"));
        }

        [Fact]
        public void PwrHSpice()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.HSpice);
            // act and assert
            Assert.Equal(-8, evaluator.EvaluateDouble("pwr(-2,3)"));
        }

        [Fact]
        public void PwrSmartSpice()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.SmartSpice);
            // act and assert
            Assert.Equal(-8, evaluator.EvaluateDouble("pwr(-2,3)"));
        }

        [Fact]
        public void MinusPowerInfixLtSpice()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.LtSpice);
            // act and assert
            Assert.Equal(0, evaluator.EvaluateDouble("-2**1.5"));
        }

        [Fact]
        public void PowMinusSmartSpice()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.SmartSpice);
            // act and assert
            Assert.Equal(Math.Pow(2, (int)1.5), evaluator.EvaluateDouble("pow(-2,1.5)"));
        }

        [Fact]
        public void PowMinusHSpice()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.HSpice);
            // act and assert
            Assert.Equal(Math.Pow(-2, (int)1.5), evaluator.EvaluateDouble("pow(-2,1.5)"));
        }

        [Fact]
        public void Sgn()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.HSpice);
            // act and assert
            Assert.Equal(0, evaluator.EvaluateDouble("sgn(0)"));
            Assert.Equal(-1, evaluator.EvaluateDouble("sgn(-1)"));
            Assert.Equal(1, evaluator.EvaluateDouble("sgn(0.1)"));
        }

        [Fact]
        public void Sqrt()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(2, evaluator.EvaluateDouble("sqrt(4)"));
        }

        [Fact]
        public void SqrtMinusHSpice()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.HSpice);
            // act and assert
            Assert.Equal(-2, evaluator.EvaluateDouble("sqrt(-4)"));
        }

        [Fact]
        public void SqrtMinusSmartSpice()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.SmartSpice);
            // act and assert
            Assert.Equal(2, evaluator.EvaluateDouble("sqrt(-4)"));
        }

        [Fact]
        public void SqrtMinusLtSpice()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.LtSpice);
            // act and assert
            Assert.Equal(0, evaluator.EvaluateDouble("sqrt(-4)"));
        }

        [Fact]
        public void DefPositive()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            evaluator.SetParameter("x1", 1);
            // act and assert
            Assert.Equal(1, evaluator.EvaluateDouble("def(x1)"));
        }

        [Fact]
        public void DefNegative()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(0, evaluator.EvaluateDouble("def(x1)"));
        }

        [Fact]
        public void Abs()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(1, evaluator.EvaluateDouble("abs(-1)"));
        }

        [Fact]
        public void Buf()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(1, evaluator.EvaluateDouble("buf(0.6)"));
            Assert.Equal(0, evaluator.EvaluateDouble("buf(0.3)"));
        }

        [Fact]
        public void Cbrt()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(2, evaluator.EvaluateDouble("cbrt(8)"));
        }

        [Fact]
        public void Ceil()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(3, evaluator.EvaluateDouble("ceil(2.9)"));
        }

        [Fact]
        public void DbSmartSpice()
        {
            // arrange
            var evaluator = new SpiceEvaluator(SpiceEvaluatorMode.SmartSpice);
            // act and assert
            Assert.Equal(20, evaluator.EvaluateDouble("db(-10)"));
        }

        [Fact]
        public void Db()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(-20, evaluator.EvaluateDouble("db(-10)"));
        }

        [Fact]
        public void Exp()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(Math.Exp(2), evaluator.EvaluateDouble("exp(2)"));
        }

        [Fact]
        public void Fabs()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(3, evaluator.EvaluateDouble("fabs(-3)"));
        }

        [Fact]
        public void Flat()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act
            var res = evaluator.EvaluateDouble("flat(10)");

            // assert
            Assert.True(res >= -10 && res <= 10);
        }

        [Fact]
        public void Floor()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(2, evaluator.EvaluateDouble("floor(2.3)"));
        }

        [Fact]
        public void Hypot()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(5, evaluator.EvaluateDouble("hypot(3,4)"));
        }

        [Fact]
        public void If()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(3, evaluator.EvaluateDouble("if(0.5, 2, 3)"));
            Assert.Equal(2, evaluator.EvaluateDouble("if(0.6, 2, 3)"));
        }

        [Fact]
        public void Int()
        {
            // arrange
            var evaluator = new SpiceEvaluator();

            // act and assert
            Assert.Equal(1, evaluator.EvaluateDouble("int(1.3)"));
        }

        [Fact]
        public void Inv()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(0, evaluator.EvaluateDouble("inv(0.51)"));
            Assert.Equal(1, evaluator.EvaluateDouble("inv(0.5)"));
        }

        [Fact]
        public void Ln()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(1, evaluator.EvaluateDouble("ln(e)"));
        }

        [Fact]
        public void Limit()
        {
            // arrange
            var evaluator = new SpiceEvaluator();

            // act and assert
            Assert.Equal(8, evaluator.EvaluateDouble("limit(10, 1, 8)"));
            Assert.Equal(1, evaluator.EvaluateDouble("limit(-1, 1, 8)"));
            Assert.Equal(4, evaluator.EvaluateDouble("limit(4, 1, 8)"));
        }

        [Fact]
        public void Log()
        {
            // arrange
            var evaluator = new SpiceEvaluator();

            // act and assert
            Assert.Equal(1, evaluator.EvaluateDouble("log(e)"));
        }

        [Fact]
        public void Log10()
        {
            // arrange
            var evaluator = new SpiceEvaluator();

            // act and assert
            Assert.Equal(1, evaluator.EvaluateDouble("log10(10)"));
        }

        [Fact]
        public void Max()
        {
            // arrange
            var evaluator = new SpiceEvaluator();

            // act and assert
            Assert.Equal(100, evaluator.EvaluateDouble("max(10, -10, 1, 20, 100, 2)"));
        }

        [Fact]
        public void Min()
        {
            // arrange
            var evaluator = new SpiceEvaluator();

            // act and assert
            Assert.Equal(-10, evaluator.EvaluateDouble("min(10, -10, 1, 20, 100, 2)"));
        }

        [Fact]
        public void Nint()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(1, evaluator.EvaluateDouble("nint(1.2)"));
            Assert.Equal(2, evaluator.EvaluateDouble("nint(1.9)"));
        }

        [Fact]
        public void URamp()
        {
            // arrange
            var evaluator = new SpiceEvaluator();

            // act and assert
            Assert.Equal(1.2, evaluator.EvaluateDouble("uramp(1.2)"));
            Assert.Equal(0, evaluator.EvaluateDouble("uramp(-0.1)"));
        }

        [Fact]
        public void U()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            Assert.Equal(1, evaluator.EvaluateDouble("u(1.2)"));
            Assert.Equal(0, evaluator.EvaluateDouble("u(-1)"));
        }

        [Fact]
        public void UnitInExpression()
        {
            // arrange
            var evaluator = new SpiceEvaluator();
            // act and assert
            Assert.Equal(100 * 1000, evaluator.EvaluateDouble("300kHz/3"));
        }

        [Fact]
        public void FibonacciFunction()
        {
            // arrange
            var p = new SpiceEvaluator();

            //TODO: It shouldn't be that messy ...
            Func<string, double[], IEvaluator, double> fibLogic = null; //TODO: Use smarter methods to define anonymous recursion in C# (there is a nice post on some nice blog on msdn)
            fibLogic = (string image, double[] args, IEvaluator evaluator) =>
            {
                double x = (double)args[0];

                if (x == 0.0)
                {
                    return 0.0;
                }

                if (x == 1.0)
                {
                    return 1.0;
                }

                return (double)fibLogic(image, new double[1] { (x - 1) }, evaluator) + (double)fibLogic(image, new double[1] { (x - 2) }, evaluator);
            };

            var fib = new Function()
            {
                ArgumentsCount = 1,
                DoubleArgsLogic = fibLogic,
                VirtualParameters = false,
            };
            p.Functions.Add("fib",  fib);

            Assert.Equal(0, p.EvaluateDouble("fib(0)"));
            Assert.Equal(1, p.EvaluateDouble("fib(1)"));
            Assert.Equal(1, p.EvaluateDouble("fib(2)"));
            Assert.Equal(2, p.EvaluateDouble("fib(3)"));
            Assert.Equal(3, p.EvaluateDouble("fib(4)"));
            Assert.Equal(5, p.EvaluateDouble("fib(5)"));
            Assert.Equal(8, p.EvaluateDouble("fib(6)"));
        }

        //[Fact]
        public void FibonacciAsParam()
        {
            var functionFactory = new FunctionFactory();

            var p = new SpiceEvaluator();
            p.Functions.Add("fib",
                functionFactory.Create("fib",
                new System.Collections.Generic.List<string>() { "x" },
                "x <= 0 ? 0 : (x == 1 ? 1 : lazy(#fib(x-1) + fib(x-2)#))"));

            Assert.Equal(0, p.EvaluateDouble("fib(0)"));
            Assert.Equal(1, p.EvaluateDouble("fib(1)"));
            Assert.Equal(1, p.EvaluateDouble("fib(2)"));
            Assert.Equal(2, p.EvaluateDouble("fib(3)"));
            Assert.Equal(3, p.EvaluateDouble("fib(4)"));
            Assert.Equal(5, p.EvaluateDouble("fib(5)"));
            Assert.Equal(8, p.EvaluateDouble("fib(6)"));
        }

        [Fact]
        public void FibonacciAsWithoutLazyParam()
        {
            var functionFactory = new FunctionFactory();

            var p = new SpiceEvaluator();
            p.Functions.Add("fib",
                functionFactory.Create("fib",
                new System.Collections.Generic.List<string>() { "x" },
                "x <= 0 ? 0 : (x == 1 ? 1 : (fib(x-1) + fib(x-2)))"));

            Assert.Equal(0, p.EvaluateDouble("fib(0)"));
            Assert.Equal(1, p.EvaluateDouble("fib(1)"));
            Assert.Equal(1, p.EvaluateDouble("fib(2)"));
            Assert.Equal(2, p.EvaluateDouble("fib(3)"));
            Assert.Equal(3, p.EvaluateDouble("fib(4)"));
            Assert.Equal(5, p.EvaluateDouble("fib(5)"));
            Assert.Equal(8, p.EvaluateDouble("fib(6)"));
        }

        //[Fact]
        public void FactAsParam()
        {
            var functionFactory = new FunctionFactory();

            var p = new SpiceEvaluator();
            p.Functions.Add("fact",
                functionFactory.Create("fact",
                new System.Collections.Generic.List<string>() { "x" },
                "x == 0 ? 1 : (x * lazy(#fact(x-1)#))"));

            Assert.Equal(1, p.EvaluateDouble("fact(0)"));
            Assert.Equal(1, p.EvaluateDouble("fact(1)"));
            Assert.Equal(2, p.EvaluateDouble("fact(2)"));
            Assert.Equal(6, p.EvaluateDouble("fact(3)"));
        }

        //[Fact]
        public void LazySimple()
        {
            var functionFactory = new FunctionFactory();

            var p = new SpiceEvaluator();
            p.Functions.Add("test_lazy",
                functionFactory.Create("test_lazy",
                    new System.Collections.Generic.List<string>() {"x"},
                    "x == 0 ? 1: lazy(#3+2#)"));

            Assert.Equal(1, p.EvaluateDouble("test_lazy(0)"));
            Assert.Equal(5, p.EvaluateDouble("test_lazy(1)"));
        }

        //[Fact]
        public void LazyError()
        {
            var functionFactory = new FunctionFactory();

            var p = new SpiceEvaluator();
            p.Functions.Add("test_lazy",
                functionFactory.Create("test_lazy",
                    new System.Collections.Generic.List<string>() {"x"},
                    "x == 0 ? 1: lazy(#1/#)"));

            Assert.Equal(1, p.EvaluateDouble("test_lazy(0)"));
        }

        //[Fact]
        public void ComplexCondBroken()
        {
            var p = new SpiceEvaluator();
            var expr = "x <= 9 ? 3 : (x == 5 ? 1 : lazy(#2/-#))";

            p.SetParameter("x", 9);
            Assert.Equal(3, p.EvaluateDouble(expr));
        }

        //[Fact]
        public void SimpleCond()
        {
            var p = new SpiceEvaluator();
            var expr = "x <= 0 ? 0 : lazy(#2#)";

            p.SetParameter("x", 0);
            Assert.Equal(0, p.EvaluateDouble(expr));

            p.SetParameter("x", 1);
            Assert.Equal(2, p.EvaluateDouble(expr));
        }

        //[Fact]
        public void Lazy()
        {
            var p = new SpiceEvaluator();
            Assert.Equal(4, p.EvaluateDouble("2 >= 0 ? lazy(#3+1#) : lazy(#4+5#)"));
            Assert.Equal(8, p.EvaluateDouble("1 <= 0 ? lazy(#1+1#) : lazy(#3+5#)"));
        }

        //[Fact]
        public void LazyFunc()
        {
            var functionFactory = new FunctionFactory();

            var p = new SpiceEvaluator();
            p.Functions.Add("test", functionFactory.Create(
                "test",
                new System.Collections.Generic.List<string>(),
                "5"));

            p.Functions.Add("test2", functionFactory.Create(
                "test2",
                new System.Collections.Generic.List<string>() {"x"},
                "x <= 0 ? 0 : (x == 1 ? 1 : lazy(#test()#))"));

            Assert.Equal(0, p.EvaluateDouble("test2(0)"));
            Assert.Equal(1, p.EvaluateDouble("test2(1)"));
            Assert.Equal(5, p.EvaluateDouble("test2(2)"));
        }

        [Fact]
        public void PolyThreeVariablesSum()
        {
            var p = new SpiceEvaluator();
            Assert.Equal(15, p.EvaluateDouble("poly(3, 3, 5, 7, 0, 1, 1, 1)"));
        }

        [Fact]
        public void PolyTwoVariablesSum()
        {
            var p = new SpiceEvaluator();
            Assert.Equal(3, p.EvaluateDouble("poly(2, 1, 2, 0, 1, 1)"));
        }

        [Fact]
        public void PolyTwoVariablesMult()
        {
            var p = new SpiceEvaluator();
            Assert.Equal(6, p.EvaluateDouble("poly(2, 3, 2, 0, 0, 0, 0, 1)"));
        }

        [Fact]
        public void PolyOneVariableSquare()
        {
            var p = new SpiceEvaluator();
            Assert.Equal(4, p.EvaluateDouble("poly(1, 2, 0, 0, 1)"));
        }

        [Fact]
        public void PolyOneVariablePowerOfThree()
        {
            var p = new SpiceEvaluator();
            Assert.Equal(8, p.EvaluateDouble("poly(1, 2, 0, 0, 0, 1)"));
        }

        [Fact]
        public void PolyOneVariableMultiple()
        {
            var p = new SpiceEvaluator();
            Assert.Equal(4, p.EvaluateDouble("poly(1, 2, 0, 2)"));
        }

        [Fact]
        public void PolyOneVariableSquerePlusConstant()
        {
            var p = new SpiceEvaluator();
            Assert.Equal(14, p.EvaluateDouble("poly(1, 2, 10, 0, 1)"));
        }
    }
}
