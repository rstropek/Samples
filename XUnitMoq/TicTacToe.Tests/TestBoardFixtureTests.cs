using System.Text.Json.Nodes;

namespace TicTacToe.Tests
{
    public class TestBoardFixtureTests
    {
        [Fact]
        public void GetBoardContent()
        {
            var data = TestBoardFixture.GetBoardContent(JsonNode.Parse("{ \"foo\": [0, 1, 2] }")!, "foo");
            Assert.True(data.SequenceEqual(new byte[] { 0, 1, 2 }));
        }

        [Fact]
        public void GetBoardContentInvalid()
            => Assert.Throws<InvalidOperationException>(() => TestBoardFixture.GetBoardContent(JsonNode.Parse("{ \"foo\": 1 }")!, "foo"));
    }
}
