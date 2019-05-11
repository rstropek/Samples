using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Linq;
using System.Text;

// Take a look at a practical application of Span in string formating based on
// the new Range struct from C# 8:
// https://github.com/dotnet/corefx/blob/a5155f3aeefd316eb6a66f21587506e3b8cf8d25/src/Common/src/CoreLib/System/Range.cs#L60

namespace Span
{
    [SimpleJob(RunStrategy.ColdStart, targetCount: 10)]
    [MemoryDiagnoser]
    public class StringsWithSpan
    {
        /// <summary>
        /// Random number generator used to generate a random age per person
        /// </summary>
        private readonly Random rand = new Random();

        /// <summary>
        /// Prefix for each line in the CSV data
        /// </summary>
        /// <remarks>
        /// We do not process first and last name in this sample. Therefore, we can keep them
        /// constant to keep the demo simple and concentrate only on the important learnings.
        /// </remarks>
        private const string Prefix = "John,Doe,";

        /// <summary>
        /// Generates CSV content with a specified number of lines
        /// </summary>
        /// <param name="numberOfLines">Number of lines that the CSV string should contain</param>
        /// <returns>String containing CSV data</returns>
        public string GenerateHugeCsv(int numberOfLines)
        {
            // Create string builder with correct length. Each CSV line contains first name,
            // last name, random age ([0..99]), and '\n'.
            var builder = new StringBuilder((Prefix.Length + 3) * numberOfLines);
            for (var i = 0; i < numberOfLines; i++)
            {
                builder.Append(Prefix);
                builder.Append(rand.Next(0, 100).ToString().PadLeft(2, ' '));
                builder.Append('\n');
            }

            return builder.ToString();
        }

        private readonly int PrefixLength = Prefix.Length;
        private ReadOnlyMemory<char> PrefixSpan = Prefix.AsMemory();

        /// <summary>
        /// Generates CSV content with a specified number of lines
        /// </summary>
        /// <param name="numberOfLines">Number of lines that the CSV string should contain</param>
        /// <returns>String containing CSV data</returns>
        /// <remarks>
        /// This implementation uses <see cref="System.Span{T}"/> and <see cref="System.Memory{T}"/> instead of 
        /// <see cref="System.Text.StringBuilder"/>.
        /// </remarks>
        public string GenerateHugeCsvWithSpan(int numberOfLines)
        {
            // Lets prepare string representations of all possible ages ([0..99])
            // Note that we must use Memory instead of Span as Span can only live on the heap.
            // In our case, we use ageStrings in a lambda function. Therefore, Memory is required.
            var ageStrings = new ReadOnlyMemory<char>[100];
            for (var i = 0; i < ageStrings.Length; i++)
            {
                ageStrings[i] = i.ToString().PadLeft(2, ' ').AsMemory();
            }

            // Let's use a string creation function that allows us to dynamically build the string's char buffer.
            return string.Create((Prefix.Length + 3) * numberOfLines, rand, (Span<char> chars, Random r) =>
            {
                for (var line = 0; line < numberOfLines; line++)
                {
                    // Get a slice for the current line
                    var lineSpan = chars.Slice(line * (PrefixLength + 3), PrefixLength + 3);

                    // Copy the prefix into the line
                    PrefixSpan.Span.CopyTo(lineSpan);

                    // Copy the random age into the line
                    ageStrings[r.Next(100)].Span.CopyTo(lineSpan.Slice(PrefixLength));

                    // Append the '\n'
                    lineSpan[PrefixLength + 2] = '\n';
                }
            });
        }

        /// <summary>
        /// Parse CSV data and calculate age statistics
        /// </summary>
        /// <param name="input">CSV data that should be parsed</param>
        public int[] StringParsing(string input)
        {
            // This array will receive the number of people per age group
            var ageStats = new int[10];
            
            // Parse and calculate using Linq
            var ageGroups = input.Split('\n')
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .GroupBy(line => int.Parse(line.Split(',')[2]) / 10);
            foreach (var ageGroup in ageGroups)
            {
                ageStats[ageGroup.Key] = ageGroup.Count();
            }

            return ageStats;
        }

        /// <summary>
        /// Parse CSV data and calculate age statistics
        /// </summary>
        /// <param name="input">CSV data that should be parsed</param>
        /// <remarks>
        /// This implementation uses <see cref="System.Span{T}"/> and <see cref="System.Memory{T}"/> instead of Linq.
        /// </remarks>
        public int[] StringParsingWithSpan(string input)
        {
            // This array will receive the number of people per age group
            var ageStats = new int[10];

            // Help variables
            byte foundCommas = 0;
            var ageStartIndex = -1;

            // Get a span for the input string
            ReadOnlySpan<char> inputSpan = input;

            // Local helper function for processing a single line
            void processLine(ReadOnlySpan<char> lineSpan, int ageBeginIndex, int ageLength)
            {
                // Ignore line in case of invalid number of commas
                if (foundCommas == 2)
                {
                    // Note that int.Parse can take a Span
                    var age = int.Parse(lineSpan.Slice(ageBeginIndex, ageLength));
                    ageStats[age / 10]++;
                }

                foundCommas = 0;
            }

            // Process input char by char
            for (var i = 0; i < inputSpan.Length; i++)
            {
                if (inputSpan[i] == ',')
                {
                    if (foundCommas == 1)
                    {
                        // We found the second comma so let's remember the begin index of the age
                        ageStartIndex = i + 1;
                    }

                    foundCommas++;
                }

                if (inputSpan[i] == '\n')
                {
                    // We found '\n' so let's process the line
                    processLine(inputSpan, ageStartIndex, i - ageStartIndex);
                }
            }

            // We are at the end of the input data, let's process the last line
            processLine(inputSpan, ageStartIndex, inputSpan.Length - ageStartIndex);

            return ageStats;
        }

        [Params(10, 1000, 100_000, 1_000_000)]
        public int NumberOfLines;

        [Benchmark]
        public void GenerateWithStringBuilder() => GenerateHugeCsv(NumberOfLines);

        [Benchmark]
        public void GenerateWithSpan() => GenerateHugeCsvWithSpan(NumberOfLines);

        [Benchmark]
        public void ParseWithLinq() => StringParsing(GenerateHugeCsv(NumberOfLines));

        [Benchmark]
        public void ParseWithSpan() => StringParsingWithSpan(GenerateHugeCsvWithSpan(NumberOfLines));
    }
}
