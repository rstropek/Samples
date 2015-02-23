using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProductionPlanning.Logic;
using System;

namespace ProductionPlanning.Tests
{
	[TestClass]
	public class TestImmutableProductRepository
	{
		/// <summary>
		/// Visitor calculating total costs
		/// </summary>
		private class CostCalculator : VisitingProductCalculator<decimal>
		{
			// Note function-bodied syntax here

			public override decimal AggregateInterimResults(decimal a, decimal b) => 
				a + b;

			public override decimal VisitProduct(ImmutableProduct product) => 
				product.CostsPerItem;

			public override decimal VisitPart(ImmutablePart part) => 
				// Price of part * number of parts
				base.VisitPart(part) * part.Amount;
		}

		[TestMethod]
		public void TestProductRepositoryCreation()
		{
			Guid p1Guid;
			Guid p2Guid;

			// Test data
			const decimal p1Costs = 2M;
			const decimal p2Costs = 4M;
			const decimal p3Costs = 6M;
			const int p1InP2Amount = 2;
			const int p2InP3Amount = 4;
			const decimal p2TotalCosts = p2Costs + (p1Costs * p1InP2Amount);
			const decimal p3TotalCosts = p3Costs + (p2TotalCosts * p2InP3Amount);

			var repo = ImmutableProductRepository.Empty
				.Add(new Product(p1Guid = Guid.NewGuid(), "Test", p1Costs))
				.Add(new Product(p2Guid = Guid.NewGuid(), "Test", p2Costs,
					// p1 is part of p2
					new[] { new Part() { ComponentProductID = p1Guid, Amount = p1InP2Amount } }))
				.Add(new Product(Guid.NewGuid(), "Test", p3Costs,
					// p2 is part of p3
					new[] { new Part() { ComponentProductID = p2Guid, Amount = p2InP3Amount } }));

			// Check if visitor calculated correct total costs
			Assert.AreEqual(
				p1Costs + p2TotalCosts + p3TotalCosts,
				new CostCalculator().Visit(repo));
		}
	}
}
