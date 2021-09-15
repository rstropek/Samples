namespace CSharpImmutables
{ 
    /// <summary>
    /// Immutable class with read-only properties
    /// </summary>
    public class ReadOnlyPropertiesWithCtor
    {
        // Note read-only properties.
        // You will get a warning if you forget to initialize them.
        // Avoid `private set` if you do not need it.

        public string FirstName { get; }
        public string LastName { get; }
        public string FullName => $"{LastName}, {FirstName}";

        // Note constructor with initialization values.
        // We can assign values to props in constructor although there are no setters. 

        public ReadOnlyPropertiesWithCtor(string firstName, string lastName)
            => (FirstName, LastName) = (firstName, lastName);
    }

    /// <summary>
    /// Similar as above, but shorter by using `record class`
    /// </summary>
    public record class ReadOnlyPropertiesRecord(string FirstName, string LastName)
    {
        public string FullName => $"{LastName}, {FirstName}";
    }

    /// <summary>
    /// Immutable class with read-only properties
    /// </summary>
    public class ReadOnlyPropertiesWithInitProps
    {
        // Note init properties.
        // You have to either make them nullable or provide a default value.
        // Otherwise you get a warning.

        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string FullName => $"{LastName ?? string.Empty}{(LastName == null || FirstName == null ? string.Empty : ", ")}{FirstName ?? string.Empty}";

        // Note missing constructor.
        // Of course there can be a constructure. But it is not required.
    }

    /// <summary>
    /// Base class for freezable objects
    /// </summary>
    public abstract class Freezable
    {
        public bool IsFrozen { get; private set; }

        public virtual void Freeze()
        {
            IsFrozen = true;
        }

        // In practice, consider implementing Clone, CloneAsFrozen.
        // For example see e.g. https://source.dot.net/#WindowsBase/System/Windows/Freezable.cs,32
    }

    public class FreezablePerson : Freezable
    {
        private string? firstName;
        public string? FirstName
        {
            get => firstName;
            set => firstName = IsFrozen ? throw new InvalidOperationException("Object is frozen") : value;
        }

        private string? lastName;
        public string? LastName
        {
            get => lastName;
            set => lastName = IsFrozen ? throw new InvalidOperationException("Object is frozen") : value;
        }

        public string FullName => $"{LastName ?? string.Empty}{(LastName == null || FirstName == null ? string.Empty : ", ")}{FirstName ?? string.Empty}";
    }
}
