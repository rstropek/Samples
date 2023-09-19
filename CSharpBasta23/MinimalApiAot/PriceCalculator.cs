namespace MinimalApiAot;

[JsonConverter(typeof(JsonStringEnumConverter<CourseTime>))]
internal enum CourseTime { OffPeek, Peak }

internal interface IPriceCalculator
{
    // New to consts in interfaces?
    // Read more at https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interface#default-interface-members
    internal const string RegularPricing = "regular";
    internal const string SchoolPricing = "school";

    decimal CalculateCoursePrice(decimal basePrice, int numberOfParticipants, CourseTime time);
}

internal class RegularPricing : IPriceCalculator
{
    public decimal CalculateCoursePrice(decimal basePrice, int numberOfParticipants, CourseTime time) =>
        (numberOfParticipants, time) switch
        {
            // Starting with the 5th participant in a group, the price per participant is reduced by 50%.
            ( > 4, _) => CalculateCoursePrice(basePrice, 4, time) + basePrice * (numberOfParticipants - 4) * 0.5m,
            // The 3rd and 4th participants in a group get a 25% discount.
            ( > 2, _) => CalculateCoursePrice(basePrice, 3, time) + basePrice * (numberOfParticipants - 3) * 0.75m,
            // The 1st and 2nd participants get 10% discount at off-peak times.
            (_, CourseTime.OffPeek) => basePrice * numberOfParticipants * 0.9m,
            // The 1st and 2nd participants pay full price at peak times.
            _ => basePrice * numberOfParticipants,
        };
}

// Note how SchoolPricing uses DI to get the regular pricing calculator via the keyed service.
internal class SchoolPricing([FromKeyedServices(IPriceCalculator.RegularPricing)] IPriceCalculator pricingCalculator) : IPriceCalculator
{
    public decimal CalculateCoursePrice(decimal basePrice, int numberOfParticipants, CourseTime time) =>
        time switch
        {
            CourseTime.Peak => pricingCalculator
                .CalculateCoursePrice(basePrice, numberOfParticipants, time),
            _ => basePrice * numberOfParticipants * 0.5m,
        };
}