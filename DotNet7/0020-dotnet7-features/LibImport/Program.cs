using System.Runtime.InteropServices;

const string stringToConvert = "42";
Console.WriteLine(Parser.Parse(stringToConvert, stringToConvert.Length));

public static partial class Parser
{
    // Note that we use the LibraryImport source generator here.
    // It generates the required marshalling code at compile time.
    // Take a look at the generated code to learn more.

    // LibraryImport is a drop-in replacement for DllImport.
    // This is how the old DllImport would have looked like:
    //[DllImport("parser.dll", CharSet = CharSet.Ansi)]
    //public extern static int Parse(string str, int length);

    // This is how the new LibraryImport looks like:
    [LibraryImport("parser.dll", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int Parse(string str, int length);
}
