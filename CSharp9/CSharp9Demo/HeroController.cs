using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NBattleshipCodingContest;
using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace CSharp9Demo
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController : ControllerBase
    {
        private readonly HeroRepository repository;
        private readonly IMapper mapper;

        public record HeroDto(string Type, string Nickname, string? Abilities = null, 
            int? Coolness = null, int? Evilness = null, string? RealLifeIdentity = null)
        {
            [JsonPropertyName("type")]
            public string Type { get; init; } = Type;

            [JsonPropertyName("nn")]
            public string Nickname { get; init; } = Nickname;

            [JsonPropertyName("abs")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? Abilities { get; init; } = Abilities;

            [JsonPropertyName("cool")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? Coolness { get; init; } = Coolness;

            [JsonPropertyName("evil")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? Evilness { get; init; } = Evilness;

            [JsonPropertyName("rlid")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? RealLifeIdentity { get; init; } = RealLifeIdentity;
        }

        public record HeroShortDto(string Nickname, int Coolness, int Evilness);

        public HeroController(HeroRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllHeroes() =>
            Ok(repository.GetAllHeroesAndFriends()
                .Select(h => h switch
                {
                    Hero he => new HeroDto(nameof(Hero), he.Nickname, he.Abilities.ToString(), he.Coolness, he.Evilness, he.RealLifeIdentity?.Nickname),
                    RegularHuman rh => new HeroDto(nameof(RegularHuman), rh.Nickname, RealLifeIdentity: rh.RealLifeName),
                    Person p => new HeroDto(nameof(Person), p.Nickname),
                    _ => throw new InvalidOperationException("Unknown type, this should never happen!")
                }));

        [HttpGet("short")]
        public IActionResult GetAllHeroesShort() =>
            Ok(repository.GetAllHeroesAndFriends()
                .OfType<Hero>()
                .Select(h => mapper.Map<HeroShortDto>(h)));
    }
}
