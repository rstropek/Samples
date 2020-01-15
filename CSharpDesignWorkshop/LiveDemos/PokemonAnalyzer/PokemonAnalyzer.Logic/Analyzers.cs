using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PokemonAnalyzer.Logic
{
    public class Pokemon
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class UglyAnalyzer
    {
        public async Task<double> GetAverageHeightAsync(params int[] pokemonIDs)
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri("https://pokeapi.co/api/v2/")
            };

            var sumHeight = 0;
            foreach(var id in pokemonIDs)
            {
                var pResponse = await client.GetAsync($"pokemon/{id}");
                var pJson = await pResponse.Content.ReadAsStringAsync();
                var p = JsonSerializer.Deserialize<Pokemon>(pJson);
                sumHeight += p.Height;
            }

            return ((double)sumHeight) / pokemonIDs.Length;
        }
    }

    public interface IPokedex
    {
        public Task<Pokemon> GetPokemonAsync(int id);
    }

    public class Pokedex : IPokedex
    {
        private readonly HttpClient client;

        public Pokedex(HttpClient client)
        {
            this.client = client;
        }

        public async Task<Pokemon> GetPokemonAsync(int id)
        {
            var pResponse = await client.GetAsync($"pokemon/{id}");
            var pJson = await pResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Pokemon>(pJson);
        }
    }

    public class GoodAnalyzer
    {
        private readonly IPokedex pokedex;

        public GoodAnalyzer(IPokedex pokedex)
        {
            this.pokedex = pokedex;
        }

        public async Task<double> GetAverageHeightAsync(params int[] pokemonIDs)
        {
            var sumHeight = 0;
            foreach (var id in pokemonIDs)
            {
                var p = await pokedex.GetPokemonAsync(id);
                sumHeight += p.Height;
            }

            return ((double)sumHeight) / pokemonIDs.Length;
        }
    }
}
