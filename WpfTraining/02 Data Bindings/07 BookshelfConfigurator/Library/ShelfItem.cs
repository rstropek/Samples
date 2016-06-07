using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookshelfConfigurator.Data
{
    public class ShelfItem : NotificationObject/*, IDataErrorInfo*/, INotifyDataErrorInfo
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
					this.RaiseErrorsChanged();
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
					this.RaiseErrorsChanged();
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
			this.RaiseErrorsChanged(nameof(this.Width));
		}

		#region IDataErrorInfo implementation
		public string Error
		{
			get { throw new NotImplementedException(); }
		}

		public string this[string columnName]
		{
			get
			{
				var maxNumberOfShelfs = ElementDimension.MaximumNumberOfShelfs(this.Height);
				if (columnName == nameof(this.NumberOfShelfs))
				{
					if (this.NumberOfShelfs > maxNumberOfShelfs)
					{
						return string.Format("Too many shelfs; maximum for this height is {0}", maxNumberOfShelfs);
					}
				}
				else if (columnName == nameof(this.Width))
				{
					if (this.Width != this.Parent.Width)
					{
						return "Width of item does not fit to width of element";
					}
				}

				return string.Empty;
			}
		}
		#endregion

		#region INotifyDataErrorInfo implementation
		private void RaiseErrorsChanged([CallerMemberName]string propertyName = null)
		{
			if (this.ErrorsChanged != null)
			{
				this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
			}
		}

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public IEnumerable GetErrors(string propertyName)
		{
			return this.GetErrorsImpl(propertyName);
		}

		private IList<string> GetErrorsImpl(string propertyName)
		{
			var result = new List<string>();
			var maxNumberOfShelfs = ElementDimension.MaximumNumberOfShelfs(this.Height);
			if (propertyName == nameof(this.NumberOfShelfs))
			{
				if (this.NumberOfShelfs > maxNumberOfShelfs)
				{
					result.Add(string.Format("Too many shelfs; maximum for this height is {0}", maxNumberOfShelfs));
				}
			}
			else if (propertyName == nameof(this.Width))
			{
				if (this.Width != this.Parent.Width)
				{
					result.Add("Width of item does not fit to width of element");
				}
			}

			return result;
		}

		public bool HasErrors
		{
			get
			{
				return this.GetErrorsImpl(nameof(this.Width)).Count != 0
					|| this.GetErrorsImpl(nameof(this.NumberOfShelfs)).Count != 0;
			}
		}
		#endregion
	}
}
