using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using IronPython.Data;
using IronPython.Hosting;
using System.IO;
using System.Text;
using System.Windows;
using System;
using IronPython.UI.Scripts;
using Microsoft.Scripting;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using System.Diagnostics;

namespace IronPython.UI.ViewModel
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		private SamplesEntities context;

		public MainWindowViewModel()
		{
			this.context = new SamplesEntities();//"Data Source=IronPythonSample.sdf");

			this.ShowSpeakersCommand = new GenericCommand(
				x => this.WindowContent == WindowContent.Sessions,
				x => this.WindowContent = WindowContent.Speakers,
				this);

			this.ShowSessionsCommand = new GenericCommand(
				x => this.WindowContent == WindowContent.Speakers,
				x => this.WindowContent = WindowContent.Sessions,
				this);

			this.RunScriptCommand = new GenericCommand(
				x => true,
				x => 
					{
						OpenFileDialog dlg = new OpenFileDialog();
						dlg.DefaultExt = ".py"; 
						dlg.Filter = "Python script (.py)|*.py"; 
						var result = dlg.ShowDialog();
						if (result.HasValue && result.Value)
						{
							Task.Factory.StartNew(() => this.RunScript(dlg.FileName));
						}
					});

			this.EraseOutputCommand = new GenericCommand(
				x => true,
				x => 
					{ 
						lock (this.scriptOutput) 
						{ 
							this.scriptOutput = new StringBuilder(); 
						}

						this.OnPropertyChanged("ScriptOutput");
					});

			this.ApproveSessionCommand = new GenericCommand(
				x => this.SelectedSession != null,
				x =>
					{
						var engine = Python.CreateEngine();
						var scope = engine.CreateScope();
						scope.SetVariable("viewModel", this);
						engine.CreateScriptSourceFromString(@"
viewModel.SelectedSession.Approved = True
viewModel.SaveChanges()
").Execute(scope);
					},
				this);

			this.SpeakerExportCommand = new GenericCommand(
				x => true,
				x =>
				{
					var engine = Python.CreateEngine();
					var scope = engine.CreateScope();
					var scriptSource = @"
import clr
clr.AddReferenceByName('Microsoft.Office.Interop.Excel, Version=11.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c')
clr.AddReference(""System.Core"")
from Microsoft.Office.Interop import *
from Microsoft.Office.Interop.Excel import *
from System import *

def export(speakers):
  ex = Excel.ApplicationClass()   
  ex.Visible = True
  ex.DisplayAlerts = False   
  workbook = ex.Workbooks.Add()
  ws = workbook.Worksheets[1]
  rowIndex = 1
  for speaker in speakers:
    ws.Cells[rowIndex, 1].Value2 = speaker.FirstName
    ws.Cells[rowIndex, 2].Value2 = speaker.LastName
    ws.Cells[rowIndex, 3].Value2 = speaker.FullName
    ws.Cells[rowIndex, 4].Value2 = speaker.NumberOfApprovedSessions
    rowIndex = rowIndex + 1";
					engine.CreateScriptSourceFromString(scriptSource).Execute(scope);
					object exportFunc = scope.GetVariable("export");
					engine.Operations.Invoke(exportFunc, this.Speakers);
				});

			this.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == "WindowContent")
					{
							OnWindowContentChanged();
					}
				};

			this.WindowContent = WindowContent.Speakers;
		}

		private void OnWindowContentChanged()
		{
			if (this.WindowContent == WindowContent.Sessions)
			{
				if (this.Sessions != null)
				{
					this.Sessions = null;
				}

				this.Sessions = context.Sessions.ToArray();
			}
			else
			{
				if (this.Speakers != null)
				{
					this.Speakers = null;
				}

				this.Speakers = context.Speakers.Include("Sessions").ToArray().AsParallel().Select(speaker => new ExtendedObject<Speaker>(speaker)).ToArray();
				this.Speakers.AsParallel().ForAll(eo => eo.AddCalculatedProperty("FullName", "lambda s: s.LastName + \", \" + s.FirstName"));
				this.Speakers.AsParallel().ForAll(eo => eo.AddCalculatedProperty("NumberOfApprovedSessions",
					//"lambda s: reduce(lambda x, y: x + 1, [session for session in s.Sessions if session.Approved], 0)"));
					"lambda s: len([session for session in s.Sessions if session.Approved])"));
			}
		}

		private void RunScript(string scriptName)
		{
			var engine = Python.CreateEngine();
			using (var stream = new ScriptOutputStream(
				s =>
				{
					this.AppendToScriptOutput(s);
					App.Current.Dispatcher.BeginInvoke(new Action(() => this.OnPropertyChanged("ScriptOutput")));
				},
				Encoding.UTF8))
			{
				engine.Runtime.IO.SetOutput(stream, Encoding.UTF8);
				var scriptSource = engine.CreateScriptSourceFromFile(scriptName);
				try
				{
					scriptSource.Execute();
				}
				catch (SyntaxErrorException e)
				{
					this.AppendToScriptOutput("Syntax error (line {0}, column {1}): {2}", e.Line, e.Column, e.Message);
					App.Current.Dispatcher.BeginInvoke(new Action(() => this.OnPropertyChanged("ScriptOutput")));
				}
			}
		}

		private WindowContent windowContent;
		public WindowContent WindowContent
		{
			get
			{
				return this.windowContent;
			}

			set
			{
				this.windowContent = value;
				this.OnPropertyChanged("WindowContent");
			}
		}

		private IList<ExtendedObject<Speaker>> speakers;
		public IList<ExtendedObject<Speaker>> Speakers
		{
			get
			{
				return this.speakers;
			}

			set
			{
				this.speakers = value;
				this.OnPropertyChanged("Speakers");
			}
		}

		private IList<Session> sessions;
		public IList<Session> Sessions
		{
			get
			{
				return this.sessions;
			}

			set
			{
				this.sessions = value;
				this.OnPropertyChanged("Sessions");
			}
		}

		private Session selectedSession;
		public Session SelectedSession
		{
			get
			{
				return this.selectedSession;
			}

			set
			{
				this.selectedSession = value;
				this.OnPropertyChanged("SelectedSession");
			}
		}

		public GenericCommand ShowSpeakersCommand { get; private set; }
		public GenericCommand ShowSessionsCommand { get; private set; }
		public GenericCommand EraseOutputCommand { get; private set; }
		public GenericCommand RunScriptCommand { get; private set; }
		public GenericCommand ApproveSessionCommand { get; private set; }
		public GenericCommand SpeakerExportCommand { get; private set; }

		private StringBuilder scriptOutput = new StringBuilder();
		private void AppendToScriptOutput(string value)
		{
			lock (this.scriptOutput)
			{
				this.scriptOutput.Append(value);
			}
		}
		private void AppendToScriptOutput(string format, params object[] args)
		{
			lock (this.scriptOutput)
			{
				this.scriptOutput.AppendFormat(format, args);
			}
		}
		public string ScriptOutput
		{
			get
			{
				lock (this.scriptOutput)
				{
					return this.scriptOutput.ToString();
				}
			}
		}

		public void SaveChanges()
		{
			this.context.SaveChanges();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
