using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineIntersection
{
	public static class LineIntersection
	{
		public static PointF? FindIntersectionPoint(PointF l1p1, PointF l1p2, PointF l2p1, PointF l2p2)
		{
			var a1 = l1p2.Y - l1p1.Y;
			var b1 = l1p1.X - l1p2.X;
			var c1 = a1 * l1p1.X + b1 * l1p1.Y;

			var a2 = l2p2.Y - l2p1.Y;
			var b2 = l2p1.X - l2p2.X;
			var c2 = a2 * l2p1.X + b2 * l2p1.Y;

			var det = a1 * b2 - a2 * b1;
			if (det != 0.0)
			{
				var ip = new PointF(
					(b2 * c1 - b1 * c2) / det,
					(a1 * c2 - a2 * c1) / det);

				if (Math.Min(l1p1.X, l1p2.X) <= ip.X && ip.X <= Math.Max(l1p1.X, l1p2.X)
					&& Math.Min(l2p1.X, l2p2.X) <= ip.X && ip.X <= Math.Max(l2p1.X, l2p2.X)
					&& Math.Min(l1p1.Y, l1p2.Y) <= ip.Y && ip.Y <= Math.Max(l1p1.Y, l1p2.Y)
					&& Math.Min(l2p1.Y, l2p2.Y) <= ip.Y && ip.Y <= Math.Max(l2p1.Y, l2p2.Y))
				{
					return ip;
				}
			}

			return null;
		}
	}
}
