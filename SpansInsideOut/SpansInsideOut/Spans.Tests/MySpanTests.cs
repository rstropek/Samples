using System;
using Xunit;

namespace Spans.Tests
{
    public class MySpanTests
    {
        [Fact]
        public void TestCreationFromArray()
        {
            var array = new[] { 1 };
            var mySpan = new MySpan<int>(array);
            var span = new Span<int>(array);

            Assert.Equal(span.Length, mySpan.Length);
            Assert.Equal(span[0], mySpan[0]);
            Assert.Throws<IndexOutOfRangeException>(() => new MySpan<int>(array)[1]);
        }

        [Fact]
        public void TestCreationFromConstrainedArray()
        {
            var array = new[] { 1, 2, 3 };
            var mySpan = new MySpan<int>(array, 1, 2);
            var span = new Span<int>(array, 1, 2);

            Assert.Equal(span.Length, mySpan.Length);
            Assert.Equal(span[0], mySpan[0]);
            Assert.Equal(span[1], mySpan[1]);
            Assert.Throws<IndexOutOfRangeException>(() => new MySpan<int>(array)[2]);
        }

        [Fact]
        public void TestCastArray()
        {
            MySpan<int> mySpan = new[] { 1 };
            Span<int> span = new[] { 1 };

            Assert.Equal(span.Length, mySpan.Length);
        }

        [Fact]
        public void TestToArray()
        {
            MySpan<int> mySpan = new[] { 1 };
            Span<int> span = new[] { 1 };

            Assert.Equal(span.ToArray(), mySpan.ToArray());
        }

        [Fact]
        public void TestWriteToIndex()
        {
            MySpan<int> mySpan = new[] { 1, 2 };
            Span<int> span = new[] { 1, 2 };
            (mySpan[^2], mySpan[1]) = (mySpan[^1], mySpan[0]);
            (span[^2], span[1]) = (span[^1], span[0]);

            Assert.Equal(span[0], mySpan[0]);
            Assert.Equal(span[1], mySpan[1]);
        }

        [Fact]
        public void TestRange()
        {
            MySpan<int> mySpan = new[] { 1, 2, 3 };
            var mySubSpan = mySpan[1..^1];
            Span<int> span = new[] { 1, 2, 3 };
            var subSpan = span[1..^1];

            Assert.Equal(subSpan.ToArray(), mySubSpan.ToArray());

            mySpan = new[] { 1, 2, 3 };
            mySubSpan = mySpan[1..];
            span = new[] { 1, 2, 3 };
            subSpan = span[1..];

            Assert.Equal(subSpan.ToArray(), mySubSpan.ToArray());
        }
    }
}
