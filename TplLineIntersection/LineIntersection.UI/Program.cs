using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace LineIntersection.UI
{
	class Program
	{
		static void Main(string[] args)
		{
			const float imageResX = 1024f;
			const float imageResY = 768f;
			const int numberOfLines = 100;

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var rand = new Random();
			var linesPoint1 = new PointF[numberOfLines];
			var linesPoint2 = new PointF[numberOfLines];
			for (var i = 0; i < numberOfLines; i++)
			{
				linesPoint1[i] = new PointF(
					(float)(rand.NextDouble() * imageResX),
					(float)(rand.NextDouble() * imageResY));
				linesPoint2[i] = new PointF(
					(float)(rand.NextDouble() * imageResX),
					(float)(rand.NextDouble() * imageResY));
			}

			stopwatch.Stop();
			Console.WriteLine("Generated lines ({0}", stopwatch.Elapsed);

			stopwatch.Reset();
			stopwatch.Start();

			var index = 0;
			var intersecions = new PointF?[(numberOfLines - 1) * (numberOfLines) / 2];
			for (var i = 0; i < numberOfLines; i++)
			{
				for (var j = i + 1; j < numberOfLines; j++)
				{
					intersecions[index++] = LineIntersection.FindIntersectionPoint(
						linesPoint1[i], linesPoint2[i],
						linesPoint1[j], linesPoint2[j]);
				}
			}

			stopwatch.Stop();
			Console.WriteLine("Calculated intersections ({0}", stopwatch.Elapsed);

			stopwatch.Reset();
			stopwatch.Start();

			using (var bitmap = new Bitmap((int)imageResX, (int)imageResY))
			{
				using (var graphic = Graphics.FromImage(bitmap))
				{
					using (var linePen = new Pen(Brushes.Black, 1))
					{
						for (var i = 0; i < linesPoint1.Length; i++)
						{
							graphic.DrawLine(linePen, linesPoint1[i], linesPoint2[i]);
						}
					}

					using (var intersectPen = new Pen(Brushes.Red, 1))
					{
						foreach (var intersect in intersecions.Where(i => i.HasValue))
						{
							graphic.DrawRectangle(intersectPen,
								intersect.Value.X - 1f, intersect.Value.Y - 1f,
								2f, 2f);
						}
					}
				}

				stopwatch.Stop();
				Console.WriteLine("Generated Bitmap ({0}", stopwatch.Elapsed);

				stopwatch.Reset();
				stopwatch.Start();

				bitmap.Save("c:\\temp\\intersect.png", ImageFormat.Png);

				stopwatch.Stop();
				Console.WriteLine("Saved Bitmap ({0}", stopwatch.Elapsed);
			}

			Process.Start("c:\\temp\\intersect.png");
		}
	}
}
