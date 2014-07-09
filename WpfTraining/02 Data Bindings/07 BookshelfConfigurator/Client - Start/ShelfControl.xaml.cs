using BookshelfConfigurator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookshelfConfigurator
{
	/// <summary>
	/// Interaction logic for ShelfControl.xaml
	/// </summary>
	public partial class ShelfControl : UserControl
	{
		public ShelfControl()
		{
			InitializeComponent();
		}

		public Shelf Bookshelf
		{
			get { return (Shelf)GetValue(BookshelfProperty); }
			set { SetValue(BookshelfProperty, value); }
		}

		public static readonly DependencyProperty BookshelfProperty =
			DependencyProperty.Register("Bookshelf", typeof(Shelf), typeof(ShelfControl), new PropertyMetadata(OnShelfChanged));

		private static void OnShelfChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var that = d as ShelfControl;
			var oldShelf = e.OldValue as Shelf;
			var newShelf = e.NewValue as Shelf;

			if (oldShelf != null)
			{
				oldShelf.DeepPropertyChanged -= that.OnShelfChanged;
			}

			if (newShelf != null)
			{
				newShelf.DeepPropertyChanged += that.OnShelfChanged;
			}

			that.OnShelfChanged(null, null);
		}

		private void OnShelfChanged(object sender, EventArgs e)
		{
			if (this.Bookshelf != null)
			{
				var builder = new StringBuilder();
				builder.Append(string.Format("Total width: {0}\n", this.Bookshelf.TotalWidthInCm));

				foreach (var element in Bookshelf.Elements)
				{
					builder.Append(string.Format("  Element: {0}\n", element.Width));
					foreach (var item in element.Items)
					{
						builder.Append(string.Format("    Item: {0} {1} {2} {3}\n", item.Width, item.Height, item.HasDoor, item.NumberOfShelfs));
					}
				}

				this.ShelfString = builder.ToString();
			}
			else
			{
				this.ShelfString = "NULL";
			}
		}

		public string ShelfString
		{
			get { return (string)GetValue(ShelfStringProperty); }
			private set { SetValue(ShelfStringPropertyKey, value); }
		}

		private static readonly DependencyPropertyKey ShelfStringPropertyKey =
			DependencyProperty.RegisterReadOnly("ShelfString", typeof(string), typeof(ShelfControl), new PropertyMetadata());
		public static readonly DependencyProperty ShelfStringProperty = ShelfStringPropertyKey.DependencyProperty;
	}
}
