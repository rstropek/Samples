# Composite WPF App WiX Boostrapper Hands-on Lab

## Introduction

This hands-on lab can be used to learn basics about authoring 
bootstrappers with [Windows Installer XML (WiX)](http://wixtoolset.org).


## The Installers

You can use the two installers *Shell* and *Extension* as a starting point.
*Shell* is an extensible WPF application and *Extension* is an extension DLL
for it.


## The Exercise

Your job is to create a bootstrapper that can be used to install both MSIs in
a single step.

1. Create a new, empty solution

2. Add a new setup project called *Shell* and copy the sourcecode from
[Shell/Product.wxs](Shell/Product.wxs).

3. Add a new setup project called *Extension* and copy the sourcecode from
[Shell/Extension.wxs](Shell/Extension.wxs).

4. Add a new boostrapper project called *Bootstrapper*

5. Add project references from *Bootstrapper* to *Shell* and *Extension*

6. Chain both MSIs in the bootstrapper. Use *$(var.Shell.TargetDir)Shell.msi*
syntax for referencing.
   * Set property *INSTALLSDK* when calling *Shell* MSI using *MsiProperty*
element

7. Add a condition (*bal:Condition*) to ensure that bootstrapper can only
be installed on Windows >= 7.

8. Test your bootstrapper
   * Install it and check whether Shell, SDK, and Extension has been installed
   * Uninstall it and check whether everything has been removed