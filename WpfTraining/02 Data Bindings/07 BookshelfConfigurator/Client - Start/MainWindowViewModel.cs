using BookshelfConfigurator.Data;
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
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Narrow, Height = ElementHeight.Small, NumberOfShelfs = 1, HasDoor = false });
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Narrow, Height = ElementHeight.Small, NumberOfShelfs = 0, HasDoor = true });
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Narrow, Height = ElementHeight.Small, NumberOfShelfs = 0, HasDoor = true });
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Narrow, Height = ElementHeight.Small, NumberOfShelfs = 1, HasDoor = false });
			this.Shelf.Elements.Add(element);

			element = new ShelfElement();
			element.Width = ElementWidth.Wide;
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Wide, Height = ElementHeight.Small, NumberOfShelfs = 1, HasDoor = false });
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Wide, Height = ElementHeight.High, NumberOfShelfs = 0, HasDoor = false });
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Wide, Height = ElementHeight.Small, NumberOfShelfs = 1, HasDoor = false });
			this.Shelf.Elements.Add(element);

			element = new ShelfElement();
			element.Width = ElementWidth.Narrow;
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Narrow, Height = ElementHeight.Small, NumberOfShelfs = 1, HasDoor = false });
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Narrow, Height = ElementHeight.Small, NumberOfShelfs = 0, HasDoor = true });
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Narrow, Height = ElementHeight.Small, NumberOfShelfs = 0, HasDoor = true });
			element.Items.Add(new ShelfItem() { Width = ElementWidth.Narrow, Height = ElementHeight.Small, NumberOfShelfs = 1, HasDoor = false });
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
	}
}
