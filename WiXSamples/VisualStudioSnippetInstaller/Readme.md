# WiX Custom Action Sample


## Introduction

This sample can be used to demonstrate the following aspects of WiX (Windows
Installer XML):

* Properties
* Directory Searches
* Custom actions written in C++
* Scheduling of custom actions in WiX
* Launch conditions


## Background

Visual Studio 2013 Xml Snippets can be found in the 
*%ProgramFiles%\Microsoft Visual Studio 12.0\Xml\1033\Snippets* folder. *1033* is
the locale ID of the installed Visual Studio copy. An installer has to dynamically
figure out which locale of Visual Studio is installed and tailor the installation
path accordingly.

[http://msdn.microsoft.com/en-us/library/bb164659.aspx](http://msdn.microsoft.com/en-us/library/bb164659.aspx)
contains a guideline how to figure out the locale of VS based on the registry.

The sample uses a custom action written in C++ to get the locale ID (LCID) from the
registry. The WiX source uses this custom action to build the appropriate installation
path.

## Hands-on Lab

1. Create a new C++ custom action project using the WiX project template.

2. Transfer the sample solution found in 
[CAVisualStudioLCID/CustomAction.cpp](CAVisualStudioLCID/CustomAction.cpp) into your 
custom action project.

3. Make sure your custom action project builds.

4. Create a WiX source that ...
   * ... references the custom action DLL as a *Binary*.
   * ... schedules the custom action before *AppSearch*.
   * ... does a directory search for Visual Studio's *Snippets* folder using the 
property set by the custom action.
   * ... has launch conditions that prevent installation if LCID or *Snippets* folder could not be found.
   * ... installs the sample snippet [SnippetInstaller/WixAddShortcut.snippet](SnippetInstaller/WixAddShortcut.snippet)
into the *Snippets* folder.

5. Test the installer.

## Sample solution

You can find the solution WiX source in [SnippetInstaller/Product.wxs](SnippetInstaller/Product.wxs).
