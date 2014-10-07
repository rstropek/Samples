# Composite WPF App WiX Hands-on Lab

## Introduction

This hands-on lab can be used to learn basics about authoring 
installers with [Windows Installer XML (WiX)](http://wixtoolset.org).


## The Solution

The solution we start with contains four projects:

* *CompositeWpfApp.Shell*:

  WPF application that is extensible by copying modules (=DLLs)
into the application's *Modules* directory. It uses the
*Managed Extensibility Framework* to load modules. The application has
one built-in module.

* *CompositeWpfApp.Contract*:

  SDK for the extensible WPF application. Module authors will need
this assembly because it provides a base type for modules. Note
that this project also creates a *NuGet* package for the SDK.

* *CompositeWpfApp.Documentation*:

  Sandcastle project that generates documentation for
*CompositeWpfApp.Contract*.

* *CompositeWpfApp.Extension*:

  Extension module based on *CompositeWpfApp.Contract*. It can be
made available to the Shell by copying it into the *Modules*
directory of the shell.


## Exercise 1 (5-10 minutes): Make youself familiar with the solution

1. Open the sample application *CompositeWpfAppWithInstaller* consisting
of the projects mentioned above.

2. Rebuild the solution.

3. Run the shell with and without the extension DLL.


## Exercise 2 (15-20 minutes): Create a basic installer

1. Add a WiX project *CompositeWpfApp.Install* to the solution.

2. Set attributes of the *Product* element

3. Set attributes of the *Package* element

4. Setup basic directory and file structure

   Install the shell and the contract DLL into the 
*%ProgramFiles(x86)%\Composite WPF App* directory. Install the extension DLL
into *%ProgramFiles(x86)%\Composite WPF App\Modules*.

5. Create a single feature that is installed by default

6. Test your installer (install, uninstall, and repair):
   * Interactively (Add/Remove Programs)
   * MSIExec (*/i* for *Install*, */x* for *Uninstall*, logging)

7. Open your installer with *Orca* and compare your WiX setup with
the MSI database.


## Exercise 3 (10 minutes): Optional feature

1. Create a separate, optional feature *Extension* for the extension. It
should not be installed if the user doesn't explicitly ask for it.

2. Move the extension DLL to the new, optional feature

3. Don't forget to make sure that the *Modules* folder is created even
if the user decides to not install the *Extension* feature

4. Test your installer:
   * Default install (*/i* for *Install*); check that extension is not installed
   * Add optional feature (*/i ... ADDLOCAL=Extension*)
   * Remove optional feature (*/i ... REMOVE=Extension*)


## Exercise 4 (10-15 minutes): SDK feature

1. Create a new, optional feature *SDK* that is installed by default

2. Extend directory and file structure:

   Install the NuGet package and the CHM help file into the 
*%ProgramFiles(x86)%\lib* directory.

3. Copy the extension DLL from the *%ProgramFiles(x86)%\Composite WPF App* 
directory (see exercise 2) into the *%ProgramFiles(x86)%\lib*. Don't add
a new file to the installer for that. Use the *CopyFile* element instead.

4. Test your installer:
   * Default install (*/i* for *Install*); check that SDK is installed
   * Remove optional feature (*/i ... REMOVE=SDK*)
   * Remove entire application. After that, try to install application without SDK
(*/i ... ADD=Shell REMOVE=SDK*)


## Excercise 5 (15-20 minutes): Shortcuts

1. Add a shortcut to the shell executable (see exercise 2) to the start menu

2. Add a shortcut to the SDK help (see exercise 4). Note that it should only
be installed if the SDK feature is installed

3. Test your installer:
   * Install entire product and check if shortcuts work
   * Check if shortcuts are uninstalled correctly

4. Add an *Uninstall* shortcut (*msiexec /x [ProductCode]*) to the start menu

5. Check if *Uninstall* shortcut correctly uninstalles the program


## Exercise 6 (15 minutes): External CAB files

1. Copy your installer project into a new project called *CompositeWpfApp.InstallCab*

2. Delete existing *MediaTemplate ...* tag

3. Add two *Media* tags for two different, external CAB files

4. Assign all files (*.exe* and *.dll*) for the shell-feature to CAB 1 and all other
files (extension dll, SDK) to CAB 2

5. Build your new installer project and test it

6. Copy MSI and two CAB files to a web server (if you do not have a web server available,
you could use [Microsoft Azure](http://azure.microsoft.com))

7. Install the product with http-Source (*msiexec /i http://yourserver/.../yourinstaller.msi*)

8. Try to install a second time without extension and SDK features. Use a web debugger like
[Fiddler](http://www.telerik.com/fiddler) to verify that only CAB 1 is downloaded from the web.


## Solution

1. Solution with single MSI file (exercises 3-5) 
see [CompositeWpfApp.Install/Product.wxs](CompositeWpfApp.Install/Product.wxs)

2. Solution with external CAB files (exercises 6) 
see [CompositeWpfApp.InstallCab/Product.wxs](CompositeWpfApp.InstallCab/Product.wxs).
