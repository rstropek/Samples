var v = new Vector3DNew(1, 2, 3);
v += new Vector3DNew(4, 5, 6);
Console.WriteLine(v);

var vTraditional = new Vector3D(1, 2, 3);
vTraditional += new Vector3D(4, 5, 6);
Console.WriteLine(vTraditional);

class Vector3D
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Vector3D(double x, double y, double z)
    {
        Console.WriteLine("Allocating a vector (traditional)");
        X = x;
        Y = y;
        Z = z;
    }

    public static Vector3D operator +(Vector3D left, Vector3D right)
    {
        return new Vector3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public override string ToString() => $"({X}, {Y}, {Z})";
}

class Vector3DNew
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public Vector3DNew(double x, double y, double z)
    {
        Console.WriteLine("Allocating a vector");
        X = x;
        Y = y;
        Z = z;
    }

    public void operator +=(Vector3DNew v)
    {
        X += v.X;
        Y += v.Y;
        Z += v.Z;
    }

    public override string ToString() => $"({X}, {Y}, {Z})";
}
