using System.ComponentModel;

namespace Samples
{
	class NotifyingPerson : Person, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public override string FirstName
		{
			get { return base.FirstName; }
			set
			{
				base.FirstName = value;
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstName)));
			}
		}

		public override string LastName
		{
			get { return base.LastName; }
			set
			{
				base.LastName = value;
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastName)));
			}
		}
	}
}
