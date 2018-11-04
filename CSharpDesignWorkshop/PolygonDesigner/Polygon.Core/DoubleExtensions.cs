using System;

namespace Polygon.Core
{
    internal static class DoubleExtensions
    {
        public static bool IsNearZero(this double testValue)
        {
            return Math.Abs(testValue) <= .000000001d;
        }
    }
}
