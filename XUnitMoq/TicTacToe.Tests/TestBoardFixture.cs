using System.Text.Json.Nodes;

namespace TicTacToe.Tests;

public class TestBoardFixture
{
    public TestBoardFixture()
    {
        var data = JsonNode.Parse(File.ReadAllText("TestBoards.json"))
            ?? throw new InvalidOperationException("Could not deserialize JSON, should NEVER happen!");

        TestBoards = new(GetBoardContent(data, "valid"), 
            GetBoardContent(data, "invalid1"), 
            GetBoardContent(data, "invalid2"));
    }

    internal static IEnumerable<byte> GetBoardContent(JsonNode testBoardJson, string boardName) =>
        testBoardJson[boardName]!.AsArray().Select(i => (byte)(i
            ?? throw new InvalidOperationException("Inconsitent JSON, should NEVER happen!")));

    public TestBoards TestBoards { get; set; }
}

public record TestBoards(IEnumerable<byte> Valid, IEnumerable<byte> Invalid1,IEnumerable<byte> Invalid2);
