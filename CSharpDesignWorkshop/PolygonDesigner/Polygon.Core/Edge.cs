using System;

namespace Polygon.Core
{
    /// <summary>
    /// Represents an edge between two points
    /// </summary>
    public struct Edge : IEquatable<Edge>
    {
        public Point From { get; }

        public Point To { get; }

        public Edge(in Point from, in Point to)
        {
            From = from;
            To = to;
        }

        public Edge(in Edge source) => (From, To) = source;

        /// <inheritdoc />
        public bool Equals(Edge other) => From == other.From && To == other.To;

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals((Edge)obj);

        /// <inheritdoc />
        public override int GetHashCode() => unchecked(From.GetHashCode() * 17 + To.GetHashCode());

        public static bool operator ==(Edge left, Edge right) => left.Equals(right);

        public static bool operator !=(Edge left, Edge right) => !left.Equals(right);

        public void Deconstruct(out Point from, out Point to)
        {
            from = From;
            to = To;
        }

        /// <summary>
        /// Returns the intersection of the two lines
        /// </summary>
        /// <remarks>
        /// Edges are passed in, but they are treated like infinite lines
        /// </remarks>
        public Point? GetIntersectionPoint(in Point lineFrom, in Point lineTo)
        {
            var direction1 = lineTo - lineFrom;
            var direction2 = To - From;
            var dotPerp = (direction1.X * direction2.Y) - (direction1.Y * direction2.X);

            // If it's 0, it means the lines are parallel so have infinite intersection points
            if (dotPerp.IsNearZero())
            {
                return null;
            }

            var c = From - lineFrom;
            var t = (c.X * direction2.Y - c.Y * direction2.X) / dotPerp;

            //	Return the intersection point
            return lineFrom + (t * direction1);
        }

        /// <summary>
        /// Tells if the test point lies on the left side of the edge line
        /// </summary>
        public bool? IsLeftOf(in Point test)
        {
            var tmp1 = To - From;
            var tmp2 = test - To;

            var x = (tmp1.X * tmp2.Y) - (tmp1.Y * tmp2.X);
            if (x < 0)
            {
                return false;
            }
            else if (x > 0)
            {
                return true;
            }
            else
            {
                //	Colinear points;
                return null;
            }
        }
    }
}
