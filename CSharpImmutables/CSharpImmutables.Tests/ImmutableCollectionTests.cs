namespace CSharpImmutables.Tests
{
    public class ImmutableCollectionsTests
    {
        [Fact]
        public void ImmutableStack()
        {
            var stack = ImmutableStack<int>.Empty;
            stack = stack.Push(0);
            stack = stack.Push(1);
            Assert.Equal(2, stack.Count());
            
            Assert.Equal(1, stack.Peek());

            stack = stack.Pop(out var top);
            Assert.Equal(1, top);
            Assert.Single(stack);
        }

        [Fact]
        public void ImmutableStackChanges()
        {
            var s1 = ImmutableStack<string>.Empty;
            var s2 = s1.Push("1");
            var s3 = s2.Push("2");
            var s4 = s3.Push("3");
            var s5 = s4.Push("4");
            var s6 = s4.Pop();
            var s7 = s6.Pop();
        }

        [Fact]
        public void ImmutableList()
        {
            var l1 = ImmutableList<string>.Empty;
            var l2 = l1.Add("1");
            var l3 = l2.Add("2");
            var l4 = l3.Add("3");
            var l5 = l4.Replace("2", "4");
        }

        [Fact]
        public void ImmutableListBuilder()
        {
            var builder = ImmutableList<string>.Empty.ToBuilder();
            builder.AddRange(Enumerable.Range(1, 7).Select(n => n.ToString()));
            var l1 = builder.ToImmutable();

            var l2 = l1.Replace("4", "99");
        }


        [Fact]
        public void ImmutableDictionary()
        {
            var d1 = ImmutableDictionary<string, string>.Empty;
            var d2 = d1.SetItem("1", "One");
            var d3 = d2.SetItem("2", "Two");
            var d4 = d3.SetItem("2", "Zwei");
        }

        [Fact]
        public void ImmutableDictionaryBuilder()
        {
            var builder = ImmutableDictionary<string, string>.Empty.ToBuilder();
            builder.AddRange(Enumerable.Range(1, 5).Select(
                n => new KeyValuePair<string, string>(n.ToString(), "FooBar")));
            var d1 = builder.ToImmutable();

            var d2 = d1.SetItem("2", "Zwei");
        }
    }
}
