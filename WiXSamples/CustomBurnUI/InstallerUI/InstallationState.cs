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
