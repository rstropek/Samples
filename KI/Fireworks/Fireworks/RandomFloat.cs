using System.Numerics;

namespace AvaloniaFireworks;

static class RandomFloat
{
    public static float NextFloat(float min, float max) => (float)Random.Shared.NextDouble() * (max - min) + min;

    public static Vector2 NextVector()
    {
        var angle = NextFloat(0, MathF.PI * 2);
        return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
    }
}