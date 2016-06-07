using System;
using System.Windows.Controls;

namespace StylesAndTemplates.Pages
{
    /// <summary>
    /// Interaction logic for FileList18.xaml
    /// </summary>

    public partial class EventSetter : System.Windows.Controls.Page
	{
		public EventSetter()
		{
			InitializeComponent();
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Console.Out.WriteLine("Selected Item: " + ((ListBox)sender).SelectedItem.ToString());
		}
	}
}