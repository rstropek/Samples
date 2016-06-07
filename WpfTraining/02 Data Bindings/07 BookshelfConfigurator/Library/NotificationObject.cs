using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookshelfConfigurator.Data
{
    public abstract class NotificationObject : INotifyPropertyChanged
	{
		protected virtual void RaisePropertyChanged([CallerMemberName]string propertyName = null)
		{
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
