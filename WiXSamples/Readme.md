# Windows Installer XML Samples

This repository contains samples I use for [Windows Installer XML (WiX)](http://wixtoolset.org/)
workshops and trainings.

**If you will participate in one of my trainings and you are looking for a list
of software and tools you are going to need, please read 
[the corresponding blog article](http://www.software-architects.com/devblog/2014/10/06/Windows-Installer-XML-Training).**

## Related blog posts

Every now and then I write blog posts with detailed information about certain
topics. Here are links to WiX-related posts (might be out of date, please
check http://www.software-architects.com for a list of all available posts):

* [Shipping large MSI installers via Azure Blob Storage](http://www.software-architects.com/devblog/2014/10/08/Shipping-large-MSI-installers-via-Azure-Blob-Storage)

## Slides

Feel free to download my [WiX slidedeck](Slides).

## Samples


### [WiX Basics](WixBasics)

Demo installer for learning WiX basics. It covers:

* Basic structure of WiX files in [Product.wxs](WixBasics\WiXBasicsSample\Product.wxs)
  (e.g. `Product`, `Package`, `MediaTemplate`, `Directory` structure, Components, Shortcuts, Registry Values, 
  Features, references to built-in UI, etc.)

* Signing of MSI packages including generation of dev certificates in 
  [SignMSI.cmd](WixBasics\WiXBasicsSample\SignMSI.cmd)


### .NET Project and Installer in a Single Solution

Another demo for learning WiX basics. In addition to [WiX Basics](WixBasics) it covers:

* Usage of project references in WiX projects (see [Product.wxs](DotNetToolWithInstaller\DotNetToolInstaller\Product.wxs)): 
  `<File Id="FILE_DotNetToolExe" Source="$(var.DotNetTool.TargetPath)" KeyPath="yes" />`

* Binder variables (see [Product.wxs](DotNetToolWithInstaller\DotNetToolInstaller\Product.wxs)): `Version="!(bind.fileVersion.FILE_DotNetToolExe)"`

* Preprocessor (see [Product.wxs](DotNetToolWithInstaller\DotNetToolInstaller\Product.wxs)): `<?if $(var.ProcessorArchitecture)=x64 ?> ... <?else ?> ... <?endif ?>`

* 


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
```XML


### 