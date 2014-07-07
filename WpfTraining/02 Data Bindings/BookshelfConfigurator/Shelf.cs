using System.Collections.ObjectModel;
using System.Linq;

namespace BookshelfConfigurator
{
	public class Shelf : NotificationObject
	{
		public Shelf()
		{
			this.Elements = new ObservableCollection<ShelfElement>();
			this.Elements.CollectionChanged += (_, __) => this.RaisePropertyChanged(() => this.TotalWidthInCm);
		}

		public ObservableCollection<ShelfElement> Elements { get; private set; }

		public double TotalWidthInCm
		{
			get { return this.Elements.Sum(se => ElementDimension.WidthInCm(se.Width)); }
		}
	}
}
