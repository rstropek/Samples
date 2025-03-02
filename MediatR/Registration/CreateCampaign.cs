using DataAccess;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Registration;

public record CreateCampaign(CreateCampaignRequest Request) : IRequest<Result<CreateCampaignResponse>>;

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
        RuleFor(x => x.Request).SetValidator(new CreateCampaignRequestValidator());
    }
}
public class CreateCampaignRequestValidator : AbstractValidator<CreateCampaignRequest>
{
    public CreateCampaignRequestValidator()
    {
        RuleFor(x => x.Name).MustBeValidCampaignName();
        RuleFor(x => x.Organizer).MustBeValidCampaignOrganizer();
        RuleFor(x => x.ReservedRatioForGirls).MustBeValidReservedRatioForGirls();
        RuleFor(x => x.PurgeDate).MustBeFuturePurgeDate();
        RuleFor(x => x.PurgeDate).PurgeDateMustBeAfterAllDates(x => x.Dates?.Select(d => d.Date));
        RuleFor(x => x.Dates).MustNotHaveDuplicateDates(x => x?.Select(d => d.Date));
        RuleForEach(x => x.Dates).SetValidator(new CreateDateRequestValidator());
    }
}

public class CreateDateRequestValidator : AbstractValidator<CreateDateRequest>
{
    public CreateDateRequestValidator()
    {
        RuleFor(x => x.Date).MustBeFutureDate();
        RuleFor(x => x.StartTime).MustBeBefore(x => x.EndTime);
        RuleForEach(x => x.DepartmentAssignments).SetValidator(new DepartmentAssignmentRequestValidator());
    }
}

public class DepartmentAssignmentRequestValidator : AbstractValidator<DepartmentAssignmentRequest>
{
    public DepartmentAssignmentRequestValidator()
    {
        RuleFor(x => x.DepartmentName).MustBeValidDepartmentName();
        RuleFor(x => x.NumberOfSeats).MustBeValidNumberOfSeats();
        RuleFor(x => x.ReservedRatioForGirls).MustBeValidReservedRatioForGirls();
    }
}

public class CreateCampaignHandler(IJsonFileRepository repository) : IRequestHandler<CreateCampaign, Result<CreateCampaignResponse>>
{
    public async Task<Result<CreateCampaignResponse>> Handle(CreateCampaign createCampaign, CancellationToken _)
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

        return Result.Ok(new CreateCampaignResponse(campaign.Id));
    }
}
