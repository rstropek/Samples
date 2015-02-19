using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ProductionPlanning.Logic
{
	using ProductRepository = IEnumerable<ImmutableProduct>;

	/// <summary>
	/// Implements an immutable product
	/// </summary>
	public class ImmutableProduct : IProduct
	{
		public ImmutableProduct(IProduct source)
			: this(source.ProductID, source.Description, source.CostsPerItem)
		{ }

		public ImmutableProduct(Guid productID, string description, decimal costsPerItem)
		{
			// Note that we can write to read-only properties 
			// inside of this constructor.

			this.ProductID = productID;
			this.CostsPerItem = costsPerItem;

			// Note that we copy reference to string here; no need to 
			// duplicate string as string is immutable in C#.
			this.Description = description;
		}

		// Note read-only, auto-implemented properties
		public Guid ProductID { get; }
		public string Description { get; }
		public decimal CostsPerItem { get; }
	}

	/// <summary>
	/// Implements an immutable composite product
	/// </summary>
	public class ImmutableCompositeProduct : ImmutableProduct, ICompositeProduct
	{
		public ImmutableCompositeProduct(ICompositeProduct source, ProductRepository productRepository)
			: this(source.ProductID, source.Description, source.CostsPerItem,
				  source.Parts, productRepository)
		{ }

		public ImmutableCompositeProduct(Guid productID, string description,
			decimal costsPerItem, IEnumerable<IPart> parts,
			ProductRepository productRepository)
			: base(productID, description, costsPerItem)
		{
			#region Check preconditions
			if (productRepository == null)
			{
				// Note the new nameof operator here
				throw new ArgumentNullException(nameof(productRepository));
			}

			if (parts == null)
			{
				throw new ArgumentNullException(nameof(parts));
			}

			if (parts.Count() == 0)
			{
				throw new ArgumentException(
					@"Parts must not be empty for an immutable composite product. 
Create a product if it does not have parts.",
					nameof(parts));
			}
			#endregion

			// Note that we can write to read-only properties 
			// inside of this constructor.

			// Check if parts is already immutable. We can reuse the
			// parts-object tree if it is.
			this.Parts = parts as IImmutableList<ImmutablePart>;
			if (this.Parts == null)
			{
				// Not yet immutable, so copy into immutable

				// Create a builder to add all source items
				var resultBuilder = ImmutableList<ImmutablePart>.Empty.ToBuilder();
				resultBuilder.AddRange(parts.Select(item =>
				{
					// Check if item is already immutable. 
					// We can reuse the object if it is.
					var immutablePart = item as ImmutablePart;
					return immutablePart != null
						? immutablePart	// Already immutable, so reuse
						: new ImmutablePart( // Not yet immutable, so copy it
							item.ComponentProductID, item.Amount, productRepository);
				}));

				// Turn builder into immutable
				this.Parts = resultBuilder.ToImmutable();
			}
		}

		public IImmutableList<ImmutablePart> Parts { get; }
		IEnumerable<IPart> ICompositeProduct.Parts => this.Parts;
	}

	/// <summary>
	/// Implements an immutable part
	/// </summary>
	public class ImmutablePart : IPart
	{
		public ImmutablePart(Guid productID, int amount, ProductRepository productRepository)
		{
			#region Check preconditions
			if (productRepository == null)
			{
				throw new ArgumentNullException(nameof(productRepository));
			}
			#endregion

			// Note that we can write to read-only properties 
			// inside of this constructor.

			this.Amount = amount;
			this.Part = productRepository.SingleOrDefault(p => p.ProductID == productID);
			if (this.Part == null)
			{
				// Note string interpolation here
				throw new ArgumentException($"Could not find product with ID {productID} in repository", nameof(productID));
			}
		}

		public ImmutableProduct Part { get; }

		// Note implicit interface implementation with function-bodied property
		Guid IPart.ComponentProductID => this.Part.ProductID;

		public int Amount { get; }
	}
}
