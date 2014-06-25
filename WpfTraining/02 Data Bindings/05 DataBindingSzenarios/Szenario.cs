using System;

namespace Samples
{
	class Szenario
	{
		private string description;
		private string sourceUri;

		public Szenario()
		{
		}

		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		public string SourceUri
		{
			get { return sourceUri; }
			set { sourceUri = value; }
		}
	}
}
