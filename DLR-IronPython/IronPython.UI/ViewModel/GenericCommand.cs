using System;
using System.Windows.Input;
using System.ComponentModel;

namespace IronPython.UI.ViewModel
{
	public class GenericCommand : ICommand
	{
		private Func<object, bool> canExecute;
		private Action<object> execute;
		private INotifyPropertyChanged source;

		public GenericCommand(Func<object, bool> canExecute, Action<object> execute, INotifyPropertyChanged source = null)
		{
			this.canExecute = canExecute;
			this.execute = execute;
			if (source != null)
			{
				this.source = source;
				this.source.PropertyChanged += (s, e) => { if (this.CanExecuteChanged != null) this.CanExecuteChanged(this, new EventArgs()); };
			}
		}

		public bool CanExecute(object parameter)
		{
			return this.canExecute(parameter);
		}

		public event System.EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			this.execute(parameter);
		}
	}
}
