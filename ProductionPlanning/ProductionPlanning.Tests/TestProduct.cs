using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductionPlanning.Logic;

namespace ProductionPlanning.Tests
{
	[TestClass]
	public class TestProduct
	{
		[TestMethod]
		public void TestProductPropertyChanged()
		{
			var product = new Product();

			// Check if events are raised
			product.CheckIf(p =>
				{
					p.ProductID = Guid.NewGuid();
					p.Description = "Test";
					p.CostsPerItem = 99.99M;
				})
				.Raised("ProductID")
				.Raised("Description")
				.Raised("CostsPerItem")
				.DidNotRaise("Parts");

			// Check that no events are raised if value did not change
			product.CheckIf(p => p.CostsPerItem = p.CostsPerItem)
				.DidNotRaise("CostsPerItem");
		}
	}
}
