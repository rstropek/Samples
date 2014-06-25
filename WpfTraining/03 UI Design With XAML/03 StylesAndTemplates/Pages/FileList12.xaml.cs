using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StylesAndTemplates.Pages
{
	/// <summary>
	/// Interaction logic for FileList1.xaml
	/// </summary>

	public partial class FileList12 : System.Windows.Controls.Page
	{
		public FileList12()
		{
			InitializeComponent();
		}
	}

	public class FileInfoTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item != null && item is FileInfo)
			{
				FileInfo fileInfo = (FileInfo)item;
				FrameworkElement frameworkElement = (FrameworkElement)container;
				if (fileInfo.IsReadOnly)
				{
					return (DataTemplate)frameworkElement.FindResource("ReadOnlyFileInfoTemplate");
				}
				else if (fileInfo.LastWriteTime.AddDays(14) > DateTime.Now)
				{
					return (DataTemplate)frameworkElement.FindResource("ModifiedFileInfoTemplate");
				}
			}
			return null;
		}
	}
}