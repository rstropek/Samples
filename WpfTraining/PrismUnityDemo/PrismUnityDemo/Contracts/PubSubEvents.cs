using Prism.Events;

namespace PrismUnityDemo.Contracts
{
	public class CloseProductDetailEvent : PubSubEvent<IProductDetailViewModel>
	{
	}

    public class ProductSelectionChangedEvent : PubSubEvent<Product>
    {
    }
}
