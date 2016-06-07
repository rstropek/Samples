using System;

namespace Samples
{
    /// <summary>
    /// Interaction logic for BindToSource.xaml
    /// </summary>

    public partial class BindingValidation : System.Windows.Controls.Page
	{
		public BindingValidation()
		{
			InitializeComponent();
            this.DataContext = this;
		}

        public TimeSpan ResultTime { get; set; }
	}
}