using System.ComponentModel;
using Logic;
using ModelContextProtocol.Server;

namespace Stdio;

[McpServerToolType]
public static class PasswordBuildTool
{
    [McpServerTool, Description("Generates an easy to remember password by concatenating a random word from the list of frequent words with a random number.")]
    public static string BuildPassword(int minimumPasswordLength) => PasswordGenerator.BuildPassword(new(minimumPasswordLength));
}
