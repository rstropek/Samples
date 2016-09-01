using System.Collections.Generic;
using System.Linq;

namespace PrismUnityDemo.Contracts
{
	public abstract class Repository
	{
		public abstract IQueryable<Product> SelectAllProducts();
	}
}
