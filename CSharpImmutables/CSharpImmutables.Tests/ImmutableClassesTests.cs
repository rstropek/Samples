namespace CSharpImmutables.Tests
{
    public class ImmutableClassesTests
    {
        [Fact]
        public void ReadOnlyPropertiesWithCtor()
        {
            var c = new ReadOnlyPropertiesWithCtor("Foo", "Bar");
            // c.LastName = "Baz"; // <<< This does not work because there is no setter
            Assert.Equal("Bar, Foo", c.FullName);
        }

        [Fact]
        public void ReadOnlyPropertiesRecord()
        {
            var c = new ReadOnlyPropertiesRecord("Foo", "Bar");
            // c.LastName = "Baz"; // <<< This does not work because record classes are immutable by default
            Assert.Equal("Bar, Foo", c.FullName);

            c = c with { LastName = "Baz" }; // <<< Does NOT change existing object, creates a copy
            Assert.Equal("Baz, Foo", c.FullName);
        }

        [Fact]
        public void ReadOnlyPropertiesWithInitProps()
        {
            var c = new ReadOnlyPropertiesWithInitProps { FirstName = "Foo", LastName = "Bar", };
            // c.LastName = "Baz"; // <<< This does not work because props are init-only
            Assert.Equal("Bar, Foo", c.FullName);

            c = new ReadOnlyPropertiesWithInitProps { LastName = "Bar", };
            Assert.Equal("Bar", c.FullName);

            c = new ReadOnlyPropertiesWithInitProps { FirstName = "Foo", };
            Assert.Equal("Foo", c.FullName);
        }

        [Fact]
        public void Freezable()
        {
            var c = new FreezablePerson { FirstName = "Foo", LastName = "Bar", };
            c.LastName = "Baz";
            c.Freeze();
            // c.LastName = "Bar"; // <<< Not possible because object is now frozen
            Assert.Equal("Baz, Foo", c.FullName);
        }
    }
}
