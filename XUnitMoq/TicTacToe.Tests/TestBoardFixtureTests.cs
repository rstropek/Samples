using System.Text.Json.Nodes;

namespace TicTacToe.Tests;

public class TestBoardFixtureTests
{
    /// <summary>
    /// Ensures that <see cref="TestBoardFixture.GetBoardContent(JsonNode, string)"/> can extract board content properly.
    /// </summary>
    [Fact]
    public void GetBoardContent()
    {
        const string jsonData = /*lang=json,strict*/ "{ \"foo\": [0, 1, 2] }";
        var data = TestBoardFixture.GetBoardContent(JsonNode.Parse(jsonData)!, "foo");
        Assert.Equal(new byte[] { 0, 1, 2 }, data);
    }

    /// <summary>
    /// Ensure proper exception in case of invalid test data.
    /// </summary>
    [Fact]
    public void GetBoardContentInvalid()
    {
        const string jsonData = /*lang=json,strict*/ "{ \"foo\": 1 }";
        Assert.Throws<InvalidOperationException>(() => TestBoardFixture.GetBoardContent(JsonNode.Parse(jsonData)!, "foo"));
    }
}
