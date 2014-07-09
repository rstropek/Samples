using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookshelfConfigurator.Data
{
	public abstract class DeepNotificationObject : NotificationObject
	{
		protected void HandleCollectionChanged<T>(NotifyCollectionChangedEventArgs e, Action<T> register = null, Action<T> unregister = null, Action final = null) 
		{
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems.Cast<T>())
				{
					if (register != null)
					{
						register(item);
					}

					var itemNotify = item as INotifyPropertyChanged;
					if (itemNotify != null)
					{
						itemNotify.PropertyChanged += this.OnChildChanged;
					}

					var itemDeepNotify = item as DeepNotificationObject;
					if (itemDeepNotify != null)
					{
						itemDeepNotify.DeepPropertyChanged += this.OnChildDeepChanged;
					}
				}
			}

			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems.Cast<T>())
				{
					if (unregister != null)
					{
						unregister(item);
					}

					var itemNotify = item as INotifyPropertyChanged;
					if (itemNotify != null)
					{
						itemNotify.PropertyChanged -= this.OnChildChanged;
					}

					var itemDeepNotify = item as DeepNotificationObject;
					if (itemDeepNotify != null)
					{
						itemDeepNotify.DeepPropertyChanged -= this.OnChildDeepChanged;
					}
				}
			}

			if (final != null)
			{
				final();
			}
		}

		private void OnChildChanged(object _, PropertyChangedEventArgs __)
		{
			this.RaiseDeepPropertyChanged();
		}

		private void OnChildDeepChanged(object _, EventArgs __)
		{
			this.RaiseDeepPropertyChanged();
		}

		protected override void RaisePropertyChanged(string propertyName = null)
		{
			base.RaisePropertyChanged(propertyName);
			this.RaiseDeepPropertyChanged();
		}

		protected void RaiseDeepPropertyChanged()
		{
			if (this.DeepPropertyChanged != null)
			{
				this.DeepPropertyChanged(this, new EventArgs());
			}
		}

		public event EventHandler DeepPropertyChanged;
	}
}
