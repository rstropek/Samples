using CommandLine;

namespace EolFixer
{
    internal class Options
    {
        [Option('p', "pattern", HelpText = "Search pattern for files", Required = false)]
        public string? FilesPattern { get; set; }

        private const string DefaultPath = ".";

        [Option("path", Default = DefaultPath, HelpText = "Path where file are looked for", Required = false)]
        public string Path { get; set; } = DefaultPath;

        [Option('v', "verbose", HelpText = "Verbose logging", Required = false)]
        public bool Verbose { get; set; }

        [Option("dry-run", Default = false, HelpText = "Simulation mode, no changes are written", Required = false)]
        public bool DryRun { get; set; }

        public void Deconstruct(out string? pattern, out bool verbose, out bool dryRun)
        {
            pattern = FilesPattern;
            verbose = Verbose;
            dryRun = DryRun;
        }
    }
}
