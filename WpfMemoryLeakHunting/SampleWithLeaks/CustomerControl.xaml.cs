// Note that this example CONTAINS MEMORY LEAKS. I use it in workshops to show how you can use
// memory profilers to find typical .NET memory leaks.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace WpfApplication19
{
	// Export class so that it can be created by MEF
	[Export("CustomerView", typeof(UserControl))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public partial class CustomerControl : UserControl, IPartImportsSatisfiedNotification, 
		IDisposable // Note that class has to implement IDisposable as it contains a member 
					// (repository) that implements IDisposable, too.
	{
		private CustomerRepository repository = new CustomerRepository();

		[Import("PrintMenuItem")]
		private MenuItem PrintMenuItem;

		public CustomerControl()
		{
			InitializeComponent();

			// For simplicity we do not implement full MVVM here. Note that you should
			// use MVVM in practice when working with XAML.
			this.DataContext = this;
		}

		public IEnumerable<Customer> Customers
		{
			get
			{
				// In practice we would find more complex data access logic. In this simple
				// sample we just select all existing customers.
				return this.repository.Customers.ToArray();
			}
		}

		public void OnImportsSatisfied()
		{
			// Connect to click event in main menu to "print" this item.
			this.PrintMenuItem.Click += (s, ea) => Debug.WriteLine("Printing {0} ...", this.ToString());
		}

		// Implementation of IDisposable
		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.repository.Dispose();
				GC.SuppressFinalize(this);
			}
		}
	}
}
