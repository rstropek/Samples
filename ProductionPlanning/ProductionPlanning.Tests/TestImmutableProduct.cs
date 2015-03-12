using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductionPlanning.Logic;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace ProductionPlanning.Tests
{
	[TestClass]
	public class TestImmutableProduct
	{
		[TestMethod]
		public void TestProductCreation()
		{
			// Create an immutable product
			var ip = new ImmutableProduct(Guid.NewGuid(), "Test", 1M);
			Assert.IsNotNull(ip);

			// Create an immutable composite product. Not that both parts
			// refer to the same product. Therefore the ImmutableProduct
			// object has to be reused.
			var icp = new ImmutableCompositeProduct(
				Guid.NewGuid(), "Test", 1M,
				new[] {
					new Part() { ComponentProductID = ip.ProductID, Amount = 5 },
					new Part() { ComponentProductID = ip.ProductID, Amount = 10 }
				},
				new[] { ip });
			Assert.IsNotNull(ip);
			Assert.AreSame(ip, icp.Parts[0].Part);
			Assert.AreSame(icp.Parts[0].Part, icp.Parts[1].Part);

			// Create an immutable composite product. Note that the part
			// list as already immutable. Therefore it has to be reused
			// and must not be copied.
			var iplist = ImmutableList<ImmutablePart>.Empty
				.Add(new ImmutablePart(ip.ProductID, 5, new[] { ip }));
			icp = new ImmutableCompositeProduct(
				Guid.NewGuid(), "Test", 1M, iplist, new[] { ip });
			Assert.AreSame(icp.Parts, iplist);

			// Create an immutable composite product. Note that the part
			// inside the part list is already immutable. Therefore it has 
			// to be reused and must not be copied.
			icp = new ImmutableCompositeProduct(
				Guid.NewGuid(), "Test", 1M, new[] { iplist[0] }, new[] { ip });
			Assert.AreSame(icp.Parts[0], iplist[0]);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestPartsNullException()
		{
			new ImmutableCompositeProduct(Guid.NewGuid(), "Test", 1M,
				null, new List<ImmutableProduct>());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestRepositoryNullException()
		{
			new ImmutableCompositeProduct(Guid.NewGuid(), "Test", 1M,
				new List<Part>(), null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TestCompositeProductWithoutPartsException()
		{
			new ImmutableCompositeProduct(Guid.NewGuid(), "Test", 1M,
				new List<Part>(), new List<ImmutableProduct>());
		}
	}
}
