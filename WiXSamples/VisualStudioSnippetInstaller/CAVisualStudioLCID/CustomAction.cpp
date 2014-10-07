#include "stdafx.h"

// This sample WiX custom action gets the LCID for Visual Studio 2013 from the registry.
// It sets the property VSLCID if LCID could be found, otherwise it does not assign any value.
UINT __stdcall GetVisualStudioLCID(MSIHANDLE hInstall)
{
	HRESULT hr = S_OK;
	UINT er = ERROR_SUCCESS;

	// Initialize WiX Custom Action
	hr = ::WcaInitialize(hInstall, "GetVisualStudioLCID");
	ExitOnFailure(hr, "Failed to initialize");
	::WcaLog(LOGMSG_STANDARD, "Initialized.");

	// Get registry key under which we can find LCID of visual studio
	// (see http://msdn.microsoft.com/en-us/library/bb164659.aspx for details)
	HKEY hkVisualStudioProKey;
	hr = ::RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("SOFTWARE\\Microsoft\\DevDiv\\vs\\Servicing\\12.0\\professional"), 0, KEY_READ, &hkVisualStudioProKey);
	if (hr == ERROR_SUCCESS)
	{
		// Registry key found, iterate over all children
		const DWORD BUFFER_LENGTH = 256;
		TCHAR subkey[BUFFER_LENGTH];
		BOOL found = false;
		for (int index = 0; !found && hr != ERROR_NO_MORE_ITEMS; index++)
		{
			DWORD bufferLength = BUFFER_LENGTH;
			hr = ::RegEnumKeyEx(hkVisualStudioProKey, index, subkey, &bufferLength, NULL, NULL, NULL, NULL);
			if (hr != ERROR_NO_MORE_ITEMS)
			{
				ExitOnFailure(hr, "Failed to get subkey");

				// Try to convert to numeric LCID
				DWORD lcid = _ttol(subkey);
				if (lcid != 0)
				{
					// Conversion was successful -> assume that this s the LCID
					found = true;
					hr = ::WcaSetProperty(TEXT("VSLCID"), subkey);
					ExitOnFailure(hr, "Failed to set property");
				}
			}
		}

		// We are done, close registry key
		hr = ::RegCloseKey(hkVisualStudioProKey);
		ExitOnFailure(hr, "Failed to close registry key");
	}
	else if (hr == ERROR_FILE_NOT_FOUND)
	{
		// Registry key was not found. VS not installed?
		WcaLog(LOGMSG_STANDARD, "Registry key not found. Visual Studio 2013 not installed?");
	}
	else
	{
		ExitOnFailure(hr, "Error while accessing registry");
	}

LExit:
	er = SUCCEEDED(hr) ? ERROR_SUCCESS : ERROR_INSTALL_FAILURE;
	return WcaFinalize(er);
}

// DllMain - Initialize and cleanup WiX custom action utils.
extern "C" BOOL WINAPI DllMain(__in HINSTANCE hInst, __in ULONG ulReason, __in LPVOID)
{
	switch (ulReason)
	{
		case DLL_PROCESS_ATTACH:
			WcaGlobalInitialize(hInst);
			break;

		case DLL_PROCESS_DETACH:
			WcaGlobalFinalize();
			break;
	}

	return TRUE;
}
