using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace RoslynVisitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var tree = CSharpSyntaxTree.ParseText(
                @"class Test { public void DoSomething() => 
                    System.Console.WriteLine(""Hello World!""); }");

            var v = new PrintMethodNamesVisitor();
            v.Visit(tree.GetRoot());

            var r = new MethodRenamingRewriter();
            var newRoot = r.Visit(tree.GetRoot());
            Console.WriteLine(newRoot.ToString());
            v.Visit(newRoot);
        }
    }

    public class PrintMethodNamesVisitor : CSharpSyntaxWalker
    {
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            Console.WriteLine(node.Identifier.Value);
            base.VisitMethodDeclaration(node);
        }
    }

    public class MethodRenamingRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node.Identifier.Value.ToString() == "DoSomething")
            {
                return node.ReplaceNode(node, SyntaxFactory.MethodDeclaration(
                    node.AttributeLists,
                    node.Modifiers,
                    node.ReturnType,
                    node.ExplicitInterfaceSpecifier,
                    SyntaxFactory.Identifier("DoSomethingElse"),
                    node.TypeParameterList,
                    node.ParameterList,
                    node.ConstraintClauses,
                    node.Body,
                    node.ExpressionBody));
            }

            return node;
        }
    }
}
