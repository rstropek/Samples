using System;

namespace InstallerUI
{
	public interface IUIInteractionService
	{
		void ShowMessageBox(string message);
		void CloseUIAndExit();
		void RunOnUIThread(Action body);
		IntPtr GetMainWindowHandle();
	}
}
