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
        public void ImmutableQueue()
        {
            var queue = ImmutableQueue<int>.Empty;
            queue = queue.Enqueue(0);
            queue = queue.Enqueue(1);
            queue = queue.Enqueue(2);
            Assert.Equal(3, queue.Count());

            Assert.Equal(0, queue.Peek());

            queue = queue.Dequeue(out var first);
            Assert.Equal(0, first);
            queue = queue.Dequeue(out var _);
            Assert.Single(queue);
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
            Assert.Equal("Zwei", d4["2"]);
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

        [Fact]
        public void ImmutableInterlockedTest()
        {
            var stack = ImmutableStack<int>.Empty;
            ImmutableInterlocked.Push(ref stack, 0);
            ImmutableInterlocked.Push(ref stack, 1);
            ImmutableInterlocked.Push(ref stack, 2);
            Assert.Equal(3, stack.Count());

            var d = ImmutableDictionary<string, string>.Empty;
            ImmutableInterlocked.AddOrUpdate(ref d, "1", "One", (_, _) => "One");
            ImmutableInterlocked.AddOrUpdate(ref d, "2", "Two", (_, _) => "Two");
            ImmutableInterlocked.AddOrUpdate(ref d, "1", "Eins", (_, _) => "Eins");
            Assert.Equal("Eins", d["1"]);
            Assert.Equal("Two", d["2"]);
        }

        private record Person(string FirstName, string LastName) 
        {
            private string? fullName;
            public string FullName => fullName ??= $"{LastName}, {FirstName}";
        }

        private class PersonComparer : IComparer<Person>
        {
            public int Compare(Person? x, Person? y)
            {
                var xName = x?.FullName ?? string.Empty;
                var yName = y?.FullName ?? string.Empty;
                if (xName == yName) return 0;
                return xName.CompareTo(yName);
            }
        }

        [Fact]
        public void PersonCompare()
        {
            Assert.True(new PersonComparer().Compare(new("A", "A"), new("B", "B")) < 0);
            Assert.True(new PersonComparer().Compare(null, new("B", "B")) < 0);
            Assert.True(new PersonComparer().Compare(new("B", "B"), new("B", "B")) == 0);
        }

        [Fact]
        public void ImmutableSortedSetTest()
        {
            var s = ImmutableSortedSet<Person>.Empty.WithComparer(new PersonComparer());

            s = s.Add(new("B", "B"));
            s = s.Add(new("C", "C"));
            s = s.Add(new("A", "A"));

            Assert.Equal(0, s.IndexOf(new("A", "A")));
        }

        [Fact]
        public void ImmutableSortedDictionaryTest()
        {
            var d1 = ImmutableSortedDictionary<string, string>.Empty;
            var d2 = d1.SetItem("1", "One");
            var d3 = d2.SetItem("2", "Two");
            var d4 = d3.SetItem("2", "Zwei");
            Assert.Equal("Zwei", d4["2"]);
        }

        [Fact]
        public void ImmutableSortedDictionaryBuilder()
        {
            var builder = ImmutableSortedDictionary<string, string>.Empty.ToBuilder();
            builder.AddRange(Enumerable.Range(1, 5).Select(
                n => new KeyValuePair<string, string>(n.ToString(), "FooBar")));
            var d1 = builder.ToImmutable();

            var d2 = d1.SetItem("2", "Zwei");
        }
    }
}
