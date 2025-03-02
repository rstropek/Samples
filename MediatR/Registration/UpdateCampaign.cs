using DataAccess;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Registration;

public record UpdateCampaign(Guid CampaignId, UpdateCampaignRequest Request) : IRequest<Result<UpdateCampaignResponse>>;

public record UpdateCampaignRequest(
    string Name,
    string Organizer,
    UpdateDateRequest[]? Dates = null,
    decimal? ReservedRatioForGirls = null,
    DateOnly? PurgeDate = null,
    CampaignStatus Status = CampaignStatus.Inactive,
    DateTimeOffset UpdatedAt = default);

public record UpdateDateRequest(
    DateOnly Date,
    UpdateDepartmentAssignmentRequest[]? DepartmentAssignments = null,
    TimeOnly? StartTime = null,
    TimeOnly? EndTime = null,
    CampaignDateStatus Status = CampaignDateStatus.Hidden
);

public record UpdateDepartmentAssignmentRequest(
    string DepartmentName,
    short NumberOfSeats,
    decimal? ReservedRatioForGirls = null
);

public record UpdateCampaignResponse(
    Campaign Campaign
);

public class UpdateCampaignValidator : AbstractValidator<UpdateCampaign>
{
    public UpdateCampaignValidator()
    {
        RuleFor(x => x.CampaignId).MustBeValidCampaignId();
        RuleFor(x => x.Request).SetValidator(new UpdateCampaignRequestValidator());
    }
}

public class UpdateCampaignRequestValidator : AbstractValidator<UpdateCampaignRequest>
{
    public UpdateCampaignRequestValidator()
    {
        RuleFor(x => x.Name).MustBeValidCampaignName();
        RuleFor(x => x.Organizer).MustBeValidCampaignOrganizer();
        RuleFor(x => x.ReservedRatioForGirls).MustBeValidReservedRatioForGirls();
        RuleFor(x => x.PurgeDate).PurgeDateMustBeAfterAllDates(x => x.Dates?.Select(d => d.Date));
        RuleFor(x => x.Dates).MustNotHaveDuplicateDates(x => x?.Select(d => d.Date));
        RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid campaign status");
        RuleForEach(x => x.Dates).SetValidator(new UpdateDateRequestValidator());
    }
}

public class UpdateDateRequestValidator : AbstractValidator<UpdateDateRequest>
{
    public UpdateDateRequestValidator()
    {
        RuleFor(x => x.StartTime).MustBeBefore(x => x.EndTime);
        RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid campaign date status");
        RuleForEach(x => x.DepartmentAssignments).SetValidator(new UpdateDepartmentAssignmentRequestValidator());
    }
}

public class UpdateDepartmentAssignmentRequestValidator : AbstractValidator<UpdateDepartmentAssignmentRequest>
{
    public UpdateDepartmentAssignmentRequestValidator()
    {
        RuleFor(x => x.DepartmentName).MustBeValidDepartmentName();
        RuleFor(x => x.NumberOfSeats).MustBeValidNumberOfSeats();
        RuleFor(x => x.ReservedRatioForGirls).MustBeValidReservedRatioForGirls();
    }
}

public class UpdateCampaignHandler(IJsonFileRepository repository) : IRequestHandler<UpdateCampaign, Result<UpdateCampaignResponse>>
{
    public async Task<Result<UpdateCampaignResponse>> Handle(UpdateCampaign updateCampaign, CancellationToken cancellationToken)
    {
        var campaignId = updateCampaign.CampaignId;
        var request = updateCampaign.Request;

        await using var campaignStream = await repository.Open(campaignId.ToString("N"), true);
        if (campaignStream is null)
        {
            return Result.Fail(new NotFound($"Campaign with ID {campaignId} not found"));
        }

        var campaign = await repository.Get<Campaign>(campaignStream);
        if (campaign is null) { return Result.Fail(new NotFound($"Could not read campaign with id {campaignId} from repository")); }

        // Optimistic concurrency check
        if (campaign.UpdatedAt != request.UpdatedAt)
        {
            return Result.Fail(new Concurrency("Optimistic concurrency control failed, campaign has been updated by another user"));
        }

        // Check if trying to activate an inactive campaign
        if (campaign.Status == CampaignStatus.Inactive && request.Status == CampaignStatus.Active)
        {
            return Result.Fail(new Forbidden("Campaigns cannot be activated using this endpoint. Use the ActivateCampaign endpoint instead."));
        }

        var updateResult = UpdateCampaign(request, campaign);
        if (updateResult.IsFailed) { return updateResult; }

        await repository.Update(campaignStream, campaign);

        return Result.Ok(new UpdateCampaignResponse(campaign));
    }

    internal static Result UpdateCampaign(UpdateCampaignRequest request, Campaign campaign)
    {
        campaign.Name = request.Name;
        campaign.Organizer = request.Organizer;
        campaign.ReservedRatioForGirls = request.ReservedRatioForGirls;
        campaign.PurgeDate = request.PurgeDate;
        campaign.Status = request.Status;

        // Update dates
        var updateResult = MergeCampaignDates(request.Dates, campaign.Dates);
        if (updateResult.IsFailed) { return updateResult; }

        campaign.UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Ok();
    }

    internal static Result MergeCampaignDates(IEnumerable<UpdateDateRequest>? dto, List<CampaignDate> dates)
    {
        var deletedDates = dates.Where(date => !(dto?.Any(d => d.Date == date.Date) ?? false)).ToArray();
        foreach (var date in deletedDates)
        {
            if (date.DepartmentAssignments.Any(da => da.Registrations.Count != 0))
            {
                return Result.Fail(new BadRequest("Cannot delete date with registrations"));
            }

            dates.Remove(date);
        }

        foreach (var dtoDate in dto ?? [])
        {
            var existingDate = dates.FirstOrDefault(d => d.Date == dtoDate.Date);
            if (existingDate is null)
            {
                dates.Add(new CampaignDate
                {
                    Date = dtoDate.Date,
                    StartTime = dtoDate.StartTime,
                    EndTime = dtoDate.EndTime,
                    Status = dtoDate.Status,
                    DepartmentAssignments = [.. dtoDate.DepartmentAssignments?.Select(a => new DepartmentAssignment
                    {
                        DepartmentName = a.DepartmentName,
                        NumberOfSeats = a.NumberOfSeats,
                        ReservedRatioForGirls = a.ReservedRatioForGirls,
                        Registrations = []
                    }) ?? []]
                });
            }
            else
            {
                var result = UpdateCampaignDate(dtoDate, existingDate);
                if (result is not null) { return result; }
            }
        }

        return Result.Ok();
    }

    internal static Result UpdateCampaignDate(UpdateDateRequest dto, CampaignDate date)
    {
        date.StartTime = dto.StartTime;
        date.EndTime = dto.EndTime;
        date.Status = dto.Status;
        var mergeResult = MergeDepartmentAssignments(dto.DepartmentAssignments, date.DepartmentAssignments);
        if (mergeResult.IsFailed) { return mergeResult; }
        return Result.Ok();
    }

    internal static Result MergeDepartmentAssignments(IEnumerable<UpdateDepartmentAssignmentRequest>? dto, List<DepartmentAssignment> assignments)
    {
        var deletedAssignments = assignments.Where(a => !(dto?.Any(b => b.DepartmentName == a.DepartmentName) ?? false)).ToArray();
        foreach (var deletedAssignment in deletedAssignments)
        {
            if (deletedAssignment.Registrations.Count != 0)
            {
                return Result.Fail(new BadRequest("Cannot delete department with registrations"));
            }

            assignments.Remove(deletedAssignment);
        }

        foreach (var assignment in dto ?? [])
        {
            var existingAssignment = assignments.FirstOrDefault(a => a.DepartmentName == assignment.DepartmentName);
            if (existingAssignment is null)
            {
                assignments.Add(new DepartmentAssignment
                {
                    DepartmentName = assignment.DepartmentName,
                    NumberOfSeats = assignment.NumberOfSeats,
                    ReservedRatioForGirls = assignment.ReservedRatioForGirls,
                    Registrations = []
                });
            }
            else
            {
                UpdateDepartmentAssignment(assignment, existingAssignment);
            }
        }

        return Result.Ok();
    }

    internal static void UpdateDepartmentAssignment(UpdateDepartmentAssignmentRequest dto, DepartmentAssignment assignment)
    {
        assignment.DepartmentName = dto.DepartmentName;
        assignment.NumberOfSeats = dto.NumberOfSeats;
        assignment.ReservedRatioForGirls = dto.ReservedRatioForGirls;
    }
}
