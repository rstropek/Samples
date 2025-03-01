using DataAccess;
using FluentValidation;
using MediatR;

namespace Registration.CreateCampaign;

public record CreateCampaign(CreateCampaignRequest Request) : IRequest<CreateCampaignResponse>;

public record CreateCampaignRequest(
       string Name,
       string Organizer,
       CreateDateRequest[]? Dates = null,
       decimal? ReservedRatioForGirls = null,
       DateOnly? PurgeDate = null);

public record CreateDateRequest(
    DateOnly Date,
    DepartmentAssignmentRequest[]? DepartmentAssignments = null,
    TimeOnly? StartTime = null,
    TimeOnly? EndTime = null
);

public record DepartmentAssignmentRequest(
    string DepartmentName,
    short NumberOfSeats,
    decimal? ReservedRatioForGirls = null
);

public record CreateCampaignResponse(
    Guid Id
    // Many APIs return more data than just the ID. This decision has to be made
    // on a case-by-case basis.
);

public class CreateCampaignValidator : AbstractValidator<CreateCampaign>
{
    public CreateCampaignValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new CreateCampaignRequestValidator());
    }
}
public class CreateCampaignRequestValidator : AbstractValidator<CreateCampaignRequest>
{
    public CreateCampaignRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Campaign name must be set");

        RuleFor(x => x.Organizer)
            .NotEmpty().WithMessage("Campaign organizer must be set");

        RuleFor(x => x.ReservedRatioForGirls)
            .InclusiveBetween(0, 1)
            .When(x => x.ReservedRatioForGirls.HasValue)
            .WithMessage("Reserved ratio for girls must be between 0 and 1");

        RuleFor(x => x.PurgeDate)
            .Must(date => date > DateOnly.FromDateTime(DateTimeOffset.Now.Date))
            .When(x => x.PurgeDate.HasValue)
            .WithMessage("Purge date for campaign must be in the future");

        RuleFor(x => x)
            .Must(x => !x.Dates!.Any(d => d.Date >= x.PurgeDate!.Value))
            .When(x => x.PurgeDate.HasValue && x.Dates != null)
            .WithMessage("Purge date for campaign must be after all dates");

        RuleFor(x => x.Dates)
            .Must(dates => !dates!.GroupBy(d => d.Date).Any(g => g.Count() > 1))
            .When(x => x.Dates != null)
            .WithMessage("Duplicate dates found");

        RuleForEach(x => x.Dates)
            .SetValidator(new CreateDateRequestValidator());
    }
}

public class CreateDateRequestValidator : AbstractValidator<CreateDateRequest>
{
    public CreateDateRequestValidator()
    {
        RuleFor(x => x.Date)
            .Must(date => date >= DateOnly.FromDateTime(DateTimeOffset.Now.Date))
            .WithMessage("Date must be in the future");

        RuleFor(x => x)
            .Must(x => x.StartTime < x.EndTime)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .WithMessage("Start time must be before end time");

        RuleForEach(x => x.DepartmentAssignments)
            .SetValidator(new DepartmentAssignmentRequestValidator());
    }
}

public class DepartmentAssignmentRequestValidator : AbstractValidator<DepartmentAssignmentRequest>
{
    public DepartmentAssignmentRequestValidator()
    {
        RuleFor(x => x.DepartmentName)
            .NotEmpty().WithMessage("Department name must be set");

        RuleFor(x => x.NumberOfSeats)
            .GreaterThan((short)0)
            .WithMessage("Number of seats must be greater than 0");

        RuleFor(x => x.ReservedRatioForGirls)
            .InclusiveBetween(0, 1)
            .When(x => x.ReservedRatioForGirls.HasValue)
            .WithMessage("Reserved ratio for girls must be between 0 and 1");
    }
}

public class CreateCampaignHandler(IJsonFileRepository repository) : IRequestHandler<CreateCampaign, CreateCampaignResponse>
{
    public async Task<CreateCampaignResponse> Handle(CreateCampaign createCampaign, CancellationToken _)
    {
        var request = createCampaign.Request;
        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Organizer = request.Organizer,
            Status = CampaignStatus.Inactive, // Campaigns are inactive by default, must be activated explicitly
            Dates = [.. request.Dates?.Select(date => new CampaignDate
            {
                Date = date.Date,
                StartTime = date.StartTime,
                EndTime = date.EndTime,
                Status = CampaignDateStatus.Active, // Dates are active by default, must be hidden explicitly
                DepartmentAssignments = [.. date.DepartmentAssignments?.Select(assignment => new DepartmentAssignment
                {
                    DepartmentName = assignment.DepartmentName,
                    NumberOfSeats = assignment.NumberOfSeats,
                    ReservedRatioForGirls = assignment.ReservedRatioForGirls
                }) ?? []],
            }) ?? []],
            ReservedRatioForGirls = request.ReservedRatioForGirls,
            PurgeDate = request.PurgeDate,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        await repository.Create(campaign.IdString, campaign);

        return new CreateCampaignResponse(campaign.Id);
    }
}
