using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var totalNumberOfClassDeclarations = 0;

            var files = Directory.EnumerateFiles(
                Directory.GetCurrentDirectory(),
                "*.cs",
                SearchOption.AllDirectories);
            foreach(var file in files)
            {
                Console.WriteLine($"Analyzing {file}...");

                var tree = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(file));
                var root = tree.GetRoot();
                totalNumberOfClassDeclarations += root.DescendantNodesAndSelf()
                    .OfType<ClassDeclarationSyntax>()
                    .Count();
            }

            Console.WriteLine($"Found {totalNumberOfClassDeclarations} class declarations");
        }
    }
}
