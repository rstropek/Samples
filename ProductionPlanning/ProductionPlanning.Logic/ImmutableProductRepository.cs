using System.Collections.Immutable;
using System.Linq;

namespace ProductionPlanning.Logic
{
	/// <summary>
	/// Implements an immutable repository of products
	/// </summary>
	public class ImmutableProductRepository
	{
		#region Constructors
		// Note that we can write to read-only properties 
		// inside of this constructor.

		private ImmutableProductRepository()
		{
			this.Products = ImmutableList<ImmutableProduct>.Empty;
		}

		internal ImmutableProductRepository(ImmutableList<ImmutableProduct> products)
		{
			this.Products = products;
		}
		#endregion

		// Note read-only static property with initialization
		public static ImmutableProductRepository Empty { get; }
			= new ImmutableProductRepository();

		public ImmutableList<ImmutableProduct> Products { get; }

		/// <summary>
		/// Returns a new repository with the product added
		/// </summary>
		public ImmutableProductRepository Add(IProduct product)
		{
			// Create new immutable (composite) product
			var compositeProduct = product as ICompositeProduct;
			var immutableProduct = (compositeProduct != null && compositeProduct?.Parts?.Count() > 0)
				? new ImmutableCompositeProduct(compositeProduct, this.Products)
				: new ImmutableProduct(product);

			// Return a new repository 
			return new ImmutableProductRepository(this.Products.Add(immutableProduct));
		}
	}

	/// <summary>
	/// Acts as the base class for calculations done using the visitor pattern
	/// </summary>
	public abstract class VisitingProductCalculator<TResult>
	{
		public abstract TResult AggregateInterimResults(TResult a, TResult b);

		public abstract TResult VisitProduct(ImmutableProduct product);

		// Note the use of function-bodied member here
		public virtual TResult VisitPart(ImmutablePart part) =>
			this.Visit(part.Part);

		public virtual TResult VisitCompositeProduct(ImmutableCompositeProduct product) =>
			AggregateInterimResults(
				// Calculate current product
				this.VisitProduct(product),
				// Aggregate values of parts
				product.Parts.Aggregate(default(TResult), (interimResult, p) =>
					AggregateInterimResults(interimResult, this.VisitPart(p))));

		public TResult Visit(ImmutableProductRepository repository) =>
			// Aggregate values of all products in repository
			repository.Products.Aggregate(default(TResult), (interimResult, p) =>
				AggregateInterimResults(interimResult, this.Visit(p)));

		/// <summary>
		/// Dispatches the visit to specialized visitor functions
		/// </summary>
		public TResult Visit(ImmutableProduct product)
		{
			var compositeProduct = product as ImmutableCompositeProduct;
			return compositeProduct != null
				? this.VisitCompositeProduct(compositeProduct)
				: this.VisitProduct(product);
		}
	}
}
