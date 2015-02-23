using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductionPlanning.Logic;
using System.Collections.Generic;
using System.Linq;

namespace ProductionPlanning.Tests
{
	[TestClass]
	public class TestProduct
	{
		[TestMethod]
		public void TestProductDictionary()
		{
			const int numberOfPartsInP = 10;
			var pRoot = new Product(Guid.NewGuid(), "Test", 10M);
			Product p;

			// Note the new dictionary initializers here
			var dict = new Dictionary<Product, Tuple<Guid, int>>()
			{
				[new Product(Guid.NewGuid(), "Test2", 20M)] = new Tuple<Guid, int>(pRoot.ProductID, 5),
				[p = new Product(Guid.NewGuid(), "Test3", 30M)] = new Tuple<Guid, int>(pRoot.ProductID, numberOfPartsInP),
				[new Product(Guid.NewGuid(), "Test4", 40M)] = new Tuple<Guid, int>(pRoot.ProductID, 15)
			};

			// Just for fun, let's change p's description
			p.Description = "Subproduct";

			Assert.AreEqual(numberOfPartsInP, dict[p].Item2);
		}

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
				.Raised(nameof(Product.ProductID))
				.Raised(nameof(Product.Description))
				.Raised(nameof(Product.CostsPerItem))
				.DidNotRaise(nameof(Product.Parts));

			// Check that no events are raised if value did not change
			product.CheckIf(p => p.CostsPerItem = p.CostsPerItem)
				.DidNotRaise(nameof(Product.CostsPerItem));
		}


		[TestMethod]
		public void TestProductErrorsChanged()
		{
			const decimal validValue = 5M;
			const decimal invalidValue = -5M;

			// Create a product with valid data
			var product = new Product(Guid.NewGuid(), "Test", validValue);

			// Check that no errors are reported
			Assert.IsFalse(product.HasErrors);
			Assert.IsTrue(string.IsNullOrEmpty(
				product.GetErrors(null).Cast<string>().First()));

			// Change product to invalid content and validate if
			// events are properly raised
			product.CheckIf(p => p.CostsPerItem = invalidValue)
				.Raised(nameof(Product.CostsPerItem))
				.RaisedErrorsChanged(nameof(Product.CostsPerItem));

			// Check if error status is reported
			Assert.IsTrue(product.HasErrors);
			Assert.IsFalse(string.IsNullOrEmpty(
				product.GetErrors(null).Cast<string>().First()));
			Assert.IsFalse(string.IsNullOrEmpty(
				product.GetErrors(nameof(Product.CostsPerItem)).Cast<string>().First()));
			Assert.IsTrue(string.IsNullOrEmpty(
				product.GetErrors(nameof(Product.Description)).Cast<string>().First()));

			// Call setter with identical value and check that events 
			// are not raised
			product.CheckIf(p => p.CostsPerItem = p.CostsPerItem)
				.DidNotRaise(nameof(Product.CostsPerItem))
				.DidNotRaiseErrorsChanged(nameof(Product.CostsPerItem));

			// Call setter with valid value and check that events
			// are properly raised.
			product.CheckIf(p => p.CostsPerItem = validValue)
				.Raised(nameof(Product.CostsPerItem))
				.RaisedErrorsChanged(nameof(Product.CostsPerItem));

			// Check error status has been set to OK
			Assert.IsFalse(product.HasErrors);
			Assert.IsTrue(string.IsNullOrEmpty(
				product.GetErrors(null).Cast<string>().First()));
		}
	}
}
