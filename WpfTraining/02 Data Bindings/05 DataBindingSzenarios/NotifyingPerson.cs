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
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("FirstName"));
			}
		}

		public override string LastName
		{
			get { return base.LastName; }
			set
			{
				base.LastName = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("LastName"));
			}
		}
	}
}
