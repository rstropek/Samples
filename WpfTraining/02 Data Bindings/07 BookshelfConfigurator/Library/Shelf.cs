using System.Collections.ObjectModel;
using System.Linq;

namespace BookshelfConfigurator.Data
{
    public class Shelf : DeepNotificationObject
	{
		public Shelf()
		{
			this.Elements = new ObservableCollection<ShelfElement>();
			this.Elements.CollectionChanged += (_, e) => this.HandleCollectionChanged<ShelfElement>(e, final: () => this.RaisePropertyChanged(nameof(TotalWidthInCm)));
		}

		public ObservableCollection<ShelfElement> Elements { get; private set; }

		public double TotalWidthInCm
		{
			get { return this.Elements.Sum(se => ElementDimension.WidthInCm(se.Width)); }
		}
	}
}
