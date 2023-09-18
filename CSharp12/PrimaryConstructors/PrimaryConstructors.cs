using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;

public class PrimaryConstructors
{
    // The following declaration leads to a warning as middleName is not used.
    // Attributes are not allowed on captured parameters (change to earlier preview
    // version of C# 12).
    // Note that parameters can contain default values;
    partial class Person(string firstName, string middleName, string lastName, int age = 0)
    {
        // Note that you cannot use "this.firstName" as "firstName"
        // is the parameter firstName in the constructor, not the
        // name of the field.
        public string FullName => $"{lastName}, {/*this.*/firstName}";

        // Note that a field can shadow a captured parameter
        public int age = age;

        // All other constructors must use primary constructor
        public Person() : this("John", "James", "Smith") { }
        public Person(string firstName, string lastName) : this(firstName, "", lastName) { }

        // Allow access to the backing field of the captured ctor parameter.
        public string FirstName
        {
            get => firstName;
            set => firstName = value;
        }

        // The following property assignment leads to an error because lastName
        // is stored twice (in lastName and in LastName; Double Storage warning).
        public string LastName { get; set; } = lastName;
    }

    // Primary constructors allow partial classes. However, only one partial
    // can contain a parameter list.
    partial class Person : IParsable<Person>
    {
        public static Person Parse(string s, IFormatProvider? provider)
        {
            // Parse names from string (lastname, firstname)
            var names = s.Split(',');
            return new Person(names[1].Trim(), "", names[0].Trim());
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Person result)
        {
            // Parse names from string (lastname, firstname)
            var names = s?.Split(',');
            if (names?.Length == 2)
            {
                result = new Person(names[1].Trim(), "", names[0].Trim());
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }

    [Fact]
    public void FullName()
    {
        var p = new Person("John", "James", "Smith");
        Assert.Equal("Smith, John", p.FullName);
    }

    [Fact]
    public void GetSetAge()
    {
        var p = new Person("John", "James", "Smith", 42);
        Assert.Equal(42, p.age);
        p.age = 43;
        Assert.Equal(43, p.age);
    }

    [Fact]
    public void UnusedFieldsNotGenerated()
    {
        // Person should have two fields, not three as middleName is not used
        Assert.Equal(3, typeof(Person).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Length);
    }

    [Fact]
    public void FieldNamesAreDifferent()
    {
        // No field should be named firstName
        Assert.Empty(typeof(Person).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Where(f => f.Name == "firstName"));
    }

    [Fact]
    public void ValuesAreWritable()
    {
        var p = new Person("John", "James", "Smith");

        // Change the value of the field through property
        p.FirstName = "Jane";

        Assert.Equal("Smith, Jane", p.FullName);
    }
}