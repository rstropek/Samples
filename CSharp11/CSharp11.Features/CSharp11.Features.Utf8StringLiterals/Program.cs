byte[] array = "hello"u8.ToArray(); // new byte[] { 0x68, 0x65, 0x6c, 0x6c, 0x6f }
ReadOnlySpan<byte> rospan = "cat"u8;// new byte[] { 0x63, 0x61, 0x74 }

// New u8 suffix.
var s2 = "hello"u8;                 // Okay and type is ReadOnlySpan<byte>
