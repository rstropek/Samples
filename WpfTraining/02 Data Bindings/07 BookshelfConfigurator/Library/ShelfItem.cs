using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BookshelfConfigurator.Data
{
	public class ShelfItem : NotificationObject
	{
		private ElementHeight HeightValue;
		public ElementHeight Height
		{
			get { return this.HeightValue; }
			set
			{
				if (this.HeightValue != value)
				{
					this.HeightValue = value;
					this.RaisePropertyChanged();
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

		private bool HasDoorValue;
		public bool HasDoor
		{
			get { return this.HasDoorValue; }
			set
			{
				if (this.HasDoorValue != value)
				{
					this.HasDoorValue = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private int NumberOfShelfsValue;
		public int NumberOfShelfs
		{
			get { return this.NumberOfShelfsValue; }
			set
			{
				if (this.NumberOfShelfsValue != value)
				{
					this.NumberOfShelfsValue = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private ShelfElement ParentValue;
		public ShelfElement Parent
		{
			get { return this.ParentValue; }
			internal set
			{
				if (this.ParentValue != value)
				{
					if (this.ParentValue != null)
					{
						this.ParentValue.PropertyChanged -= this.OnParentChanged;
					}

					if (value != null)
					{
						value.PropertyChanged += this.OnParentChanged;
					}

					this.ParentValue = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private void OnParentChanged(object _, PropertyChangedEventArgs ea)
		{
		}
	}
}
