using Moq;
using PokemonAnalyzer.Logic;
using System.Threading.Tasks;
using Xunit;

namespace PokemonAnalyzer.Tests
{
    public class AnalyzersTest
    {
        [Fact]
        public async Task TestAverageHeightUgly()
        {
            var analyzer = new UglyAnalyzer();
            var result = await analyzer.GetAverageHeightAsync(1, 2, 3);

            Assert.Equal((7d + 10d + 20d) / 3d, result);
        }

        [Fact]
        public async Task TestAverageHeight()
        {
            var pokedexMock = new Mock<IPokedex>(MockBehavior.Loose);
            pokedexMock.Setup(m => m.GetPokemonAsync(1)).ReturnsAsync(new Pokemon { Height = 2 }).Verifiable();
            pokedexMock.Setup(m => m.GetPokemonAsync(2)).ReturnsAsync(new Pokemon { Height = 4 }).Verifiable();
            pokedexMock.Setup(m => m.GetPokemonAsync(3)).ReturnsAsync(new Pokemon { Height = 6 }).Verifiable();

            var analyzer = new GoodAnalyzer(pokedexMock.Object);
            var result = await analyzer.GetAverageHeightAsync(1, 2, 3);

            Assert.Equal(4d, result);
            pokedexMock.Verify();
        }
    }
}
