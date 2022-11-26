# ArithmeticString
The tool lib of converting string into an formula, and calculate the answer.

## Quick Start

```csharp
using ArithmeticString.Compiling;

class Programe{
	static void Main(string[] args){
		string formula = "x + 1";
		Equation<float> equation = FloatCompile.Compile(formula);
		
		// result = 2
		Console.WriteLine(equation.PutVariable("x", 1).Solve());
	}
}
```

## Environment

Framework: ```.NET Core 3.1```

## Tutorial

### Element of Formula

- **Constant**

	Constant is the basic element of the formula. It must be declare as numeric characters such as ```1```, ```2.1```, ...etc.
	```csharp
	var formula = "x";
	var equation = FloatCompile.Compile(formula);
	
	// result = 1
	Console.WriteLine(equation.Solve());
	```

- **Variable**

	Variable is detected if it's not declare as other type. After compiled the formula, we may put the variable by specific value.
	```csharp
	var formula = "x + 1";
	var equation = FloatCompile.Compile(formula);
	
	// result = 2
	Console.WriteLine(equation.PutVariable("x", 1).Solve());
	```

- **Operator**

	Operator will calculate the left and right element of the formula. And it will follow the priority of the operator (like ```*``` = ```/``` > ```+```, ```-```) by default. Also, you may customize your operator and priority (see #Advance Usage).
	```csharp
	var formula = "1 + 2 * 3";
	var equation = FloatCompile.Compile(formula);
	
	// result = 7
	Console.WriteLine(equation.Solve());
	```

- **Function**

	We can define a function if the whole formula is too long or the sub-formula is be used frequently. Define a function of the sub-formula is an efficient measure.
	```csharp
	var script = "square(x) = x * x; 1 + squre(3);";
	var equation = FloatCompile.Compile(formula);
	
	// result = 10
	Console.WriteLine(equation.Solve());
	```
	
- **Decorator**

	Use the decorator to modified the result of sub-formula such as ```Negative```.
	```csharp
	var formula = "1 + -2";
	var equation = FloatCompile.Compile(formula);
	
	// result = -1
	Console.WriteLine(equation.Solve());
	```
	
### Advanced Usage

- **Custom Define Compiler**
	
	Define custom operator by implementing ```IOperator<T>``` interface, and call the ```InstallOperator<T>``` method to install the operator with specific operator symble.
	```csharp
	using ArithmeticString.Compiling.Operators.Float;
	
	void Example(){
		var compiler = new Compile<float>(new FloatParser());
		var compiler.InstallOperator<PlusOperator>("+");
		
		var formula = "1 + 2";
		var equation = compiler.Compile(formula);
		
		// result = 3
		Console.WriteLine(equation.Solve());
	}
	
	class PlusOperator : IOperator<float>
	{
        public float Calculate(float x, float y)
        {
            return x + y;
        }
	}
	```

- **Function with unspecific length parameter**

	If we need to define a function with unspecific parameter, we may call ```InstallFunc``` and pass the function into it.
	```csharp
	using ArithmeticString.Compiling.Operators.Float;
	
	void Example(){
		var compiler = new Compile<float>(new FloatParser());
		var compiler.InstallFunc("Sigma", Sigma);
		
		var formula = "Sigma(1, 2, 3)";
		var equation = compiler.Compile(formula);
		
		// result = 6
		Console.WriteLine(equation.Solve());
	}
	
	float Sigma(float[] args){
		float sum = 0;
		foreach(var arg in args){
			sum += arg;
		}
		return sum;
	}
	```
	

## Future Feature


## Reference

- Expression Tree (see [Wiki](https://en.wikipedia.org/wiki/Binary_expression_tree))