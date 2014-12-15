using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Newtonsoft.Json;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;

namespace InstallerUI
{
	public class InstallerUIBootstrapper : BootstrapperApplication, IUIInteractionService
	{
		private BootstrapperApplicationData bootstrapperApplicationData;

		private Window installerMainWindow;
		private IntPtr installerMainWindowHandle;

		/// <summary>
		/// Entry point for WiX.
		/// </summary>
		protected override void Run()
		{
			this.Engine.Log(LogLevel.Verbose, "Running the custom WPF UI.");

			// Uncomment the following line to debug bootstrapper
			// Debugger.Launch();

			using (var container = this.SetupCompositionContainer())
			{
				// Get metadata from BootstrapperApplicationData.xml and add it to the log 
				// for demonstration purposes
				this.bootstrapperApplicationData = new BootstrapperApplicationData();
				this.Engine.Log(LogLevel.Verbose, JsonConvert.SerializeObject(this.bootstrapperApplicationData));

				// Create main window with associated view model
				this.Engine.Log(LogLevel.Verbose, "Creating a UI.");
				this.installerMainWindow = container.GetExportedValue<Window>("InstallerMainWindow");
				this.installerMainWindowHandle = new WindowInteropHelper(this.installerMainWindow).EnsureHandle();

				// Kick off detect which will populate the view models.
				this.Engine.Detect();

				// Show UI.
				if (this.Command.Display == Display.Passive || this.Command.Display == Display.Full)
				{
					this.installerMainWindow.Show();
				}

				System.Windows.Threading.Dispatcher.Run();

				this.Engine.Quit(0);
				this.Engine.Log(LogLevel.Verbose, "Exiting custom WPF UI.");
			}
		}

		private CompositionContainer SetupCompositionContainer()
		{
			var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
			var container = new CompositionContainer(catalog);
			container.ComposeExportedValue<BootstrapperApplication>(this);
			container.ComposeExportedValue<Engine>(this.Engine);
			container.ComposeExportedValue<IUIInteractionService>(this);
			return container;
		}

		public void ShowMessageBox(string message)
		{
			this.installerMainWindow.Dispatcher.BeginInvoke(new Action(() => MessageBox.Show(message)), null);
		}

		public void CloseUIAndExit()
		{
			this.installerMainWindow.Dispatcher.BeginInvoke(new Action(() => this.installerMainWindow.Close()));
		}

		public void RunOnUIThread(Action body)
		{
			this.installerMainWindow.Dispatcher.BeginInvoke(body, null);
		}

		public IntPtr GetMainWindowHandle()
		{
			return this.installerMainWindowHandle;
		}
	}
}
