using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookshelfConfigurator
{
	public class MainWindowViewModel : NotificationObject
	{
		public MainWindowViewModel()
		{
			this.Shelf = new Shelf();

			var element = new ShelfElement();
			element.Width = ElementWidth.Narrow;
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Narrow, Height = ElementHeight.Small, NumberOfShelfs = 0, HasDoor = false });
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Narrow, Height = ElementHeight.Small, NumberOfShelfs = 1, HasDoor = true });
			this.Shelf.Elements.Add(element);

			element = new ShelfElement();
			element.Width = ElementWidth.Medium;
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Medium, Height = ElementHeight.High, NumberOfShelfs = 2, HasDoor = false });
			this.Shelf.Elements.Add(element);
		}

		private Shelf ShelfValue;
		public Shelf Shelf
		{
			get { return this.ShelfValue; }
			private set
			{
				if (this.ShelfValue != value)
				{
					this.ShelfValue = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private object SelectedObjectValue;
		public object SelectedObject
		{
			get { return this.SelectedObjectValue; }
			set
			{
				if (this.SelectedObjectValue != value)
				{
					this.SelectedObjectValue = value;
					this.RaisePropertyChanged();
				}
			}
		}
	}
}
