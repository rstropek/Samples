using System;
using System.Linq;

namespace Polygon.Core
{
    public class SutherlandHodgman : PolygonClipper
    {
        /// <summary>
        /// This clips the subject polygon against the clip polygon (gets the intersection of the two polygons)
        /// </summary>
        /// <remarks>
        /// Based on the implementation from https://rosettacode.org/wiki/Sutherland-Hodgman_polygon_clipping
        /// </remarks>
        /// <param name="subjectPoly">Can be concave or convex</param>
        /// <param name="clipPoly">Must be convex</param>
        /// <returns>The intersection of the two polygons (or null)</returns>
        public ReadOnlySpan<Point> GetIntersectedPolygon(in ReadOnlySpan<Point> subjectPoly, in ReadOnlySpan<Point> clipPoly)
        {
            if (subjectPoly.Length < 3 || clipPoly.Length < 3)
            {
                throw new ArgumentException($"The polygons passed in must have at least 3 points: subject={subjectPoly.Length}, clip={clipPoly.Length}");
            }

            var outputList = subjectPoly.ToArray().ToList();

            //	Make sure it's clockwise
            if (!IsClockwise(subjectPoly))
            {
                outputList.Reverse();
            }

            //	Walk around the clip polygon clockwise
            foreach (var clipEdge in IterateEdgesClockwise(clipPoly))
            {
                var inputList = outputList.ToList();		//	clone it
                outputList.Clear();

                if (inputList.Count == 0)
                {
                    //	Sometimes when the polygons don't intersect, this list goes to zero.  Jump out to avoid an index out of range exception
                    break;
                }

                var S = inputList[inputList.Count - 1];

                foreach (var E in inputList)
                {
                    if (IsInside(clipEdge, E))
                    {
                        if (!IsInside(clipEdge, S))
                        {
                            var point = clipEdge.GetIntersectionPoint(S, E);
                            if (point == null)
                            {
                                throw new ApplicationException("Line segments don't intersect");		//	may be colinear, or may be a bug
                            }
                            else
                            {
                                outputList.Add(point.Value);
                            }
                        }

                        outputList.Add(E);
                    }
                    else if (IsInside(clipEdge, S))
                    {
                        Point? point = clipEdge.GetIntersectionPoint(S, E);
                        if (point == null)
                        {
                            throw new ApplicationException("Line segments don't intersect");		//	may be colinear, or may be a bug
                        }
                        else
                        {
                            outputList.Add(point.Value);
                        }
                    }

                    S = E;
                }
            }

            //	Exit Function
            return outputList.ToArray();
        }

        /// <summary>
        /// This iterates through the edges of the polygon, always clockwise
        /// </summary>
        private static ReadOnlySpan<Edge> IterateEdgesClockwise(in ReadOnlySpan<Point> polygon)
        {
            Span<Edge> edges = new Edge[polygon.Length];
            if (IsClockwise(polygon))
            {
                // Already clockwise
                for (var cntr = 0; cntr < polygon.Length - 1; cntr++)
                {
                    edges[cntr] = new Edge(polygon[cntr], polygon[cntr + 1]);
                }

                edges[polygon.Length - 1] = new Edge(polygon[polygon.Length - 1], polygon[0]);
            }
            else
            {
                // Reverse
                for (var cntr = polygon.Length - 1; cntr > 0; cntr--)
                {
                    edges[cntr] = new Edge(polygon[cntr], polygon[cntr - 1]);
                }

                edges[0] = new Edge(polygon[0], polygon[polygon.Length - 1]);
            }

            return edges;
        }

        private static bool IsInside(in Edge edge, in Point test)
        {
            var isLeft = edge.IsLeftOf(test);
            if (isLeft == null)
            {
                //	Colinear points should be considered inside
                return true;
            }

            return !isLeft.Value;
        }

        private static bool IsClockwise(in ReadOnlySpan<Point> polygon)
        {
            for (var cntr = 2; cntr < polygon.Length; cntr++)
            {
                var isLeft = new Edge(polygon[0], polygon[1]).IsLeftOf(polygon[cntr]);
                if (isLeft != null)		//	some of the points may be colinear.  That's ok as long as the overall is a polygon
                {
                    return !isLeft.Value;
                }
            }

            throw new ArgumentException("All the points in the polygon are colinear");
        }
    }
}
