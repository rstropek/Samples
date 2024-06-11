global using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices;

// In this sample, we want to use the new Primary Constructor feature
// of C# 12 to simplify our code. However, the class is not used at
// runtime. It is just used to define call interceptions at compile-time.
// Therefore, we get warnings as the captured ctor parameter values
// are (by design) not used (i.e. no fields are generated in the class). 
// By disabling the warning, we can use the Primary Constructor feature 
// without any warnings.
#pragma warning disable CS9113

// Note that we need the experimental interceptors feature for this sample.
// See also InterceptorsPreview feature in .csproj file.

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class InterceptsLocationAttribute(string filePath, int line, int character) : Attribute
{
}
