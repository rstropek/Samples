using System.Numerics;

public static class MathHelpers
{
    public static float DotProduct(ReadOnlySpan<float> span1, ReadOnlySpan<float> span2)
    {
        if (span1.Length != span2.Length)
        {
            throw new ArgumentException("Vectors must be of the same length");
        }

        int length = span1.Length;
        int simdLength = System.Numerics.Vector<float>.Count;
        float result = 0.0f;

        // Process chunks of simdLength
        int i;
        for (i = 0; i <= length - simdLength; i += simdLength)
        {
            Vector<float> v1 = new(span1.Slice(i, simdLength));
            Vector<float> v2 = new(span2.Slice(i, simdLength));
            result += Vector.Dot(v1, v2);
        }

        // Process remaining elements
        for (; i < length; i++) { result += span1[i] * span2[i]; }

        return result;
    }
}