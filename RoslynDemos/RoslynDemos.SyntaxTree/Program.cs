using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

// Tip: If you are looking for more Roslyn samples,
// look at https://github.com/dotnet/roslyn/tree/master/src/Samples

namespace RoslynDemos.SyntaxTree
{
	class Program
	{
		static void Main(string[] args)
		{
			ReadSyntaxTree();
			ChangeSyntaxTree();
			SyntaxTreeVisitor();
		}

		static void ReadSyntaxTree()
		{
			const string text = "class Basta2015 { void CSharpWorkshop(int numberOfAttendees) { } }";

			// Generate syntax tree by parsing the text
			var tree = SyntaxFactory.ParseSyntaxTree(text);

			// Walk down to method declaration
			var root = (CompilationUnitSyntax)tree.GetRoot();
			var bastaClass = (TypeDeclarationSyntax)root.Members[0];
			var methodDeclaration = (MethodDeclarationSyntax)bastaClass.Members[0];

			// Get identifier of first parameter of method 
			var parameter = methodDeclaration.ParameterList.Parameters[0];
			Console.WriteLine($"The name of the identifier is '{parameter.Identifier}'.");

			// Now let's look for the parameter using Linq
			parameter = root
				.DescendantNodes()
				.OfType<ParameterSyntax>()
				.First();
			Console.WriteLine($"The name of the identifier is '{parameter.Identifier}'.");
		}

		static void ChangeSyntaxTree()
		{
			const string text = "class Basta2015 { void CSharpWorkshop(int numberOfAttendees) { } }";

			// Generate syntax tree by parsing the text
			var tree = SyntaxFactory.ParseSyntaxTree(text);
			var root = (CompilationUnitSyntax)tree.GetRoot();

			// Find method using Linq
			var method = root
				.DescendantNodes()
				.OfType<MethodDeclarationSyntax>()
				.Where(m => m.Identifier.ValueText == "CSharpWorkshop")
				.Single();

			// Update method. Note that we cannot really update the method declaration as it is
			// immutable. We have to create a copy of the method.
			var newMethod = method.Update(
				method.AttributeLists,
				method.Modifiers,
				method.ReturnType,
				method.ExplicitInterfaceSpecifier,
				SyntaxFactory.Identifier("HavingFunWithRoslyn"),
				method.TypeParameterList,
				method.ParameterList,
				method.ConstraintClauses,
				method.Body,
				method.SemicolonToken);

			// Now replace the old method. Note that we get a new syntax tree as the
			// original one is immutable.
			root = root.ReplaceNode(method, newMethod);
			Console.WriteLine(root.GetText());
		}

		#region SyntaxTreeVisitor
		static void SyntaxTreeVisitor()
		{
			const string text = @"class Basta2015 { void CSharpWorkshop(int numberOfAttendees) { } 
				void LetsHaveFunWithRoslyn() { } }";

			var tree = SyntaxFactory.ParseSyntaxTree(text);
			new MethodNamePrinter().Visit(tree.GetRoot());
		}

		private class MethodNamePrinter : CSharpSyntaxRewriter
		{
			public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
			{
				Console.WriteLine(node.Identifier.ValueText);

				// Note that we could return a new node here, too.
				return base.VisitMethodDeclaration(node);
			}
		}
		#endregion
	}
}