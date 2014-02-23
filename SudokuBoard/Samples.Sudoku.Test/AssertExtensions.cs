using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.Sudoku.Test
{
	public static class AssertExtensions
	{
		public static void ThrowsException<T>(Action body) where T : Exception
		{
			var exception = false;
			try
			{
				body();
			}
			catch (Exception ex)
			{
				exception = ex is T;
			}

			if (!exception)
			{
				throw new AssertFailedException();
			}
		}

		public async static Task ThrowsExceptionAsync<T>(Func<Task> body) where T : Exception
		{
			var exception = false;
			try
			{
				await body();
			}
			catch (Exception ex)
			{
				exception = ex is T;
			}

			if (!exception)
			{
				throw new AssertFailedException();
			}
		}

		public static T WaitForAsync<T>(Func<Task<T>> body)
		{
			var t = body();
			t.Wait();
			return t.Result;
		}
	}
}
