using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Newtonsoft.Json;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace InstallerUI
{
	[Export]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public class InstallerMainWindowViewModel : BindableBase
	{
		private BootstrapperApplication bootstrapper;
		private Engine engine;

		[Import]
		private IUIInteractionService interactionService = null;

		[ImportingConstructor]
		public InstallerMainWindowViewModel(BootstrapperApplication bootstrapper, Engine engine)
		{
			this.bootstrapper = bootstrapper;
			this.engine = engine;

			// For demo purposes, we set two variables here. They are passed on to the chained MSIs.
			engine.StringVariables["Prerequisite"] = "1";
			engine.StringVariables["InstallLevel"] = "100";

			// Setup commands
			this.InstallCommandValue = new DelegateCommand(
				() => engine.Plan(LaunchAction.Install),
				() => !this.Installing && this.State == InstallationState.DetectedAbsent);
			this.UninstallCommandValue = new DelegateCommand(
				() => engine.Plan(LaunchAction.Uninstall),
				() => !this.Installing && this.State == InstallationState.DetectedPresent);

			// Setup event handlers
			bootstrapper.DetectBegin += (_, ea) =>
				{
					this.LogEvent("DetectBegin", ea);

					// Set installation state that controls the install/uninstall buttons
					this.interactionService.RunOnUIThread(
						() => this.State = ea.Installed ? InstallationState.DetectedPresent : InstallationState.DetectedAbsent);
				};
			bootstrapper.DetectRelatedBundle += (_, ea) =>
				{
					this.LogEvent("DetectRelatedBundle", ea);

					// Save flag indicating whether this is a downgrade operation
					this.interactionService.RunOnUIThread(() => this.Downgrade |= ea.Operation == RelatedOperation.Downgrade);
				};
			bootstrapper.DetectComplete += (s, ea) =>
				{
					this.LogEvent("DetectComplete");
					this.DetectComplete(s, ea);
				};
			bootstrapper.PlanComplete += (_, ea) =>
				{
					this.LogEvent("PlanComplete", ea);

					// Start apply phase
					if (ea.Status >= 0 /* Success */)
					{
						this.engine.Apply(this.interactionService.GetMainWindowHandle());
					}
				};
			bootstrapper.ApplyBegin += (_, ea) =>
				{
					this.LogEvent("ApplyBegin");

					// Set flag indicating that apply phase is running
					this.interactionService.RunOnUIThread(() => this.Installing = true);
				};
			bootstrapper.ExecutePackageBegin += (_, ea) =>
				{
					this.LogEvent("ExecutePackageBegin", ea);

					// Trigger display of currently processed package
					this.interactionService.RunOnUIThread(() => this.CurrentPackage = ea.PackageId);
				};
			bootstrapper.ExecutePackageComplete += (_, ea) =>
				{
					this.LogEvent("ExecutePackageComplete", ea);

					// Remove currently processed package
					this.interactionService.RunOnUIThread(() => this.CurrentPackage = string.Empty);
				};
			bootstrapper.ExecuteProgress += (_, ea) =>
			{
				this.LogEvent("ExecuteProgress", ea);

				// Update progress indicator
				this.interactionService.RunOnUIThread(() =>
				{
					this.LocalProgress = ea.ProgressPercentage;
					this.GlobalProgress = ea.OverallPercentage;
				});
			};
			bootstrapper.ApplyComplete += (_, ea) =>
				{
					this.LogEvent("ApplyComplete", ea);

					// Everything is done, let's close the installer
					this.interactionService.CloseUIAndExit();
				};
		}

		private void SetupEventHandlersForLogging()
		{
			this.bootstrapper.Startup += (_, ea) => this.LogEvent("Startup");
			this.bootstrapper.Shutdown += (_, ea) => this.LogEvent("Shutdown");
			this.bootstrapper.SystemShutdown += (_, ea) => this.LogEvent("SystemShutdown", ea);
			this.bootstrapper.DetectCompatiblePackage += (_, ea) => this.LogEvent("DetectCompatiblePackage", ea);
			this.bootstrapper.DetectForwardCompatibleBundle += (_, ea) => this.LogEvent("DetectForwardCompatibleBundle", ea);
			this.bootstrapper.DetectMsiFeature += (_, ea) => this.LogEvent("DetectMsiFeature", ea);
			this.bootstrapper.DetectPackageBegin += (_, ea) => this.LogEvent("DetectPackageBegin", ea);
			this.bootstrapper.DetectPackageComplete += (_, ea) => this.LogEvent("DetectPackageComplete", ea);
			this.bootstrapper.DetectPriorBundle += (_, ea) => this.LogEvent("DetectPriorBundle", ea);
			this.bootstrapper.DetectRelatedMsiPackage += (_, ea) => this.LogEvent("DetectRelatedMsiPackage", ea);
			this.bootstrapper.DetectTargetMsiPackage += (_, ea) => this.LogEvent("DetectTargetMsiPackage", ea);
			this.bootstrapper.DetectUpdate += (_, ea) => this.LogEvent("DetectUpdate", ea);
			this.bootstrapper.DetectUpdateBegin += (_, ea) => this.LogEvent("DetectUpdateBegin", ea);
			this.bootstrapper.DetectUpdateComplete += (_, ea) => this.LogEvent("DetectUpdateComplete", ea);
			this.bootstrapper.Elevate += (_, ea) => this.LogEvent("Elevate", ea);
			this.bootstrapper.Error += (_, ea) => this.LogEvent("Error", ea);
			this.bootstrapper.ExecuteBegin += (_, ea) => this.LogEvent("ExecuteBegin", ea);
			this.bootstrapper.ExecuteComplete += (_, ea) => this.LogEvent("ExecuteComplete", ea);
			this.bootstrapper.ExecuteFilesInUse += (_, ea) => this.LogEvent("ExecuteFilesInUse", ea);
			this.bootstrapper.ExecuteMsiMessage += (_, ea) => this.LogEvent("ExecuteMsiMessage", ea);
			this.bootstrapper.ExecutePatchTarget += (_, ea) => this.LogEvent("ExecutePatchTarget", ea);
			this.bootstrapper.LaunchApprovedExeBegin += (_, ea) => this.LogEvent("LaunchApprovedExeBegin");
			this.bootstrapper.LaunchApprovedExeComplete += (_, ea) => this.LogEvent("LaunchApprovedExeComplete", ea);
			this.bootstrapper.PlanBegin += (_, ea) => this.LogEvent("PlanBegin", ea);
			this.bootstrapper.PlanCompatiblePackage += (_, ea) => this.LogEvent("PlanCompatiblePackage", ea);
			this.bootstrapper.PlanMsiFeature += (_, ea) => this.LogEvent("PlanMsiFeature", ea);
			this.bootstrapper.PlanPackageBegin += (_, ea) => this.LogEvent("PlanPackageBegin", ea);
			this.bootstrapper.PlanPackageComplete += (_, ea) => this.LogEvent("PlanPackageComplete", ea);
			this.bootstrapper.PlanRelatedBundle += (_, ea) => this.LogEvent("PlanRelatedBundle", ea);
			this.bootstrapper.PlanTargetMsiPackage += (_, ea) => this.LogEvent("PlanTargetMsiPackage", ea);
			this.bootstrapper.Progress += (_, ea) => this.LogEvent("Progress", ea);
			this.bootstrapper.RegisterBegin += (_, ea) => this.LogEvent("RegisterBegin");
			this.bootstrapper.RegisterComplete += (_, ea) => this.LogEvent("RegisterComplete", ea);
			this.bootstrapper.ResolveSource += (_, ea) => this.LogEvent("ResolveSource", ea);
			this.bootstrapper.RestartRequired += (_, ea) => this.LogEvent("RestartRequired", ea);
			this.bootstrapper.UnregisterBegin += (_, ea) => this.LogEvent("UnregisterBegin", ea);
			this.bootstrapper.UnregisterComplete += (_, ea) => this.LogEvent("UnregisterComplete", ea);
		}

		#region Properties for data binding
		private DelegateCommand InstallCommandValue;
		public ICommand InstallCommand { get { return this.InstallCommandValue; } }

		private DelegateCommand UninstallCommandValue;
		public ICommand UninstallCommand { get { return this.UninstallCommandValue; } }

		private InstallationState StateValue;
		public InstallationState State
		{
			get { return this.StateValue; }
			set
			{
				this.SetProperty(ref this.StateValue, value);
				this.InstallCommandValue.RaiseCanExecuteChanged();
				this.UninstallCommandValue.RaiseCanExecuteChanged();
			}
		}

		private bool DowngradeValue;
		public bool Downgrade
		{
			get { return this.DowngradeValue; }
			set { this.SetProperty(ref this.DowngradeValue, value); }
		}

		private int LocalProgressValue;
		public int LocalProgress
		{
			get { return this.LocalProgressValue; }
			set { this.SetProperty(ref this.LocalProgressValue, value); }
		}

		private int GlobalProgressValue;
		public int GlobalProgress
		{
			get { return this.GlobalProgressValue; }
			set { this.SetProperty(ref this.GlobalProgressValue, value); }
		}

		private string CurrentPackageValue;
		public string CurrentPackage
		{
			get { return this.CurrentPackageValue; }
			set { this.SetProperty(ref this.CurrentPackageValue, value); }
		}

		private bool InstallingValue;
		public bool Installing
		{
			get { return this.InstallingValue; }
			set
			{
				this.SetProperty(ref this.InstallingValue, value);
				this.InstallCommandValue.RaiseCanExecuteChanged();
				this.UninstallCommandValue.RaiseCanExecuteChanged();
			}
		}
		#endregion

		private void DetectComplete(object sender, DetectCompleteEventArgs e)
		{
			// If necessary, parse the command line string before any planning
			// (e.g. detect installation folder)

			if (LaunchAction.Uninstall == this.bootstrapper.Command.Action)
			{
				this.engine.Log(LogLevel.Verbose, "Invoking automatic plan for uninstall");
				this.engine.Plan(LaunchAction.Uninstall);
			}
			else if (e.Status >= 0 /* Success */)
			{
				if (this.Downgrade)
				{
					// What do you want to do in case of downgrade?
					// Here: Stop installation

					string message = "Sorry, we do not support downgrades.";
					this.engine.Log(LogLevel.Verbose, message);
					if (this.bootstrapper.Command.Display == Display.Full)
					{
						this.interactionService.ShowMessageBox(message);
						this.interactionService.CloseUIAndExit();
					}
				}

				if (this.bootstrapper.Command.Action == LaunchAction.Layout)
				{
					// Copies all of the Bundle content to a specified directory
					this.engine.Plan(LaunchAction.Layout);
				}
				else if (this.bootstrapper.Command.Display != Display.Full)
				{
					// If we're not waiting for the user to click install, dispatch plan with the default action.
					this.engine.Log(LogLevel.Verbose, "Invoking automatic plan for non-interactive mode.");
					this.engine.Plan(LaunchAction.Install);
				}
			}
		}

		private void LogEvent(string eventName, EventArgs arguments = null)
		{
			this.engine.Log(
				LogLevel.Verbose,
				arguments == null ? string.Format("EVENT: {0}", eventName) : string.Format("EVENT: {0} ({1})", eventName, JsonConvert.SerializeObject(arguments)));
		}
	}
}
