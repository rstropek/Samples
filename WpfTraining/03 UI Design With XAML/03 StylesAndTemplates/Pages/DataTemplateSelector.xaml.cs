using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace StylesAndTemplates.Pages
{
    /// <summary>
    /// Interaction logic for FileList1.xaml
    /// </summary>

    public partial class DataTemplateSelectorPage : System.Windows.Controls.Page
	{
		public DataTemplateSelectorPage()
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