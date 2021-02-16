using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace RecordsInsideOut
{
    public class JsonSupport
    {
        private record Hero(string Name = "", string Universe = "", bool CanFly = false)
        {
            [JsonPropertyName("flying")]
            public bool CanFly { get; init; } = CanFly;
        }

        [Fact]
        public void Hero_To_And_From_Json()
        {
            var heroes = new Hero[]
            {
                new("Homelander", "DC", true),
                new("Jessica Jones", "Marvel", false)
            };
            var hJson = JsonSerializer.Serialize(heroes);
            Assert.Contains("\"flying\":", hJson);

            var heroesConverted = JsonSerializer.Deserialize<Hero[]>(hJson);
            Assert.Equal(heroesConverted, heroes);
        }
    }
}
