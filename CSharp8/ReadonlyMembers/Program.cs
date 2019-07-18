using System;

// Learn more about Readonly Members at https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#readonly-members

namespace ReadonlyMembers
{
    struct MutableVector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        private string FormatVectorAsString()
        {
            X = Math.Round(X, 0);
            Y = Math.Round(Y, 0);
            Z = Math.Round(Z, 0);

            return $"{X}/{Y}/{Z}";
        }

        public override string ToString() => FormatVectorAsString();
    }

    struct MutableCopiedVector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        private string FormatVectorAsString()
        {
            X = Math.Round(X, 0);
            Y = Math.Round(Y, 0);
            Z = Math.Round(Z, 0);

            return $"{X}/{Y}/{Z}";
        }

        // Note that ToString is marked as readonly
        public readonly override string ToString() => FormatVectorAsString();
    }

    class Program
    {
        static void Main()
        {
            var v = new MutableVector() { X = 1.1d, Y = 2d, Z = 3d };
            var vAsString = v.ToString();
            Console.WriteLine(vAsString);
            Console.WriteLine(v.X);

            var cv = new MutableCopiedVector() { X = 1.1d, Y = 2d, Z = 3d };
            var cvAsString = cv.ToString();
            Console.WriteLine(cvAsString);
            Console.WriteLine(cv.X);
        }
    }
}
