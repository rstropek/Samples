using NBattleshipCodingContest;
using System;
using System.Linq;
using Xunit;

namespace CSharp9Demo.Tests
{
    public class Repository
    {
        [Fact]
        public void TestDanger()
        {
            var h = new Hero("Homelander", Abilities.Fly | Abilities.Indestructible | Abilities.Superstrength, 9, 9);
            Assert.Equal(DangerLevel.Supervillain, h.Danger);
        }

        [Fact]
        public void TestGetAllHeroesAndFriends()
        {
            var hr = new HeroRepository();
            Assert.NotEmpty(hr.GetAllHeroesAndFriends());
        }

        [Fact]
        public void TestAdd()
        {
            var hr = new HeroRepository();
            hr.AddOrUpdate(new RegularHuman("Foo", "Bar"));
            Assert.NotNull(hr.GetPersonByName("Foo"));
            Assert.Null(hr.GetPersonByName("Doe"));
        }

        [Fact]
        public void TestGetByName()
        {
            var hr = new HeroRepository();
            Assert.NotNull(hr.GetPersonByName("Homelander"));
            Assert.Null(hr.GetPersonByName("Doe"));
        }

        [Fact]
        public void TestGetCoolFlyingHeroes()
        {
            var hr = new HeroRepository();
            var result = hr.GetCoolFlyingHeroes();

            Assert.Equal(3, result.Count());
            Assert.Equal(2, result.Where(r => r.Contains("not cool")).Count());
        }
    }
}
