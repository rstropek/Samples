using System.Collections.Generic;
using System.Linq;

namespace PrismUnityDemo.Contracts
{
	public interface IRepository
	{
		IQueryable<Product> SelectAllProducts();
	}
}
