using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductionPlanning.Logic
{
	/// <summary>
	/// Represents a product's base data
	/// </summary>
	public interface IProduct
	{
		[Key]
		Guid ProductID { get; }

		string Description { get; }

		decimal CostsPerItem { get; }
	}

	/// <summary>
	/// Represents a product that consists of other products (=parts)
	/// </summary>
	public interface ICompositeProduct : IProduct
	{
		IEnumerable<IPart> Parts { get; }
	}

	/// <summary>
	/// Represents a part's base data
	/// </summary>
	public interface IPart
	{
		Guid ComponentProductID { get; }

		int Amount { get; }
	}
}
