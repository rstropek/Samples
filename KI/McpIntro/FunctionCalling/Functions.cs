using Logic;
using OpenAI.Chat;
using OpenAI.Responses;

namespace FunctionCalling;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

public record Password(string PasswordValue);

public static class PasswordFunctions
{
    public static readonly ResponseTool BuildPasswordTool = ResponseTool.CreateFunctionTool(
        functionName: nameof(BuildPasswordTool),
        functionDescription: "Generates an easy to remember password by concatenating a random word from the list of frequent words with a random number.",
        functionParameters: BinaryData.FromBytes("""
        {
            "type": "object",
            "properties": {
                "minimumPasswordLength": {
                    "type": "integer",
                    "description": "Minimum length of the password. Set to 0 for default length (15 characters)"
                }
            },
            "required": ["minimumPasswordLength"],
            "additionalProperties": false
        }
        """u8.ToArray()),
        strictModeEnabled: true
    );

    public static string BuildPassword(BuildPasswordParameters parameters) => PasswordGenerator.BuildPassword(parameters);
}