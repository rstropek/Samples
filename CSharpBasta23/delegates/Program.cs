using System.Linq.Expressions;

// Define an anonymous delegate and invoke it. The MathOp delegate is a delegate type that 
// represents a method that takes two integers as input and returns an integer.
// The following code defines an instance of the MathOp delegate and assigns an anonymous
// function to it. The function takes two integers x and y as input and returns their sum.
// The function is then invoked by calling the op1 delegate instance and passing 10 and 20 as arguments.
MathOp op1 = delegate(int x, int y) { return x + y; };
Console.WriteLine($"10 + 20 = {op1(10, 20)}");

// Both delegate and => are used to define anonymous functions in C#, but they have different syntax.
// The code above uses the delegate keyword to define an anonymous delegate. The code below uses
// the lambda operator => to define a lambda expression. Both of these constructs result in the 
// same behavior, but the syntax of the lambda expression is more concise and widely used in modern C#.
op1 = (x, y) => x + y;
Console.WriteLine($"10 + 20 = {op1(10, 20)}");

// The Func delegate type is a generic delegate that can be used to represent any method that 
// takes one or more input parameters and returns a value. The number and types of the input 
// parameters are specified as type arguments to the Func delegate. In this case, Func<int, int, int> 
// specifies that the delegate type takes two int parameters and returns an int value.
Func<int, int, int> f1 = (x, y) => x + y;
Console.WriteLine($"10 + 20 = {f1(10, 20)}");

// The following line of code is possible because the natural type of a lambda function
// is Func<T>. This features is available in C# >= 10. Read more at
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions#natural-type-of-a-lambda-expression
var f2 = (int x, int y) => x + y;
Console.WriteLine($"10 + 20 = {f2(10, 20)}");

// Let's switch from Lambda expression to static private method...
static int Add(int x, int y) => x + y;
f2 = Add;
Console.WriteLine($"10 + 20 = {f2(10, 20)}");

// In C#, the Delegate class is the base class for all delegate types. A delegate is
// a type that represents a reference to a method. Beside other features, Delegate
// offers easy access to the delegate's method info (reflection).
Delegate d1 = Add;
Console.WriteLine($"10 + 20 = {d1.DynamicInvoke(10, 20)}");
var mi = d1.Method;
foreach(var p in mi.GetParameters())
{
	Console.WriteLine($"{p.ParameterType} {p.Name}");
}

// This function takes in a Delegate object d as input and returns a Func<string> 
// delegate that represents a function that returns a string representation of the 
// result of calling the method represented by d with random parameters.
static Func<string> ConvertToStringReturner(Delegate d)
{
	// Get method info from delegate
	var mi = d.Method;
	
	// Inspect method parameters. Here we assume, that all parameters are of type int.
	// We fill each parameter with a random value. In ASP.NET Core Minimal API,
	// the runtime would fill parameters with path parameters, query parameters, etc.
	var randomParams = mi.GetParameters().Select(p => Expression.Constant(Random.Shared.Next(100))).ToArray();
	
	// Call the delegate d
	var instance = mi.IsStatic ? null : Expression.Constant(d.Target);
	var dExpr = Expression.Call(instance,
								  d.Method,
								  randomParams);
	
	// Convert the result of delegate d into string by calling ToString
	var toStringExpr = Expression.Call(dExpr, 
									   typeof(object).GetMethod(nameof(object.ToString))!);
	
	// Create a lambda expression and compile the code
	var expression = Expression.Lambda<Func<string>>(toStringExpr);
	var f3 = expression.Compile();
	return f3;
}

// Try calling ConvertToStringReturner with a function
var f3 = ConvertToStringReturner(Add);
Console.WriteLine($"The result of adding two random numbers is {f3()}");

// Try calling ConvertToStringReturner with a lambda expression
f3 = ConvertToStringReturner((int x, int y) => x + y);
Console.WriteLine($"The result of adding two random numbers is {f3()}");

delegate int MathOp(int x, int y);
