using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

// Tip: If you are looking for more Roslyn samples,
// look at https://github.com/dotnet/roslyn-sdk/tree/master/samples

namespace RoslynDemos.Compilation
{
	class Program
	{
		static void Main(string[] args)
		{
			// ATTENTION: This program is just for demo purposes. It is
			// easy to inject code. Add proper checks in practice.

			const string expressionShell = @"public class Calculator
			{
				public static object Evaluate()
				{
					return $;
				} 
			}";

			while (true)
			{
				// Ask user for a C# expression
				Console.WriteLine("Please enter an expression:");
				var expression = Console.ReadLine();

				// Dynamically generate source code
				var dynamicCode = expressionShell.Replace("$", expression);

				// Parse and compile the source code
				var tree = SyntaxFactory.ParseSyntaxTree(dynamicCode);
				var compilation = CSharpCompilation.Create(
					"calc.dll",
					options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
					syntaxTrees: new[] { tree },
					references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });

				// Check for errors
				var diagnostics = compilation.GetDiagnostics();
				if (diagnostics.Count() > 0)
				{
					foreach (var diagnostic in diagnostics)
					{
						Console.WriteLine(diagnostic.GetMessage());
					}
				}
				else
				{
					// Emit assembly in-memory (no DLL is generated on disk)
					Assembly compiledAssembly;
					using (var stream = new MemoryStream())
					{
						var compileResult = compilation.Emit(stream);
						compiledAssembly = Assembly.Load(stream.GetBuffer());
					}

					// Dynamically call method and print result
					var calculator = compiledAssembly.GetType("Calculator");
					var evaluate = calculator.GetMethod("Evaluate");
					Console.WriteLine(evaluate.Invoke(null, null).ToString());
				}
			}
		}
	}
}
