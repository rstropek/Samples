namespace MinimalApiAot;

public static partial class Log
{
    // Note enhanced logger message attribute (source code generator).
    // Read more at https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#loggermessageattribute-constructors
    [LoggerMessage(Level = LogLevel.Debug, 
        Message = "Invalid group size {groupSize} for pricing scheme '{pricingScheme}', maximum is {maxGroupSize}.")]
    public static partial void InvalidGroupSize(
        ILogger logger, int groupSize, string pricingScheme, int maxGroupSize);
}
