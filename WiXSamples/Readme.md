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

Demo installer for learning WiX basics.

### [Composite WPF Application With Installer](CompositeWpfAppWithInstaller)

This sample contains a composite WPF application that uses MEF (Managed
Extensibility Framework) for loading extensions and Sandcastle for generating
SDK documentation.

The sample contains two setup
projects. [CompositeWpfAppWithInstaller/CompositeWpfApp.Install](CompositeWpfAppWithInstaller/CompositeWpfApp.Install)
generates a single installer. 
[CompositeWpfAppWithInstaller/CompositeWpfApp.InstallCab](CompositeWpfAppWithInstaller/CompositeWpfApp.InstallCab)
generates an installer with multiple CAB files.

The sample also contains a BURN bootstrapper sample in 
[CompositeWpfAppWithInstaller/CompositeWpfApp.Bootstrapper](CompositeWpfAppWithInstaller/CompositeWpfApp.Bootstrapper).
It create two separate MSI files (one for the application, one for the extension) and chains
them into a single installer exe.



### [Harvesting and installing COM DLLs](ComDll)

This sample demonstrate how to create a WiX setup installing a COM DLL. It also comes
with a [simple COM server](ComDll/ComDllToRegister) written in C# that can be used for experiments.

The sample harvests the COM DLL during compilation. Check [project file](ComDll/ComInstaller/ComInstaller.wixproj)
if you want to see how this is done. The [installer](ComDll/ComInstaller/Product.wxs) itself is therefore 
quite simple.


