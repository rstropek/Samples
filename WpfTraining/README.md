# WPF Training

This folder contains samples that I use during WPF Trainings. Most samples are 
as small as possible to demonstrate a certain concept of WPF.

# XAML Introduction

These samples focus on the basic concepts of XAML. They do not focus on WPF, they
should help to understand the XAML declarative programming language.

## Behind the scenes

### Element Naming

I use these samples to demonstrate naming of elements (`x:Name`) in WPF XAML
files.

* `01 ElementNaming` shows element naming during compile time.

* `02 ElementNaming Runtime` shows how to use the `LogicalTreeHelper` class to bind
named elements to properties. This is useful for scenarios in which WPF XAML is 
loaded during runtime.

### Application Definition Files

This sample demonstrates how to add a custom object to an `App.xaml` file.

### WPF XAML Hello World

These are not the classical `Hello World` sample for WPF. I use them during trainings
to show what is happening when XAML files are compiled.

* `04 Hello World` shows that XAML pages do not necessarily contain WPF classes. This
sample generates a class using XAML that has nothing to do with WPF. It is used in a
console application.

* `05 Hello World Runtime` is a variant of the first sample. It shows how to load
the XAML file during runtime.

* `06 Hello World Embedded Code` is another variant of the sample. It shows how to
embed C# code in XAML. Note that this is not a recommended approach. However, it 
typically helps to understand what's going on in the background during XAML compilation.

