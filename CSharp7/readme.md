# C# 7 Experimental Code Samples

## Introduction

I use these simple code samples to demonstrate new C# language features in Visual Studio 2017.

**Note:** You can use *C# Interactive* to demo the C# 7 features, too. To launch C# interactive, run `csi` in a Developer Command Prompt.


## Content

* [LocalFunctionLambda](LocalFunctionLambda) demonstrates some local functions using  lambda expressions. [LocalFunction](LocalFunction) implements the same, but this  time with the new C# 7 *local functions* feature. During demos, I use `ILDasm`  to dig deeper into the implementation details. I also demonstrate the differences  regarding the *Garbage Collector* in case of many calls to a local function.

* [PatternMatching](PatternMatching) contains samples for the new *Pattern Matching*  feature of C#.

* [RefReturns](RefReturns) contains a sample demonstrating the new ref return Feature
  of C# 7. During demos, I use `ILDasm` to dig deeper into the implementation details.

* [Tuples](Tuples) contains a sample demonstrating tuples in C# 7.


