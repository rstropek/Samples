using System;
using System.IO;
using System.Threading.Tasks;

namespace AsyncStreams
{
    // Good, old IDisposable...
    class MyWriterWrapper : IDisposable
    {
        private readonly StreamWriter writer;

        public MyWriterWrapper(string path)
        {
            writer = new StreamWriter(path);
        }

        public Task WriteLineAsync(string value) => writer.WriteLineAsync(value);

        public void Dispose()
        {
            writer.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    // Now we can have an async disposable. Take a look at the implementation of
    // StreamWriter.DisposeAsync and note async flush.
    // https://github.com/dotnet/corefx/blob/release/3.0/src/Common/src/CoreLib/System/IO/StreamWriter.cs#L231
    class MyWriterWrapperNew : IAsyncDisposable
    {
        private readonly StreamWriter writer;

        public MyWriterWrapperNew(string path)
        {
            writer = new StreamWriter(path);
        }

        public Task WriteLineAsync(string value) => writer.WriteLineAsync(value);

        public async ValueTask DisposeAsync()
        {
            await writer.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
