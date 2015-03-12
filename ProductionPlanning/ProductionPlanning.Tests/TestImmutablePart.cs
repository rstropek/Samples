using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductionPlanning.Logic;
using System;
using System.Collections.Generic;

namespace ProductionPlanning.Tests
{
	[TestClass]
	public class TestImmutablePart
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestMissingProductID()
		{
			Guid productID = Guid.NewGuid();
			try
			{
				new ImmutablePart(productID, 5, new List<ImmutableProduct>());
			}
			// Note new exception handling condition here
			catch (ArgumentException ex) if (!ex.Message.Contains(productID.ToString()))
			{
				// Suppress exception if message is not correct
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestRepositoryNullException()
		{
			new ImmutablePart(Guid.NewGuid(), 5, null);
		}
	}
}
