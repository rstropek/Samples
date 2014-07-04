using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace WpfApplication19
{
	public class Customer : INotifyPropertyChanged
	{
		private string FirstNameValue;
		[Key]
		public string FirstName
		{
			get { return this.FirstNameValue; }
			set
			{
				if (this.FirstNameValue != value)
				{
					this.FirstNameValue = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private string LastNameValue;
		public string LastName
		{
			get { return this.LastNameValue; }
			set
			{
				if (this.LastNameValue != value)
				{
					this.LastNameValue = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
