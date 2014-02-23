namespace Samples.Sudoku.Test
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using System.Diagnostics.Contracts;

	[TestClass]
	public class InitializeTests
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext tc)
		{
			////Contract.ContractFailed += (sender, e) =>
			////{
			////	e.SetUnwind(); // cause code to abort after event
			////	Assert.Fail(e.FailureKind.ToString() + ":" + e.Message);
			////};
		}
	}
}
