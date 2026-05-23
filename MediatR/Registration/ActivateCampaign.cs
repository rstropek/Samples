using DataAccess;
using FluentResults;
using FluentValidation;
using MediatR;

namespace Registration;

public record ActivateCampaign(Guid CampaignId) : IRequest<Result<Campaign>>;

public class ActivateCampaignValidator : AbstractValidator<ActivateCampaign>
{
    public ActivateCampaignValidator()
    {
        RuleFor(x => x.CampaignId).MustBeValidCampaignId();
    }
}

public class ActivateCampaignHandler(IJsonFileRepository repository) : IRequestHandler<ActivateCampaign, Result<Campaign>>
{
    public async Task<Result<Campaign>> Handle(ActivateCampaign activateCampaign, CancellationToken cancellationToken)
    {
        var campaignId = activateCampaign.CampaignId;

        await using var campaignStream = await repository.Open(campaignId.ToString("N"), true);
        if (campaignStream is null)
        {
            return Result.Fail(new NotFound($"Campaign with ID {campaignId} not found"));
        }

        var campaign = await repository.Get<Campaign>(campaignStream);
        if (campaign is null) { return Result.Fail(new NotFound($"Could not read campaign with id {campaignId} from repository")); }

        if (campaign.Status == CampaignStatus.Active) { return Result.Ok(campaign); }

        if (!campaign.Dates.Any(date => date.Status == CampaignDateStatus.Active))
        {
            return Result.Fail(new BadRequest("Campaign has no active dates"));
        }

        campaign.Status = CampaignStatus.Active;
        campaign.UpdatedAt = DateTime.UtcNow;

        await repository.Update(campaignStream, campaign);

        return Result.Ok(campaign);
    }
}
