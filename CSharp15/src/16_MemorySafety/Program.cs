// ============================================================================
//  C# 15 memory safety (PREVIEW): `unsafe` marks the operations that touch
//  memory the runtime doesn't check, and `unsafe` on a member becomes a
//  contract the CALLER has to honor (requires-unsafe), very much like Rust.
//
//  Running example: a Base45 decoder over Span<T> (the encoding in EU Digital
//  COVID Certificate QR codes), see Base45.cs:
//    DecodeChecked   plain span code, bounds-checked
//    DecodeUnchecked Unsafe.Add, no bounds checks, `unsafe` = requires-unsafe
//    Decode          the safe boundary: checks the contract, then calls it
// ============================================================================
using System.Runtime.InteropServices;
using MemorySafety;

// ---------------------------------------------------------------------------
// (A) The safe API. Callers don't need `unsafe`, whatever happens inside.
// ---------------------------------------------------------------------------
Console.WriteLine("== (A) Decode RFC 9285 test vectors ==");
foreach (var encoded in (string[])["BB8", "%69 VD92EX0", "UJCLQE7W581", "QED8WEX0", "BB8K1"])
{
    byte[] bytes = Base45.Decode(encoded);
    Console.WriteLine($"{encoded,-13} -> \"{System.Text.Encoding.ASCII.GetString(bytes)}\"");
}

// ---------------------------------------------------------------------------
// (B) A CALLER bug: the destination is sized without the trailing single byte.
//     To keep the demo deterministic, "our" buffer and a neighbour's buffer
//     are two slices of one array.
// ---------------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("== (B) The caller gets the length wrong ==");
const string Input = "BB8K1";                       // 3 + 2 characters -> 3 bytes "ABA"
int wrongLength = Input.Length / 3 * 2;             // 2: forgot the trailing byte
byte[] block = new byte[6];
Span<byte> dest = block.AsSpan(0, wrongLength);
Span<byte> neighbour = block.AsSpan(wrongLength);   // someone else's data

neighbour.Fill(0xEE);
try { Base45.DecodeChecked(Input, dest); }
catch (IndexOutOfRangeException) { Console.WriteLine("DecodeChecked   : IndexOutOfRangeException"); }
Console.WriteLine($"  neighbour     : {Convert.ToHexString(neighbour)}");

neighbour.Fill(0xEE);
try { Base45.Decode(Input, dest); }
catch (ArgumentException ex) { Console.WriteLine($"Decode          : ArgumentException ({ex.Message})"); }
Console.WriteLine($"  neighbour     : {Convert.ToHexString(neighbour)}");

neighbour.Fill(0xEE);
int written;
unsafe { written = Base45.DecodeUnchecked(Input, dest); }   // the caller broke the promise
Console.WriteLine($"DecodeUnchecked : no error, wrote {written} bytes into a {dest.Length}-byte span");
Console.WriteLine($"  neighbour     : {Convert.ToHexString(neighbour)}   <- silently overwritten");

// What the compiler enforces with updated-memory-safety-rules:
//   Base45.DecodeUnchecked(Input, dest);
//     -> error CS9362: 'Base45.DecodeUnchecked(ReadOnlySpan<char>, Span<byte>)' must be used
//        in an unsafe context because it is marked as 'unsafe'
//   ref byte r = ref MemoryMarshal.GetReference(dest);
//     -> error CS9362: the .NET 11 BCL marks MemoryMarshal.GetReference, Unsafe.Add,
//        MemoryMarshal.CreateSpan, ... as requires-unsafe, too
// So every line that can corrupt memory is inside an `unsafe` block or expression,
// and `grep unsafe` finds the complete audit list.

// ---------------------------------------------------------------------------
// (C) Pointers: having one is no longer unsafe, dereferencing one is.
// ---------------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("== (C) Pointer relaxations ==");
Span<byte> hello = Base45.Decode("%69 VD92EX0");    // "Hello!!"
fixed (byte* p = hello)                             // pinning + pointer: no `unsafe` needed
{
    byte* scratch = stackalloc byte[sizeof(long)];  // stackalloc to pointer, sizeof: no `unsafe`
    // Console.WriteLine((char)*p);                 // error CS9360: This operation may only be
    //                                              // used in an unsafe context
    char first = (char)unsafe(*p);                  // C# 15: `unsafe(expr)` for one expression
    unsafe { scratch[0] = p[1]; }
    Console.WriteLine($"first = '{first}', copied = '{(char)unsafe(*scratch)}'");
}

// ---------------------------------------------------------------------------
// (D) extern members must say whether calling them is safe.
// ---------------------------------------------------------------------------
Console.WriteLine();
Console.WriteLine("== (D) safe extern ==");
int pid = OperatingSystem.IsWindows() ? (int)Native.GetCurrentProcessId() : Native.getpid();
Console.WriteLine($"native pid = {pid}, Environment.ProcessId = {Environment.ProcessId}");

// The compiler records the opt-in on the module (like [RefSafetyRules] for C# 11 ref safety):
var rules = typeof(Base45).Module.GetCustomAttributesData()
    .Single(a => a.AttributeType.Name == "MemorySafetyRulesAttribute");
Console.WriteLine($"module: [{rules.AttributeType.Name.Replace("Attribute", "")}({rules.ConstructorArguments[0].Value})]");

namespace MemorySafety
{
    internal static partial class Native
    {
        // `safe`: no pointers, no buffers, nothing the caller has to promise.
        // Without `safe` or `unsafe`: error CS9389 'extern' member must be marked 'unsafe' or 'safe'.
        [LibraryImport("libc")]
        internal static safe partial int getpid();

        [LibraryImport("kernel32")]
        internal static safe partial uint GetCurrentProcessId();
    }
}
