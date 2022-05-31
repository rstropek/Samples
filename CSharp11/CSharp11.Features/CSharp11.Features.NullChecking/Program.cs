static string BuildFullName1(Person p)
{
    // We compile with #nullable enable, so we rely on
    // p and p.FirstName to not be null.
    if (p.FirstName.Length == 0)
    {
        return p.FirstName;
    }

    return $"{p.LastName}, {p.FirstName}";
}

static string BuildFullName2(Person p)
{
    if (p is null)
    {
        throw new ArgumentNullException(nameof(p));
    }

    if (p.FirstName.Length == 0)
    {
        return p.FirstName;
    }

    return $"{p.LastName}, {p.FirstName}";
}

static string BuildFullName3(Person p!!)
{
    // We compile with #nullable enable, so we rely on
    // p and p.FirstName to not be null.
    if (p.FirstName.Length == 0)
    {
        return p.FirstName;
    }

    return $"{p.LastName}, {p.FirstName}";
}

var p = new Person("Foo", "Bar");
Console.WriteLine(BuildFullName3(p));

// Pass null and supress nullable warning (shouldn't do that,
// but sh... sometimes happen).
Console.WriteLine(BuildFullName3(null!));

record Person(string FirstName, string LastName);
