using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IronPython.UI.Scripts
{
	public sealed class ScriptOutputStream : Stream
	{
		private Action<string> write;
		private Encoding encoding;
		private BlockingCollection<byte[]> chunks;
		private Task processingTask;

		public ScriptOutputStream(Action<string> write, Encoding encoding)
		{
			this.write = write;
			this.encoding = encoding;
			chunks = new BlockingCollection<byte[]>();
			this.processingTask = Task.Factory.StartNew(() =>
				{
					foreach (var chunk in chunks.GetConsumingEnumerable())
					{
						write(this.encoding.GetString(chunk));
					}
				}, 
				TaskCreationOptions.LongRunning);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			var chunk = new byte[count];
			Buffer.BlockCopy(buffer, offset, chunk, 0, count);
			this.chunks.Add(chunk);
		}

		public override void Close()
		{
			this.chunks.CompleteAdding();
			try 
			{ 
				this.processingTask.Wait(); 
			}
			finally 
			{ 
				base.Close(); 
			}
		}

		public override bool CanRead
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get { throw new NotImplementedException(); }
		}

		public override long Position
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}
	}
}
