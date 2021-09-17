namespace CSharpImmutables.Tests
{
    public class ImmutableStructsTests
    {
        [Fact]
        public void MutableVector2d()
        {
            var v = new MutableVector2d();
            Assert.Equal(new(0d, 0d), v);

            v = new(1d, 1d);
            Assert.Equal(new(1d, 1d), v);

            v = new MutableVector2d { X = 1d, Y = 1d, };
            Assert.Equal(new(1d, 1d), v);

            v = new(1d, 1d);
            v.Double();
            Assert.Equal(new(2d, 2d), v);

            v = new(1d, 1d);
            v.Move(2d, 2d);
            Assert.Equal(new(3d, 3d), v);

            v = new(1d, 1d);
            Assert.Equal(Math.Sqrt(2d), v.Distance);
        }

        [Fact]
        public void ImmutableVector2d()
        {
            var v = new ImmutableVector2d();
            Assert.Equal(new(0d, 0d), v);

            v = new(1d, 1d);
            Assert.Equal(new(1d, 1d), v);

            v = new ImmutableVector2d { X = 1d, Y = 1d, };
            Assert.Equal(new(1d, 1d), v);

            v = new(1d, 1d);
            v = v.Double();
            Assert.Equal(new(2d, 2d), v);

            v = new(1d, 1d);
            Assert.Equal(Math.Sqrt(2d), v.Distance);
        }

        [Fact]
        public void VectorRecord2d()
        {
            var v = new VectorRecord2d();
            Assert.Equal(new(0d, 0d), v);

            v = new(1d, 1d);
            Assert.Equal(new(1d, 1d), v);

            v = new VectorRecord2d { X = 1d, Y = 1d, };
            Assert.Equal(new(1d, 1d), v);

            v = new(1d, 1d);
            v = v.Double();
            Assert.Equal(new(2d, 2d), v);

            v = new(1d, 1d);
            Assert.Equal(Math.Sqrt(2d), v.Distance);
        }

        [Fact]
        public void MutableLine2d()
        {
            // Let's create a line and move it a little bit.
            var l = new MutableLine2d(new(1d, 1d), new(2d, 2d));
            l.Move(1d, 1d);

            // Check if moving was successful
            Assert.Equal(new(2d, 2d), l.Start);
            Assert.Equal(new(3d, 3d), l.End);
        }

        [Fact]
        public void ImmutableLine2d()
        {
            // Let's create a line and move it a little bit.
            var l = new ImmutableLine2d(new(1d, 1d), new(2d, 2d));
            l.Move(1d, 1d);

            // You would expect the next two unit tests to work. They don't
            // because of defensive copying.
            // Assert.Equal(new(2d, 2d), l.Start);
            // Assert.Equal(new(3d, 3d), l.End);
            
            Assert.Equal(new(1d, 1d), l.Start);
            Assert.Equal(new(2d, 2d), l.End);
        }
    }
}
