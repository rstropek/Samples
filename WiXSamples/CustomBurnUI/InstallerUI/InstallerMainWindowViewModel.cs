using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
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

			engine.StringVariables["Prerequisite"] = "1";
			engine.StringVariables["InstallLevel"] = "100";

			this.InstallCommandValue = new DelegateCommand(
				() => engine.Plan(LaunchAction.Install),
				() => this.State == InstallationState.DetectedAbsent);

			this.UninstallCommandValue = new DelegateCommand(
				() => engine.Plan(LaunchAction.Uninstall),
				() => this.State == InstallationState.DetectedPresent);

			// Setup event handlers
			// Note that we setup handlers for lots of events. Many of them just write to the log
			// for demonstration purposes.
			bootstrapper.Startup += (_, ea) => this.LogEvent("Startup");
			bootstrapper.Shutdown += (_, ea) => this.LogEvent("Shutdown");
			bootstrapper.SystemShutdown += (_, ea) => this.LogEvent("SystemShutdown", ea);
			bootstrapper.DetectBegin += (_, ea) =>
				{
					this.LogEvent("DetectBegin", ea);
					this.interactionService.RunOnUIThread(
						() => this.State = ea.Installed ? InstallationState.DetectedPresent : InstallationState.DetectedAbsent);
				};
			bootstrapper.DetectCompatiblePackage += (_, ea) => this.LogEvent("DetectCompatiblePackage", ea);
			bootstrapper.DetectForwardCompatibleBundle += (_, ea) => this.LogEvent("DetectForwardCompatibleBundle", ea);
			bootstrapper.DetectMsiFeature += (_, ea) => this.LogEvent("DetectMsiFeature", ea);
			bootstrapper.DetectPackageBegin += (_, ea) => this.LogEvent("DetectPackageBegin", ea);
			bootstrapper.DetectPackageComplete += (_, ea) => this.LogEvent("DetectPackageComplete", ea);
			bootstrapper.DetectPriorBundle += (_, ea) => this.LogEvent("DetectPriorBundle", ea);
			bootstrapper.DetectRelatedMsiPackage += (_, ea) => this.LogEvent("DetectRelatedMsiPackage", ea);
			bootstrapper.DetectTargetMsiPackage += (_, ea) => this.LogEvent("DetectTargetMsiPackage", ea);
			bootstrapper.DetectUpdate += (_, ea) => this.LogEvent("DetectUpdate", ea);
			bootstrapper.DetectUpdateBegin += (_, ea) => this.LogEvent("DetectUpdateBegin", ea);
			bootstrapper.DetectUpdateComplete += (_, ea) => this.LogEvent("DetectUpdateComplete", ea);
			bootstrapper.DetectRelatedBundle += (_, ea) =>
				{
					this.LogEvent("DetectRelatedBundle", ea);
					this.interactionService.RunOnUIThread(() => this.Downgrade |= ea.Operation == RelatedOperation.Downgrade);
				};
			bootstrapper.DetectComplete += (s, ea) =>
				{
					this.LogEvent("DetectComplete");
					this.DetectComplete(s, ea);
				};

			bootstrapper.ApplyBegin += (_, ea) => this.LogEvent("ApplyBegin");
			bootstrapper.ApplyComplete += (_, ea) =>
			{
				this.LogEvent("ApplyComplete", ea);

				// Everything is done, let's close the installer
				this.interactionService.ShowMessageBox(string.Format("Installation complete, Status = {0}", ea.Status));
				this.interactionService.CloseUIAndExit();
			};
			bootstrapper.Elevate += (_, ea) => this.LogEvent("Elevate", ea);
			bootstrapper.Error += (_, ea) => this.LogEvent("Error", ea);
			bootstrapper.ExecuteBegin += (_, ea) => this.LogEvent("ExecuteBegin", ea);
			bootstrapper.ExecuteComplete += (_, ea) => this.LogEvent("ExecuteComplete", ea);
			bootstrapper.ExecuteFilesInUse += (_, ea) => this.LogEvent("ExecuteFilesInUse", ea);
			bootstrapper.ExecuteMsiMessage += (_, ea) => this.LogEvent("ExecuteMsiMessage", ea);
			bootstrapper.ExecutePackageBegin += (_, ea) =>
			{
				this.LogEvent("ExecutePackageBegin", ea);
				this.interactionService.RunOnUIThread(() => this.CurrentPackage = ea.PackageId);
			};
			bootstrapper.ExecutePackageComplete += (_, ea) =>
			{
				this.LogEvent("ExecutePackageComplete", ea);
				this.interactionService.RunOnUIThread(() => this.CurrentPackage = string.Empty);
			};
			bootstrapper.ExecutePatchTarget += (_, ea) => this.LogEvent("ExecutePatchTarget", ea);
			bootstrapper.ExecuteProgress += (_, ea) =>
			{
				this.LogEvent("ExecuteProgress", ea);
				this.interactionService.RunOnUIThread(() =>
				{
					this.LocalProgress = ea.ProgressPercentage;
					this.GlobalProgress = ea.OverallPercentage;
				});
			};
			bootstrapper.LaunchApprovedExeBegin += (_, ea) => this.LogEvent("LaunchApprovedExeBegin");
			bootstrapper.LaunchApprovedExeComplete += (_, ea) => this.LogEvent("LaunchApprovedExeComplete", ea);
			bootstrapper.PlanBegin += (_, ea) => this.LogEvent("PlanBegin", ea);
			bootstrapper.PlanCompatiblePackage += (_, ea) => this.LogEvent("PlanCompatiblePackage", ea);
			bootstrapper.PlanComplete += (_, ea) => 
				{
					this.LogEvent("PlanComplete", ea);
					if (ea.Status >= 0 /* Success */)
					{
						this.engine.Apply(this.interactionService.GetMainWindowHandle());
					}
				};
			bootstrapper.PlanMsiFeature += (_, ea) => this.LogEvent("PlanMsiFeature", ea);
			bootstrapper.PlanPackageBegin += (_, ea) =>this.LogEvent("PlanPackageBegin", ea);
			bootstrapper.PlanPackageComplete += (_, ea) => this.LogEvent("PlanPackageComplete", ea);
			bootstrapper.PlanRelatedBundle += (_, ea) => this.LogEvent("PlanRelatedBundle", ea);
			bootstrapper.PlanTargetMsiPackage += (_, ea) => this.LogEvent("PlanTargetMsiPackage", ea);
			bootstrapper.Progress += (_, ea) => this.LogEvent("Progress", ea);
			bootstrapper.RegisterBegin += (_, ea) => this.LogEvent("RegisterBegin");
			bootstrapper.RegisterComplete += (_, ea) => this.LogEvent("RegisterComplete", ea);
			bootstrapper.ResolveSource += (_, ea) => this.LogEvent("ResolveSource", ea);
			bootstrapper.RestartRequired += (_, ea) => this.LogEvent("RestartRequired", ea);
			bootstrapper.UnregisterBegin += (_, ea) => this.LogEvent("UnregisterBegin", ea);
			bootstrapper.UnregisterComplete += (_, ea) => this.LogEvent("UnregisterComplete", ea);
		}

		private void LogEvent(string eventName, EventArgs arguments = null)
		{
			this.engine.Log(
				LogLevel.Verbose,
				arguments == null ? string.Format("EVENT: {0}", eventName) : string.Format("EVENT: {0} ({1})", eventName, JsonConvert.SerializeObject(arguments)));
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
	}
}
