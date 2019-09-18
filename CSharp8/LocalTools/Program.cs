using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EolFixer
{
    public class Program
    {
        #region Helper methods
        private static void DisplayDryRunWarningMessage()
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Dry-run mode, no changes will be written.");
            Console.ForegroundColor = oldColor;
        }
        #endregion

        public static void Main(string[] args)
        {
            // Parse command line arguments
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    // Command line arguments parsed successfully

                    // Display a warning message if we are in dry-run mode
                    if (options.DryRun)
                    {
                        DisplayDryRunWarningMessage();
                    }

                    // Process the specified folder
                    ProcessFolder(options.Path, options);
                });
        }

        private static void ProcessFolder(string path, Options options)
        {
            // Make sure that specified path exists
            if (!Directory.Exists(path))
            {
                throw new IOException($"Specified path '{path}' does not exist.");
            }

            // Note use of deconstructor
            var (searchPattern, verbose, _) = options;
            ProcessFiles(path, options, searchPattern);
            ProcessSubdirectories(path, options, verbose);
        }

        private static void ProcessSubdirectories(string path, Options options, bool verbose)
        {
            foreach (var subPath in Directory.EnumerateDirectories(path))
            {
                if (verbose)
                {
                    Console.WriteLine($"Start processing of subfolder {subPath}");
                }

                ProcessFolder(subPath, options);

                if (verbose)
                {
                    Console.WriteLine($"Finished processing of subfolder {subPath}");
                }
            }
        }

        private static void ProcessFiles(string path, Options options, string? searchPattern)
        {
            IEnumerable<string> fileEnumerator;
            if (searchPattern == null)
            {
                fileEnumerator = Directory.EnumerateFiles(path);
            }
            else
            {
                fileEnumerator = Directory.EnumerateFiles(path, searchPattern);
            }

            // Replace end-of-lines in all files
            foreach (var file in fileEnumerator)
            {
                ReplaceEol(file, options);
            }
        }

        private static void ReplaceEol(string filePath, Options options)
        {
            var (_, verbose, dryRun) = options;
            if (verbose)
            {
                Console.WriteLine($"Start processing of file {filePath}");
            }

            string source;
            Encoding encoding;
            using (var reader = new StreamReader(filePath))
            {
                encoding = reader.CurrentEncoding;
                source = reader.ReadToEnd();
            }

            source = source.Replace("\r\n", "\n");

            using (var writer = new StreamWriter(filePath, dryRun, encoding))
            {
                if (!dryRun)
                {
                    writer.Write(source);
                }
            }

            if (verbose)
            {
                Console.WriteLine($"Finished processing of file {filePath}");
            }
        }
    }
}
