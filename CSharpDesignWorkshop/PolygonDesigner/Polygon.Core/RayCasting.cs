using System;

namespace Polygon.Core
{
    public class RayCasting : ContainmentChecker
    {
        private bool Intersects(in Point A, in Point B, in Point P)
        {
            if (A.Y > B.Y)
            {
                return Intersects(B, A, P);
            }

            var py = P.Y;
            if (P.Y == A.Y || P.Y == B.Y)
            {
                py += 0.0001;
            }

            if (py > B.Y || py < A.Y || P.X >= Math.Max(A.X, B.X))
            {
                return false;
            }

            if (P.X < Math.Min(A.X, B.X))
            {
                return true;
            }

            var red = (py - A.Y) / (P.X - A.X);
            var blue = (B.Y - A.Y) / (B.X - A.X);
            return red >= blue;
        }

        /// <summary>
        /// Check if a given point is inside a given shape
        /// </summary>
        /// <remarks>
        /// Based on Ray-Casting algorithm at https://rosettacode.org/wiki/Ray-casting_algorithm
        /// </remarks>
        public bool Contains(in ReadOnlySpan<Point> shape, in Point point)
        {
            var inside = false;
            for (int i = 0; i < shape.Length; i++)
            {
                if (Intersects(shape[i], shape[(i + 1) % shape.Length], point))
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}
