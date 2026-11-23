using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MemorySafety;

/// <summary>
/// Base45 (RFC 9285), the text encoding inside EU Digital COVID Certificate QR codes
/// ("HC1:" + Base45 + zlib + COSE/CBOR). Every 2 bytes become 3 characters
/// (n = c + d*45 + e*45²), a trailing single byte becomes 2 characters.
/// </summary>
public static class Base45
{
    private const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";

    // ASCII -> digit value, -1 for characters outside the alphabet.
    private static readonly sbyte[] Digits = BuildDigits();

    public static int GetDecodedLength(int encodedLength) => encodedLength % 3 == 1
        ? throw new FormatException("Base45 input can't have a length of 3n+1.")
        : encodedLength / 3 * 2 + encodedLength % 3 / 2;

    // ------------------------------------------------------------------------
    // (1) Plain span code. Every s[i] and dest[o] is bounds-checked by the
    //     runtime, so a wrong length is an exception, never a memory corruption.
    // ------------------------------------------------------------------------
    public static int DecodeChecked(ReadOnlySpan<char> s, Span<byte> dest)
    {
        int i = 0, o = 0;
        for (; i + 2 < s.Length; i += 3)
        {
            int n = Digit(s[i]) + Digit(s[i + 1]) * 45 + Digit(s[i + 2]) * 45 * 45;
            if (n > 0xFFFF) throw new FormatException($"Group at {i} is out of range.");
            dest[o++] = (byte)(n >> 8);
            dest[o++] = (byte)n;
        }
        if (s.Length - i == 2)
        {
            int n = Digit(s[i]) + Digit(s[i + 1]) * 45;
            if (n > 0xFF) throw new FormatException($"Group at {i} is out of range.");
            dest[o++] = (byte)n;
        }
        else if (s.Length - i == 1) throw new FormatException("Dangling Base45 character.");
        return o;
    }

    // ------------------------------------------------------------------------
    // (2) The same loop without bounds checks: raw refs + Unsafe.Add.
    //
    //     `unsafe` on the member = REQUIRES-UNSAFE (like `unsafe fn` in Rust):
    //     every caller needs an unsafe context and takes over the obligation:
    //       * dest.Length >= GetDecodedLength(s.Length)
    //
    //     Under the updated rules the modifier does NOT make the body an unsafe
    //     context (like Rust 2024's unsafe_op_in_unsafe_fn), so the body needs
    //     its own `unsafe { }` block for MemoryMarshal / Unsafe (both
    //     requires-unsafe in the .NET 11 BCL).
    // ------------------------------------------------------------------------
    public static unsafe int DecodeUnchecked(ReadOnlySpan<char> s, Span<byte> dest)
    {
        unsafe
        {
            ref char src = ref MemoryMarshal.GetReference(s);
            ref byte dst = ref MemoryMarshal.GetReference(dest);
            int i = 0, o = 0;
            for (; i + 2 < s.Length; i += 3)
            {
                int n = Digit(Unsafe.Add(ref src, i))
                      + Digit(Unsafe.Add(ref src, i + 1)) * 45
                      + Digit(Unsafe.Add(ref src, i + 2)) * 45 * 45;
                if (n > 0xFFFF) throw new FormatException($"Group at {i} is out of range.");
                Unsafe.Add(ref dst, o++) = (byte)(n >> 8);   // no check: trusts the caller
                Unsafe.Add(ref dst, o++) = (byte)n;
            }
            if (s.Length - i == 2)
            {
                int n = Digit(Unsafe.Add(ref src, i)) + Digit(Unsafe.Add(ref src, i + 1)) * 45;
                if (n > 0xFF) throw new FormatException($"Group at {i} is out of range.");
                Unsafe.Add(ref dst, o++) = (byte)n;
            }
            else if (s.Length - i == 1) throw new FormatException("Dangling Base45 character.");
            return o;
        }
    }

    // ------------------------------------------------------------------------
    // (3) The safe boundary. No `unsafe` modifier, so callers need nothing:
    //     this method vouches for the contract of DecodeUnchecked itself.
    //     This is the one place a reviewer has to audit.
    // ------------------------------------------------------------------------
    public static int Decode(ReadOnlySpan<char> s, Span<byte> dest)
    {
        int length = GetDecodedLength(s.Length);
        if (dest.Length < length)
            throw new ArgumentException($"Need {length} bytes, got {dest.Length}.", nameof(dest));

        // SAFETY: dest.Length >= GetDecodedLength(s.Length) was checked above.
        unsafe { return DecodeUnchecked(s, dest); }
    }

    public static byte[] Decode(ReadOnlySpan<char> s)
    {
        var result = new byte[GetDecodedLength(s.Length)];
        Decode(s, result);
        return result;
    }

    // Character validation stays checked in all variants: a bad character is
    // bad input, not a contract the caller can promise away.
    private static int Digit(char c) => c < Digits.Length && Digits[c] >= 0
        ? Digits[c]
        : throw new FormatException($"'{c}' is not a Base45 character.");

    private static sbyte[] BuildDigits()
    {
        var digits = new sbyte[128];
        digits.AsSpan().Fill(-1);
        for (int i = 0; i < Alphabet.Length; i++) digits[Alphabet[i]] = (sbyte)i;
        return digits;
    }
}
