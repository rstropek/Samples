byte[] array = "hello";             // new byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f }
Span<byte> span = "dog";            // new byte[] { 0x64, 0x6f, 0x67 }
ReadOnlySpan<byte> rospan = "cat";  // new byte[] { 0x63, 0x61, 0x74 }

// Will work with string constants, too.
const string data = "dog";
ReadOnlySpan<byte> strspan = data;  // new byte[] { 0x64, 0x6f, 0x67 }

// New u8 suffix.
var s2 = "hello"u8;                 // Okay and type is byte[]
