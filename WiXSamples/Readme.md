# Windows Installer XML Samples

This repository contains samples I use for [Windows Installer XML (WiX)](http://wixtoolset.org/)
workshops and trainings.

**If you will participate in one of my trainings and you are looking for a list
of software and tools you are going to need, please read 
[the corresponding blog article](http://www.software-architects.com/devblog/2014/10/06/Windows-Installer-XML-Training).**

Although my main business is our SaaS solution [time cockpit](http://www.timecockpit.com), I also offer trainings
and workshops on various software development topics. MSI with WiX is one of them. **If you like my samples and slides
and you want to get a jump-start by doing a workshop with me, please contact me at rainer@timecockpit.com.**


## Related blog posts

Every now and then I write blog posts with detailed information about certain
topics. Here are links to WiX-related posts (might be out of date, please
check http://www.software-architects.com for a list of all available posts):

* [Shipping large MSI installers via Azure Blob Storage](http://www.software-architects.com/devblog/2014/10/08/Shipping-large-MSI-installers-via-Azure-Blob-Storage)


## Slides

Feel free to download my [WiX slidedeck](Slides).


## Resources

* [Homepage of the WiX Toolset](http://wixtoolset.org/)
* [WiX Docs (v3.x)](http://wixtoolset.org/documentation/manual/v3/)
* [Windows Installer Docs on MSDN](http://msdn.microsoft.com/en-us/library/cc185688.aspx)
* Great Book: Ramirez, Nick: WiX 3.6: A Developer's Guide to Windows Installer XML, Packt Publishing
  ([Sponsored Amazon Link](https://www.amazon.de/dp/B009YW82A0?tag=timecockpit-21&camp=2906&creative=19474&linkCode=as4&creativeASIN=B009YW82A0&adid=1EG5FPDE5WHXHGTSAK7A&))
* [Windows Dev Center (Desktop Development)](http://msdn.microsoft.com/en-US/windows/desktop/aa904949.aspx)


## Samples


### [WiX Basics](WixBasics)

Demo installer for learning WiX basics. It covers:

* Basic structure of WiX files in [Product.wxs](WixBasics\WiXBasicsSample\Product.wxs)
  (e.g. `Product`, `Package`, `MediaTemplate`, `Directory` structure, Components, Shortcuts, Registry Values, 
  Features, references to built-in UI, etc.)
* Signing of MSI packages including generation of dev certificates in 
  [SignMSI.cmd](WixBasics\WiXBasicsSample\SignMSI.cmd)


### [.NET Project and Installer in a Single Solution](DotNetToolWithInstaller)

Another demo for learning WiX basics. In addition to [WiX Basics](WixBasics) it covers:

* Usage of project references in WiX projects (see [Product.wxs](DotNetToolWithInstaller\DotNetToolInstaller\Product.wxs)): 
  `<File Id="FILE_DotNetToolExe" Source="$(var.DotNetTool.TargetPath)" KeyPath="yes" />`
* Binder variables (see [Product.wxs](DotNetToolWithInstaller\DotNetToolInstaller\Product.wxs)): `Version="!(bind.fileVersion.FILE_DotNetToolExe)"`
* Preprocessor (see [Product.wxs](DotNetToolWithInstaller\DotNetToolInstaller\Product.wxs)): `<?if $(var.ProcessorArchitecture)=x64 ?> ... <?else ?> ... <?endif ?>`


### Registry Searches

Another demo for learning WiX basics. In addition to [WiX Basics](WixBasics) it covers registry 
searches (see [Product.wxs](RegistrySearch\RegistrySearch\Product.wxs)).


### [Composite WPF Application With Installer](CompositeWpfAppWithInstaller)

This sample contains a composite WPF application that uses MEF (*Managed
Extensibility Framework*) for loading extensions and *Sandcastle* for generating
SDK documentation (see [CompositeWpfApp.Documentation](CompositeWpfAppWithInstaller\CompositeWpfApp.Documentation)).

The sample contains two setup
projects. [CompositeWpfApp.Install](CompositeWpfAppWithInstaller/CompositeWpfApp.Install)
generates a single installer. 
[CompositeWpfApp.InstallCab](CompositeWpfAppWithInstaller/CompositeWpfApp.InstallCab)
generates an installer with multiple CAB files. See blog article [Shipping large MSI installers via Azure Blob Storage](http://www.software-architects.com/devblog/2014/10/08/Shipping-large-MSI-installers-via-Azure-Blob-Storage)
for how to deploy MSI packages with multiple CAB files via web and *Microsoft Azure*.

The sample also contains a *Burn* bootstrapper sample in 
[CompositeWpfApp.Bootstrapper](CompositeWpfAppWithInstaller/CompositeWpfApp.Bootstrapper).
It create two separate MSI files (one for the application, one for the extension) and chains
them into a single installer exe (see [Bundle.wxs](CompositeWpfAppWithInstaller\CompositeWpfApp.Bootstrapper\Bootstrapper\Bundle.wxs)). It uses the built-in UI of *Burn*.


### [Harvesting and installing COM DLLs](ComDll)

This sample demonstrate how to create a WiX setup installing a COM DLL. It also comes
with a [simple COM server](ComDll/ComDllToRegister) written in C# that can be used for experiments 
(see [SimpleComServer.cs](ComDll\ComDllToRegister\SimpleComServer.cs)).

The sample harvests the COM DLL during compilation. Check [project file](ComDll/ComInstaller/ComInstaller.wixproj)
if you want to see how this is done. The [installer](ComDll/ComInstaller/Product.wxs) itself is therefore 
quite simple.

```XML
<ItemGroup>
	<HarvestFile Include="$(ProjectDir)..\ComDllToRegister\bin\$(Configuration)\ComDllToRegister.dll">
		<ComponentGroupName>ComDll</ComponentGroupName>
		<DirectoryRefId>INSTALLFOLDER</DirectoryRefId>
		<Link>ComDllToRegister.dll</Link>
		<PreprocessorVariable>var.ComDllToRegister.TargetDir</PreprocessorVariable>
		<SuppressRootDirectory>true</SuppressRootDirectory>
	</HarvestFile>
</ItemGroup>
```


### [Installing Windows Services](WindowsService)

I have created this sample to demonstrate the creation of MSI packages for installing *Windows Services*.

The sample contains a [simple demo service written in C#](WindowsService\ServiceToInstall). This demo service
just writes into *Windows Eventlog* on a regular basis (see [EventLoggingService.cs](WindowsService\ServiceToInstall\EventLoggingService.cs)).

The installer used to install the demo service is shown in [Product.wxs](WindowsService\ServiceInstaller\Product.wxs).

```XML
<Component Id="CMP_Service" Directory="INSTALLFOLDER">
	<!-- Install service executable -->
	<File Id="FILE_EventLoggingService" 
			Source="$(var.ServiceToInstall.TargetPath)"
			KeyPath="yes" />
	<!-- Install service -->
	<ServiceInstall Id="InstallELS"
					Name="WiXEventLoggingService"
					Description="WiX EventLoggingService Sample"
					Start="auto"
					ErrorControl="normal"
					Type="ownProcess"/>
	<!-- Set start/stop/remove options -->
	<ServiceControl Id="ControllELS"
					Name="WiXEventLoggingService"
					Start="install"
					Stop="both"
					Remove="uninstall"
					Wait="yes" />
</Component>
```


### [C++ Custom Actions](VisualStudioSnippetInstaller)

This is a more advanced WiX sample. It demonstrates the development of a custom action written in C++. 
The scenario of the sample is a MSI package for installing a *Visual Studio Snippet*. The target directory
is determined by a combination of different custom actions.

A detailed description of the sample can be found at [VisualStudioSnippetInstaller](VisualStudioSnippetInstaller).


### [.NET Custom Actions and Custom MSI UIs](CustomUIWithSql)

This is a more advanced WiX sample. It demonstrates the development of a custom action written in C# to validate
a given SQL Server name (see [CustomAction.cs](CustomUIWithSql\ValidateSqlServerNameAction\CustomAction.cs)). 
If you want to play with it and debug the C# custom action, just add the following two lines to the beginning of the
custom action:

```C#
Debugger.Launch();
Debugger.Break();
```

The C# custom action is used in the installer [Product.wxs](CustomUIWithSql\Installer\Product.wxs). Note that
this installer has a custom WiX UI in [AdditionalDialog.wxs](CustomUIWithSql\Installer\AdditionalDialog.wxs). It
calls into the C# custom action.


### [Adding the IIS Server Role, installing ASP.NET web applications](WebInstaller)

This sample consists of two parts. First, [Product.wxs](WebInstaller\IisInstaller\Product.wxs) 
contains an installer that makes a *Windows Server 2012*
a web server by adding the necessary features using [dism.exe](http://support.microsoft.com/kb/2736284).
It does that by using WiX's `CAQuietExec64` feature so that the output windows of `dism.exe` is hidden.
Additionally, this sample shows how to use the WiX *WixUtilExtension* to find out if IIS is already
installed (`IISMAJORVERSION` property).

The second part of the sample demonstrates how to install an *OWIN ASP.NET* web application into IIS using WiX.
For that, the sample contains a very simple web site in [Startup.cs](WebInstaller\WebHelloWorld\Startup.cs).
The installer [Product.wxs](WebInstaller\Setup\Product.wxs) performs the following tasks:

* Install the files necessary for the OWIN web site
* Create a new website in IIS (`iis:WebSite`)
* Create a new virtual directory in the website (`iis:WebDirProperties`)
* Creates a new application pool for the application (`iis:WebAppPool`). Note that the pool is configured to
  use the *v4.0 integrated pipeline* so that OWIN will work.
* Creates a new application in the website (`iis:WebApplication`) and makes it using the new application pool


### [Creating Patches](Patch)

This sample ([Patch](Patch)) just demonstrates the basics of patch creation with WiX. 


### [Custom *Burn* Bootstrapper UI](CustomBurnUI)

This sample demonstrates how to use WPF (*Windows Presentation Foundation*) to create a custom bootrapper
UI using WiX Burn. It consists of the following parts:

* Two installers ([FirstInstaller](CustomBurnUI/FirstInstaller) and [SecondInstaller](CustomBurnUI/SecondInstaller))
  that are chained together in a bootstrapper (see [Bundle.wxs](CustomBurnUI/Bootstrapper/Bundle.wxs)).
* A WPF UI for the bootstrapper (see [InstallerUI](CustomBurnUI/InstallerUI)).

Note that:

* The WPF UI uses MEF (*Managed Extensibility Framework*) for dependency injection
* The WPF UI is built based on the MVVM (*Model View ViewModel*) design principle using the *Microsoft Prism* framework
* For demo purposes, the WPF UI handles (nearly) all Burn events and writes them into the log (including parameter
  values, see [InstallerMainWindowViewModel.cs](CustomBurnUI/InstallerUI/InstallerMainWindowViewModel.cs)). If you want to learn about custom bootstrapper UIs, you can run the sample and generate a log file
  (`Bootstrapper.exe /log log.txt`). Afterwards take a look at the log file. You will see the order and parameters
  of the Burn events. Play with different scenarios (e.g. install, uninstall, etc.) to understand how the events work.

Tip: A great source for learning how to use WPF together with Burn is the installer of the WiX toolset itself. 
Grab the sourcecode (see [http://wixtoolset.org](http://wixtoolset.org)) and step it through in the debugger 
(see tip above about how to attach a debugger to C# code running with WiX and/or Burn).
