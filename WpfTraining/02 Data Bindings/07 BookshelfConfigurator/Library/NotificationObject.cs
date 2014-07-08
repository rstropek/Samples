using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BookshelfConfigurator.Data
{
	public class NotificationObject : INotifyPropertyChanged
	{
		protected static string PropertyName<T>(Expression<Func<T>> ex)
		{
			var lambda = ex as LambdaExpression;
			if (lambda != null)
			{
				var memberAccess = lambda.Body as MemberExpression;
				return memberAccess != null ? memberAccess.Member.Name : string.Empty;
			}

			return string.Empty;
		}

		protected void RaisePropertyChanged<T>(Expression<Func<T>> ex)
		{
			this.RaisePropertyChanged(NotificationObject.PropertyName(ex));
		}

		protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
