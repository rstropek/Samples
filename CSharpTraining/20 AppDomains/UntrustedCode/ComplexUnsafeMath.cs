using System;
using System.IO;

public class ComplexUnsafeMath : MarshalByRefObject
{
	public double DoUnsafeCalculation(double x, double y)
	{
		var fi = new FileInfo(@"C:\temp\Blog_20150406_3.jpg");
		return x * y;
	}
}
