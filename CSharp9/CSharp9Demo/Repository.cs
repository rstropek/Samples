using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBattleshipCodingContest
{
    [Flags]
    public enum Abilities
    {
        None = 0b0000_0000,
        Fly = 0b0000_0001,
        Levitate = 0b0000_0010,
        Superstrength = 0b0000_0100,
        Invisible = 0b0000_1000,
        DoesNotAge = 0b0001_0000,
        Indestructible = 0b0010_0000,
        TechnologicalGenius = 0b0100_0000,
        Others = 0b1000_0000,
        QuiteALot = Fly | Indestructible | Superstrength
    }

    public enum DangerLevel
    {
        Supervillain,
        YouBetterRun,
        Teddybear
    }

    public abstract record Person(string Nickname)
    {
        public static implicit operator KeyValuePair<string, Person>(Person p) => new(p.Nickname, p);
    }

    public record RegularHuman(string Nickname, string RealLifeName) : Person(Nickname);

    public record Hero(string Nickname, Abilities Abilities, int Coolness, int Evilness, RegularHuman? RealLifeIdentity = null)
        : Person(Nickname)
    {
        public DangerLevel Danger => (Coolness * Evilness) switch
        {
            > 8 * 8 => DangerLevel.Supervillain,
            < 10 => DangerLevel.Teddybear,
            _ => DangerLevel.YouBetterRun
        };

        public Person? Assistant { get; init; }

        public bool CanFly => Abilities.HasFlag(Abilities.Fly);
    }

    public class HeroRepository
    {
        private static readonly ConcurrentDictionary<string, Person> heroesAndFriends;

        static HeroRepository()
        {
            RegularHuman bruceWayne, alfred;
            Hero tick;
            heroesAndFriends = new(new KeyValuePair<string, Person>[]
            {
                new Hero("Homelander", Abilities.QuiteALot, 9, 9),
                bruceWayne = new RegularHuman("Bruce", "Bruce Wayne"),
                alfred = new("Alfred", "Alfred Pennyworth"),
                new Hero("Batman", Abilities.TechnologicalGenius, 7, 1, bruceWayne) { Assistant = alfred },
                tick = new Hero("Tick", Abilities.Superstrength, 1, 0),
                tick with { Nickname = "The Groot" }
            });
        }

        public void AddOrUpdate(Person p) => heroesAndFriends[p.Nickname] = p;

        public bool TryRemove(Person p) => heroesAndFriends.TryRemove(p);

        public IReadOnlyCollection<Person> GetAllHeroesAndFriends() => heroesAndFriends.Values.ToArray();

        public Person? GetPersonByName(string nickname) =>
            heroesAndFriends.TryGetValue(nickname, out var person) ? person : null;

        public IReadOnlyCollection<Hero> GetAverageHeroesWithAssistants(int? minEvilness = null) =>
            heroesAndFriends.Values
                .OfType<Hero>()
                .Where(h => h.Assistant != null)
                .Where(h => h.Coolness is > 3 and < 8 && (minEvilness == null || h.Evilness <= minEvilness))
                .ToArray();

        public IEnumerable<string> GetCoolFlyingHeroes() =>
            heroesAndFriends.Values
                .OfType<Hero>()
                .Select(h => h switch
                {
                    { CanFly: true, Coolness: >= 8, Nickname: var heroName } => $"{heroName} is cool and can fly",
                    { Nickname: var heroName } => $"{heroName} is not cool or cannot fly"
                });
    }
}
