using DataAccess;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Registration;

public record GetCampaigns() : IRequest<Result<IEnumerable<GetCampaignsResponse>>>;

public record GetCampaignsResponse(
    Guid CampaignId,
    string Name,
    DateOnly Date,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string DepartmentNames
);

public class GetCampaignsHandler(IJsonFileRepository repository) : IRequestHandler<GetCampaigns, Result<IEnumerable<GetCampaignsResponse>>>
{
    public async Task<Result<IEnumerable<GetCampaignsResponse>>> Handle(GetCampaigns _, CancellationToken cancellationToken)
    {
        var campaigns = new List<GetCampaignsResponse>();
        foreach (var c in repository.EnumerateAll())
        {
            await using var campaignStream = await repository.Open(c.Id, false);
            if (campaignStream is null) { continue; }

            var campaign = await repository.Get<Campaign>(campaignStream);
            if (campaign is null) { continue; }

            var activeDates = campaign.Dates
                .Where(d => d.Date >= DateOnly.FromDateTime(DateTime.UtcNow))
                .Where(d => d.Status == CampaignDateStatus.Active);
            if (!activeDates.Any()) { continue; }

            campaigns.AddRange(activeDates.Select(d =>
            {
                var departmentNames = string.Join(", ", d.DepartmentAssignments.Select(a => a.DepartmentName));
                return new GetCampaignsResponse(
                    campaign.Id,
                    campaign.Name,
                    d.Date,
                    d.StartTime,
                    d.EndTime,
                    departmentNames
                );
            }));
        }

        return Result.Ok(campaigns.AsEnumerable());
    }
}
