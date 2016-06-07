using System.Collections.ObjectModel;

namespace BookshelfConfigurator.Data
{
    public class ShelfElement : DeepNotificationObject
	{
		public ShelfElement()
		{
			this.Items = new ObservableCollection<ShelfItem>();
			this.Items.CollectionChanged += (_, e) => this.HandleCollectionChanged<ShelfItem>(
				e,
				item => item.Parent = this,
				item => item.Parent = null);
		}
	
		private ElementWidth WidthValue;
		public ElementWidth Width
		{
			get { return this.WidthValue; }
			set
			{
				if (this.WidthValue != value)
				{
					this.WidthValue = value;
					this.RaisePropertyChanged();
				}
			}
		}
		
		public ObservableCollection<ShelfItem> Items { get; private set; }
	}
}
