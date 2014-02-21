namespace MyCustomBuildActivities
{
	using Microsoft.TeamFoundation.Build.Client;
	using System;
	using System.Activities;
	using System.IO;

	[BuildActivity(HostEnvironmentOption.All)]
	public class GenerateVersionFile : CodeActivity
	{
		public InArgument<DateTime> InputDate { get; set; }

		public InArgument<string> Configuration { get; set; }

		public InArgument<string> FilePath { get; set; }

		public InArgument<string> TemplatePath { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			var timestamp = this.InputDate.Get(context);
			var configuration = this.Configuration.Get(context);

			var version = new Version(timestamp.Year, timestamp.Month * 100 + timestamp.Day, timestamp.Hour * 100 + timestamp.Minute, timestamp.Second);

			var filePath = this.FilePath.Get(context);
			var versionFileTemplate = this.TemplatePath.Get(context);

			File.WriteAllText(filePath, string.Format(File.ReadAllText(versionFileTemplate), version.ToString(4), configuration));
		}
	}
}
