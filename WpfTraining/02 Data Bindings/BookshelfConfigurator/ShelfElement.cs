using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace BookshelfConfigurator
{
	public class ShelfElement : NotificationObject
	{
		public ShelfElement()
		{
			this.Items = new ObservableCollection<ShelfItem>();
			this.Items.CollectionChanged += this.OnItemsChanged;
		}

		private void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems.Cast<ShelfItem>())
				{
					item.Parent = this;
				}
			}

			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems.Cast<ShelfItem>())
				{
					item.Parent = null;
				}
			}
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
