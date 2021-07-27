using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace DummyCodeGenerator
{
    /// <summary>
    /// Generate a configurable ammount of dummy code
    /// </summary>
    [Generator]
    public class DummyGenerator : ISourceGenerator
    {
        private const string attributeText = @"
using System;

namespace DummyCodeGenerator
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class GenerateAttribute : Attribute
    {
        public GenerateAttribute() { }

        public int Length { get; set; }
    }
}
";
        /// <summary>
        /// Visitor class that finds methods to generate
        /// </summary>
        class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<IMethodSymbol> CandidateMethods { get; } = new List<IMethodSymbol>();

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                // We are looking for partial public methods
                if (context.Node is MethodDeclarationSyntax mds
                    && mds.Modifiers.Any(SyntaxKind.PartialKeyword)
                    && mds.Modifiers.Any(SyntaxKind.PublicKeyword))
                {
                    // Method must have GenerateAttribute and must return void
                    if (context.SemanticModel.GetDeclaredSymbol(mds) is IMethodSymbol method
                        && method.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "DummyCodeGenerator.GenerateAttribute")
                        && method.ReturnsVoid)
                    {
                        // Found a method to generate
                        CandidateMethods.Add(method);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization((i) => i.AddSource("GenerateAttribute", attributeText));

            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Make sure we found any methods to generate
            if (context.SyntaxContextReceiver is not SyntaxReceiver receiver || receiver.CandidateMethods == null)
            {
                return;
            }

            // Get attribute type
            var attributeSymbol = context.Compilation.GetTypeByMetadataName("DummyCodeGenerator.GenerateAttribute");

            // Create string builder for source generation
            var sourceBuilder = new StringBuilder();

            foreach(var method in receiver.CandidateMethods)
            {
                var type = method.ContainingType;
                var ns = type.ContainingNamespace;

                // Get Length property of attribute (default value is 10)
                var attributeData = method
                    .GetAttributes()
                    .Single(ad => ad.AttributeClass?.Equals(attributeSymbol, SymbolEqualityComparer.Default) ?? false);
                var lengthOpt = attributeData
                    .NamedArguments
                    .SingleOrDefault(kvp => kvp.Key == "Length").Value.Value as int? ?? 10;

                var rnd = new Random();
                sourceBuilder.Append($@"
using System;
using static System.Diagnostics.Debug;
namespace {ns.ToDisplayString()}
{{
    partial class {type.Name}
    {{
        public partial void {method.Name}()
        {{
            WriteLine(""Generating dummy code with length = {lengthOpt}"");
            var rnd = new Random();
");
                // Generate dummy code
                for(var i = 0; i < lengthOpt; i++)
                {
                    sourceBuilder.Append($@"
            var num{i} = rnd.Next(100000);
            if (num{i} == {rnd.Next(100000)}) WriteLine($""Guess it correctly, it is {{num{i}}}"");

                    ");
                }

                sourceBuilder.Append($@"
            WriteLine(""Done evaluating"");
        }}
    }}
}}
");
            }

            // Inject the created source into the users compilation
            context.AddSource("dummy_generated.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}
