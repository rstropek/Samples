using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace JitProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var i = 0; i < 1000; i++)
            {
                // Generate two random strings
                var s1 = RandomString(50);
                var s2 = RandomString(50);

                // Calculate similarity
                var result = CalculateSimilarity(s1, s2);
                // Console.WriteLine($"{s1} - {s2} = {result}");
            }
        }

        private static MetadataReference[] references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };

        private static int CalculateSimilarity(string s1, string s2)
        {
            // Scenario: Calculate similarity of two strings (e.g. customer names)
            // Algorithm should be pluggable --> read from C# text (e.g. from DB,
            // here from resources).

            // Use Roslyn to parse text
            var syntaxTree = CSharpSyntaxTree.ParseText(ReadMacroFromResources());

            // Compile to an in-memory assembly
            var assembly = CompileToAssembly(syntaxTree, references);

            // Call comparison algorithm using Reflection
            var type = assembly.GetType("JitProblem.LevenshteinDistance");
            return (int)type.GetMethod("Compute").Invoke(null, new[] { s1, s2 });
        }

        private static int seed = 0;

        private static string RandomString(int size)
        {
            var random = new Random(++seed);
            var builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                builder.Append(Convert.ToChar(random.Next(65, 65 + 26)));
            }

            return builder.ToString();
        }

        private static string ReadMacroFromResources()
        {
            string macroText;
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "JitProblem.LevenshteinMacro.cs";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                macroText = reader.ReadToEnd();
            }

            return macroText;
        }

        private static Assembly CompileToAssembly(SyntaxTree macro, MetadataReference[] references)
        {
            var assemblyName = Path.GetRandomFileName();
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { macro },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(ms.ToArray());
            }
        }
    }
}
