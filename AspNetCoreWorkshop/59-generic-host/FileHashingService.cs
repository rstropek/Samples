using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace GenericHost
{
    /// <summary>
    /// Moves new images from a given folder into a target folder and appends a file hash to the file name.
    /// </summary>
    class FileHashingService : IHostedService, IDisposable
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private FileSystemWatcher fsw;

        public FileHashingService(ILogger<FileHashingService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public void Dispose()
        {
            fsw?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting file hashing service");
            logger.LogInformation($"Watching folder '{configuration["folderToWatch"]}'");

            fsw = new FileSystemWatcher()
            {
                Path = configuration["folderToWatch"],
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.LastWrite,
                IncludeSubdirectories = false,
                Filter = "*.jpg",
            };
            fsw.Changed += (_, e) =>
            {
                using (var shHash = new SHA512Managed())
                {
                    byte[] fileHash;
                    try
                    {
                        using (var stream = File.OpenRead(e.FullPath))
                        {
                            fileHash = shHash.ComputeHash(stream);
                        }
                    }
                    catch (Exception)
                    {
                        // Ignoring files that cannot be opened
                        return;
                    }

                    var destFileName = BuildDestinationFileName(e.FullPath, fileHash);
                    if (!File.Exists(destFileName))
                    {
                        logger.LogInformation($"Moving '{e.FullPath}' to '{destFileName}'");
                        File.Move(e.FullPath, destFileName);
                    }
                    else
                    {
                        logger.LogInformation($"'{destFileName}' already exists, not moving any file");
                    }
                }
            };
            fsw.EnableRaisingEvents = true;

            return Task.CompletedTask;
        }

        private static string BuildDestinationFileName(string sourceFileName, byte[] fileHash)
        {
            // Note use of Span and string building
            var baseName = Path.Combine(Path.GetDirectoryName(sourceFileName), "target", $"{Path.GetFileNameWithoutExtension(sourceFileName)}-");
            var extension = Path.GetExtension(sourceFileName);
            var destFileName = string.Create(baseName.Length + extension.Length + fileHash.Length * 2, 0, (buffer, __) =>
            {
                baseName.AsSpan().CopyTo(buffer);

                // Copy hash into file name
                buffer = buffer.Slice(baseName.Length);
                foreach (var b in fileHash)
                {
                    $"{b,0:x2}".AsSpan().CopyTo(buffer.Slice(0, 2));
                    buffer = buffer.Slice(2);
                }

                extension.AsSpan().CopyTo(buffer);
            });

            return destFileName;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping face recognition service");

            fsw.Dispose();
            fsw = null;

            return Task.CompletedTask;
        }
    }
}
