using ICSharpCode.AvalonEdit.Document;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RoslynScripting
{
	public class ScriptExecutorControlViewModel : NotificationObject
	{
		private readonly ScriptEngine engine;
		private Session session;

		public ScriptExecutorControlViewModel()
		{
			this.engine = new ScriptEngine();
			this.CodeDocument = new TextDocument();

			this.ClearResultWindowCommand = new DelegateCommand(
				() =>
				{
					this.MessagesBuilder.Clear();
					this.RaisePropertyChanged("Messages");
				});

			this.AddReferenceCommand = new DelegateCommand(
				() =>
				{
					this.ExecuteWithExceptionRedirection(() => this.engine.AddReference(this.ReferenceAssemblyName));
					this.ReferenceAssemblyName = string.Empty;
					this.RaisePropertyChanged("References");
					this.RaisePropertyChanged("ReferenceAssemblyName");
				});
			this.AddRunningCommand = new DelegateCommand(
				() =>
				{
					this.ExecuteWithExceptionRedirection(() => this.engine.AddReference(Assembly.GetExecutingAssembly()));
					this.RaisePropertyChanged("References");
				});
			this.AddImportCommand = new DelegateCommand(
				() =>
				{
					this.ExecuteWithExceptionRedirection(() => this.engine.ImportNamespace(this.NamespaceName));
					this.NamespaceName = string.Empty;
					this.RaisePropertyChanged("Namespaces");
					this.RaisePropertyChanged("NamespaceName");
				});
			this.CreateNewSessionCommand = new DelegateCommand(() => this.session = this.engine.CreateSession(
				new ScriptApi((message) => 
				{
					this.MessagesBuilder.Append(message);
					this.MessagesBuilder.Append('\n');
					this.RaisePropertyChanged("Messages");
				})));
			this.RunCommand = new DelegateCommand(this.Run);

			this.CreateNewSessionCommand.Execute();
		}

		public DelegateCommand AddReferenceCommand { get; private set; }
		public DelegateCommand AddRunningCommand { get; private set; }
		public string ReferenceAssemblyName { get; set; }
		public IEnumerable<string> References
		{
			get
			{
				return this.engine.GetReferences().AsEnumerable().Select(r => System.IO.Path.GetFileName(r.Display));
			}
		}

		public DelegateCommand AddImportCommand { get; private set; }
		public string NamespaceName { get; set; }
		public IEnumerable<string> Namespaces
		{
			get
			{
				return this.engine.GetImportedNamespaces().AsEnumerable();
			}
		}

		private readonly StringBuilder MessagesBuilder = new StringBuilder();
		public string Messages
		{
			get
			{
				return this.MessagesBuilder.ToString();
			}
		}
		public DelegateCommand ClearResultWindowCommand { get; private set; }

		public TextDocument CodeDocument { get; private set; }
		public DelegateCommand RunCommand { get; private set; }
		public DelegateCommand CreateNewSessionCommand { get; private set; }

		private void Run()
		{
			object result = null;
			this.ExecuteWithExceptionRedirection(() => result = this.session.Execute(this.CodeDocument.Text));
			if (result != null)
			{
				this.MessagesBuilder.Append(result.ToString());
				this.MessagesBuilder.Append('\n');
				this.RaisePropertyChanged("Messages");
			}
		}

		private void ExecuteWithExceptionRedirection(Action body)
		{
			try
			{
				body();
			}
			catch (Exception ex)
			{
				this.MessagesBuilder.AppendFormat(
					"A {0} exception occurred.\n{1}\n{2}",
					ex.GetType().Name,
					ex.Message,
					ex.StackTrace);
				this.RaisePropertyChanged("Messages");
			}
		}
	}
}
