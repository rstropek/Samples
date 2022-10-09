using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using static System.Console;

// This is how Hello World can look like in C#
{
    var s = "System.Console.WriteLine(\n\t\"Hello World!\"\n);";
    WriteLine(s);
}

{
    var greet = "Hello World!";
    var s = $"System.Console.WriteLine(\n\t\"{greet}\"\n);";
    WriteLine(s);
}

{
    var s = @"System.Console.WriteLine(
        ""Hello World!""
);";
    WriteLine(s);
}

{
    var greet = "Hello World!";
    var s = @$"System.Console.WriteLine(
        ""{greet}""
);";
    WriteLine(s);
}

{
    var s = "using static System.Console;\nnamespace Demo\n{\n\tpublic class Program\n\t{\n\t\tpublic void Main()\n\t\t{\n\t\t\tWriteLine(\"Hello World!\");\n\t\t}\n\t}\n}";
    WriteLine(s);
}

{
    var s = @"using static System.Console;
namespace Demo
{
    public class Program
    {
        public void Main()
        {
            WriteLine(""Hello World!"");
        }
    }
}";
    WriteLine(s);
}

{
    // Note: First and last newline are ignored
    // Be careful when mixing tabs and spaces. Shouldn't to that.
    var s = """
            using static System.Console;
            namespace Demo
            {
                public class Program
                {
                    public void Main()
                    {
                        WriteLine("Hello World!");
                    }
                }
            }
            """;
    WriteLine(s);
}

{
    WriteLine(""""
              The new Raw String Literal feature is great:
              System.Console.WriteLine("""
                                       Hello!
                                       """);
              """");
}

{
    var greet = "Hello!";
    WriteLine($""""
              The new Raw String Literal feature is great:
              System.Console.WriteLine("""
                                       {greet}
                                       """);
              """");
}

{
    using var conn = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;");
    await conn.OpenAsync();
    const string query = """
        SELECT  TABLES.TABLE_NAME as Name
        FROM    INFORMATION_SCHEMA.TABLES
        ORDER BY TABLES.TABLE_NAME
        """;
    Console.WriteLine(query);
    var tabs = await conn.QueryAsync<Table>(query);
    WriteLine(JsonSerializer.Serialize(tabs, new JsonSerializerOptions { WriteIndented = true }));
}

record Table(string Name);