MyVersionAttribute<T>? GetVersion<T>(Type t)
{
    return t.GetCustomAttributes(false)
        .OfType<MyVersionAttribute<T>>()
        .FirstOrDefault();
}

Console.WriteLine($"The version is {GetVersion<int>(typeof(A))!.Version}");
Console.WriteLine($"The version is {GetVersion<string>(typeof(B))!.Version}");
Console.WriteLine($"The version is {GetVersion<int>(typeof(B))?.Version ?? -1}");

[AttributeUsage(AttributeTargets.Class)]
class MyVersionAttribute<T> : Attribute
{
    public T Version { get; }

    public MyVersionAttribute(T version)
    {
        Version = version;
    }
}

[MyVersion<int>(42)]
class A { }

[MyVersion<string>("4.2")]
class B { }
