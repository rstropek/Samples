using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace LineIntersection.Tests
{
	[TestClass]
	public class LineIntersectionTests
	{
		[TestMethod]
		public void Intersections()
		{
			var l1p1 = new PointF(0.0f, 0.0f);
			var l1p2 = new PointF(10.0f, 10.0f);
			var l2p1 = new PointF(0.0f, 10.0f);
			var l2p2 = new PointF(10.0F, 0.0f);
			var intersection = LineIntersection.FindIntersectionPoint(l1p1, l1p2, l2p1, l2p2);
			Assert.IsTrue(intersection.HasValue);
			Assert.AreEqual(5.0f, intersection.Value.X);
			Assert.AreEqual(5.0f, intersection.Value.Y);

			l1p1 = new PointF(0.0f, 0.0f);
			l1p2 = new PointF(10.0f, 10.0f);
			l2p1 = new PointF(0.0f, 30.0f);
			l2p2 = new PointF(30.0F, 0.0f);
			intersection = LineIntersection.FindIntersectionPoint(l1p1, l1p2, l2p1, l2p2);
			Assert.IsFalse(intersection.HasValue);

			l1p1 = new PointF(50.0f, 50.0f);
			l1p2 = new PointF(60.0f, 60.0f);
			l2p1 = new PointF(0.0f, 10.0f);
			l2p2 = new PointF(10.0F, 0.0f);
			intersection = LineIntersection.FindIntersectionPoint(l1p1, l1p2, l2p1, l2p2);
			Assert.IsFalse(intersection.HasValue);

			l1p1 = new PointF(0.0f, 0.0f);
			l1p2 = new PointF(10.0f, 10.0f);
			l2p1 = new PointF(1.0f, 0.0f);
			l2p2 = new PointF(11.0F, 10.0f);
			intersection = LineIntersection.FindIntersectionPoint(l1p1, l1p2, l2p1, l2p2);
			Assert.IsFalse(intersection.HasValue);
		}
	}
}
