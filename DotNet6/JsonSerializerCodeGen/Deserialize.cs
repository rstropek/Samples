using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JsonSerializerCodeGen
{
    [JsonSerializable(typeof(Person[]))]
    public partial class MyDeserializationContext : JsonSerializerContext
    {
    }

    [SimpleJob(RunStrategy.ColdStart, targetCount: 100)]
    public class DeserializeColdStart
    {
        [Benchmark]
        public void DeserializeJsonBlob()
        {
            var people = JsonSerializer.Deserialize<Person[]>(JsonData.JsonBlob);
        }

        [Benchmark]
        public void DeserializeJsonBlobGenerated()
        {
            var people = JsonSerializer.Deserialize(JsonData.JsonBlob, MyDeserializationContext.Default.PersonArray);
        }
    }

    [SimpleJob(targetCount: 10)]
    public class Deserialize
    {
        [Benchmark]
        public void DeserializeJsonBlob()
        {
            var people = JsonSerializer.Deserialize<Person[]>(JsonData.JsonBlob);
        }

        [Benchmark]
        public void DeserializeJsonBlobGenerated()
        {
            var people = JsonSerializer.Deserialize(JsonData.JsonBlob, MyDeserializationContext.Default.PersonArray);
        }
    }
}
