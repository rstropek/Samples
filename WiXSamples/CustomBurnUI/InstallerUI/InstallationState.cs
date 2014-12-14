using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallerUI
{
	public enum InstallationState
	{
		Initializing,
		DetectedAbsent,
		DetectedPresent,
		DetectedNewer,
		Applying,
		Applied,
		Failed,
	}
}
