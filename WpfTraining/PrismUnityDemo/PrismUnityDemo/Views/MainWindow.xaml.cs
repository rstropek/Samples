using Microsoft.Practices.Unity;
using Prism.Events;
using PrismUnityDemo.Contracts;
using System.Windows;

namespace PrismUnityDemo.Views
{
    public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			this.DataContext = this;
		}
        public GlobalCommands GlobalCommands { get; private set; }

        [InjectionMethod]
		public void OnInitialize(GlobalCommands globalCommands, IEventAggregator eventAggregator,
            IUnityContainer container)
		{
            this.GlobalCommands = globalCommands;

            var navController = container.Resolve<MainWindowNavigationController>();
			navController.RegisterDefaultSubscriptions(eventAggregator);
		}
	}
}
