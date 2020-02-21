using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

// Read more at https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-core-3-0#fast-built-in-json-support

namespace Json
{
    class Program
    {
        static void Main()
        {
            var json = File.ReadAllText("DemoData.json");

            // Use the low-level Utf8JsonReader to read stream of JSON tokens
            DumpAllStrings(Encoding.UTF8.GetBytes(json).AsSpan());

            // Use the low-level Utf8JsonWriter to write JSON
            WriteJson();

            // Use JsonDocument to get a DOM of the JSON
            DumpJson(json);

            // Use JsonSerializer to serialize and deserialize JSON
            Console.WriteLine(JsonSerializer.Serialize(new[] { 1, 1, 2, 3, 5, 8, 13 }));
            Console.WriteLine(JsonSerializer.Deserialize<int[]>("[1, 1, 2, 3, 5, 8, 13]").Sum());
        }

        public static void DumpAllStrings(ReadOnlySpan<byte> dataUtf8)
        {
            // Note that Utf8JsonReader is a ref struct
            var json = new Utf8JsonReader(dataUtf8);
            while (json.Read())
            {
                JsonTokenType tokenType = json.TokenType;
                if (tokenType == JsonTokenType.String)
                { 
                        Console.WriteLine(json.GetString());
                }
            }
        }

        public static void WriteJson()
        {
            var destination = new MemoryStream();
            using (var json = new Utf8JsonWriter(destination))
            {
                json.WriteStartObject();
                json.WriteString("Foo", "Bar");
                json.WriteEndObject();
            }

            Console.WriteLine(Encoding.UTF8.GetString(destination.ToArray()));
        }

        public static void DumpJson(string jsonString)
        {
            // Use new JSON parser to parse string (note: IDisposable)
            using var document = JsonDocument.Parse(jsonString);
            DumpElement(document.RootElement);

            static void DumpElement(JsonElement element, string prefix = "")
            {
                // Note that a JsonElement has a type (ValueKind)
                switch (element.ValueKind)
                {
                    case JsonValueKind.Object:
                        // This is how you enumerate properties of an object
                        foreach (var prop in element.EnumerateObject())
                        {
                            // Each property has a Name and a type (ValueKind)
                            switch (prop.Value.ValueKind)
                            {
                                case JsonValueKind.Object:
                                case JsonValueKind.Array:
                                    // Print name as header and recursively print subelements
                                    Console.WriteLine($"{prefix}{prop.Name}:");
                                    DumpElement(prop.Value, prefix + '\t');
                                    break;
                                case JsonValueKind.Null:
                                    Console.WriteLine($"{prefix}{prop.Name}: Null");
                                    break;
                                default:
                                    Console.WriteLine($"{prefix}{prop.Name}: {prop.Value}");
                                    break;
                            }
                        }
                        break;
                    case JsonValueKind.Array:
                        // You can enumerate array elements with EnumerateArray or a for-loop
                        for (var i = 0; i < element.GetArrayLength(); i++)
                        {
                            var prop = element[i];
                            switch (prop.ValueKind)
                            {
                                case JsonValueKind.Object:
                                case JsonValueKind.Array:
                                    // Print index as header and recursively print subelements
                                    Console.WriteLine($"{prefix}[{i}]:");
                                    DumpElement(prop, prefix + '\t');
                                    break;
                                case JsonValueKind.Null:
                                    Console.WriteLine($"{prefix}[{i}]: Null");
                                    break;
                                default:
                                    Console.WriteLine($"{prefix}[{i}]: {prop}");
                                    break;
                            }
                        }
                        break;
                    default:
                        // Element is a simple property, no object or array
                        Console.WriteLine($"{prefix}{element}");
                        break;
                }
            }
        }
    }
}
