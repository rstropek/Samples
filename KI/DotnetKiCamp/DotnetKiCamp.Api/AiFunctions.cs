using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.AI.OpenAI;
using Microsoft.Data.SqlClient;
using Dapper;

namespace DotnetKiCamp;

/// <summary>
/// Abstraction for <see cref="AiFunctions"/> to make it easier to test.
/// </summary>
public interface IAiFunctions
{
    void EnsureFunctionsAreInCompletionsOptions(ChatCompletionsOptions options);
    Task<ChatRequestToolMessage> Execute(FunctionCall call);
}

/// <summary>
/// Implements function tools for our example.
/// </summary>
public class AiFunctions(SqlConnection sqlConnection) : IAiFunctions
{
    private const string GetCustomersName = "getCustomers";

    // Unfortunately, the OpenAI library is not NativeAOT compatible. If you
    // absolutely need NativeAOT, use the OpenAI REST API directly (or wait
    // until Microsoft adds support for NativeAOT to the OpenAI library).
    private readonly ChatCompletionsFunctionToolDefinition GetCustomersTool = new(
        new FunctionDefinition()
        {
            Name = GetCustomersName,
            Description = """
                Gets a filtered list of customers. At least one filter MUST be provided in 
                the parameters. The result list is limited to 25 customer.
                """,
            // Parameters must be in JSON Schema format. You could create the JSON using
            // System.Text.Json, but specifying the JSON as a string is also an option
            // (and sometimes easier). It would be nice to derive the schema from code
            // (e.g. using a source generator), but that is not supported yet.
            Parameters = BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "customerID": { "type": "integer", "description": "Optional filter for the customer ID." },
                        "firstName": { "type": "string", "description": "Optional filter for the first name (true if first name contains filter value)." },
                        "middleName": { "type": "string", "description": "Optional filter for the middle name (true if middle name contains filter value)." },
                        "lastName": { "type": "string", "description": "Optional filter for the last name (true if last name contains filter value)." },
                        "companyName": { "type": "string", "description": "Optional filter for the company name (true if company name contains filter value)." }
                    },
                    "required": []
                }
                """)
        });

    private async Task<IEnumerable<GetCustomersResult>> GetCustomers(GetCustomersArgument args)
    {
        var sqlParameters = new Dictionary<string, object>();
        var sqlBulider = new StringBuilder("SELECT TOP 25 CustomerID, FirstName, MiddleName, LastName, CompanyName FROM SalesLT.Customer WHERE CustomerID >= 29485");

        if (args.CustomerID != null)
        {
            sqlBulider.Append($" AND CustomerID = @CustomerID");
            sqlParameters.Add("@CustomerID", args.CustomerID);
        }

        if (!string.IsNullOrEmpty(args.FirstName))
        {
            sqlBulider.Append($" AND FirstName LIKE @FirstName");
            sqlParameters.Add("@FirstName", $"%{args.FirstName}%");
        }

        if (!string.IsNullOrEmpty(args.MiddleName))
        {
            sqlBulider.Append($" AND MiddleName LIKE @MiddleName");
            sqlParameters.Add("@MiddleName", $"%{args.MiddleName}%");
        }

        if (!string.IsNullOrEmpty(args.LastName))
        {
            sqlBulider.Append($" AND LastName LIKE @LastName");
            sqlParameters.Add("@LastName", $"%{args.LastName}%");
        }

        if (!string.IsNullOrEmpty(args.CompanyName))
        {
            sqlBulider.Append($" AND CompanyName LIKE @CompanyName");
            sqlParameters.Add("@CompanyName", $"%{args.CompanyName}%");
        }

        await sqlConnection.OpenAsync();
        var customers = await sqlConnection.QueryAsync<GetCustomersResult>(sqlBulider.ToString(), sqlParameters);

        return customers;
    }

    /// <summary>
    /// Ensures that the function tools are present in the given options.
    /// </summary>
    /// <param name="options">Options to which the function tools should be added</param>
    public void EnsureFunctionsAreInCompletionsOptions(ChatCompletionsOptions options)
    {
        if (options.Tools.Count == 0)
        {
            options.Tools.Add(GetCustomersTool);
        }
    }

    /// <summary>
    /// Execute a given function call
    /// </summary>
    /// <param name="call"></param>
    /// <returns>
    /// Result of the function call
    /// </returns>
    public async Task<ChatRequestToolMessage> Execute(FunctionCall call)
    {
        // Note that we do no throw an exception here if something goes wrong. Instead, we
        // let ChatGPT know, that something is wrong so that it can fix its own mistake.

        string resultJson;
        switch (call.Name)
        {
            case GetCustomersName:
                {
                    var args = JsonSerializer.Deserialize(call.ArgumentJson.ToString(), AiFunctionsSerializerContext.Default.GetCustomersArgument);
                    if (args == null || (string.IsNullOrEmpty(args.FirstName) && string.IsNullOrEmpty(args.MiddleName) && string.IsNullOrEmpty(args.LastName) && string.IsNullOrEmpty(args.CompanyName) && args.CustomerID == null))
                    {
                        resultJson = JsonSerializer.Serialize(new ErrorResponse("Invalid arguments. At least one filter must be provided"), AiFunctionsSerializerContext.Default.ErrorResponse);
                        break;
                    }

                    var customers = await GetCustomers(args);
                    resultJson = JsonSerializer.Serialize(customers, AiFunctionsSerializerContext.Default.IEnumerableGetCustomersResult);
                    break;
                }
            default:
                {
                    resultJson = JsonSerializer.Serialize(new ErrorResponse("Unknown function name"), AiFunctionsSerializerContext.Default.ErrorResponse);
                    break;
                }
        }

        return new ChatRequestToolMessage(resultJson!, call.Id);
    }

    internal record ErrorResponse(string Error);

    internal record GetCustomersArgument(int? CustomerID, string? FirstName, string? MiddleName, string? LastName, string? CompanyName);

    internal record GetCustomersResult(int CustomerID, string FirstName, string MiddleName, string LastName, string CompanyName);
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(AiFunctions.GetCustomersArgument))]
[JsonSerializable(typeof(IEnumerable<AiFunctions.GetCustomersResult>))]
[JsonSerializable(typeof(AiFunctions.ErrorResponse))]
internal partial class AiFunctionsSerializerContext : JsonSerializerContext { }
