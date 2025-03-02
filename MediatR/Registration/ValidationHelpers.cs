using FluentValidation;

namespace Registration;

public static class ValidationHelpers
{
    public static IRuleBuilderOptions<T, string> MustBeValidCampaignName<T>(this IRuleBuilder<T, string> ruleBuilder) => ruleBuilder.NotEmpty().WithMessage("Campaign name must be set");
    public static IRuleBuilderOptions<T, string> MustBeValidCampaignOrganizer<T>(this IRuleBuilder<T, string> ruleBuilder) => ruleBuilder.NotEmpty().WithMessage("Campaign organizer must be set");
    public static IRuleBuilderOptions<T, decimal?> MustBeValidReservedRatioForGirls<T>(this IRuleBuilder<T, decimal?> ruleBuilder) => ruleBuilder.Must(x => x is null || (x >= 0m && x <= 1m)).WithMessage($"Reserved ratio for girls must be null or between 0 and 1");
    public static IRuleBuilderOptions<T, DateOnly?> MustBeFuturePurgeDate<T>(this IRuleBuilder<T, DateOnly?> ruleBuilder) => ruleBuilder.Must(x => x is null || x > DateOnly.FromDateTime(DateTimeOffset.Now.Date)).WithMessage($"Purge date must be in the future");
    public static IRuleBuilderOptions<T, DateOnly?> PurgeDateMustBeAfterAllDates<T>(this IRuleBuilder<T, DateOnly?> ruleBuilder, Func<T, IEnumerable<DateOnly>?> getDates)
    {
        return ruleBuilder
            .Must((instance, value) =>
            {
                if (value == null) { return true; }
                var dates = getDates(instance);
                if (dates == null) { return true; }
                return !dates.Any(o => o >= value);
            })
            .WithMessage($"Purge date must be after all other dates");
    }
    public static IRuleBuilderOptions<T, T2> MustNotHaveDuplicateDates<T, T2>(this IRuleBuilder<T, T2> ruleBuilder, Func<T2, IEnumerable<DateOnly>?> getDates)
    {
        return ruleBuilder
            .Must(x => x == null || (!getDates(x)?.GroupBy(d => d).Any(g => g.Count() > 1) ?? false))
            .WithMessage("Duplicate dates found");
    }
    public static IRuleBuilderOptions<T, DateOnly> MustBeFutureDate<T>(this IRuleBuilder<T, DateOnly> ruleBuilder) => ruleBuilder.Must(x => x > DateOnly.FromDateTime(DateTimeOffset.Now.Date)).WithMessage($"Date must be in the future");
    public static IRuleBuilderOptions<T, TimeOnly?> MustBeBefore<T>(this IRuleBuilder<T, TimeOnly?> ruleBuilder, Func<T, TimeOnly?> getEndDate)
    {
        return ruleBuilder
            .Must((instance, value) =>
            {
                if (value == null) { return true; }
                var endDate = getEndDate(instance);
                if (endDate == null) { return true; }
                return value < endDate;
            })
            .WithMessage($"Date must be before end date");
    }
    public static IRuleBuilderOptions<T, string> MustBeValidDepartmentName<T>(this IRuleBuilder<T, string> ruleBuilder) => ruleBuilder.NotEmpty().WithMessage("Department name must be set");
    public static IRuleBuilderOptions<T, short> MustBeValidNumberOfSeats<T>(this IRuleBuilder<T, short> ruleBuilder) => ruleBuilder.Must(x => x > 0).WithMessage("Number of seats must be greater than 0");
    public static IRuleBuilderOptions<T, Guid> MustBeValidCampaignId<T>(this IRuleBuilder<T, Guid> ruleBuilder) => ruleBuilder.NotEmpty().WithMessage("Campaign ID must be set");
}
